using System;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }
        public string Folder { get; set; }
        public DateTime LastView { get; set; }
        public DateTime LastChecked { get; set; }
        public ulong[] Descriptors { get; set; }
        public string NextName { get; set; }
        public float Sim { get; set; }
        public string FileName => HelperPath.GetFileName(Name, Folder);

        public Img(
            string name,
            string folder,
            DateTime lastview,
            DateTime lastchecked,
            ulong[] descriptors,
            string nextname,
            float sim)
        {
            Name = name;
            Folder = folder;
            LastView = lastview;
            LastChecked = lastchecked;
            Descriptors = descriptors;
            NextName = nextname;
            Sim = sim;
        }
    }
}