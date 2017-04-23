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
using OxyPlot;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Seismic_Acquisition
{
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class History : Window
    {
        bool play = false;
        bool pause = false;
        bool opened = false;
        int currentPos = 0;
        string historyFolder;
        DateTime historyDate;
        DispatcherTimer playTimer;
        List<string> dataPath = new List<string>();
        public History()
        {
            InitializeComponent();
        }

        private void ViewHistory()
        {
            dataPath.Clear();
            var files = Directory.GetFiles(historyFolder, "*.sdf");
            foreach (var file in files)
            {
                dataPath.Add(file);
            }
            if (dataPath.Count == 0)
            {
                MessageBox.Show(historyDate.ToString("yyyy年MM月dd日") + "没有数据", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                currentPos = 0;
                LoadData();

            }
        }

        private void LoadData()
        {
            string timeSpan = "-";
            int H = Convert.ToInt32(dataPath[currentPos].Substring(dataPath[currentPos].Length - 8, 4).Substring(0, 2));
            int M = Convert.ToInt32(dataPath[currentPos].Substring(dataPath[currentPos].Length - 8, 4).Substring(2, 2));
            timeSpan += (H / 10).ToString();
            timeSpan += (H % 10).ToString();
            timeSpan += ":";
            timeSpan += (M / 10).ToString();
            timeSpan += (M % 10).ToString();
            if((M -= 10) == -10)
            {
                M += 60;
                if (H-- == -1)
                {
                    H += 24;
                }
            }
            timeSpan = (M % 10).ToString() + timeSpan;
            timeSpan = (M / 10).ToString() + timeSpan;
            timeSpan = ":" + timeSpan;
            timeSpan = (H % 10).ToString() + timeSpan;
            timeSpan = (H / 10).ToString() + timeSpan;

            textBlockStatus.Text = historyDate.ToString("yyyy年MM月dd日 ") + timeSpan;


            listViewData.Items.Clear();
            History_DataEastWest.Clear();
            History_DataNorthSouth.Clear();

            string buffer = "";
            try
            {
                StreamReader streamReader = new StreamReader(dataPath[currentPos], Encoding.Default);
                buffer = streamReader.ReadToEnd();
                buffer = ZipUtil.Decompress(buffer);
                streamReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            string[] tmp = buffer.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in tmp)
            {
                string[] data = row.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                double time = ((Convert.ToDouble(data[0].Substring(0, 2)) * 60 + Convert.ToDouble(data[0].Substring(3, 2))) * 60 + Convert.ToDouble(data[0].Substring(6, 2))) * 60 + Convert.ToDouble(data[0].Substring(9, 3)) / 1000.0d;
                History_DataEastWest.Add(new DataPoint(time, Convert.ToDouble(data[1])));
                History_DataNorthSouth.Add(new DataPoint(time, Convert.ToDouble(data[2])));

                var item = new { listViewDataTime = data[0], listViewDataEastWest = data[1], listViewDataNorthSouth = data[2] };

                listViewData.Items.Add(item);
            }
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

        private void toolOpen_Click(object sender, RoutedEventArgs e)
        {
            string folder = "";
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择历史文件路径";
            dialog.SelectedPath = Environment.CurrentDirectory + "\\data\\";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                opened = true;
                imagePrevious.Source = new BitmapImage(new Uri("Resources/1_View_Previous.png", UriKind.RelativeOrAbsolute));
                imageRewind.Source = new BitmapImage(new Uri("Resources/1_View_Rewind.png", UriKind.RelativeOrAbsolute));
                imagePlay.Source = new BitmapImage(new Uri("Resources/1_View_Play.png", UriKind.RelativeOrAbsolute));
                imageForward.Source = new BitmapImage(new Uri("Resources/1_View_Forward.png", UriKind.RelativeOrAbsolute));
                imageNext.Source = new BitmapImage(new Uri("Resources/1_View_Next.png", UriKind.RelativeOrAbsolute));

                historyFolder = dialog.SelectedPath;
                folder = historyFolder.Substring(historyFolder.Length - 10, 10);
                historyDate = new DateTime(Convert.ToInt32(folder.Substring(0, 4)), 
                    Convert.ToInt32(folder.Substring(5, 2)), Convert.ToInt32(folder.Substring(8, 2)));
                ViewHistory();
            }
        }

        private void toolPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (opened)
            {
                if ((currentPos -= 3) < 0)
                {
                    currentPos += dataPath.Count;
                }
                LoadData();
            }
        }

        private void toolRewind_Click(object sender, RoutedEventArgs e)
        {
            if (opened)
            {
                if (--currentPos < 0)
                {
                    currentPos += dataPath.Count;
                }
                LoadData();
            }
        }
        private void toolPlay_Click(object sender, RoutedEventArgs e)
        {
            if (opened)
            {
                if (play)
                {
                    playTimer.Stop();
                    pause = true;
                    play = false;
                    imagePlay.Source = new BitmapImage(new Uri("Resources/1_View_Play.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    if (!pause)
                    {
                        currentPos = -1;
                    }
                    play = true;
                    playTimer.Start();
                    imagePlay.Source = new BitmapImage(new Uri("Resources/1_View_Pause.png", UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void toolForward_Click(object sender, RoutedEventArgs e)
        {
            if (opened)
            {
                if (++currentPos >= dataPath.Count)
                {
                    currentPos -= dataPath.Count;
                }
                LoadData();
            }
        }

        private void toolNext_Click(object sender, RoutedEventArgs e)
        {
            if (opened)
            {
                if ((currentPos += 3) >= dataPath.Count)
                {
                    currentPos -= dataPath.Count;
                }
                LoadData();
            }
        }

        private void toolExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public ObservableCollection<DataPoint> History_DataNorthSouth
        {
            get { return (ObservableCollection<DataPoint>)GetValue(History_DataPropertyNS); }
            set { SetValue(History_DataPropertyNS, value); }
        }
        public ObservableCollection<DataPoint> History_DataEastWest
        {
            get { return (ObservableCollection<DataPoint>)GetValue(History_DataPropertyEW); }
            set { SetValue(History_DataPropertyEW, value); }
        }

        public static readonly DependencyProperty History_DataPropertyNS =
            DependencyProperty.Register("History_DataNorthSouth", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));
        public static readonly DependencyProperty History_DataPropertyEW =
            DependencyProperty.Register("History_DataEastWest", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));

        private void toolOpen_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "打开历史文件夹";
        }

        private void toolOpen_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolRewind_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "向前30分钟";
        }

        private void toolRewind_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolPrevious_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "向前10分钟";
        }

        private void toolPrevious_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolPlay_MouseEnter(object sender, MouseEventArgs e)
        {
            if (play)
            {
                textBlockInstruction.Text = "暂停播放";
            }
            else
            {
                textBlockInstruction.Text = "开始播放";
            }
        }

        private void toolPlay_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolForward_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "向前10分钟";
        }

        private void toolForward_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolNext_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "向前30分钟";
        }

        private void toolNext_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void toolExit_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "退出历史回看系统";
        }

        private void toolExit_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockInstruction.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            playTimer = new DispatcherTimer();
            playTimer.Interval = TimeSpan.FromMilliseconds(200);
            playTimer.Tick += playTimer_Tick;
        }

        void playTimer_Tick(object sender, EventArgs e)
        {
            currentPos++;
            LoadData();
            if(currentPos == dataPath.Count)
            {
                playTimer.Stop();
                pause = false;
                play = false;
                imagePlay.Source = new BitmapImage(new Uri("Resources/1_View_Play.png", UriKind.RelativeOrAbsolute));

            }
        }

        private void LinearAxis_History_NS_Loaded(object sender, RoutedEventArgs e)
        {
            // graph limitation
            LinearAxis_History_NS.Maximum = Properties.Settings.Default.History_NS_Maximum;
            LinearAxis_History_NS.Minimum = Properties.Settings.Default.History_NS_Minimum;
            LinearAxis_History_EW.Maximum = Properties.Settings.Default.History_EW_Maximum;
            LinearAxis_History_EW.Minimum = Properties.Settings.Default.History_EW_Minimum;
        }
    }
}
