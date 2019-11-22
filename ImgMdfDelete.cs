using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void DeleteNextName(string name)
        {
            var scope = _imgList
                .Select(e => e.Value)
                .Where(e => e.NextName.Equals(name))
                .ToArray();

            foreach (var img in scope)
            {
                ResetNextName(img);
            }
        }

        public void DeleteImg(Img img)
        {
            if (!_imgList.TryRemove(img.Name, out _))
            {
                return;
            }

            DeleteNextName(img.Name);

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

            HelperRecycleBin.Delete(img.File);
        }
    }
}