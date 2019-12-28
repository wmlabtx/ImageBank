using System;
using System.Linq;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public string ComputeSim()
        {
            AppVars.SuspendEvent.WaitOne(Timeout.Infinite);

            var dt = DateTime.Now;

            var nameX = GetNextToCheck();
            if (string.IsNullOrEmpty(nameX))
            {
                return null;
            }

            var imgpanelX = GetImgPanel(nameX);
            if (imgpanelX == null)
            {
                Delete(nameX);
                return null;
            }

            if (!FindNextName(nameX, out var nameY, out var message))
            {
                return null;
            }

            var imgpanelY = GetImgPanel(nameY);
            if (imgpanelY == null)
            {
                Delete(nameY);
                return null;
            }

            _findtimes.Enqueue((float)DateTime.Now.Subtract(dt).TotalSeconds);
            if (_findtimes.Count > 100)
            {
                _findtimes.Dequeue();
            }

            if (_findtimes.Count > 0)
            {
                _avgtimes = _findtimes.Average();
            }

            AppVars.DistanceMedian = GetDistanceMedian();

            return message;
        }
    }
}