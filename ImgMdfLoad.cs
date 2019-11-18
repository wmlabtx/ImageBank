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
                sb.Append($"{AppConsts.AttrPerson}, "); // 1
                sb.Append($"{AppConsts.AttrPHash}, "); // 2
                sb.Append($"{AppConsts.AttrDistance}, "); // 3
                sb.Append($"{AppConsts.AttrLastView}, "); // 4
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 5
                sb.Append($"{AppConsts.AttrLastChanged}, "); // 6
                sb.Append($"{AppConsts.AttrNextName}, "); // 7
                sb.Append($"{AppConsts.AttrOffset}, "); // 8
                sb.Append($"{AppConsts.AttrLenght}, "); // 9
                sb.Append($"{AppConsts.AttrCrc}, "); // 10
                sb.Append($"{AppConsts.AttrOrbs}, "); // 11
                sb.Append($"{AppConsts.AttrSim}, "); // 12
                sb.Append($"{AppConsts.AttrId}, "); // 13
                sb.Append($"{AppConsts.AttrLastId} "); // 14
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
                            var person = reader.GetString(1);
                            var buffer = (byte[])reader[2];
                            var phash = HelperDescriptors.ConvertBufferToPHash(buffer);
                            var distance = reader.GetInt32(3);
                            var lastview = reader.GetDateTime(4);
                            var lastchecked = reader.GetDateTime(5);
                            var lastchanged = reader.GetDateTime(6);
                            var nextname = reader.GetString(7);
                            var offset = reader.GetInt64(8);
                            var lenght = reader.GetInt32(9);
                            var crc = reader.GetString(10);
                            buffer = (byte[])reader[11];
                            var orbs = HelperDescriptors.ConvertBufferToMat(buffer);
                            var sim = (float)reader.GetDouble(12);
                            var id = reader.GetInt32(13);
                            var lastid = reader.GetInt32(14);
                            var img = new Img(name, person, phash, distance, lastview, lastchecked, lastchanged, nextname, offset, lenght, crc, orbs, sim, id, lastid);
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