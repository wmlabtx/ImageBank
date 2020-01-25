using System;

namespace ImageBank
{
    public class Img
    {
        public string Hash { get; }
        public int Id { get; }

        private int _generation;
        public int Generation
        {
            get => _generation;
            set
            {
                _generation = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrGeneration, value);
            }
        }

        private string _nexthash;
        public string NextHash
        {
            get => _nexthash;
            set
            {
                _nexthash = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrNextHash, value);
            }
        }

        private float _sim;
        public float Sim
        {
            get => _sim;
            set
            {
                _sim = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrSim, value);
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
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrLastId, value);
            }
        }

        private DateTime _lastview;
        public DateTime LastView
        {
            get => _lastview;
            set
            {
                _lastview = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrLastView, value);
            }
        }

        private DateTime _lastchange;
        public DateTime LastChange
        {
            get => _lastchange;
            set
            {
                _lastchange = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrLastChange, value);
            }
        }

        public uint[] Descriptors { get; }

        public string Subdirectory
        {
            get
            {
                var hash = Math.Abs(Hash.GetHashCode()) % 100;
                var subdirectory = $"{hash:D2}";
                return subdirectory;
            }
        }

        public string Directory
        {
            get
            {
                var directory = $"{AppConsts.PathCollection}{Subdirectory}\\";
                return directory;
            }
        }

        public string File
        {
            get
            {
                var filename = $"{Directory}{Hash}{AppConsts.DatExtension}";
                return filename;
            }
        }

        public Img(
            string hash,
            int id,
            int generation,
            DateTime lastview,
            string nexthash,
            float sim,
            int lastid,
            DateTime lastchange,
            uint[] descriptors)
        {
            Hash = hash;
            Id = id;
            _generation = generation;
            _lastview = lastview;
            _nexthash = nexthash;
            _sim = sim;
            _lastid = lastid;
            _lastchange = lastchange;
            Descriptors = descriptors;
        }
    }
}