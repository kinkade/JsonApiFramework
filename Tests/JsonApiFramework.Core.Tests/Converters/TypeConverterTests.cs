// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

using FluentAssertions;

using JsonApiFramework.Converters;
using JsonApiFramework.Reflection;
using JsonApiFramework.XUnit;

using Xunit;
using Xunit.Abstractions;

namespace JsonApiFramework.Tests.Converters
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
        [MemberData("TryConvertGenericTestData")]
        public void TestTypeConveterTryConvertGeneric(string name, IUnitTest[] unitTestCollection)
        {
            this.WriteLine(name);
            this.WriteLine();
            this.WriteDashedLine();
            this.WriteLine();

            foreach (var unitTest in unitTestCollection)
            {
                unitTest.Execute(this);

                this.WriteLine();
                this.WriteDashedLine();
                this.WriteLine();
            }
        }
        #endregion

        // PRIVATE FIELDS ////////////////////////////////////////////////////
        #region Test Data

        #region Sample Data
        public const string DefaultDateTimeFormat = "O";
        public const string FullDateTimeFormat = "F";
        public static readonly IFormatProvider SpanishMexicoCulture = CultureInfo.CreateSpecificCulture("es-MX");

        public static readonly TypeConverterContext FormatDateTimeContext = new TypeConverterContext
            {
                Format = FullDateTimeFormat,
                DateTimeStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            };

        public static readonly TypeConverterContext FormatAndFormatProviderDateTimeContext = new TypeConverterContext
            {
                Format = FullDateTimeFormat,
                FormatProvider = SpanishMexicoCulture,
                DateTimeStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            };

        public static readonly TypeConverterContext FormatDateTimeOffsetContext = new TypeConverterContext
            {
                Format = FullDateTimeFormat,
                DateTimeStyles = DateTimeStyles.AssumeUniversal
            };

        public static readonly TypeConverterContext FormatAndFormatProviderDateTimeOffsetContext = new TypeConverterContext
            {
                Format = FullDateTimeFormat,
                FormatProvider = SpanishMexicoCulture,
                DateTimeStyles = DateTimeStyles.AssumeUniversal
            };

        public static readonly DateTime TestDateTime = new DateTime(1968, 5, 20, 20, 2, 42, 0, DateTimeKind.Utc);
        public static readonly string TestDateTimeString = TestDateTime.ToString(DefaultDateTimeFormat);
        public static readonly string TestDateTimeStringWithFormat = TestDateTime.ToString(FullDateTimeFormat);
        public static readonly string TestDateTimeStringWithFormatAndFormatProvider = TestDateTime.ToString(FullDateTimeFormat, SpanishMexicoCulture);

        public static readonly DateTimeOffset TestDateTimeOffset = new DateTimeOffset(1968, 5, 20, 20, 2, 42, 0, TimeSpan.Zero);
        public static readonly string TestDateTimeOffsetString = TestDateTimeOffset.ToString(DefaultDateTimeFormat);
        public static readonly string TestDateTimeOffsetStringWithFormat = TestDateTimeOffset.ToString(FullDateTimeFormat);
        public static readonly string TestDateTimeOffsetStringWithFormatAndFormatProvider = TestDateTimeOffset.ToString(FullDateTimeFormat, SpanishMexicoCulture);


        public const string DefaultTimeSpanFormat = "c";
        public const string GeneralShortTimeSpanFormat = "g";
        public static readonly IFormatProvider FrenchFranceCulture = CultureInfo.CreateSpecificCulture("fr-FR");

        public static readonly TypeConverterContext FormatTimeSpanContext = new TypeConverterContext
            {
                Format = GeneralShortTimeSpanFormat
            };

        public static readonly TypeConverterContext FormatAndFormatProviderTimeSpanContext = new TypeConverterContext
            {
                Format = GeneralShortTimeSpanFormat,
                FormatProvider = FrenchFranceCulture
            };

        public static readonly TimeSpan TestTimeSpan = new TimeSpan(42, 12, 24, 36, 123);
        public static readonly string TestTimeSpanString = TestTimeSpan.ToString(DefaultTimeSpanFormat);
        public static readonly string TestTimeSpanStringWithFormat = TestTimeSpan.ToString(GeneralShortTimeSpanFormat);
        public static readonly string TestTimeSpanStringWithFormatAndFormatProvider = TestTimeSpan.ToString(GeneralShortTimeSpanFormat, FrenchFranceCulture);

        public const string TestGuidString = "5167e9e1-a15f-41e1-af46-442ffcd37f1b";
        public static readonly Guid TestGuid = new Guid(TestGuidString);
        public static readonly byte[] TestGuidByteArray = TestGuid.ToByteArray();

        public const string TestUriString = "https://api.example.com:8002/api/en-us/articles/42";
        public static readonly Uri TestUri = new Uri(TestUriString);

        public static readonly byte[] TestByteArray = { 42, 24, 48, 84, 12, 21, 68, 86 };
        public const string TestByteArrayString = "KhgwVAwVRFY=";

        public static readonly Type TestType = typeof(TypeConverterTests);
        public static readonly string TestTypeString = TestType.GetCompactQualifiedName();

        public const int TestRedOrdinal = 0;
        public const int TestGreenOrdinal = 1;
        public const int TestBlueOrdinal = 42;
        public const string TestBlueString = "Blue";
        public const string TestBlueLowercaseString = "blue";
        public const string IntegerEnumFormat = "D";
        public const string TestBlueOrdinalAsString = "42";

        public static readonly TypeConverterContext FormatEnumContext = new TypeConverterContext
            {
                Format = IntegerEnumFormat
            };

        // ReSharper disable UnusedMember.Global
        public enum PrimaryColor
        {
            Red = TestRedOrdinal,
            Green = TestGreenOrdinal,
            Blue = TestBlueOrdinal
        };
        // ReSharper restore UnusedMember.Global

        public interface IInterface
        { }

        public class BaseClass : IInterface
        { }

        public class DerivedClass : BaseClass
        { }

        public static readonly BaseClass TestBaseClass = new BaseClass();
        public static readonly DerivedClass TestDerivedClass = new DerivedClass();
        #endregion

        public static readonly IEnumerable<object[]> TryConvertGenericTestData = new[]
            {
                // Simple Types /////////////////////////////////////////////

                // Nullable Types ///////////////////////////////////////////

                #region Nullable<Byte>ToXXX
                new object []
                {
                    "Nullable<Byte>ToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<byte?, bool>("Nullable<Byte>ToBool", null, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<byte?, bool>("Nullable<Byte>ToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<byte?, byte>("Nullable<Byte>ToByte", null, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<byte?, byte>("Nullable<Byte>ToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, byte[]>("Nullable<Byte>ToByteArray", null, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<byte?, byte[]>("Nullable<Byte>ToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<byte?, char>("Nullable<Byte>ToChar", null, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<byte?, char>("Nullable<Byte>ToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<byte?, DateTime>("Nullable<Byte>ToDateTime", null, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<byte?, DateTime>("Nullable<Byte>ToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<byte?, DateTimeOffset>("Nullable<Byte>ToDateTimeOffset", null, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<byte?, DateTimeOffset>("Nullable<Byte>ToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<byte?, decimal>("Nullable<Byte>ToDecimal", null, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<byte?, decimal>("Nullable<Byte>ToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, double>("Nullable<Byte>ToDouble", null, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<byte?, double>("Nullable<Byte>ToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, PrimaryColor>("Nullable<Byte>ToEnum", null, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<byte?, PrimaryColor>("Nullable<Byte>ToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<byte?, float>("Nullable<Byte>ToFloat", null, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<byte?, float>("Nullable<Byte>ToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, Guid>("Nullable<Byte>ToGuid", null, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<byte?, Guid>("Nullable<Byte>ToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<byte?, int>("Nullable<Byte>ToInt", null, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<byte?, int>("Nullable<Byte>ToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, long>("Nullable<Byte>ToLong", null, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<byte?, long>("Nullable<Byte>ToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, sbyte>("Nullable<Byte>ToSByte", null, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<byte?, sbyte>("Nullable<Byte>ToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, short>("Nullable<Byte>ToShort", null, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<byte?, short>("Nullable<Byte>ToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, string>("Nullable<Byte>ToString", null, ConvertResult.Success, null),
                            new TryConvertGenericTest<byte?, string>("Nullable<Byte>ToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<byte?, TimeSpan>("Nullable<Byte>ToTimeSpan", null, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<byte?, TimeSpan>("Nullable<Byte>ToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<byte?, Type>("Nullable<Byte>ToType", null, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<byte?, Type>("Nullable<Byte>ToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<byte?, uint>("Nullable<Byte>ToUInt", null, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<byte?, uint>("Nullable<Byte>ToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, ulong>("Nullable<Byte>ToULong", null, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<byte?, ulong>("Nullable<Byte>ToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, ushort>("Nullable<Byte>ToUShort", null, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<byte?, ushort>("Nullable<Byte>ToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte?, Uri>("Nullable<Byte>ToUri", null, ConvertResult.Failure, default(Uri)),
                            new TryConvertGenericTest<byte?, Uri>("Nullable<Byte>ToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            //new TryConvertGenericTest<byte?, bool?>("Nullable<Byte>ToNullable<Bool>", 42, ConvertResult.Success, true),
                            //new TryConvertGenericTest<byte?, byte?>("Nullable<Byte>ToNullable<Byte>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, char?>("Nullable<Byte>ToNullable<Char>", 42, ConvertResult.Success, '*'),
                            //new TryConvertGenericTest<byte?, DateTime?>("Nullable<Byte>ToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            //new TryConvertGenericTest<byte?, DateTimeOffset?>("Nullable<Byte>ToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            //new TryConvertGenericTest<byte?, decimal?>("Nullable<Byte>ToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, double?>("Nullable<Byte>ToNullable<Double>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, PrimaryColor?>("Nullable<Byte>ToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            //new TryConvertGenericTest<byte?, float?>("Nullable<Byte>ToNullable<Float>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, Guid?>("Nullable<Byte>ToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            //new TryConvertGenericTest<byte?, int?>("Nullable<Byte>ToNullable<Int>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, long?>("Nullable<Byte>ToNullable<Long>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, sbyte?>("Nullable<Byte>ToNullable<SByte>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, short?>("Nullable<Byte>ToNullable<Short>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, TimeSpan?>("Nullable<Byte>ToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            //new TryConvertGenericTest<byte?, uint?>("Nullable<Byte>ToNullable<UInt>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, ulong?>("Nullable<Byte>ToNullable<ULong>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, ushort?>("Nullable<Byte>ToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            //new TryConvertGenericTest<byte?, IInterface>("Nullable<Byte>ToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            //new TryConvertGenericTest<byte?, BaseClass>("Nullable<Byte>ToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            //new TryConvertGenericTest<byte?, DerivedClass>("Nullable<Byte>ToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                // Interface/Class Types ////////////////////////////////////
            };

        public static readonly IEnumerable<object[]> TryConvertGenericTestData2 = new[]
            {
                // Simple Types /////////////////////////////////////////////

                #region BoolToXXX
                new object []
                {
                    "BoolToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<bool, bool>("BoolToBool", true, ConvertResult.Success, true),
                            new TryConvertGenericTest<bool, byte>("BoolToByte", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, byte[]>("BoolToByteArray", true, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<bool, char>("BoolToChar", true, ConvertResult.Success, (char)1),
                            new TryConvertGenericTest<bool, DateTime>("BoolToDateTime", true, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<bool, DateTimeOffset>("BoolToDateTimeOffset", true, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<bool, decimal>("BoolToDecimal", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, double>("BoolToDouble", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, PrimaryColor>("BoolToEnum", true, ConvertResult.Success, PrimaryColor.Green),
                            new TryConvertGenericTest<bool, float>("BoolToFloat", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, Guid>("BoolToGuid", true, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<bool, int>("BoolToInt", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, long>("BoolToLong", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, sbyte>("BoolToSByte", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, short>("BoolToShort", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, string>("BoolToString", false, ConvertResult.Success, "False"),
                            new TryConvertGenericTest<bool, string>("BoolToString", true, ConvertResult.Success, "True"),
                            new TryConvertGenericTest<bool, TimeSpan>("BoolToTimeSpan", true, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<bool, Type>("BoolToType", true, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<bool, uint>("BoolToUInt", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, ulong>("BoolToULong", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, ushort>("BoolToUShort", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, Uri>("BoolToUri", true, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<bool, bool?>("BoolToNullable<Bool>", true, ConvertResult.Success, true),
                            new TryConvertGenericTest<bool, byte?>("BoolToNullable<Byte>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, char?>("BoolToNullable<Char>", true, ConvertResult.Success, (char)1),
                            new TryConvertGenericTest<bool, DateTime?>("BoolToNullable<DateTime>", true, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<bool, DateTimeOffset?>("BoolToNullable<DateTimeOffset>", true, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<bool, decimal?>("BoolToNullable<Decimal>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, double?>("BoolToNullable<Double>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, PrimaryColor?>("BoolToNullable<Enum>", true, ConvertResult.Success, PrimaryColor.Green),
                            new TryConvertGenericTest<bool, float?>("BoolToNullable<Float>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, Guid?>("BoolToNullable<Guid>", true, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<bool, int?>("BoolToNullable<Int>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, long?>("BoolToNullable<Long>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, sbyte?>("BoolToNullable<SByte>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, short?>("BoolToNullable<Short>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, TimeSpan?>("BoolToNullable<TimeSpan>", true, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<bool, uint?>("BoolToNullable<UInt>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, ulong?>("BoolToNullable<ULong>", true, ConvertResult.Success, 1),
                            new TryConvertGenericTest<bool, ushort?>("BoolToNullable<UShort>", true, ConvertResult.Success, 1),

                            // Interface/Class Types
                            new TryConvertGenericTest<bool, IInterface>("BoolToInterface", true, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<bool, BaseClass>("BoolToBaseClass", true, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<bool, DerivedClass>("BoolToDerivedClass", true, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region ByteToXXX
                new object []
                {
                    "ByteToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<byte, bool>("ByteToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<byte, byte>("ByteToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, byte[]>("ByteToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<byte, char>("ByteToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<byte, DateTime>("ByteToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<byte, DateTimeOffset>("ByteToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<byte, decimal>("ByteToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, double>("ByteToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, PrimaryColor>("ByteToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<byte, float>("ByteToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, Guid>("ByteToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<byte, int>("ByteToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, long>("ByteToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, sbyte>("ByteToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, short>("ByteToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, string>("ByteToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<byte, TimeSpan>("ByteToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<byte, Type>("ByteToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<byte, uint>("ByteToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, ulong>("ByteToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, ushort>("ByteToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, Uri>("ByteToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<byte, bool?>("ByteToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<byte, byte?>("ByteToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, char?>("ByteToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<byte, DateTime?>("ByteToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<byte, DateTimeOffset?>("ByteToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<byte, decimal?>("ByteToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, double?>("ByteToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, PrimaryColor?>("ByteToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<byte, float?>("ByteToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, Guid?>("ByteToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<byte, int?>("ByteToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, long?>("ByteToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, sbyte?>("ByteToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, short?>("ByteToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, TimeSpan?>("ByteToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<byte, uint?>("ByteToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, ulong?>("ByteToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<byte, ushort?>("ByteToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<byte, IInterface>("ByteToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<byte, BaseClass>("ByteToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<byte, DerivedClass>("ByteToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region ByteArrayToXXX
                new object []
                {
                    "ByteArrayToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<byte[], bool>("ByteArrayToBool", TestByteArray, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<byte[], byte>("ByteArrayToByte", TestByteArray, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<byte[], byte[]>("ByteArrayToByteArray", TestByteArray, ConvertResult.Success, TestByteArray),
                            new TryConvertGenericTest<byte[], char>("ByteArrayToChar", TestByteArray, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<byte[], DateTime>("ByteArrayToDateTime", TestByteArray, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<byte[], DateTimeOffset>("ByteArrayToDateTimeOffset", TestByteArray, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<byte[], decimal>("ByteArrayToDecimal", TestByteArray, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<byte[], double>("ByteArrayToDouble", TestByteArray, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<byte[], PrimaryColor>("ByteArrayToEnum", TestByteArray, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<byte[], float>("ByteArrayToFloat", TestByteArray, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<byte[], Guid>("ByteArrayToGuid", TestGuidByteArray, ConvertResult.Success, TestGuid),
                            new TryConvertGenericTest<byte[], int>("ByteArrayToInt", TestByteArray, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<byte[], long>("ByteArrayToLong", TestByteArray, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<byte[], sbyte>("ByteArrayToSByte", TestByteArray, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<byte[], short>("ByteArrayToShort", TestByteArray, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<byte[], string>("ByteArrayToString", TestByteArray, ConvertResult.Success, TestByteArrayString),
                            new TryConvertGenericTest<byte[], TimeSpan>("ByteArrayToTimeSpan", TestByteArray, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<byte[], Type>("ByteArrayToType", TestByteArray, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<byte[], uint>("ByteArrayToUInt", TestByteArray, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<byte[], ulong>("ByteArrayToULong", TestByteArray, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<byte[], ushort>("ByteArrayToUShort", TestByteArray, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<byte[], Uri>("ByteArrayToUri", TestByteArray, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<byte[], bool?>("ByteArrayToNullable<Bool>", TestByteArray, ConvertResult.Failure, default(bool?)),
                            new TryConvertGenericTest<byte[], byte?>("ByteArrayToNullable<Byte>", TestByteArray, ConvertResult.Failure, default(byte?)),
                            new TryConvertGenericTest<byte[], char?>("ByteArrayToNullable<Char>", TestByteArray, ConvertResult.Failure, default(char?)),
                            new TryConvertGenericTest<byte[], DateTime?>("ByteArrayToNullable<DateTime>", TestByteArray, ConvertResult.Failure, new DateTime?()),
                            new TryConvertGenericTest<byte[], DateTimeOffset?>("ByteArrayToNullable<DateTimeOffset>", TestByteArray, ConvertResult.Failure, new DateTimeOffset?()),
                            new TryConvertGenericTest<byte[], decimal?>("ByteArrayToNullable<Decimal>", TestByteArray, ConvertResult.Failure, default(decimal?)),
                            new TryConvertGenericTest<byte[], double?>("ByteArrayToNullable<Double>", TestByteArray, ConvertResult.Failure, default(double?)),
                            new TryConvertGenericTest<byte[], PrimaryColor?>("ByteArrayToNullable<Enum>", TestByteArray, ConvertResult.Failure, default(PrimaryColor?)),
                            new TryConvertGenericTest<byte[], float?>("ByteArrayToNullable<Float>", TestByteArray, ConvertResult.Failure, default(float?)),
                            new TryConvertGenericTest<byte[], Guid?>("ByteArrayToNullable<Guid>", TestGuidByteArray, ConvertResult.Success, TestGuid),
                            new TryConvertGenericTest<byte[], int?>("ByteArrayToNullable<Int>", TestByteArray, ConvertResult.Failure, default(int?)),
                            new TryConvertGenericTest<byte[], long?>("ByteArrayToNullable<Long>", TestByteArray, ConvertResult.Failure, default(long?)),
                            new TryConvertGenericTest<byte[], sbyte?>("ByteArrayToNullable<SByte>", TestByteArray, ConvertResult.Failure, default(sbyte?)),
                            new TryConvertGenericTest<byte[], short?>("ByteArrayToNullable<Short>", TestByteArray, ConvertResult.Failure, default(short?)),
                            new TryConvertGenericTest<byte[], TimeSpan?>("ByteArrayToNullable<TimeSpan>", TestByteArray, ConvertResult.Failure, new TimeSpan?()),
                            new TryConvertGenericTest<byte[], uint?>("ByteArrayToNullable<UInt>", TestByteArray, ConvertResult.Failure, default(uint?)),
                            new TryConvertGenericTest<byte[], ulong?>("ByteArrayToNullable<ULong>", TestByteArray, ConvertResult.Failure, default(ulong?)),
                            new TryConvertGenericTest<byte[], ushort?>("ByteArrayToNullable<UShort>", TestByteArray, ConvertResult.Failure, default(ushort?)),

                            // Interface/Class Types
                            new TryConvertGenericTest<byte[], IInterface>("ByteArrayToInterface", TestByteArray, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<byte[], BaseClass>("ByteArrayToBaseClass", TestByteArray, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<byte[], DerivedClass>("ByteArrayToDerivedClass", TestByteArray, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region CharToXXX
                new object []
                {
                    "CharToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<char, bool>("CharToBool", '*', ConvertResult.Success, true),
                            new TryConvertGenericTest<char, byte>("CharToByte", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, byte[]>("CharToByteArray", '*', ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<char, char>("CharToChar", '*', ConvertResult.Success, '*'),
                            new TryConvertGenericTest<char, DateTime>("CharToDateTime", '*', ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<char, DateTimeOffset>("CharToDateTimeOffset", '*', ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<char, decimal>("CharToDecimal", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, double>("CharToDouble", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, PrimaryColor>("CharToEnum", '*', ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<char, float>("CharToFloat", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, Guid>("CharToGuid", '*', ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<char, int>("CharToInt", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, long>("CharToLong", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, sbyte>("CharToSByte", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, short>("CharToShort", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, string>("CharToString", '*', ConvertResult.Success, "*"),
                            new TryConvertGenericTest<char, TimeSpan>("CharToTimeSpan", '*', ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<char, Type>("CharToType", '*', ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<char, uint>("CharToUInt", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, ulong>("CharToULong", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, ushort>("CharToUShort", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, Uri>("CharToUri", '*', ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<char, bool?>("CharToNullable<Bool>", '*', ConvertResult.Success, true),
                            new TryConvertGenericTest<char, byte?>("CharToNullable<Byte>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, char?>("CharToNullable<Char>", '*', ConvertResult.Success, '*'),
                            new TryConvertGenericTest<char, DateTime?>("CharToNullable<DateTime>", '*', ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<char, DateTimeOffset?>("CharToNullable<DateTimeOffset>", '*', ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<char, decimal?>("CharToNullable<Decimal>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, double?>("CharToNullable<Double>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, PrimaryColor?>("CharToNullable<Enum>", '*', ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<char, float?>("CharToNullable<Float>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, Guid?>("CharToNullable<Guid>", '*', ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<char, int?>("CharToNullable<Int>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, long?>("CharToNullable<Long>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, sbyte?>("CharToNullable<SByte>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, short?>("CharToNullable<Short>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, TimeSpan?>("CharToNullable<TimeSpan>", '*', ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<char, uint?>("CharToNullable<UInt>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, ulong?>("CharToNullable<ULong>", '*', ConvertResult.Success, 42),
                            new TryConvertGenericTest<char, ushort?>("CharToNullable<UShort>", '*', ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<char, IInterface>("CharToInterface", '*', ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<char, BaseClass>("CharToBaseClass", '*', ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<char, DerivedClass>("CharToDerivedClass", '*', ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region DateTimeToXXX
                new object []
                {
                    "DateTimeToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<DateTime, bool>("DateTimeToBool", TestDateTime, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<DateTime, byte>("DateTimeToByte", TestDateTime, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<DateTime, byte[]>("DateTimeToByteArray", TestDateTime, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<DateTime, char>("DateTimeToChar", TestDateTime, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<DateTime, DateTime>("DateTimeToDateTime", TestDateTime, ConvertResult.Success, TestDateTime),
                            new TryConvertGenericTest<DateTime, DateTimeOffset>("DateTimeToDateTimeOffset", TestDateTime, ConvertResult.Success, TestDateTimeOffset),
                            new TryConvertGenericTest<DateTime, decimal>("DateTimeToDecimal", TestDateTime, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<DateTime, double>("DateTimeToDouble", TestDateTime, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<DateTime, PrimaryColor>("DateTimeToEnum", TestDateTime, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<DateTime, float>("DateTimeToFloat", TestDateTime, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<DateTime, Guid>("DateTimeToGuid", TestDateTime, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<DateTime, int>("DateTimeToInt", TestDateTime, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<DateTime, long>("DateTimeToLong", TestDateTime, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<DateTime, sbyte>("DateTimeToSByte", TestDateTime, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<DateTime, short>("DateTimeToShort", TestDateTime, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<DateTime, string>("DateTimeToString", TestDateTime, ConvertResult.Success, TestDateTimeString),
                            new TryConvertGenericTest<DateTime, string>("DateTimeToStringWithFormat", TestDateTime, ConvertResult.Success, TestDateTimeStringWithFormat, FormatDateTimeContext),
                            new TryConvertGenericTest<DateTime, string>("DateTimeToStringWithFormatAndFormatProvider", TestDateTime, ConvertResult.Success, TestDateTimeStringWithFormatAndFormatProvider, FormatAndFormatProviderDateTimeContext),
                            new TryConvertGenericTest<DateTime, TimeSpan>("DateTimeToTimeSpan", TestDateTime, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<DateTime, Type>("DateTimeToType", TestDateTime, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<DateTime, uint>("DateTimeToUInt", TestDateTime, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<DateTime, ulong>("DateTimeToULong", TestDateTime, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<DateTime, ushort>("DateTimeToUShort", TestDateTime, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<DateTime, Uri>("DateTimeToUri", TestDateTime, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<DateTime, bool?>("DateTimeToNullable<Bool>", TestDateTime, ConvertResult.Failure, default(bool?)),
                            new TryConvertGenericTest<DateTime, byte?>("DateTimeToNullable<Byte>", TestDateTime, ConvertResult.Failure, default(byte?)),
                            new TryConvertGenericTest<DateTime, char?>("DateTimeToNullable<Char>", TestDateTime, ConvertResult.Failure, default(char?)),
                            new TryConvertGenericTest<DateTime, DateTime?>("DateTimeToNullable<DateTime>", TestDateTime, ConvertResult.Success, TestDateTime),
                            new TryConvertGenericTest<DateTime, DateTimeOffset?>("DateTimeToNullable<DateTimeOffset>", TestDateTime, ConvertResult.Success, TestDateTimeOffset),
                            new TryConvertGenericTest<DateTime, decimal?>("DateTimeToNullable<Decimal>", TestDateTime, ConvertResult.Failure, default(decimal?)),
                            new TryConvertGenericTest<DateTime, double?>("DateTimeToNullable<Double>", TestDateTime, ConvertResult.Failure, default(double?)),
                            new TryConvertGenericTest<DateTime, PrimaryColor?>("DateTimeToNullable<Enum>", TestDateTime, ConvertResult.Failure, default(PrimaryColor?)),
                            new TryConvertGenericTest<DateTime, float?>("DateTimeToNullable<Float>", TestDateTime, ConvertResult.Failure, default(float?)),
                            new TryConvertGenericTest<DateTime, Guid?>("DateTimeToNullable<Guid>", TestDateTime, ConvertResult.Failure, new Guid?()),
                            new TryConvertGenericTest<DateTime, int?>("DateTimeToNullable<Int>", TestDateTime, ConvertResult.Failure, default(int?)),
                            new TryConvertGenericTest<DateTime, long?>("DateTimeToNullable<Long>", TestDateTime, ConvertResult.Failure, default(long?)),
                            new TryConvertGenericTest<DateTime, sbyte?>("DateTimeToNullable<SByte>", TestDateTime, ConvertResult.Failure, default(sbyte?)),
                            new TryConvertGenericTest<DateTime, short?>("DateTimeToNullable<Short>", TestDateTime, ConvertResult.Failure, default(short?)),
                            new TryConvertGenericTest<DateTime, TimeSpan?>("DateTimeToNullable<TimeSpan>", TestDateTime, ConvertResult.Failure, new TimeSpan?()),
                            new TryConvertGenericTest<DateTime, uint?>("DateTimeToNullable<UInt>", TestDateTime, ConvertResult.Failure, default(uint?)),
                            new TryConvertGenericTest<DateTime, ulong?>("DateTimeToNullable<ULong>", TestDateTime, ConvertResult.Failure, default(ulong?)),
                            new TryConvertGenericTest<DateTime, ushort?>("DateTimeToNullable<UShort>", TestDateTime, ConvertResult.Failure, default(ushort?)),

                            // Interface/Class Types
                            new TryConvertGenericTest<DateTime, IInterface>("DateTimeToInterface", TestDateTime, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<DateTime, BaseClass>("DateTimeToBaseClass", TestDateTime, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<DateTime, DerivedClass>("DateTimeToDerivedClass", TestDateTime, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region DateTimeOffsetToXXX
                new object []
                {
                    "DateTimeOffsetToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<DateTimeOffset, bool>("DateTimeOffsetToBool", TestDateTimeOffset, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<DateTimeOffset, byte>("DateTimeOffsetToByte", TestDateTimeOffset, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<DateTimeOffset, byte[]>("DateTimeOffsetToByteArray", TestDateTimeOffset, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<DateTimeOffset, char>("DateTimeOffsetToChar", TestDateTimeOffset, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<DateTimeOffset, DateTime>("DateTimeOffsetToDateTime", TestDateTimeOffset, ConvertResult.Success, TestDateTime),
                            new TryConvertGenericTest<DateTimeOffset, DateTimeOffset>("DateTimeOffsetToDateTimeOffset", TestDateTimeOffset, ConvertResult.Success, TestDateTimeOffset),
                            new TryConvertGenericTest<DateTimeOffset, decimal>("DateTimeOffsetToDecimal", TestDateTimeOffset, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<DateTimeOffset, double>("DateTimeOffsetToDouble", TestDateTimeOffset, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<DateTimeOffset, PrimaryColor>("DateTimeOffsetToEnum", TestDateTimeOffset, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<DateTimeOffset, float>("DateTimeOffsetToFloat", TestDateTimeOffset, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<DateTimeOffset, Guid>("DateTimeOffsetToGuid", TestDateTimeOffset, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<DateTimeOffset, int>("DateTimeOffsetToInt", TestDateTimeOffset, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<DateTimeOffset, long>("DateTimeOffsetToLong", TestDateTimeOffset, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<DateTimeOffset, sbyte>("DateTimeOffsetToSByte", TestDateTimeOffset, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<DateTimeOffset, short>("DateTimeOffsetToShort", TestDateTimeOffset, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<DateTimeOffset, string>("DateTimeOffsetToString", TestDateTimeOffset, ConvertResult.Success, TestDateTimeOffsetString),
                            new TryConvertGenericTest<DateTimeOffset, string>("DateTimeOffsetToStringWithFormat", TestDateTimeOffset, ConvertResult.Success, TestDateTimeOffsetStringWithFormat, FormatDateTimeOffsetContext),
                            new TryConvertGenericTest<DateTimeOffset, string>("DateTimeOffsetToStringWithFormatAndFormatProvider", TestDateTimeOffset, ConvertResult.Success, TestDateTimeOffsetStringWithFormatAndFormatProvider, FormatAndFormatProviderDateTimeOffsetContext),
                            new TryConvertGenericTest<DateTimeOffset, TimeSpan>("DateTimeOffsetToTimeSpan", TestDateTimeOffset, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<DateTimeOffset, Type>("DateTimeOffsetToType", TestDateTimeOffset, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<DateTimeOffset, uint>("DateTimeOffsetToUInt", TestDateTimeOffset, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<DateTimeOffset, ulong>("DateTimeOffsetToULong", TestDateTimeOffset, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<DateTimeOffset, ushort>("DateTimeOffsetToUShort", TestDateTimeOffset, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<DateTimeOffset, Uri>("DateTimeOffsetToUri", TestDateTimeOffset, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<DateTimeOffset, bool?>("DateTimeOffsetToNullable<Bool>", TestDateTimeOffset, ConvertResult.Failure, default(bool?)),
                            new TryConvertGenericTest<DateTimeOffset, byte?>("DateTimeOffsetToNullable<Byte>", TestDateTimeOffset, ConvertResult.Failure, default(byte?)),
                            new TryConvertGenericTest<DateTimeOffset, char?>("DateTimeOffsetToNullable<Char>", TestDateTimeOffset, ConvertResult.Failure, default(char?)),
                            new TryConvertGenericTest<DateTimeOffset, DateTime?>("DateTimeOffsetToNullable<DateTime>", TestDateTimeOffset, ConvertResult.Success, TestDateTime),
                            new TryConvertGenericTest<DateTimeOffset, DateTimeOffset?>("DateTimeOffsetToNullable<DateTimeOffset>", TestDateTimeOffset, ConvertResult.Success, TestDateTimeOffset),
                            new TryConvertGenericTest<DateTimeOffset, decimal?>("DateTimeOffsetToNullable<Decimal>", TestDateTimeOffset, ConvertResult.Failure, default(decimal?)),
                            new TryConvertGenericTest<DateTimeOffset, double?>("DateTimeOffsetToNullable<Double>", TestDateTimeOffset, ConvertResult.Failure, default(double?)),
                            new TryConvertGenericTest<DateTimeOffset, PrimaryColor?>("DateTimeOffsetToNullable<Enum>", TestDateTimeOffset, ConvertResult.Failure, default(PrimaryColor?)),
                            new TryConvertGenericTest<DateTimeOffset, float?>("DateTimeOffsetToNullable<Float>", TestDateTimeOffset, ConvertResult.Failure, default(float?)),
                            new TryConvertGenericTest<DateTimeOffset, Guid?>("DateTimeOffsetToNullable<Guid>", TestDateTimeOffset, ConvertResult.Failure, new Guid?()),
                            new TryConvertGenericTest<DateTimeOffset, int?>("DateTimeOffsetToNullable<Int>", TestDateTimeOffset, ConvertResult.Failure, default(int?)),
                            new TryConvertGenericTest<DateTimeOffset, long?>("DateTimeOffsetToNullable<Long>", TestDateTimeOffset, ConvertResult.Failure, default(long?)),
                            new TryConvertGenericTest<DateTimeOffset, sbyte?>("DateTimeOffsetToNullable<SByte>", TestDateTimeOffset, ConvertResult.Failure, default(sbyte?)),
                            new TryConvertGenericTest<DateTimeOffset, short?>("DateTimeOffsetToNullable<Short>", TestDateTimeOffset, ConvertResult.Failure, default(short?)),
                            new TryConvertGenericTest<DateTimeOffset, TimeSpan?>("DateTimeOffsetToNullable<TimeSpan>", TestDateTimeOffset, ConvertResult.Failure, new TimeSpan?()),
                            new TryConvertGenericTest<DateTimeOffset, uint?>("DateTimeOffsetToNullable<UInt>", TestDateTimeOffset, ConvertResult.Failure, default(uint?)),
                            new TryConvertGenericTest<DateTimeOffset, ulong?>("DateTimeOffsetToNullable<ULong>", TestDateTimeOffset, ConvertResult.Failure, default(ulong?)),
                            new TryConvertGenericTest<DateTimeOffset, ushort?>("DateTimeOffsetToNullable<UShort>", TestDateTimeOffset, ConvertResult.Failure, default(ushort?)),

                            // Interface/Class Types
                            new TryConvertGenericTest<DateTimeOffset, IInterface>("DateTimeOffsetToInterface", TestDateTimeOffset, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<DateTimeOffset, BaseClass>("DateTimeOffsetToBaseClass", TestDateTimeOffset, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<DateTimeOffset, DerivedClass>("DateTimeOffsetToDerivedClass", TestDateTimeOffset, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region DecimalToXXX
                new object []
                {
                    "DecimalToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<decimal, bool>("DecimalToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<decimal, byte>("DecimalToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, byte[]>("DecimalToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<decimal, char>("DecimalToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<decimal, DateTime>("DecimalToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<decimal, DateTimeOffset>("DecimalToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<decimal, decimal>("DecimalToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, double>("DecimalToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, PrimaryColor>("DecimalToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<decimal, float>("DecimalToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, Guid>("DecimalToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<decimal, int>("DecimalToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, long>("DecimalToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, sbyte>("DecimalToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, short>("DecimalToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, string>("DecimalToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<decimal, TimeSpan>("DecimalToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<decimal, Type>("DecimalToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<decimal, uint>("DecimalToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, ulong>("DecimalToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, ushort>("DecimalToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, Uri>("DecimalToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<decimal, bool?>("DecimalToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<decimal, byte?>("DecimalToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, char?>("DecimalToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<decimal, DateTime?>("DecimalToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<decimal, DateTimeOffset?>("DecimalToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<decimal, decimal?>("DecimalToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, double?>("DecimalToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, PrimaryColor?>("DecimalToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<decimal, float?>("DecimalToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, Guid?>("DecimalToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<decimal, int?>("DecimalToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, long?>("DecimalToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, sbyte?>("DecimalToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, short?>("DecimalToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, TimeSpan?>("DecimalToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<decimal, uint?>("DecimalToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, ulong?>("DecimalToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<decimal, ushort?>("DecimalToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<decimal, IInterface>("DecimalToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<decimal, BaseClass>("DecimalToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<decimal, DerivedClass>("DecimalToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region DoubleToXXX
                new object []
                {
                    "DoubleToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<double, bool>("DoubleToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<double, byte>("DoubleToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, byte[]>("DoubleToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<double, char>("DoubleToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<double, DateTime>("DoubleToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<double, DateTimeOffset>("DoubleToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<double, decimal>("DoubleToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, double>("DoubleToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, PrimaryColor>("DoubleToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<double, float>("DoubleToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, Guid>("DoubleToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<double, int>("DoubleToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, long>("DoubleToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, sbyte>("DoubleToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, short>("DoubleToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, string>("DoubleToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<double, TimeSpan>("DoubleToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<double, Type>("DoubleToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<double, uint>("DoubleToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, ulong>("DoubleToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, ushort>("DoubleToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, Uri>("DoubleToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<double, bool?>("DoubleToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<double, byte?>("DoubleToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, char?>("DoubleToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<double, DateTime?>("DoubleToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<double, DateTimeOffset?>("DoubleToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<double, decimal?>("DoubleToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, double?>("DoubleToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, PrimaryColor?>("DoubleToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<double, float?>("DoubleToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, Guid?>("DoubleToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<double, int?>("DoubleToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, long?>("DoubleToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, sbyte?>("DoubleToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, short?>("DoubleToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, TimeSpan?>("DoubleToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<double, uint?>("DoubleToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, ulong?>("DoubleToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<double, ushort?>("DoubleToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<double, IInterface>("DoubleToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<double, BaseClass>("DoubleToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<double, DerivedClass>("DoubleToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region EnumToXXX
                new object []
                {
                    "EnumToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<PrimaryColor, bool>("EnumToBool", PrimaryColor.Red, ConvertResult.Success, false),
                            new TryConvertGenericTest<PrimaryColor, bool>("EnumToBool", PrimaryColor.Blue, ConvertResult.Success, true),
                            new TryConvertGenericTest<PrimaryColor, byte>("EnumToByte", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, byte[]>("EnumToByteArray", PrimaryColor.Blue, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<PrimaryColor, char>("EnumToChar", PrimaryColor.Blue, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<PrimaryColor, DateTime>("EnumToDateTime", PrimaryColor.Blue, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<PrimaryColor, DateTimeOffset>("EnumToDateTimeOffset", PrimaryColor.Blue, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<PrimaryColor, decimal>("EnumToDecimal", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, double>("EnumToDouble", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, PrimaryColor>("EnumToEnum", PrimaryColor.Blue, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<PrimaryColor, float>("EnumToFloat", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, Guid>("EnumToGuid", PrimaryColor.Blue, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<PrimaryColor, int>("EnumToInt", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, long>("EnumToLong", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, sbyte>("EnumToSByte", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, short>("EnumToShort", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, string>("EnumToString", PrimaryColor.Blue, ConvertResult.Success, TestBlueString),
                            new TryConvertGenericTest<PrimaryColor, string>("EnumToStringWithFormat", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinalAsString, FormatEnumContext),
                            new TryConvertGenericTest<PrimaryColor, TimeSpan>("EnumToTimeSpan", PrimaryColor.Blue, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<PrimaryColor, Type>("EnumToType", PrimaryColor.Blue, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<PrimaryColor, uint>("EnumToUInt", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, ulong>("EnumToULong", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, ushort>("EnumToUShort", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, Uri>("EnumToUri", PrimaryColor.Blue, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<PrimaryColor, bool?>("EnumToNullable<Bool>", PrimaryColor.Red, ConvertResult.Success, false),
                            new TryConvertGenericTest<PrimaryColor, bool?>("EnumToNullable<Bool>", PrimaryColor.Blue, ConvertResult.Success, true),
                            new TryConvertGenericTest<PrimaryColor, byte?>("EnumToNullable<Byte>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, char?>("EnumToNullable<Char>", PrimaryColor.Blue, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<PrimaryColor, DateTime?>("EnumToNullable<DateTime>", PrimaryColor.Blue, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<PrimaryColor, DateTimeOffset?>("EnumToNullable<DateTimeOffset>", PrimaryColor.Blue, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<PrimaryColor, decimal?>("EnumToNullable<Decimal>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, double?>("EnumToNullable<Double>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, PrimaryColor?>("EnumToNullable<Enum>", PrimaryColor.Blue, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<PrimaryColor, float?>("EnumToNullable<Float>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, Guid?>("EnumToNullable<Guid>", PrimaryColor.Blue, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<PrimaryColor, int?>("EnumToNullable<Int>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, long?>("EnumToNullable<Long>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, sbyte?>("EnumToNullable<SByte>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, short?>("EnumToNullable<Short>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, TimeSpan?>("EnumToNullable<TimeSpan>", PrimaryColor.Blue, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<PrimaryColor, uint?>("EnumToNullable<UInt>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, ulong?>("EnumToNullable<ULong>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),
                            new TryConvertGenericTest<PrimaryColor, ushort?>("EnumToNullable<UShort>", PrimaryColor.Blue, ConvertResult.Success, TestBlueOrdinal),

                            // Interface/Class Types
                            new TryConvertGenericTest<PrimaryColor, IInterface>("EnumToInterface", PrimaryColor.Blue, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<PrimaryColor, BaseClass>("EnumToBaseClass", PrimaryColor.Blue, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<PrimaryColor, DerivedClass>("EnumToDerivedClass", PrimaryColor.Blue, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region FloatToXXX
                new object []
                {
                    "FloatToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<float, bool>("FloatToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<float, byte>("FloatToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, byte[]>("FloatToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<float, char>("FloatToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<float, DateTime>("FloatToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<float, DateTimeOffset>("FloatToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<float, decimal>("FloatToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, double>("FloatToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, PrimaryColor>("FloatToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<float, float>("FloatToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, Guid>("FloatToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<float, int>("FloatToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, long>("FloatToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, sbyte>("FloatToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, short>("FloatToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, string>("FloatToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<float, TimeSpan>("FloatToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<float, Type>("FloatToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<float, uint>("FloatToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, ulong>("FloatToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, ushort>("FloatToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, Uri>("FloatToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<float, bool?>("FloatToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<float, byte?>("FloatToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, char?>("FloatToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<float, DateTime?>("FloatToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<float, DateTimeOffset?>("FloatToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<float, decimal?>("FloatToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, double?>("FloatToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, PrimaryColor?>("FloatToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<float, float?>("FloatToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, Guid?>("FloatToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<float, int?>("FloatToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, long?>("FloatToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, sbyte?>("FloatToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, short?>("FloatToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, TimeSpan?>("FloatToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<float, uint?>("FloatToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, ulong?>("FloatToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<float, ushort?>("FloatToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<float, IInterface>("FloatToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<float, BaseClass>("FloatToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<float, DerivedClass>("FloatToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region GuidToXXX
                new object []
                {
                    "GuidToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<Guid, bool>("GuidToBool", TestGuid, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<Guid, byte>("GuidToByte", TestGuid, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<Guid, byte[]>("GuidToByteArray", TestGuid, ConvertResult.Success, TestGuidByteArray),
                            new TryConvertGenericTest<Guid, char>("GuidToChar", TestGuid, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<Guid, DateTime>("GuidToDateTime", TestGuid, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<Guid, DateTimeOffset>("GuidToDateTimeOffset", TestGuid, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<Guid, decimal>("GuidToDecimal", TestGuid, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<Guid, double>("GuidToDouble", TestGuid, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<Guid, PrimaryColor>("GuidToEnum", TestGuid, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<Guid, float>("GuidToFloat", TestGuid, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<Guid, Guid>("GuidToGuid", TestGuid, ConvertResult.Success, TestGuid),
                            new TryConvertGenericTest<Guid, int>("GuidToInt", TestGuid, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<Guid, long>("GuidToLong", TestGuid, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<Guid, sbyte>("GuidToSByte", TestGuid, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<Guid, short>("GuidToShort", TestGuid, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<Guid, string>("GuidToString", TestGuid, ConvertResult.Success, TestGuidString),
                            new TryConvertGenericTest<Guid, TimeSpan>("GuidToTimeSpan", TestGuid, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<Guid, Type>("GuidToType", TestGuid, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<Guid, uint>("GuidToUInt", TestGuid, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<Guid, ulong>("GuidToULong", TestGuid, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<Guid, ushort>("GuidToUShort", TestGuid, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<Guid, Uri>("GuidToUri", TestGuid, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<Guid, bool?>("GuidToNullable<Bool>", TestGuid, ConvertResult.Failure, default(bool?)),
                            new TryConvertGenericTest<Guid, byte?>("GuidToNullable<Byte>", TestGuid, ConvertResult.Failure, default(byte?)),
                            new TryConvertGenericTest<Guid, char?>("GuidToNullable<Char>", TestGuid, ConvertResult.Failure, default(char?)),
                            new TryConvertGenericTest<Guid, DateTime?>("GuidToNullable<DateTime>", TestGuid, ConvertResult.Failure, new DateTime?()),
                            new TryConvertGenericTest<Guid, DateTimeOffset?>("GuidToNullable<DateTimeOffset>", TestGuid, ConvertResult.Failure, new DateTimeOffset?()),
                            new TryConvertGenericTest<Guid, decimal?>("GuidToNullable<Decimal>", TestGuid, ConvertResult.Failure, default(decimal?)),
                            new TryConvertGenericTest<Guid, double?>("GuidToNullable<Double>", TestGuid, ConvertResult.Failure, default(double?)),
                            new TryConvertGenericTest<Guid, PrimaryColor?>("GuidToNullable<Enum>", TestGuid, ConvertResult.Failure, default(PrimaryColor?)),
                            new TryConvertGenericTest<Guid, float?>("GuidToNullable<Float>", TestGuid, ConvertResult.Failure, default(float?)),
                            new TryConvertGenericTest<Guid, Guid?>("GuidToNullable<Guid>", TestGuid, ConvertResult.Success, TestGuid),
                            new TryConvertGenericTest<Guid, int?>("GuidToNullable<Int>", TestGuid, ConvertResult.Failure, default(int?)),
                            new TryConvertGenericTest<Guid, long?>("GuidToNullable<Long>", TestGuid, ConvertResult.Failure, default(long?)),
                            new TryConvertGenericTest<Guid, sbyte?>("GuidToNullable<SByte>", TestGuid, ConvertResult.Failure, default(sbyte?)),
                            new TryConvertGenericTest<Guid, short?>("GuidToNullable<Short>", TestGuid, ConvertResult.Failure, default(short?)),
                            new TryConvertGenericTest<Guid, TimeSpan?>("GuidToNullable<TimeSpan>", TestGuid, ConvertResult.Failure, new TimeSpan?()),
                            new TryConvertGenericTest<Guid, uint?>("GuidToNullable<UInt>", TestGuid, ConvertResult.Failure, default(uint?)),
                            new TryConvertGenericTest<Guid, ulong?>("GuidToNullable<ULong>", TestGuid, ConvertResult.Failure, default(ulong?)),
                            new TryConvertGenericTest<Guid, ushort?>("GuidToNullable<UShort>", TestGuid, ConvertResult.Failure, default(ushort?)),

                            // Interface/Class Types
                            new TryConvertGenericTest<Guid, IInterface>("GuidToInterface", TestGuid, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<Guid, BaseClass>("GuidToBaseClass", TestGuid, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<Guid, DerivedClass>("GuidToDerivedClass", TestGuid, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region IntToXXX
                new object []
                {
                    "IntToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<int, bool>("IntToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<int, byte>("IntToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, byte[]>("IntToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<int, char>("IntToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<int, DateTime>("IntToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<int, DateTimeOffset>("IntToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<int, decimal>("IntToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, double>("IntToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, PrimaryColor>("IntToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<int, float>("IntToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, Guid>("IntToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<int, int>("IntToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, long>("IntToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, sbyte>("IntToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, short>("IntToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, string>("IntToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<int, TimeSpan>("IntToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<int, Type>("IntToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<int, uint>("IntToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, ulong>("IntToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, ushort>("IntToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, Uri>("IntToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<int, bool?>("IntToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<int, byte?>("IntToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, char?>("IntToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<int, DateTime?>("IntToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<int, DateTimeOffset?>("IntToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<int, decimal?>("IntToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, double?>("IntToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, PrimaryColor?>("IntToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<int, float?>("IntToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, Guid?>("IntToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<int, int?>("IntToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, long?>("IntToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, sbyte?>("IntToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, short?>("IntToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, TimeSpan?>("IntToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<int, uint?>("IntToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, ulong?>("IntToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<int, ushort?>("IntToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<int, IInterface>("IntToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<int, BaseClass>("IntToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<int, DerivedClass>("IntToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region LongToXXX
                new object []
                {
                    "LongToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<long, bool>("LongToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<long, byte>("LongToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, byte[]>("LongToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<long, char>("LongToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<long, DateTime>("LongToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<long, DateTimeOffset>("LongToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<long, decimal>("LongToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, double>("LongToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, PrimaryColor>("LongToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<long, float>("LongToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, Guid>("LongToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<long, int>("LongToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, long>("LongToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, sbyte>("LongToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, short>("LongToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, string>("LongToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<long, TimeSpan>("LongToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<long, Type>("LongToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<long, uint>("LongToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, ulong>("LongToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, ushort>("LongToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, Uri>("LongToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<long, bool?>("LongToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<long, byte?>("LongToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, char?>("LongToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<long, DateTime?>("LongToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<long, DateTimeOffset?>("LongToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<long, decimal?>("LongToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, double?>("LongToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, PrimaryColor?>("LongToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<long, float?>("LongToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, Guid?>("LongToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<long, int?>("LongToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, long?>("LongToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, sbyte?>("LongToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, short?>("LongToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, TimeSpan?>("LongToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<long, uint?>("LongToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, ulong?>("LongToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<long, ushort?>("LongToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<long, IInterface>("LongToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<long, BaseClass>("LongToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<long, DerivedClass>("LongToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region SByteToXXX
                new object []
                {
                    "SByteToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<sbyte, bool>("SByteToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<sbyte, byte>("SByteToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, byte[]>("SByteToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<sbyte, char>("SByteToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<sbyte, DateTime>("SByteToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<sbyte, DateTimeOffset>("SByteToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<sbyte, decimal>("SByteToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, double>("SByteToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, PrimaryColor>("SByteToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<sbyte, float>("SByteToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, Guid>("SByteToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<sbyte, int>("SByteToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, long>("SByteToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, sbyte>("SByteToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, short>("SByteToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, string>("SByteToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<sbyte, TimeSpan>("SByteToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<sbyte, Type>("SByteToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<sbyte, uint>("SByteToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, ulong>("SByteToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, ushort>("SByteToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, Uri>("SByteToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<sbyte, bool?>("SByteToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<sbyte, byte?>("SByteToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, char?>("SByteToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<sbyte, DateTime?>("SByteToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<sbyte, DateTimeOffset?>("SByteToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<sbyte, decimal?>("SByteToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, double?>("SByteToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, PrimaryColor?>("SByteToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<sbyte, float?>("SByteToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, Guid?>("SByteToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<sbyte, int?>("SByteToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, long?>("SByteToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, sbyte?>("SByteToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, short?>("SByteToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, TimeSpan?>("SByteToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<sbyte, uint?>("SByteToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, ulong?>("SByteToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<sbyte, ushort?>("SByteToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<sbyte, IInterface>("SByteToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<sbyte, BaseClass>("SByteToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<sbyte, DerivedClass>("SByteToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region ShortToXXX
                new object []
                {
                    "ShortToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<short, bool>("ShortToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<short, byte>("ShortToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, byte[]>("ShortToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<short, char>("ShortToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<short, DateTime>("ShortToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<short, DateTimeOffset>("ShortToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<short, decimal>("ShortToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, double>("ShortToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, PrimaryColor>("ShortToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<short, float>("ShortToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, Guid>("ShortToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<short, int>("ShortToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, long>("ShortToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, sbyte>("ShortToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, short>("ShortToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, string>("ShortToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<short, TimeSpan>("ShortToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<short, Type>("ShortToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<short, uint>("ShortToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, ulong>("ShortToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, ushort>("ShortToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, Uri>("ShortToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<short, bool?>("ShortToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<short, byte?>("ShortToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, char?>("ShortToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<short, DateTime?>("ShortToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<short, DateTimeOffset?>("ShortToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<short, decimal?>("ShortToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, double?>("ShortToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, PrimaryColor?>("ShortToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<short, float?>("ShortToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, Guid?>("ShortToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<short, int?>("ShortToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, long?>("ShortToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, sbyte?>("ShortToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, short?>("ShortToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, TimeSpan?>("ShortToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<short, uint?>("ShortToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, ulong?>("ShortToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<short, ushort?>("ShortToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<short, IInterface>("ShortToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<short, BaseClass>("ShortToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<short, DerivedClass>("ShortToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region StringToXXX
                new object []
                {
                    "StringToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<string, bool>("StringToBool", "False", ConvertResult.Success, false),
                            new TryConvertGenericTest<string, bool>("StringToBool", "True", ConvertResult.Success, true),
                            new TryConvertGenericTest<string, byte>("StringToByte", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, byte[]>("StringToByteArray", TestByteArrayString, ConvertResult.Success, TestByteArray),
                            new TryConvertGenericTest<string, char>("StringToChar", "*", ConvertResult.Success, '*'),
                            new TryConvertGenericTest<string, DateTime>("StringToDateTime", TestDateTimeString, ConvertResult.Success, TestDateTime),
                            new TryConvertGenericTest<string, DateTime>("StringToDateTimeWithFormat", TestDateTimeStringWithFormat, ConvertResult.Success, TestDateTime, FormatDateTimeContext),
                            new TryConvertGenericTest<string, DateTime>("StringToDateTimeWithFormatAndFormatProvider", TestDateTimeStringWithFormatAndFormatProvider, ConvertResult.Success, TestDateTime, FormatAndFormatProviderDateTimeContext),
                            new TryConvertGenericTest<string, DateTimeOffset>("StringToDateTimeOffset", TestDateTimeOffsetString, ConvertResult.Success, TestDateTimeOffset),
                            new TryConvertGenericTest<string, DateTimeOffset>("StringToDateTimeOffsetWithFormat", TestDateTimeOffsetStringWithFormat, ConvertResult.Success, TestDateTimeOffset, FormatDateTimeOffsetContext),
                            new TryConvertGenericTest<string, DateTimeOffset>("StringToDateTimeOffsetWithFormatAndFormatProvider", TestDateTimeOffsetStringWithFormatAndFormatProvider, ConvertResult.Success, TestDateTimeOffset, FormatAndFormatProviderDateTimeOffsetContext),
                            new TryConvertGenericTest<string, decimal>("StringToDecimal", "42.1", ConvertResult.Success, 42.1m),
                            new TryConvertGenericTest<string, double>("StringToDouble", "42.2", ConvertResult.Success, 42.2),
                            new TryConvertGenericTest<string, PrimaryColor>("StringToEnum", "42", ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<string, PrimaryColor>("StringToEnum", TestBlueString, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<string, PrimaryColor>("StringToEnum", TestBlueLowercaseString, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<string, float>("StringToFloat", "42.3", ConvertResult.Success, (float)42.3),
                            new TryConvertGenericTest<string, Guid>("StringToGuid", TestGuidString, ConvertResult.Success, TestGuid),
                            new TryConvertGenericTest<string, int>("StringToInt", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, long>("StringToLong", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, sbyte>("StringToSByte", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, short>("StringToShort", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, string>("StringToString", "The quick brown fox jumps over the lazy dog", ConvertResult.Success, "The quick brown fox jumps over the lazy dog"),
                            new TryConvertGenericTest<string, TimeSpan>("StringToTimeSpan", TestTimeSpanString, ConvertResult.Success, TestTimeSpan),
                            new TryConvertGenericTest<string, TimeSpan>("StringToTimeSpanWithFormat", TestTimeSpanStringWithFormat, ConvertResult.Success, TestTimeSpan, FormatTimeSpanContext),
                            new TryConvertGenericTest<string, TimeSpan>("StringToTimeSpanWithFormatAndFormatProvider", TestTimeSpanStringWithFormatAndFormatProvider, ConvertResult.Success, TestTimeSpan, FormatAndFormatProviderTimeSpanContext),
                            new TryConvertGenericTest<string, Type>("StringToType", TestTypeString, ConvertResult.Success, TestType),
                            new TryConvertGenericTest<string, uint>("StringToUInt", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, ulong>("StringToULong", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, Uri>("StringToUri", TestUriString, ConvertResult.Success, TestUri),
                            new TryConvertGenericTest<string, ushort>("StringToUShort", "42", ConvertResult.Success, 42),

                            // Nullable Types
                            new TryConvertGenericTest<string, bool?>("StringToNullable<Bool>", "False", ConvertResult.Success, false),
                            new TryConvertGenericTest<string, bool?>("StringToNullable<Bool>", "True", ConvertResult.Success, true),
                            new TryConvertGenericTest<string, bool?>("StringToNullable<Bool>", null, ConvertResult.Success, new bool?()),
                            new TryConvertGenericTest<string, byte?>("StringToNullable<Byte>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, byte?>("StringToNullable<Byte>", null, ConvertResult.Success, new byte?()),
                            new TryConvertGenericTest<string, char?>("StringToNullable<Char>", "*", ConvertResult.Success, '*'),
                            new TryConvertGenericTest<string, char?>("StringToNullable<Char>", null, ConvertResult.Success, new char?()),
                            new TryConvertGenericTest<string, DateTime?>("StringToNullable<DateTime>", TestDateTimeString, ConvertResult.Success, TestDateTime),
                            new TryConvertGenericTest<string, DateTime?>("StringToNullable<DateTime>WithFormat", TestDateTimeStringWithFormat, ConvertResult.Success, TestDateTime, FormatDateTimeContext),
                            new TryConvertGenericTest<string, DateTime?>("StringToNullable<DateTime>WithFormatAndFormatProvider", TestDateTimeStringWithFormatAndFormatProvider, ConvertResult.Success, TestDateTime, FormatAndFormatProviderDateTimeContext),
                            new TryConvertGenericTest<string, DateTime?>("StringToNullable<DateTime>", null, ConvertResult.Success, new DateTime?()),
                            new TryConvertGenericTest<string, DateTimeOffset?>("StringToNullable<DateTimeOffset>", TestDateTimeOffsetString, ConvertResult.Success, TestDateTimeOffset),
                            new TryConvertGenericTest<string, DateTimeOffset?>("StringToNullable<DateTimeOffset>WithFormat", TestDateTimeOffsetStringWithFormat, ConvertResult.Success, TestDateTimeOffset, FormatDateTimeOffsetContext),
                            new TryConvertGenericTest<string, DateTimeOffset?>("StringToNullable<DateTimeOffset>WithFormatAndFormatProvider", TestDateTimeOffsetStringWithFormatAndFormatProvider, ConvertResult.Success, TestDateTimeOffset, FormatAndFormatProviderDateTimeOffsetContext),
                            new TryConvertGenericTest<string, DateTimeOffset?>("StringToNullable<DateTimeOffset>", null, ConvertResult.Success, new DateTimeOffset?()),
                            new TryConvertGenericTest<string, decimal?>("StringToNullable<Decimal>", "42.1", ConvertResult.Success, 42.1m),
                            new TryConvertGenericTest<string, decimal?>("StringToNullable<Decimal>", null, ConvertResult.Success, new decimal?()),
                            new TryConvertGenericTest<string, double?>("StringToNullable<Double>", "42.2", ConvertResult.Success, 42.2),
                            new TryConvertGenericTest<string, double?>("StringToNullable<Double>", null, ConvertResult.Success, new double?()),
                            new TryConvertGenericTest<string, PrimaryColor?>("StringToNullable<Enum>", "42", ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<string, PrimaryColor?>("StringToNullable<Enum>", TestBlueString, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<string, PrimaryColor?>("StringToNullable<Enum>", TestBlueLowercaseString, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<string, PrimaryColor?>("StringToNullable<Enum>", null, ConvertResult.Success, new PrimaryColor?()),
                            new TryConvertGenericTest<string, float?>("StringToNullable<Float>", "42.3", ConvertResult.Success, (float)42.3),
                            new TryConvertGenericTest<string, float?>("StringToNullable<Float>", null, ConvertResult.Success, new float?()),
                            new TryConvertGenericTest<string, Guid?>("StringToNullable<Guid>", TestGuidString, ConvertResult.Success, TestGuid),
                            new TryConvertGenericTest<string, Guid?>("StringToNullable<Guid>", null, ConvertResult.Success, new Guid?()),
                            new TryConvertGenericTest<string, int?>("StringToNullable<Int>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, int?>("StringToNullable<Int>", null, ConvertResult.Success, new int?()),
                            new TryConvertGenericTest<string, long?>("StringToNullable<Long>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, long?>("StringToNullable<Long>", null, ConvertResult.Success, new long?()),
                            new TryConvertGenericTest<string, sbyte?>("StringToNullable<SByte>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, sbyte?>("StringToNullable<SByte>", null, ConvertResult.Success, new sbyte?()),
                            new TryConvertGenericTest<string, short?>("StringToNullable<Short>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, short?>("StringToNullable<Short>", null, ConvertResult.Success, new short?()),
                            new TryConvertGenericTest<string, TimeSpan?>("StringToNullable<TimeSpan>", TestTimeSpanString, ConvertResult.Success, TestTimeSpan),
                            new TryConvertGenericTest<string, TimeSpan?>("StringToNullable<TimeSpan>WithFormat", TestTimeSpanStringWithFormat, ConvertResult.Success, TestTimeSpan, FormatTimeSpanContext),
                            new TryConvertGenericTest<string, TimeSpan?>("StringToNullable<TimeSpan>WithFormatAndFormatProvider", TestTimeSpanStringWithFormatAndFormatProvider, ConvertResult.Success, TestTimeSpan, FormatAndFormatProviderTimeSpanContext),
                            new TryConvertGenericTest<string, TimeSpan?>("StringToNullable<TimeSpan>", null, ConvertResult.Success, new TimeSpan?()),
                            new TryConvertGenericTest<string, uint?>("StringToNullable<UInt>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, uint?>("StringToNullable<UInt>", null, ConvertResult.Success, new uint?()),
                            new TryConvertGenericTest<string, ulong?>("StringToNullable<ULong>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, ulong?>("StringToNullable<ULong>", null, ConvertResult.Success, new ulong?()),
                            new TryConvertGenericTest<string, ushort?>("StringToNullable<UShort>", "42", ConvertResult.Success, 42),
                            new TryConvertGenericTest<string, ushort?>("StringToNullable<UShort>", null, ConvertResult.Success, new ushort?()),

                            // Interface/Class Types
                            new TryConvertGenericTest<string, IInterface>("StringToInterface", "42", ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<string, BaseClass>("StringToBaseClass", "42", ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<string, DerivedClass>("StringToDerivedClass", "42", ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region TimeSpanToXXX
                new object []
                {
                    "TimeSpanToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<TimeSpan, bool>("TimeSpanToBool", TestTimeSpan, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<TimeSpan, byte>("TimeSpanToByte", TestTimeSpan, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<TimeSpan, byte[]>("TimeSpanToByteArray", TestTimeSpan, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<TimeSpan, char>("TimeSpanToChar", TestTimeSpan, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<TimeSpan, DateTime>("TimeSpanToDateTime", TestTimeSpan, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<TimeSpan, DateTimeOffset>("TimeSpanToDateTimeOffset", TestTimeSpan, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<TimeSpan, decimal>("TimeSpanToDecimal", TestTimeSpan, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<TimeSpan, double>("TimeSpanToDouble", TestTimeSpan, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<TimeSpan, PrimaryColor>("TimeSpanToEnum", TestTimeSpan, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<TimeSpan, float>("TimeSpanToFloat", TestTimeSpan, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<TimeSpan, Guid>("TimeSpanToGuid", TestTimeSpan, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<TimeSpan, int>("TimeSpanToInt", TestTimeSpan, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<TimeSpan, long>("TimeSpanToLong", TestTimeSpan, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<TimeSpan, sbyte>("TimeSpanToSByte", TestTimeSpan, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<TimeSpan, short>("TimeSpanToShort", TestTimeSpan, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<TimeSpan, string>("TimeSpanToString", TestTimeSpan, ConvertResult.Success, TestTimeSpanString),
                            new TryConvertGenericTest<TimeSpan, string>("TimeSpanToStringWithFormat", TestTimeSpan, ConvertResult.Success, TestTimeSpanStringWithFormat, FormatTimeSpanContext),
                            new TryConvertGenericTest<TimeSpan, string>("TimeSpanToStringWithFormatAndFormatProvider", TestTimeSpan, ConvertResult.Success, TestTimeSpanStringWithFormatAndFormatProvider, FormatAndFormatProviderTimeSpanContext),
                            new TryConvertGenericTest<TimeSpan, TimeSpan>("TimeSpanToTimeSpan", TestTimeSpan, ConvertResult.Success, TestTimeSpan),
                            new TryConvertGenericTest<TimeSpan, Type>("TimeSpanToType", TestTimeSpan, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<TimeSpan, uint>("TimeSpanToUInt", TestTimeSpan, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<TimeSpan, ulong>("TimeSpanToULong", TestTimeSpan, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<TimeSpan, ushort>("TimeSpanToUShort", TestTimeSpan, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<TimeSpan, Uri>("TimeSpanToUri", TestTimeSpan, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<TimeSpan, bool?>("TimeSpanToNullable<Bool>", TestTimeSpan, ConvertResult.Failure, default(bool?)),
                            new TryConvertGenericTest<TimeSpan, byte?>("TimeSpanToNullable<Byte>", TestTimeSpan, ConvertResult.Failure, default(byte?)),
                            new TryConvertGenericTest<TimeSpan, char?>("TimeSpanToNullable<Char>", TestTimeSpan, ConvertResult.Failure, default(char?)),
                            new TryConvertGenericTest<TimeSpan, DateTime?>("TimeSpanToNullable<DateTime>", TestTimeSpan, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<TimeSpan, DateTimeOffset?>("TimeSpanToNullable<DateTimeOffset>", TestTimeSpan, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<TimeSpan, decimal?>("TimeSpanToNullable<Decimal>", TestTimeSpan, ConvertResult.Failure, default(decimal?)),
                            new TryConvertGenericTest<TimeSpan, double?>("TimeSpanToNullable<Double>", TestTimeSpan, ConvertResult.Failure, default(double?)),
                            new TryConvertGenericTest<TimeSpan, PrimaryColor?>("TimeSpanToNullable<Enum>", TestTimeSpan, ConvertResult.Failure, default(PrimaryColor?)),
                            new TryConvertGenericTest<TimeSpan, float?>("TimeSpanToNullable<Float>", TestTimeSpan, ConvertResult.Failure, default(float?)),
                            new TryConvertGenericTest<TimeSpan, Guid?>("TimeSpanToNullable<Guid>", TestTimeSpan, ConvertResult.Failure, new Guid?()),
                            new TryConvertGenericTest<TimeSpan, int?>("TimeSpanToNullable<Int>", TestTimeSpan, ConvertResult.Failure, default(int?)),
                            new TryConvertGenericTest<TimeSpan, long?>("TimeSpanToNullable<Long>", TestTimeSpan, ConvertResult.Failure, default(long?)),
                            new TryConvertGenericTest<TimeSpan, sbyte?>("TimeSpanToNullable<SByte>", TestTimeSpan, ConvertResult.Failure, default(sbyte?)),
                            new TryConvertGenericTest<TimeSpan, short?>("TimeSpanToNullable<Short>", TestTimeSpan, ConvertResult.Failure, default(short?)),
                            new TryConvertGenericTest<TimeSpan, TimeSpan?>("TimeSpanToNullable<TimeSpan>", TestTimeSpan, ConvertResult.Success, TestTimeSpan),
                            new TryConvertGenericTest<TimeSpan, uint?>("TimeSpanToNullable<UInt>", TestTimeSpan, ConvertResult.Failure, default(uint?)),
                            new TryConvertGenericTest<TimeSpan, ulong?>("TimeSpanToNullable<ULong>", TestTimeSpan, ConvertResult.Failure, default(ulong?)),
                            new TryConvertGenericTest<TimeSpan, ushort?>("TimeSpanToNullable<UShort>", TestTimeSpan, ConvertResult.Failure, default(ushort?)),

                            // Interface/Class Types
                            new TryConvertGenericTest<TimeSpan, IInterface>("TimeSpanToInterface", TestTimeSpan, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<TimeSpan, BaseClass>("TimeSpanToBaseClass", TestTimeSpan, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<TimeSpan, DerivedClass>("TimeSpanToDerivedClass", TestTimeSpan, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region TypeToXXX
                new object []
                {
                    "TypeToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<Type, bool>("TypeToBool", TestType, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<Type, byte>("TypeToByte", TestType, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<Type, byte[]>("TypeToByteArray", TestType, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<Type, char>("TypeToChar", TestType, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<Type, DateTime>("TypeToDateTime", TestType, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<Type, DateTimeOffset>("TypeToDateTimeOffset", TestType, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<Type, decimal>("TypeToDecimal", TestType, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<Type, double>("TypeToDouble", TestType, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<Type, PrimaryColor>("TypeToEnum", TestType, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<Type, float>("TypeToFloat", TestType, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<Type, Guid>("TypeToGuid", TestType, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<Type, int>("TypeToInt", TestType, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<Type, long>("TypeToLong", TestType, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<Type, sbyte>("TypeToSByte", TestType, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<Type, short>("TypeToShort", TestType, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<Type, string>("TypeToString", TestType, ConvertResult.Success, TestTypeString),
                            new TryConvertGenericTest<Type, TimeSpan>("TypeToTimeSpan", TestType, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<Type, Type>("TypeToType", TestType, ConvertResult.Success, TestType),
                            new TryConvertGenericTest<Type, uint>("TypeToUInt", TestType, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<Type, ulong>("TypeToULong", TestType, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<Type, ushort>("TypeToUShort", TestType, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<Type, Uri>("TypeToUri", TestType, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<Type, bool?>("TypeToNullable<Bool>", TestType, ConvertResult.Failure, default(bool?)),
                            new TryConvertGenericTest<Type, byte?>("TypeToNullable<Byte>", TestType, ConvertResult.Failure, default(byte?)),
                            new TryConvertGenericTest<Type, char?>("TypeToNullable<Char>", TestType, ConvertResult.Failure, default(char?)),
                            new TryConvertGenericTest<Type, DateTime?>("TypeToNullable<DateTime>", TestType, ConvertResult.Failure, new DateTime?()),
                            new TryConvertGenericTest<Type, DateTimeOffset?>("TypeToNullable<DateTimeOffset>", TestType, ConvertResult.Failure, new DateTimeOffset?()),
                            new TryConvertGenericTest<Type, decimal?>("TypeToNullable<Decimal>", TestType, ConvertResult.Failure, default(decimal?)),
                            new TryConvertGenericTest<Type, double?>("TypeToNullable<Double>", TestType, ConvertResult.Failure, default(double?)),
                            new TryConvertGenericTest<Type, PrimaryColor?>("TypeToNullable<Enum>", TestType, ConvertResult.Failure, default(PrimaryColor?)),
                            new TryConvertGenericTest<Type, float?>("TypeToNullable<Float>", TestType, ConvertResult.Failure, default(float?)),
                            new TryConvertGenericTest<Type, Guid?>("TypeToNullable<Guid>", TestType, ConvertResult.Failure, new Guid?()),
                            new TryConvertGenericTest<Type, int?>("TypeToNullable<Int>", TestType, ConvertResult.Failure, default(int?)),
                            new TryConvertGenericTest<Type, long?>("TypeToNullable<Long>", TestType, ConvertResult.Failure, default(long?)),
                            new TryConvertGenericTest<Type, sbyte?>("TypeToNullable<SByte>", TestType, ConvertResult.Failure, default(sbyte?)),
                            new TryConvertGenericTest<Type, short?>("TypeToNullable<Short>", TestType, ConvertResult.Failure, default(short?)),
                            new TryConvertGenericTest<Type, TimeSpan?>("TypeToNullable<TimeSpan>", TestType, ConvertResult.Failure, new TimeSpan?()),
                            new TryConvertGenericTest<Type, uint?>("TypeToNullable<UInt>", TestType, ConvertResult.Failure, default(uint?)),
                            new TryConvertGenericTest<Type, ulong?>("TypeToNullable<ULong>", TestType, ConvertResult.Failure, default(ulong?)),
                            new TryConvertGenericTest<Type, ushort?>("TypeToNullable<UShort>", TestType, ConvertResult.Failure, default(ushort?)),

                            // Interface/Class Types
                            new TryConvertGenericTest<Type, IInterface>("TypeToInterface", TestType, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<Type, BaseClass>("TypeToBaseClass", TestType, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<Type, DerivedClass>("TypeToDerivedClass", TestType, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region UIntToXXX
                new object []
                {
                    "UIntToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<uint, bool>("UIntToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<uint, byte>("UIntToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, byte[]>("UIntToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<uint, char>("UIntToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<uint, DateTime>("UIntToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<uint, DateTimeOffset>("UIntToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<uint, decimal>("UIntToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, double>("UIntToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, PrimaryColor>("UIntToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<uint, float>("UIntToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, Guid>("UIntToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<uint, int>("UIntToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, long>("UIntToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, sbyte>("UIntToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, short>("UIntToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, string>("UIntToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<uint, TimeSpan>("UIntToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<uint, Type>("UIntToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<uint, uint>("UIntToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, ulong>("UIntToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, ushort>("UIntToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, Uri>("UIntToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<uint, bool?>("UIntToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<uint, byte?>("UIntToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, char?>("UIntToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<uint, DateTime?>("UIntToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<uint, DateTimeOffset?>("UIntToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<uint, decimal?>("UIntToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, double?>("UIntToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, PrimaryColor?>("UIntToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<uint, float?>("UIntToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, Guid?>("UIntToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<uint, int?>("UIntToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, long?>("UIntToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, sbyte?>("UIntToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, short?>("UIntToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, TimeSpan?>("UIntToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<uint, uint?>("UIntToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, ulong?>("UIntToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<uint, ushort?>("UIntToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<uint, IInterface>("UIntToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<uint, BaseClass>("UIntToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<uint, DerivedClass>("UIntToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region ULongToXXX
                new object []
                {
                    "ULongToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<ulong, bool>("ULongToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<ulong, byte>("ULongToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, byte[]>("ULongToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<ulong, char>("ULongToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<ulong, DateTime>("ULongToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<ulong, DateTimeOffset>("ULongToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<ulong, decimal>("ULongToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, double>("ULongToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, PrimaryColor>("ULongToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<ulong, float>("ULongToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, Guid>("ULongToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<ulong, int>("ULongToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, long>("ULongToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, sbyte>("ULongToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, short>("ULongToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, string>("ULongToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<ulong, TimeSpan>("ULongToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<ulong, Type>("ULongToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<ulong, uint>("ULongToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, ulong>("ULongToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, ushort>("ULongToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, Uri>("ULongToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<ulong, bool?>("ULongToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<ulong, byte?>("ULongToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, char?>("ULongToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<ulong, DateTime?>("ULongToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<ulong, DateTimeOffset?>("ULongToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<ulong, decimal?>("ULongToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, double?>("ULongToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, PrimaryColor?>("ULongToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<ulong, float?>("ULongToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, Guid?>("ULongToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<ulong, int?>("ULongToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, long?>("ULongToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, sbyte?>("ULongToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, short?>("ULongToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, TimeSpan?>("ULongToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<ulong, uint?>("ULongToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, ulong?>("ULongToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ulong, ushort?>("ULongToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<ulong, IInterface>("ULongToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<ulong, BaseClass>("ULongToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<ulong, DerivedClass>("ULongToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region UShortToXXX
                new object []
                {
                    "UShortToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<ushort, bool>("UShortToBool", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<ushort, byte>("UShortToByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, byte[]>("UShortToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<ushort, char>("UShortToChar", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<ushort, DateTime>("UShortToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<ushort, DateTimeOffset>("UShortToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<ushort, decimal>("UShortToDecimal", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, double>("UShortToDouble", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, PrimaryColor>("UShortToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<ushort, float>("UShortToFloat", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, Guid>("UShortToGuid", 42, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<ushort, int>("UShortToInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, long>("UShortToLong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, sbyte>("UShortToSByte", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, short>("UShortToShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, string>("UShortToString", 42, ConvertResult.Success, "42"),
                            new TryConvertGenericTest<ushort, TimeSpan>("UShortToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<ushort, Type>("UShortToType", 42, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<ushort, uint>("UShortToUInt", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, ulong>("UShortToULong", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, ushort>("UShortToUShort", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, Uri>("UShortToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            new TryConvertGenericTest<ushort, bool?>("UShortToNullable<Bool>", 42, ConvertResult.Success, true),
                            new TryConvertGenericTest<ushort, byte?>("UShortToNullable<Byte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, char?>("UShortToNullable<Char>", 42, ConvertResult.Success, '*'),
                            new TryConvertGenericTest<ushort, DateTime?>("UShortToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            new TryConvertGenericTest<ushort, DateTimeOffset?>("UShortToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            new TryConvertGenericTest<ushort, decimal?>("UShortToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, double?>("UShortToNullable<Double>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, PrimaryColor?>("UShortToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            new TryConvertGenericTest<ushort, float?>("UShortToNullable<Float>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, Guid?>("UShortToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            new TryConvertGenericTest<ushort, int?>("UShortToNullable<Int>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, long?>("UShortToNullable<Long>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, sbyte?>("UShortToNullable<SByte>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, short?>("UShortToNullable<Short>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, TimeSpan?>("UShortToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            new TryConvertGenericTest<ushort, uint?>("UShortToNullable<UInt>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, ulong?>("UShortToNullable<ULong>", 42, ConvertResult.Success, 42),
                            new TryConvertGenericTest<ushort, ushort?>("UShortToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            new TryConvertGenericTest<ushort, IInterface>("UShortToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<ushort, BaseClass>("UShortToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<ushort, DerivedClass>("UShortToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                #region UriToXXX
                new object []
                {
                    "UriToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<Uri, bool>("UriToBool", TestUri, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<Uri, byte>("UriToByte", TestUri, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<Uri, byte[]>("UriToByteArray", TestUri, ConvertResult.Failure, default(byte[])),
                            new TryConvertGenericTest<Uri, char>("UriToChar", TestUri, ConvertResult.Failure, default(char)),
                            new TryConvertGenericTest<Uri, DateTime>("UriToDateTime", TestUri, ConvertResult.Failure, default(DateTime)),
                            new TryConvertGenericTest<Uri, DateTimeOffset>("UriToDateTimeOffset", TestUri, ConvertResult.Failure, default(DateTimeOffset)),
                            new TryConvertGenericTest<Uri, decimal>("UriToDecimal", TestUri, ConvertResult.Failure, default(decimal)),
                            new TryConvertGenericTest<Uri, double>("UriToDouble", TestUri, ConvertResult.Failure, default(double)),
                            new TryConvertGenericTest<Uri, PrimaryColor>("UriToEnum", TestUri, ConvertResult.Failure, default(PrimaryColor)),
                            new TryConvertGenericTest<Uri, float>("UriToFloat", TestUri, ConvertResult.Failure, default(float)),
                            new TryConvertGenericTest<Uri, Guid>("UriToGuid", TestUri, ConvertResult.Failure, default(Guid)),
                            new TryConvertGenericTest<Uri, int>("UriToInt", TestUri, ConvertResult.Failure, default(int)),
                            new TryConvertGenericTest<Uri, long>("UriToLong", TestUri, ConvertResult.Failure, default(long)),
                            new TryConvertGenericTest<Uri, sbyte>("UriToSByte", TestUri, ConvertResult.Failure, default(sbyte)),
                            new TryConvertGenericTest<Uri, short>("UriToShort", TestUri, ConvertResult.Failure, default(short)),
                            new TryConvertGenericTest<Uri, string>("UriToString", TestUri, ConvertResult.Success, TestUriString),
                            new TryConvertGenericTest<Uri, TimeSpan>("UriToTimeSpan", TestUri, ConvertResult.Failure, default(TimeSpan)),
                            new TryConvertGenericTest<Uri, Type>("UriToType", TestUri, ConvertResult.Failure, default(Type)),
                            new TryConvertGenericTest<Uri, uint>("UriToUInt", TestUri, ConvertResult.Failure, default(uint)),
                            new TryConvertGenericTest<Uri, ulong>("UriToULong", TestUri, ConvertResult.Failure, default(ulong)),
                            new TryConvertGenericTest<Uri, ushort>("UriToUShort", TestUri, ConvertResult.Failure, default(ushort)),
                            new TryConvertGenericTest<Uri, Uri>("UriToUri", TestUri, ConvertResult.Success, TestUri),

                            // Nullable Types
                            new TryConvertGenericTest<Uri, bool?>("UriToNullable<Bool>", TestUri, ConvertResult.Failure, default(bool?)),
                            new TryConvertGenericTest<Uri, byte?>("UriToNullable<Byte>", TestUri, ConvertResult.Failure, default(byte?)),
                            new TryConvertGenericTest<Uri, char?>("UriToNullable<Char>", TestUri, ConvertResult.Failure, default(char?)),
                            new TryConvertGenericTest<Uri, DateTime?>("UriToNullable<DateTime>", TestUri, ConvertResult.Failure, new DateTime?()),
                            new TryConvertGenericTest<Uri, DateTimeOffset?>("UriToNullable<DateTimeOffset>", TestUri, ConvertResult.Failure, new DateTimeOffset?()),
                            new TryConvertGenericTest<Uri, decimal?>("UriToNullable<Decimal>", TestUri, ConvertResult.Failure, default(decimal?)),
                            new TryConvertGenericTest<Uri, double?>("UriToNullable<Double>", TestUri, ConvertResult.Failure, default(double?)),
                            new TryConvertGenericTest<Uri, PrimaryColor?>("UriToNullable<Enum>", TestUri, ConvertResult.Failure, default(PrimaryColor?)),
                            new TryConvertGenericTest<Uri, float?>("UriToNullable<Float>", TestUri, ConvertResult.Failure, default(float?)),
                            new TryConvertGenericTest<Uri, Guid?>("UriToNullable<Guid>", TestUri, ConvertResult.Failure, new Guid?()),
                            new TryConvertGenericTest<Uri, int?>("UriToNullable<Int>", TestUri, ConvertResult.Failure, default(int?)),
                            new TryConvertGenericTest<Uri, long?>("UriToNullable<Long>", TestUri, ConvertResult.Failure, default(long?)),
                            new TryConvertGenericTest<Uri, sbyte?>("UriToNullable<SByte>", TestUri, ConvertResult.Failure, default(sbyte?)),
                            new TryConvertGenericTest<Uri, short?>("UriToNullable<Short>", TestUri, ConvertResult.Failure, default(short?)),
                            new TryConvertGenericTest<Uri, TimeSpan?>("UriToNullable<TimeSpan>", TestUri, ConvertResult.Failure, new TimeSpan?()),
                            new TryConvertGenericTest<Uri, uint?>("UriToNullable<UInt>", TestUri, ConvertResult.Failure, default(uint?)),
                            new TryConvertGenericTest<Uri, ulong?>("UriToNullable<ULong>", TestUri, ConvertResult.Failure, default(ulong?)),
                            new TryConvertGenericTest<Uri, ushort?>("UriToNullable<UShort>", TestUri, ConvertResult.Failure, default(ushort?)),

                            // Interface/Class Types
                            new TryConvertGenericTest<Uri, IInterface>("UriToInterface", TestUri, ConvertResult.Failure, default(IInterface)),
                            new TryConvertGenericTest<Uri, BaseClass>("UriToBaseClass", TestUri, ConvertResult.Failure, default(BaseClass)),
                            new TryConvertGenericTest<Uri, DerivedClass>("UriToDerivedClass", TestUri, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                // Nullable Types ///////////////////////////////////////////
                #region Nullable<Byte>ToXXX
                new object []
                {
                    "Nullable<Byte>ToXXX - Generic",
                    new object []
                        {
                            // Simple Types
                            new TryConvertGenericTest<byte?, bool>("Nullable<Byte>ToBool", null, ConvertResult.Failure, default(bool)),
                            new TryConvertGenericTest<byte?, bool>("Nullable<Byte>ToBool", 42, ConvertResult.Success, true),

                            new TryConvertGenericTest<byte?, byte>("Nullable<Byte>ToByte", null, ConvertResult.Failure, default(byte)),
                            new TryConvertGenericTest<byte?, byte>("Nullable<Byte>ToByte", 42, ConvertResult.Success, 42),

                            //new TryConvertGenericTest<byte?, byte[]>("Nullable<Byte>ToByteArray", 42, ConvertResult.Failure, default(byte[])),
                            //new TryConvertGenericTest<byte?, char>("Nullable<Byte>ToChar", 42, ConvertResult.Success, '*'),
                            //new TryConvertGenericTest<byte?, DateTime>("Nullable<Byte>ToDateTime", 42, ConvertResult.Failure, default(DateTime)),
                            //new TryConvertGenericTest<byte?, DateTimeOffset>("Nullable<Byte>ToDateTimeOffset", 42, ConvertResult.Failure, default(DateTimeOffset)),
                            //new TryConvertGenericTest<byte?, decimal>("Nullable<Byte>ToDecimal", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, double>("Nullable<Byte>ToDouble", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, PrimaryColor>("Nullable<Byte>ToEnum", 42, ConvertResult.Success, PrimaryColor.Blue),
                            //new TryConvertGenericTest<byte?, float>("Nullable<Byte>ToFloat", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, Guid>("Nullable<Byte>ToGuid", 42, ConvertResult.Failure, default(Guid)),
                            //new TryConvertGenericTest<byte?, int>("Nullable<Byte>ToInt", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, long>("Nullable<Byte>ToLong", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, sbyte>("Nullable<Byte>ToSByte", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, short>("Nullable<Byte>ToShort", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, string>("Nullable<Byte>ToString", 42, ConvertResult.Success, "42"),
                            //new TryConvertGenericTest<byte?, TimeSpan>("Nullable<Byte>ToTimeSpan", 42, ConvertResult.Failure, default(TimeSpan)),
                            //new TryConvertGenericTest<byte?, Type>("Nullable<Byte>ToType", 42, ConvertResult.Failure, default(Type)),
                            //new TryConvertGenericTest<byte?, uint>("Nullable<Byte>ToUInt", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, ulong>("Nullable<Byte>ToULong", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, ushort>("Nullable<Byte>ToUShort", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, Uri>("Nullable<Byte>ToUri", 42, ConvertResult.Failure, default(Uri)),

                            // Nullable Types
                            //new TryConvertGenericTest<byte?, bool?>("Nullable<Byte>ToNullable<Bool>", 42, ConvertResult.Success, true),
                            //new TryConvertGenericTest<byte?, byte?>("Nullable<Byte>ToNullable<Byte>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, char?>("Nullable<Byte>ToNullable<Char>", 42, ConvertResult.Success, '*'),
                            //new TryConvertGenericTest<byte?, DateTime?>("Nullable<Byte>ToNullable<DateTime>", 42, ConvertResult.Failure, default(DateTime?)),
                            //new TryConvertGenericTest<byte?, DateTimeOffset?>("Nullable<Byte>ToNullable<DateTimeOffset>", 42, ConvertResult.Failure, default(DateTimeOffset?)),
                            //new TryConvertGenericTest<byte?, decimal?>("Nullable<Byte>ToNullable<Decimal>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, double?>("Nullable<Byte>ToNullable<Double>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, PrimaryColor?>("Nullable<Byte>ToNullable<Enum>", 42, ConvertResult.Success, PrimaryColor.Blue),
                            //new TryConvertGenericTest<byte?, float?>("Nullable<Byte>ToNullable<Float>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, Guid?>("Nullable<Byte>ToNullable<Guid>", 42, ConvertResult.Failure, default(Guid?)),
                            //new TryConvertGenericTest<byte?, int?>("Nullable<Byte>ToNullable<Int>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, long?>("Nullable<Byte>ToNullable<Long>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, sbyte?>("Nullable<Byte>ToNullable<SByte>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, short?>("Nullable<Byte>ToNullable<Short>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, TimeSpan?>("Nullable<Byte>ToNullable<TimeSpan>", 42, ConvertResult.Failure, default(TimeSpan?)),
                            //new TryConvertGenericTest<byte?, uint?>("Nullable<Byte>ToNullable<UInt>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, ulong?>("Nullable<Byte>ToNullable<ULong>", 42, ConvertResult.Success, 42),
                            //new TryConvertGenericTest<byte?, ushort?>("Nullable<Byte>ToNullable<UShort>", 42, ConvertResult.Success, 42),

                            // Interface/Class Types
                            //new TryConvertGenericTest<byte?, IInterface>("Nullable<Byte>ToInterface", 42, ConvertResult.Failure, default(IInterface)),
                            //new TryConvertGenericTest<byte?, BaseClass>("Nullable<Byte>ToBaseClass", 42, ConvertResult.Failure, default(BaseClass)),
                            //new TryConvertGenericTest<byte?, DerivedClass>("Nullable<Byte>ToDerivedClass", 42, ConvertResult.Failure, default(DerivedClass)),
                        }
                },
                #endregion

                // Interface/Class Types ////////////////////////////////////
            };
        #endregion

        #region Test Types
        public enum ConvertResult
        {
            Success,
            Failure
        }

        public class TryConvertGenericTest<TSource, TTarget> : UnitTest
        {
            // PUBLIC CONSTRUCTORS //////////////////////////////////////////
            #region Constructors
            public TryConvertGenericTest(string name, TSource source, ConvertResult expectedResult, TTarget expectedValue, TypeConverterContext context = null)
                : base(name)
            {
                this.Source = source;
                this.ExpectedResult = expectedResult;
                this.ExpectedValue = expectedValue;
                this.Context = context;
            }
            #endregion

            // PROTECTED METHODS ////////////////////////////////////////////
            #region UnitTest Overrides
            protected override void Arrange()
            {
                this.TypeConverter = new TypeConverter();

                var sourceAsString = ValueAsString(this.Source);

                this.WriteLine("Source:    {0} ({1})", sourceAsString, typeof(TSource).Name);
                this.WriteLine();

                var expectedValueAsString = ValueAsString(this.ExpectedValue);

                this.WriteLine("Expected");
                this.WriteLine("  Result:  {0}", this.ExpectedResult);
                this.WriteLine("  Value:   {0} ({1})", expectedValueAsString, typeof(TTarget).Name);
                this.WriteLine();
            }

            protected override void Act()
            {
                var source = this.Source;
                var context = this.Context;
                TTarget actualValue;
                var actualResult = this.TypeConverter.TryConvert(source, context, out actualValue);

                this.ActualResult = actualResult ? ConvertResult.Success : ConvertResult.Failure;
                this.ActualValue = actualValue;

                var actualValueAsString = ValueAsString(this.ActualValue);

                this.WriteLine("Actual");
                this.WriteLine("  Result:  {0}", this.ActualResult);
                this.WriteLine("  Value:   {0} ({1})", actualValueAsString, typeof(TTarget).Name);
            }

            protected override void Assert()
            {
                this.ActualResult.Should().Be(this.ExpectedResult);

                switch (this.ActualResult)
                {
                    case ConvertResult.Success:
                        {
                            // Special case if source type is nullable.
                            if (typeof(TSource).IsNullableType())
                            {
                                // Determine if nullable has a value or not.
                                var hasValue = GetNullableHasValue(this.Source);
                                if (hasValue)
                                {
                                    this.ActualValue.Should().BeOfType<TTarget>();
                                    this.ActualValue.Should().Be(this.ExpectedValue);
                                }
                                else
                                {
                                    if (typeof(TTarget).IsClass())
                                    {
                                        this.ActualValue.Should().BeNull();                                        
                                    }
                                    else
                                    {
                                        this.ActualValue.Should().BeOfType<TTarget>();
                                        this.ActualValue.Should().Be(default(TTarget));
                                    }
                                }
                                return;
                            }

                            // Special case if target type is nullable.
                            if (typeof(TTarget).IsNullableType())
                            {
                                // Determine if nullable has a value or not.
                                var hasValue = GetNullableHasValue(this.ActualValue);
                                if (hasValue)
                                {
                                    var underlyingType = Nullable.GetUnderlyingType(typeof(TTarget));
                                    this.ActualValue.Should().BeOfType(underlyingType);
                                    this.ActualValue.Should().Be(this.ExpectedValue);
                                }
                                else
                                {
                                    this.ActualValue.Should().BeNull();
                                }
                                return;
                            }

                            // Special case if target type is byte array.
                            if (typeof(TTarget) == typeof(byte[]))
                            {
                                this.ActualValue.Should().BeOfType<TTarget>();

                                var expectedValue = this.ExpectedValue as byte[];
                                var actualValue = this.ActualValue as byte[];
                                actualValue.Should().ContainInOrder(expectedValue);
                                return;
                            }

                            // Special case if target type is type.
                            if (typeof(TTarget) == typeof(Type))
                            {
                                this.ActualValue.Should().BeAssignableTo<Type>();
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
            private ITypeConverter TypeConverter { get; set; }
            private ConvertResult ActualResult { get; set; }
            private TTarget ActualValue { get; set; }
            #endregion

            #region User Supplied Properties
            private TSource Source { get; set; }

            private ConvertResult ExpectedResult { get; set; }
            private TTarget ExpectedValue { get; set; }
            private TypeConverterContext Context { get; set; }
            #endregion

            // PRIVATE METHODS //////////////////////////////////////////////
            #region Methods
            private static bool GetNullableHasValue<T>(T nullable)
            {
                var nullableType = typeof(T);
                var instanceExpression = Expression.Parameter(nullableType, "i");
                var propertyInfo = nullableType.GetProperty(StaticReflection.GetMemberName<int?>(x => x.HasValue), BindingFlags.Public | BindingFlags.Instance);
                var propertyExpression = Expression.Property(instanceExpression, propertyInfo);
                var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(propertyExpression, instanceExpression);
                var labmda = lambdaExpression.Compile();
                var hasValue = labmda(nullable);
                return hasValue;
            }

            private static string ValueAsString<TValue>(TValue value)
            {
                var valueAsString = value.SafeToString();
                var valueType = typeof(TValue);

                if (!valueType.IsNullableType() && valueType.IsValueType())
                    return valueAsString;

                var valueAsObject = (object)value;
                if (valueAsObject == null)
                    valueAsString = "null";
                return valueAsString;
            }
            #endregion
        }
        #endregion
    }
}
