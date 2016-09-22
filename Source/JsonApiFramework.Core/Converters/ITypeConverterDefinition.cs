// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;

namespace JsonApiFramework.Converters
{
    public interface ITypeConverterDefinition
    {
        // PUBLIC PROPERTIES ////////////////////////////////////////////////
        #region Methods
        Type SourceType { get; }
        Type TargetType { get; }
        #endregion
    }

    public interface ITypeConverterDefinition<in TSource, TTarget> : ITypeConverterDefinition
    {
        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Methods
        bool TryConvert(TSource source, IFormatProvider formatProvider, out TTarget target);
        #endregion
    }
}
