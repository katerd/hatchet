using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests;

[TestFixture]
public class ObjectTests
{
    public class TestPropertyClass
    {
        public string StringProperty { get; set; }
    }

    [Test]
    public void Serialize_ATestPropertyClass_ReturnsValueAsAString()
    {
        // Arrange
        var testClass = new TestPropertyClass
        {
            StringProperty = "Wow"
        };

        // Act
        var result = HatchetConvert.Serialize(testClass);

        // Assert
        result.Should().Be(
            "{\n"+
            "  StringProperty Wow\n" +
            "}");
    }

    public class TestFieldClass
    {
        public string StringField;
    }

    [Test]
    public void Serialize_ATestFieldClass_ReturnsValueAsAString()
    {
        // Arrange
        var testClass = new TestFieldClass
        {
            StringField = "Wow"
        };

        // Act
        var result = HatchetConvert.Serialize(testClass);

        // Assert
        result.Should().Be(
            "{\n" +
            "  StringField Wow\n" +
            "}");
    }

    public class TestNestedClass
    {
        public string TestValue;
        public TestNestedClass NestedClass;
    }

    [Test]
    public void Serialize_ATestNestedClass_ReturnsValueAsAString()
    {
        // Arrange
        var root = new TestNestedClass
        {
            TestValue = "Root",
            NestedClass = new TestNestedClass
            {
                TestValue = "FirstChild",
                NestedClass = new TestNestedClass
                {
                    TestValue = "SecondChild"
                }
            }
        };

        // Act
        var result = HatchetConvert.Serialize(root);

        // Assert
        result.Should().Be(
            "{\n" +
            "  TestValue Root\n"+
            "  NestedClass {\n" +
            "    TestValue FirstChild\n" +
            "    NestedClass {\n" +
            "      TestValue SecondChild\n" +
            "    }\n" +
            "  }\n" +
            "}");
    }

    public class TestClassNullString
    {
        public string NullProperty { get; set; }
        public string NullField;
    }

    [Test]
    public void Serialize_TestClassNullString_ReturnsValueAsAString()
    {
        // Arrange
        var input = new TestClassNullString
        {
            NullField = null,
            NullProperty = null
        };

        // Act
        var result = HatchetConvert.Serialize(input);

        // Assert
        result.Should().Be("{\n}");
    }
}