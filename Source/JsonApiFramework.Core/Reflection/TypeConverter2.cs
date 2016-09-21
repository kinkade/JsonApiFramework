// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Linq.Expressions;

namespace JsonApiFramework.Reflection
{
    /// <summary>
    /// Utility class that converts object from one type to another type.
    /// http://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int
    /// </summary>
    public static class TypeConverter2
    {
        public static bool TryConvert<TTarget, TSource>(TSource source, IFormatProvider targetFormatProvider, out TTarget target)
        {
            return CastTo<TTarget>.TryFrom(source, out target);
        }

        private static class CastTo<TTarget>
        {
            // PUBLIC METHODS ///////////////////////////////////////////////////
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

            private static class Cache<TSource>
            {
                public static readonly Func<TSource, TTarget> Caster = Get();

                private static Func<TSource, TTarget> Get()
                {
                    var p = Expression.Parameter(typeof(TSource));
                    var c = Expression.ConvertChecked(p, typeof(TTarget));
                    return Expression.Lambda<Func<TSource, TTarget>>(c, p).Compile();
                }
            }
        }
    }
}
