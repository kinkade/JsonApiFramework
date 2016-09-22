// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;

namespace JsonApiFramework.Converters
{
    /// <summary>
    /// Represents an exception that is thrown when this component is unable
    /// to convert between types.
    /// </summary>
    public class TypeConverterException : Exception
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public TypeConverterException()
        { }

        public TypeConverterException(string message)
            : base(message)
        { }

        public TypeConverterException(string message, Exception innerException)
            : base(message, innerException)
        { }
        #endregion
    }
}