using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class TypeFactoryTests
    {
        public class TestClass
        {
            public TestValue Value { get; set; }
        }

        public class TestValue
        {
            public string Value { get; }

            public TestValue(string value)
            {
                Value = value;
            }
        }

        [Test]
        public void Deserialize_PropertyWithTypeConstructedByFactory_ValueIsObtainedFromTheFactory()
        {
            // Arrange
            var input = "{ Value 1234 }";

            // Act
            var result = HatchetConvert.Deserialize<TestClass>(input);

            // Assert
            result.Value.Value.Should().Be("1234");
        }
    }
}