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
                sb.Append($"{AppConsts.AttrLastView}, "); // 2
                sb.Append($"{AppConsts.AttrNextHash}, "); // 3
                sb.Append($"{AppConsts.AttrSim}, "); // 4
                sb.Append($"{AppConsts.AttrLastId}, "); // 5
                sb.Append($"{AppConsts.AttrLastChange}, "); // 6
                sb.Append($"{AppConsts.AttrDescriptors} "); // 7
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
                            var lastview = reader.GetDateTime(2);
                            var nexthash = reader.GetString(3);
                            var sim = reader.GetFloat(4);
                            var lastid = reader.GetInt32(5);
                            var lastchange = reader.GetDateTime(6);
                            var buffer = (byte[])reader[7];
                            var descriptors = HelperDescriptors.To64(buffer);
                            var img = new Img(
                                hash: hash,
                                id: id,
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