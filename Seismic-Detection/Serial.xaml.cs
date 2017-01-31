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
using System.IO.Ports;

namespace Seismic_Detection
{
    /// <summary>
    /// Serial.xaml 的交互逻辑
    /// </summary>
    public partial class Serial : Window
    {
        MainWindow mainWindow;
        public Serial(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSerialPort();
            LoadBaudRate();
        }

        private void LoadSerialPort()
        {
            ComboBox_PortName.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach(string port in ports)
            {
                ComboBox_PortName.Items.Add(port);
            }
            if (ports.Length == 0)
            {
                ComboBox_PortName.Items.Add("无可用串口");
            }
            ComboBox_PortName.SelectedIndex = 0;
        }

        private void LoadBaudRate()
        {
            ComboBox_BaudRate.Items.Clear();
            ComboBox_BaudRate.Items.Add("1200");
            ComboBox_BaudRate.Items.Add("2400");
            ComboBox_BaudRate.Items.Add("4800");
            ComboBox_BaudRate.Items.Add("9600");
            ComboBox_BaudRate.Items.Add("19200");
            ComboBox_BaudRate.Items.Add("38400");
            ComboBox_BaudRate.SelectedIndex = 0;
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            string portName = ComboBox_PortName.SelectedValue.ToString();
            int baudRate = Convert.ToInt32(ComboBox_BaudRate.SelectedValue.ToString());
            if(mainWindow.usart.Open(portName, baudRate, Parity.None, 8, StopBits.One))
            {
                Close();
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
