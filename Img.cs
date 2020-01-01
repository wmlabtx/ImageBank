using OpenCvSharp;
using System;

namespace ImageBank
{
    public class Img
    {
        public string Hash { get; }

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
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrFolder, value);
            }
        }

        private string _nexthash;
        public string NextHash
        {
            get
            {
                return _nexthash;
            }
            set
            {
                _nexthash = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrNextHash, value);
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
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrLastView, value);
            }
        }

        private DateTime _lastcheck;
        public DateTime LastCheck
        {
            get
            {
                return _lastcheck;
            }
            set
            {
                _lastcheck = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrLastCheck, value);
            }
        }

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
                var buffer = HelperConvertors.ConvertMatToBuffer(_orbs);
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrOrbs, buffer);
            }
        }

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
            string folder,
            string nexthash,
            DateTime lastview,
            DateTime lastcheck,
            Mat orbs)
        {
            Hash = hash;
            _folder = folder;
            _nexthash = nexthash;
            _lastview = lastview;
            _lastcheck = lastcheck;
            _orbs = orbs; 
        }
    }
}