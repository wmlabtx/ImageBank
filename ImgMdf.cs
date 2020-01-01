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
        private readonly ConcurrentDictionary<string, Flann> _flannList = new ConcurrentDictionary<string, Flann>();

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

        private DateTime GetMinLastCheck()
        {
            var min = (_imgList.Count == 0 ?
                DateTime.Now :
                _imgList.Min(e => e.Value.LastCheck))
                .AddSeconds(-1);
            return min;
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

            var nexthashes = _imgList.OrderBy(e => e.Value.LastView).Select(e => e.Key).ToArray();
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

            var minlastcheck = _imgList.Min(e => e.Value.LastCheck);
            var hash = _imgList.FirstOrDefault(e => e.Value.LastCheck.Equals(minlastcheck)).Key;
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