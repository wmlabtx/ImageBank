using System;
using System.Collections;

namespace ImageBank
{
    public class OrbPoint
    {
        private readonly ulong[] _descriptor;

        public OrbPoint(byte[] array, int offset)
        {
            _descriptor = new ulong[4];
            Buffer.BlockCopy(array, offset, _descriptor, 0, 32);
        }

        public object HelperConvertor { get; private set; }

        public byte[] GetBuffer()
        {
            var buffer = HelperConvertors.ConvertToBytes(_descriptor);
            return buffer;
        }

        public bool IsBit(int index)
        {
            var ba = new BitArray(GetBuffer());
            return ba[index];
        }
    }
}
