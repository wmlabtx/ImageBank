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
        public float Quality { get; }
        public int Generation { get; }
        public DateTime LastChange { get; }
        public Bitmap Bitmap { get; }
        public long Length { get; }
        public int Descriptors { get; }

        public ImgPanel(int id, string name, string path, DateTime lastview, int generation, float quality, DateTime lastchange, Bitmap bitmap, long length, int descriptors)
        {
            Id = id;
            Name = name;
            Path = path;
            LastView = lastview;
            Generation = generation;
            Quality = quality;
            LastChange = lastchange;
            Bitmap = bitmap;
            Length = length;
            Descriptors = descriptors;
        }
    }
}
