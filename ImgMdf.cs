using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private readonly object _sqlLock = new object();
        private readonly SqlConnection _sqlConnection;

        public readonly OrbsFile OrbsFile = new OrbsFile(AppConsts.FileOrbs, AppConsts.MaxOrbsSize);
        private readonly ConcurrentDictionary<string, Img> _imgList = new ConcurrentDictionary<string, Img>();        
        private readonly ConcurrentDictionary<int, object> _availableOrbsSlots = new ConcurrentDictionary<int, object>();

        private readonly Queue<float> _findtimes = new Queue<float>();
        private float _avgtimes = 0f;

        public ImgMdf()
        {
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppConsts.FileDatabase};Connection Timeout=30";
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        public void UpdateView(string name)
        {
            if (_imgList.TryGetValue(name, out var img))
            {
                img.LastView = DateTime.Now;
            }
        }

        private int GetNextId()
        {
            var id = _imgList.Count == 0 ?
                1 :
                _imgList.Max(e => e.Value.Id) + 1;
            return id;
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

        private int GetCountNotView()
        {
            var count = _imgList.Count == 0 ?
                0 :
                _imgList.Values.Count(e => e.Distance < AppVars.DistanceMedian && e.LastView < e.LastChange);
            return count;
        }

        private string GetNextToView()
        {
            if (_imgList.Count == 0)
            {
                return null;
            }

            var scopenotview = _imgList
                .Values
                .Where(e => e.Distance < AppVars.DistanceMedian && e.LastView < e.LastChange)
                .ToArray();

            if (scopenotview.Length > 0)
            {
                var mindistance = scopenotview.Min(e => e.Distance);
                var name = scopenotview.FirstOrDefault(e => e.Distance == mindistance).Name;
                return name;
            }
            else
            {
                var minlastview = _imgList.Min(e => e.Value.LastView);
                var name = _imgList.FirstOrDefault(e => e.Value.LastView.Equals(minlastview)).Key;
                return name;
            }
        }

        private string GetNextToCheck()
        {
            if (_imgList.Count == 0)
            {
                return null;
            }

            var minlastcheck = _imgList.Min(e => e.Value.LastCheck);
            var name = _imgList.FirstOrDefault(e => e.Value.LastCheck.Equals(minlastcheck)).Key;
            return name;
        }

        private float GetDistanceMedian()
        {
            if (_imgList.Count < 100)
            {
                return 0f;
            }

            var distances = _imgList.Select(e => e.Value.Distance).OrderBy(e => e).ToArray();
            var distancemedian = distances[distances.Length / 20];
            return distancemedian;
        }

        private void ResetNextName(Img img)
        {
            img.LastId = 0;
            img.NextName = img.Name;
            img.Distance = 257f;
            img.LastCheck = GetMinLastCheck();
            img.LastChange = DateTime.Now;
            SqlUpdateNameNext(img);
        }

        private void ResetRefers(string name)
        {
            var scope = _imgList
                .Values
                .Where(e => e.NextName.Equals(name))
                .ToList();

            scope.ForEach(e => ResetNextName(e));
        }
    }
}