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

        private int _id;

        public ImgMdf()
        {
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppConsts.FileDatabase};Connection Timeout=30";
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        private bool GetPairToCompare(ref string hashX, out string hashY)
        {
            hashY = null;
            while (true)
            {
                if (string.IsNullOrEmpty(hashX)) {
                    var scopetoview = _imgList
                        .Values
                        .Where(e => e.LastId >= 0)
                        .ToArray();

                    if (scopetoview.Length == 0) {
                        return false;
                    }

                    var mingeneration = scopetoview.Min(e => e.Generation);
                    scopetoview = scopetoview
                        .Where(e => e.Generation == mingeneration)
                        .ToArray();

                    var scopechanged = scopetoview
                        .Where(e => e.LastView < e.LastChange)
                        .ToArray();

                    if (scopechanged.Length > 0) {
                        scopetoview = scopechanged;
                    }

                    var minlastview = DateTime.MaxValue;
                    foreach (var img in scopetoview) {
                        if (_imgList.TryGetValue(img.NextHash, out var imgY)) {
                            if (imgY.LastView < minlastview) {
                                minlastview = imgY.LastView;
                                hashX = img.Hash;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(hashX)) {
                    return false;
                }
                
                if (!_imgList.TryGetValue(hashX, out var imgX))
                {
                    Delete(hashX);
                    hashX = null;
                    continue;
                }

                hashY = imgX.NextHash;
                if (!_imgList.ContainsKey(hashY))
                {
                    imgX.LastId = 0;
                    hashX = null;
                    continue;
                }

                return true;
            }
        }

        public void UpdateGeneration(string hash)
        {
            if (_imgList.TryGetValue(hash, out var img))
            {
                img.Generation++;
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

        public int GetMaxId()
        {
            var maxid = _imgList.Count == 0 ? 0 : _imgList.Max(e => e.Value.Id);
            return maxid;
        }

        private int GetFreshCount()
        {
            var freshcount = 0;
            var scopetoview = _imgList
                .Values
                .Where(e => e.LastId >= 0)
                .ToArray();

            if (scopetoview.Length > 0) {
                var mingeneration = scopetoview.Min(e => e.Generation);
                scopetoview = scopetoview
                    .Where(e => e.Generation == mingeneration)
                    .ToArray();

                freshcount = scopetoview
                    .Count(e => e.LastView < e.LastChange);
            }

            return freshcount;
        }

        private string GetNextToCheck()
        {
            if (_imgList.Count == 0) {
                return null;
            }

            var scopetocheck = _imgList
                .Values
                .Where(e => e.LastId < e.Id)
                .ToArray();

            if (scopetocheck.Length == 0) {
                return null;
            }

            var hash = scopetocheck.Aggregate((m, e) => e.LastId < m.LastId ? e : m).Hash;
            return hash;
        }

        private int AllocateId()
        {
            _id++;
            SqlUpdateVar(AppConsts.AttrId, _id);
            return _id;
        }
    }
}