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
                sb.Append($"{AppConsts.AttrLastView}, "); // 2
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 3
                sb.Append($"{AppConsts.AttrDescriptors}, "); // 4
                sb.Append($"{AppConsts.AttrNextName}, "); // 5
                sb.Append($"{AppConsts.AttrSim}, "); // 6
                sb.Append($"{AppConsts.AttrOffset}, "); // 7
                sb.Append($"{AppConsts.AttrLenght}, "); // 8
                sb.Append($"{AppConsts.AttrCrc} "); // 9
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
                            var lastview = reader.GetDateTime(2);
                            var lastchecked = reader.GetDateTime(3);
                            var buffer = (byte[])reader[4];
                            var udescriptors = HelperDescriptors.ConvertToDescriptors(buffer);
                            var nextname = reader.GetString(5);
                            var sim = (float)reader.GetDouble(6);
                            var offset = reader.GetInt64(7);
                            var lenght = reader.GetInt32(8);
                            var crc = reader.GetString(9);

                            var img = new Img(name, node, lastview, lastchecked, udescriptors, nextname, sim, offset, lenght, crc);
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

            /*
            var scope = _imgList.Select(e => e.Value).ToArray();
            foreach (var img in scope)
            {
                if (img.Node.StartsWith("Ls."))
                {
                    img.Node = "Ls";
                    UpdateNode(img);
                }

                if (img.Node.StartsWith("Kandid.Baku.School 177.Ay"))
                {
                    img.Node = "Kandid.Aygun";
                    UpdateNode(img);
                }
                else
                {
                    if (img.Node.StartsWith("Kandid.Baku.Ankara.Po"))
                    {
                        img.Node = "Kandid.Polina";
                        UpdateNode(img);
                    }
                    else
                    {
                        if (img.Node.StartsWith("Kandid.Baku.School 160.Na"))
                        {
                            img.Node = "Kandid.Narmina";
                            UpdateNode(img);
                        }
                        else
                        {
                            if (img.Node.StartsWith("Kandid.Ba") || img.Node.StartsWith("Kandid.Ar") || img.Node.StartsWith("Kandid.Ka"))
                            {
                                img.Node = "Kandid";
                                UpdateNode(img);
                            }
                        }
                    }
                }
            }
            */
        }
    }
}