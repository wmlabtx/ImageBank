﻿namespace ImageBank
{
    public static class AppConsts
    {
        public const string PathRoot = @"D:\Users\Murad\Documents\SDB\";
        public const string PathSource = PathRoot + @"hp\";
        public const string FileDatabase = PathRoot + @"db\images.mdf";
        public const string FileData = PathRoot + @"db\images_data.ddf";

        public const int MaxHammingDistance = 64;

        public const string DatExtension = ".dat";
        public const string WebpExtension = ".webp";
        public const string JpgExtension = ".jpg";
        public const string JpegExtension = ".jpeg";
        public const string PngExtension = ".png";
        public const string BmpExtension = ".bmp";

        public const double WindowMargin = 5.0;
        public const double TimeLapse = 500.0;

        public const string AttrName = "Name";
        public const string AttrPerson = "Person";
        public const string AttrPHash = "PHash";
        public const string AttrDistance = "Distance";
        public const string AttrLastView = "LastView";
        public const string AttrLastChecked = "LastChecked";
        public const string AttrLastChanged = "LastChanged";
        public const string AttrNextName = "NextName";
        public const string AttrOffset = "Offset";
        public const string AttrLenght = "Lenght";
        public const string AttrCrc = "Crc";
    }
}
