using System;
using System.Security.Cryptography;

namespace ImageBank
{
    public static class HelperRandom
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        public static int Next(int max)
        {
            var buffer = new byte[sizeof(ulong)];
            Rng.GetBytes(buffer);
            var random = (int)((double)BitConverter.ToUInt64(buffer, 0) * max / ulong.MaxValue);
            return random;
        }
    }
}
