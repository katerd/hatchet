using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.DeserializeTests
{
    [TestFixture]
    public class EnumTests
    {
        public enum TestEnum
        {
            Alpha = 0,
            Bravo = 1,
            Charlie = 2,
            Delta = 3,
            Echo = 4
        }

        public class EnumTestClass
        {
            public TestEnum Property;
            public TestEnum Field;
        }

        [Test]
        public void Deserialize_SingleEnum_ShouldReturnAnEnum()
        {
            // Arrange
            var input = "Alpha";

            // Act
            var result = HatchetConvert.Deserialize<TestEnum>(ref input);

            // Assert
            result.Should().Be(TestEnum.Alpha);
        }

        [Test]
        public void Deserialize_ListOfEnums_ShouldReturnListOfEnums()
        {
            // Arrange
            var input = "[Alpha Bravo charlie DELTA]";

            // Act
            var result = HatchetConvert.Deserialize<List<TestEnum>>(ref input);

            // Assert
            result.Should().ContainInOrder(TestEnum.Alpha, TestEnum.Bravo, TestEnum.Charlie, TestEnum.Delta);
        }

        [Test]
        public void Deserialize_ObjectOfEnums_ShouldSetEnumProperties()
        {
            // Arrange
            var input = "{ Property Alpha Field Delta }";

            // Act
            var result = HatchetConvert.Deserialize<EnumTestClass>(ref input);

            // Assert
            result.Should().NotBeNull();
            result.Property.Should().Be(TestEnum.Alpha);
            result.Field.Should().Be(TestEnum.Delta);
        }
    }
}