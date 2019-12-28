using System;
using System.Data.SqlClient;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Load(IProgress<string> progress)
        {
            lock (_sqlLock)
            {
                _imgList.Clear();
                _availableOrbsSlots.Clear();
                for (var slot = 0; slot < 100000; slot++)
                {
                    _availableOrbsSlots.TryAdd(slot, null);
                }

                var sb = new StringBuilder();
                sb.Append("SELECT ");
                sb.Append($"{AppConsts.AttrName}, "); // 0
                sb.Append($"{AppConsts.AttrFolder}, "); // 1
                sb.Append($"{AppConsts.AttrLastView}, "); // 2
                sb.Append($"{AppConsts.AttrLastCheck}, "); // 3
                sb.Append($"{AppConsts.AttrLastChange}, "); // 4
                sb.Append($"{AppConsts.AttrNextName}, "); // 5
                sb.Append($"{AppConsts.AttrDistance}, "); // 6
                sb.Append($"{AppConsts.AttrId}, "); // 7
                sb.Append($"{AppConsts.AttrLastId}, "); // 8
                sb.Append($"{AppConsts.AttrOrbsSlot}, "); // 9
                sb.Append($"{AppConsts.AttrOrbsLength} "); // 10
                sb.Append("FROM Images");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var dt = DateTime.Now;
                        progress.Report("Loading...");
                        while (reader.Read())
                        {
                            var name = reader.GetString(0);
                            var folder = reader.GetString(1);
                            var lastview = reader.GetDateTime(2);
                            var lastcheck = reader.GetDateTime(3);
                            var lastchange = reader.GetDateTime(4);
                            var nextname = reader.GetString(5);
                            var distance = (float)reader.GetDouble(6);
                            var id = reader.GetInt32(7);
                            var lastid = reader.GetInt32(8);
                            var orbsslot = reader.GetInt32(9);
                            var orbslength = reader.GetInt32(10);

                            var img = new Img(
                                name: name,
                                folder: folder,
                                lastview: lastview,
                                lastcheck: lastcheck,
                                lastchange: lastchange,
                                nextname: nextname,
                                distance: distance,
                                id: id,
                                lastid: lastid,
                                orbsslot: orbsslot,
                                orbslength: orbslength);

                            _imgList.TryAdd(name, img);
                            _availableOrbsSlots.TryRemove(orbsslot, out _);

                            if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse)
                            {
                                dt = DateTime.Now;
                                progress.Report($"Loading {_imgList.Count}...");
                            }
                        }

                        progress.Report("Database loaded");
                    }
                }
            }
        }
    }
}