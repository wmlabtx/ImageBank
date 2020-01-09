﻿using System;

namespace ImageBank
{
    public class Img
    {
        public string Hash { get; }
        public int Id { get; }

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

        private float _sim;
        public float Sim
        {
            get
            {
                return _sim;
            }
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

        private DateTime _lastchange;
        public DateTime LastChange
        {
            get
            {
                return _lastchange;
            }
            set
            {
                _lastchange = value;
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrLastChange, value);
            }
        }

        private ulong[] _descriptors;

        public ulong[] Descriptors
        {
            get
            {
                return _descriptors;
            }
            set
            {
                _descriptors = value;
                var buffer = HelperDescriptors.From64(value);
                AppVars.Collection.SqlUpdateProperty(this, AppConsts.AttrDescriptors, value);
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
            int id,
            DateTime lastview,
            string nexthash,
            float sim,
            int lastid,
            DateTime lastchange,
            ulong[] descriptors)
        {
            Hash = hash;
            Id = id;
            _lastview = lastview;
            _nexthash = nexthash;
            _sim = sim;
            _lastid = lastid;
            _lastchange = lastchange;
            _descriptors = descriptors;
        }
    }
}