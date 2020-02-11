namespace ImageBank
{
    public static class AppConsts
    {
        private const string PathRoot = @"D:\Users\Murad\Documents\SDb\";
        public const string PathCollection = PathRoot + @"Hp\";
        public const string PathRecycle = PathRoot + @"Rb\";
        public const string FileDatabase = PathRoot + @"Db\images.mdf";
        public const string PathLegacy = @"Legacy\";
        public const string PrefixMzx = @"mzx";

        public const int MaxImportImages = 200000;
        public const int MaxImages = 200000;

        public const string DatExtension = ".dat";
        public const string WebpExtension = ".webp";
        public const string JpgExtension = ".jpg";
        public const string JpegExtension = ".jpeg";
        public const string PngExtension = ".png";
        public const string BmpExtension = ".bmp";

        public const double WindowMargin = 5.0;
        public const double TimeLapse = 500.0;

        public const string TableImages = "Images";
        public const string AttrId = "Id";
        public const string AttrName = "Name";
        public const string AttrPath = "Path";
        public const string AttrChecksum = "Checksum";
        public const string AttrGeneration = "Generation";
        public const string AttrLastView = "LastView";
        public const string AttrNextId = "NextId";
        public const string AttrMatch = "Match";
        public const string AttrLastCheck = "LastCheck";
        public const string AttrLastChange = "LastChange";
        public const string AttrDescriptors = "Descriptors";
        public const string TableVars = "Vars";
    }
}
