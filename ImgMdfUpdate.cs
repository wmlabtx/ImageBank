using System.Data.SqlClient;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void UpdateNameNext(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrLastId} = @{AppConsts.AttrLastId}, ");
                sb.Append($"{AppConsts.AttrNextName} = @{AppConsts.AttrNextName}, ");
                sb.Append($"{AppConsts.AttrDistance} = @{AppConsts.AttrDistance}, ");
                sb.Append($"{AppConsts.AttrLastChecked} = @{AppConsts.AttrLastChecked}, ");
                sb.Append($"{AppConsts.AttrLastChanged} = @{AppConsts.AttrLastChanged} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastId}", img.LastId);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextName}", img.NextName);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDistance}", img.Distance);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChecked}", img.LastChecked);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChanged}", img.LastChanged);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProperty(Img img, string key, object val)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{key} = @{key} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{key}", val);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
