using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests
{
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
    }
}