using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using OxyPlot;
using System.IO.Ports;

namespace Seismic_Detection
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        double time = 0d;
        Voltage voltage = new Voltage();
        public Usart usart = new Usart();
        Detector detector = new Detector();
        DispatcherTimer dispatcherTimer_dataCollection = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer_dataCollection.Interval = TimeSpan.FromMilliseconds(50);
            dispatcherTimer_dataCollection.Tick += dispatcherTimer_dataCollection_Tick;
            //dispatcherTimer_dataCollection.Start();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setDisplayDate();

            Banner banner = new Banner();
            banner.ShowDialog(this);
        }

        private void dispatcherTimer_dataCollection_Tick(object sender, EventArgs e)
        {
            dispatcherTimer_dataCollection.Stop();

            DateTime now = DateTime.Now;
            time = (double)((now.Hour * 60 + now.Minute) * 60 + now.Second) + now.Millisecond / 1000.0d;

            Voltage voltage_new = usart.Receive();
            if (voltage_new.dataEW != 0 && voltage_new.dataNS != 0)
            {
                voltage.dataNS = voltage_new.dataNS;
                voltage.dataEW = voltage_new.dataEW;
            }

            DataEW.Add(new DataPoint(time, voltage.dataEW));
            DataNS.Add(new DataPoint(time, voltage.dataNS));

            var item = new { ListView_DateTime = calcTimeString(time), ListView_DataEW = voltage.dataEW.ToString("F3"), ListView_DataNS = voltage.dataNS.ToString("F3") };
            
            ListView_Data.Items.Add(item);

            ListView_Data.ScrollIntoView(item);

            //time += 0.1;

            if (Convert.ToInt32(time + 0.01) == 24 * 60 * 60)
            {
                saveData();
                MessageBox.Show("Save");

                DataEW.Clear();
                DataNS.Clear();
                ListView_Data.Items.Clear();
                time = 0d;

                setDisplayDate();
            }

            dispatcherTimer_dataCollection.Start();
        }

        private string calcTimeString(double time)
        {
            string ans = "";
            int tmp = Convert.ToInt32(time * 10 + 0.1);
            ans += Convert.ToInt32(tmp / 10 / 60 / 60 / 10).ToString();
            ans += Convert.ToInt32(tmp / 10 / 60 / 60 % 10).ToString();
            ans += ":";
            ans += Convert.ToInt32((tmp / 10 / 60 % 60) / 10).ToString();
            ans += Convert.ToInt32((tmp / 10 / 60 % 60) % 10).ToString();
            ans += ":";
            ans += Convert.ToInt32(tmp / 10 % 60 / 10).ToString();
            ans += Convert.ToInt32(tmp / 10 % 60 % 10).ToString();
            ans += ".";
            ans += (Convert.ToInt32(tmp % 10 * 100).ToString() == "0" ? "000" : Convert.ToInt32(tmp % 10 * 100).ToString());
            return ans;
        }

        private void setDisplayDate()
        {
            string ans = "";
            DateTime dateTime = DateTime.Now;
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

            ans += dateTime.ToString("yyyy年MM月dd日") + Environment.NewLine;
            ans += weekdays[Convert.ToInt32(dateTime.DayOfWeek)] + Environment.NewLine;
            ans += ChineseCalendar.GetLunarCalendar(dateTime);
            Date.Text = ans;
        }

        private void saveData()
        {
            string buffer = "", tmp;
            DateTime dateTime = DateTime.Now;
            for (int i = 0; i < ListView_Data.Items.Count; i++)
            {
                tmp = ListView_Data.Items[i].ToString();
                tmp = tmp.Replace("{ ListView_DateTime = ", "");
                tmp = tmp.Replace(", ListView_DataEW = ", " ");
                tmp = tmp.Replace(", ListView_DataNS = ", " ");
                tmp = tmp.Replace(" }", "");
                buffer += tmp + Environment.NewLine;
            }
            
            //buffer = ZipUtil.Compress(buffer);

            if(!Directory.Exists("data"))
            {
                try
                {
                    Directory.CreateDirectory("data");
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            try
            {
                FileStream fileStream = new FileStream("data\\" + dateTime.ToString("yyyy年MM月dd日") + ".sdf", FileMode.Create);
                byte[] data = Encoding.Default.GetBytes(buffer);
                fileStream.Write(data, 0, data.Length);
                fileStream.Flush();
                fileStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public ObservableCollection<DataPoint> DataNS
        {
            get { return (ObservableCollection<DataPoint>)GetValue(DataPropertyNS); }
            set { SetValue(DataPropertyNS, value); }
        }
        public ObservableCollection<DataPoint> DataEW
        {
            get { return (ObservableCollection<DataPoint>)GetValue(DataPropertyEW); }
            set { SetValue(DataPropertyEW, value); }
        }

        public static readonly DependencyProperty DataPropertyNS =
            DependencyProperty.Register("DataNS", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));
        public static readonly DependencyProperty DataPropertyEW =
            DependencyProperty.Register("DataEW", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));

        private void MenuItem_StartAcquire_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(MenuItem_StartAcquire.Header) == "开始采集")
            {
                Serial serial = new Serial(this);
                serial.ShowDialog();
                if (usart.IsOpen())
                {
                    dispatcherTimer_dataCollection.Start();
                    MenuItem_StartAcquire.Header = "停止采集";
                }
            }
            else
            {
                if (usart.Close())
                {
                    dispatcherTimer_dataCollection.Stop();
                    MenuItem_StartAcquire.Header = "开始采集";
                }
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认退出系统？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            Banner banner = new Banner();
            banner.ShowDialog(this);
        }

        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Help doucment
            string help = "【使用说明】" + Environment.NewLine;
            help += "鼠标左键：获取当前时间坐标对应的信号强度" + Environment.NewLine;
            help += "鼠标右键：拖动视图" + Environment.NewLine;
            help += "鼠标滚轮：放大、缩小视图" + Environment.NewLine;
            help += "滚轮框选：放大特定区域" + Environment.NewLine;
            help += "双击滚轮：恢复默认视图";

            MessageBox.Show(help, "帮助", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Seismic Detection Files (*.sdf)|*.sdf"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                History history = new History(openFileDialog.FileName);
                history.Show();
            }
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            saveData();
        }
    }
}
