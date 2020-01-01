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
                sb.Append($"{AppConsts.AttrFolder}, "); // 1
                sb.Append($"{AppConsts.AttrNextHash}, "); // 2
                sb.Append($"{AppConsts.AttrLastView}, "); // 3
                sb.Append($"{AppConsts.AttrLastCheck}, "); // 4
                sb.Append($"{AppConsts.AttrOrbs} "); // 5
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
                            var hash = reader.GetString(0);
                            var folder = reader.GetString(1);
                            var nexthash = reader.GetString(2);
                            var lastview = reader.GetDateTime(3);
                            var lastcheck = reader.GetDateTime(4);
                            var buffer = (byte[])reader[5];
                            var orbs = HelperConvertors.ConvertBufferToMat(buffer);
                            var img = new Img(
                                hash: hash,
                                folder: folder,
                                nexthash: nexthash,
                                lastview: lastview,
                                lastcheck: lastcheck,
                                orbs: orbs);

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