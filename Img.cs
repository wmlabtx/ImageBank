using System;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }
        public int Stars { get; set; }
        public DateTime LastView { get; set; }
        public DateTime LastChecked { get; set; }
        public DateTime LastChanged { get; set; }
        public ulong[] Descriptors { get; set; }
        public string NextName { get; set; }
        public float Sim { get; set; }
        public long Offset { get; set; }
        public int Lenght { get; set; }
        public string Crc { get; set; }

        public Img(
            string name,
            int stars,
            DateTime lastview,
            DateTime lastchecked,
            DateTime lastchanged,
            ulong[] descriptors,
            string nextname,
            float sim,
            long offset,
            int lenght,
            string crc)
        {
            Name = name;
            Stars = stars;
            LastView = lastview;
            LastChecked = lastchecked;
            LastChanged = lastchanged;
            Descriptors = descriptors;
            NextName = nextname;
            Sim = sim;
            Offset = offset;
            Lenght = lenght;
            Crc = crc;
        }

        public void SetViewed()
        {
            LastView = DateTime.Now;
            Stars++;
        }
    }
}