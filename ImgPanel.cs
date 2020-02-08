using System;
using System.Drawing;

namespace ImageBank
{
    public class ImgPanel
    {
        public int Id { get; }
        public string Name { get; }
        public string Path { get; }
        public DateTime LastView { get; }
        public int Match { get; }
        public int Generation { get; }
        public DateTime LastChange { get; }
        public Bitmap Bitmap { get; }
        public long Length { get; }

        public ImgPanel(int id, string name, string path, DateTime lastview, int generation, int match, DateTime lastchange, Bitmap bitmap, long length)
        {
            Id = id;
            Name = name;
            Path = path;
            LastView = lastview;
            Generation = generation;
            Match = match;
            LastChange = lastchange;
            Bitmap = bitmap;
            Length = length;
        }
    }
}
