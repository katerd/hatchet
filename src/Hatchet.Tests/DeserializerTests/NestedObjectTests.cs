using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.DeserializerTests
{
    [TestFixture]
    public class NestedObjectTests
    {
        [Test]
        public void Deserialize_NestedObjects_ObjectsAreNestedWithTheirProperties()
        {
            // Arrange
            var input = "{ outer { inner { name innerProperty } name outerProperty } } }";

            var deserializer = new Deserializer();

            // Act
            var result = (Dictionary<string, object>)deserializer.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            var outer = (Dictionary<string, object>) result["outer"];
            outer.Should().NotBeNull();

            var inner = (Dictionary<string, object>)outer["inner"];
            inner.Should().NotBeNull();

            ((string) outer["name"]).Should().Be("outerProperty");
            ((string) inner["name"]).Should().Be("innerProperty");
        }

        [Test]
        public void Deserialize_NestedObjectsCondensedString_ObjectsAreNestedWithTheirProperties()
        {
            // Arrange
            var input = "{outer{inner{name'innerProperty'}name'outerProperty'}}}";

            var deserializer = new Deserializer();

            // Act
            var result = (Dictionary<string, object>)deserializer.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            var outer = (Dictionary<string, object>)result["outer"];
            outer.Should().NotBeNull();

            var inner = (Dictionary<string, object>)outer["inner"];
            inner.Should().NotBeNull();

            ((string)outer["name"]).Should().Be("outerProperty");
            ((string)inner["name"]).Should().Be("innerProperty");
        }
    }
}