using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Delete(string name)
        {
            if (_imgList.TryRemove(name, out var img))
            {
                _availableOrbsSlots.TryAdd(img.OrbsSlot, null);
                SqlDelete(img);
                HelperRecycleBin.Delete(img.DataFile);
            }

            ResetRefers(name);
        }
    }
}