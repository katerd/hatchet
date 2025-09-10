using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests;

[TestFixture]
public class FloatListTests
{
    [Test]
    public void Deserialize_AStringList_ShouldReturnAListOfFloats()
    {
        // Arrange
        var input = "[0.1 -0.2 0.3 0.4]";

        // Act
        var result = HatchetConvert.Deserialize<List<float>>(input);

        // Assert
        result.Should().HaveCount(4);
        result[0].Should().BeApproximately(0.1f, 0.001f);
        result[1].Should().BeApproximately(-0.2f, 0.001f);
        result[2].Should().BeApproximately(0.3f, 0.001f);
        result[3].Should().BeApproximately(0.4f, 0.001f);
    }
}

[TestFixture]
public class DoubleListTests
{
    [Test]
    public void Deserialize_AStringList_ShouldReturnAListOfFloats()
    {
        // Arrange
        var input = "[0.1 -0.2 0.3 0.4]";

        // Act
        var result = HatchetConvert.Deserialize<List<double>>(input);

        // Assert
        result.Should().HaveCount(4);
        result[0].Should().BeApproximately(0.1f, 0.001f);
        result[1].Should().BeApproximately(-0.2f, 0.001f);
        result[2].Should().BeApproximately(0.3f, 0.001f);
        result[3].Should().BeApproximately(0.4f, 0.001f);
    }
}