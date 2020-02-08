using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private static readonly Mutex _sqlLock = new Mutex();
        private static SqlConnection _sqlConnection;

        private readonly ConcurrentDictionary<int, Img> _imgList = new ConcurrentDictionary<int, Img>();
        private readonly ConcurrentDictionary<string, Img> _nameList = new ConcurrentDictionary<string, Img>();
        private readonly ConcurrentDictionary<string, Img> _checksumList = new ConcurrentDictionary<string, Img>();

        private int _id;

        public ImgMdf()
        {
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppConsts.FileDatabase};Connection Timeout=30";
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        private bool GetPairToCompare(out int idX, out int idY)
        {
            idX = -1;
            idY = -1;
            while (true) {
                var scopetoview = _imgList
                    .Values
                    .Where(e => e.LastId >= e.Id && e.GetDescriptors().Length > 0)
                    .ToArray();

                if (scopetoview.Length == 0) {
                    return false;
                }

                var mingeneration = scopetoview.Min(e => e.Generation);
                scopetoview = scopetoview
                    .Where(e => e.Generation == mingeneration)
                    .ToArray();

                long min = long.MaxValue;
                foreach (var img in scopetoview) {
                    if (_imgList.TryGetValue(img.NextId, out var imgY)) {
                        var mint = img.LastView.Ticks + imgY.LastView.Ticks;
                        if (mint < min) {
                            min = mint;
                            idX = img.Id;
                            idY = imgY.Id;
                        }
                    }
                }

                if (idX < 0 || idY < 0) {
                    return false;
                }

                return true;
            }
        }

        public void UpdateGeneration(int id)
        {
            if (_imgList.TryGetValue(id, out var img))
            {
                img.Generation++;
            }
        }

        public void UpdateLastView(int id)
        {
            if (_imgList.TryGetValue(id, out var img)) {
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

        private string GetPrompt()
        {
            var counters = new SortedDictionary<int, int>();
            var scope = _imgList
                .Values
                .Where(e => e.LastId >= e.Id && e.GetDescriptors().Length > 0)
                .ToArray();

            foreach (var img in scope) {
                if (counters.ContainsKey(img.Generation)) {
                    counters[img.Generation]++;
                }
                else {
                    counters.Add(img.Generation, 1);
                }
            }

            var sb = new StringBuilder();
            var countwrong = _imgList.Count(e => e.Value.GetDescriptors().Length == 0);
            if (countwrong > 0) {
                sb.Append($"x:{countwrong}");
            }

            var generations = counters.Keys.ToArray();
            for (var i = generations.Length - 1; i >= 0; i--) {
                if (sb.Length > 0) {
                    sb.Append('/');
                }

                sb.Append($"g{generations[i]}:{counters[generations[i]]}");
            }

            sb.Append($"/{_imgList.Count}");
            sb.Append(": ");
            return sb.ToString();
        }

        private int GetNextToCheck()
        {
            if (_imgList.Count == 0) {
                return -1;
            }

            var scopetocheck = _imgList
                .Values
                .Where(e => e.LastId < e.Id)
                .ToArray();

            if (scopetocheck.Length == 0) {
                return -1;
            }

            var id = scopetocheck.Aggregate((m, e) => e.LastId < m.LastId ? e : m).Id;
            return id;
        }

        private int GetNextToComputeHashes()
        {
            if (_imgList.Count == 0) {
                return -1;
            }

            var scopetocheck = _imgList
                .Values
                .Where(e => e.GetDescriptors().Length == 0)
                .ToArray();

            if (scopetocheck.Length == 0) {
                return -1;
            }

            var id = scopetocheck.Aggregate((m, e) => e.Id < m.Id ? e : m).Id;
            return id;
        }

        private int AllocateId()
        {
            _id++;
            SqlUpdateVar(AppConsts.AttrId, _id);
            return _id;
        }

        private string GetSuggestedLegacyPath()
        {
            var filescounts = new int[99];
            int idlegacy;
            foreach (var img in _imgList) {
                idlegacy = HelperPath.GetIdLegacy(img.Value.Path);
                if (idlegacy >= 0 && idlegacy <= 99) {
                    filescounts[idlegacy]++;
                }
            }

            idlegacy = 0;
            while (
                idlegacy <= 99 &&
                filescounts[idlegacy] >= AppConsts.MaxImages / 100) {
                idlegacy++;
            }

            var path = HelperPath.GetLegacyPath(idlegacy);
            var fullpath = $"{AppConsts.PathCollection}{path}";
            if (!Directory.Exists(fullpath)) {
                Directory.CreateDirectory(fullpath);
            }

            return path;
        }
    }
}