using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private int FindNextName(Img imgX)
        {
            var scope = _imgList
                    .Where(e => !e.Value.Name.Equals(imgX.Name))
                    .Select(e => e.Value)
                    .ToArray();

            imgX.LastChecked = DateTime.Now;
            SqlUpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);

            if (scope.Length == 0)
            {
                imgX.NextName = imgX.Name;
                imgX.Sim = 0f;
                SqlUpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);

                imgX.LastUpdated = imgX.LastChecked;
                SqlUpdateLastUpdated(imgX.Name, imgX.LastUpdated);

                return 0;
            }

            var nextimg = GetImgByName(imgX.NextName);
            if (nextimg == null || imgX.Name.Equals(nextimg) || !HelperPath.FolderComparable(imgX.Folder, nextimg.Folder))
            {
                imgX.Sim = 0f;
                SqlUpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);
            }

            var updates = 0;
            foreach (var imgY in scope)
            {
                if (HelperPath.FolderComparable(imgX.Folder, imgY.Folder))
                {
                    var sim = GetSim(imgX.Descriptors, imgY.Descriptors);
                    if (sim > imgX.Sim)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Sim = sim;
                        SqlUpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);

                        imgX.LastUpdated = DateTime.Now;
                        SqlUpdateLastUpdated(imgX.Name, imgX.LastUpdated);

                        if (HelperPath.FolderComparable(imgY.Folder, imgX.Folder) && sim > imgY.Sim)
                        {
                            imgY.NextName = imgX.Name;
                            imgY.Sim = sim;
                            SqlUpdateLink(imgY.Name, imgY.NextName, imgY.Sim, imgY.LastChecked);

                            imgY.LastUpdated = DateTime.Now;
                            SqlUpdateLastUpdated(imgY.Name, imgY.LastUpdated);

                            updates++;
                        }
                    }                    
                }
                else
                {
                    if (HelperPath.FolderComparable(imgY.Folder, imgX.Folder))
                    {
                        var sim = GetSim(imgY.Descriptors, imgX.Descriptors);
                        if (sim > imgY.Sim)
                        {
                            imgY.NextName = imgX.Name;
                            imgY.Sim = sim;
                            SqlUpdateLink(imgY.Name, imgY.NextName, imgY.Sim, imgY.LastChecked);

                            imgY.LastUpdated = DateTime.Now;
                            SqlUpdateLastUpdated(imgY.Name, imgY.LastUpdated);

                            updates++;
                        }
                    }
                }
            }

            return updates;
        }
    }
}
