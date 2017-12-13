using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests
{
    [TestFixture]
    public class CircularReferenceErrorTests
    {
        public class ClassA
        {
            public ClassB PropertyB { get; set; }
        }

        public class ClassB
        {
            public ClassA PropertyA { get; set; }
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
    }
}