using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Delete(string hash)
        {
            if (_imgList.TryRemove(hash, out var img)) {
                HelperRecycleBin.Delete(img.File);
            }

            SqlDelete(hash);
            ResetRefers(hash);
        }

        private void ResetRefers(string hash)
        {
            _imgList
                .Values
                .Where(e => e.NextHash.Equals(hash))
                .ToList()
                .ForEach(e => e.LastId = -1);
        }
    }
}