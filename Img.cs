using OpenCvSharp;
using System;
using System.IO;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }
        
        private DateTime _lastview;
        public DateTime LastView
        {
            get
            {
                return _lastview;
            }
            set
            {
                _lastview = value;
                AppVars.Collection.UpdateProperty(this, AppConsts.AttrLastView, _lastview);
            }
        }

        public DateTime LastChecked { get; set; }
        public DateTime LastChanged { get; set; }
        public string NextName { get; set; }

        private Mat _orbs;
        public Mat Orbs
        {
            get
            {
                return _orbs;
            }
            set
            {
                _orbs = value;
                var buffer = HelperDescriptors.ConvertMatToBuffer(_orbs);
                AppVars.Collection.UpdateProperty(this, AppConsts.AttrOrbs, buffer);
            }
        }

        public float Sim { get; set; }

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                AppVars.Collection.UpdateProperty(this, AppConsts.AttrId, _id);
            }
        }

        public int LastId { get; set; }

        public string Subdirectory
        {
            get
            {
                var hash = Math.Abs(Name.GetHashCode()) % 100;
                var dir = $"{hash:D2}";
                return dir;
            }
        }

        public string Folder
        {
            get
            {
                var directory = $"{AppConsts.PathData}{Subdirectory}\\";
                return directory;
            }
        }

        public string File
        {
            get
            {
                var filename = $"{Folder}{Name}{AppConsts.DatExtension}";
                return filename;
            }
        }

        public Img(
            string name,
            DateTime lastview,
            DateTime lastchecked,
            DateTime lastchanged,
            string nextname,
            Mat orbs,
            float sim,
            int id,
            int lastid)
        {
            Name = name;
            _lastview = lastview;
            LastChecked = lastchecked;
            LastChanged = lastchanged;
            NextName = nextname;
            _orbs = orbs;
            Sim = sim;
            _id = id;
            LastId = lastid;
        }

        public void WriteData(byte[] jpgdata)
        {
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            var buffer = HelperEncrypting.Encrypt(jpgdata, Name);
            System.IO.File.WriteAllBytes(File, buffer);
        }

        public byte[] GetData()
        {
            if (!System.IO.File.Exists(File))
            {
                return null;
            }

            var buffer = System.IO.File.ReadAllBytes(File);
            var data = HelperEncrypting.Decrypt(buffer, Name);
            return data;
        }
    }
}