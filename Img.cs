using System;

namespace ImageBank
{
    public class Img
    {
        public int Id { get; }
        public string Name { get; }
        public string Path { get; }
        public string Checksum { get; }

        private int _generation;
        public int Generation
        {
            get => _generation;
            set
            {
                _generation = value;
                ImgMdf.SqlUpdateProperty(this, AppConsts.AttrGeneration, value);
            }
        }

        private int _nextid;
        public int NextId
        {
            get => _nextid;
            set
            {
                _nextid = value;
                ImgMdf.SqlUpdateProperty(this, AppConsts.AttrNextId, value);
            }
        }

        private int _match;
        public int Match
        {
            get => _match;
            set
            {
                _match = value;
                ImgMdf.SqlUpdateProperty(this, AppConsts.AttrMatch, value);
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
                ImgMdf.SqlUpdateProperty(this, AppConsts.AttrLastId, value);
            }
        }

        private DateTime _lastview;
        public DateTime LastView
        {
            get => _lastview;
            set
            {
                _lastview = value;
                ImgMdf.SqlUpdateProperty(this, AppConsts.AttrLastView, value);
            }
        }

        private DateTime _lastchange;
        public DateTime LastChange
        {
            get => _lastchange;
            set
            {
                _lastchange = value;
                ImgMdf.SqlUpdateProperty(this, AppConsts.AttrLastChange, value);
            }
        }

        private uint[] _descriptors;

        public uint[] GetDescriptors()
        {
            return _descriptors;
        }

        public void SetDescriptors(uint[] array)
        {
            _descriptors = array;
            var buffer = HelperConvertors.ConvertToBuffer(_descriptors);
            ImgMdf.SqlUpdateProperty(this, AppConsts.AttrDescriptors, buffer);
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
                var filename = $"{Directory}{Name}{AppConsts.JpgExtension}";
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
            uint[] descriptors)
        {
            Id = id;
            Name = name;
            Path = path;
            Checksum = checksum;
            _generation = generation;
            _lastview = lastview;
            _nextid = nextid;
            _match = match;
            _lastid = lastid;
            _lastchange = lastchange;
            _descriptors = descriptors;
        }
    }
}