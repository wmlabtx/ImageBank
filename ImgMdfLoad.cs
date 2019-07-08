using OpenCvSharp;
using System;
using System.Data.SqlClient;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Load(IProgress<string> progress)
        {
            _imgList.Clear();
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("SELECT ");
                sb.Append($"{AppConsts.AttrName}, "); // 0
                sb.Append($"{AppConsts.AttrFolder}, "); // 1
                sb.Append($"{AppConsts.AttrLastView}, "); // 2
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 3
                sb.Append($"{AppConsts.AttrLastUpdated}, "); // 4
                sb.Append($"{AppConsts.AttrDescriptors}, "); // 5
                sb.Append($"{AppConsts.AttrNextName}, "); // 6
                sb.Append($"{AppConsts.AttrSim} "); // 7
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
                            var lastchecked = reader.GetDateTime(3);
                            var lastupdated = reader.GetDateTime(4);
                            var bytedescriptors = (byte[]) reader[5];
                            Mat matdescriptors = HelperDescriptors.ConvertToMatDescriptors(bytedescriptors);
                            var nextname = reader.GetString(6);
                            var sim = (float)reader.GetDouble(7);

                            var img = new Img(name, folder, lastview, lastchecked, lastupdated, matdescriptors, nextname, sim);
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