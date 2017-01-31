using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismic_Detection
{
    public class Voltage
    {
        public Voltage(double _dataEW = 0, double _dataNS = 0)
        {
            dataEW = _dataEW;
            dataNS = _dataNS;
        }

        public double dataEW;
        public double dataNS;
    }
}
