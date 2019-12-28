using System.IO;

namespace ImageBank
{
    public static class HelperPath
    {
        public static string GetPassword(string filename)
        {
            var password = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            return password;
        }

        public static string GetFolder(string filename)
        {
            var folder = Path.GetDirectoryName(filename);
            folder = folder.Substring(AppConsts.PathCollection.Length);
            return folder;
        }

        public static string GetFileName(string name, string folder)
        {
            var filename = $"{AppConsts.PathCollection}{folder}\\{name}{AppConsts.JpgExtension}";
            return filename;
        }
    }
}