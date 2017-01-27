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