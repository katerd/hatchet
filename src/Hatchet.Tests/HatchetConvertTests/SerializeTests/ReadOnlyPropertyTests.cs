using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests
{
    [TestFixture]
    public class ReadOnlyPropertyTests
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedMember.Global
        // ReSharper disable ConvertPropertyToExpressionBody
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public class DemoClass
        {
            public string GetOnlyPropertyWithoutBody { get; }

            public string LambdaProperty => "";

            public string GetOnlyPropertyWithBody
            {
                get { return ""; }
            }

            public DemoClass()
            {
                GetOnlyPropertyWithoutBody = "";
            }
        }

        [Test]
        public void Serialize_ClassWithReadOnlyProperties_PropertiesAreNotSerialized()
        {
            // Arrange
            var input = new DemoClass();

            // Act
            var result = HatchetConvert.Serialize(input);

            // Assert
            result.Should().Be("{\n}");
        }
    }
}