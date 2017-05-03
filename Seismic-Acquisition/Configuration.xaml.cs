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

namespace Seismic_Acquisition
{
    /// <summary>
    /// Configuration.xaml 的交互逻辑
    /// </summary>
    public partial class Configuration : Window
    {
        Acquisition acquistion = null;
        History history = null;
        public Configuration(Acquisition _acquisition, History _history)
        {
            InitializeComponent();
            acquistion = _acquisition;
            history = _history;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Acquire_EW_Maximum.Text = Properties.Settings.Default.Acquire_EW_Maximum.ToString();
            Acquire_EW_Minimum.Text = Properties.Settings.Default.Acquire_EW_Minimum.ToString();
            Acquire_NS_Maximum.Text = Properties.Settings.Default.Acquire_NS_Maximum.ToString();
            Acquire_NS_Minimum.Text = Properties.Settings.Default.Acquire_NS_Minimum.ToString();

            History_EW_Maximum.Text = Properties.Settings.Default.History_EW_Maximum.ToString();
            History_EW_Minimum.Text = Properties.Settings.Default.History_EW_Minimum.ToString();
            History_NS_Maximum.Text = Properties.Settings.Default.History_NS_Maximum.ToString();
            History_NS_Minimum.Text = Properties.Settings.Default.History_NS_Minimum.ToString();

            Acquire_EW_Up.Text = Properties.Settings.Default.Acquire_EW_Up.ToString();
            Acquire_EW_Down.Text = Properties.Settings.Default.Acquire_EW_Down.ToString();
            Acquire_NS_Up.Text = Properties.Settings.Default.Acquire_NS_Up.ToString();
            Acquire_NS_Down.Text = Properties.Settings.Default.Acquire_NS_Down.ToString();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (!(isInteger(Acquire_EW_Maximum.Text) && isInteger(Acquire_EW_Minimum.Text) && 
                isInteger(Acquire_NS_Maximum.Text) && isInteger(Acquire_NS_Minimum.Text) && 
                isInteger(History_EW_Maximum.Text) && isInteger(History_EW_Minimum.Text) && 
                isInteger(History_NS_Maximum.Text) && isInteger(History_NS_Minimum.Text) && 
                isDouble(Acquire_EW_Up.Text) && isDouble(Acquire_EW_Down.Text) && 
                isDouble(Acquire_NS_Up.Text) && isDouble(Acquire_NS_Down.Text)))
            {
                MessageBox.Show("请输入合法的数值。", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                Properties.Settings.Default.Acquire_EW_Maximum = Convert.ToInt32(Acquire_EW_Maximum.Text);
                Properties.Settings.Default.Acquire_EW_Minimum = Convert.ToInt32(Acquire_EW_Minimum.Text);
                Properties.Settings.Default.Acquire_NS_Maximum = Convert.ToInt32(Acquire_NS_Maximum.Text);
                Properties.Settings.Default.Acquire_NS_Minimum = Convert.ToInt32(Acquire_NS_Minimum.Text);

                Properties.Settings.Default.History_EW_Maximum = Convert.ToInt32(History_EW_Maximum.Text);
                Properties.Settings.Default.History_EW_Minimum = Convert.ToInt32(History_EW_Minimum.Text);
                Properties.Settings.Default.History_NS_Maximum = Convert.ToInt32(History_NS_Maximum.Text);
                Properties.Settings.Default.History_NS_Minimum = Convert.ToInt32(History_NS_Minimum.Text);

                Properties.Settings.Default.Acquire_EW_Up = Convert.ToDouble(Acquire_EW_Up.Text);
                Properties.Settings.Default.Acquire_EW_Down = Convert.ToDouble(Acquire_EW_Down.Text);
                Properties.Settings.Default.Acquire_NS_Up = Convert.ToDouble(Acquire_NS_Up.Text);
                Properties.Settings.Default.Acquire_NS_Down = Convert.ToDouble(Acquire_NS_Down.Text);

                Properties.Settings.Default.Save();

                if (acquistion != null)
                {
                    try
                    {
                        acquistion.LinearAxis_Acquire_EW.Maximum = Convert.ToInt32(Acquire_EW_Maximum.Text);
                        acquistion.LinearAxis_Acquire_EW.Minimum = Convert.ToInt32(Acquire_EW_Minimum.Text);
                        acquistion.LinearAxis_Acquire_NS.Maximum = Convert.ToInt32(Acquire_NS_Maximum.Text);
                        acquistion.LinearAxis_Acquire_NS.Minimum = Convert.ToInt32(Acquire_NS_Minimum.Text);

                        acquistion.acquireEWUp = Convert.ToDouble(Acquire_EW_Up.Text);
                        acquistion.acquireEWDown = Convert.ToDouble(Acquire_EW_Down.Text);
                        acquistion.acquireNSUp = Convert.ToDouble(Acquire_NS_Up.Text);
                        acquistion.acquireNSDown = Convert.ToDouble(Acquire_NS_Down.Text);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }

                if (history != null)
                {
                    try
                    {
                        history.LinearAxis_History_EW.Maximum = Convert.ToInt32(History_EW_Maximum.Text);
                        history.LinearAxis_History_EW.Minimum = Convert.ToInt32(History_EW_Minimum.Text);
                        history.LinearAxis_History_NS.Maximum = Convert.ToInt32(History_NS_Maximum.Text);
                        history.LinearAxis_History_NS.Minimum = Convert.ToInt32(History_NS_Minimum.Text);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                this.Close();
            }
        }

        private bool isInteger(string str)
        {
            int x = Convert.ToInt32(str);
            return str == x.ToString();
        }

        private bool isDouble(string str)
        {
            double x = Convert.ToDouble(str);
            return str == x.ToString();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("确认放弃修改并提出？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
