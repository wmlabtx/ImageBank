using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public byte[] GetJpgData(Img img)
        {
            var offset = _dataStream.Seek(img.Offset, SeekOrigin.Begin);
            if (offset != img.Offset)
            {
                return null;
            }

            var array = new byte[img.Lenght];
            var read = _dataStream.Read(array, 0, array.Length);
            if (read != array.Length)
            {
                return null;
            }

            var crc = HelperCrc.GetCrc(array);
            if (!crc.Equals(img.Crc))
            {
                return null;
            }

            var jpgdata = HelperEncrypting.Decrypt(array, img.Name);
            if (jpgdata == null)
            {
                return null;
            }

            return jpgdata;
        }

        public void WriteData(long offset, byte[] array)
        {
            _dataStream.Seek(offset, SeekOrigin.Begin);
            _dataStream.Write(array, 0, array.Length);
            _dataStream.Flush();
        }

        public void SetJpgData(Img img, byte[] jpgdata)
        {
            lock (_sqlLock)
            {
                var array = HelperEncrypting.Encrypt(jpgdata, img.Name);
                img.Crc = HelperCrc.GetCrc(array);
                img.Lenght = array.Length;
                img.Offset = AppVars.Collection.GetSuggestedOffset(img.Lenght);
                WriteData(img.Offset, array);

                var sb = new StringBuilder();
                sb.Append("UPDATE Images SET ");
                sb.Append($"{AppConsts.AttrOffset} = @{AppConsts.AttrOffset}, ");
                sb.Append($"{AppConsts.AttrLenght} = @{AppConsts.AttrLenght}, ");
                sb.Append($"{AppConsts.AttrCrc} = @{AppConsts.AttrCrc} ");
                sb.Append("WHERE ");
                sb.Append($"{AppConsts.AttrName} = @{AppConsts.AttrName}");
                var sqltext = sb.ToString();
                using (var sqlCommand = new SqlCommand(sqltext, _sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrOffset}", img.Offset);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrLenght}", img.Lenght);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrCrc}", img.Crc);
                    sqlCommand.Parameters.AddWithValue($"@{AppConsts.AttrName}", img.Name);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public long GetSuggestedOffset(int lenght)
        {
            if (_imgList.Count == 0)
            {
                return 0L;
            }

            var imgarray = _imgList.OrderBy(e => e.Value.Offset).Select(e => e.Value).ToArray();
            var index = 0;
            var offset = 0L;
            while (index < imgarray.Length && offset + lenght > imgarray[index].Offset)
            {
                offset = imgarray[index].Offset + imgarray[index].Lenght;
                index++;
            }

            if (index >= imgarray.Length)
            {
                offset = imgarray[index - 1].Offset + imgarray[index - 1].Lenght;
            }

            return offset;
        }
    }
}