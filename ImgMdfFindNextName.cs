using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            var scopefolder = _imgList
                .Where(e => e.Value.Node.Equals(img.Node) && !e.Value.Name.Equals(img.Name))
                .Select(e => e.Value)
                .ToArray();

            if (scopefolder.Length == 0)
            {
                scopefolder = _imgList
                                .Select(e => e.Value)
                                .ToArray();
            }

            img.SetNextName(scopefolder[scopefolder.Length / 2].Name, GetMinLastChecked());
            UpdateNameNext(img);
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
                imgX.SetNextName();
                UpdateNameNext(imgX);
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
                        imgX.SetNextName(imgY.Name, sim);
                    }

                    if (HelperPath.NodesComparable(imgY.Node, imgX.Node) && imgX.Descriptors.Length != imgY.Descriptors.Length && sim > imgY.Sim)
                    {
                        imgY.SetNextName(imgX.Name, sim);
                        UpdateNameNext(imgY);
                        updates++;
                    }
                }

                /*
                if (HelperPath.NodesComparable(imgY.Node, imgX.Node))
                {
                    if (sim < 0f || imgX.Descriptors.Length != imgY.Descriptors.Length)
                    {
                        sim = HelperDescriptors.GetSim(imgY.Descriptors, imgX.Descriptors);
                    }

                    if (sim > imgY.Sim)
                    {
                        imgY.SetNextName(imgX.Name, sim);
                        UpdateNameNext(imgY);
                        updates++;
                    }
                }
                */
            }

            if (!oldname.Equals(imgX.NextName))
            {
                UpdateNameNext(imgX);
            }

            return updates;
        }
    }
}
