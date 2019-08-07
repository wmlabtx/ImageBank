using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            if (img.Cluster == 0)
            {
                var scopefolder = _imgList
                    .Where(e => !e.Value.Name.Equals(img.Name))
                    .Select(e => e.Value)
                    .ToArray();

                img.SetNextName(scopefolder[scopefolder.Length / 2].Name, GetMinLastChecked());
            }
            else
            {
                var scopefolder = _imgList
                    .Where(e => !e.Value.Name.Equals(img.Name) && e.Value.Cluster != img.Cluster)
                    .Select(e => e.Value)
                    .ToArray();

                img.SetNextName(scopefolder[scopefolder.Length / 2].Name, GetMinLastChecked());
            }

            UpdateNameNext(img);
        }

        private int FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            ResetNextName(imgX);
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
            foreach (var imgY in scope)
            {
                if (imgX.Cluster == 0 || imgX.Cluster != imgY.Cluster)
                {
                    var sim = HelperDescriptors.GetSim(imgX.Descriptors, imgY.Descriptors);
                    if (sim > imgX.Sim)
                    {
                        imgX.SetNextName(imgY.Name, sim);
                    }

                    if (sim > imgY.Sim)
                    {
                        imgY.SetNextName(imgX.Name, sim);
                        UpdateNameNext(imgY);
                        updates++;
                    }
                }
            }

            if (!oldname.Equals(imgX.NextName))
            {
                UpdateNameNext(imgX);
            }

            return updates;
        }
    }
}
