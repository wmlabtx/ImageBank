using OpenCvSharp;
using System;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }
        
        private string _person;
        public string Person
        {
            get
            {
                return _person;
            }
            set
            {
                _person = value;
                AppVars.Collection.UpdateProperty(this, AppConsts.AttrPerson, _person);
            }
        }

        private ulong _phash;
        public ulong PHash
        {
            get
            {
                return _phash;
            }
            set
            {
                _phash = value;
                var buffer = HelperDescriptors.ConvertPHashToBuffer(_phash);
                AppVars.Collection.UpdateProperty(this, AppConsts.AttrPHash, buffer);
            }
        }

        public int Distance { get; set; }


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
        public long Offset { get; set; }
        public int Lenght { get; set; }
        public string Crc { get; set; }

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
            string crc,
            Mat orbs,
            float sim,
            int id,
            int lastid)
        {
            Name = name;
            _person = person;
            _phash = phash;
            Distance = distance;
            _lastview = lastview;
            LastChecked = lastchecked;
            LastChanged = lastchanged;
            NextName = nextname;
            Offset = offset;
            Lenght = lenght;
            Crc = crc;
            _orbs = orbs;
            Sim = sim;
            _id = id;
            LastId = lastid;
        }
    }
}