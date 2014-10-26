using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class IntegerListTests
    {
        [Test]
        public void Deserialize_AStringList_ShouldReturnAListOfStrings()
        {
            // Arrange
            var input = "[100 200 300 -400]";

            // Act
            var result = HatchetConvert.Deserialize<List<int>>(ref input);

            // Assert
            result.Should().HaveCount(4);
            result[0].Should().Be(100);
            result[1].Should().Be(200);
            result[2].Should().Be(300);
            result[3].Should().Be(-400);
        }
    }
}