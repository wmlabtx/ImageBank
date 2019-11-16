using System;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }
        public string Person { get; set; }
        public ulong PHash { get; set; }
        public int Distance { get; set; }
        public DateTime LastView { get; set; }
        public DateTime LastChecked { get; set; }
        public DateTime LastChanged { get; set; }
        public string NextName { get; set; }
        public long Offset { get; set; }
        public int Lenght { get; set; }
        public string Crc { get; set; }

        public Img(
            string name,
            string person,
            ulong phash,
            int distance,
            DateTime lastview,
            DateTime lastchecked,
            DateTime lastchanged,
            string nextname,
            long offset,
            int lenght,
            string crc)
        {
            Name = name;
            Person = person;
            PHash = phash;
            Distance = distance;
            LastView = lastview;
            LastChecked = lastchecked;
            LastChanged = lastchanged;
            NextName = nextname;
            Offset = offset;
            Lenght = lenght;
            Crc = crc;
        }

        public void SetViewed()
        {
            LastView = DateTime.Now;
            AppVars.Collection.UpdateLastView(this);
        }
    }
}