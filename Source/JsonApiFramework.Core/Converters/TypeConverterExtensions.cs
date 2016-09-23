// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Diagnostics.Contracts;

namespace JsonApiFramework.Converters
{
    /// <summary>
    /// Extension methods built from the <c>ITypeConverter</c> abstraction.
    /// </summary>
    public static class TypeConverterExtensions
    {
        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Extensions Methods
        public static TTarget Convert<TSource, TTarget>(this ITypeConverter typeConverter, TSource source, TypeConverterContext context)
        {
            Contract.Requires(typeConverter != null);

            try
            {
                TTarget target;
                if (typeConverter.TryConvert(source, context, out target))
                {
                    return target;
                }
            }
            catch (Exception exception)
            {
                throw TypeConverterException.Create<TSource, TTarget>(source, exception);
            }

            throw TypeConverterException.Create<TSource, TTarget>(source);
        }

        public static TTarget Convert<TSource, TTarget>(this ITypeConverter typeConverter, TSource source)
        {
            Contract.Requires(typeConverter != null);

            return typeConverter.Convert<TSource, TTarget>(source, null);
        }

        public static bool TryConvert<TSource, TTarget>(this ITypeConverter typeConverter, TSource source, out TTarget target)
        {
            Contract.Requires(typeConverter != null);

            return typeConverter.TryConvert(source, null, out target);
        }
        #endregion
    }
}
