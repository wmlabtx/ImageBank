using System.Collections.Generic;
using System.Linq;

namespace ImageBank
{
    public class SiftNode
    {
        public int NodeId { get; }
        public int ClusterId { get; set; }
        public List<SiftPoint> N { get; }
        public SiftPoint C { get; private set; }

        public SiftNode(int nodeid)
        {
            NodeId = nodeid;
            N = new List<SiftPoint>();
        }

        public int Count => N.Count;        

        public int LongestEdge { get; private set; }

        public int EdgeLength { get; private set; }

        public int EdgeMedian { get; private set; }

        public int R0 { get; set; }
        public int R1 { get; set; }

        public void Update()
        {
            EdgeLength = 0;
            var edgemedian = new byte[128];
            for (var i = 0; i < 128; i++)
            {
                var edge = N.Select(e => e.V[i]).OrderBy(e => e).ToArray();
                var edgelength = edge[edge.Length - 1] - edge[0];
                edgemedian[i] = edge[edge.Length / 2];
                if (edgelength > EdgeLength)
                {
                    LongestEdge = i;
                    EdgeLength = edgelength;
                    EdgeMedian = edgemedian[i];
                }
            }

            C = new SiftPoint(edgemedian, 0);
        }
    }
}