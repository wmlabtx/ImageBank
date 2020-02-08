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

        public static string GetPath(string filename)
        {
            var path = Path.GetDirectoryName(filename).Substring(AppConsts.PathCollection.Length);
            return path;
        }

        public static string GetPassword(string filename)
        {
            var password = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            return password;
        }

        public static string GetFileName(string name, string path)
        {
            var filename = $"{AppConsts.PathCollection}{path}\\{name}{AppConsts.JpgExtension}";
            return filename;
        }

        public static string GetLegacyPath(int id)
        {
            return $"{AppConsts.PathLegacy}{id:D02}";
        }

        public static int GetIdLegacy(string path)
        {
            if (path.Length < 4) {
                return -1;
            }

            var backslash = path.Substring(path.Length - 3, 1);
            if (!backslash.Equals("\\")) {
                return -1;
            }

            var str = path.Substring(path.Length - 2, 2);
            if (!int.TryParse(str, out var id)) {
                return -1;
            }

            if (id < 0) {
                return -1;
            }

            return id;
        }

        public static string AddChecksum(string name, string checksum)
        {
            var suffix = $"({checksum.Substring(0, 8)})";
            var pos = name.LastIndexOf('(');
            if (pos < 0) {
                return $"{name}{suffix}";
            }
            else {
                return $"{name.Substring(0, pos)}{suffix}";
            }
        }
    }
}