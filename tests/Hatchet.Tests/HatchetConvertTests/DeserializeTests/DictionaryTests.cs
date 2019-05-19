using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class DictionaryTests
    {
        private class DemoClass
        {
            public Dictionary<string, string> Dict;

            public DemoClass()
            {
                Dict = new Dictionary<string, string>();
            }
        }

        private enum DemoEnum
        {
            Red,
            Green,
            Blue
        }

        [Test]
        public void Deserialize_EmptyDictionaryInClass_ReturnsEmptyDictionaryInClass()
        {
            // Arrange
            var raw = "{ Dict {} }";

            // Act
            var result = HatchetConvert.Deserialize<DemoClass>(raw);

            // Assert
            result.Dict.Should().BeEmpty();
        }

        [Test]
        public void Deserialize_DictionaryInClass_ReturnsDictionaryInClass()
        {
            // Arrange
            var raw = "{ Dict { Hello World } }";

            // Act
            var result = HatchetConvert.Deserialize<DemoClass>(raw);

            // Assert
            result.Dict["Hello"].Should().Be("World");
        }

        [Test]
        public void Deserialize_NestedDictionary_ReturnsNestedDictionary()
        {
            // Arrange
            var raw = " { outerKey { innerKey innerValue} }";

            // Act
            var result = HatchetConvert.Deserialize<Dictionary<string, Dictionary<string, string>>>(raw);

            // Assert
            result["outerKey"]["innerKey"].Should().Be("innerValue");
        }

        [Test]
        public void Deserialize_DictionaryWithEnumKey_ReturnsEnumKeyedDictionary()
        {
            // Arrange
            var raw = " { Red 10 Blue 8 }";

            // Act
            var result = HatchetConvert.Deserialize<Dictionary<DemoEnum, float>>(raw);

            // Assert
            result[DemoEnum.Red].Should().BeApproximately(10, 0.0001f);
            result[DemoEnum.Blue].Should().BeApproximately(8, 0.0001f);
        }
    }
}