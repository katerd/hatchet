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

            public TestListObject()
            {
            }

            public TestListObject(string value)
            {
                Value = value;
            }

            [HatchetConstructor]
            public static TestListObject Create(string value)
            {
                return new TestListObject("StaticConstructor-" + value);
            }
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

        [Test]
        public void Deserialize_ListOfTestListObjectUsingHatchetConstructor_InstancesAreCreated()
        {
            // Arrange
            var input = "[{Value Value1} {Value Value2} 'TastyBacon']";
            
            // Act
            var result = HatchetConvert.Deserialize<List<TestListObject>>(input);

            // Assert
            result.Should().HaveCount(3);
            result[2].Value.Should().Be("StaticConstructor-TastyBacon");
        }

        [Test]
        public void Deserialize_ListOfTestObjectUsingSingleParameterHatchetConstructor_InstancesAreCreated()
        {
            // Arrange
            var input = "[super tasty bacon]";
            
            // Act
            var result = HatchetConvert.Deserialize<List<TestListObject>>(input);

            // Assert
            result.Should().HaveCount(3);
            result[0].Value.Should().Be("StaticConstructor-super");
            result[1].Value.Should().Be("StaticConstructor-tasty");
            result[2].Value.Should().Be("StaticConstructor-bacon");
        }
    }
}