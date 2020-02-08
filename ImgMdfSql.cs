using System.Text;
using System.Data.SqlClient;

namespace ImageBank
{
    public partial class ImgMdf
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static void SqlUpdateProperty(Img img, string key, object val)
        {
            _sqlLock.WaitOne();
            var sqltext = $"UPDATE {AppConsts.TableImages} SET {key} = @{key} WHERE {AppConsts.AttrId} = @{AppConsts.AttrId}";
            using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue($"@{key}", val);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrId}", img.Id);
                sqlCommand.ExecuteNonQuery();
            }

            _sqlLock.ReleaseMutex();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static void SqlUpdateVar(string key, object val)
        {
            _sqlLock.WaitOne();
            var sqltext = $"UPDATE {AppConsts.TableVars} SET {key} = @{key}";
            using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue($"@{key}", val);
                sqlCommand.ExecuteNonQuery();
            }

            _sqlLock.ReleaseMutex();
        }

        private static void SqlDelete(int id)
        {
            _sqlLock.WaitOne();
            using (var sqlCommand = _sqlConnection.CreateCommand())
            {
                sqlCommand.Connection = _sqlConnection;
                sqlCommand.CommandText = $"DELETE FROM {AppConsts.TableImages} WHERE {AppConsts.AttrId} = @{AppConsts.AttrId}";
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrId}", id);
                sqlCommand.ExecuteNonQuery();
            }

            _sqlLock.ReleaseMutex();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        private static void SqlAdd(Img img)
        {
            _sqlLock.WaitOne();
            using (var sqlCommand = _sqlConnection.CreateCommand())
            {
                sqlCommand.Connection = _sqlConnection;
                var sb = new StringBuilder();
                sb.Append($"INSERT INTO {AppConsts.TableImages} (");
                sb.Append($"{AppConsts.AttrId}, ");
                sb.Append($"{AppConsts.AttrName}, ");
                sb.Append($"{AppConsts.AttrPath}, ");
                sb.Append($"{AppConsts.AttrChecksum}, ");
                sb.Append($"{AppConsts.AttrGeneration}, ");
                sb.Append($"{AppConsts.AttrNextId}, ");
                sb.Append($"{AppConsts.AttrMatch}, ");
                sb.Append($"{AppConsts.AttrLastId}, ");
                sb.Append($"{AppConsts.AttrLastView}, ");
                sb.Append($"{AppConsts.AttrLastChange}, ");
                sb.Append($"{AppConsts.AttrDescriptors}");
                sb.Append(") VALUES (");
                sb.Append($"@{AppConsts.AttrId}, ");
                sb.Append($"@{AppConsts.AttrName}, ");
                sb.Append($"@{AppConsts.AttrPath}, ");
                sb.Append($"@{AppConsts.AttrChecksum}, ");
                sb.Append($"@{AppConsts.AttrGeneration}, ");
                sb.Append($"@{AppConsts.AttrNextId}, ");
                sb.Append($"@{AppConsts.AttrMatch}, ");
                sb.Append($"@{AppConsts.AttrLastId}, ");
                sb.Append($"@{AppConsts.AttrLastView}, ");
                sb.Append($"@{AppConsts.AttrLastChange}, ");
                sb.Append($"@{AppConsts.AttrDescriptors}");
                sb.Append(")");
                sqlCommand.CommandText = sb.ToString();
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrId}", img.Id);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrPath}", img.Path);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrChecksum}", img.Checksum);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrGeneration}", img.Generation);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextId}", img.NextId);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrMatch}", img.Match);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastId}", img.LastId);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastView}", img.LastView);
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChange}", img.LastChange);
                var buffer = HelperConvertors.ConvertToBuffer(img.GetDescriptors());
                sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDescriptors}", buffer);
                sqlCommand.ExecuteNonQuery();
            }

            _sqlLock.ReleaseMutex();
        }
    }
}