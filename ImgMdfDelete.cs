using System.Collections.Generic;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Delete(int id)
        {
            lock (_imglock) {
                if (_imgList.TryGetValue(id, out var img)) {                    
                    _nameList.Remove(img.Name);
                    _checksumList.Remove(img.Checksum);
                    foreach (var descriptor in img.GetDescriptors()) {
                        if (_descriptorList.TryGetValue(descriptor, out SortedDictionary<int, int> tree)) {
                            if (tree.ContainsKey(img.Id)) {
                                tree[img.Id]--;
                                if (tree[img.Id] == 0) {
                                    tree.Remove(img.Id);
                                }
                            }
                        }
                    }

                    Helper.DeleteToRecycleBin(img.File);
                    _imgList.Remove(id);
                }
            }

            SqlDelete(id);
            ResetRefers(id);
        }

        private void ResetRefers(int id)
        {
            lock (_imglock) {
                _imgList
                    .Values
                    .Where(e => e.NextId == id)
                    .ToList()
                    .ForEach(e => e.LastId = -1);
            }
        }
    }
}