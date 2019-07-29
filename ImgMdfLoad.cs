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
                var sb = new StringBuilder();
                sb.Append("SELECT ");
                sb.Append($"{AppConsts.AttrName}, "); // 0
                sb.Append($"{AppConsts.AttrNode}, "); // 1
                sb.Append($"{AppConsts.AttrGeneration}, "); // 2
                sb.Append($"{AppConsts.AttrLastView}, "); // 3
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 4
                sb.Append($"{AppConsts.AttrDescriptors}, "); // 5
                sb.Append($"{AppConsts.AttrNextName}, "); // 6
                sb.Append($"{AppConsts.AttrSim}, "); // 7
                sb.Append($"{AppConsts.AttrOffset}, "); // 8
                sb.Append($"{AppConsts.AttrLenght}, "); // 9
                sb.Append($"{AppConsts.AttrCrc} "); // 10
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
                            var node = reader.GetString(1);
                            var generation = reader.GetInt32(2);
                            var lastview = reader.GetDateTime(3);
                            var lastchecked = reader.GetDateTime(4);
                            var buffer = (byte[])reader[5];
                            var udescriptors = HelperDescriptors.ConvertToDescriptors(buffer);
                            var nextname = reader.GetString(6);
                            var sim = (float)reader.GetDouble(7);
                            var offset = reader.GetInt64(8);
                            var lenght = reader.GetInt32(9);
                            var crc = reader.GetString(10);

                            var img = new Img(name, node, generation, lastview, lastchecked, udescriptors, nextname, sim, offset, lenght, crc);
                            _imgList.TryAdd(name, img);

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