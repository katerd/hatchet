using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests;

[TestFixture]
public class EmptyFileTests
{
    [Test]
    public void Parse_EmptyFile_ReturnsNull()
    {
        // Arrange
        var input = "";

        var parser = new Parser();

        // Act
        var result = parser.Parse(ref input);

        // Assert
        result.Should().BeNull();
    } 
}