using System;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void MoveTo(string name, string destfolder)
        {
            /*
            var img = GetImgByName(name);
            if (img == null)
            {
                return;
            }

            img.Folder = destfolder;
            HelperSql.UpdateFolder(name, destfolder);
            var imgNext = GetImgByName(img.NextName);
            if (imgNext == null || !destfolder.Equals(imgNext.Folder))
            {
                ResetNextName(img);
                var scopefolder = _imgList
                    .Where(e => e.Value.Folder.Equals(destfolder))
                    .Select(e => e.Value)
                    .ToArray();

                if (scopefolder.Length == 0)
                {
                    img.NextName = img.Name;
                }
                else
                {
                    img.NextName = scopefolder[scopefolder.Length / 2].Name;
                }

                HelperSql.UpdateLink(name, img.NextName, img.Sim, img.LastChecked);
            }
            */
        }
    }
}
