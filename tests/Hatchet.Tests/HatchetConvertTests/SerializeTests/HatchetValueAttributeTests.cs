using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

[TestFixture]
public class HatchetValueAttributeTests
{
    public class DemoClass
    {
        [HatchetValue]
        public string HatchetProperty => "This Is A Test";
    }
        
    [Test]
    public void Serialize_WithCustomSerializer_ValueIsWritten()
    {
        // Arrange
        var input = new DemoClass();
            
        // Act
        var result = HatchetConvert.Serialize(input);

        // Assert
        result.Should().Be("This Is A Test");
    }
}