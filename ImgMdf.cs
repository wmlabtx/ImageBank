using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private readonly object _sqlLock = new object();
        private readonly SqlConnection _sqlConnection;

        private readonly ConcurrentDictionary<string, Img> _imgList = new ConcurrentDictionary<string, Img>();

        public ImgMdf()
        {
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppConsts.FileDatabase};Connection Timeout=30";
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        public void UpdateView(string hash)
        {
            if (_imgList.TryGetValue(hash, out var img))
            {
                img.LastView = DateTime.Now;
            }
        }

        private DateTime GetMinLastView()
        {
            var min = (_imgList.Count == 0 ?
                DateTime.Now :
                _imgList.Min(e => e.Value.LastView))
                .AddSeconds(-1);
            return min;
        }

        private string GetNextToView()
        {
            if (_imgList.Count == 0)
            {
                return null;
            }

            var toview = _imgList.Values.Count(e => e.LastView < e.LastChange && e.LastId > 0);
            var nexthashes = toview > 0 ?
                _imgList
                    .Values
                    .Where(e => e.LastView < e.LastChange && e.LastId > 0)
                    .OrderByDescending(e => e.Sim)
                    .Select(e => e.Hash)
                    .ToArray() :
                _imgList
                    .Values
                    .Where(e => e.LastId > 0)
                    .OrderBy(e => e.LastView)
                    .Select(e => e.Hash)
                    .ToArray();

                /*
            var nexthashes = 
                _imgList
                    .Values
                    .Where(e => e.LastId > 0)
                    .OrderBy(e => e.LastView)
                    .Select(e => e.Hash)
                    .ToArray();
                    */

            foreach (var nexthash in nexthashes)
            {
                if (_imgList.ContainsKey(nexthash))
                {
                    return nexthash;
                }
            }

            return null;
        }

        private string GetNextToCheck()
        {
            if (_imgList.Count == 0)
            {
                return null;
            }

            var maxid = _imgList.Count == 0 ? 0 : _imgList.Max(e => e.Value.Id);
            var zeroid = _imgList.Count(e => e.Value.LastId == 0);
            var scopetocheck = zeroid > 0 ?
                _imgList
                    .Values
                    .Where(e => e.LastId == 0)
                    .ToArray():
                _imgList
                    .Values
                    .Where(e => e.LastId <= maxid)
                    .ToArray();

            if (scopetocheck.Length == 0)
            {
                return null;
            }

            var index = HelperRandom.Next(scopetocheck.Length);
            var hash = scopetocheck[index].Hash;
            return hash;
        }

        private string GetNextHash(string hash)
        {
            if (!_imgList.TryGetValue(hash, out var img))
            {
                return null;
            }

            return img.NextHash;
        }

        public bool ContainsKey(string hash)
        {
            return _imgList.ContainsKey(hash);
        }
    }
}