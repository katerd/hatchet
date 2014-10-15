using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests
{
    [TestFixture]
    public class ListPropertyTests
    {
        [Test]
        public void Deserialize_WithAListOfStringValues_TheValuesAreLoadedIntoAList()
        {
            // Arrange
            var input = "{ items [hello world] }";

            var parser = new Parser();

            // Act
            var result = (Dictionary<string, object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();
            var list = (List<object>) result["items"];

            list.Should().HaveCount(2);
            ((string) list[0]).Should().Be("hello");
            ((string) list[1]).Should().Be("world");
        } 
    }
}