using System;
using System.IO;
using System.Text;

namespace ImageBank
{
    public static class HelperPath
    {
        public static string GetFileName(string name, string folder)
        {
            var sb = new StringBuilder(AppConsts.PathRoot);
            if (!string.IsNullOrEmpty(folder))
            {
                sb.Append(folder);
                sb.Append('\\');
            }

            sb.Append(name);
            sb.Append(AppConsts.JpgExtension);

            return sb.ToString();
        }

        public static string GetName(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename)?.ToLowerInvariant();
            return name;
        }

        public static string GetFolder(string filename)
        {
            if (string.IsNullOrEmpty(filename) || filename.Length <= AppConsts.PathRoot.Length)
            {
                return null;
            }

            var part = filename.Substring(AppConsts.PathRoot.Length);
            var pos = part.LastIndexOf('\\');
            if (pos <= 0)
            {
                return string.Empty;
            }

            var folder = part.Substring(0, pos);
            return folder;
        }

        public static bool FolderComparable(string folderX, string folderY)
        {
            if (
                IsLegacy(folderX) ||
                folderY.StartsWith(folderX, StringComparison.InvariantCultureIgnoreCase)
                )
            {
                return true;
            }

            return false;
        }

        public static bool IsLegacy(string folder)
        {
            return folder.StartsWith(AppConsts.FolderLegacy, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}