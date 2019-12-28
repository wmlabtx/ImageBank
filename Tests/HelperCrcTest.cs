using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageBank.Tests
{
    [TestClass]
    public class HelperCrcTest
    {
        [TestMethod]
        public void GetCrcTest()
        {
            var array = new byte[] { 0x00, 0x01, 0x02, 0x03 };
            var crc0123 = HelperHash.CalculateHash(array);
            Assert.AreEqual(crc0123, "xxxx-xxxx-xxxx");
        }
    }
}
