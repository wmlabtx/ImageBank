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
                sb.Append($"{AppConsts.AttrGeneration}, "); // 2
                sb.Append($"{AppConsts.AttrLastView}, "); // 3
                sb.Append($"{AppConsts.AttrNextHash}, "); // 4
                sb.Append($"{AppConsts.AttrSim}, "); // 5
                sb.Append($"{AppConsts.AttrLastId}, "); // 6
                sb.Append($"{AppConsts.AttrLastChange}, "); // 7
                sb.Append($"{AppConsts.AttrDescriptors} "); // 8
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
                            var generation = reader.GetInt32(2);
                            var lastview = reader.GetDateTime(3);
                            var nexthash = reader.GetString(4);
                            var sim = reader.GetFloat(5);
                            var lastid = reader.GetInt32(6);
                            var lastchange = reader.GetDateTime(7);
                            var buffer = (byte[])reader[8];
                            var descriptors = Array.ConvertAll(buffer, e => (int)e);
                            var img = new Img(
                                hash: hash,
                                id: id,
                                generation: generation,
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