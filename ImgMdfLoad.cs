using System;
using System.Data.SqlClient;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public void Load(IProgress<string> progress)
        {
            _sqlLock.WaitOne();
            _imgList.Clear();
            _checksumList.Clear();
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append($"{AppConsts.AttrId}, "); // 0
            sb.Append($"{AppConsts.AttrName}, "); // 1
            sb.Append($"{AppConsts.AttrPath}, "); // 2
            sb.Append($"{AppConsts.AttrChecksum}, "); // 3
            sb.Append($"{AppConsts.AttrGeneration}, "); // 4
            sb.Append($"{AppConsts.AttrLastView}, "); // 5
            sb.Append($"{AppConsts.AttrNextId}, "); // 6
            sb.Append($"{AppConsts.AttrMatch}, "); // 7
            sb.Append($"{AppConsts.AttrLastId}, "); // 8
            sb.Append($"{AppConsts.AttrLastChange}, "); // 9
            sb.Append($"{AppConsts.AttrDescriptors} "); // 10
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
                        var id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var path = reader.GetString(2);
                        var checksum = reader.GetString(3);
                        var generation = reader.GetInt32(4);
                        var lastview = reader.GetDateTime(5);
                        var nextid = reader.GetInt32(6);
                        var match = reader.GetInt32(7);
                        var lastid = reader.GetInt32(8);
                        var lastchange = reader.GetDateTime(9);
                        var buffer = (byte[])reader[10];
                        var descriptors = new uint[buffer.Length / sizeof(uint)];
                        Buffer.BlockCopy(buffer, 0, descriptors, 0, buffer.Length);
                        var img = new Img(
                            id: id,
                            name: name,
                            path: path,
                            checksum: checksum,
                            generation: generation,
                            lastview: lastview,
                            nextid: nextid,
                            match: match,
                            lastid: lastid,
                            lastchange: lastchange,
                            descriptors: descriptors);

                        _imgList.TryAdd(id, img);
                        _nameList.TryAdd(name, img);
                        _checksumList.TryAdd(checksum, img);

                        if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse)
                        {
                            dt = DateTime.Now;
                            progress.Report($"Loading {_imgList.Count}...");
                        }
                    }
                }
            }

            progress.Report("Loading vars...");

            _id = 0;

            sb.Length = 0;
            sb.Append("SELECT ");
            sb.Append($"{AppConsts.AttrId} "); // 0
            sb.Append($"FROM {AppConsts.TableVars}");
            sqltext = sb.ToString();
            using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection)) {
                using (var reader = sqlCommand.ExecuteReader()) {
                    while (reader.Read()) {
                        _id = reader.GetInt32(0);
                        break;
                    }
                }
            }

            _sqlLock.ReleaseMutex();
            progress.Report("Database loaded");
        }
    }
}