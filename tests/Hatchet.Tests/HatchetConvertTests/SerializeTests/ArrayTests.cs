using System.Collections;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

[TestFixture]
public class ArrayTests
{
    [Test]
    public void Serialize_AnEmptyArray_ReturnsValueAsAString()
    {
        // Arrange
        var value = new int[0];

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("[]");
    }

    [Test]
    public void Serialize_AnArrayOfIntegers_ReturnsArrayAsAString()
    {
        // Arrange
        var value = new[] {100, 200, 300};

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("[100 200 300]");
    }

    [Test]
    public void Serialize_AnArrayOfStrings_ReturnsArrayAsAString()
    {
        // Arrange
        var value = new[] {"Hello", "'this'", "is a test"};

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("[Hello \"'this'\" \"is a test\"]");
    }

    [Test]
    public void Serialize_AnArrayOfBooleans_ReturnsArrayAsAString()
    {
        // Arrange
        var value = new[] {true, false, true};

        // Act
        var result = HatchetConvert.Serialize(value);

        // Assert
        result.Should().Be("[true false true]");
    }
}