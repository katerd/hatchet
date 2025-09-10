using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

[TestFixture]
public class DictionaryTests
{
    [Test]
    public void Serialize_EmptyDictionary_ReturnsValueAsAString()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>();

        // Act
        var result = HatchetConvert.Serialize(dictionary);

        // Assert
        result.Should().Be("{}");
    }

    [Test]
    public void Serialize_DictionaryWithSpacesInKey_ThrowsException()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>();
        dictionary["Example Key"] = "1000";

        // Act
        HatchetException caughtException = null;
        try
        {
            HatchetConvert.Serialize(dictionary);
        }
        catch (HatchetException hatchetException)
        {
            caughtException = hatchetException;
        }

        // Assert
        caughtException.Should().NotBeNull();
        caughtException.Message.Should()
            .Contain("`Example Key` is an invalid key. Key cannot contain spaces.");
    }

    [Test]
    public void Serialize_ADictionaryOfStrings_ReturnsDictionaryAsAString()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>();
        dictionary["a"] = "1000";
        dictionary["b"] = "2000";

        // Act
        var result = HatchetConvert.Serialize(dictionary);

        // Assert
        result.Should().Be(
            "{\n" +
            "  a 1000\n" +
            "  b 2000\n" +
            "}");
    }

    [Test]
    public void Serialize_ADictionaryOfLists_ReturnsDictionaryAsAString()
    {
        // Arrange
        var dictionary = new Dictionary<string, int[]>();
        dictionary["a"] = new[] {100, 200};
        dictionary["b"] = new int[0];

        // Act
        var result = HatchetConvert.Serialize(dictionary);

        // Assert
        result.Should().Be(
            "{\n" +
            "  a [100 200]\n" +
            "  b []\n" + 
            "}");
    }

    [Test]
    public void Serialize_NestedDictionaries_ReturnsNestedDictionarysAsAString()
    {
        // Arrange
        var outer = new Dictionary<string, object>();
        var inner = new Dictionary<string, object>();

        outer["value"] = inner;
        inner["value"] = "InnerValue";

        // Act
        var result = HatchetConvert.Serialize(outer);

        // Assert
        result.Should().Be(
            "{\n" +
            "  value {\n" +
            "    value InnerValue\n" +
            "  }\n" +
            "}");
    }
}