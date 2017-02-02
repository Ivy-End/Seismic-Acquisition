using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismic_Acquisition
{
    public class Signal
    {
        public Signal(double _dataEastWest = 0, double _dataNorthSouth = 0)
        {
            dataEastWest = _dataEastWest;
            dataNorthSouth = _dataNorthSouth;
        }

        public double dataEastWest;
        public double dataNorthSouth;
    }
}
