using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            double phaseEW = 0, phaseNS = 0;
            string buffer = "";
            string time = "";
            string filename = "";
            int length = 5;
            DateTime dateNow = DateTime.Now.AddDays(-length);
            for (int day = 1; day <= length; day++)
            {
                dateNow = dateNow.AddDays(1);
                if (!Directory.Exists("..\\..\\..\\Seismic-Acquisition\\bin\\Debug\\data\\" + dateNow.ToString("yyyy.MM.dd")))
                {
                    try
                    {
                        Directory.CreateDirectory("..\\..\\..\\Seismic-Acquisition\\bin\\Debug\\data\\" + dateNow.ToString("yyyy.MM.dd"));
                        Console.WriteLine("文件夹 data\\" + dateNow.ToString("yyyy.MM.dd") + "创建成功.");

                        for (int i = 0; i < 24; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                buffer = "";
                                filename = "";
                                filename += (i / 10).ToString();
                                filename += (i % 10).ToString();
                                filename += j.ToString() + '0';
                                if (filename == "0000")
                                {
                                    filename = "2400";
                                }

                                int H = i, M = j * 10, S = 0, MS = 0;
                                if ((M -= 10) == -10)
                                {
                                    M += 50;
                                    if (--H == -1)
                                    {
                                        H += 23;
                                    }
                                }

                                for (int k = 0; k < 10 * 60 * 2; k++)
                                {

                                    time = "";
                                    time += (H / 10).ToString();
                                    time += (H % 10).ToString();
                                    time += ':';
                                    time += (M / 10).ToString();
                                    time += (M % 10).ToString();
                                    time += ':';
                                    time += (S / 10).ToString();
                                    time += (S % 10).ToString();
                                    time += '.';
                                    time += (MS / 100).ToString();
                                    time += "00 ";

                                    if ((MS += 500) == 1000)
                                    {
                                        MS = 0;
                                        if (++S == 60)
                                        {
                                            M++;
                                            S = 0;
                                        }
                                    }

                                    buffer += time + " ";
                                    buffer += Math.Sin(Math.Cos(phaseEW += 0.3)).ToString("F3") + " ";
                                    buffer += Math.Sin(Math.Sqrt(Math.Pow(Math.Sin(phaseNS += 0.2), 2))).ToString("F3") + Environment.NewLine;
                                }

                                buffer = ZipUtil.Compress(buffer);

                                try
                                {
                                    FileStream fileStream = new FileStream("..\\..\\..\\Seismic-Acquisition\\bin\\Debug\\data\\" + dateNow.ToString("yyyy.MM.dd") + "\\" + filename + ".sdf", FileMode.Create);
                                    byte[] data = Encoding.Default.GetBytes(buffer);
                                    fileStream.Write(data, 0, data.Length);
                                    fileStream.Flush();
                                    fileStream.Close();
                                    Console.WriteLine("文件 data\\" + dateNow.ToString("yyyy.MM.dd") + "\\" + filename + ".sdf 创建成功");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }            
        }
    }
}
