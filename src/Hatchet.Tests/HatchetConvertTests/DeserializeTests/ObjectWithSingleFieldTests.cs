using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class ObjectWithSingleFieldTests
    {
        private class TestClass
        {
            public string StringField;
        }

        [Test]
        public void Deserialize_WithAnObjectWithOneStringPropertySingleWord_ThePropertyShouldBePopulated()
        {
            // Arrange
            var input = "{ stringField Hello }";

            // Act
            var result = HatchetConvert.Deserialize<TestClass>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.StringField.Should().Be("Hello");
        }

        [Test]
        public void Deserialize_WithAnObjectWithOneStringPropertyLongString_ThePropertyShouldBePopulated()
        {
            // Arrange
            var input = "{ stringField \"Hello World.\" }";

            // Act
            var result = HatchetConvert.Deserialize<TestClass>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.StringField.Should().Be("Hello World.");
        }

        [Test]
        public void Deserialize_WithAnObjectWithOneStringPropertyEmptyString_ThePropertyShouldBePopulated()
        {
            // Arrange
            var input = "{ stringField \"\" }";

            // Act
            var result = HatchetConvert.Deserialize<TestClass>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.StringField.Should().Be(string.Empty);
        }

        [Test]
        public void Deserialize_WithAnObjectWithOneStringPropertyContainingParenthesesString_ThePropertyShouldBePopulated()
        {
            // Arrange
            var input = "{ stringField \"{ Nasty String { Very Nasty String } }\" }";

            // Act
            var result = HatchetConvert.Deserialize<TestClass>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.StringField.Should().Be("{ Nasty String { Very Nasty String } }");
        }
    }
}