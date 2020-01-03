namespace ImageBank
{
    public static class AppConsts
    {
        public const string PathRoot = @"D:\Users\Murad\Documents\SDb\";
        public const string PathCollection = PathRoot + @"Hp\";
        public const string FileDatabase = PathRoot + @"Db\images.mdf";
        public const string FileSiftDescriptors = PathRoot + @"Db\images_siftd.bin";
        public const string FileSiftClusters = PathRoot + @"Db\images_sift.csv";

        public const int MaxOrbsInImage = 250; // 250 * 32 = 8000
        public const int MaxDescriptorsInImage = 1000; // 1000 * 2 = 2000
        public const int MaxImportImages = 1000000;
        public const int MaxFlann = 500;
        public const int MaxDescriptorsForClustering = 6553600;
        public const int MaxClusters = 0x10000;

        public const string DatExtension = ".dat";
        public const string WebpExtension = ".webp";
        public const string JpgExtension = ".jpg";
        public const string JpegExtension = ".jpeg";
        public const string PngExtension = ".png";
        public const string BmpExtension = ".bmp";

        public const double WindowMargin = 5.0;
        public const double TimeLapse = 500.0;

        public const string AttrHash = "Hash";
        public const string AttrFolder = "Folder";
        public const string AttrNextHash = "NextHash";
        public const string AttrLastView = "LastView";
        public const string AttrLastCheck = "LastCheck";
        public const string AttrOrbs = "Orbs";
    }
}
