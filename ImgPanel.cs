using System;
using System.Drawing;

namespace ImageBank
{
    public class ImgPanel
    {
        public string Hash { get; }
        public string Subdirectory { get; }
        public DateTime LastView { get; }
        public float Sim { get; }
        public int Generation { get; }
        public DateTime LastChange { get; }
        public Bitmap Bitmap { get; }
        public int Length { get; }

        public ImgPanel(string hash, string subdirectory, DateTime lastview, int generation, float sim, DateTime lastchange, Bitmap bitmap, int length)
        {
            Hash = hash;
            Subdirectory = subdirectory;
            LastView = lastview;
            Generation = generation;
            Sim = sim;
            LastChange = lastchange;
            Bitmap = bitmap;
            Length = length;
        }
    }
}
