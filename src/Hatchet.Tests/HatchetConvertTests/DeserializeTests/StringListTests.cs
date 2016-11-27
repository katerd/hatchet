using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class StringListTests
    {
        [Test]
        public void Deserialize_AStringList_ShouldReturnAListOfStrings()
        {
            // Arrange
            var input = "[this is a string list]";

            // Act
            var result = HatchetConvert.Deserialize<List<string>>(input);

            // Assert
            result.Should().HaveCount(5);
            result[0].Should().Be("this");
            result[1].Should().Be("is");
            result[2].Should().Be("a");
            result[3].Should().Be("string");
            result[4].Should().Be("list");
        }

        [Test]
        public void Deserialize_AStringIntoAStringList_ShouldThrowAnException()
        {
            // Arrange
            var input = "thisIsNotAList";

            Exception caughtException = null;

            // Act Assert
            try
            {
                HatchetConvert.Deserialize<List<string>>(input);
            }
            catch (HatchetException exception)
            {
                caughtException = exception;
            }

            caughtException.Should().NotBeNull();

        } 
    }
}