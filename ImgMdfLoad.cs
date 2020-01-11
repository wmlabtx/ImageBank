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

                var sb = new StringBuilder();
                sb.Append("SELECT ");
                sb.Append($"{AppConsts.AttrHash}, "); // 0
                sb.Append($"{AppConsts.AttrId}, "); // 1
                sb.Append($"{AppConsts.AttrRatio}, "); // 2
                sb.Append($"{AppConsts.AttrGeneration}, "); // 3
                sb.Append($"{AppConsts.AttrStars}, "); // 4
                sb.Append($"{AppConsts.AttrLastView}, "); // 5
                sb.Append($"{AppConsts.AttrNextHash}, "); // 6
                sb.Append($"{AppConsts.AttrSim}, "); // 7
                sb.Append($"{AppConsts.AttrLastId}, "); // 8
                sb.Append($"{AppConsts.AttrLastChange}, "); // 9
                sb.Append($"{AppConsts.AttrDescriptors} "); // 10
                sb.Append($"FROM {AppConsts.TableImages}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var dt = DateTime.Now;
                        progress.Report("Loading...");
                        while (reader.Read())
                        {
                            var hash = reader.GetString(0);
                            var id = reader.GetInt32(1);
                            var ratio = reader.GetFloat(2);
                            var generation = reader.GetInt32(3);
                            var stars = reader.GetInt32(4);
                            var lastview = reader.GetDateTime(5);
                            var nexthash = reader.GetString(6);
                            var sim = reader.GetFloat(7);
                            var lastid = reader.GetInt32(8);
                            var lastchange = reader.GetDateTime(9);
                            var buffer = (byte[])reader[10];
                            var descriptors = HelperDescriptors.To64(buffer);
                            var img = new Img(
                                hash: hash,
                                id: id,
                                ratio: ratio,
                                generation: generation,
                                stars: stars,
                                lastview: lastview,
                                nexthash: nexthash,
                                sim: sim,
                                lastid: lastid,
                                lastchange: lastchange,
                                descriptors: descriptors);

                            _imgList.TryAdd(hash, img);

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