using System.IO;

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
                FindNextName(img);
            }
        }
    }
}
