﻿// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using JsonApiFramework.Reflection;

namespace JsonApiFramework.Converters
{
    /// <summary>
    /// Type converter that converts from one type to another type.
    /// </summary>
    /// <notes>
    /// One major design goal is to minimize boxing/unboxing. When there is a
    /// legal cast between the types, boxing/unboxing is eliminated by using
    /// dynamically built (cached) casting lambdas.
    /// 
    /// Also any reflection should only happen once and the result of the
    /// reflection cached for future use to avoid repetitive reflection.
    /// </notes>
    public class TypeConverter : ITypeConverter
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public TypeConverter()
        {
            this.AddDefaultDefinitions();
        }
        #endregion

        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Convert Methods
        public TTarget Convert<TSource, TTarget>(TSource source, TypeConverterContext context)
        {
            TTarget target;

            // Try convert with a type converter definition if possible.
            if (this.TryConvertByDefinitionStrict(source, context, out target))
                return target;

            // Try programmatic conversion for special cases if possible.
            // 1. Enumerations (source or target)
            if (this.TryConvertByEnumerationStrict(source, context, out target))
                return target;

            // Try direct cast.
            if (TryConvertByCastStrict(source, context, out target))
                return target;

            // Unable to convert between source and target types.
            var typeConverterException = TypeConverterException.Create<TSource, TTarget>(source);
            throw typeConverterException;
        }
        #endregion

        // PRIVATE PROPERTIES ///////////////////////////////////////////////
        #region Properties
        private IDictionary<Tuple<Type, Type>, ITypeConverterDefinition> TypeConverterDefinitions { get; set; }
        #endregion

        // PRIVATE METHODS //////////////////////////////////////////////////
        #region Convert Implementation Methods
        private static TTarget Cast<TSource, TTarget>(TSource source)
        {
            return CastTo<TTarget>.From(source);
        }

        private TTarget ConvertToEnum<TSource, TTarget>(TSource source, TypeConverterContext context)
        {
            // Handle when source type can be converted to an integer.
            int sourceAsInt;
            if (this.TryConvert(source, context, out sourceAsInt))
            {
                // Casting from an integer to an enumeration always works.
                return CastTo<TTarget>.From(sourceAsInt);
            }

            // Handle when source type is a string.
            if (typeof(TSource).IsString())
            {
                var sourceAsString = CastTo<string>.From(source);

                TTarget target;
                if (EnumTryParse<TTarget>.From(sourceAsString, out target))
                    return target;
            }

            // Unable to convert source type to an enumeration.
            var typeConverterException = TypeConverterException.Create<TSource, TTarget>(source);
            throw typeConverterException;
        }

        private TTarget ConvertToNullableEnum<TSource, TTarget>(TSource source, TypeConverterContext context)
        {
            // Handle when source type is a string and null or empty.
            var isSourceString = typeof(TSource).IsString();
            var sourceAsString = isSourceString ? CastTo<string>.From(source) : null;
            if (isSourceString && String.IsNullOrWhiteSpace(sourceAsString))
            {
                return New<TTarget>.WithZeroArguments();
            }

            // Handle when source type can be converted to an integer.
            int sourceAsInt;
            if (this.TryConvert(source, context, out sourceAsInt))
            {
                // Casting from an integer to an enumeration always works.
                return CastTo<TTarget>.From(sourceAsInt);
            }

            // Handle when source type is a string.
            if (isSourceString)
            {
                TTarget target;
                if (TypeConverterNullableEnumTryParse<TTarget>.From(sourceAsString, out target))
                    return target;
            }

            // Unable to convert source type to an enumeration.
            var typeConverterException = TypeConverterException.Create<TSource, TTarget>(source);
            throw typeConverterException;
        }

        /// <summary>
        /// Try and convert by type converter definition if a definition
        /// exists for the source and target types.
        /// </summary>
        /// <remarks>
        /// Strict means if type converter definition exists for the source
        /// and target types, perform conversion and let any exception be thrown.
        /// </remarks>
        private bool TryConvertByDefinitionStrict<TSource, TTarget>(TSource source, TypeConverterContext context, out TTarget target)
        {
            ITypeConverterDefinition<TSource, TTarget> definition;
            if (!this.TryGetTypeConverterDefinition(out definition))
            {
                target = default(TTarget);
                return false;
            }

            try
            {
                target = definition.Convert(source, context);
                return true;
            }
            catch (Exception exception)
            {
                var typeConverterException = TypeConverterException.Create<TSource, TTarget>(source, exception);
                throw typeConverterException;
            }
        }

        /// <summary>
        /// Try and convert by type casting from source to target types.
        /// </summary>
        /// <remarks>
        /// Strict means perform cast and let any exception be thrown.
        /// </remarks>
        private static bool TryConvertByCastStrict<TSource, TTarget>(TSource source, TypeConverterContext context, out TTarget target)
        {
            try
            {
                target = Cast<TSource, TTarget>(source);
                return true;
            }
            catch (Exception exception)
            {
                var typeConverterException = TypeConverterException.Create<TSource, TTarget>(source, exception);
                throw typeConverterException;
            }
        }

        /// <summary>
        /// Try and convert by enumeration for source to target types if applicable.
        /// </summary>
        /// <remarks>
        /// Strict means if source or target types are enumerations, then perform conversion
        /// and let any exception be thrown.
        /// </remarks>
        private bool TryConvertByEnumerationStrict<TSource, TTarget>(TSource source, TypeConverterContext context, out TTarget target)
        {
            var targetType = typeof(TTarget);

            var isTargetTypeEnum = targetType.IsEnum();
            if (isTargetTypeEnum)
            {
                target = ConvertToEnum<TSource, TTarget>(source, context);
                return true;
            }

            var isTargetTypeNullable = targetType.IsNullableType();
            if (isTargetTypeNullable)
            {
                var isTargetTypeNullableUnderlyingTypeEnum = Nullable.GetUnderlyingType(targetType).IsEnum();
                if (isTargetTypeNullableUnderlyingTypeEnum)
                {
                    target = ConvertToNullableEnum<TSource, TTarget>(source, context);
                    return true;
                }
            }

            var sourceType = typeof(TSource);

            var isSourceTypeEnum = sourceType.IsEnum();
            if (isSourceTypeEnum)
            {
                if (targetType.IsString())
                {
                    var targetAsString = EnumToString<TSource>.From(source, context);
                    target = Cast<string, TTarget>(targetAsString);
                    return true;
                }

                if (targetType == typeof(bool) ||
                    targetType == typeof(decimal) ||
                    targetType == typeof(bool?) ||
                    targetType == typeof(decimal?))
                {
                    var sourceAsInteger = Cast<TSource, int>(source);
                    target = this.Convert<int, TTarget>(sourceAsInteger);
                    return true;
                }
            }

            target = default(TTarget);
            return false;
        }
        #endregion

        #region TypeConverterDefinition Methods
        private void AddDefaultDefinitions()
        {
            foreach (var definition in DefaultDefinitions)
            {
                this.AddTypeConverterDefinition(definition);
            }
        }

        private void AddTypeConverterDefinition(ITypeConverterDefinition definition)
        {
            Contract.Requires(definition != null);

            this.TypeConverterDefinitions = this.TypeConverterDefinitions ?? new Dictionary<Tuple<Type, Type>, ITypeConverterDefinition>();

            var key = CreateTypeConverterDefinitionKey(definition);
            this.TypeConverterDefinitions.Add(key, definition);
        }

        private static Tuple<Type, Type> CreateTypeConverterDefinitionKey(ITypeConverterDefinition definition)
        {
            Contract.Requires(definition != null);

            var sourceType = definition.SourceType;
            var targetType = definition.TargetType;

            var key = CreateTypeConverterDefinitionKey(sourceType, targetType);
            return key;
        }

        private static Tuple<Type, Type> CreateTypeConverterDefinitionKey(Type sourceType, Type targetType)
        {
            Contract.Requires(sourceType != null);
            Contract.Requires(targetType != null);

            var key = new Tuple<Type, Type>(sourceType, targetType);
            return key;
        }

        private bool TryGetTypeConverterDefinition<TSource, TTarget>(out ITypeConverterDefinition<TSource, TTarget> definition)
        {
            definition = null;

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var key = CreateTypeConverterDefinitionKey(sourceType, targetType);
            ITypeConverterDefinition value;
            if (!this.TypeConverterDefinitions.TryGetValue(key, out value))
                return false;

            definition = (ITypeConverterDefinition<TSource, TTarget>)value;
            return true;
        }
        #endregion

        #region TypeConverterDefinitionFunc Implementation Methods
        private static string ConvertDateTimeToString(DateTime dateTime, TypeConverterContext context)
        {
            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? dateTime.ToString("O")
                : dateTime.ToString(context.SafeGetFormat(), context.SafeGetFormatProvider());
        }

        private static string ConvertDateTimeOffsetToString(DateTimeOffset dateTimeOffset, TypeConverterContext context)
        {
            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? dateTimeOffset.ToString("O")
                : dateTimeOffset.ToString(context.SafeGetFormat(), context.SafeGetFormatProvider());
        }

        private static string ConvertGuidToString(Guid guid, TypeConverterContext context)
        {
            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? guid.ToString("D")
                : guid.ToString(context.SafeGetFormat());
        }

        private static DateTime? ConvertStringToNullableDateTime(string str, TypeConverterContext context)
        {
            if (String.IsNullOrWhiteSpace(str))
                return new DateTime?();

            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? DateTime.Parse(str, context.SafeGetFormatProvider(), context.SafeGetDateTimeStyles())
                : DateTime.ParseExact(str, context.SafeGetFormat(), context.SafeGetFormatProvider(), context.SafeGetDateTimeStyles());
        }

        private static DateTimeOffset? ConvertStringToNullableDateTimeOffset(string str, TypeConverterContext context)
        {
            if (String.IsNullOrWhiteSpace(str))
                return new DateTimeOffset?();

            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? DateTimeOffset.Parse(str, context.SafeGetFormatProvider(), context.SafeGetDateTimeStyles())
                : DateTimeOffset.ParseExact(str, context.SafeGetFormat(), context.SafeGetFormatProvider(), context.SafeGetDateTimeStyles());
        }

        private static Guid? ConvertStringToNullableGuid(string str, TypeConverterContext context)
        {
            if (String.IsNullOrWhiteSpace(str))
                return new Guid?();

            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? Guid.Parse(str)
                : Guid.ParseExact(str, context.SafeGetFormat());
        }

        private static TimeSpan? ConvertStringToNullableTimeSpan(string str, TypeConverterContext context)
        {
            if (String.IsNullOrWhiteSpace(str))
                return new TimeSpan?();

            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? TimeSpan.Parse(str, context.SafeGetFormatProvider())
                : TimeSpan.ParseExact(str, context.SafeGetFormat(), context.SafeGetFormatProvider());
        }

        private static string ConvertTimeSpanToString(TimeSpan timeSpan, TypeConverterContext context)
        {
            return String.IsNullOrWhiteSpace(context.SafeGetFormat())
                ? timeSpan.ToString("c")
                : timeSpan.ToString(context.SafeGetFormat(), context.SafeGetFormatProvider());
        }
        #endregion

        #region Enumeration Methods
        private static bool NullableEnumTryParse<T>(string value, bool ignoreCase, out T? result)
            where T : struct
        {
            T enumeration;
            if (Enum.TryParse(value, ignoreCase, out enumeration))
            {
                result = enumeration;
                return true;
            }

            result = null;
            return false;
        }
        #endregion

        // PRIVATE FIELDS ///////////////////////////////////////////////////
        #region Fields
        private static readonly ITypeConverterDefinition[] DefaultDefinitions =
            {
                new TypeConverterDefinitionFunc<bool, decimal>((s, c) => System.Convert.ToDecimal(s)),
                new TypeConverterDefinitionFunc<bool, decimal?>((s, c) => System.Convert.ToDecimal(s)),
                new TypeConverterDefinitionFunc<bool, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<byte, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<byte, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<byte, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<byte[], Guid>((s, c) => new Guid(s)),
                new TypeConverterDefinitionFunc<byte[], Guid?>((s, c) => s != null ? new Guid(s) : new Guid?()),
                new TypeConverterDefinitionFunc<byte[], string>((s, c) => s != null ? System.Convert.ToBase64String(s) : null),
                new TypeConverterDefinitionFunc<char, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<char, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<char, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<DateTime, string>(ConvertDateTimeToString),
                new TypeConverterDefinitionFunc<DateTimeOffset, DateTime>((s, c) => s.DateTime),
                new TypeConverterDefinitionFunc<DateTimeOffset, DateTime?>((s, c) => s.DateTime),
                new TypeConverterDefinitionFunc<DateTimeOffset, string>(ConvertDateTimeOffsetToString),
                new TypeConverterDefinitionFunc<decimal, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<decimal, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<decimal, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<double, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<double, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<double, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<float, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<float, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<float, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<Guid, byte[]>((s, c) => s.ToByteArray()),
                new TypeConverterDefinitionFunc<Guid, string>(ConvertGuidToString),
                new TypeConverterDefinitionFunc<int, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<int, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<int, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<long, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<long, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<long, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<sbyte, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<sbyte, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<sbyte, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<short, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<short, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<short, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, bool>((s, c) => System.Convert.ToBoolean(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, bool?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToBoolean(s, c.SafeGetFormatProvider()) : new bool?()),
                new TypeConverterDefinitionFunc<string, byte>((s, c) => System.Convert.ToByte(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, byte?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToByte(s, c.SafeGetFormatProvider()) : new byte?()),
                new TypeConverterDefinitionFunc<string, byte[]>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.FromBase64String(s) : null),
                new TypeConverterDefinitionFunc<string, char>((s, c) => System.Convert.ToChar(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, char?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToChar(s, c.SafeGetFormatProvider()) : new char?()),
                new TypeConverterDefinitionFunc<string, DateTime>((s, c) => ConvertStringToNullableDateTime(s, c).GetValueOrDefault()),
                new TypeConverterDefinitionFunc<string, DateTime?>(ConvertStringToNullableDateTime),
                new TypeConverterDefinitionFunc<string, DateTimeOffset>((s, c) => ConvertStringToNullableDateTimeOffset(s, c).GetValueOrDefault()),
                new TypeConverterDefinitionFunc<string, DateTimeOffset?>(ConvertStringToNullableDateTimeOffset),
                new TypeConverterDefinitionFunc<string, decimal>((s, c) => System.Convert.ToDecimal(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, decimal?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToDecimal(s, c.SafeGetFormatProvider()) : new decimal?()),
                new TypeConverterDefinitionFunc<string, double>((s, c) => System.Convert.ToDouble(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, double?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToDouble(s, c.SafeGetFormatProvider()) : new double?()),
                new TypeConverterDefinitionFunc<string, float>((s, c) => System.Convert.ToSingle(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, float?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToSingle(s, c.SafeGetFormatProvider()) : new float?()),
                new TypeConverterDefinitionFunc<string, Guid>((s, c) => ConvertStringToNullableGuid(s, c).GetValueOrDefault()),
                new TypeConverterDefinitionFunc<string, Guid?>(ConvertStringToNullableGuid),
                new TypeConverterDefinitionFunc<string, int>((s, c) => System.Convert.ToInt32(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, int?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToInt32(s, c.SafeGetFormatProvider()) : new int?()),
                new TypeConverterDefinitionFunc<string, long>((s, c) => System.Convert.ToInt64(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, long?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToInt64(s, c.SafeGetFormatProvider()) : new long?()),
                new TypeConverterDefinitionFunc<string, sbyte>((s, c) => System.Convert.ToSByte(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, sbyte?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToSByte(s, c.SafeGetFormatProvider()) : new sbyte?()),
                new TypeConverterDefinitionFunc<string, short>((s, c) => System.Convert.ToInt16(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, short?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToInt16(s, c.SafeGetFormatProvider()) : new short?()),
                new TypeConverterDefinitionFunc<string, TimeSpan>((s, c) => ConvertStringToNullableTimeSpan(s, c).GetValueOrDefault()),
                new TypeConverterDefinitionFunc<string, TimeSpan?>(ConvertStringToNullableTimeSpan),
                new TypeConverterDefinitionFunc<string, Type>((s, c) => Type.GetType(s, true)),
                new TypeConverterDefinitionFunc<string, uint>((s, c) => System.Convert.ToUInt32(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, uint?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToUInt32(s, c.SafeGetFormatProvider()) : new uint?()),
                new TypeConverterDefinitionFunc<string, ulong>((s, c) => System.Convert.ToUInt64(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, ulong?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToUInt64(s, c.SafeGetFormatProvider()) : new ulong?()),
                new TypeConverterDefinitionFunc<string, Uri>((s, c) => !String.IsNullOrWhiteSpace(s) ? new Uri(s, UriKind.RelativeOrAbsolute) : null),
                new TypeConverterDefinitionFunc<string, ushort>((s, c) => System.Convert.ToUInt16(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, ushort?>((s, c) => !String.IsNullOrWhiteSpace(s) ? System.Convert.ToUInt16(s, c.SafeGetFormatProvider()) : new ushort?()),
                new TypeConverterDefinitionFunc<TimeSpan, string>(ConvertTimeSpanToString),
                new TypeConverterDefinitionFunc<Type, string>((s, c) => s != null ? s.GetCompactQualifiedName() : null),
                new TypeConverterDefinitionFunc<uint, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<uint, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<uint, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<ulong, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<ulong, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<ulong, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<ushort, bool>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<ushort, bool?>((s, c) => System.Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<ushort, string>((s, c) => System.Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<Uri, string>((s, c) => s != null ? s.ToString() : null),
            };
        #endregion

        // PRIVATE TYPES ////////////////////////////////////////////////////
        #region Types
        /// <summary>
        /// Returns dynamically built lambdas (cached) to cast between types.
        /// </summary>
        /// <notes>
        /// Avoids unnecessary boxing/unboxing for value types.
        /// </notes>
        private static class CastTo<TTarget>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            public static TTarget From<TSource>(TSource source)
            {
                return Cache<TSource>.CastImpl(source);
            }
            #endregion

            // PRIVATE TYPES ////////////////////////////////////////////////////
            #region Types
            private static class Cache<TSource>
            {
                #region Public Properties
                public static Func<TSource, TTarget> CastImpl { get { return _castImpl.Value; } }
                #endregion

                #region Private Methods
                private static Func<TSource, TTarget> CreateCastImpl()
                {
                    var parameterExpression = Expression.Parameter(typeof(TSource));
                    var convertExpression = Expression.Convert(parameterExpression, typeof(TTarget));
                    var convertLambda = Expression
                        .Lambda<Func<TSource, TTarget>>(convertExpression, parameterExpression)
                        .Compile();
                    return convertLambda;
                }
                #endregion

                #region Private Fields
                // ReSharper disable InconsistentNaming
                private static readonly Lazy<Func<TSource, TTarget>> _castImpl = new Lazy<Func<TSource, TTarget>>(CreateCastImpl, LazyThreadSafetyMode.PublicationOnly);
                // ReSharper restore InconsistentNaming
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// Returns dynamically built lambdas (cached) to call the ToString
        /// instance method on the source object when TSource is an enumeration.
        /// </summary>
        private static class EnumToString<TSource>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            public static string From(TSource source, TypeConverterContext context)
            {
                var format = context.SafeGetFormat();
                return String.IsNullOrWhiteSpace(format)
                    ? Cache.ToStringImpl(source)
                    : Cache.ToStringWithFormatImpl(source, format);
            }
            #endregion

            // PRIVATE TYPES ////////////////////////////////////////////////////
            #region Types
            private static class Cache
            {
                #region Public Properties
                // ReSharper disable StaticFieldInGenericType
                public static Func<TSource, string> ToStringImpl { get { return _toStringImpl.Value; } }
                public static Func<TSource, string, string> ToStringWithFormatImpl { get { return _toStringWithFormatImpl.Value; } }
                // ReSharper restore StaticFieldInGenericType
                #endregion

                #region Private Methods
                private static Func<TSource, string> CreateToStringImpl()
                {
                    var enumType = typeof(Enum);
                    var toStringMethod = enumType
                        .GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

                    var sourceType = typeof(TSource);
                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var callExpression = Expression.Call(sourceExpression, toStringMethod);
                    var callLambdaExpression = Expression.Lambda<Func<TSource, string>>(callExpression, sourceExpression);
                    var callLambda = callLambdaExpression.Compile();
                    return callLambda;
                }

                private static Func<TSource, string, string> CreateToStringWithFormatImpl()
                {
                    var enumType = typeof(Enum);
                    var toStringWithFormatMethod = enumType
                        .GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, typeof(string));

                    var sourceType = typeof(TSource);
                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var formatExpression = Expression.Parameter(typeof(string), "format");
                    // ReSharper disable PossiblyMistakenUseOfParamsMethod
                    var callExpression = Expression.Call(sourceExpression, toStringWithFormatMethod, formatExpression);
                    // ReSharper restore PossiblyMistakenUseOfParamsMethod
                    var callLambdaExpression = Expression.Lambda<Func<TSource, string, string>>(callExpression, sourceExpression, formatExpression);
                    var callLambda = callLambdaExpression.Compile();
                    return callLambda;
                }
                #endregion

                #region Private Fields
                // ReSharper disable InconsistentNaming
                // ReSharper disable StaticFieldInGenericType
                private static readonly Lazy<Func<TSource, string>> _toStringImpl = new Lazy<Func<TSource, string>>(CreateToStringImpl, LazyThreadSafetyMode.PublicationOnly);
                private static readonly Lazy<Func<TSource, string, string>> _toStringWithFormatImpl = new Lazy<Func<TSource, string, string>>(CreateToStringWithFormatImpl, LazyThreadSafetyMode.PublicationOnly);
                // ReSharper restore StaticFieldInGenericType
                // ReSharper restore InconsistentNaming
                #endregion
            }
            #endregion
        }

        private delegate bool EnumTryParseDelegate<TTarget>(string source, out TTarget target);

        /// <summary>
        /// Returns dynamically built lambdas (cached) to call Enum.TryParse static method.
        /// </summary>
        private static class EnumTryParse<TTarget>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            public static bool From(string source, out TTarget target)
            {
                try
                {
                    return Cache.EnumTryParseImpl(source, out target);
                }
                catch (Exception)
                {
                    target = default(TTarget);
                    return false;
                }
            }
            #endregion

            // PRIVATE TYPES ////////////////////////////////////////////////////
            #region Types
            private static class Cache
            {
                #region Public Properties
                public static EnumTryParseDelegate<TTarget> EnumTryParseImpl { get { return _enumTryParseImpl.Value; } }
                #endregion

                #region Private Methods
                private static EnumTryParseDelegate<TTarget> CreateEnumTryParseImpl()
                {
                    // public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct;
                    var enumType = typeof(Enum);
                    var tryParseMethodInfoOpen = enumType.GetMethods().Single(x => x.Name == "TryParse" && x.GetParameters().Count() == 3);
                    var tryParseMethodInfoClosed = tryParseMethodInfoOpen.MakeGenericMethod(typeof(TTarget));

                    var arguments = tryParseMethodInfoClosed.GetParameters();
                    var sourceType = arguments[0].ParameterType;
                    var resultType = arguments[2].ParameterType;

                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var ignoreCaseExpression = Expression.Constant(true, typeof(bool));
                    var resultExpression = Expression.Parameter(resultType, "result");
                    var callExpression = Expression.Call(tryParseMethodInfoClosed, sourceExpression, ignoreCaseExpression, resultExpression);
                    var callDelegateExpression = Expression.Lambda<EnumTryParseDelegate<TTarget>>(callExpression, sourceExpression, resultExpression);
                    var callDelegate = callDelegateExpression.Compile();

                    return callDelegate;
                }
                #endregion

                #region Private Fields
                // ReSharper disable InconsistentNaming
                // ReSharper disable StaticFieldInGenericType
                private static readonly Lazy<EnumTryParseDelegate<TTarget>> _enumTryParseImpl = new Lazy<EnumTryParseDelegate<TTarget>>(CreateEnumTryParseImpl, LazyThreadSafetyMode.PublicationOnly);
                // ReSharper restore StaticFieldInGenericType
                // ReSharper restore InconsistentNaming
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// Returns dynamically built lambdas (cached) to call TypeConverter.NullableEnumTryParse
        /// static method.
        /// </summary>
        private static class TypeConverterNullableEnumTryParse<TTarget>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            public static bool From(string source, out TTarget target)
            {
                try
                {
                    return Cache.NullableEnumTryParseImpl(source, out target);
                }
                catch (Exception)
                {
                    target = default(TTarget);
                    return false;
                }
            }
            #endregion

            // PRIVATE TYPES ////////////////////////////////////////////////////
            #region Types
            private static class Cache
            {
                #region Public Properties
                public static EnumTryParseDelegate<TTarget> NullableEnumTryParseImpl { get { return _nullableEnumTryParseImpl.Value; } }
                #endregion

                #region Private Methods
                private static EnumTryParseDelegate<TTarget> CreateNullableEnumTryParseImpl()
                {
                    // public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct;
                    var typeConverterType = typeof(TypeConverter);
                    var tryParseMethodInfoOpen = typeConverterType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x => x.Name == "NullableEnumTryParse");

                    var targetType = typeof(TTarget);
                    var targetUnderlyingType = Nullable.GetUnderlyingType(targetType);
                    var tryParseMethodInfoClosed = tryParseMethodInfoOpen.MakeGenericMethod(targetUnderlyingType);

                    var arguments = tryParseMethodInfoClosed.GetParameters();
                    var sourceType = arguments[0].ParameterType;
                    var resultType = arguments[2].ParameterType;

                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var ignoreCaseExpression = Expression.Constant(true, typeof(bool));
                    var resultExpression = Expression.Parameter(resultType, "result");
                    var callExpression = Expression.Call(tryParseMethodInfoClosed, sourceExpression, ignoreCaseExpression, resultExpression);
                    var callDelegateExpression = Expression.Lambda<EnumTryParseDelegate<TTarget>>(callExpression, sourceExpression, resultExpression);
                    var callDelegate = callDelegateExpression.Compile();

                    return callDelegate;
                }
                #endregion

                #region Private Fields
                // ReSharper disable InconsistentNaming
                // ReSharper disable StaticFieldInGenericType
                private static readonly Lazy<EnumTryParseDelegate<TTarget>> _nullableEnumTryParseImpl = new Lazy<EnumTryParseDelegate<TTarget>>(CreateNullableEnumTryParseImpl, LazyThreadSafetyMode.PublicationOnly);
                // ReSharper restore StaticFieldInGenericType
                // ReSharper restore InconsistentNaming
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// Returns dynamically built lambdas (cached) to call new TTarget()
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        private static class New<TTarget>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            public static TTarget WithZeroArguments()
            { return Cache.NewWithZeroArgumentsImpl(); }
            #endregion

            // PRIVATE TYPES ////////////////////////////////////////////////////
            #region Types
            private static class Cache
            {
                #region Public Properties
                public static Func<TTarget> NewWithZeroArgumentsImpl { get { return _newWithZeroArgumentsImpl.Value; } }
                #endregion

                #region Private Methods
                private static Func<TTarget> CreateNewWithZeroArgumentsImpl()
                {
                    var targetType = typeof(TTarget);
                    var newExpression = Expression.New(targetType);
                    var newLambdaExpression = Expression.Lambda<Func<TTarget>>(newExpression);
                    var newLambda = newLambdaExpression.Compile();
                    return newLambda;
                }
                #endregion

                #region Private Fields
                // ReSharper disable InconsistentNaming
                // ReSharper disable StaticFieldInGenericType
                private static readonly Lazy<Func<TTarget>> _newWithZeroArgumentsImpl = new Lazy<Func<TTarget>>(CreateNewWithZeroArgumentsImpl, LazyThreadSafetyMode.PublicationOnly);
                // ReSharper restore StaticFieldInGenericType
                // ReSharper restore InconsistentNaming
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
