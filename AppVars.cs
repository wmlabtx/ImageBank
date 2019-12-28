using System;
using System.Threading;

namespace ImageBank
{
    public static class AppVars
    {
        public static readonly ImgMdf Collection = new ImgMdf();
        public static ImgPanel[] ImgPanel = new ImgPanel[2];
        public static Progress<string> Progress;
        public static ManualResetEvent SuspendEvent;
        public static float DistanceMedian = 257f;
    }
}