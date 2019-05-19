using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests
{
    [TestFixture]
    public class LineCommentTests
    {
        [Test]
        public void Parse_WithEmptyLineCommentOnly_ReturnsNull()
        {
            // Arrange
            var input = "//";

            var parser = new Parser();

            // Act
            var result = parser.Parse(ref input);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Parse_WithLineCommentOnlyNoLineEnd_ReturnsNull()
        {
            // Arrange
            var input = "// this is a line comment";

            var parser = new Parser();

            // Act
            var result = parser.Parse(ref input);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Parse_WithLineCommentOnlyWindowsLineEnd_ReturnsNull()
        {
            // Arrange
            var input = "// this is a line comment\r\n";

            var parser = new Parser();

            // Act
            var result = parser.Parse(ref input);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Parse_WithLineCommentOnlyUnixLineEnd_ReturnsNull()
        {
            // Arrange
            var input = "// this is a line comment\n";

            var parser = new Parser();

            // Act
            var result = parser.Parse(ref input);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Parse_WithLineCommentOnlyMacLineEnd_ReturnsNull()
        {
            // Arrange
            var input = "// this is a line comment\r";

            var parser = new Parser();

            // Act
            var result = parser.Parse(ref input);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Parse_WithLineCommentAndString_ReturnsString()
        {
            // Arrange
            var input = "// this is a sample string\n\"Sample String\"";

            var parser = new Parser();

            // Act
            var result = parser.Parse(ref input);

            // Assert
            result.Should().Be("Sample String");
        }

        [Test]
        public void Parse_WithLineCommentsSurroundingString_ReturnsString()
        {
            // Arrange
            var input = "// this is a sample string\n\"Sample String\"\nSimples.";

            var parser = new Parser();

            // Act
            var result = parser.Parse(ref input);

            // Assert
            result.Should().Be("Sample String");
        }

        [Test]
        public void Parse_MultiLineListWithSurroundingSingleLineCommentsUnix_ReturnsListWithCorrectValues()
        {
            // Arrange
            var input =
                "// This is a list \n [ 'one' \n // This is a comment \n 'two' \n // This is also a comment \r 'three' \n // More comments \n ] \n \\ The end";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().HaveCount(3);
            ((string)result[0]).Should().Be("one");
            ((string)result[1]).Should().Be("two");
            ((string)result[2]).Should().Be("three");
        }

        [Test]
        public void Parse_MultiLineListWithSurroundingSingleLineCommentsWindows_ReturnsListWithCorrectValues()
        {
            // Arrange
            var input =
                "// This is a list \r\n [ 'one' \r\n // This is a comment \n 'two' \r\n // This is also a comment \r 'three' \r\n // More comments \r\n ] \r\n \\ The end";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().HaveCount(3);
            ((string)result[0]).Should().Be("one");
            ((string)result[1]).Should().Be("two");
            ((string)result[2]).Should().Be("three");
        }

        [Test]
        public void Parse_MultiLineListWithSurroundingSingleLineCommentsMac_ReturnsListWithCorrectValues()
        {
            // Arrange
            var input =
                "// This is a list \r [ 'one' \r // This is a comment \r 'two' \r // This is also a comment \r 'three' \r // More comments \r ] \r \\ The end";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().HaveCount(3);
            ((string)result[0]).Should().Be("one");
            ((string)result[1]).Should().Be("two");
            ((string)result[2]).Should().Be("three");
        }

        [Test]
        public void Parse_MultiLineListWithSingleLineCommentsUnix_ReturnsListWithCorrectValues()
        {
            // Arrange
            var input =
                "[ 'one' \n // This is a comment \n 'two' \n // This is also a comment \n 'three' \n // More comments \n ]";

            var parser = new Parser();
            
            // Act
            var result = (List<object>) parser.Parse(ref input);

            // Assert
            result.Should().HaveCount(3);
            ((string) result[0]).Should().Be("one");
            ((string) result[1]).Should().Be("two");
            ((string) result[2]).Should().Be("three");
        }

        [Test]
        public void Parse_MultiLineListWithSingleLineCommentsWindows_ReturnsListWithCorrectValues()
        {
            // Arrange
            var input =
                "[ 'one' \r\n // This is a comment \n 'two' \r\n // This is also a comment \r\n 'three' \r\n // More comments \r ]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().HaveCount(3);
            ((string)result[0]).Should().Be("one");
            ((string)result[1]).Should().Be("two");
            ((string)result[2]).Should().Be("three");
        }

        [Test]
        public void Parse_MultiLineListWithSingleLineCommentsMac_ReturnsListWithCorrectValues()
        {
            // Arrange
            var input =
                "[ 'one' \r // This is a comment \r 'two' \r // This is also a comment \r 'three' \r // More comments \r ]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().HaveCount(3);
            ((string)result[0]).Should().Be("one");
            ((string)result[1]).Should().Be("two");
            ((string)result[2]).Should().Be("three");
        }

        [Test]
        public void Parse_ObjectDefinitionWithSingleLineComments_ReturnsObjectWithCorrectValue()
        {
            // Arrange
            var input = "{ // Magic incoming \n needsMagic true \n//\n// We always need magic\n// \n }";

            var parser = new Parser();

            // Act
            var result = (Dictionary<string, object>) parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();
            ((string) result["needsMagic"]).Should().Be("true");
        }
    }
}