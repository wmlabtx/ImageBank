using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            var scopefolder = _imgList
                .Where(e => e.Value.Node.Equals(img.Node))
                .Select(e => e.Value)
                .ToArray();

            if (scopefolder.Length == 0)
            {
                scopefolder = _imgList
                                .Select(e => e.Value)
                                .ToArray();
            }

            img.NextName = scopefolder[scopefolder.Length / 2].Name;
            img.Sim = 0f;
            img.LastChecked = GetMinLastChecked();
            UpdateLink(img);
        }

        private int FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null || !HelperPath.NodesComparable(imgX.Node, imgY.Node))
            {
                ResetNextName(imgX);
            }
            else
            {
                imgX.LastChecked = DateTime.Now;
                UpdateLink(imgX);
            }
            
            var scope = _imgList
                .Where(e => !e.Value.Name.Equals(imgX.Name))
                .Select(e => e.Value)
                .ToArray();

            if (scope.Length == 0)
            {
                return 0;
            }

            var updates = 0;
            var oldname = imgX.NextName;
            foreach (var e in scope)
            {
                var sim = -1f;
                imgY = e;
                if (HelperPath.NodesComparable(imgX.Node, imgY.Node))
                {
                    sim = HelperDescriptors.GetSim(imgX.Descriptors, imgY.Descriptors);
                    if (sim > imgX.Sim)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Sim = sim;
                    }
                }

                if (HelperPath.NodesComparable(imgY.Node, imgX.Node))
                {
                    if (sim < 0f || imgX.Descriptors.Length != imgY.Descriptors.Length)
                    {
                        sim = HelperDescriptors.GetSim(imgY.Descriptors, imgX.Descriptors);
                    }

                    if (sim > imgY.Sim)
                    {
                        imgY.Gen = 0;
                        imgY.NextName = imgX.Name;
                        imgY.Sim = sim;
                        imgY.LastChecked = DateTime.Now;
                        UpdateLink(imgY);
                        updates++;
                    }
                }
            }

            if (!oldname.Equals(imgX.NextName))
            {
                UpdateLink(imgX);
            }

            return updates;
        }
    }
}
