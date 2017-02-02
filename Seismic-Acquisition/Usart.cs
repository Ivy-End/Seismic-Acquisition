using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;

namespace Seismic_Acquisition
{
    public class Usart
    {
        private SerialPort serialPort;
        Queue<Signal> dataSet = new Queue<Signal>();
        string dataSetTmp = "";

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
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
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
            catch (Exception)
            {
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
                dataSetTmp += (char)tmp;
            }
            DataGroup();
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

        public string GetDataString()
        {
            return dataSetTmp;
        }

        

        public void DataGroup()
        {
            while (dataSetTmp.IndexOf("\r\n") != -1)
            {
                double dataNS = 0, dataEW = 0;
                string dataTmp = "";
                int pos = dataSetTmp.IndexOf("\r\n");
                if (pos != -1)
                {
                    dataTmp = dataSetTmp.Substring(0, pos);
                    dataSetTmp = dataSetTmp.Substring(pos + 1, dataSetTmp.Length - pos - 2);
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
                        Signal signal = new Signal(dataEW, dataNS);
                        dataSet.Enqueue(signal);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }

        public void ClearBuffer()
        {
            dataSetTmp = "";
        }

        public int Count()
        {
            return dataSet.Count;
        }

        public Signal Pop()
        {
            if (dataSet.Count > 0)
            {
                return dataSet.Dequeue();
            }
            else
            {
                return new Signal();
            }
        }
    }
}
