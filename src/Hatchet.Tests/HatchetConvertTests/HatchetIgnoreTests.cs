using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests
{
    [TestFixture]
    public class HatchetIgnoreTests
    {
        private class IgnoredProperty
        {
            public string NotIgnored { get; set; }

            [HatchetIgnore]
            public string Ignored { get; set; }
        }

        private class IgnoredField
        {
            public string NotIgnored;

            [HatchetIgnore]
            public string Ignored;
        }

        [Test]
        public void SerializeObjectEndToEnd_WithIgnoredProperty_ThePropertyShouldNotPersist()
        {
            // Arrange
            var input = new IgnoredProperty { NotIgnored = "Hello", Ignored = "World"};
            var raw = HatchetConvert.Serialize(input);

            // Act
            var result = HatchetConvert.Deserialize<IgnoredProperty>(raw);

            // Assert
            raw.Should().NotContain("Ignored World", "Ignored property should not be serialized");
            result.NotIgnored.Should().Be("Hello", "Non-ignored property should not be affected");
            result.Ignored.Should().Be(default(string), "Ignored property should not be set");
        }

        [Test]
        public void SerializeObjectEndToEnd_WithIgnoredField_TheFieldShouldNotPersist()
        {
            // Arrange
            var input = new IgnoredField { NotIgnored = "Hello", Ignored = "World" };
            var raw = HatchetConvert.Serialize(input);

            // Act
            var result = HatchetConvert.Deserialize<IgnoredField>(raw);

            // Assert
            raw.Should().NotContain("Ignored World", "Ignored field should not be serialized");
            result.NotIgnored.Should().Be("Hello", "Non-ignored property should not be affected");
            result.Ignored.Should().Be(default(string), "Ignored field should not be set");
        }

        [Test]
        public void DeserializeObject_WithIgnoredProperty_ThePropertyShouldHaveADefaultValue()
        {
            // Arrange
            var input = "{ NotIgnored Hello Ignored World }";

            // Act
            var result = HatchetConvert.Deserialize<IgnoredProperty>(input);

            // Assert
            result.Ignored.Should().Be(default(string));
        }

        [Test]
        public void DeserializeObject_WithIgnoredField_ThePropertyShouldHaveADefaultValue()
        {
            // Arrange
            var input = "{ NotIgnored Hello Ignored World }";

            // Act
            var result = HatchetConvert.Deserialize<IgnoredField>(input);

            // Assert
            result.Ignored.Should().Be(default(string));
        }

        [Test]
        public void SerializeObject_WithIgnoredField_PropertyShouldNotBeWritten()
        {
            // Arrange
            var input = new IgnoredField { NotIgnored = "Hello", Ignored = "World" };

            // Act
            var result = HatchetConvert.Serialize(input);

            // Assert
            result.Should().NotContain("Ignored World");
        }

        [Test]
        public void SerializeObject_WithIgnoredProperty_PropertyShouldNotBeWritten()
        {
            // Arrange
            var input = new IgnoredProperty { NotIgnored = "Hello", Ignored = "World"};

            // Act
            var result = HatchetConvert.Serialize(input);

            // Assert
            result.Should().NotContain("Ignored World");
        }
    }
}