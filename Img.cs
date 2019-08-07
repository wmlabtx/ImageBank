using System;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }
        public int Cluster { get; set; }
        public int Gen { get; private set; }
        public DateTime LastView { get; set; }
        public DateTime LastChecked { get; private set; }
        public ulong[] Descriptors { get; set; }
        public string NextName { get; private set; }
        public float Sim { get; private set; }
        public long Offset { get; set; }
        public int Lenght { get; set; }
        public string Crc { get; set; }

        public const int GenNew = 0;
        public const int GenModified = 1;
        public const int GenViewed = 2;

        public Img(
            string name,
            int cluster,
            int gen,
            DateTime lastview,
            DateTime lastchecked,
            ulong[] descriptors,
            string nextname,
            float sim,
            long offset,
            int lenght,
            string crc)
        {
            Name = name;
            Cluster = cluster;
            Gen = gen;
            LastView = lastview;
            LastChecked = lastchecked;
            Descriptors = descriptors;
            NextName = nextname;
            Sim = sim;
            Offset = offset;
            Lenght = lenght;
            Crc = crc;
        }

        public void SetNextName(string nextname, float sim)
        {
            NextName = nextname;
            Sim = sim;
            LastChecked = DateTime.Now;
            if (Gen == GenViewed)
            {
                Gen = GenModified;
            }
        }

        public void SetNextName(string nextname, DateTime min)
        {
            NextName = nextname;
            LastChecked = min;
            Sim = 0f;
            if (Gen == GenViewed)
            {
                Gen = GenModified;
            }
        }

        public void SetNextName()
        {
            LastChecked = DateTime.Now;
        }

        public void SetViewed()
        {
            LastView = DateTime.Now;
            Gen = GenViewed;
        }
    }
}