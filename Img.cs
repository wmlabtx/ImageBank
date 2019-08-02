using System;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }
        public string Node { get; set; }
        public DateTime LastView { get; set; }
        public DateTime LastChecked { get; set; }
        public ulong[] Descriptors { get; set; }
        public string NextName { get; set; }
        public float Sim { get; set; }
        public long Offset { get; set; }
        public int Lenght { get; set; }
        public string Crc { get; set; }

        public Img(
            string name,
            string node,
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
            Node = node;
            LastView = lastview;
            LastChecked = lastchecked;
            Descriptors = descriptors;
            NextName = nextname;
            Sim = sim;
            Offset = offset;
            Lenght = lenght;
            Crc = crc;
        }
    }
}