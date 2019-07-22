using System;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void MoveTo(string name, string destfolder)
        {
            var filenamenew = HelperPath.GetFileName(name, destfolder);
            if (File.Exists(filenamenew))
            {
                return;
            }

            var img = GetImgByName(name);
            if (img == null)
            {
                return;
            }

            File.Move(img.FileName, filenamenew);
            img.Folder = destfolder;
            SqlUpdateFolder(name, destfolder);
            var imgNext = GetImgByName(img.NextName);
            if (imgNext == null || !destfolder.Equals(imgNext.Folder))
            {
                img.Sim = 0f;
                img.LastChecked = DateTime.Now.AddYears(-10);
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

                SqlUpdateLink(name, img.NextName, img.Sim, img.LastChecked);
            }
        }
    }
}
