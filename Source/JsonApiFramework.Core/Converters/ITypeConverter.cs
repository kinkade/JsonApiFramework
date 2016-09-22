// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;

namespace JsonApiFramework.Converters
{
    /// <summary>
    /// Abstract a type converter that converts from one type to another type.
    /// </summary>
    public interface ITypeConverter
    {
        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Methods
        bool TryConvert<TSource, TTarget>(TSource source, string format, IFormatProvider formatProvider, out TTarget target);
        #endregion
    }
}
