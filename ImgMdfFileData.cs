using System.IO;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public byte[] ReadJpgData(string hash, string directory)
        {
            var filename = HelperPath.GetFileName(hash, directory);
            if (!File.Exists(filename))
            {
                return null;
            }

            var data = File.ReadAllBytes(filename);
            var jpgdata = HelperEncrypting.Decrypt(data, hash);
            return jpgdata;
        }

        public void WriteJpgData(string hash, string directory, byte[] jpgdata)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filename = HelperPath.GetFileName(hash, directory);
            var data = HelperEncrypting.Encrypt(jpgdata, hash);
            File.WriteAllBytes(filename, data);
        }
    }
}
