using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Interface
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        StreamWriter swIn;
        DispatcherTimer timer1 = new DispatcherTimer();
        int count;
        string PackageName;
        string MainActivityName;

        public MainWindow()
        {
            InitializeComponent();
            ((LineSeries)Chart111.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(1, 100), 
                new KeyValuePair<int, int>(2, 200), 
                new KeyValuePair<int, int>(3, 300),
                new KeyValuePair<int, int>(4, 400),
                new KeyValuePair<int, int>(5, 500),
                new KeyValuePair<int, int>(6, 600),
                new KeyValuePair<int, int>(7, 700),
                new KeyValuePair<int, int>(8, 800),
                new KeyValuePair<int, int>(9, 900),
                new KeyValuePair<int, int>(10, 1000),
            };
            ((LineSeries)Chart111.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(1, 320), 
                new KeyValuePair<int, int>(2, 610), 
                new KeyValuePair<int, int>(3, 900),
                new KeyValuePair<int, int>(4, 1230),
                new KeyValuePair<int, int>(5, 1500),
                new KeyValuePair<int, int>(6, 1750),
                new KeyValuePair<int, int>(7, 2010),
                new KeyValuePair<int, int>(8, 2400),
                new KeyValuePair<int, int>(9, 2789),
                new KeyValuePair<int, int>(10, 2987),
            };
            ((LineSeries)Chart112.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(1, 100), 
                new KeyValuePair<int, int>(2, 200), 
                new KeyValuePair<int, int>(3, 300),
                new KeyValuePair<int, int>(4, 400),
                new KeyValuePair<int, int>(5, 500),
                new KeyValuePair<int, int>(6, 600),
                new KeyValuePair<int, int>(7, 700),
                new KeyValuePair<int, int>(8, 800),
                new KeyValuePair<int, int>(9, 900),
                new KeyValuePair<int, int>(10, 1000),
            };
            ((LineSeries)Chart112.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(1, 800), 
                new KeyValuePair<int, int>(2, 810),
                new KeyValuePair<int, int>(3, 810),
                new KeyValuePair<int, int>(4, 810),
                new KeyValuePair<int, int>(5, 800),
                new KeyValuePair<int, int>(6, 820),
                new KeyValuePair<int, int>(7, 800),
                new KeyValuePair<int, int>(8, 820),
                new KeyValuePair<int, int>(9, 810),
                new KeyValuePair<int, int>(10, 810),
            };
            ((LineSeries)Chart120.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(1, 100), 
                new KeyValuePair<int, int>(2, 200), 
                new KeyValuePair<int, int>(3, 300),
                new KeyValuePair<int, int>(4, 400),
                new KeyValuePair<int, int>(5, 500),
                new KeyValuePair<int, int>(6, 600),
                new KeyValuePair<int, int>(7, 700),
                new KeyValuePair<int, int>(8, 800),
                new KeyValuePair<int, int>(9, 900),
                new KeyValuePair<int, int>(10, 1000),
            };
            ((LineSeries)Chart120.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(1, 80), 
                new KeyValuePair<int, int>(2, 160), 
                new KeyValuePair<int, int>(3, 260),
                new KeyValuePair<int, int>(4, 350),
                new KeyValuePair<int, int>(5, 400),
                new KeyValuePair<int, int>(6, 480),
                new KeyValuePair<int, int>(7, 590),
                new KeyValuePair<int, int>(8, 630),
                new KeyValuePair<int, int>(9, 730),
                new KeyValuePair<int, int>(10, 810),
            };
            //((LineSeries)mcChart.Series[1]).ItemsSource = new KeyValuePair<int, double>[] { 
            //    new KeyValuePair<int, double>(1, 552), 
            //    new KeyValuePair<int, double>(2, 540), 
            //    new KeyValuePair<int, double>(3, 560),
            //    new KeyValuePair<int, double>(4, 575),
            //    new KeyValuePair<int, double>(5, 545),
            //};
            timer1.Interval = new TimeSpan(0, 0, 10);
            timer1.Tick += timer1_Tick;
        }

        private void button_start_Click_1(object sender, RoutedEventArgs e)
        {
            count = 1;
            Process ProcessOnCapture = new Process();

            ProcessOnCapture.StartInfo.FileName = "cmd.exe";
            ProcessOnCapture.StartInfo.UseShellExecute = false;
            ProcessOnCapture.StartInfo.RedirectStandardInput = true;
            ProcessOnCapture.StartInfo.RedirectStandardOutput = true;
            ProcessOnCapture.StartInfo.CreateNoWindow = true;
            ProcessOnCapture.OutputDataReceived += (s, em) => { 
                Dispatcher.Invoke(() => 
                { 
                  textBox.AppendText(em.Data + "\r\n"); 
                  textBox.ScrollToEnd(); 
                 }); 
            };
            ProcessOnCapture.Start();
            ProcessOnCapture.BeginOutputReadLine();
            swIn = ProcessOnCapture.StandardInput;//标准输入流 
            swIn.Write("cd /d E:\\Program Files\\android-sdk\\platform-tools" + Environment.NewLine);
            swIn.Write("adb shell" + Environment.NewLine);
            swIn.Write("su" + Environment.NewLine);
            WriteLog("已获取最高权限\r\n");

            WriteLog("尝试第" + count.ToString() + "次抓包");
            swIn.Write("pm clear " + PackageName + Environment.NewLine);
            WriteLog("已清除应用历史数据");

            swIn.Write("date -s \"20130527.142005\"" + Environment.NewLine);
            WriteLog("已将时间设置为20130527.142005");

            swIn.Write("cat sdcard/contacts2.db > data/data/com.android.providers.contacts/databases/contacts2.db" + Environment.NewLine);
            WriteLog("已传输第" + count.ToString() + "组联系人数据");

            swIn.Write("cat sdcard/logs.db > data/data/com.sec.android.provider.logsprovider/databases/logs.db" + Environment.NewLine);
            WriteLog("已传输第" + count.ToString() + "组通话记录数据");

            swIn.Write("cat sdcard/mmssms.db > data/data/com.android.providers.telephony/databases/mmssms.db" + Environment.NewLine);
            WriteLog("已传输第" + count.ToString() + "组短消息数据");

            WriteLog("第" + count.ToString() + "组隐私数据传输完成");

            swIn.Write("am start -n " + PackageName + "/" + MainActivityName + Environment.NewLine);
            WriteLog("已启动应用");

            swIn.Write("tcpdump -p -vv -s 0 -w/sdcard/2.pcap" + Environment.NewLine);
            WriteLog("正在进行第" + count.ToString() + "次抓包");
            timer1.Start();
        }

        private void button_stop_Click_1(object sender, RoutedEventArgs e)
        {
            timer1.Stop();
            swIn.Write("\x3" + Environment.NewLine);
            //Dispatcher.Invoke(() => { swIn.Write("\x3" + Environment.NewLine); swIn.Write("tcpdump -p -vv -s 0 -w/sdcard/2.pcap" + System.Environment.NewLine); });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            swIn.Write("\x3" + Environment.NewLine);
            WriteLog("第" + count.ToString() + "次抓包完成\r\n");
            ++count;
            WriteLog("尝试第" + count.ToString() + "次抓包");
            if (progressBar.Value < progressBar.Maximum)
                progressBar.Value += 1;
            swIn.Write("pm clear " + PackageName + Environment.NewLine);
            WriteLog("已清除应用历史数据");

            swIn.Write("date -s \"20130527.142005\"" + Environment.NewLine);
            WriteLog("已将时间设置为20130527.142005");

            swIn.Write("cat sdcard/contacts2.db > data/data/com.android.providers.contacts/databases/contacts2.db" + Environment.NewLine);
            WriteLog("已传输第" + count.ToString() + "组联系人数据");

            swIn.Write("cat sdcard/logs.db > data/data/com.sec.android.provider.logsprovider/databases/logs.db" + Environment.NewLine);
            WriteLog("已传输第" + count.ToString() + "组通话记录数据");

            swIn.Write("cat sdcard/mmssms.db > data/data/com.android.providers.telephony/databases/mmssms.db" + Environment.NewLine);
            WriteLog("已传输第" + count.ToString() + "组短消息数据");

            WriteLog("第" + count.ToString() + "组隐私数据传输完成");

            swIn.Write("am start -n " + PackageName + "/" + MainActivityName + Environment.NewLine);
            WriteLog("已启动应用");

            swIn.Write("tcpdump -p -vv -s 0 -w/sdcard/2.pcap" + Environment.NewLine);
            WriteLog("正在进行第" + count.ToString() + "次抓包\r\n");
        }

        private void WriteLog(string log)
        {
            Dispatcher.Invoke(() => { logBox.AppendText(log + "\r\n"); logBox.ScrollToEnd(); });
        }

        private void button_install_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog().Value == true)
            {
                Process P = new Process();
                P.StartInfo.FileName = "cmd.exe";
                P.StartInfo.UseShellExecute = false;
                P.StartInfo.RedirectStandardInput = true;
                P.StartInfo.RedirectStandardOutput = true;
                P.StartInfo.CreateNoWindow = true;
                P.OutputDataReceived += (s, em) => 
                { 
                    if (em.Data.StartsWith("package: name='")) 
                    { 
                        PackageName = AnalyzePackageName(em.Data); 
                    }
                    else if (em.Data.StartsWith("launchable-activity: name='"))
                    {
                        MainActivityName = AnalyzeMainActivityName(em.Data);
                    }
                };
                P.Start();
                P.BeginOutputReadLine();
                StreamWriter sw = P.StandardInput;
                sw.Write("cd /d E:\\Program Files\\android-sdk\\platform-tools" + Environment.NewLine);
                sw.Write("aapt dump badging \"" + ofd.FileName + "\"" + Environment.NewLine);
                //MessageBox.Show(sr.ReadToEnd());
            }
        }

        private string AnalyzePackageName(string param)
        {
            for (int i = 15; i < param.Length; ++ i)
                if (param[i] == '\'')
                    return param.Substring(15, i - 15);
            return null;
        }

        private string AnalyzeMainActivityName(string param)
        {
            for (int i = 27; i < param.Length; ++ i)
                if (param[i] == '\'')
                    return param.Substring(27, i - 27);
            return null;
        }
    }
}
