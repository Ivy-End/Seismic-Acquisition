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
using System.Windows.Threading;

namespace Seismic_Acquisition
{
    /// <summary>
    /// Banner.xaml 的交互逻辑
    /// </summary>
    public partial class Banner : Window
    {
        public Banner()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer bannerTimer = new DispatcherTimer();
            bannerTimer.Interval = TimeSpan.FromSeconds(2);
            bannerTimer.Tick += bannerTimer_Tick;
            bannerTimer.Start();
        }

        private void bannerTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
