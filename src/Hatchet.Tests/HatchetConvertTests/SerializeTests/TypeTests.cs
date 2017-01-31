using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        class Two : Base
        {
            
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

        [Test]
        public void Serialize_CollectionOfAbstractClass_EachElementHasNameAttributeWritten()
        {
            // Arrange
            var collection = new List<Base> {new One(), new Two()};

            // Act
            var result = HatchetConvert.Serialize(collection);

            // Assert
            result.Should().Contain("Class One");
            result.Should().Contain("Class Two");
        }
    }
}