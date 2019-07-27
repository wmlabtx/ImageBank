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
            HelperSql.UpdateLink(img);
        }

        private void FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null || !HelperPath.FolderComparable(imgX.Folder, imgY.Folder))
            {
                ResetNextName(imgX);
            }
            else
            {
                imgX.LastChecked = DateTime.Now;
                HelperSql.UpdateLink(imgX);
            }
            
            var scope = _imgList
                .Where(e => 
                    !e.Value.Name.Equals(imgX.Name) && 
                    HelperPath.FolderComparable(imgX.Folder, e.Value.Folder))
                .Select(e => e.Value)
                .ToArray();

            if (scope.Length == 0)
            {
                return;
            }

            var oldname = imgX.NextName;
            foreach (var e in scope)
            {
                var sim = HelperDescriptors.GetSim(imgX.Descriptors, e.Descriptors);
                if (sim > imgX.Sim)
                {
                    imgX.NextName = e.Name;
                    imgX.Sim = sim;
                }                    
            }

            if (!oldname.Equals(imgX.NextName))
            {
                HelperSql.UpdateLink(imgX);
            }
        }
    }
}
