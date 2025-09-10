using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests;

[TestFixture]
public class BlockCommentTests
{
    [Test]
    public void Parse_WithABlockCommentInsideAList_ItShouldParseTheRestOfTheList()
    {
        // Arrange
        var input = "[ 1/*a wild comment appears*/2 3 ]";

        var parser = new Parser();

        // Act
        var result = (List<object>) parser.Parse(ref input);

        // Assert
        result.Count.Should().Be(3);
        ((string) result[0]).Should().Be("1");
        ((string) result[1]).Should().Be("2");
        ((string) result[2]).Should().Be("3");
    }

    [Test]
    public void Parse_WithMultipleBlockCommentInsideAList_ItShouldParseTheRestOfTheList()
    {
        // Arrange
        var input = "[/*value1*/1 /*valuie2*/2 /*value3*/3]";

        var parser = new Parser();

        // Act
        var result = (List<object>)parser.Parse(ref input);

        // Assert
        result.Count.Should().Be(3);
        ((string)result[0]).Should().Be("1");
        ((string)result[1]).Should().Be("2");
        ((string)result[2]).Should().Be("3");
    }

    [Test]
    public void Parse_WithABlockCommentBeforeAList_ItShouldParseTheRestOfTheList()
    {
        // Arrange
        var input = "/* example comment */[ 1 2 3 ]";

        var parser = new Parser();

        // Act
        var result = (List<object>)parser.Parse(ref input);

        // Assert
        result.Count.Should().Be(3);
        ((string)result[0]).Should().Be("1");
        ((string)result[1]).Should().Be("2");
        ((string)result[2]).Should().Be("3");
    }

    [Test]
    public void Parse_WithABlockCommentAfterAList_ItShouldParseTheRestOfTheList()
    {
        // Arrange
        var input = "[ 1 2 3 ] /* example comment */";

        var parser = new Parser();

        // Act
        var result = (List<object>)parser.Parse(ref input);

        // Assert
        result.Count.Should().Be(3);
        ((string)result[0]).Should().Be("1");
        ((string)result[1]).Should().Be("2");
        ((string)result[2]).Should().Be("3");
    }

    [Test]
    public void Parse_WithABlockCommentOnly_ItShouldReturnNull()
    {
        // Arrange
        var input = "  /* example comment */  ";

        var parser = new Parser();

        // Act
        var result = parser.Parse(ref input);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Parse_WithEmptyTwoBlockCommentsOnly_ItShouldReturnNull()
    {
        // Arrange
        var input = "/**//**/";

        var parser = new Parser();

        // Act
        var result = parser.Parse(ref input);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Parse_WithSparseTwoBlockCommentsOnly_ItShouldReturnNull()
    {
        // Arrange
        var input = "  /* example comment */  /* second block comment */ ";

        var parser = new Parser();

        // Act
        var result = parser.Parse(ref input);

        // Assert
        result.Should().BeNull();
    }
}