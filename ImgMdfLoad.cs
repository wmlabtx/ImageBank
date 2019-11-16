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
                            var person = reader.GetString(1);
                            var buffer = (byte[])reader[2];
                            var phash = HelperDescriptors.ConvertToPHash(buffer);
                            var distance = reader.GetInt32(3);
                            var lastview = reader.GetDateTime(4);
                            var lastchecked = reader.GetDateTime(5);
                            var lastchanged = reader.GetDateTime(6);
                            var nextname = reader.GetString(7);
                            var offset = reader.GetInt64(8);
                            var lenght = reader.GetInt32(9);
                            var crc = reader.GetString(10);

                            var img = new Img(name, person, phash, distance, lastview, lastchecked, lastchanged, nextname, offset, lenght, crc);
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