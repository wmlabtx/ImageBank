using System.Text;
using System.Data.SqlClient;
using System;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void SqlUpdateProperty(Img img, string key, object val)
        {
            lock (_sqlLock)
            {
                var sqltext = $"UPDATE {AppConsts.TableImages} SET {key} = @{key} WHERE {AppConsts.AttrHash} = @{AppConsts.AttrHash}";
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{key}", val);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrHash}", img.Hash);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void SqlDelete(string hash)
        {
            lock (_sqlLock)
            {
                using (var sqlCommand = _sqlConnection.CreateCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = $"DELETE FROM {AppConsts.TableImages} WHERE {AppConsts.AttrHash} = @{AppConsts.AttrHash}";
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrHash}", hash);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private void SqlAdd(Img img)
        {
            lock (_sqlLock)
            {
                using (var sqlCommand = _sqlConnection.CreateCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    var sb = new StringBuilder();
                    sb.Append($"INSERT INTO {AppConsts.TableImages} (");
                    sb.Append($"{AppConsts.AttrHash}, ");
                    sb.Append($"{AppConsts.AttrId}, ");
                    sb.Append($"{AppConsts.AttrGeneration}, ");
                    sb.Append($"{AppConsts.AttrLastView}, ");
                    sb.Append($"{AppConsts.AttrNextHash}, ");
                    sb.Append($"{AppConsts.AttrSim}, ");
                    sb.Append($"{AppConsts.AttrLastId}, ");
                    sb.Append($"{AppConsts.AttrLastChange}, ");
                    sb.Append($"{AppConsts.AttrDescriptors}");
                    sb.Append(") VALUES (");
                    sb.Append($"@{AppConsts.AttrHash}, ");
                    sb.Append($"@{AppConsts.AttrId}, ");
                    sb.Append($"@{AppConsts.AttrGeneration}, ");
                    sb.Append($"@{AppConsts.AttrLastView}, ");
                    sb.Append($"@{AppConsts.AttrNextHash}, ");
                    sb.Append($"@{AppConsts.AttrSim}, ");
                    sb.Append($"@{AppConsts.AttrLastId}, ");
                    sb.Append($"@{AppConsts.AttrLastChange}, ");
                    sb.Append($"@{AppConsts.AttrDescriptors}");
                    sb.Append(")");
                    sqlCommand.CommandText = sb.ToString();
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrHash}", img.Hash);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrId}", img.Id);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrGeneration}", img.Generation);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastView}", img.LastView);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextHash}", img.NextHash);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrSim}", img.Sim);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastId}", img.LastId);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChange}", img.LastChange);
                    var buffer = Array.ConvertAll(img.Descriptors, e => (byte)e);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDescriptors}", buffer);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}