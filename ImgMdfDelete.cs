using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Delete(string hash)
        {
            if (_imgList.TryRemove(hash, out var img))
            {
                SqlDelete(img);
                HelperRecycleBin.Delete(img.File);
            }

            ResetRefers(hash);
        }

        private void ResetNextHash(Img img)
        {
            img.NextHash = img.Hash;
            img.LastCheck = GetMinLastCheck();
        }

        private void ResetRefers(string hash)
        {
            var scope = _imgList
                .Values
                .Where(e => e.NextHash.Equals(hash))
                .ToList();

            scope.ForEach(e => ResetNextHash(e));
        }
    }
}