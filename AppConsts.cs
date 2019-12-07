namespace ImageBank
{
    public static class AppConsts
    {
        public const string PathRoot = @"D:\Users\Murad\Documents\SDB\";
        public const string PathData = PathRoot + @"hp\";
        public const string PathSource = PathRoot + @"rw\";
        public const string FileDatabase = PathRoot + @"db\images.mdf";

        public const int MaxDescriptorsInImage = 250;

        public const string DatExtension = ".dat";
        public const string WebpExtension = ".webp";
        public const string JpgExtension = ".jpg";
        public const string JpegExtension = ".jpeg";
        public const string PngExtension = ".png";
        public const string BmpExtension = ".bmp";

        public const double WindowMargin = 5.0;
        public const double TimeLapse = 500.0;

        public const string AttrName = "Name";
        public const string AttrId = "Id";
        public const string AttrLastId = "LastId";
        public const string AttrLastView = "LastView";
        public const string AttrLastChecked = "LastChecked";
        public const string AttrLastChanged = "LastChanged";
        public const string AttrNextName = "NextName";
        public const string AttrDescriptors = "Descriptors";
        public const string AttrSim = "Sim";
    }
}
