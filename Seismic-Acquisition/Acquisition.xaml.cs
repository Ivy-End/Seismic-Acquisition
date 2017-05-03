using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
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
using OxyPlot;
using System.IO;

namespace Seismic_Acquisition
{
    /// <summary>
    /// Acquisition.xaml 的交互逻辑
    /// </summary>
    public partial class Acquisition : Window
    {
        double time;
        int dataCount = 0;
        int timerCount = 0;
        string validPort = "";
        bool connected = false;
        bool acquisition = false;
        public double acquireEWUp = 0, acquireEWDown = 0, acquireNSUp = 0, acquireNSDown = 0;
        Usart usart = new Usart();
        DispatcherTimer clockTimer = new DispatcherTimer();
        public Acquisition()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // timeset
            clockTimer.Interval = TimeSpan.FromMilliseconds(500);
            clockTimer.Tick += clockTimer_Tick;
            clockTimer.Start();

            // statusbar
            textBlockStatus.Text = "未连接";

            // graph limitation
            LinearAxis_Acquire_NS.Maximum = Properties.Settings.Default.Acquire_NS_Maximum;
            LinearAxis_Acquire_NS.Minimum = Properties.Settings.Default.Acquire_NS_Minimum;
            LinearAxis_Acquire_EW.Maximum = Properties.Settings.Default.Acquire_EW_Maximum;
            LinearAxis_Acquire_EW.Minimum = Properties.Settings.Default.Acquire_EW_Minimum;

            // threshold
            acquireEWUp = Properties.Settings.Default.Acquire_EW_Up;
            acquireEWDown = Properties.Settings.Default.Acquire_EW_Down;
            acquireNSUp = Properties.Settings.Default.Acquire_NS_Up;
            acquireNSDown = Properties.Settings.Default.Acquire_NS_Down;
        }

        private string GetTimeString(DateTime now)
        {
            string ans = "";
            ans += (now.Hour / 10).ToString();
            ans += (now.Hour % 10).ToString();
            ans += ":";
            ans += (now.Minute / 10).ToString();
            ans += (now.Minute % 10).ToString();
            ans += ":";
            ans += (now.Second / 10).ToString();
            ans += (now.Second % 10).ToString();
            ans += ".";
            ans += (now.Millisecond / 100).ToString();
            ans += (now.Millisecond % 100 / 10).ToString();
            ans += (now.Millisecond % 100 % 10).ToString();
            return ans;
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            if(++timerCount == 2)
            {
                textBlockDateTime.Text = DateTime.Now.ToString("HH:mm:ss\r\nyyyy年MM月dd日");
                timerCount = 0;

                AutoSaveData();
            }

            if (acquisition)
            {
                AcquireData();

                //dataCount++;

                textBlockStatistics.Text = "总计接收数据：" + dataCount.ToString();
            }
        }

        private void AutoSaveData()
        {
            DateTime nowTime = DateTime.Now;
            if (nowTime.Second == 0 && nowTime.Minute % 10 == 0)
            {
                string filename = "";
                string buffer = "", tmp = "";
                filename += (nowTime.Hour / 10).ToString();
                filename += (nowTime.Hour % 10).ToString();
                filename += (nowTime.Minute / 10).ToString();
                filename += (nowTime.Minute % 10).ToString();

                for (int i = 0; i < listViewData.Items.Count; i++)
                {
                    tmp = listViewData.Items[i].ToString();
                    tmp = tmp.Replace("{ listViewDataTime = ", "");
                    tmp = tmp.Replace(", listViewDataEastWest = ", " ");
                    tmp = tmp.Replace(", listViewDataNorthSouth = ", " ");
                    tmp = tmp.Replace(" }", "");
                    buffer += tmp + Environment.NewLine;
                }

                buffer = ZipUtil.Compress(buffer);

                if (!Directory.Exists("data"))
                {
                    try
                    {
                        Directory.CreateDirectory("data");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                if (nowTime.Hour == 0 && nowTime.Minute == 0)
                {
                    nowTime = nowTime.AddDays(-1);
                    filename = "2400";
                }


                if (!Directory.Exists("data\\" + nowTime.ToString("yyyy.MM.dd")))
                {
                    try
                    {
                        Directory.CreateDirectory("data\\" + nowTime.ToString("yyyy.MM.dd"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                try
                {
                    FileStream fileStream = new FileStream("data\\" + nowTime.ToString("yyyy.MM.dd") + "\\" + filename + ".sdf", FileMode.Create);
                    byte[] data = Encoding.Default.GetBytes(buffer);
                    fileStream.Write(data, 0, data.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                // TODO: Compress day file

                listViewData.Items.Clear();
                Acquire_DataNorthSouth.Clear();
                Acquire_DataEastWest.Clear();

                TimeSpanAxis_Acquire_EW.Minimum = (nowTime.Hour * 60 + nowTime.Minute) * 60 + nowTime.Second;
                TimeSpanAxis_Acquire_EW.Maximum = (nowTime.Hour * 60 + nowTime.Minute) * 60 + nowTime.Second + 600;
                TimeSpanAxis_Acquire_NS.Minimum = (nowTime.Hour * 60 + nowTime.Minute) * 60 + nowTime.Second;
                TimeSpanAxis_Acquire_NS.Maximum = (nowTime.Hour * 60 + nowTime.Minute) * 60 + nowTime.Second + 600;
            }
        }

        private void AcquireData()
        {
            DateTime now = DateTime.Now;
            time = (double)((now.Hour * 60 + now.Minute) * 60 + now.Second) + now.Millisecond / 1000.0d;

            Signal signal = usart.Pop();
            if (signal.dataEastWest >= acquireEWDown && signal.dataEastWest <= acquireEWUp)
            {
                signal.dataEastWest = (acquireEWDown + acquireEWUp) / 2;
            }
            if (signal.dataNorthSouth >= acquireNSDown && signal.dataNorthSouth <= acquireNSUp)
            {
                signal.dataNorthSouth = (acquireNSDown + acquireNSUp) / 2;
            }
            if (signal.dataEastWest != 0 || signal.dataNorthSouth != 0)
            {

                Acquire_DataEastWest.Add(new DataPoint(time, signal.dataEastWest));
                Acquire_DataNorthSouth.Add(new DataPoint(time, signal.dataNorthSouth));

                var item = new
                {
                    listViewDataTime = GetTimeString(now),
                    listViewDataEastWest = signal.dataEastWest.ToString("F3"),
                    listViewDataNorthSouth = signal.dataNorthSouth.ToString("F3")
                };

                dataCount++;

                listViewData.Items.Add(item);

                listViewData.ScrollIntoView(item);
            }
        }

        private bool portConnect(string portName)
        {
            if(usart.Open(portName, 9600, Parity.None, 8, StopBits.One))
            {
                return true;
            }
            return false;
        }

        public ObservableCollection<DataPoint> Acquire_DataNorthSouth
        {
            get { return (ObservableCollection<DataPoint>)GetValue(Acquire_DataPropertyNS); }
            set { SetValue(Acquire_DataPropertyNS, value); }
        }
        public ObservableCollection<DataPoint> Acquire_DataEastWest
        {
            get { return (ObservableCollection<DataPoint>)GetValue(Acquire_DataPropertyEW); }
            set { SetValue(Acquire_DataPropertyEW, value); }
        }

        public static readonly DependencyProperty Acquire_DataPropertyNS =
            DependencyProperty.Register("Acquire_DataNorthSouth", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));
        public static readonly DependencyProperty Acquire_DataPropertyEW =
            DependencyProperty.Register("Acquire_DataEastWest", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));

        private int getCurrentTime(DateTime datetime)
        {
            return (datetime.Hour * 60 + datetime.Minute) * 60 + datetime.Second;
        }

        private int getNextTime(DateTime datetime)
        {
            while (!(datetime.Second == 0 && datetime.Minute % 10 == 0))
            {
                datetime = datetime.AddSeconds(1);
            }
            return (datetime.Hour * 60 + datetime.Minute) * 60 + datetime.Second;
        }
        
        private void toolConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!connected)
            {
                string[] ports = SerialPort.GetPortNames();
                int totalCount = ports.Count();

                for(int i = 0; i < totalCount; i++)
                {
                    if (portConnect(ports[i]))
                    {
                        System.Threading.Thread.Sleep(1500);
                        usart.Close();

                        if (usart.Count() > 0)
                        {
                            connected = true;
                            validPort = ports[i];
                            textBlockStatus.Text = "已连接：" + validPort;
                            MessageBox.Show("连接成功：" + validPort, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (listViewData.Items.Count == 0)
                            {
                                DateTime datetime = DateTime.Now;
                                int tmp = getCurrentTime(datetime);
                                int end = getNextTime(datetime);
                                TimeSpanAxis_Acquire_EW.Minimum = tmp;
                                TimeSpanAxis_Acquire_EW.Maximum = end;
                                TimeSpanAxis_Acquire_NS.Minimum = tmp;
                                TimeSpanAxis_Acquire_NS.Maximum = end;
                            }
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    usart.Close();
                }

                if (connected)
                {
                    if (!portConnect(validPort))
                    {
                        connected = false;
                    }
                    else
                    {
                        acquisition = true;
                        imageConnect.Source = new BitmapImage(new Uri("Resources/Connected_True.png", UriKind.RelativeOrAbsolute));
                        imageStart.Source = new BitmapImage(new Uri("Resources/Pause.png", UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                    MessageBox.Show("没有可用的信号源，请检查设备是否连接正确。", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if(usart.Close())
                {
                    connected = false;
                    acquisition = false;
                    textBlockStatus.Text = "未连接";
                    imageConnect.Source = new BitmapImage(new Uri("Resources/Connected_False.png", UriKind.RelativeOrAbsolute));
                    imageStart.Source = new BitmapImage(new Uri("Resources/Start_False.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    MessageBox.Show("设备正在运行，无法关闭，请稍后重试。", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void toolExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void toolStart_Click(object sender, RoutedEventArgs e)
        {
            if (!connected)
            {
                return;
            }
            if (!acquisition)
            {
                acquisition = true;
                imageStart.Source = new BitmapImage(new Uri("Resources/Pause.png", UriKind.RelativeOrAbsolute));
                usart.ClearBuffer();
            }
            else
            {
                acquisition = false;
                imageStart.Source = new BitmapImage(new Uri("Resources/Start_True.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void toolConnect_MouseEnter(object sender, MouseEventArgs e)
        {
            if (connected)
            {
                textBlockInstruction.Text = "断开地震信号采集设备";
            }
            else
            {
                textBlockInstruction.Text = "连接地震信号采集设备";
            }
        }

        private void toolConnect_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolExit_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "退出地震信号采集系统";
        }

        private void toolExit_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolStart_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!acquisition)
            {
                textBlockInstruction.Text = "开始采集地震信号";
            }
            else
            {
                textBlockInstruction.Text = "停止采集地震信号";
            }
        }

        private void toolStart_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (usart.IsOpen())
            {
                usart.Close();
            }

            connected = false;
            acquisition = false;

            clockTimer.Stop();
            listViewData.Items.Clear();
            Acquire_DataEastWest.Clear();
            Acquire_DataNorthSouth.Clear();
        }
    }
}
