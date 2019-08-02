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

        public static string GetPassword(string filename)
        {
            var password = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            return password;
        }

        public static string GetName(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename)?.ToLowerInvariant();
            return name;
        }

        public static string GetNode(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var pos = name.LastIndexOf('.');
            if (pos <= 0)
            {
                return string.Empty;
            }

            var node = name.Substring(0, pos);
            return node;
        }

        public static bool NodesComparable(string nodeX, string nodeY)
        {
            if (
                string.IsNullOrEmpty(nodeX) ||
                (!string.IsNullOrEmpty(nodeY) && nodeY.StartsWith(nodeX, StringComparison.InvariantCultureIgnoreCase))
                )
            {
                return true;
            }

            return false;
        }
    }
}