using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests
{
    [TestFixture]
    public class ListTests
    {
        [Test]
        public void Serialize_AnEmptyList_ReturnsValueAsAString()
        {
            // Arrange
            var emptyList = new List<string>();

            // Act
            var result = HatchetConvert.Serialize(emptyList);

            // Assert
            result.Should().Be("[]");
        }

        [Test]
        public void Serialize_AListOfStrings_ReturnsValueAsAString()
        {
            // Arrange
            var list = new List<string> {"This", "is", "'a test'"};

            // Act
            var result = HatchetConvert.Serialize(list);

            // Assert
            result.Should().Be("[This is 'a test']");
        }

        [Test]
        public void Serialize_AListOfIntegers_ReturnsValueAsAString()
        {
            // Arrange
            var list = new List<int> {100, 200, -300};

            // Act
            var result = HatchetConvert.Serialize(list);

            // Assert
            result.Should().Be("[100 200 -300]");
        }
    }
}