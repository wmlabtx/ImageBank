using System.Collections.Generic;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void AddToMemory(Img img)
        {
            lock (_imglock) {
                _imgList.Add(img.Id, img);
                _nameList.Add(img.Name, img);
                _checksumList.Add(img.Checksum, img);

                foreach (var descriptor in img.GetDescriptors()) {
                    if (!_descriptorList.TryGetValue(descriptor, out SortedDictionary<int, int> tree)) {
                        tree = new SortedDictionary<int, int>();
                        _descriptorList.Add(descriptor, tree);
                    }

                    if (!tree.ContainsKey(img.Id)) {
                        tree.Add(img.Id, 1);
                    }
                    else {
                        tree[img.Id]++;
                    }
                }
            }
        }

        private void Add(Img img)
        {
            AddToMemory(img);
            SqlAdd(img);
        }
    }
}
