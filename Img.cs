using System;

namespace ImageBank
{
    public class Img
    {
        public int Id { get; }
        public string Name { get; }

        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrPath, value);
            }
        }

        public string Checksum { get; }

        private int _generation;
        public int Generation
        {
            get => _generation;
            set
            {
                _generation = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrGeneration, value);
            }
        }

        private int _nextid;
        public int NextId
        {
            get => _nextid;
            set
            {
                _nextid = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrNextId, value);
            }
        }

        private int _distance;
        public int Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrDistance, value);
            }
        }

        private int _lastid;
        public int LastId
        {
            get
            {
                return _lastid;
            }
            set
            {
                _lastid = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrLastId, value);
            }
        }

        private DateTime _lastview;
        public DateTime LastView
        {
            get => _lastview;
            set
            {
                _lastview = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrLastView, value);
            }
        }

        private DateTime _lastchange;
        public DateTime LastChange
        {
            get => _lastchange;
            set
            {
                _lastchange = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrLastChange, value);
            }
        }

        public ulong Phash { get; set; }

        private readonly ulong[] _orbv;

        public ulong[] Orbv()
        {
            return _orbv;
        }

        public string Directory
        {
            get
            {
                var directory = $"{AppConsts.PathCollection}{Path}\\";
                return directory;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
        public string File
        {
            get
            {
                var filename = Helper.GetFileName(Name.ToLowerInvariant(), Path);
                return filename;
            }
        }

        public Img(
            int id,
            string name,
            string path,
            string checksum,
            int generation,
            DateTime lastview,
            int nextid,
            int distance,
            int lastid,
            DateTime lastchange,
            ulong phash,
            ulong[] orbv)
        {
            Id = id;
            Name = name;
            _path = path;
            Checksum = checksum;
            _generation = generation;
            _lastview = lastview;
            _nextid = nextid;
            _distance = distance;
            _lastid = lastid;
            _lastchange = lastchange;
            Phash = phash;
            _orbv = orbv;
        }
    }
}