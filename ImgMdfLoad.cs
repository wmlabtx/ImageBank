﻿using System;
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
                sb.Append($"{AppConsts.AttrStars}, "); // 1
                sb.Append($"{AppConsts.AttrLastView}, "); // 2
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 3
                sb.Append($"{AppConsts.AttrLastChanged}, "); // 4
                sb.Append($"{AppConsts.AttrDescriptors}, "); // 5
                sb.Append($"{AppConsts.AttrNextName}, "); // 6
                sb.Append($"{AppConsts.AttrSim}, "); // 7
                sb.Append($"{AppConsts.AttrOffset}, "); // 8
                sb.Append($"{AppConsts.AttrLenght}, "); // 9
                sb.Append($"{AppConsts.AttrCrc}, "); // 10
                sb.Append($"{AppConsts.AttrNode} "); // 11
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
                            var stars = reader.GetInt32(1);
                            var lastview = reader.GetDateTime(2);
                            var lastchecked = reader.GetDateTime(3);
                            var lastchanged = reader.GetDateTime(4);
                            var buffer = (byte[])reader[5];
                            var matdescriptors = HelperDescriptors.ConvertToMatDescriptors(buffer);
                            var nextname = reader.GetString(6);
                            var sim = (float)reader.GetDouble(7);
                            var offset = reader.GetInt64(8);
                            var lenght = reader.GetInt32(9);
                            var crc = reader.GetString(10);
                            var node = reader.GetString(11);

                            var img = new Img(name, stars, lastview, lastchecked, lastchanged, matdescriptors, nextname, sim, offset, lenght, crc, node);
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