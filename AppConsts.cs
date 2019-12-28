namespace ImageBank
{
    public static class AppConsts
    {
        public const string PathRoot = @"D:\Users\Murad\Documents\Sdb\";
        public const string PathCollection = PathRoot + @"Hp\";
        public const string PathDatabase = PathRoot + @"Db\";
        public const string FolderLegacy = "Legacy";
        public const string FileDatabase = PathDatabase + @"images.mdf";
        public const string FileOrbs = PathDatabase + @"images_orbs.ddf";

        public const int MaxDescriptorsInImage = 500;
        public const int MaxOrbsSize = MaxDescriptorsInImage * 32; // 16000        

        public const string DatExtension = ".dat";
        public const string WebpExtension = ".webp";
        public const string JpgExtension = ".jpg";
        public const string JpegExtension = ".jpeg";
        public const string PngExtension = ".png";
        public const string BmpExtension = ".bmp";

        public const double WindowMargin = 5.0;
        public const double TimeLapse = 500.0;
        public const int MaxImport = 100000;

        public const string AttrName = "Name";
        public const string AttrFolder = "Folder";
        public const string AttrLastView = "LastView";
        public const string AttrLastCheck = "LastCheck";
        public const string AttrLastChange = "LastChange";
        public const string AttrNextName = "NextName";
        public const string AttrDistance = "Distance";
        public const string AttrId = "Id";
        public const string AttrLastId = "LastId";
        public const string AttrOrbsSlot = "OrbsSlot";
        public const string AttrOrbsLength = "OrbsLength";
    }
}
