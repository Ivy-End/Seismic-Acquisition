using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismic_Detection
{
    public class Detector
    {
        double Count = 0;

        public double NextDataEW()
        {
            double ans = Math.Sin(Count);
            Count += 0.1;
            return ans;
        }

        public double NextDataNS()
        {
            double ans = Math.Cos(Count);
            Count += 0.1;
            return ans;
        }
    }
}
