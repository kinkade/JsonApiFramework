// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Diagnostics.Contracts;

namespace JsonApiFramework.Converters
{
    public class TypeConverterDefinitionFunc<TSource, TTarget> : ITypeConverterDefinition<TSource, TTarget>
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public TypeConverterDefinitionFunc(Func<TSource, IFormatProvider, TTarget> converter)
        {
            Contract.Requires(converter != null);

            this.Converter = converter;
        }
        #endregion

        // PUBLIC PROPERTIES ////////////////////////////////////////////////
        #region ITypeConverterDefinition Implementation
        public Type SourceType { get { return typeof(TSource); } }
        public Type TargetType { get { return typeof(TTarget); } }
        #endregion

        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region ITypeConverterDefinition<TSource, TTarget> Implementation
        public bool TryConvert(TSource source, IFormatProvider formatProvider, out TTarget target)
        {
            target = this.Converter(source, formatProvider);
            return true;
        }
        #endregion

        // PRIVATE PROPERTIES ///////////////////////////////////////////////
        #region Properties
        private Func<TSource, IFormatProvider, TTarget> Converter { get; set; }
        #endregion
    }
}
