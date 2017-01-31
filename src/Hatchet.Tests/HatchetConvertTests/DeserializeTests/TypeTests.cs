using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class TypeTests
    {
        [SetUp]
        public void Setup()
        {
            HatchetTypeRegistry.Clear();
            HatchetTypeRegistry.Add<One>();
            HatchetTypeRegistry.Add<Two>();
        }
        
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
        public void Deserialize_CollectionOfAbstractTypes_CollectionIsReturned()
        {
            // Arrange
            var input = "[ { Class One Value COne } { Class Two Value CTwo } ]";

            // Act
            var result = HatchetConvert.Deserialize<IEnumerable<Base>>(input);

            // Assert
            result.Should().HaveCount(2);
        }

        abstract class Base
        {
            
        }

        class One : Base
        {
            public string Value;
        }

        class Two : Base
        {
            public string Value;
        }

    }
}