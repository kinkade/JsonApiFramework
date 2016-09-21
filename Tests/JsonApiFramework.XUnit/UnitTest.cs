using System;
using System.Diagnostics.Contracts;

namespace JsonApiFramework.XUnit
{
    /// <summary>Base class for an individual unit test for a xUnit test.</summary>
    public abstract class UnitTest : IUnitTest
    {
        // PUBLIC PROPERTIES ////////////////////////////////////////////
        #region IUnitTest Implementation
        public string Name { get; private set; }
        #endregion

        // PUBLIC METHODS ///////////////////////////////////////////////
        #region IUnitTest Implementation
        public void Execute(XUnitTest xUnitTest)
        {
            this.XUnitTest = xUnitTest;

            this.WriteLine("Test Name: {0}", this.Name);
            this.WriteLine(String.Empty);

            this.Arrange();
            this.Act();
            this.Assert();
        }
        #endregion

        // PROTECTED CONSTRUCTORS ///////////////////////////////////////
        #region Constructors
        protected UnitTest(string name)
        {
            Contract.Requires(String.IsNullOrWhiteSpace(name) == false);

            this.Name = name;
        }
        #endregion

        // PROTECTED PROPERTIES /////////////////////////////////////////////
        #region Properties
        protected XUnitTest XUnitTest { get; private set; }
        #endregion

        // PROTECTED METHODS ////////////////////////////////////////////
        #region UnitTest Overrides
        protected virtual void Arrange()
        { }

        protected virtual void Act()
        { }

        protected virtual void Assert()
        { }
        #endregion

        #region Write Methods
        protected void WriteLine()
        {
            this.XUnitTest.WriteLine(String.Empty);
        }

        protected void WriteLine(string message)
        {
            this.XUnitTest.WriteLine(message);
        }

        protected void WriteLine(string format, params object[] args)
        {
            this.XUnitTest.WriteLine(format, args);
        }
        #endregion

    }
}