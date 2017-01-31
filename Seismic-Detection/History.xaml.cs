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
using System.Windows.Shapes;
using System.IO;
using OxyPlot;

namespace Seismic_Detection
{
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class History : Window
    {
        string fileName;
        string dataBuffer;


        public History(string fileName)
        {
            InitializeComponent();

            this.fileName = fileName;
        }

        private void loadData()
        {
            try
            {
                StreamReader streamReader = new StreamReader(fileName, Encoding.Default);
                dataBuffer = streamReader.ReadToEnd();
                //dataBuffer = ZipUtil.Decompress(dataBuffer);
                streamReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void displayData()
        {
            string[] tmp = dataBuffer.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach(string row in tmp)
            {
                string[] data = row.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                string[] time = data[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                double dataTime = (Convert.ToDouble(time[0]) * 60 + Convert.ToDouble(time[1])) * 60 + Convert.ToDouble(time[2].Substring(0, 2)) + Convert.ToDouble(time[2].Substring(3, 3)) / 1000.0d;
                double dataEW = Convert.ToDouble(data[1]);
                double dataNS = Convert.ToDouble(data[2]);
                addToView(dataTime, dataEW, dataNS);
            }
        }

        private void addToView(double time, double dataEW, double dataNS)
        {
            DataEW_History.Add(new DataPoint(time, dataEW));
            DataNS_History.Add(new DataPoint(time, dataNS));

            var item = new { ListView_DateTime = calcTimeString(time), ListView_DataEW = dataEW.ToString("F3"), ListView_DataNS = dataNS.ToString("F3") };

            ListView_Data.Items.Add(item);
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
            string name = fileName.Substring(fileName.LastIndexOf('\\') + 1, 11);
            DateTime dateTime = new DateTime(Convert.ToInt32(name.Substring(0, 4)), Convert.ToInt32(name.Substring(5, 2)), Convert.ToInt32(name.Substring(8, 2)));
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

            ans += dateTime.ToString("yyyy年MM月dd日") + Environment.NewLine;
            ans += weekdays[Convert.ToInt32(dateTime.DayOfWeek)] + Environment.NewLine;
            ans += ChineseCalendar.GetLunarCalendar(dateTime);
            Date.Text = ans;
        }

        public ObservableCollection<DataPoint> DataNS_History
        {
            get { return (ObservableCollection<DataPoint>)GetValue(DataPropertyNS_History); }
            set { SetValue(DataPropertyNS_History, value); }
        }
        public ObservableCollection<DataPoint> DataEW_History
        {
            get { return (ObservableCollection<DataPoint>)GetValue(DataPropertyEW_History); }
            set { SetValue(DataPropertyEW_History, value); }
        }

        public static readonly DependencyProperty DataPropertyNS_History =
            DependencyProperty.Register("DataNS_History", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));
        public static readonly DependencyProperty DataPropertyEW_History =
            DependencyProperty.Register("DataEW_History", typeof(ObservableCollection<DataPoint>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<DataPoint>()));
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setDisplayDate();

            loadData();
            displayData();


        }
    }
}
