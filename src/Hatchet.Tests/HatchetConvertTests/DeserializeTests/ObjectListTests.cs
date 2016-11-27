using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class ObjectListTests
    {
        private class TestListObject
        {
            public string Value { get; set; }
            public int I { get; set; }
        }

        [Test]
        public void Deserialize_ListOfTestListObject_PropertiesArePopulated()
        {
            // Arrange
            var input = "[{ Value Value1 I 100} {Value Value2 I 200}]";

            // Act
            var result = HatchetConvert.Deserialize<List<TestListObject>>(input);

            // Assert
            result.Should().HaveCount(2);
            result[0].Value.Should().Be("Value1");
            result[0].I.Should().Be(100);

            result[1].Value.Should().Be("Value2");
            result[1].I.Should().Be(200);
        }
    }
}