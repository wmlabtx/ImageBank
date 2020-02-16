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

        private int _match;
        public int Match
        {
            get => _match;
            set
            {
                _match = value;
                ImgMdf.SqlUpdateProperty(Id, AppConsts.AttrMatch, value);
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

        public float Quality { get; }

        private readonly uint[] _descriptors;

        public uint[] GetDescriptors()
        {
            return _descriptors;
        }

        public string Directory
        {
            get
            {
                var directory = $"{AppConsts.PathCollection}{Path}\\";
                return directory;
            }
        }

        public string File
        {
            get
            {
                var filename = $"{Directory}{Name}{AppConsts.MzxExtension}";
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
            int match,
            int lastid,
            DateTime lastchange,
            float quality,
            uint[] descriptors)
        {
            Id = id;
            Name = name;
            _path = path;
            Checksum = checksum;
            _generation = generation;
            _lastview = lastview;
            _nextid = nextid;
            _match = match;
            _lastid = lastid;
            _lastchange = lastchange;
            Quality = quality;
            _descriptors = descriptors;
        }
    }
}