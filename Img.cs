using OpenCvSharp;
using System;

namespace ImageBank
{
    public class Img
    {
        public string Name { get; }

        private string _folder;
        public string Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrFolder, _folder);
            }
        }

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
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrLastView, _lastview);
            }
        }

        public DateTime LastCheck { get; set; }
        public DateTime LastChange { get; set; }
        public string NextName { get; set; }
        public float Distance { get; set; }

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
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrId, _id);
            }
        }

        public int LastId { get; set; }

        private int _orbsslot;
        public int OrbsSlot
        {
            get
            {
                return _orbsslot;
            }
            set
            {
                _orbsslot = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrOrbsSlot, _orbsslot);
            }
        }

        private int _orbslength;
        public int OrbsLength
        {
            get
            {
                return _orbslength;
            }
            set
            {
                _orbslength = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrOrbsLength, _orbslength);
            }
        }

        public string DataFile
        {
            get
            {
                var filename = HelperPath.GetFileName(Name, Folder);
                return filename;
            }
        }

        public Mat Orbs
        {
            get
            {
                var mat = AppVars.Collection.OrbsFile.Get(OrbsSlot, OrbsLength);
                return mat;
            }
            set
            {
                AppVars.Collection.OrbsFile.Set(OrbsSlot, value);
            }
        }

        public Img(
            string name,
            string folder,
            DateTime lastview,
            DateTime lastcheck,
            DateTime lastchange,
            string nextname,
            float distance,
            int id,
            int lastid,
            int orbsslot,
            int orbslength)
        {
            Name = name;
            _folder = folder;
            _lastview = lastview;
            LastCheck = lastcheck;
            LastChange = lastchange;
            NextName = nextname;
            Distance = distance;
            _id = id;
            LastId = lastid;
            _orbsslot = orbsslot;
            _orbslength = orbslength;
        }
    }
}