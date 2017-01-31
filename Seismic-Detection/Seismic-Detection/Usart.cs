using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;

namespace Seismic_Detection
{
    public class Usart
    {
        private SerialPort serialPort;
        Queue<Voltage> DataSet = new Queue<Voltage>();
        string dataSet = "";

        public Usart()
        {
            serialPort = new SerialPort();
        }

        public bool IsOpen()
        {
            return serialPort.IsOpen;
        }

        public bool Close()
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                    return true;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "串口配置");
                    return false;
                }
            }
            return false;
        }

        public bool Open(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {
                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.Parity = parity;
                serialPort.DataBits = dataBits;
                serialPort.StopBits = stopBits;
                serialPort.DataReceived += serialPort_DataReceived;
                serialPort.Open();
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "串口配置", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int length = serialPort.BytesToRead;
            byte[] buffer = new byte[length];
            serialPort.Read(buffer, 0, length);
            foreach (byte tmp in buffer)
            {
                dataSet += (char)tmp;
            }
            Modify();
        }

        public void Send(string msg)
        {
            msg = msg.Trim();
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("串口未打开", "串口配置", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            serialPort.Write(msg);
        }

        private void Modify()
        {
            while(dataSet.IndexOf('\n') != -1)
            {
                double dataNS = 0, dataEW = 0;
                string dataTmp = "";
                int pos = dataSet.IndexOf('\n');
                if (pos != -1)
                {
                    dataTmp = dataSet.Substring(0, pos);
                    dataSet = dataSet.Substring(pos + 1, dataSet.Length - pos - 1);
                    if (dataTmp[0] != '[') { continue; }
                    try
                    {
                        dataTmp = dataTmp.Substring(1, dataTmp.Length - 2);
                        string[] dataSplit = dataTmp.Split(' ');
                        foreach (string dataSplitItem in dataSplit)
                        {
                            if (dataSplitItem[0] == 'N')
                            {
                                dataNS = Convert.ToDouble(dataSplitItem.Substring(1, dataSplitItem.Length - 1));
                            }
                            else
                            {
                                dataEW = Convert.ToDouble(dataSplitItem.Substring(1, dataSplitItem.Length - 1));
                            }
                        }
                        Voltage voltage = new Voltage(dataEW, dataNS);
                        DataSet.Enqueue(voltage);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }

        public Voltage Receive()
        {
            if (DataSet.Count > 0)
            {
                return DataSet.Dequeue();
            }
            else
            {
                return new Voltage(0, 0);
            }
        }
    }
}
