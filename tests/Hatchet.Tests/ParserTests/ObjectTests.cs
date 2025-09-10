using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests;

[TestFixture]
public class ObjectTests
{
    [Test]
    public void Parse_WithAnObjectWithOneStringPropertySingleWord_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty Hello }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        ((string)result["stringProperty"]).Should().Be("Hello");
    }

    [Test]
    public void Parse_WithAnObjectWithOneStringPropertyWithUnderscore_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty Hello_World }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        ((string)result["stringProperty"]).Should().Be("Hello_World");
    }

    [Test]
    public void Parse_WithAnObjectWithOneStringPropertyLongString_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"Hello World.\" }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        result.Should().NotBeNull();
        ((string)result["stringProperty"]).Should().Be("Hello World.");
    }

    [Test]
    public void Parse_WithLotsOfExtraSpaces_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "    { \n stringProperty  \n \"Hello World.\" \n\n      }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        result.Should().NotBeNull();
        ((string)result["stringProperty"]).Should().Be("Hello World.");
    }

    [Test]
    public void Parse_WithAnObjectWithOneStringPropertyEmptyString_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"\" }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        result.Should().NotBeNull();
        ((string)result["stringProperty"]).Should().Be("");
    }

    [Test]
    public void Parse_WithAnObjectWithOneStringPropertyContainingParenthesesString_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"{ Nasty String { Very Nasty String } }\" }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        result.Should().NotBeNull();
        ((string)result["stringProperty"]).Should().Be("{ Nasty String { Very Nasty String } }");
    }

    [Test]
    public void Parse_WithAnObjectWithOneStringPropertyContainingQuotes_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"Hello 'Person'\" }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        result.Should().NotBeNull();
        ((string)result["stringProperty"]).Should().Be("Hello 'Person'");
    }

    [Test]
    public void Parse_WithAnObjectWithOneStringPropertyContainingEscapedQuotes_ThePropertyShouldBePopulated()
    {
        // Arrange
        var input = "{ stringProperty \"Hello \\\"Person\\\"\" }";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>)parser.Parse(ref input);

        // Assert
        result.Should().NotBeNull();
        ((string)result["stringProperty"]).Should().Be("Hello \"Person\"");
    }
}