using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests
{
    [TestFixture]
    public class SingleStringPropertyTests
    {
        [Test]
        public void Deserialize_WithAnObjectWithOneStringPropertySingleWord_ThePropertyShouldBePopulated()
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
        public void Deserialize_WithAnObjectWithOneStringPropertyLongString_ThePropertyShouldBePopulated()
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
        public void Deserialize_WithLotsOfExtraSpaces_ThePropertyShouldBePopulated()
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
        public void Deserialize_WithAnObjectWithOneStringPropertyEmptyString_ThePropertyShouldBePopulated()
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
        public void Deserialize_WithAnObjectWithOneStringPropertyContainingParenthesesString_ThePropertyShouldBePopulated()
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
        public void Deserialize_WithAnObjectWithOneStringPropertyContainingQuotes_ThePropertyShouldBePopulated()
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
        public void Deserialize_WithAnObjectWithOneStringPropertyContainingEscapedQuotes_ThePropertyShouldBePopulated()
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
}