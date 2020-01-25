using Crc32C;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ImageBank
{
    public static class HelperHash
    {
        public static string ComputeName(byte[] array)
        {
            var uintcrc = Crc32CAlgorithm.Compute(array);
            var crc = $"{uintcrc:x8}";
            return crc;
        }
    }
}