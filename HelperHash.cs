using System.Security.Cryptography;
using System.Text;

namespace ImageBank
{
    public static class HelperHash
    {
        public static string Compute(byte[] array)
        {
            string hashString;
            using (var sha256 = SHA256.Create()) {
                var hash = sha256.ComputeHash(array);
                StringBuilder result = new StringBuilder(64);
                foreach (var e in hash) {
                    result.Append(e.ToString("x2"));
                }
                hashString = result.ToString();
            }
            
            return hashString;
        }
    }
}