using System;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

[TestFixture]
public class SingleValueTests
{
    [Test]
    public void Serialize_AGuid_ReturnsValueAsAString()
    {
        // Arrange
        var value = new Guid("fa1831fe-78ec-4b83-aac7-289d1d0c60db");

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("fa1831fe-78ec-4b83-aac7-289d1d0c60db");
    }

    [Test]
    public void Serialize_ADecimal_ReturnsValueAsAString()
    {
        // Arrange
        const decimal value = 14431.18m;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("14431.18");
    }

    [Test]
    public void Serialize_ADate_ReturnsValueAsAString()
    {
        // Arrange
        var date = DateTime.Parse("2014-08-01 12:34:00");
        var iso8601 = "\"" + date.ToString("O") + "\"";

        // Act
        var result = HatchetConvert.Serialize(date);

        // Assert
        result.Should().Be(iso8601);
    }

    [Test]
    public void Serialize_AChar_ReturnsValueAsAString()
    {
        // Arrange
        const char value = 'a';

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("a");
    }

    [Test]
    public void Serialize_AFloat_ReturnsValueAsAString()
    {
        // Arrange
        const float value = 123.456f;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().StartWith("123.45");
    }

    [Test]
    public void Serialize_ADouble_ReturnsValueAsAString()
    {
        // Arrange
        const double value = 1234.5678;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().StartWith("1234.567");
    }

    [Test]
    public void Serialize_ABoolean_ReturnsValueAsAString()
    {
        // Arrange
        const bool value = true;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("true");
    }

    [Test]
    public void Serialize_APositiveInteger_ReturnsValueAsAString()
    {
        // Arrange
        const int value = 1234;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("1234");
    }

    [Test]
    public void Serialize_ANegativeInteger_ReturnsValueAsAString()
    {
        // Arrange
        const int value = -1234;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("-1234");
    }

    [Test]
    public void Serialize_APositiveLong_ReturnsValueAsAString()
    {
        // Arrange
        const long value = 12345678901234567;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("12345678901234567");
    }

    [Test]
    public void Serialize_ANegativeLong_ReturnsValueAsAString()
    {
        // Arrange
        const long value = -12345678901234567;

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("-12345678901234567");
    }

    [Test]
    public void Serialize_ASingleWordString_ReturnsAString()
    {
        // Arrange
        const string value = "goodMorning";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("goodMorning");
    }

    [Test]
    public void Serialize_AStringWithSpaces_ReturnsAQuotedString()
    {
        // Arrange
        const string value = "This is a long string";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("\"This is a long string\"");
    }

    [Test]
    public void Serialize_AnEmptyString_ReturnsAnEmptyQuotedString()
    {
        // Arrange
        const string value = "";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("\"\"");
    }

    [Test]
    public void Serialize_AWhollySingleQuotedString_ReturnsQuotedString()
    {
        // Arrange
        const string value = "'Hello'";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("\"'Hello'\"");
    }

    [Test]
    public void Serialize_AStringContainingSingleQuotes_ReturnsADoubleQuotedString()
    {
        // Arrange
        const string value = "Quoted string 'for what'";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("\"Quoted string 'for what'\"");
    }

    [Test]
    public void Serialize_AStringContainingDoubleQuotes_ReturnASingleQuotedString()
    {
        // Arrange
        const string value = "Quoted string \"for what\"";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("'Quoted string \"for what\"'");
    }

    [Test]
    public void Serialize_AStringContainingBothQuoteTypes_EscapesDoubleQuotes()
    {
        // Arrange
        const string value = "\"Quoted string\" 'for what'";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("\"\\\"Quoted string\\\" 'for what'\"");
    }

    [Test]
    public void Serialize_AStringWithNewLinesWindows_ReturnsStringBlock()
    {
        // Arrange
        const string value = "This value\r\ngoes over\r\nthree lines";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("![This value\r\ngoes over\r\nthree lines]!");
    }

    [Test]
    public void Serialize_AStringWithNewLinesUnix_ReturnsStringBlock()
    {
        // Arrange
        const string value = "This value\ngoes over\nthree lines";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("![This value\ngoes over\nthree lines]!");
    }

    [Test]
    public void Serialize_AStringWithNewLinesMac_ReturnsStringBlock()
    {
        // Arrange
        const string value = "This value\rgoes over\rthree lines";

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("![This value\rgoes over\rthree lines]!");
    }
}