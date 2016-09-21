// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Collections.Generic;

using FluentAssertions;

using JsonApiFramework.Reflection;
using JsonApiFramework.XUnit;

using Xunit;
using Xunit.Abstractions;

namespace JsonApiFramework.Tests.Reflection
{
    public class TypeConverterTests : XUnitTest
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public TypeConverterTests(ITestOutputHelper output)
            : base(output)
        { }
        #endregion

        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region Test Methods
        [Theory]
        [MemberData("TryConvertTestData")]
        public void TestTypeConveterTryConvert(IUnitTest[] unitTestCollection)
        {
            foreach (var unitTest in unitTestCollection)
            {
                unitTest.Execute(this);

                this.Output.WriteLine(String.Empty);
                this.Output.WriteLine("-----------------------------------------------------------------------------");
                this.Output.WriteLine(String.Empty);
            }
        }
        #endregion

        // PRIVATE FIELDS ////////////////////////////////////////////////////
        #region Test Data
        public static readonly IEnumerable<object[]> TryConvertTestData = new[]
            {
                new object []
                {
                    new object []
                        {
                            // Simple Types
                            new TryConvertTest<bool, bool>("BoolToBool", true, ConvertResult.Success, true),
                            new TryConvertTest<bool, byte>("BoolToByte", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, byte[]>("BoolToByteArray", true, ConvertResult.Failure, default(byte[])),
                            new TryConvertTest<bool, char>("BoolToChar", true, ConvertResult.Success, (char)1),
                            new TryConvertTest<bool, DateTime>("BoolToDateTime", true, ConvertResult.Failure, default(DateTime)),
                            new TryConvertTest<bool, DateTimeOffset>("BoolToDateTimeOffset", true, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertTest<bool, decimal>("BoolToDecimal", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, double>("BoolToDouble", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, PrimaryColor>("BoolToEnum", true, ConvertResult.Success, (PrimaryColor)1),
                            new TryConvertTest<bool, float>("BoolToFloat", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, Guid>("BoolToGuid", true, ConvertResult.Failure, default(Guid)),
                            new TryConvertTest<bool, int>("BoolToInt", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, long>("BoolToLong", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, sbyte>("BoolToSByte", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, short>("BoolToShort", true, ConvertResult.Success, 1),
                            //new TryConvertTest<bool, string>("BoolToString", true, ConvertResult.Success, "True"),
                            new TryConvertTest<bool, TimeSpan>("BoolToTimeSpan", true, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertTest<bool, Type>("BoolToType", true, ConvertResult.Failure, default(Type)),
                            new TryConvertTest<bool, uint>("BoolToUInt", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, ulong>("BoolToULong", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, ushort>("BoolToUShort", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, Uri>("BoolToUri", true, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertTest<bool, bool?>("BoolToNullable<Bool>", true, ConvertResult.Success, true),
                            new TryConvertTest<bool, byte?>("BoolToNullable<Byte>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, char?>("BoolToNullable<Char>", true, ConvertResult.Success, (char)1),
                            new TryConvertTest<bool, DateTime?>("BoolToNullable<DateTime>", true, ConvertResult.Failure, new DateTime?()),
                            new TryConvertTest<bool, DateTimeOffset?>("BoolToNullable<DateTimeOffset>", true, ConvertResult.Failure, new DateTimeOffset?()),
                            new TryConvertTest<bool, decimal?>("BoolToNullable<Decimal>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, double?>("BoolToNullable<Double>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, PrimaryColor?>("BoolToNullable<Enum>", true, ConvertResult.Success, (PrimaryColor)1),
                            new TryConvertTest<bool, float?>("BoolToNullable<Float>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, Guid?>("BoolToNullable<Guid>", true, ConvertResult.Failure, new Guid?()),
                            new TryConvertTest<bool, int?>("BoolToNullable<Int>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, long?>("BoolToNullable<Long>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, sbyte?>("BoolToNullable<SByte>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, short?>("BoolToNullable<Short>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, TimeSpan?>("BoolToNullable<TimeSpan>", true, ConvertResult.Failure, new TimeSpan?()),
                            new TryConvertTest<bool, uint?>("BoolToNullable<UInt>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, ulong?>("BoolToNullable<ULong>", true, ConvertResult.Success, 1),
                            new TryConvertTest<bool, ushort?>("BoolToNullable<UShort>", true, ConvertResult.Success, 1),

                            // Interface/Class Types
                            new TryConvertTest<bool, IInterface>("BoolToInterface", true, ConvertResult.Failure, default(IInterface)),
                            new TryConvertTest<bool, BaseClass>("BoolToBaseClass", true, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertTest<bool, DerivedClass>("BoolToDerivedClass", true, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
            };

        public static readonly DateTime TestDateTime = new DateTime(1968, 5, 20, 20, 2, 42, 123, DateTimeKind.Utc);
        public static readonly string TestDateTimeString = TestDateTime.ToString("O");

        public static readonly DateTimeOffset TestDateTimeOffset = new DateTimeOffset(1968, 5, 20, 20, 2, 42, 123, TimeSpan.Zero);
        public static readonly string TestDateTimeOffsetString = TestDateTimeOffset.ToString("O");

        public static readonly TimeSpan TestTimeSpan = new TimeSpan(42, 0, 0, 0, 0);
        public static readonly string TestTimeSpanString = TestTimeSpan.ToString("c");

        public const string TestGuidString = "5167e9e1-a15f-41e1-af46-442ffcd37f1b";
        public static readonly Guid TestGuid = new Guid(TestGuidString);
        public static readonly byte[] TestGuidByteArray = TestGuid.ToByteArray();

        public const string TestUriString = "https://api.example.com:8002/api/en-us/articles/42";
        public static readonly Uri TestUri = new Uri(TestUriString);

        public static readonly byte[] TestByteArray = { 42, 24, 48, 84, 12, 21, 68, 86 };
        public const string TestByteArrayString = "KhgwVAwVRFY=";

        public static readonly Type TestType = typeof(TypeConverterTests);
        public static readonly string TestTypeString = TestType.GetCompactQualifiedName();

        public const int TestRedOrdinalValue0 = 0;
        public const int TestGreenOrdinalValue24 = 24;
        public const int TestBlueOrdinalValue42 = 42;

        public const string TestBlueString = "Blue";

        // ReSharper disable UnusedMember.Global
        public enum PrimaryColor
        {
            Red = TestRedOrdinalValue0,
            Green = TestGreenOrdinalValue24,
            Blue = TestBlueOrdinalValue42
        };
        // ReSharper restore UnusedMember.Global

        public const int TestEnumOrdinal = TestBlueOrdinalValue42;
        public const string TestEnumString = TestBlueString;
        public const PrimaryColor TestEnum = PrimaryColor.Blue;

        public interface IInterface
        { }

        public class BaseClass : IInterface
        { }

        public class DerivedClass : BaseClass
        { }

        public static readonly BaseClass TestBaseClass = new BaseClass();
        public static readonly DerivedClass TestDerivedClass = new DerivedClass();
        #endregion

        #region Test Types
        public enum ConvertResult
        {
            Success,
            Failure
        }

        public class TryConvertTest<TSource, TTarget> : UnitTest
        {
            // PUBLIC CONSTRUCTORS //////////////////////////////////////////
            #region Constructors
            public TryConvertTest(string name, TSource source, ConvertResult expectedResult, TTarget expectedValue)
                : base(name)
            {
                this.Source = source;
                this.ExpectedResult = expectedResult;
                this.ExpectedValue = expectedValue;
            }
            #endregion

            // PROTECTED METHODS ////////////////////////////////////////////
            #region UnitTest Overrides
            protected override void Arrange()
            {
                this.WriteLine("Source:    {0} ({1})", this.Source, typeof(TSource).Name);
                this.WriteLine();

                this.WriteLine("Expected");
                this.WriteLine("  Result:  {0}", this.ExpectedResult);
                this.WriteLine("  Value:   {0} ({1})", this.ExpectedValue, typeof(TTarget).Name);
                this.WriteLine();
            }

            protected override void Act()
            {
                var source = this.Source;
                TTarget actualValue;
                var actualResult = TypeConverter2.TryConvert(source, out actualValue);

                this.ActualResult = actualResult ? ConvertResult.Success : ConvertResult.Failure;
                this.ActualValue = actualValue;

                this.WriteLine("Actual");
                this.WriteLine("  Result:  {0}", this.ActualResult);
                this.WriteLine("  Value:   {0} ({1})", this.ActualValue, typeof(TTarget).Name);
            }

            protected override void Assert()
            {
                this.ActualResult.Should().Be(this.ExpectedResult);
                switch (this.ActualResult)
                {
                    case ConvertResult.Success:
                        {
                            // Special case if target type is nullable.
                            if (typeof(TTarget).IsNullableType())
                            {
                                var underlyingType = Nullable.GetUnderlyingType(typeof(TTarget));
                                this.ActualValue.Should().BeOfType(underlyingType);
                                this.ActualValue.Should().Be(this.ExpectedValue);
                                return;
                            }

                            this.ActualValue.Should().BeOfType<TTarget>();
                            this.ActualValue.Should().Be(this.ExpectedValue);
                        }
                        break;

                    case ConvertResult.Failure:
                        {
                            this.ActualValue.Should().Be(default(TTarget));
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endregion

            // PRIVATE PROPERTIES ///////////////////////////////////////////
            #region Calculated Properties
            private ConvertResult ActualResult { get; set; }
            private TTarget ActualValue { get; set; }
            #endregion

            #region User Supplied Properties
            private TSource Source { get; set; }

            private ConvertResult ExpectedResult { get; set; }
            private TTarget ExpectedValue { get; set; }
            #endregion
        }
        #endregion
    }
}
