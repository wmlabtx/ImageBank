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
            imgX.Sim = 0f;
            var scope = _imgList
                .Where(e => !e.Value.Name.Equals(imgX.Name))
                .Select(e => e.Value)
                .ToArray();

            if (scope.Length == 0)
            {
                imgX.LastChecked = DateTime.Now;
                return 0;
            }

            var updates = 0;
            var oldname = imgX.NextName;
            foreach (var imgY in scope)
            {
                var sim = HelperDescriptors.GetSim(imgX.Descriptors, imgY.Descriptors);
                if (sim > imgX.Sim)
                {
                    imgX.NextName = imgY.Name;
                    imgX.Sim = sim;
                }

                if (sim > imgY.Sim)
                {
                    imgY.NextName = imgX.Name;
                    imgY.Sim = sim;
                    imgY.LastChanged = DateTime.Now;
                    UpdateNameNext(imgY);
                    updates++;
                }
            }

            imgX.LastChecked = DateTime.Now;
            if (!imgX.NextName.Equals(oldname) || Math.Abs(oldsim - imgX.Sim) > 0.0001)
            {
                imgX.LastChanged = DateTime.Now;
                UpdateNameNext(imgX);
            }

            return updates;
        }
    }
}