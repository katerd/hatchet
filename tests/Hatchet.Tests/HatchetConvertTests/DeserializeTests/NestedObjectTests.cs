using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests;

[TestFixture]
public class NestedObjectTests
{
    class Inner
    {
        public string StrValue { get; set; }
        public int IntValue { get; set; }
    }

    class Outer
    {
        public Inner Inner { get; set; }
    }

    [Test]
    public void Deserialize_NestedObject_PropertiesArePopulated()
    {
        // Arrange
        var input = "{ inner { strValue Hello intValue 1234} } }";

        // Act
        var result = HatchetConvert.Deserialize<Outer>(input);

        // Assert
        result.Should().NotBeNull();
        result.Inner.Should().NotBeNull();
        result.Inner.IntValue.Should().Be(1234);
        result.Inner.StrValue.Should().Be("Hello");
    } 
}