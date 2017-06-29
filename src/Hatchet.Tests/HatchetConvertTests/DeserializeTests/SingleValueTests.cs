using System;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class SingleValueTests
    {
        [Test]
        public void Deserialize_ANonQuotedDateTime_ShouldReturnADateTime()
        {
            // Arrange
            var date = DateTime.Parse("2013-01-02");
            var input = date.ToString("yyyy-MM-dd");

            // Act
            var result = HatchetConvert.Deserialize<DateTime>(input);

            // Assert
            result.Should().Be(date);
        }

        [Test]
        public void Deserialize_AQuotedDateTime_ShouldReturnADateTime()
        {
            // Arrange
            var date = DateTime.Parse("2014-10-01 07:20:00");
            var input = "'" + date.ToString("O") + "'";

            // Act
            var result = HatchetConvert.Deserialize<DateTime>(input);

            // Assert
            result.Should().Be(date);
        }

        [Test]
        public void Deserialize_AByte_ShouldReturnAByte()
        {
            // Arrange
            var input = "14";

            // Act
            var result = HatchetConvert.Deserialize<byte>(input);

            // Assert
            result.Should().Be(14);
        }

        [Test]
        public void Deserialize_AChar_ShouldReturnAChar()
        {
            // Arrange
            var input = "a";

            // Act
            var result = HatchetConvert.Deserialize<char>(input);

            // Assert
            result.Should().Be('a');
        }

        [Test]
        public void Deserialize_AGuid_ShouldReturnAGuid()
        {
            // Arrange
            var input = new Guid("{879FC5CF-74EE-4525-A457-C1CD2F5C451E}");

            // Act
            var result = HatchetConvert.Deserialize<Guid>(input.ToString());

            // Assert
            result.Should().Be(input);
        }

        [Test]
        public void Deserialize_AnSbyte_ShouldReturnAnSbyte()
        {
            // Arrange
            var input = "-8";

            // Act
            var result = HatchetConvert.Deserialize<sbyte>(input);

            // Assert
            result.Should().Be(-8);
        }

        [Test]
        public void Deserialize_AShort_ShouldReturnAShort()
        {
            // Arrange
            var input = "-372";

            // Act
            var result = HatchetConvert.Deserialize<short>(input);

            // Assert
            result.Should().Be(-372);
        }

        [Test]
        public void Deserialize_AUshort_ShouldReturnAUshort()
        {
            // Arrange
            var input = "8192";

            // Act
            var result = HatchetConvert.Deserialize<ushort>(input);

            // Assert
            result.Should().Be(8192);
        }

        [Test]
        public void Deserialize_ADecimal_ShouldReturnADecimal()
        {
            // Arrange
            var input = "4377.89";

            // Act
            var result = HatchetConvert.Deserialize<decimal>(input);

            // Assert
            result.Should().Be(4377.89m);
        }

        [Test]
        public void Deserialize_AUint_ShouldReturnAUint()
        {
            // Arrange
            var input = "487182";

            // Act
            var result = HatchetConvert.Deserialize<uint>(input);

            // Assert
            result.Should().Be(487182);
        }

        [Test]
        public void Deserialize_AUlong_ShouldReturnAUlong()
        {
            // Arrange
            var input = "1818118181";

            // Act
            var result = HatchetConvert.Deserialize<ulong>(input);

            // Assert
            result.Should().Be(1818118181L);
        }

        [Test]
        public void Deserialize_ALong_ShouldReturnALong()
        {
            // Arrange
            var input = "123105051";

            // Act
            var result = HatchetConvert.Deserialize<long>(input);

            // Assert
            result.Should().Be(123105051L);
        }

        [Test]
        public void Deserialize_AString_ShouldReturnAString()
        {
            // Arrange
            var input = "'Hello World'";

            // Act
            var result = HatchetConvert.Deserialize<string>(input);

            // Assert
            result.Should().Be("Hello World");
        } 

        [Test]
        public void Deserialize_AnInteger_ShouldReturnAnInteger()
        {
            // Arrange
            var input = "1234";

            // Act
            var result = HatchetConvert.Deserialize<int>(input);

            // Assert
            result.Should().Be(1234);
        } 

        [Test]
        public void Deserialize_AFloat_ShouldReturnAFloat()
        {
            // Arrange
            var input = "12.34";

            // Act
            var result = HatchetConvert.Deserialize<float>(input);

            // Assert
            result.Should().BeApproximately(12.34f, 0.0001f);
        } 

        [Test]
        public void Deserialize_ABoolean_ShouldReturnABoolean()
        {
            // Arrange
            var input = "true";

            // Act
            var result = HatchetConvert.Deserialize<bool>(input);

            // Assert
            result.Should().Be(true);
        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("null", null)]
        public void Deserialize_ANullableBoolean_ShouldReturnABoolean(string input, bool? expected)
        {
            // Act
            var result = HatchetConvert.Deserialize<bool?>(input);

            // Assert
            result.Should().Be(expected);
        }

        [TestCase("1", 1)]
        [TestCase("null", null)]
        public void Deserialize_ANullableInt_ShouldReturnAnInt(string input, int? expected)
        {
            // Act
            var result = HatchetConvert.Deserialize<int?>(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}