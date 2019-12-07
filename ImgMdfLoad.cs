using System;
using System.Data.SqlClient;
using System.Linq;
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
                sb.Append($"{AppConsts.AttrLastView}, "); // 1
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 2
                sb.Append($"{AppConsts.AttrLastChanged}, "); // 3
                sb.Append($"{AppConsts.AttrNextName}, "); // 4
                sb.Append($"{AppConsts.AttrOrbs}, "); // 5
                sb.Append($"{AppConsts.AttrSim}, "); // 6
                sb.Append($"{AppConsts.AttrId}, "); // 7
                sb.Append($"{AppConsts.AttrLastId} "); // 8
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
                            var lastview = reader.GetDateTime(1);
                            var lastchecked = reader.GetDateTime(2);
                            var lastchanged = reader.GetDateTime(3);
                            var nextname = reader.GetString(4);
                            var buffer = (byte[])reader[5];
                            var orbs = HelperDescriptors.ConvertBufferToMat(buffer);
                            var sim = (float)reader.GetDouble(6);
                            var id = reader.GetInt32(7);
                            var lastid = reader.GetInt32(8);
                            var img = new Img(name, lastview, lastchecked, lastchanged, nextname, orbs, sim, id, lastid);
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