using System;
namespace ImageBank
{
    public class SiftPoint
    {
        public byte[] V { get; }

        public SiftPoint(byte[] buffer, int offset)
        {
            V = new byte[128];
            Buffer.BlockCopy(buffer, offset, V, 0, 128);
        }
    }
}
