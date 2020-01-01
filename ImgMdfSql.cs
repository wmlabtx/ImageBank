using System.Text;
using System.Data.SqlClient;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void SqlUpdateProperty(Img img, string key, object val)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{key} = @{key} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrHash} = @{AppConsts.AttrHash}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{key}", val);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrHash}", img.Hash);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void SqlDelete(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("DELETE FROM Images WHERE ");
                sb.Append($"{AppConsts.AttrHash} = @{AppConsts.AttrHash}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrHash}", img.Hash);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private void SqlAdd(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("INSERT INTO Images (");
                sb.Append($"{AppConsts.AttrHash}, ");
                sb.Append($"{AppConsts.AttrFolder}, ");
                sb.Append($"{AppConsts.AttrNextHash}, ");
                sb.Append($"{AppConsts.AttrLastView}, ");
                sb.Append($"{AppConsts.AttrLastCheck}, ");
                sb.Append($"{AppConsts.AttrOrbs}");
                sb.Append(") VALUES (");
                sb.Append($"@{AppConsts.AttrHash}, ");
                sb.Append($"@{AppConsts.AttrFolder}, ");
                sb.Append($"@{AppConsts.AttrNextHash}, ");
                sb.Append($"@{AppConsts.AttrLastView}, ");
                sb.Append($"@{AppConsts.AttrLastCheck}, ");
                sb.Append($"@{AppConsts.AttrOrbs}");
                sb.Append(")");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrHash}", img.Hash);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrFolder}", img.Folder);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextHash}", img.NextHash);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastView}", img.LastView);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastCheck}", img.LastCheck);
                    var buffer = HelperConvertors.ConvertMatToBuffer(img.Orbs);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrOrbs}", buffer);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
