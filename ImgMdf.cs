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

        private bool GetPairToCompare(ref string hashX, out string hashY)
        {
            hashY = null;
            if (string.IsNullOrEmpty(hashX))
            {
                var scopeX = _imgList
                    .Values
                    .Where(e => e.LastId > 0)
                    .ToArray();

                if (scopeX.Length == 0)
                {
                    return false;
                }

                var mingeneration = scopeX.Min(e => e.Generation);
                scopeX = scopeX
                    .Where(e => e.Generation == mingeneration)
                    .ToArray();

                var scopeFresh = scopeX
                    .Where(e => e.LastView < e.LastChange)
                    .ToArray();

                if (scopeFresh.Length > 0)
                {
                    scopeX = scopeFresh;
                }

                double deltaX = 0;
                foreach (var imgA in scopeX)
                {
                    if (imgA.Hash.Equals(imgA.NextHash) || !_imgList.ContainsKey(imgA.NextHash))
                    {
                        continue;
                    }

                    if (!_imgList.TryGetValue(imgA.NextHash, out var imgB))
                    {
                        return false;
                    }

                    var delta = 
                        Math.Pow(DateTime.Now.Subtract(imgA.LastView).TotalMinutes, 2) + 
                        Math.Pow(DateTime.Now.Subtract(imgB.LastView).TotalMinutes, 2);

                    if (delta > deltaX)
                    {
                        deltaX = delta;
                        hashX = imgA.Hash;
                    }
                }

                if (deltaX < 0.01)
                {
                    return false;
                }
            }

            if (!_imgList.TryGetValue(hashX, out var imgX))
            {
                return false;
            }

            hashY = imgX.NextHash;
            if (!_imgList.ContainsKey(hashY))
            {
                return false;
            }

            return true; 
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

        private string GetNextToCheck()
        {
            if (_imgList.Count == 0)
            {
                return null;
            }

            var maxid = _imgList.Max(e => e.Value.Id);
            var scopetocheck= _imgList
                    .Values
                    .Where(e => e.LastId <= maxid)
                    .ToArray();

            if (scopetocheck.Length == 0)
            {
                return null;
            }

            var minlastid = scopetocheck.Min(e => e.LastId);
            var hash = scopetocheck
                .FirstOrDefault(e => e.LastId == minlastid)
                .Hash;

            return hash;
        }
    }
}