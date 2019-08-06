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
                sb.Append($"{AppConsts.AttrGen} = @{AppConsts.AttrGen}, ");
                sb.Append($"{AppConsts.AttrNextName} = @{AppConsts.AttrNextName}, ");
                sb.Append($"{AppConsts.AttrSim} = @{AppConsts.AttrSim}, ");
                sb.Append($"{AppConsts.AttrLastChecked} = @{AppConsts.AttrLastChecked} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrGen}", img.Gen);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextName}", img.NextName);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrSim}", img.Sim);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChecked}", img.LastChecked);
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

        public void UpdateNode(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrNode} = @{AppConsts.AttrNode} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNode}", img.Node);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /*
        public void UpdateGen(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrGen} = @{AppConsts.AttrGen} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrGen}", img.Gen);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
        */

        private void UpdateDescriptors(Img img)
        {
            lock (_sqlLock)
            {
                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrDescriptors} = @{AppConsts.AttrDescriptors} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    var buffer = HelperDescriptors.ConvertToByteArray(img.Descriptors);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDescriptors}", buffer);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
