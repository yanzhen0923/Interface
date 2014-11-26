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

namespace Interface
{
    /// <summary>
    /// DataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataWindow : Window
    {
        public DataWindow(byte[] bytes)
        {
            InitializeComponent();
            SetTextBox(bytes);
        }

        private void SetTextBox(byte[] bytes)
        {
            AddLineToHexTextBox("                                                     十六进制数据                                                                    字符");
            int Size = 16;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += Size) 
            {
                if (i + Size > bytes.Length) 
                    Size = bytes.Length- i;
                sb.Clear();
                sb.Append(i.ToString("X").PadLeft(8,'0') + "     ");
                for (int j = i; j < i + Size;++ j )
                {
                    sb.Append(bytes[j].ToString("X").PadLeft(2, '0').PadLeft(4,' '));
                }
                
                sb.Append("              ");
                for (int j = i; j < i + Size; ++j)
                {
                    if (bytes[j] >= 33 && bytes[j] <= 126)
                        sb.Append(((char)bytes[j]).ToString());
                    else
                        sb.Append(".");
                }
                int k = 107 - sb.Length;
                StringBuilder tp = new StringBuilder();
                for (int d = 0; d < k; ++ d)
                    tp.Append(" ");
                sb.Insert(sb.Length - 15, tp.ToString());
                StringBuilder tp0 = new StringBuilder();
                if (sb.Length < 107)
                {
                    int m = 107 - sb.Length;
                    for (int d = 0; d < m; ++d)
                        tp0.Append(" ");
                }
                sb.Insert(sb.Length - 15, tp0.ToString());
                AddLineToHexTextBox(sb.ToString());
            }
        }

        private void AddLineToHexTextBox(string str)
        {
            Dispatcher.Invoke(() =>
                {
                    HexTextbox.AppendText(str + "\r\n");
                }
            );
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Activate();
        }
    }
}
