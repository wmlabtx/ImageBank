namespace ImageBank
{
    public static class AppConsts
    {
        private const string PathRoot = @"D:\Users\Murad\Documents\SDb\";
        public const string PathCollection = PathRoot + @"Hp\";
        public const string PathLegacy = PathRoot + @"Legacy\";
        public const string FileDatabase = PathRoot + @"Db\images.mdf";
        public const string PrefixName = @"MX.";

        public const int MaxImages = 200000;
        public const int MaxDescriptorsInImage = 2000;

        public const string WebpExtension = ".webp";
        public const string JpgExtension = ".jpg";
        public const string JpegExtension = ".jpeg";
        public const string PngExtension = ".png";
        public const string BmpExtension = ".bmp";

        public const double WindowMargin = 5.0;
        public const double TimeLapse = 500.0;

        public const string TableImages = "Images";
        public const string AttrId = "Id"; // 1 or 567 or 128092
        public const string AttrName = "Name"; // mx.f6 or mx.3 or mx.b77sk
        public const string AttrPath = "Path"; // Legacy\00 or Ls\Rina
        public const string AttrChecksum = "Checksum"; // f0s...44j (50 lenght)
        public const string AttrGeneration = "Generation"; // 0,1,2,...
        public const string AttrLastView = "LastView";
        public const string AttrNextId = "NextId"; // 1 or 567 or 128092
        public const string AttrDistance = "Distance"; // 0 or 44 or 255
        public const string AttrLastId = "LastId"; // 1 or 567 or 128092
        public const string AttrLastChange = "LastChange";
        public const string AttrPhash = "Phash";
        public const string AttrOrbv = "Orbv";
        public const string TableVars = "Vars";
    }
}
