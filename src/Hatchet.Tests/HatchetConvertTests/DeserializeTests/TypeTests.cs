using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class TypeByGenericTests : TypeTests
    {
        [SetUp]
        public void Setup()
        {
            HatchetTypeRegistry.Clear();
            HatchetTypeRegistry.Add<One>();
            HatchetTypeRegistry.Add<Two>();
        }
    }

    [TestFixture]
    public class TypeByTypeInfoTests : TypeTests
    {
        [SetUp]
        public void Setup()
        {
            HatchetTypeRegistry.Clear();
            HatchetTypeRegistry.Add(typeof(One));
            HatchetTypeRegistry.Add(typeof(Two));
        }
    }

    public abstract class TypeTests
    {
        [Test]
        public void Deserialize_ObjectOfTypeOne_CorrectInstanceIsReturned()
        {
            // Arrange
            var input = "{ Class One Value Hello }";

            // Act
            var result = HatchetConvert.Deserialize<Base>(input);

            // Assert
            result.Should().BeOfType<One>();
        }

        [Test]
        public void Deserialize_ObjectOfTypeTwo_CorrectInstanceIsReturned()
        {
            // Arrange
            var input = "{ Class Two Value Hello }";

            // Act
            var result = HatchetConvert.Deserialize<Base>(input);

            // Assert
            result.Should().BeOfType<Two>();
        }

        [Test]
        public void Deserialize_CollectionOfAbstractTypesByName_CollectionIsReturned()
        {
            // Arrange
            var input = "[ @One { Value COne } @Two { Value CTwo } ]";

            // Act
            var result = HatchetConvert.Deserialize<IEnumerable<Base>>(input).ToList();

            // Assert
            result.Should().HaveCount(2);
            ((One) result[0]).Value.Should().Be("COne");
            ((Two) result[1]).Value.Should().Be("CTwo");
        }
        
        [Test]
        public void Deserialize_CollectionOfAbstractTypesByNameWithDefaultValues_CollectionIsReturned()
        {
            // Arrange
            var input = "[ @One @Two ]";

            // Act
            var result = HatchetConvert.Deserialize<IEnumerable<Base>>(input).ToList();

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public void Deserialize_CollectionOfAbstractTypes_CollectionIsReturned()
        {
            // Arrange
            var input = "[ { Class One Value COne } { Class Two Value CTwo } ]";

            // Act
            var result = HatchetConvert.Deserialize<IEnumerable<Base>>(input).ToList();

            // Assert
            result.Should().HaveCount(2);
            ((One) result[0]).Value.Should().Be("COne");
            ((Two) result[1]).Value.Should().Be("CTwo");
        }

        [Test]
        public void Deserialize_ClassWithSingleTaggedConstructor_InstanceIsReturned()
        {
            // Arrange
            var input = "{ arg Hello }";

            // Act
            var result = HatchetConvert.Deserialize<TaggedConstructor>(input);

            // Assert
            result.Arg.Should().Be("Hello");
        }

        [Test]
        public void Deserialize_ClassWithSingleTaggedConstructorWithDifferentlyCasedName_InstanceIsReturned()
        {
            // Arrange
            var input = "{ ARG Hello }";

            // Act
            var result = HatchetConvert.Deserialize<TaggedConstructor>(input);

            // Assert
            result.Arg.Should().Be("Hello");
        }

        [Test]
        public void Deserialize_ClassWithTwoTaggedConstructors_MostSpecificConstructorIsUsed()
        {
            // Arrange
            var input = "{ arg1 Hello arg2 World }";

            // Act
            Action act = () => HatchetConvert.Deserialize<TwoTaggedConstructors>(input);

            // Assert
            act.ShouldThrow<HatchetException>()
                .WithMessage("Only one constructor can be tagged with [HatchetConstructor]");
        }

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        public class TwoTaggedConstructors
        {
            [HatchetConstructor]
            public TwoTaggedConstructors(string arg1)
            {
            }

            [HatchetConstructor]
            public TwoTaggedConstructors(string arg1, string arg2)
            {
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        public class TaggedConstructor
        {
            public string Arg { get; }

            [HatchetConstructor]
            public TaggedConstructor(string arg)
            {
                Arg = arg;
            }
        }

        public abstract class Base
        {

        }

        public class One : Base
        {
            public string Value;
        }

        public class Two : Base
        {
            public string Value;
        }

    }
}