using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

[TestFixture]
public class HashSetTests
{
    [Test]
    public void Serialize_HashSetOfValues_SetIsOutput()
    {
        // Arrange
        var obj = new HashSet<int> { 10, 20, 30 };

        // Act
        var result = HatchetConvert.Serialize(obj);

        // Assert
        result.Should().Be("[10 20 30]");
    }

    [Test]
    public void Serialize_EmptyHashSet_EmptySetIsOutput()
    {
        // Arrange
        var obj = new HashSet<int>();

        // Act
        var result = HatchetConvert.Serialize(obj);

        // Assert
        result.Should().Be("[]");
    }
}