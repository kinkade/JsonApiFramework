// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

using JsonApiFramework.Expressions;
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
        public bool TryConvert<TSource, TTarget>(TSource source, TypeConverterContext context, out TTarget target)
        {
            // Handle when there exists a valid cast between source and
            // target types.
            var castResult = CastTo<TTarget>.TryFrom(source, out target);
            if (castResult)
                return true;

            // Handle when there exists a type converter definition between
            // source and target types.
            ITypeConverterDefinition<TSource, TTarget> definition;
            if (this.TryGetTypeConverterDefinition(out definition))
            {
                var definitionConvertResult = TryConvertWithTypeConverterDefinition(definition, source, context, out target);
                if (definitionConvertResult)
                    return true;
            }

            // Handle when target type is an Enum.
            if (TryConvertToEnum(source, context, out target))
                return true;

            // If we get here, unable to convert between types.
            return false;
        }
        #endregion

        // PRIVATE PROPERTIES ///////////////////////////////////////////////
        #region Properties
        private IDictionary<Tuple<Type, Type>, ITypeConverterDefinition> TypeConverterDefinitions { get; set; }
        #endregion

        // PRIVATE METHODS //////////////////////////////////////////////////
        #region Special Case Methods
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

        private bool TryConvertToEnum<TSource, TTarget>(TSource source, TypeConverterContext context, out TTarget target)
        {
            target = default(TTarget);

            // If target is not an enumeration type and not a nullable type,
            // then nothing to do for this method.
            var targetType = typeof(TTarget);
            var isTargetTypeEnum = targetType.IsEnum();
            var isTargetTypeNullableEnum = targetType.IsNullableType() && Nullable.GetUnderlyingType(targetType).IsEnum();
            if (!isTargetTypeEnum && !isTargetTypeNullableEnum)
                return false;

            // Handle when source type can be converted to an integer.
            int sourceAsInt;
            if (this.TryConvert(source, out sourceAsInt))
            {
                // Casting from an integer to an enumeration always works.
                target = CastTo<TTarget>.From(sourceAsInt);
                return true;
            }

            // Handle when source type is a string.
            if (typeof(TSource).IsString())
            {
                var sourceAsString = CastTo<string>.From(source);
                return Enum<TTarget>.TryParse(sourceAsString, out target);




                //var enumType = typeof(Enum);
                //var tryParseMethodInfoOpen = enumType.GetMethods().FirstOrDefault(x => x.Name == "TryParse");
                //var tryParseMethodInfoClosed = tryParseMethodInfoOpen.MakeGenericMethod(targetType);
                //var arguments = tryParseMethodInfoClosed.GetParameters();

                //var argument1Type = arguments[0].ParameterType;
                //var argument2Type = arguments[1].ParameterType;

                //var argument1Expression = Expression.Parameter(argument1Type, "source");
                //var argument2Expression = Expression.Parameter(argument2Type, "target");
                //var callExpression = Expression.Call(tryParseMethodInfoClosed, argument1Expression, argument2Expression);
                //var callLambdaExpression = Expression.Lambda<EnumTryParseDelegate<TTarget>>(callExpression, argument1Expression, argument2Expression);
                //var callLambda = callLambdaExpression.Compile();
                //var tryParseResult = callLambda(sourceAsString, out target);
                //return tryParseResult;
            }

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

        private static bool TryConvertWithTypeConverterDefinition<TSource, TTarget>(ITypeConverterDefinition<TSource, TTarget> definition, TSource source, TypeConverterContext context, out TTarget target)
        {
            Contract.Requires(definition != null);

            try
            {
                var result = definition.TryConvert(source, context, out target);
                return result;
            }
            catch (Exception)
            {
                target = default(TTarget);
                return false;
            }
        }
        #endregion

        // PRIVATE FIELDS ///////////////////////////////////////////////////
        #region Fields
        private static readonly ITypeConverterDefinition[] DefaultDefinitions =
            {
                new TypeConverterDefinitionFunc<bool, decimal>((s, c) => Convert.ToDecimal(s)),
                new TypeConverterDefinitionFunc<bool, decimal?>((s, c) => Convert.ToDecimal(s)),
                new TypeConverterDefinitionFunc<bool, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<byte, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<byte, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<byte, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<byte[], string>((s, c) => Convert.ToBase64String(s)),
                new TypeConverterDefinitionFunc<char, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<char, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<char, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<DateTime, string>(ConvertDateTimeToString),
                new TypeConverterDefinitionFunc<DateTimeOffset, DateTime>((s, c) => s.DateTime),
                new TypeConverterDefinitionFunc<DateTimeOffset, DateTime?>((s, c) => s.DateTime),
                new TypeConverterDefinitionFunc<DateTimeOffset, string>(ConvertDateTimeOffsetToString),
                new TypeConverterDefinitionFunc<decimal, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<decimal, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<decimal, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<double, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<double, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<double, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<float, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<float, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<float, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<int, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<int, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<int, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<long, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<long, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<long, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<sbyte, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<sbyte, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<sbyte, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<short, bool>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<short, bool?>((s, c) => Convert.ToBoolean(s)),
                new TypeConverterDefinitionFunc<short, string>((s, c) => Convert.ToString(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, bool>((s, c) => Convert.ToBoolean(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, bool?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToBoolean(s, c.SafeGetFormatProvider()) : new bool?()),
                new TypeConverterDefinitionFunc<string, byte>((s, c) => Convert.ToByte(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, byte?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToByte(s, c.SafeGetFormatProvider()) : new byte?()),
                new TypeConverterDefinitionFunc<string, char>((s, c) => Convert.ToChar(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, char?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToChar(s, c.SafeGetFormatProvider()) : new char?()),
                new TypeConverterDefinitionFunc<string, DateTime>((s, c) => ConvertStringToNullableDateTime(s, c).GetValueOrDefault()),
                new TypeConverterDefinitionFunc<string, DateTime?>(ConvertStringToNullableDateTime),
                new TypeConverterDefinitionFunc<string, DateTimeOffset>((s, c) => ConvertStringToNullableDateTimeOffset(s, c).GetValueOrDefault()),
                new TypeConverterDefinitionFunc<string, DateTimeOffset?>(ConvertStringToNullableDateTimeOffset),
                new TypeConverterDefinitionFunc<string, decimal>((s, c) => Convert.ToDecimal(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, decimal?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToDecimal(s, c.SafeGetFormatProvider()) : new decimal?()),
                new TypeConverterDefinitionFunc<string, double>((s, c) => Convert.ToDouble(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, double?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToDouble(s, c.SafeGetFormatProvider()) : new double?()),
                new TypeConverterDefinitionFunc<string, float>((s, c) => Convert.ToSingle(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, float?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToSingle(s, c.SafeGetFormatProvider()) : new float?()),
                new TypeConverterDefinitionFunc<string, int>((s, c) => Convert.ToInt32(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, int?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToInt32(s, c.SafeGetFormatProvider()) : new int?()),
                new TypeConverterDefinitionFunc<string, long>((s, c) => Convert.ToInt64(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, long?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToInt64(s, c.SafeGetFormatProvider()) : new long?()),
                new TypeConverterDefinitionFunc<string, sbyte>((s, c) => Convert.ToSByte(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, sbyte?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToSByte(s, c.SafeGetFormatProvider()) : new sbyte?()),
                new TypeConverterDefinitionFunc<string, short>((s, c) => Convert.ToInt16(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, short?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToInt16(s, c.SafeGetFormatProvider()) : new short?()),
                new TypeConverterDefinitionFunc<string, uint>((s, c) => Convert.ToUInt32(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, uint?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToUInt32(s, c.SafeGetFormatProvider()) : new uint?()),
                new TypeConverterDefinitionFunc<string, ulong>((s, c) => Convert.ToUInt64(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, ulong?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToUInt64(s, c.SafeGetFormatProvider()) : new ulong?()),
                new TypeConverterDefinitionFunc<string, ushort>((s, c) => Convert.ToUInt16(s, c.SafeGetFormatProvider())),
                new TypeConverterDefinitionFunc<string, ushort?>((s, c) => !String.IsNullOrWhiteSpace(s) ? Convert.ToUInt16(s, c.SafeGetFormatProvider()) : new ushort?()),
            };


                            //new TryConvertGenericTest<string, PrimaryColor?>("StringToNullable<Enum>", "42", ConvertResult.Success, (PrimaryColor)42),
                            //new TryConvertGenericTest<string, Guid?>("StringToNullable<Guid>", "42", ConvertResult.Failure, default(Guid?)),
                            //new TryConvertGenericTest<string, TimeSpan?>("StringToNullable<TimeSpan>", "42", ConvertResult.Failure, default(TimeSpan?)),

        #endregion

        // PRIVATE TYPES ////////////////////////////////////////////////////
        #region Types
        /// <summary>
        /// Slick class that uses dynamically built cached lamdas to cast
        /// between types.
        /// </summary>
        /// <notes>
        /// Avoids unecessary boxing/unboxing between value types.
        /// </notes>
        /// <seealso cref="http://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int"/>
        private static class CastTo<TTarget>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            // ReSharper disable UnusedMember.Local
            public static TTarget From<TSource>(TSource source)
            {
                return Cache<TSource>.Caster(source);
            }
            // ReSharper restore UnusedMember.Local

            public static bool TryFrom<TSource>(TSource source, out TTarget target)
            {
                try
                {
                    target = Cache<TSource>.Caster(source);
                    return true;
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
            private static class Cache<TSource>
            {
                #region Public Fields
                public static readonly Func<TSource, TTarget> Caster = CreateCaster();
                #endregion

                #region Private Methods
                private static Func<TSource, TTarget> CreateCaster()
                {
                    var parameterExpression = Expression.Parameter(typeof(TSource));
                    var convertExpression = Expression.ConvertChecked(parameterExpression, typeof(TTarget));
                    var convertLambda = Expression
                        .Lambda<Func<TSource, TTarget>>(convertExpression, parameterExpression)
                        .Compile();
                    return convertLambda;
                }
                #endregion
            }
            #endregion
        }

        private delegate bool EnumTryParseDelegate<TTarget>(string source, out TTarget target);

        private static class Enum<TTarget>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////
            #region Methods
            public static bool TryParse(string source, out TTarget target)
            {
                try
                {
                    return Cache.EnumTryParser(source, out target);
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
                #region Public Fields
                public static readonly EnumTryParseDelegate<TTarget> EnumTryParser = CreateEnumTryParser();
                #endregion

                #region Private Methods
                private static EnumTryParseDelegate<TTarget> CreateEnumTryParser()
                {
                    var enumType = typeof(Enum);
                    var tryParseMethodInfoOpen = enumType.GetMethods().FirstOrDefault(x => x.Name == "TryParse");
                    var tryParseMethodInfoClosed = tryParseMethodInfoOpen.MakeGenericMethod(typeof(TTarget));
                    var arguments = tryParseMethodInfoClosed.GetParameters();

                    var argument1Type = arguments[0].ParameterType;
                    var argument2Type = arguments[1].ParameterType;

                    var argument1Expression = Expression.Parameter(argument1Type, "source");
                    var argument2Expression = Expression.Parameter(argument2Type, "target");
                    var callExpression = Expression.Call(tryParseMethodInfoClosed, argument1Expression, argument2Expression);
                    var callDelegateExpression = Expression.Lambda<EnumTryParseDelegate<TTarget>>(callExpression, argument1Expression, argument2Expression);
                    var callDelegate = callDelegateExpression.Compile();

                    return callDelegate;
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
