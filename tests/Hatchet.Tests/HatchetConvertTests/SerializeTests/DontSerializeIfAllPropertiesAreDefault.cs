﻿using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests
{
    [TestFixture]
    public class DontSerializeIfAllPropertiesAreDefault
    {
        public struct Value
        {
            public int A;
            public int B;
            public int C;
        }
        
        public class Test
        {
            public Value Value { get; set; }
        }
        
        [Test]
        public void Serialize_ObjectWherePropertiesOnValueAreDefault_ValueIsNotSerialized()
        {
            // Arrange
            var input = new Test();

            // Act
            var response = HatchetConvert.Serialize(input);

            // Assert
            response.Should().Be("{\n}");
        }

        [Test]
        public void Serialize_StructWherePropertiesOnValueAreDefault_ValueIsNotSerialized()
        {
            // Arrange
            var input = new Value();

            // Act
            var response = HatchetConvert.Serialize(input, new SerializeOptions { IncludeDefaultValues = false });

            // Assert
            response.Should().Be("{\n}");
        }
        
        [Test]
        public void Serialize_ObjectWhenPropertiesOnValueAreDefaultButConfigSaysOtherwise_ValuesAreSerialized()
        {
            // Arrange
            var input = new Value();

            // Act
            var response = HatchetConvert.Serialize(input, new SerializeOptions { IncludeDefaultValues = true });

            // Assert
            response.Should().Be("{\n  A 0\n  B 0\n  C 0\n}");
        }
    }
}