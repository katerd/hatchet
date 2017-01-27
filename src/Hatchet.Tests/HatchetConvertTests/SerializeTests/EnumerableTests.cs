using System.Collections.Generic;
using NUnit.Framework;

namespace Hatchet.Tests.HatchetConvertTests.SerializeTests
{
    [TestFixture]
    public class EnumerableTests
    {
        class Test
        {
            public IEnumerable<string> Items;
        }

        [Test]
        public void Serialize_ObjectWithIEnumerable_ItemsAreWritten()
        {
            // Arrange
            var obj = new Test {Items = new List<string> {"Hi", "There"}};

            // Act
            var result = HatchetConvert.Serialize(obj);

            // Assert
        }
    }
}