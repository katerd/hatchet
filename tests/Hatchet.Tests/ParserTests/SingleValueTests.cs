using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests;

[TestFixture]
public class SingleValueTests
{
    [Test]
    public void Parse_SingleWorld_ReturnsString()
    {
        // Arrange
        var input = "awesome";

        var parser = new Parser();

        // Act
        var result = (string)parser.Parse(ref input);

        // Assert
        result.Should().Be("awesome");
    }

    [Test]
    public void Parse_PositiveInteger_ReturnsString()
    {
        // Arrange
        var input = "1000";

        var parser = new Parser();

        // Act
        var result = (string)parser.Parse(ref input);

        // Assert
        result.Should().Be("1000");
    }

    [Test]
    public void Parse_NegativeInteger_ReturnsString()
    {
        // Arrange
        var input = "-1000";

        var parser = new Parser();

        // Act
        var result = (string)parser.Parse(ref input);

        // Assert
        result.Should().Be("-1000");
    }

    [Test]
    public void Parse_PositiveFloat_ReturnsString()
    {
        // Arrange
        var input = "3.141";

        var parser = new Parser();

        // Act
        var result = (string)parser.Parse(ref input);

        // Assert
        result.Should().Be("3.141");
    }

    [Test]
    public void Parse_NegativeFloat_ReturnsString()
    {
        // Arrange
        var input = "-3.141";

        var parser = new Parser();

        // Act
        var result = (string)parser.Parse(ref input);

        // Assert
        result.Should().Be("-3.141");
    }

    [Test]
    public void Parse_FilePathWindows_ReturnsString()
    {
        // Arrange
        var input = @"c:\windows\system32\drivers\etc\hosts";

        var parser = new Parser();

        // Act
        var result = (string)parser.Parse(ref input);

        // Assert
        result.Should().Be(input);
    }

    [Test]
    public void Parse_FilePathUnix_ReturnsString()
    {
        // Arrange
        var input = @"/dev/sda1";

        var parser = new Parser();

        // Act
        var result = (string)parser.Parse(ref input);

        // Assert
        result.Should().Be(input);
    } 

    [Test]            
    public void Parse_QuotedString_ReturnsString()
    {
        // Arrange
        var input = "'Hello world'";

        var parser = new Parser();

        // Act
        var result = (string) parser.Parse(ref input);

        // Assert
        result.Should().Be("Hello world");
    }        
        
    [Test]            
    public void Parse_QuotedNonAsciiString_ReturnsString()
    {
        // Arrange
        var input = "'Ĥēĺĺō world'";

        var parser = new Parser();

        // Act
        var result = (string) parser.Parse(ref input);

        // Assert
        result.Should().Be("Ĥēĺĺō world");
    }
}