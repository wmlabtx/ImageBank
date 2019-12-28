using System;
using System.Security.Cryptography;
using System.Text;

namespace ImageBank
{
    public static class HelperHash
    {
        public static string Pattern => "xxxx-xxxx-xxxx";

        public static string CalculateHash(byte[] array)
        {
            const string cs = "0123456789bcdfghjklmnpqrstvwxz";
            const string cg = "0123456789aeiouy";

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
                sb.Append(cs[crcushort[1] % cs.Length]);
                sb.Append(cg[crcushort[2] % cg.Length]);
                sb.Append(cs[crcushort[3] % cs.Length]);
                sb.Append(cg[crcushort[4] % cg.Length]);
                sb.Append('-');
                sb.Append(cg[crcushort[5] % cg.Length]);
                sb.Append(cs[crcushort[6] % cs.Length]);
                sb.Append(cs[crcushort[7] % cs.Length]);
                sb.Append(cg[crcushort[9] % cg.Length]);
                sb.Append('-');
                sb.Append(cs[crcushort[11] % cs.Length]);
                sb.Append(cg[crcushort[12] % cg.Length]);
                sb.Append(cg[crcushort[13] % cg.Length]);
                sb.Append(cs[crcushort[14] % cs.Length]);
                return sb.ToString();
            }
        }
    }
}