using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            img.NextName = img.Name;
            img.Sim = 0f;
            img.LastChecked = GetMinLastChecked();
            UpdateNameNext(img);
        }

        private int FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            var oldsim = imgX.Sim;

            imgX.NextName = imgX.Name;
            imgX.Sim = 0f;
            imgX.LastChecked = DateTime.Now;            

            if (imgX.Descriptors.Length == 0)
            {
                var jpgdata = GetJpgData(imgX);
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return 0;
                }

                var crcname = HelperCrc.GetCrc(jpgdata);
                Assert.IsTrue(crcname.Equals(imgX.Name));
                if (HelperDescriptors.ComputeDescriptors(jpgdata, out var descriptors))
                {
                    imgX.Descriptors = descriptors;
                    UpdateDescriptors(imgX);
                }
            }

            var scope = _imgList
                .Where(e => !e.Value.Name.Equals(imgX.Name) && e.Value.Descriptors.Length > 0)
                .Select(e => e.Value)                
                .ToArray();

            if (scope.Length == 0)
            {
                UpdateNameNext(imgX);
                return 0;
            }

            var updates = 0;            
            foreach (var imgY in scope)
            {
                if (string.IsNullOrEmpty(imgX.Node) || (!string.IsNullOrEmpty(imgX.Node) && !string.IsNullOrEmpty(imgY.Node) && imgY.Node.StartsWith(imgX.Node)))
                {
                    var sim = HelperDescriptors.GetSim(imgX.Descriptors, imgY.Descriptors);
                    if (sim > imgX.Sim)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Sim = sim;
                        imgX.LastChecked = DateTime.Now;                        
                    }
                }
            }

            if (!imgX.NextName.Equals(oldnextname) || Math.Abs(oldsim - imgX.Sim) > 0.0001)
            {                
                imgX.LastChanged = DateTime.Now;
            }

            UpdateNameNext(imgX);
            return updates;
        }
    }
}