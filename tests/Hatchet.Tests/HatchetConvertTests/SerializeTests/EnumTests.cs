using System;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

[TestFixture]
public class EnumTests
{
    [Flags]
    enum TestEnum
    {
        One = 1,
        Two = 2,
        Four = 4
    }

    [Test]
    public void Serialize_EnumValue_CorrectValueIsWritten()
    {
        // Arrange
        var input = TestEnum.Four;

        // Act
        var result = HatchetConvert.Serialize(input);

        // Assert
        result.Should().Be(TestEnum.Four.ToString());
    }

    [Test]
    public void Serialize_EnumFlags_CorrectValuesAreWritten()
    {
        // Arrange
        var input = TestEnum.One | TestEnum.Two;

        // Act
        var result = HatchetConvert.Serialize(input);

        // Assert
        result.Should().Be("[One, Two]");
    }
}