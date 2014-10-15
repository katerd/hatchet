using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.ParserTests
{
    [TestFixture]
    public class ListPropertyTests
    {
        [Test]
        public void Parse_WithAListOfStringValues_TheValuesAreLoadedIntoAList()
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
        public void Parse_WithAListOfEmptyLists_TheEmptyListsAreInstantiated()
        {
            // Arrange
            var input = "[[][]]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            var list1 = (List<object>)result[0];
            list1.Should().HaveCount(0);
            var list2 = (List<object>)result[1];
            list2.Should().HaveCount(0);
        }

        [Test]
        public void Parse_WithAnEmptyList_TheEmptyListIsInstantiated()
        {
            // Arrange
            var input = " [ ] ";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            result.Should().HaveCount(0);
        }

        [Test]
        public void Parse_WithAListOfListsWithIntegers_TheChildListsAreLoadedWithTheirValues()
        {
            // Arrange
            var input = "[[1 2 3][-4 -5 -6]]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            var list1 = (List<object>) result[0];
            list1[0].Should().Be("1");
            list1[1].Should().Be("2");
            list1[2].Should().Be("3");

            var list2 = (List<object>) result[1];
            list2[0].Should().Be("-4");
            list2[1].Should().Be("-5");
            list2[2].Should().Be("-6");
        }

        [Test]
        public void Parse_WithASprawlingListOfListsWithIntegers_TheChildListsAreLoadedWithTheirValues()
        {
            // Arrange
            var input = "[\n\n[1\t2\n\n3]\n[-4\t-5\t-6]\n]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            var list1 = (List<object>)result[0];
            list1[0].Should().Be("1");
            list1[1].Should().Be("2");
            list1[2].Should().Be("3");

            var list2 = (List<object>)result[1];
            list2[0].Should().Be("-4");
            list2[1].Should().Be("-5");
            list2[2].Should().Be("-6");
        }

        [Test]
        public void Parse_WithNakedListOfStringValues_TheValuesAreLoadedIntoAList()
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
        public void Parse_WithNakedListOfIntegerValues_TheValuesAreLoadedIntoAList()
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
        public void Parse_WithNakedListOfFloatValues_TheValuesAreLoadedIntoAList()
        {
            // Arrange
            var input = "[8.6968612 -3.477894 -1.987779832 0.0]";

            var parser = new Parser();

            // Act
            var result = (List<object>)parser.Parse(ref input);

            // Assert
            result.Should().NotBeNull();

            result.Should().HaveCount(4);
            ((string)result[0]).Should().Be("8.6968612");
            ((string)result[1]).Should().Be("-3.477894");
            ((string)result[2]).Should().Be("-1.987779832");
            ((string)result[3]).Should().Be("0.0");
        }

        [Test]
        public void Parse_WithNakedAListOfObjects_TheObjectsShouldBeLoadedIntoTheList()
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