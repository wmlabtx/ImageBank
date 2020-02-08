using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Delete(int id)
        {
            if (_imgList.TryRemove(id, out var img)) {
                _nameList.TryRemove(img.Name, out _);
                _checksumList.TryRemove(img.Checksum, out _);
                HelperRecycleBin.Delete(img.File);
            }

            SqlDelete(id);
            ResetRefers(id);
        }

        private void ResetRefers(int id)
        {
            _imgList
                .Values
                .Where(e => e.NextId == id)
                .ToList()
                .ForEach(e => e.LastId = -1);
        }
    }
}