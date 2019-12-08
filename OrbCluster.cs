using System;
using System.Collections;
using System.Collections.Generic;

namespace ImageBank
{
    public class OrbCluster
    {
        public List<OrbPoint> OrbPoints;

        public OrbCluster()
        {
            OrbPoints = new List<OrbPoint>();
        }

        private int[] GetStat()
        {
            var bstat = new int[256];
            foreach (var orbpoint in OrbPoints)
            {
                var ba = new BitArray(orbpoint.GetBuffer());
                for (var i = 0; i < 256; i++)
                {
                    if (ba[i])
                    {
                        bstat[i]++;
                    }
                }
            }

            return bstat;
        }

        public int GetMedianBit()
        {
            var bstat = GetStat();
            var mid = OrbPoints.Count / 2;
            var medianbit = 0;
            var mindelta = OrbPoints.Count;
            for (var i = 0; i < 256; i++)
            {
                var delta = Math.Abs(bstat[i] - mid);
                if (delta < mindelta)
                {
                    medianbit = i;
                    mindelta = delta;
                }
            }

            return medianbit;
        }

        public ulong[] GetCenter()
        {
            var bstat = GetStat();
            var mid = OrbPoints.Count / 2;
            var result = new byte[32];
            var ib = 0;
            byte mask = 0x01;
            for (var i = 0; i < 256; i++)
            {
                if (bstat[i] > mid)
                {
                    result[ib] |= mask;
                }

                if (mask == 0x80)
                {
                    ib++;
                    mask = 0x01;
                }
                else
                {
                    mask <<= 1;
                }
            }

            var center = HelperConvertors.ConvertToUlongs(result);
            return center;
        }
    }
}
