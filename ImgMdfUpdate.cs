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
                sb.Append($"{AppConsts.AttrNextName} = @{AppConsts.AttrNextName}, ");
                sb.Append($"{AppConsts.AttrDistance} = @{AppConsts.AttrDistance}, ");
                sb.Append($"{AppConsts.AttrLastChecked} = @{AppConsts.AttrLastChecked}, ");
                sb.Append($"{AppConsts.AttrLastChanged} = @{AppConsts.AttrLastChanged} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextName}", img.NextName);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDistance}", img.Distance);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChecked}", img.LastChecked);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChanged}", img.LastChanged);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void UpdateLastView(Img img)
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

        private void UpdatePHash(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrPHash} = @{AppConsts.AttrPHash} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    var buffer = HelperDescriptors.ConvertToBuffer(img.PHash);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrPHash}", buffer);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePerson(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrPerson} = @{AppConsts.AttrPerson} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrPerson}", img.Person);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
