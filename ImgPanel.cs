using System;
using System.Drawing;

namespace ImageBank
{
    public class ImgPanel
    {
        public string Name { get; }
        public string Folder { get; }
        public int FolderSize { get; }
        public DateTime LastView { get; }
        public DateTime LastChange { get; }
        public float Distance { get; }
        public Bitmap Bitmap { get; }
        public int Length { get; }

        public ImgPanel(string name, string folder, int foldersize, DateTime lastview, DateTime lastchange, float distance, Bitmap bitmap, int length)
        {
            Name = name;
            Folder = folder;
            FolderSize = foldersize;
            LastView = lastview;
            LastChange = lastchange;
            Distance = distance;
            Bitmap = bitmap;
            Length = length;
        }
    }
}
