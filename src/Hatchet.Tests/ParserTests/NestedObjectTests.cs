using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests
{
    [TestFixture]
    public class NestedObjectTests
    {
        [Test]
        public void Parse_NestedObjects_ObjectsAreNestedWithTheirProperties()
        {
            // Arrange
            var input = "{ outer { inner { name innerProperty } name outerProperty } } }";

            var parser = new Parser();

            // Act
            var result = (Dictionary<string, object>)parser.Parse(ref input);

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
        public void Parse_NestedObjectsCondensedString_ObjectsAreNestedWithTheirProperties()
        {
            // Arrange
            var input = "{outer{inner{name'innerProperty'}name'outerProperty'}}}";

            var deserializer = new Parser();

            // Act
            var parser = (Dictionary<string, object>)deserializer.Parse(ref input);

            // Assert
            parser.Should().NotBeNull();

            var outer = (Dictionary<string, object>)parser["outer"];
            outer.Should().NotBeNull();

            var inner = (Dictionary<string, object>)outer["inner"];
            inner.Should().NotBeNull();

            ((string)outer["name"]).Should().Be("outerProperty");
            ((string)inner["name"]).Should().Be("innerProperty");
        }
    }
}