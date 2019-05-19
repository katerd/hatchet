using System;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class ErrorTests
    {
        class NoValidCtor
        {
            public NoValidCtor(string foo)
            {
                
            }
        }

        abstract class Base
        {
            
        }

        class Demo : Base
        {
            
        }

        [Test]
        public void DeserializeClass_WithoutConstructor_AnExceptionIsThrown()
        {
            // Arrange
            var input = "{}";

            // Act
            Action action = () => HatchetConvert.Deserialize<NoValidCtor>(input);

            // Assert
            action.Should().Throw<HatchetException>().And.Message.Should()
                .Contain("NoValidCtor").And.Contain("constructor");
        }

        [Test]
        public void DeserializeClass_WithNamedClassThatIsNotInRegistry_AnExceptionIsThrown()
        {
            // Arrange
            var input = "{ Class Demo }";

            // Act
            Action action = () => HatchetConvert.Deserialize<Demo>(input);

            // Assert
            action.Should().Throw<HatchetException>().And.Message.Should()
                .Contain("Type is not registered").And.Contain("Demo");
        }
    }
}