using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests;

[TestFixture]
public class BadStructureTests
{
    [Test]
    public void Parse_WithObjectMissingValue_ShouldThrowException()
    {
        // Arrange
        var input = "{ name }";

        var parser = new Parser();

        // Act
        // Assert
        parser.Invoking(p => p.Parse(ref input))
            .Should().Throw<HatchetException>()
            .WithMessage("Property `name` defined at byte 3 is missing a value");
    }
}