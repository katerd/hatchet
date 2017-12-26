using System;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests
{
    [TestFixture]
    public class CircularReferenceErrorTests
    {
        public class ClassA
        {
            public ClassB PropertyB { get; set; }
            public int Value { get; set; }
        }

        public class ClassB
        {
            public ClassA PropertyA { get; set; }
            public int Value { get; set; }
        }
        
        [Test]
        public void Serialize_ObjectGraphWithCircularReference_ExceptionIsThrown()
        {
            // Arrange
            var input = new ClassA();
            input.PropertyB = new ClassB
            {
                PropertyA = input
            };
            
            // Act
            Action t = () => HatchetConvert.Serialize(input);
            
            // Assert
            t.ShouldThrow<CircularReferenceException>();
        }

        [Test]
        public void Serialize_ObjectGraphWithNoCircularReference_NoExceptionIsThrown()
        {
            // Arrange
            var input = new ClassA
            {
                Value = 10,
                PropertyB = new ClassB
                {
                    Value = 10
                }
            };
            
            // Act & Assert
            HatchetConvert.Serialize(input);
        }
    }
}