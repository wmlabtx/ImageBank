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
                sb.Append($"{AppConsts.AttrNode}, ");
                sb.Append($"{AppConsts.AttrGeneration}, ");
                sb.Append($"{AppConsts.AttrLastView}, ");
                sb.Append($"{AppConsts.AttrLastChecked}, ");
                sb.Append($"{AppConsts.AttrDescriptors}, ");
                sb.Append($"{AppConsts.AttrNextName}, ");
                sb.Append($"{AppConsts.AttrSim}, ");
                sb.Append($"{AppConsts.AttrOffset}, ");
                sb.Append($"{AppConsts.AttrLenght}, ");
                sb.Append($"{AppConsts.AttrCrc}");
                sb.Append(") VALUES (");
                sb.Append($"@{AppConsts.AttrName}, ");
                sb.Append($"@{AppConsts.AttrNode}, ");
                sb.Append($"@{AppConsts.AttrGeneration}, ");
                sb.Append($"@{AppConsts.AttrLastView}, ");
                sb.Append($"@{AppConsts.AttrLastChecked}, ");
                sb.Append($"@{AppConsts.AttrDescriptors}, ");
                sb.Append($"@{AppConsts.AttrNextName}, ");
                sb.Append($"@{AppConsts.AttrSim}, ");
                sb.Append($"@{AppConsts.AttrOffset}, ");
                sb.Append($"@{AppConsts.AttrLenght}, ");
                sb.Append($"@{AppConsts.AttrCrc}");
                sb.Append(")");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNode}", img.Node);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrGeneration}", img.Generation);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastView}", img.LastView);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLastChecked}", img.LastChecked);
                    var buffer = HelperDescriptors.ConvertToByteArray(img.Descriptors);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrDescriptors}", buffer);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrNextName}", img.NextName);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrSim}", img.Sim);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrOffset}", img.Offset);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLenght}", img.Lenght);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrCrc}", img.Crc);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
