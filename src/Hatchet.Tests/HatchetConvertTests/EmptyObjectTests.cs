﻿using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests
{
    [TestFixture]
    public class EmptyObjectTests
    {
        private class EmptyObject { }

        [Test]
        public void Deserialize_WithEmptyObject1_EmptyObjectIsReturned()
        {
            // Arrange
            var source = "{  } ";

            // Act
            var result = HatchetConvert.Deserialize<EmptyObject>(ref source);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void Deserialize_WithEmptyObject2_EmptyObjectIsReturned()
        {
            // Arrange
            var source = "{}";

            // Act
            var result = HatchetConvert.Deserialize<EmptyObject>(ref source);

            // Assert
            result.Should().NotBeNull();
        } 
    }
}