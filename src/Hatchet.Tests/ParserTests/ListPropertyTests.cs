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

        [Test]
        public void Deserialize_WithNakedListOfStringValues_TheValuesAreLoadedIntoAList()
        {
            // Arrange
            var input = "[hello world]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            result.Should().HaveCount(2);
            ((string)result[0]).Should().Be("hello");
            ((string)result[1]).Should().Be("world");
        }

        [Test]
        public void Deserialize_WithNakedListOfIntegerValues_TheValuesAreLoadedIntoAList()
        {
            // Arrange
            var input = "[1111 2222 -3333]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            result.Should().HaveCount(3);
            ((string)result[0]).Should().Be("1111");
            ((string)result[1]).Should().Be("2222");
            ((string)result[2]).Should().Be("-3333");
        }

        [Test]
        public void Deserialize_WithNakedAListOfObjects_TheObjectsShouldBeLoadedIntoTheList()
        {
            // Arrange
            var input = "[ { name ONE} { name TWO } ]";

            var parser = new Parser();

            // Act
            var result = (List<object>) parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var one = (Dictionary<string, object>) result[0];
            one["name"].Should().Be("ONE");

            var two = (Dictionary<string, object>)result[1];
            two["name"].Should().Be("TWO");

        }
    }
}