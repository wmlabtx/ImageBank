using System;
using System.Security.Cryptography;
using System.Text;

namespace ImageBank
{
    public static class HelperHash
    {
        public static string CalculateHash(byte[] array)
        {
            const string c = "0123456789abcdefghijklmnopqrstuvwxyz";

            if (array == null || array.Length == 0)
            {
                return null;
            }

            using (var sha256 = SHA256.Create())
            {
                var crc = sha256.ComputeHash(array, 0, array.Length);
                var crcushort = new ushort[crc.Length / sizeof(ushort)];
                Buffer.BlockCopy(crc, 0, crcushort, 0, crc.Length);
                var sb = new StringBuilder();
                sb.Append(c[crcushort[1] % c.Length]);
                sb.Append(c[crcushort[3] % c.Length]);
                sb.Append(c[crcushort[5] % c.Length]);
                sb.Append(c[crcushort[7] % c.Length]);
                sb.Append(c[crcushort[9] % c.Length]);
                sb.Append(c[crcushort[11] % c.Length]);
                sb.Append(c[crcushort[13] % c.Length]);
                sb.Append(c[crcushort[15] % c.Length]);
                return sb.ToString();
            }
        }
    }
}