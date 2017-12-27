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

        public class ClassC
        {
            public ClassA PropertyA1 { get; set; }
            public ClassA PropertyA2 { get; set; }
        }

        public class ClassD
        {
            public ClassA[] PropertyA { get; set; }
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

        [Test]
        public void Serialize_ObjectSerializedTwice_NoExceptionIsThrown()
        {
            // Arrange
            var prop = new ClassA();
            var input = new ClassC
            {
                PropertyA1 = prop,
                PropertyA2 = prop
            };

            // Act & Assert
            HatchetConvert.Serialize(input);
        }

        [Test]
        public void Serialize_ObjectSerializedTwiceInList_NoExceptionIsThrown()
        {
            // Arrrange
            var prop = new ClassA();
            
            var input = new ClassD
            {
                PropertyA = new[]
                {
                    prop, prop
                }
            };

            // Act & Assert
            HatchetConvert.Serialize(input);
        }
    }
}