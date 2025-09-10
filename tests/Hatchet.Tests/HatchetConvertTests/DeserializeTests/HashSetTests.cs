using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests;

[TestFixture]
public class HashSetTests
{
    [Test]
    public void Deserialize_ListOfValuesIntoHashSet_SetIsPopulated()
    {
        // Arrange
        var input = "[10 20 30]";

        // Act
        var result = HatchetConvert.Deserialize<HashSet<int>>(input);

        // Assert
        result.Should().Equal(10, 20, 30);
    }

    [Test]
    public void Deserialize_EmptyHashSet_SetIsEmpty()
    {
        // Arrange
        var input = "[]";

        // Act
        var result = HatchetConvert.Deserialize<HashSet<int>>(input);

        // Assert
        result.Should().BeEmpty();
    }
}