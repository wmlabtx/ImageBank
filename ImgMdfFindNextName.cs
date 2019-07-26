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
            HelperSql.UpdateLink(img.Name, img.NextName, img.Sim, img.LastChecked);
        }

        private void FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;

            if (imgX.Name.Equals(imgX.NextName) || !_imgList.ContainsKey(imgX.NextName))
            {
                ResetNextName(imgX);
            }

            var foldersize = GetFolderSize(imgX.Folder);
            var scope = _imgList
                    .Where(e => 
                        !e.Value.Name.Equals(imgX.Name) &&
                        (foldersize < 2 || HelperPath.FolderComparable(imgX.Folder, e.Value.Folder)))
                    .Select(e => e.Value)
                    .ToArray();

            if (scope.Length == 0)
            {
                return;
            }

            foreach (var imgY in scope)
            {
                var dt = DateTime.Now;
                var sim = HelperDescriptors.GetSim(imgX.Descriptors, imgY.Descriptors);
                var el = DateTime.Now.Subtract(dt).TotalMilliseconds;
                if (sim > imgX.Sim)
                {
                    imgX.NextName = imgY.Name;
                    imgX.Sim = sim;
                    HelperSql.UpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);
                }                    
            }
        }
    }
}
