using System.Data.SqlClient;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Add(Img img)
        {
            lock (_sqlLock)
            {
                if (!_imgList.TryAdd(img.Name, img))
                {
                    return;
                }

                var sb = new StringBuilder();
                sb.Append("INSERT INTO Images (");
                sb.Append($"{AppConsts.AttrName}, ");
                sb.Append($"{AppConsts.AttrLastView}, ");
                sb.Append($"{AppConsts.AttrLastChecked}, ");
                sb.Append($"{AppConsts.AttrLastChanged}, ");
                sb.Append($"{AppConsts.AttrNextName}, ");
                sb.Append($"{AppConsts.AttrOrbs}, ");
                sb.Append($"{AppConsts.AttrSim}, "); 
                sb.Append($"{AppConsts.AttrId}, "); 
                sb.Append($"{AppConsts.AttrLastId}");
                sb.Append(") VALUES (");
                sb.Append($"@{AppConsts.AttrName}, ");
                sb.Append($"@{AppConsts.AttrLastView}, ");
                sb.Append($"@{AppConsts.AttrLastChecked}, ");
                sb.Append($"@{AppConsts.AttrLastChanged}, ");
                sb.Append($"@{AppConsts.AttrNextName}, ");
                sb.Append($"@{AppConsts.AttrOrbs}, ");
                sb.Append($"@{AppConsts.AttrSim}, ");
                sb.Append($"@{AppConsts.AttrId}, ");
                sb.Append($"@{AppConsts.AttrLastId}");
                sb.Append(")");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastView}", img.LastView);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChecked}", img.LastChecked);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChanged}", img.LastChanged);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextName}", img.NextName);
                    var buffer = HelperDescriptors.ConvertMatToBuffer(img.Orbs);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrOrbs}", buffer);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrSim}", img.Sim);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrId}", img.Id);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastId}", img.LastId);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
