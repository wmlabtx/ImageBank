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
    }
}