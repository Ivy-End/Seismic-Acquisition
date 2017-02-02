using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Seismic_Acquisition
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            windowSetup();

            Banner banner = new Banner();
            banner.ShowDialog();
        }

        private void windowSetup()
        {
            // position
            double width = System.Windows.SystemParameters.WorkArea.Width;
            double height = System.Windows.SystemParameters.WorkArea.Height;
            this.Left = (width - this.Width) / 2;
            this.Top = height - this.Height;

            // log
            textBoxLog.Text += "-------------------- " + DateTime.Now.ToString().Split(' ')[0].Replace('/', '-') + ".log --------------------" + Environment.NewLine;
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            ShellAbout(GetForegroundWindow(), "关于地震信号检测系统", "未经作者允许，不得擅自复制、拷贝、修改本软件。", IntPtr.Zero);
        }

        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("Document.chm");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuAcquisition_Click(object sender, RoutedEventArgs e)
        {
            Acquisition acquisition = new Acquisition();
            acquisition.Show();
        }

        private void menuHistory_Click(object sender, RoutedEventArgs e)
        {
            History history = new History();
            history.Show();
        }

        private void menuConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Configuration configuration = new Configuration();
            configuration.Show();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        [DllImport("shell32.dll")]
        public extern static int ShellAbout(IntPtr hWnd, string szApp, string szOtherStuff, IntPtr hIcon);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
