using ImageBank;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class HelperTest
    {
        [TestMethod]
        public void TestComputeHash3250()
        {
            var array = new byte[] { 0x00, 0x01, 0x02, 0x03 };
            var hash1 = Helper.ComputeHash3250(array);
            Assert.IsTrue(hash1.Length == 50);
            array[0] = 0x04;
            var hash2 = Helper.ComputeHash3250(array);
            Assert.IsTrue(hash2.Length == 50);
            Assert.IsFalse(hash1.SequenceEqual(hash2));
        }
    }
}