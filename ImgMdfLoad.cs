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
                sb.Append($"{AppConsts.AttrId}, "); // 1
                sb.Append($"{AppConsts.AttrLastId}, "); // 2
                sb.Append($"{AppConsts.AttrLastView}, "); // 3
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 4
                sb.Append($"{AppConsts.AttrLastChanged}, "); // 5
                sb.Append($"{AppConsts.AttrNextName}, "); // 6
                sb.Append($"{AppConsts.AttrDescriptors}, "); // 7
                sb.Append($"{AppConsts.AttrSim} "); // 8
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
                            var id = reader.GetInt32(1);
                            var lastid = reader.GetInt32(2);
                            var lastview = reader.GetDateTime(3);
                            var lastchecked = reader.GetDateTime(4);
                            var lastchanged = reader.GetDateTime(5);
                            var nextname = reader.GetString(6);                            
                            var buffer = (byte[])reader[7];
                            var descriptors = HelperConvertors.ConvertBufferToMat(buffer);
                            var sim = (float)reader.GetDouble(8);                            
                            var img = new Img(name, id, lastid, lastview, lastchecked, lastchanged, nextname, descriptors, sim);
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