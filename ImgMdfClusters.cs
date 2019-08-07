using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public int GetAvailableCluster()
        {
            var cluster = _imgList.Max(e => e.Value.Cluster) + 1;
            return cluster;
        }

        public int GetClusterSize(int cluster)
        {
            return _imgList.Count(e => e.Value.Cluster == cluster);
        }

        public void CombineClusters(Img imgX, Img imgY)
        {
            if (imgX.Cluster == 0 && imgY.Cluster == 0)
            {
                imgX.Cluster = GetAvailableCluster();
                UpdateCluster(imgX);
                imgY.Cluster = imgX.Cluster;
                UpdateCluster(imgY);
            }
            else
            {
                if (imgX.Cluster != 0 && imgY.Cluster == 0)
                {
                    imgY.Cluster = imgX.Cluster;
                    UpdateCluster(imgY);
                }
                else
                {
                    if (imgX.Cluster == 0 && imgY.Cluster != 0)
                    {
                        imgX.Cluster = imgY.Cluster;
                        UpdateCluster(imgX);
                    }
                    else
                    {
                        if (imgX.Cluster < imgY.Cluster)
                        {
                            imgY.Cluster = imgX.Cluster;
                            UpdateCluster(imgY);
                        }
                        else
                        {
                            if (imgX.Cluster > imgY.Cluster)
                            {
                                imgX.Cluster = imgY.Cluster;
                                UpdateCluster(imgX);
                            }
                            else
                            {
                                imgY.Cluster = GetAvailableCluster();
                                UpdateCluster(imgY);
                            }
                        }
                    }
                }
            }
        }
    }
}
