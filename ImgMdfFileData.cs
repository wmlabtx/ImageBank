using System.IO;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void WriteFileData(string name, string folder, byte[] jpgdata)
        {
            var path = $"{AppConsts.PathCollection}{folder}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filename = HelperPath.GetFileName(name, folder);
            File.WriteAllBytes(filename, jpgdata);
        }
    }
}
