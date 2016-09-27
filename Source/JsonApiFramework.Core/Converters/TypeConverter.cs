// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
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
            if (TryConvertByCastStrict(source, out target))
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
        private TTarget ConvertToEnum<TSource, TTarget>(TSource source, TypeConverterContext context)
        {
            // Handle when source type can be converted to an integer.
            int sourceAsInt;
            if (this.TryConvert(source, context, out sourceAsInt))
            {
                // Casting from an integer to an enumeration always works.
                var target = Functions.Cast<int, TTarget>(sourceAsInt);
                return target;
            }

            // Handle when source type is a string.
            if (typeof(TSource).IsString())
            {
                var sourceAsString = Functions.Cast<TSource, string>(source);

                TTarget target;
                if (Functions.EnumTryParse(sourceAsString, out target))
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
            var sourceAsString = isSourceString ? Functions.Cast<TSource, string>(source) : null;
            if (isSourceString && String.IsNullOrWhiteSpace(sourceAsString))
            {
                return Functions.NewWithZeroArguments<TTarget>();
            }

            // Handle when source type can be converted to an integer.
            int sourceAsInt;
            if (this.TryConvert(source, context, out sourceAsInt))
            {
                // Casting from an integer to an enumeration always works.
                var target = Functions.Cast<int, TTarget>(sourceAsInt);
                return target;
            }

            // Handle when source type is a string.
            if (isSourceString)
            {
                TTarget target;
                if (Functions.NullableEnumTryParse(sourceAsString, out target))
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
        private static bool TryConvertByCastStrict<TSource, TTarget>(TSource source, out TTarget target)
        {
            try
            {
                target = Functions.Cast<TSource, TTarget>(source);
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
                    var targetAsString = Functions.EnumToString(source, context);
                    target = Functions.Cast<string, TTarget>(targetAsString);
                    return true;
                }

                if (targetType == typeof(bool) ||
                    targetType == typeof(decimal) ||
                    targetType == typeof(bool?) ||
                    targetType == typeof(decimal?))
                {
                    var sourceAsInteger = Functions.Cast<TSource, int>(source);
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

        private static bool ConvertNullableByteToBool(byte? source, TypeConverterContext context)
        {
            if (source.HasValue)
                return System.Convert.ToBoolean(source.Value);

            var typeConverterException = TypeConverterException.Create<byte?, bool>(null);
            throw typeConverterException;
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
        // ReSharper disable UnusedMember.Local
        private static bool NullableEnumTryParse<T>(string value, bool ignoreCase, out T? result)
        // ReSharper restore UnusedMember.Local
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
                new TypeConverterDefinitionFunc<byte?, bool>(ConvertNullableByteToBool),
                new TypeConverterDefinitionFunc<byte?, string>((s, c) => s.HasValue ? System.Convert.ToString(s.Value) : null),
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
                new TypeConverterDefinitionFunc<Uri, string>((s, c) => s != null ? s.ToString() : null)
            };
        #endregion

        // PRIVATE TYPES ////////////////////////////////////////////////////
        #region Types
        /// <summary>
        /// Delegate to match the signature of many "TryParse" like functions
        /// to build dynamic lambdas from.
        /// </summary>
        /// <notes>
        /// Needed to handle the "out" parameter correctly.
        /// </notes>
        private delegate bool TryParseDelegate<TTarget>(string source, out TTarget target);
        
        /// <summary>
        /// Returns dynamically built lambdas (cached) to perform various
        /// conversion functions.
        /// </summary>
        /// <notes>
        /// Design goals include:
        /// - Avoid unnecessary boxing/unboxing for value types.
        /// - Use reflection only once upon startup only if needed.
        /// </notes>
        private static class Functions
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            public static TTarget Cast<TSource, TTarget>(TSource source)
            {
                var target = Cache<TSource, TTarget>.Cast(source);
                return target;
            }

            public static string EnumToString<TSource>(TSource source, TypeConverterContext context)
            {
                var format = context.SafeGetFormat();
                var target = String.IsNullOrWhiteSpace(format)
                    ? Cache<TSource>.EnumToString(source)
                    : Cache<TSource>.EnumToStringWithFormat(source, format);
                return target;
            }

            public static bool EnumTryParse<TTarget>(string source, out TTarget target)
            {
                try
                {
                    var result = Cache<TTarget>.EnumTryParse(source, out target);
                    return result;
                }
                catch (Exception)
                {
                    target = default(TTarget);
                    return false;
                }
            }

            public static TTarget NewWithZeroArguments<TTarget>()
            {
                var target = Cache<TTarget>.NewWithZeroArguments();
                return target;
            }

            public static bool NullableEnumTryParse<TTarget>(string source, out TTarget target)
            {
                try
                {
                    var result = Cache<TTarget>.NullableEnumTryParse(source, out target);
                    return result;
                }
                catch (Exception)
                {
                    target = default(TTarget);
                    return false;
                }
            }
            #endregion

            // PRIVATE TYPES ////////////////////////////////////////////////
            #region Types
            private static class Cache<T>
            {
                // PUBLIC PROPERTIES ////////////////////////////////////////////
                #region Properties
                public static Func<T, string> EnumToString { get { return _enumToString.Value; } }
                public static Func<T, string, string> EnumToStringWithFormat { get { return _enumToStringWithFormat.Value; } }
                public static TryParseDelegate<T> EnumTryParse { get { return _enumTryParse.Value; } }
                public static Func<T> NewWithZeroArguments { get { return _newWithZeroArguments.Value; } }
                public static TryParseDelegate<T> NullableEnumTryParse { get { return _nullableEnumTryParse.Value; } }
                #endregion

                // PRIVATE METHODS //////////////////////////////////////////////
                #region Methods
                private static Func<T, string> CreateEnumToString()
                {
                    var enumType = typeof(Enum);
                    var toStringMethod = enumType
                        .GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

                    var sourceType = typeof(T);
                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var callExpression = Expression.Call(sourceExpression, toStringMethod);
                    var callLambdaExpression = Expression.Lambda<Func<T, string>>(callExpression, sourceExpression);
                    var callLambda = callLambdaExpression.Compile();

                    return callLambda;
                }

                private static Func<T, string, string> CreateEnumToStringWithFormat()
                {
                    var enumType = typeof(Enum);
                    var toStringWithFormatMethod = enumType
                        .GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, typeof(string));

                    var sourceType = typeof(T);
                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var formatExpression = Expression.Parameter(typeof(string), "format");
                    // ReSharper disable PossiblyMistakenUseOfParamsMethod
                    var callExpression = Expression.Call(sourceExpression, toStringWithFormatMethod, formatExpression);
                    // ReSharper restore PossiblyMistakenUseOfParamsMethod
                    var callLambdaExpression = Expression.Lambda<Func<T, string, string>>(callExpression, sourceExpression, formatExpression);
                    var callLambda = callLambdaExpression.Compile();

                    return callLambda;
                }

                private static TryParseDelegate<T> CreateEnumTryParse()
                {
                    // public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct;
                    var enumType = typeof(Enum);
                    var tryParseMethodInfoOpen = enumType.GetMethods().Single(x => x.Name == "TryParse" && x.GetParameters().Count() == 3);
                    var tryParseMethodInfoClosed = tryParseMethodInfoOpen.MakeGenericMethod(typeof(T));

                    var arguments = tryParseMethodInfoClosed.GetParameters();
                    var sourceType = arguments[0].ParameterType;
                    var resultType = arguments[2].ParameterType;

                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var ignoreCaseExpression = Expression.Constant(true, typeof(bool));
                    var resultExpression = Expression.Parameter(resultType, "result");
                    var callExpression = Expression.Call(tryParseMethodInfoClosed, sourceExpression, ignoreCaseExpression, resultExpression);
                    var callDelegateExpression = Expression.Lambda<TryParseDelegate<T>>(callExpression, sourceExpression, resultExpression);
                    var callDelegate = callDelegateExpression.Compile();

                    return callDelegate;
                }

                private static Func<T> CreateNewWithZeroArguments()
                {
                    var targetType = typeof(T);
                    var newExpression = Expression.New(targetType);
                    var newLambdaExpression = Expression.Lambda<Func<T>>(newExpression);
                    var newLambda = newLambdaExpression.Compile();
                    return newLambda;
                }

                private static TryParseDelegate<T> CreateNullableEnumTryParse()
                {
                    // public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct;
                    var typeConverterType = typeof(TypeConverter);
                    var tryParseMethodInfoOpen = typeConverterType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x => x.Name == "NullableEnumTryParse");

                    var targetType = typeof(T);
                    var targetUnderlyingType = Nullable.GetUnderlyingType(targetType);
                    var tryParseMethodInfoClosed = tryParseMethodInfoOpen.MakeGenericMethod(targetUnderlyingType);

                    var arguments = tryParseMethodInfoClosed.GetParameters();
                    var sourceType = arguments[0].ParameterType;
                    var resultType = arguments[2].ParameterType;

                    var sourceExpression = Expression.Parameter(sourceType, "source");
                    var ignoreCaseExpression = Expression.Constant(true, typeof(bool));
                    var resultExpression = Expression.Parameter(resultType, "result");
                    var callExpression = Expression.Call(tryParseMethodInfoClosed, sourceExpression, ignoreCaseExpression, resultExpression);
                    var callDelegateExpression = Expression.Lambda<TryParseDelegate<T>>(callExpression, sourceExpression, resultExpression);
                    var callDelegate = callDelegateExpression.Compile();

                    return callDelegate;
                }
                #endregion

                // PRIVATE FIELDS ///////////////////////////////////////////////
                #region Private Fields
                // ReSharper disable InconsistentNaming
                private static readonly Lazy<Func<T, string>> _enumToString = new Lazy<Func<T, string>>(CreateEnumToString, LazyThreadSafetyMode.PublicationOnly);
                private static readonly Lazy<Func<T, string, string>> _enumToStringWithFormat = new Lazy<Func<T, string, string>>(CreateEnumToStringWithFormat, LazyThreadSafetyMode.PublicationOnly);
                private static readonly Lazy<TryParseDelegate<T>> _enumTryParse = new Lazy<TryParseDelegate<T>>(CreateEnumTryParse, LazyThreadSafetyMode.PublicationOnly);
                private static readonly Lazy<Func<T>> _newWithZeroArguments = new Lazy<Func<T>>(CreateNewWithZeroArguments, LazyThreadSafetyMode.PublicationOnly);
                private static readonly Lazy<TryParseDelegate<T>> _nullableEnumTryParse = new Lazy<TryParseDelegate<T>>(CreateNullableEnumTryParse, LazyThreadSafetyMode.PublicationOnly);
                // ReSharper restore InconsistentNaming
                #endregion
            }

            private static class Cache<TSource, TTarget>
            {
                // PUBLIC PROPERTIES ////////////////////////////////////////////
                #region Properties
                public static Func<TSource, TTarget> Cast { get { return _castImpl.Value; } }
                #endregion

                // PRIVATE METHODS //////////////////////////////////////////////
                #region Methods
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

                // PRIVATE FIELDS ///////////////////////////////////////////////
                #region Private Fields
                // ReSharper disable InconsistentNaming
                private static readonly Lazy<Func<TSource, TTarget>> _castImpl = new Lazy<Func<TSource, TTarget>>(CreateCastImpl, LazyThreadSafetyMode.PublicationOnly);
                // ReSharper restore InconsistentNaming
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
