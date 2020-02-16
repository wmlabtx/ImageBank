using ImageBank;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class BrisqueTest
    {
        [TestMethod]
        public void CompareQuality()
        {
            var filenames = new string[]
            {
                @"img\org.jpg",
                @"img\org_bwresized.jpg",
                @"img\org_compressed.jpg",
                @"img\org_crop.jpg",
                @"img\org_nologo.jpg",
                @"img\org_nosim1.jpg",
                @"img\org_nosim2.jpg",
                @"img\org_r10.jpg",
                @"img\org_r90.jpg",
                @"img\org_resaved.jpg",
                @"img\org_resized.jpg",
                @"img\org_sim1.jpg",
                @"img\org_sim2.jpg"
            };

            var sb = new StringBuilder();
            foreach (var filename in filenames) {
                var quality = Helper.ComputeQuality(filename);
                sb.AppendLine($"{filename}: {quality:F2}");
            }

            File.WriteAllText(@"brisque_report.txt", sb.ToString());
        }
    }
}
