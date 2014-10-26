using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests
{
    [TestFixture]
    public class ObjectTests
    {
        public class TestPropertyClass
        {
            public string StringProperty { get; set; }
            public string StringField;
            public int IntProperty { get; set; }
            public int IntField;
            public int[] IntList;
            public string[] StringList;
            public List<int> IntArray;
            public List<string> StringArray;
        }

        [Test]
        public void Serialize_ATestPropertyClass_ReturnsValueAsAString()
        {
            // Arrange
            var testClass = new TestPropertyClass
            {
                IntArray = new List<int> {1, 2, 3, 4},
                IntField = 1000,
                IntList = new[] {2, 4, 6, 8},
                IntProperty = 2000,
                StringArray = new List<string> {"Hello", "World"},
                StringField = "This is a test",
                StringList = new[] {"Such", "String"},
                StringProperty = "Wow"
            };

            // Act
            var result = HatchetConvert.Serialize(testClass);

            // Assert
            result.Should().Be(
                "{\n"+
                "  StringProperty Wow\n" +
                "  StringField \"This is a test\"\n" +
                "  IntProperty 2000\n" +
                "  IntField 1000\n" +
                "  IntList [2 4 6 8]\n" +
                "  StringList [Such String]\n" +
                "  IntArray [1 2 3 4]\n" +
                "  StringArray [Hello World]\n" +
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
                "      TestValue SecondChild {\n" +
                "    }\n" +
                "  }\n" +
                "}");
        }
    }
}