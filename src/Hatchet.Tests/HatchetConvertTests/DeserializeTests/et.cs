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

        [Test]
        public void DeserializeClass_WithoutConstructor_AnExceptionIsThrown()
        {
            // Arrange
            var input = "{}";

            // Act
            Action action = () => HatchetConvert.Deserialize<NoValidCtor>(input);

            // Assert
            action.ShouldThrow<HatchetException>().And.Message.Should()
                .Contain("NoValidCtor").And.Contain("constructor");
        }
    }
}