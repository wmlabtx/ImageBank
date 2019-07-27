using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ImageBank
{
    public static class HelperSql
    {
        private static readonly object _sqlLock = new object();
        private static readonly SqlConnection  _sqlConnection;

        static HelperSql()
        {
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppConsts.FileDatabase};";
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        public static byte[] GetData(Img img)
        {
            var filename = img.FileName;
            if (!File.Exists(filename))
            {
                return null;
            }

            var data = File.ReadAllBytes(filename);
            if (data == null)
            {
                return null;
            }

            return data;
        }

        public static void SetData(Img img, byte[] data)
        {
            var filename = img.FileName;
            File.WriteAllBytes(filename, data);
        }

        public static void UpdateLink(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrNextName} = @{AppConsts.AttrNextName}, ");
                sb.Append($"{AppConsts.AttrSim} = @{AppConsts.AttrSim}, ");
                sb.Append($"{AppConsts.AttrLastChecked} = @{AppConsts.AttrLastChecked} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextName}", img.NextName);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrSim}", img.Sim);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChecked}", img.LastChecked);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateLastView(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrLastView} = @{AppConsts.AttrLastView} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastView}", img.LastView);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateGeneration(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrGeneration} = @{AppConsts.AttrGeneration} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrGeneration}", img.Generation);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateFolder(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrFolder} = @{AppConsts.AttrFolder} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrFolder}", img.Folder);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /*
        private void SqlUpdateDescriptors(string name, Mat matdescriptors)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrDescriptors} = @{AppConsts.AttrDescriptors} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SQLiteCommand(sqltext, _sqlConnection))
                {
                    var descriptors = HelperDescriptors.ConvertToByteDescriptors(matdescriptors);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDescriptors}", descriptors);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
        */

        public static void AddImg(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("INSERT INTO Images (");
                sb.Append($"{AppConsts.AttrName}, ");
                sb.Append($"{AppConsts.AttrFolder}, ");
                sb.Append($"{AppConsts.AttrGeneration}, ");
                sb.Append($"{AppConsts.AttrLastView}, ");
                sb.Append($"{AppConsts.AttrLastChecked}, ");
                sb.Append($"{AppConsts.AttrDescriptors}, ");
                sb.Append($"{AppConsts.AttrNextName}, ");
                sb.Append($"{AppConsts.AttrSim}");
                sb.Append(") VALUES (");
                sb.Append($"@{AppConsts.AttrName}, ");
                sb.Append($"@{AppConsts.AttrFolder}, ");
                sb.Append($"@{AppConsts.AttrGeneration}, ");
                sb.Append($"@{AppConsts.AttrLastView}, ");
                sb.Append($"@{AppConsts.AttrLastChecked}, ");
                sb.Append($"@{AppConsts.AttrDescriptors}, ");
                sb.Append($"@{AppConsts.AttrNextName}, ");
                sb.Append($"@{AppConsts.AttrSim}");
                sb.Append(")");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrFolder}", img.Folder);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrGeneration}", img.Generation);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastView}", img.LastView);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChecked}", img.LastChecked);
                    var buffer = HelperDescriptors.ConvertToByteArray(img.Descriptors);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDescriptors}", buffer);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextName}", img.NextName);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrSim}", img.Sim);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteImgAndFile(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("DELETE FROM Images WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            var filename = img.FileName;
            if (File.Exists(filename))
            {
                HelperRecycleBin.Delete(filename);
            }
        }

        public static ConcurrentDictionary<string, Img> Load(IProgress<string> progress)
        {
            var _imgList = new ConcurrentDictionary<string, Img>();

            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("SELECT ");
                sb.Append($"{AppConsts.AttrName}, "); // 0
                sb.Append($"{AppConsts.AttrFolder}, "); // 1
                sb.Append($"{AppConsts.AttrGeneration}, "); // 2
                sb.Append($"{AppConsts.AttrLastView}, "); // 3
                sb.Append($"{AppConsts.AttrLastChecked}, "); // 4
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
                            var generation = (int)reader.GetByte(2);
                            var lastview = reader.GetDateTime(3);
                            var lastchecked = reader.GetDateTime(4);
                            var buffer = (byte[])reader[5];
                            var udescriptors = HelperDescriptors.ConvertToDescriptors(buffer);
                            var nextname = reader.GetString(6);
                            var sim = (float)reader.GetDouble(7);

                            var img = new Img(name, folder, generation, lastview, lastchecked, udescriptors, nextname, sim);
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

            return _imgList;
        }
    }
}
