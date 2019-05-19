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

        public class TestClassTwo
        {
            public TestValueTwo Value { get; set; }
        }

        public class TestValueTwo
        {
            public string Value { get; set; }

            [HatchetConstructor]
            public static TestValueTwo Create(string value)
            {
                return new TestValueTwo { Value = value };
            }
        }

        [Test]
        public void Deserialize_PropertyWithTypeThatHasAConstructorThatTakesAString_ValueIsObtainedFromTheFactory()
        {
            // Arrange
            var input = "{ Value 1234 }";

            // Act
            var result = HatchetConvert.Deserialize<TestClass>(input);

            // Assert
            result.Value.Value.Should().Be("1234");
        }

        [Test]
        public void Deserialize_PropertyWithTypeThatHasAStaticConstructorMethod_ValueIsObtainedFromTheFactory()
        {
            // Arrange
            var input = "{ Value 1234 }";

            // Act
            var result = HatchetConvert.Deserialize<TestClassTwo>(input);

            // Assert
            result.Value.Value.Should().Be("1234");
        }


    }
}