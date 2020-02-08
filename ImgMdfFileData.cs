using System.IO;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public static byte[] ReadJpgData(string name, string path)
        {
            var filename = HelperPath.GetFileName(name, path);
            if (!File.Exists(filename)) {
                return null;
            }

            var jpgdata = File.ReadAllBytes(filename);
            return jpgdata;
        }

        public static void WriteJpgData(string name, string path, byte[] jpgdata)
        {
            var directory = $"{AppConsts.PathCollection}{path}";
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            var filename = HelperPath.GetFileName(name, path);
            File.WriteAllBytes(filename, jpgdata);
        }
    }
}
