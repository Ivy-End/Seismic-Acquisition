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
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class History : Window
    {
        bool play = false;
        bool dateSelected = false;
        DateTime historyDate;
        public History(bool today = false)
        {
            InitializeComponent();
            if (today)
            {
                dateSelected = true;
                historyDate = DateTime.Now;

                ViewHistory();
            }
        }

        private void ViewHistory()
        {

        }

        private void toolOpen_Click(object sender, RoutedEventArgs e)
        {
            string folder = "";
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择历史文件路径";
            dialog.SelectedPath = Environment.CurrentDirectory + "\\data";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folder = dialog.SelectedPath;
                folder = folder.Substring(folder.Length - 10, 10);
                historyDate = new DateTime(Convert.ToInt32(folder.Substring(0, 4)), 
                    Convert.ToInt32(folder.Substring(5, 2)), Convert.ToInt32(folder.Substring(8, 2)));
                MessageBox.Show(historyDate.ToString());
            }

        }

        private void toolRewind_Click(object sender, RoutedEventArgs e)
        {

        }

        private void toolPrevious_Click(object sender, RoutedEventArgs e)
        {

        }

        private void toolPlay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void toolForward_Click(object sender, RoutedEventArgs e)
        {

        }

        private void toolNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void toolExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

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
    }
}
