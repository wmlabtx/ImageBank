using System;
using System.Security.Cryptography;
using System.Text;

namespace ImageBank
{
    public static class HelperCrc
    {
        public static string GetCrc(byte[] array)
        {
            const string charlist = "0123456789abcdefghijklmnopqrstuvwxyz";

            if (array == null || array.Length == 0)
            {
                return null;
            }

            var crc = SHA256.Create().ComputeHash(array, 0, array.Length);
            var crcushort = new ushort[crc.Length / sizeof(ushort)];
            Buffer.BlockCopy(crc, 0, crcushort, 0, crc.Length);

            var sb = new StringBuilder();
            for (var i = 0; i < 9; i++)
            {
                sb.Append(charlist[crcushort[i] % charlist.Length]);
                if (i == 2 || i == 5)
                {
                    sb.Append('-');
                }
            }

            return sb.ToString();
        }
    }
}
