using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;

            imgX.NextName = imgX.Name;
            imgX.Sim = 0f;
            imgX.LastChecked = DateTime.Now;
            SqlUpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);

            var scope = _imgList
                    .Where(e => !e.Value.Name.Equals(imgX.Name))
                    .Select(e => e.Value)
                    .ToArray();

            if (scope.Length == 0)
            {
                return;
            }

            var foldersize = GetFolderSize(imgX.Folder);
            //var updates = 0;
            foreach (var imgY in scope)
            {
                if (foldersize < 2 || HelperPath.FolderComparable(imgX.Folder, imgY.Folder))
                {
                    var sim = GetSim(imgX.Descriptors, imgY.Descriptors);
                    if (sim > imgX.Sim)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Sim = sim;
                        SqlUpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);

                        /*
                        if (HelperPath.FolderComparable(imgY.Folder, imgX.Folder) && imgY.Descriptors.Rows == imgX.Descriptors.Rows && sim > imgY.Sim)
                        {
                            imgY.NextName = imgX.Name;
                            imgY.Sim = sim;
                            SqlUpdateLink(imgY.Name, imgY.NextName, imgY.Sim, imgY.LastChecked);

                            updates++;
                        }
                        */
                    }                    
                }
                /*
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
                */
            }
        }
    }
}
