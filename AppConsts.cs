namespace ImageBank
{
    public static class AppConsts
    {
        private const string PathRoot = @"D:\Users\Murad\Documents\SDb\";
        public const string PathCollection = PathRoot + @"Hp\";
        public const string PathRecycle = PathRoot + @"Rb\";
        public const string FileDatabase = PathRoot + @"Db\images.mdf";

        public const int MaxImportImages = 128 * 1024 /*16*/;
        public const int MaxImages = 128 * 1024;

        public const string DatExtension = ".dat";
        public const string WebpExtension = ".webp";
        public const string JpgExtension = ".jpg";
        public const string JpegExtension = ".jpeg";
        public const string PngExtension = ".png";
        public const string BmpExtension = ".bmp";

        public const double WindowMargin = 5.0;
        public const double TimeLapse = 500.0;

        public const string TableImages = "Images";
        public const string AttrHash = "Hash";
        public const string AttrId = "Id";
        public const string AttrGeneration = "Generation";
        public const string AttrLastView = "LastView";
        public const string AttrNextHash = "NextHash";
        public const string AttrSim = "Sim";
        public const string AttrLastId = "LastId";
        public const string AttrLastChange = "LastChange";
        public const string AttrDescriptors = "Descriptors";
        public const string TableVars = "Vars";
    }
}
