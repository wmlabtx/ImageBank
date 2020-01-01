using System.IO;

namespace ImageBank
{
    public static class HelperPath
    {
        public static string GetName(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            return name;
        }

        public static string GetPassword(string filename)
        {
            var password = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            return password;
        }

        public static string GetFileName(string hash, string directory)
        {
            var filename = $"{directory}\\{hash}{AppConsts.DatExtension}";
            return filename;
        }
    }
}