using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests;

[TestFixture]
public class ObjectWithSinglePropertyTests
{
    private class TestClass
    {
        public string StringProperty { get; set; } 
    }

    [Test]
    public void Deserialize_WithAnObjectWithOneStringPropertySingleWord_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty Hello }";

        // Act
        var result = HatchetConvert.Deserialize<TestClass>(input);

        // Assert
        result.Should().NotBeNull();
        result.StringProperty.Should().Be("Hello");
    }

    [Test]
    public void Deserialize_WithAnObjectWithOneStringPropertyLongString_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"Hello World.\" }";

        // Act
        var result = HatchetConvert.Deserialize<TestClass>(input);

        // Assert
        result.Should().NotBeNull();
        result.StringProperty.Should().Be("Hello World.");
    }

    [Test]
    public void Deserialize_WithAnObjectWithOneStringPropertyEmptyString_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"\" }";

        // Act
        var result = HatchetConvert.Deserialize<TestClass>(input);

        // Assert
        result.Should().NotBeNull();
        result.StringProperty.Should().Be(string.Empty);
    }

    [Test]
    public void Deserialize_WithAnObjectWithOneStringPropertyContainingParenthesesString_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"{ Nasty String { Very Nasty String } }\" }";

        // Act
        var result = HatchetConvert.Deserialize<TestClass>(input);

        // Assert
        result.Should().NotBeNull();
        result.StringProperty.Should().Be("{ Nasty String { Very Nasty String } }");
    } 
}