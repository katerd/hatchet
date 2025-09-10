using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests;

[TestFixture]
public class NonTerminatedTests
{
    [Test]
    public void Parse_UnterminatedObjectInList_ShouldThrowException()
    {
        // Arrange
        var input = "[{}{]";

        var parser = new Parser();

        // Act
        // Assert
        parser.Invoking(p => p.Parse(ref input))
            .Should().Throw<HatchetException>()
            .WithMessage("Expected any of `}` at byte 5");
    }

    [Test]
    public void Parse_UnterminatedNestedList_ShouldThrowException()
    {
        // Arrange
        var input = "[[]";

        var parser = new Parser();

        // Act
        // Assert
        parser.Invoking(p => p.Parse(ref input))
            .Should().Throw<HatchetException>()
            .WithMessage("List opened at byte 1 is not closed");
    }

    [Test]
    public void Parse_WithUnterminatedObject_ShouldThrowException()
    {
        // Arrange
        var input = "{";

        var parser = new Parser();

        // Act
        // Assert
        parser.Invoking(p => p.Parse(ref input))
            .Should().Throw<HatchetException>()
            .WithMessage("Expected `}` at byte 2");
    }

    [Test]
    public void Parse_WithLoneStringThatIsNotTerminated_ShouldThrowException()
    {
        // Arrange
        var input = "'This is a string";

        var parser = new Parser();

        // Act
        // Assert
        parser.Invoking(p => p.Parse(ref input))
            .Should().Throw<HatchetException>()
            .WithMessage("String starting at byte 1 is not terminated");
    }

    [Test]
    public void Parse_WithStringInListThatIsNotTerminated_ShouldThrowException()
    {
        // Arrange
        var input = "['string one' 'string two]";

        var parser = new Parser();

        // Act
        // Assert
        parser.Invoking(p => p.Parse(ref input))
            .Should().Throw<HatchetException>()
            .WithMessage("String starting at byte 15 is not terminated");
    }
}