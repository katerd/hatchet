using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class InterfaceTests
    {
        public interface ITestInterface
        {
            string Value { get; set; }
        }

        public class TestInterfaceImpl : ITestInterface
        {
            public string Value { get; set; }
        }
        
        [Test]
        public void Deserialize_ObjectThatHasPropertyWithInterfaceType_Succeeds()
        {
            // Given
            var input = "{ Class TestInterfaceImpl Value 'Hello World'}";
            
            HatchetTypeRegistry.Add<TestInterfaceImpl>();
            
            // When
            var result = HatchetConvert.Deserialize<ITestInterface>(input);
            
            // Then
            result.Value.Should().Be("Hello World");
        }
    }
}