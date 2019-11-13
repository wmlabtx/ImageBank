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

            var scope = _imgList
                .Where(e => !e.Value.Name.Equals(imgX.Name) && e.Value.Descriptors.Size.Height > 0)
                .Select(e => e.Value)
                .ToArray();

            if (scope.Length == 0)
            {
                UpdateNameNext(imgX);
                return 0;
            }

            imgX.LastChecked = DateTime.Now;
            imgX.NextName = imgX.Name;
            imgX.Sim = 0f;

            var updates = 0;

            foreach (var imgY in scope)
            {
                if (string.IsNullOrEmpty(imgX.Node) || imgY.Node.StartsWith(imgX.Node))
                {
                    var sim = HelperDescriptors.GetSim(imgX.Descriptors, imgY.Descriptors);
                    if (sim > imgX.Sim)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Sim = sim;
                        imgX.LastChecked = DateTime.Now;
                    }

                    /*
                    var siminv = HelperDescriptors.GetSim(imgY.Descriptors, imgX.Descriptors);
                    if (siminv > imgY.Sim)
                    {
                        imgY.NextName = imgX.Name;
                        imgY.Sim = siminv;
                        imgY.LastChanged = DateTime.Now;
                        UpdateNameNext(imgY);
                        updates++;
                    }
                    */
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