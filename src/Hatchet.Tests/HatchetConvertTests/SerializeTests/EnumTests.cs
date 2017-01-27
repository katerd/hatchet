using FluentAssertions;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests
{
    [TestFixture]
    public class EnumTests
    {
        enum TestEnum
        {
            One,
            Two,
            Four
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
    }
}