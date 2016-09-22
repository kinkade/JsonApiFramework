// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq.Expressions;

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
        public bool TryConvert<TSource, TTarget>(TSource source, IFormatProvider formatProvider, out TTarget target)
        {
            // Handle when there exists a valid cast between source and
            // target types.
            var castResult = CastTo<TTarget>.TryFrom(source, out target);
            if (castResult)
                return true;

            // Handle when there exists a type converter definition between
            // source and target types.
            ITypeConverterDefinition<TSource, TTarget> definition;
            if (this.GetTypeConverterDefinition(out definition))
            {
                var definitionConvertResult = definition.TryConvert(source, formatProvider, out target);
                if (definitionConvertResult)
                    return true;
            }

            // If we get here, unable to convert between types.
            return false;
        }
        #endregion

        // PRIVATE PROPERTIES ///////////////////////////////////////////////
        #region Properties
        private IDictionary<Tuple<Type, Type>, ITypeConverterDefinition> TypeConverterDefinitions { get; set; }
        #endregion

        // PRIVATE METHODS //////////////////////////////////////////////////
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

        private bool GetTypeConverterDefinition<TSource, TTarget>(out ITypeConverterDefinition<TSource, TTarget> definition)
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

        // PRIVATE FIELDS ///////////////////////////////////////////////////
        #region Fields
        private static readonly ITypeConverterDefinition[] DefaultDefinitions =
            {
                new TypeConverterDefinitionFunc<bool, decimal>((s, fp) => Convert.ToDecimal(s)),
                new TypeConverterDefinitionFunc<bool, decimal?>((s, fp) => Convert.ToDecimal(s)),
                new TypeConverterDefinitionFunc<bool, string>((s, fp) => Convert.ToString(s, fp ?? CultureInfo.InvariantCulture))
            };
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
                public static readonly Func<TSource, TTarget> Caster = Get();
                #endregion

                #region Private Methods
                private static Func<TSource, TTarget> Get()
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
        #endregion
    }
}
