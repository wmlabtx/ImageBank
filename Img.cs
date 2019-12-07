using Emgu.CV.Cuda;
using System;
using System.IO;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }

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

        private GpuMat _descriptors;
        public GpuMat Descriptors
        {
            get
            {
                return _descriptors;
            }
            set
            {
                _descriptors = value;
                var buffer = HelperConvertors.ConvertMatToBuffer(_descriptors);
                AppVars.Collection.UpdateProperty(this, AppConsts.AttrDescriptors, buffer);
             }
        }

        public float Sim { get; set; }

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
            int id,
            int lastid,
            DateTime lastview,
            DateTime lastchecked,
            DateTime lastchanged,
            string nextname,
            GpuMat descriptors,
            float sim)
        {
            Name = name;
            _id = id;
            LastId = lastid;
            _lastview = lastview;
            LastChecked = lastchecked;
            LastChanged = lastchanged;
            NextName = nextname;
            _descriptors = descriptors;
            Sim = sim;
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