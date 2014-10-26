using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class EmptyListTests
    {
        [Test]
        public void Deserialize_AnEmptyStringList_ShouldReturnAnEmptyList()
        {
            // Arrange
            var input = "[]";

            // Act
            var result = HatchetConvert.Deserialize<List<string>>(ref input);

            // Assert
            result.Should().HaveCount(0);
        }

        [Test]
        public void Deserialize_AnEmptyIntegerList_ShouldReturnAnEmptyList()
        {
            // Arrange
            var input = "[]";

            // Act
            var result = HatchetConvert.Deserialize<List<int>>(ref input);

            // Assert
            result.Should().HaveCount(0);
        }

        [Test]
        public void Deserialize_AnEmptyFloatList_ShouldReturnAnEmptyList()
        {
            // Arrange
            var input = "[]";

            // Act
            var result = HatchetConvert.Deserialize<List<float>>(ref input);

            // Assert
            result.Should().HaveCount(0);
        }
    }
}