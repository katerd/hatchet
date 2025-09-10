using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests;

[TestFixture]
public class TextBlockTests
{
    [Test]
    public void Parse_WithNakedTextBlock_ReturnsText()
    {
        // Arrange
        var input = "![This is some text]!";

        var parser = new Parser();

        // Act
        var result = (string) parser.Parse(ref input);

        // Assert
        result.Should().Be("This is some text");
    }

    [Test]
    public void Parse_WithTextBlockInObject_ReturnsText()
    {
        // Arrange
        var input = "{ value ![This is some \n text]!}";

        var parser = new Parser();

        // Act
        var result = (Dictionary<string, object>) parser.Parse(ref input);

        // Assert
        var str = (string) result["value"];
        str.Should().Be("This is some \n text");
    }

    [Test]
    public void Parse_WithListOfTextBlocks_ReturnsListWithText()
    {
        // Arrange
        var input = "[![String one]!\n![String two]!]";

        var parser = new Parser();

        // Act
        var result = (List<object>)parser.Parse(ref input);

        // Assert
        var str1 = (string)result[0];
        str1.Should().Be("String one");

        var str2 = (string)result[1];
        str2.Should().Be("String two");

    }
}