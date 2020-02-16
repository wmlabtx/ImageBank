using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBank
{
    /*
    [TestClass]
    public class HelperDescriptorsTest
    {
        [TestMethod]
        public void CompareImages()
        {
            if (HelperImages.GetBitmapFromFile(@"img\org_nojpg.jpg", out _, out _, out _, out _, out _)) {
                Assert.Fail();
            }

            if (!HelperImages.GetBitmapFromFile(@"img\org.jpg", out byte[] org_data, out _, out _, out _, out _)) {
                Assert.Fail();
            }

            if (!HelperDescriptors.Compute(org_data, out var org_hashes)) {
                Assert.Fail();
            }

            var sb = new StringBuilder();
            CalculateHashesAndCompare(@"img\org_resaved.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_r90.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_r10.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_resized.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_bwresized.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_crop.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_nologo.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_sim1.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_sim2.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_nosim1.jpg", org_hashes, sb);
            CalculateHashesAndCompare(@"img\org_nosim2.jpg", org_hashes, sb);
            File.WriteAllText(@"img\report.txt", sb.ToString());
        }

        private void CalculateHashesAndCompare(string name, uint[] org_hashes, StringBuilder sb)
        {
            if (!HelperImages.GetBitmapFromFile(name, out byte[] data, out _, out _, out _, out _)) {
                Assert.Fail();
            }

            if (!HelperDescriptors.Compute(data, out var hashes)) {
                Assert.Fail();
            }

            var sim = HelperDescriptors.Intersection(hashes, org_hashes);
            sb.AppendLine($"{name}: {sim:F2}");
        }
    }
    */
}
