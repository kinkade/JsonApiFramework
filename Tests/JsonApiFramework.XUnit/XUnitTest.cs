// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Diagnostics.Contracts;

using Xunit.Abstractions;

namespace JsonApiFramework.XUnit
{
    /// <summary>Base class for all xUnit collection of unit tests</summary>
    public abstract class XUnitTest
    {
        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Write Methods
        public void WriteLine()
        {
            this.Output.WriteLine(String.Empty);
        }

        public void WriteLine(string message)
        {
            this.Output.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            this.Output.WriteLine(format, args);
        }
        #endregion

        // PROTECTED CONSTRUCTORS ///////////////////////////////////////////
        #region Constructors
        protected XUnitTest(ITestOutputHelper output)
        {
            Contract.Requires(output != null);

            this.Output = output;
        }
        #endregion

        // PRIVATE PROPERTIES ///////////////////////////////////////////////
        #region Properties
        public ITestOutputHelper Output { get; private set; }
        #endregion
    }
}
