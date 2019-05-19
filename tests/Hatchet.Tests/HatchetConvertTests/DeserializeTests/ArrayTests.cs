using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void Deserialize_ArrayOfIntegers_ReturnsArrayOfIntegers()
        {
            // Arrange
            var input = "[1 2 -3 4 5]";

            // Act
            var result = HatchetConvert.Deserialize<int[]>(input);

            // Assert
            result.Should().ContainInOrder(1, 2, -3, 4, 5);
        }

        [Test]
        public void Deserialize_ArrayOfStrings_ReturnsArrayOfStrings()
        {
            // Arrange
            var input = "[This is a test]";

            // Act
            var result = HatchetConvert.Deserialize<string[]>(input);

            // Assert
            result.Should().ContainInOrder("This", "is", "a", "test");
        }

        [Test]
        public void Deserialize_EmptyArrayOfStrings_ReturnEmptyStringArray()
        {
            // Arrange
            var input = "[]";

            // Act
            var result = HatchetConvert.Deserialize<string[]>(input);

            // Assert
            result.Should().HaveCount(0);
            result.Should().BeOfType(typeof (string[]));
        }
    }
}