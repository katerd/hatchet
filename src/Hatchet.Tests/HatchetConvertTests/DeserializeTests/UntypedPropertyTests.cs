using System.Collections;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class UntypedPropertyTests
    {
        private class ClassWithObject
        {
            public object Prop { get; set; }
        }

        [Test]
        public void Convert_ClassWithObjectPropertyFromADictionary_ItShouldDeserializeIntoAnIDictionary()
        {
            // Arrange
            var input = "{ Prop { A 100 B 200 }}";

            // Act
            var result = HatchetConvert.Deserialize<ClassWithObject>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.Prop.Should().NotBeNull();
            result.Prop.Should().BeAssignableTo<IDictionary>();

            var asDictionary = (IDictionary)result.Prop;
            asDictionary["A"].Should().Be("100");
            asDictionary["B"].Should().Be("200");
        }

        [Test]
        public void Convert_ClassWithObjectPropertyFromAList_ItShouldDeserializeIntoAnIList()
        {
            // Arrange
            var input = "{ Prop [ 100 200 300 ] } ";

            // Act
            var result = HatchetConvert.Deserialize<ClassWithObject>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.Prop.Should().NotBeNull();
            result.Prop.Should().BeAssignableTo<IList>();

            var asList = (IList) result.Prop;
            asList[0].Should().Be("100");
            asList[1].Should().Be("200");
            asList[2].Should().Be("300");
        }

        [Test]
        public void Convert_ClassWithObjectPropertyFromAString_ItShouldDeserializeIntoAString()
        {
            // Arrange
            var input = "{ Prop \"This is a prop\" }";

            // Act
            var result = HatchetConvert.Deserialize<ClassWithObject>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.Prop.Should().NotBeNull();
            result.Prop.Should().Be("This is a prop");
        }

        [Test]
        public void Convert_ClassWithObjectPropertyFromAnInteger_ItShouldDeserializeIntoAString()
        {
            // Arrange
            var input = "{ Prop 100 }";

            // Act
            var result = HatchetConvert.Deserialize<ClassWithObject>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.Prop.Should().NotBeNull();
            result.Prop.Should().Be("100");
        }
    }
}