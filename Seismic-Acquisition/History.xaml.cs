using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Seismic_Acquisition
{
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class History : Window
    {
        bool dateSelected = false;
        DateTime historyDate;
        public History(bool today = false)
        {
            InitializeComponent();
            if (today)
            {
                dateSelected = true;
                historyDate = DateTime.Now;

                ViewHistory();
            }
        }

        private void ViewHistory()
        {

        }
    }
}
