using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.DeserializerTests
{
    [TestFixture]
    public class ListPropertyTests
    {
        [Test]
        public void Deserialize_WithAListOfStringValues_TheValuesAreLoadedIntoAList()
        {
            // Arrange
            var input = "{ items [hello world] }";

            var deserializer = new Deserializer();

            // Act
            var result = (Dictionary<string, object>)deserializer.Parse(ref input);

            // Assert
            result.Should().NotBeNull();
            var list = (List<object>) result["items"];

            list.Should().HaveCount(2);
            ((string) list[0]).Should().Be("hello");
            ((string) list[1]).Should().Be("world");
        } 
    }
}