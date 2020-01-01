using System;
using System.Drawing;

namespace ImageBank
{
    public class ImgPanel
    {
        public string Hash { get; }
        public string Folder { get; }
        public int FolderSize { get; }
        public DateTime LastView { get; }
        public DateTime LastCheck { get; }
        public Bitmap Bitmap { get; }
        public int Length { get; }

        public ImgPanel(string hash, string folder, int foldersize, DateTime lastview, DateTime lastcheck, Bitmap bitmap, int length)
        {
            Hash = hash;
            Folder = folder;
            FolderSize = foldersize;
            LastView = lastview;
            LastCheck = lastcheck;
            Bitmap = bitmap;
            Length = length;
        }
    }
}
