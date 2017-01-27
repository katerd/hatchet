using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests
{
    [TestFixture]
    public class TypeTests
    {
        abstract class Base
        {
            
        }

        class One : Base
        {
            public string Hello;
        }

        class Container
        {
            public Base Base;
        }

        [SetUp]
        public void Setup()
        {
            HatchetTypeRegistry.Clear();
            HatchetTypeRegistry.Add<One>();
        }

        [Test]
        public void Serialize_SubclassOfAbstractClass_ClassNameIsSerialized()
        {
            // Arrange
            var b = new One {Hello = "World"};

            var c = new Container {Base = b};

            var inter = HatchetConvert.Serialize(c);

            // Act
            var result = HatchetConvert.Deserialize<Container>(inter);

            // Assert
            ((One) result.Base).Hello.Should().Be("World");
        }
    }
}