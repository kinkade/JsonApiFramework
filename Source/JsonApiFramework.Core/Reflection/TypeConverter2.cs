// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Linq.Expressions;

namespace JsonApiFramework.Reflection
{
    /// <summary>
    /// Type converter that converts from one type to another type. 
    /// </summary>
    /// <notes>
    /// For the generic versions, boxing/unboxing is eliminated by using
    /// dynamically built lamdas that avoid boxing/unboxing between value types.
    /// </notes>
    public static class TypeConverter2
    {
        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Convert Methods
        public static bool TryConvert<TTarget, TSource>(TSource source, out TTarget target)
        {
            // Handle nominal case when there exists a valid cast between
            // source and target types.
            var validCastResult = CastTo<TTarget>.TryFrom(source, out target);
            if (validCastResult)
                return true;

            // Handle special case when there exists an available conversion
            // between source and target types.

            // Handle case when there exists a convert using Convert.ChangeType
            // method.
            if (typeof(TTarget) == typeof(decimal) && typeof(TSource) == typeof(bool))
            {
                var sourceAsBool = CastTo<bool>.From(source);
                var targetAsDecimal = Convert.ToDecimal(sourceAsBool);
                target = CastTo<TTarget>.From(targetAsDecimal);
                return true;
            }

            // If we get here, unable to convert between types.
            return false;
        }
        #endregion

        // PRIVATE METHODS //////////////////////////////////////////////////
        #region Methods
        #endregion

        // PRIVATE FIELDS ///////////////////////////////////////////////////
        #region Fields
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
            public static TTarget From<TSource>(TSource source)
            {
                return Cache<TSource>.Caster(source);
            }

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
