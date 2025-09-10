using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests;

[TestFixture]
public class ArrayTests
{
    [Test]
    public void Deserialize_ArrayOfIntegers_ReturnsArrayOfIntegers()
    {
        // Arrange
        const string input = "[1 2 -3 4 5]";

        // Act
        var result = HatchetConvert.Deserialize<int[]>(input);

        // Assert
        result.Should().Equal(1, 2, -3, 4, 5);
    }

    [Test]
    public void Deserialize_ArrayOfStrings_ReturnsArrayOfStrings()
    {
        // Arrange
        const string input = "[This is a test]";

        // Act
        var result = HatchetConvert.Deserialize<string[]>(input);

        // Assert
        result.Should().Equal("This", "is", "a", "test");
    }

    [Test]
    public void Deserialize_EmptyArrayOfStrings_ReturnEmptyStringArray()
    {
        // Arrange
        const string input = "[]";

        // Act
        var result = HatchetConvert.Deserialize<string[]>(input);

        // Assert
        result.Should().BeOfType<string[]>();
        result.Should().BeEmpty();
    }
    
    [Test]
    public void Deserialize_ArrayOfNulls_ReturnEmptyStringArray()
    {
        // Arrange
        const string input = "[null null null]";

        // Act
        var result = HatchetConvert.Deserialize<object[]>(input);

        // Assert
        result.Should().Equal((object)null, null, null);
    }
}
