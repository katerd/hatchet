using System;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests
{
    public class StructTests
    {
        public struct DemoEmpty
        {
            
        }

        public struct DemoOne
        {
            public int A;
            public int B;
            public string Hello;
        }

        [Test]
        public void Serialize_DemoEmptyStruct_EmptyStructIsWritten()
        {
            // Arrange
            var input = new DemoEmpty();

            // Act
            var result = HatchetConvert.Serialize(input);

            // Assert
            result.Should().Be(@"{}");
        }

        [Test]
        public void SerializeAndDeserialize_DemoOneStruct_PropertiesAreWritten()
        {
            // Arrange
            var input = new DemoOne
            {
                A = 10,
                B = 20,
                Hello = "World"
            };

            // Act
            var inter = HatchetConvert.Serialize(input);

            Console.WriteLine(inter);

            var result = HatchetConvert.Deserialize<DemoOne>(inter);

            // Assert
            result.A.Should().Be(10);
            result.B.Should().Be(20);
            result.Hello.Should().Be("World");
        }
    }
}