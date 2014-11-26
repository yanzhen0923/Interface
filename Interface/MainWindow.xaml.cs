using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Text;
using System.Windows.Controls.DataVisualization.Charting;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Documents;
using System.Data;
using System.Text.RegularExpressions;

namespace Interface
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        AnalyzeData ad = new AnalyzeData();
        int tp = 1400;
        StreamWriter Auto_swIn;
        StreamWriter Manual_swIn;
        int Auto_Count;
        int Manual_Count = 1;
        Process ps = new Process();
        string PackageName;
        string MainActivityName;
        int[] Judge = new int[5];
        
        public MainWindow()
        {
            InitializeComponent();
            #region 图表生成代码
            
            
            //((LineSeries)Chart111.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
            //    new KeyValuePair<int, int>(1, 320), 
            //    new KeyValuePair<int, int>(2, 610), 
            //    new KeyValuePair<int, int>(3, 900),
            //    new KeyValuePair<int, int>(4, 1230),
            //    new KeyValuePair<int, int>(5, 1500),
            //    new KeyValuePair<int, int>(6, 1750),
            //    new KeyValuePair<int, int>(7, 2010),
            //    new KeyValuePair<int, int>(8, 2400),
            //    new KeyValuePair<int, int>(9, 2789),
            //    new KeyValuePair<int, int>(10, 2987),
            //};
            //((LineSeries)Chart112.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
            //    new KeyValuePair<int, int>(1, 100), 
            //    new KeyValuePair<int, int>(2, 200), 
            //    new KeyValuePair<int, int>(3, 300),
            //    new KeyValuePair<int, int>(4, 400),
            //    new KeyValuePair<int, int>(5, 500),
            //    new KeyValuePair<int, int>(6, 600),
            //    new KeyValuePair<int, int>(7, 700),
            //    new KeyValuePair<int, int>(8, 800),
            //    new KeyValuePair<int, int>(9, 900),
            //    new KeyValuePair<int, int>(10, 1000),
            //};
            //((LineSeries)Chart112.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
            //    new KeyValuePair<int, int>(1, 800), 
            //    new KeyValuePair<int, int>(2, 810),
            //    new KeyValuePair<int, int>(3, 810),
            //    new KeyValuePair<int, int>(4, 810),
            //    new KeyValuePair<int, int>(5, 800),
            //    new KeyValuePair<int, int>(6, 820),
            //    new KeyValuePair<int, int>(7, 800),
            //    new KeyValuePair<int, int>(8, 820),
            //    new KeyValuePair<int, int>(9, 810),
            //    new KeyValuePair<int, int>(10, 810),
            //};
            //((LineSeries)Chart120.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
            //    new KeyValuePair<int, int>(1, 100), 
            //    new KeyValuePair<int, int>(2, 200), 
            //    new KeyValuePair<int, int>(3, 300),
            //    new KeyValuePair<int, int>(4, 400),
            //    new KeyValuePair<int, int>(5, 500),
            //    new KeyValuePair<int, int>(6, 600),
            //    new KeyValuePair<int, int>(7, 700),
            //    new KeyValuePair<int, int>(8, 800),
            //    new KeyValuePair<int, int>(9, 900),
            //    new KeyValuePair<int, int>(10, 1000),
            //};
            //((LineSeries)Chart120.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
            //    new KeyValuePair<int, int>(1, 80), 
            //    new KeyValuePair<int, int>(2, 160), 
            //    new KeyValuePair<int, int>(3, 260),
            //    new KeyValuePair<int, int>(4, 350),
            //    new KeyValuePair<int, int>(5, 400),
            //    new KeyValuePair<int, int>(6, 480),
            //    new KeyValuePair<int, int>(7, 590),
            //    new KeyValuePair<int, int>(8, 630),
            //    new KeyValuePair<int, int>(9, 730),
            //    new KeyValuePair<int, int>(10, 810),
            //};

            //((ColumnSeries)this.mcChart.Series[0]).ItemsSource = new KeyValuePair<string, int>[]{
            //    new KeyValuePair<string,int>("电话号码",400),
            //    new KeyValuePair<string,int>("电子邮件",399),
            //};
            #endregion
        }

        /// <summary>
        /// 将开始抓包的次序信息写入窗口
        /// </summary>
        /// <param name="count">次序编号</param>
        private void WriteLog(int count)
        {
            Dispatcher.Invoke(() =>
            {
                logBox.AppendText("\r\n正在尝试第" + count.ToString() + "次抓包" + "\r\n");
            });
        }

        /// <summary>
        /// 将抓包前的准备信息写入日志窗口
        /// </summary>
        /// <param name="log">在隐式cmd窗口截获的实时输出</param>
        private void WriteLog(string log)
        {
            Dispatcher.Invoke(() =>
            {
                if (log.Contains("pm clear ") && log.Contains("#"))
                    logBox.AppendText("已清除应用历史数据\r\n");
                else if (log.Contains("date -s") && log.Contains("#"))
                    logBox.AppendText("已重置时间\r\n");
                else if (log.Contains("contacts2.db") && log.Contains("#"))
                    logBox.AppendText("已传送联系人数据\r\n");
                else if (log.Contains("mmssms.db") && log.Contains("#"))
                    logBox.AppendText("已传送短信息数据\r\n");
                else if (log.Contains("logs.db") && log.Contains("#"))
                    logBox.AppendText("已传送通话记录数据\r\n");
                else if (log.StartsWith("Starting"))
                    logBox.AppendText("已启动应用\r\n");
                else if (log.Contains("tcpdump ") && log.Contains("#"))
                    logBox.AppendText("正在抓包\r\n");
                logBox.ScrollToEnd();
            });
        }

        /// <summary>
        /// 解析安装包报名
        /// </summary>
        /// <param name="param">需要解析的字符串</param>
        /// <returns>安装包包名</returns>
        private string AnalyzePackageName(string param)
        {
            for (int i = 15; i < param.Length; ++i)
                if (param[i] == '\'')
                    return param.Substring(15, i - 15);
            return null;

        }

        /// <summary>
        /// 解析安装包的可启动界面名称,即"launchable-activity"
        /// </summary>
        /// <param name="param">需要解析的字符串</param>
        /// <returns>可启动界面名称</returns>
        private string AnalyzeMainActivityName(string param)
        {
            for (int i = 27; i < param.Length; ++i)
                if (param[i] == '\'')
                    return param.Substring(27, i - 27);
            return null;
        }

        /// <summary>
        /// 将判断是否停止抓包的数组的内容设置成各不相同的数值
        /// </summary>
        private void SetJudge()
        {
            for (int i = 0; i < 5; ++i)
            {
                Judge[i] = i * i * i + 19;
            }
        }

       /// <summary>
       /// 判断数组中的最大值
       /// </summary>
       /// <returns>数组中的最大值</returns>
        private int GetMax()
        {
            int temp = -1;
            for (int i = 0; i < 5; ++i)
                if (Judge[i] > temp)
                    temp = Judge[i];
            return temp;
        }

        /// <summary>
        /// 判断数组中的最小值
        /// </summary>
        /// <returns>数组中的最小值</returns>
        private int GetMin()
        {
            int temp = 0x3f3f3f;
            for (int i = 0; i < 5; ++i)
                if (Judge[i] < temp)
                    temp = Judge[i];
            return temp;
        }

        /// <summary>
        /// 自动判断抓包是否停止
        /// 抓包挺住条件为:在5秒内数据包增量小于2
        /// </summary>
        /// <param name="Gots">隐式cmd窗口</param>
        private void JudgeStop(string Gots,bool IsAuto)
        {
            
            int i;
            for (i = 3; i >= 0; --i)
            {
                Judge[i + 1] = Judge[i];
            }
            Judge[0] = int.Parse(Gots.Substring(4,Gots.Length - 4));
            if((GetMax() - GetMin()) <= 2)
            {
                if (IsAuto)
                    CaptureTest();
                else
                    Manual_swIn.Write("\x3" + Environment.NewLine);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string GetContactsID(int count)
        {
            if (count <= 10)
                return "contacts2_" + count.ToString() + "00.db";
            else return "contacts2.db";
        }

        private string GetSMSID(int count)
        {
            return "mmssms.db";
        }

        private string GetCallLogID(int count)
        {
            if (count >= 21 && count <= 30)
                return "logs.db";
            return "logs.db";
        }

        private void button_start_Click_1(object sender, RoutedEventArgs e)
        {
            Auto_Count = 1;
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
                  WriteLog(em.Data);
                  if (em.Data.StartsWith("Got "))
                  {
                      JudgeStop(em.Data,true);
                  }
                 });
            };


            ProcessOnCapture.Start();
            ProcessOnCapture.BeginOutputReadLine();
            Auto_swIn = ProcessOnCapture.StandardInput;//标准输入流
            Auto_swIn.Write("cd /d D:\\Program Files\\adt-bundle\\sdk\\platform-tools" + Environment.NewLine);
            Auto_swIn.Write("adb shell" + Environment.NewLine);
            Auto_swIn.Write("su" + Environment.NewLine);
             Dispatcher.Invoke(() => {
                 logBox.AppendText("已获取最高权限\r\n");
             });
            WriteLog(1);
            if (ClearHistory.IsChecked == true)
            {
                Auto_swIn.Write("pm clear " + PackageName + Environment.NewLine);
            }

            Auto_swIn.Write("date -s \"20130527.142005\"" + Environment.NewLine);
            Auto_swIn.Write("cat sdcard/" + GetContactsID(Auto_Count) + " > data/data/com.android.providers.contacts/databases/contacts2.db" + Environment.NewLine);

            Auto_swIn.Write("cat sdcard/"+ GetCallLogID(Auto_Count) + " > data/data/com.sec.android.provider.logsprovider/databases/logs.db" + Environment.NewLine);
    
            Auto_swIn.Write("cat sdcard/" + GetSMSID(Auto_Count) + " > data/data/com.android.providers.telephony/databases/mmssms.db" + Environment.NewLine);

            Auto_swIn.Write("am start -n " + PackageName + "/" + MainActivityName + Environment.NewLine);

            SetJudge();
            Auto_swIn.Write("tcpdump -p -vv -s 0 -w/sdcard/" + Auto_Count.ToString() + ".pcap" + Environment.NewLine);
            Dispatcher.Invoke(() =>
                {
                    button_stop.IsEnabled = true;
                });
        }

        private void button_stop_Click_1(object sender, RoutedEventArgs e)
        {
            //Auto_swIn.Write("\x3" + Environment.NewLine);
            ((LineSeries)Chart211.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(0, 0), 
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
            ((LineSeries)Chart211.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
                new KeyValuePair<int, int>(0, 325), 
                new KeyValuePair<int, int>(1, 224), 
                new KeyValuePair<int, int>(2, 265), 
                new KeyValuePair<int, int>(3, 335),
                new KeyValuePair<int, int>(4, 370),
                new KeyValuePair<int, int>(5, 224),
                new KeyValuePair<int, int>(6, 236),
                new KeyValuePair<int, int>(7, 226),
                new KeyValuePair<int, int>(8, 330),
                new KeyValuePair<int, int>(9, 256),
                new KeyValuePair<int, int>(10, 268),
            };
            //((LineSeries)Chart211.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
            //    new KeyValuePair<int, int>(0, 0), 
            //    new KeyValuePair<int, int>(1, 420), 
            //    new KeyValuePair<int, int>(2, 823), 
            //    new KeyValuePair<int, int>(3, 1260),
            //    new KeyValuePair<int, int>(4, 1520),
            //    new KeyValuePair<int, int>(5, 1987),
            //    new KeyValuePair<int, int>(6, 2600),
            //    new KeyValuePair<int, int>(7, 2920),
            //    new KeyValuePair<int, int>(8, 3333),
            //    new KeyValuePair<int, int>(9, 3658),
            //    new KeyValuePair<int, int>(10, 4500),
            //};
        }

        private void SendPcapFileToPC()
        {
            //Auto_swIn.Write("exit" + Environment.NewLine);
            StreamWriter sw;
            Process ProcessOnCapture = new Process();
            ProcessOnCapture.StartInfo.FileName = "cmd.exe";
            ProcessOnCapture.StartInfo.UseShellExecute = false;
            ProcessOnCapture.StartInfo.RedirectStandardInput = true;
            ProcessOnCapture.StartInfo.RedirectStandardOutput = true;
            ProcessOnCapture.StartInfo.CreateNoWindow = true;
            ProcessOnCapture.OutputDataReceived += (s, em) =>
            {
                Dispatcher.Invoke(() =>
                {
                    textBox.AppendText(em.Data + "\r\n");
                    textBox.ScrollToEnd();
                });

            };
            ProcessOnCapture.Start();
            ProcessOnCapture.BeginOutputReadLine();
            sw = ProcessOnCapture.StandardInput;
            sw.Write("cd /d D:\\Program Files\\adt-bundle\\sdk\\platform-tools" + Environment.NewLine);
            for (int i = 1; i <= 40; ++i)
            {
                sw.Write("adb pull sdcard/" + i.ToString() + ".pcap "+ Directory.GetCurrentDirectory() + "\\" + (i - 1).ToString() + ".pcap" + Environment.NewLine);
            }
        }

        private void CaptureTest()
        {
            Auto_swIn.Write("\x3" + Environment.NewLine);
            Dispatcher.Invoke(() =>
            {
                logBox.AppendText("第" + Auto_Count.ToString() + "次抓包完成\r\n");
            });
            ++Auto_Count;
            if (Auto_Count >= 11)
            {
                return;
            }
            WriteLog(Auto_Count);
            if (progressBar.Value < progressBar.Maximum)
                progressBar.Value += 1;
            if (ClearHistory.IsChecked == true)
            {
                Auto_swIn.Write("pm clear " + PackageName + Environment.NewLine);
            }

            Auto_swIn.Write("date -s \"20130527.142005\"" + Environment.NewLine);

            Auto_swIn.Write("cat sdcard/" + GetContactsID(Auto_Count) + " > data/data/com.android.providers.contacts/databases/contacts2.db" + Environment.NewLine);

            Auto_swIn.Write("cat sdcard/" + GetCallLogID(Auto_Count) + " > data/data/com.sec.android.provider.logsprovider/databases/logs.db" + Environment.NewLine);

            Auto_swIn.Write("cat sdcard/" + GetSMSID(Auto_Count) + " > data/data/com.android.providers.telephony/databases/mmssms.db" + Environment.NewLine);

            Auto_swIn.Write("am start -n " + PackageName + "/" + MainActivityName + Environment.NewLine);

            SetJudge();

            Auto_swIn.Write("tcpdump -p -vv -s 0 -w/sdcard/" + Auto_Count.ToString() + ".pcap" + Environment.NewLine);
        }

        private void button_install_Click_1(object sender, RoutedEventArgs e)
        {

            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Filter = "Android应用程序 | *.apk";
            if (ofd.ShowDialog().Value == true)
            {
                Dispatcher.Invoke(()=>
                    {
                        logBox.AppendText("正在尝试安装测试应用 请稍候\r\n");
                    });
                Process P = new Process();
                P.StartInfo.FileName = "cmd.exe";
                P.StartInfo.UseShellExecute = false;
                P.StartInfo.RedirectStandardInput = true;
                P.StartInfo.RedirectStandardOutput = true;
                P.StartInfo.CreateNoWindow = true;
                P.OutputDataReceived += (s, em) => 
                {
                    Dispatcher.Invoke(() =>
                    {
                        textBox.AppendText(em.Data + "\r\n");
                        textBox.ScrollToEnd();
                    }); 
                    if (em.Data.StartsWith("package: name='")) 
                    { 
                        PackageName = AnalyzePackageName(em.Data);
                    }
                    else if (em.Data.StartsWith("launchable-activity: name='"))
                    {
                        MainActivityName = AnalyzeMainActivityName(em.Data);
                    }
                    else if (em.Data.Contains("Success") || em.Data.Contains("ALREADY_EXIST"))
                    {
                        Dispatcher.Invoke(() =>
                            {
                                button_start.IsEnabled = true;
                                StartSingle.IsEnabled = true;
                                logBox.AppendText("安装完成\r\n");
                            }
                        );
                    }
                    else if (em.Data.Contains("INSTALL_FAILED"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            logBox.AppendText("安装失败\r\n");
                        }
                        );
                    }
                };
                P.Start();
                P.BeginOutputReadLine();
                StreamWriter sw = P.StandardInput;
                sw.Write("cd /d D:\\Program Files\\adt-bundle\\sdk\\platform-tools" + Environment.NewLine);
                sw.Write("aapt dump badging \"" + ofd.FileName + "\"" + Environment.NewLine);
                sw.Write("adb install -r " + ofd.FileName + Environment.NewLine);
            }
        }

        private void test_Click_1(object sender, RoutedEventArgs e)
        {
            SendPcapFileToPC();
            
            Process GetLocalIP = new Process();
            StreamWriter sw1;
            StreamWriter sw2;
            GetLocalIP.StartInfo.FileName = "cmd.exe";
            GetLocalIP.StartInfo.UseShellExecute = false;
            GetLocalIP.StartInfo.RedirectStandardInput = true;
            GetLocalIP.StartInfo.RedirectStandardOutput = true;
            GetLocalIP.StartInfo.CreateNoWindow = true;
            GetLocalIP.OutputDataReceived += (s, em) =>
            {
                //MessageBox.Show(em.Data);
                if (em.Data != null && em.Data.Contains("pdp0"))
                {
                    ad.LocalIP = em.Data;
                }
            };
            GetLocalIP.Start();
            GetLocalIP.BeginOutputReadLine();
            sw1 = GetLocalIP.StandardInput;
            sw1.Write("cd /d D:\\Program Files\\adt-bundle\\sdk\\platform-tools" + Environment.NewLine);
            sw1.Write("adb shell netcfg" + Environment.NewLine);
            sw1.Write("exit" + Environment.NewLine);
            GetLocalIP.WaitForExit();

            Process PcapToPy = new Process();
            PcapToPy.StartInfo.FileName = "cmd.exe";
            PcapToPy.StartInfo.UseShellExecute = false;
            PcapToPy.StartInfo.RedirectStandardInput = true;
            PcapToPy.StartInfo.RedirectStandardOutput = true;
            PcapToPy.StartInfo.CreateNoWindow = true;
            PcapToPy.Start();
            sw2 = PcapToPy.StandardInput;
            sw2.Write("cd /d D:\\Program Files\\Wireshark" + Environment.NewLine);
            for (int q = 0; q < 10; ++q)
            {
                sw2.Write("tshark -r " + Directory.GetCurrentDirectory() + "\\" + q.ToString() + ".pcap -V > " + Directory.GetCurrentDirectory() + "\\" + q.ToString() + ".py" + Environment.NewLine);
            }
            sw2.Write("exit" + Environment.NewLine);
            PcapToPy.WaitForExit();
            ad.Run();

            Dispatcher.Invoke(() =>
            {
                ((LineSeries)Chart211.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                    new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(10,10)
                    };
                //((LineSeries)Chart212.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                //    new KeyValuePair<int, int>(1, 0),
                //    new KeyValuePair<int, int>(10,10)
                //    };
                ((LineSeries)Chart22.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                    new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(10,10)
                    };
                ((LineSeries)Chart23.Series[0]).ItemsSource = new KeyValuePair<int, int>[] { 
                    new KeyValuePair<int, int>(1, 0),
                    new KeyValuePair<int, int>(10,10)
                    };
                ((LineSeries)Chart211.Series[1]).ItemsSource = new KeyValuePair<int, int>[] { 
                    new KeyValuePair<int, int>(1, ad.Total[0] / tp), 
                    new KeyValuePair<int, int>(2, ad.Total[1] / tp),
                    new KeyValuePair<int, int>(3, ad.Total[2] / tp),
                    new KeyValuePair<int, int>(4, ad.Total[3] / tp),
                    new KeyValuePair<int, int>(5, ad.Total[4] / tp),
                    new KeyValuePair<int, int>(6, ad.Total[5] / tp),
                    new KeyValuePair<int, int>(7, ad.Total[6] / tp),
                    new KeyValuePair<int, int>(8, ad.Total[7] / tp),
                    new KeyValuePair<int, int>(9, ad.Total[8] / tp),
                    new KeyValuePair<int, int>(10, ad.Total[9] / tp),
                    };
                ((LineSeries)Chart211.Series[2]).ItemsSource = new KeyValuePair<int, double>[] {

                    new KeyValuePair<int,double>(1,ad.R + ad.K),
                    new KeyValuePair<int,double>(10,ad.R + ad.K * 10),
                    };
                ((ColumnSeries)mcChart.Series[0]).ItemsSource = new KeyValuePair<int, double>[]{
                    new KeyValuePair<int,double>(1,ad.finalres)
                };
                int hehe = 0;
                for (int t = 0; t < 10; ++t)
                {
                    
                    for (int v = 0; v < ad.PacketData[t].Count; ++v)
                    {
                        if (ad.PacketData[t][v].Aimlen > 0)
                        {
                            try
                            {
                                hza.Items.Add(new
                                {
                                    Number = t.ToString(),
                                    SubNumber = v.ToString(),
                                    StdLength = ((t + 1) * 100).ToString(),
                                    ProType = ad.PacketData[t][v].Protocol,
                                    TargetIP = ad.PacketData[t][v].TargetIP,
                                    UploadDataLength = ad.PacketData[t][v].Aimlen,
                                    //UploadDataContent = Encoding.UTF8.GetString(ad.ap[t].PacketList[ad.PacketData[t][v].id - 1].newbuffer, ad.ap[t].PacketList[ad.PacketData[t][v].id - 1].newbuffer.Length - ad.PacketData[t][v].Aimlen, ad.PacketData[t][v].Aimlen)
                                }
                                );
                            }
                            catch {
                                MessageBox.Show("zizi");
                                hehe += 1;
                            }
                        }
                    }
                }
            });
     
            MessageBox.Show("相似度为 " + ad.finalres.ToString());
        }

        private void StopSingle_Click_1(object sender, RoutedEventArgs e)
        {
            Manual_swIn.Write("\x3" + Environment.NewLine);
            Dispatcher.Invoke(() =>
                {
                    logBox.AppendText("第" + (Manual_Count - 1).ToString() + "次抓包完成" + "\r\n");
                }
            );
            return;
        }

        private void StartSingle_Click_1(object sender, RoutedEventArgs e)
        {
            if (Manual_Count == 1)
            {
                ps.StartInfo.FileName = "cmd.exe";
                ps.StartInfo.UseShellExecute = false;
                ps.StartInfo.RedirectStandardInput = true;
                ps.StartInfo.RedirectStandardOutput = true;
                ps.StartInfo.CreateNoWindow = true;
                ps.OutputDataReceived += (s, em) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        textBox.AppendText(em.Data + "\r\n");
                        textBox.ScrollToEnd();
                        WriteLog(em.Data);
                    });
                };
                ps.Start();
                ps.BeginOutputReadLine();
                Manual_swIn = ps.StandardInput;//标准输入流
                Manual_swIn.Write("cd /d D:\\Program Files\\adt-bundle\\sdk\\platform-tools" + Environment.NewLine);
                Manual_swIn.Write("adb shell" + Environment.NewLine);
                Manual_swIn.Write("su" + Environment.NewLine);
                Dispatcher.Invoke(() =>
                {
                    logBox.AppendText("已获取最高权限\r\n");
                });
                StopSingle.IsEnabled = true;
                SingleCapture();
            }
            else
            {
                SingleCapture();
            }
            
        }

        private void SingleCapture()
        {
            WriteLog(Manual_Count);
           
            if (Manual_Count >= 11)
            {
                SendPcapFileToPC();
                return;
            }
            
            if (progressBar.Value < progressBar.Maximum)
                progressBar.Value += 1;
            if (ClearHistory.IsChecked == true)
            {
                Manual_swIn.Write("pm clear " + PackageName + Environment.NewLine);
            }

            Manual_swIn.Write("date -s \"20130527.142005\"" + Environment.NewLine);

            Manual_swIn.Write("cat sdcard/" + GetContactsID(Manual_Count) + " > data/data/com.android.providers.contacts/databases/contacts2.db" + Environment.NewLine);

            Manual_swIn.Write("cat sdcard/" + GetCallLogID(Manual_Count) + " > data/data/com.sec.android.provider.logsprovider/databases/logs.db" + Environment.NewLine);

            Manual_swIn.Write("cat sdcard/" + GetSMSID(Manual_Count) + " > data/data/com.android.providers.telephony/databases/mmssms.db" + Environment.NewLine);

            Manual_swIn.Write("am start -n " + PackageName + "/" + MainActivityName + Environment.NewLine);

            Manual_swIn.Write("tcpdump -p -vv -s 0 -w/sdcard/" + Manual_Count.ToString() + ".pcap" + Environment.NewLine);

            ++Manual_Count;
            
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
                {
                    Auto.IsEnabled = true;
                    Manual.IsEnabled = false;
                });
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Auto.IsEnabled = false;
                Manual.IsEnabled = true;
            });
        }

        private void hza_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {   
            
            if (hza.SelectedItem != null)
            {
                Regex r = new Regex(@"[0-9]{1,5}");
                var tp = r.Matches(hza.SelectedItem.ToString());
                int t = int.Parse(tp[0].Value);
                int v = int.Parse(tp[1].Value);
                int x = int.Parse(tp[tp.Count - 1].Value);
                //MessageBox.Show(Encoding.UTF8.GetString(ad.ap[t].PacketList[ad.PacketData[t][v].id - 1].newbuffer,
                //    ad.ap[t].PacketList[ad.PacketData[t][v].id - 1].newbuffer.Length - ad.PacketData[t][v].Aimlen,
                //    ad.PacketData[t][v].Aimlen));
                byte[] bytes = new byte[x];
                Array.Copy(ad.ap[t].PacketList[ad.PacketData[t][v].id - 1].newbuffer,ad.ap[t].PacketList[ad.PacketData[t][v].id - 1].newbuffer.Length - x,bytes,0,x);
                DataWindow dw = new DataWindow(bytes);
                dw.Show();
            }
        }
    }
}
