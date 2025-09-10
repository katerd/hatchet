using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

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

    class HasStaticFields
    {
#pragma warning disable 169, 414
        public static string One = "One";
        internal static string Two = "Two";
        private static string Three = "Three";
#pragma warning restore 169, 414
    }

    class HasStaticProperties
    {
        public static string One { get; set; } = "One";
        public static string Two { get; set; } = "Two";
        public static string Three { get; set; } = "Three";
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

    [Test]
    public void Serialize_ClassWithStaticFields_StaticPropertiesAreNotSerialized()
    {
        // Arrange
        var input = new HasStaticFields();

        // Act
        var result = HatchetConvert.Serialize(input);

        // Assert
        result.Should().NotContain("One");
        result.Should().NotContain("Two");
        result.Should().NotContain("Three");
    }

    [Test]
    public void Serialize_ClassWithStaticProperties_StaticPropertiesAreNotSerialized()
    {
        // Arrange
        var input = new HasStaticProperties();

        // Act
        var result = HatchetConvert.Serialize(input);

        // Assert
        result.Should().NotContain("One");
        result.Should().NotContain("Two");
        result.Should().NotContain("Three");
    }
}