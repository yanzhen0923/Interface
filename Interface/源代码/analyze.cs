using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Runtime.InteropServices;

namespace Security_Detection_System_for_Android_Apps
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    

    public partial class MainWindow : Window
    {
        public string AnalyzeProtocol(string Range)
        {
            int i;
            for (i = Range.Length - 2; i >= 0; --i)
            {
                if (Range[i] == ':' || Range[i] == ' ')
                    break;
            }
            return (Range.Substring(i + 1, Range.Length - i - 2));
        }
        public class PublicHeader
        {
            public List<string> AnalyzedData;
            public string Protocol;
            public string AnalyzeIP(string Range)
            {
                int i;
                for (i = Range.Length - 2; i >= 0; --i)
                    if (Range[i] == '(')
                        break;
                return Range.Substring(i + 1, Range.Length - i - 2);
            }
        }

        public class IPHeader : PublicHeader
        {
            public int CurPos;
            public string Source;
            public string Destination;
            public string SourcePort;
            public string DestinationPort;
            public bool GoodChecksum;
            public bool BadChecksum;
            public void IPRead()
            {
                for (CurPos = 0; CurPos < AnalyzedData.Count; ++ CurPos)
                {
                    if (AnalyzedData[CurPos].StartsWith("    Source:"))
                    {
                        Source = AnalyzeIP(AnalyzedData[CurPos]);
                        Destination = AnalyzeIP(AnalyzedData[CurPos + 1]);
                        CurPos += 1;
                        break;
                    }
                }
                for (; CurPos < AnalyzedData.Count; ++ CurPos)
                {
                    if (AnalyzedData[CurPos].StartsWith("    Source port:"))
                    {
                        SourcePort = AnalyzeIP(AnalyzedData[CurPos]);
                        DestinationPort = AnalyzeIP(AnalyzedData[CurPos + 1]);
                        CurPos += 1;
                        break;
                    }
                }
            }
        }

        public class ARPHeader : PublicHeader
        {
            public string SenderMac;
            public string SenderIP;
            public string TargetMac;
            public string TargetIP;
            public int Option, Pos;
            int AnalyzeOption(string Range)
            {
                int i, j;
                for (j = Range.Length - 1; j > 0; -- j)
                {
                    if (Range[j] == ')')
                    {
                        for (i = j; i > 0; -- i)
                        {
                            if(Range[i] == '(')
                                return int.Parse(Range.Substring(i + 1,j - i - 1));
                        }
                    }
                }
                return -1;
            }
            public void ARPRead()
            {
                for (Pos = 0; Pos < AnalyzedData.Count; ++Pos)
                {
                    if(AnalyzedData[Pos].StartsWith("    Opcode:"))
                    {
                        Option = AnalyzeOption(AnalyzedData[Pos]);
                        SenderMac = AnalyzeIP(AnalyzedData[Pos + 1]);
                        SenderIP = AnalyzeIP(AnalyzedData[Pos + 2]);
                        TargetMac = AnalyzeIP(AnalyzedData[Pos + 3]);
                        TargetIP = AnalyzeIP(AnalyzedData[Pos + 4]);
                        break;
                    }
                }
            }

        }

        public class UDPHeader : IPHeader
        {
            public void UDPRead()
            {
                IPRead();
            }
        }

        public class TCPHeader : IPHeader
        {
            public void TCPRead()
            {
                IPRead();
            }
        }

        public class DNSHeader : UDPHeader
        {
            public void DNSRead()
            {
                UDPRead();
            }
        }

        public class HTTPHeader : TCPHeader
        {
            object type;
            string TheType;
            public void GetType()
            {
                for (int i = CurPos; i < AnalyzedData.Count; ++i)
                {

                }
            }
        }

        public class HTTPPOSTHeader : HTTPHeader
        {
            public void HTTPPOSTHeaderRead()
            {
                TCPRead();
            }
            
        }

        public class HTTPGETHeader : HTTPHeader
        {

        }

        public class HTTPCONHeader : HTTPHeader
        {

        }

        public class HTTP1_1Header : HTTPHeader
        {

        }
        
        public class SOCKSHeader : TCPHeader
        {

        }

        public class DHCPHeader : UDPHeader
        {

        }

        public class EAPOLHeader : PublicHeader
        {

        }

        public class NBNSHeader : UDPHeader
        {

        }

        Process ProcessOnCapture = new Process();
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        static extern byte MapVirtualKey(byte wCode, int wMap);

        

       
             

        public MainWindow()
        {
            InitializeComponent();
            ARPHeader a = new ARPHeader();
        }

        private void Button_StartCap_Click(object sender, RoutedEventArgs e)
        {
            ProcessOnCapture = null;
            ProcessOnCapture = new Process();
            ProcessOnCapture.StartInfo.FileName = "cmd.exe";
            ProcessOnCapture.StartInfo.UseShellExecute = false;
            ProcessOnCapture.StartInfo.RedirectStandardInput = true;
            ProcessOnCapture.StartInfo.RedirectStandardOutput = true;
            ProcessOnCapture.StartInfo.RedirectStandardError = true;

            //ProcessOnCapture.StartInfo.CreateNoWindow = true;
            ProcessOnCapture.Start();
            StreamWriter swIn = ProcessOnCapture.StandardInput;//标准输入流 
            swIn.AutoFlush = true;
            StreamReader srOut = ProcessOnCapture.StandardOutput;//标准输出流
            StreamReader srErr = ProcessOnCapture.StandardError;//标准错误流 
            swIn.Write("cd /d E:\\Program Files\\android-sdk\\platform-tools" + System.Environment.NewLine);
            swIn.Write("adb shell" + System.Environment.NewLine);

            swIn.Write("su" + System.Environment.NewLine);
            ////ProcessOnCapture.WaitForExit();
            swIn.Write("tcpdump -p -vv -s 0 -w/sdcard/2.pcap" + System.Environment.NewLine);
            Thread.Sleep(10000);
            System.Windows.Forms.SendKeys.Send("^c");
            swIn.Write("exit" + System.Environment.NewLine);
            // System.Windows.MessageBox.Show(srOut.ReadToEnd());
            //string s = srOut.ReadToEnd();
            //string err = srErr.ReadToEnd();
            //if (ProcessOnCapture.HasExited == false)
            //{
            //    ProcessOnCapture.Kill();
            //    System.Windows.MessageBox.Show(s);
            //}
            //else
            //{
            //    System.Windows.MessageBox.Show(s);
            //}
            //swIn.Close();
            //srOut.Close();
            //srErr.Close();
            //ProcessOnCapture.Close();



        }

        private void Button_StopCap_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Read_Click(object sender, RoutedEventArgs e)
        {
            string MyDosComLine1, MyDosComLine2;

            MyDosComLine1 = "cd /d E:\\Program Files\\Wireshark";
            MyDosComLine2 = "tshark -r 1.pcap -V>" + System.Environment.CurrentDirectory + "\\34.txt";

            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "cmd.exe";//打开DOS控制平台 
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.CreateNoWindow = true;//是否显示DOS窗口，true代表隐藏;
            myProcess.StartInfo.RedirectStandardInput = true;
            myProcess.StartInfo.RedirectStandardOutput = true;
            myProcess.StartInfo.RedirectStandardError = true;
            myProcess.Start();
            StreamWriter sIn = myProcess.StandardInput;//标准输入流 
            sIn.AutoFlush = true;
            StreamReader sOut = myProcess.StandardOutput;//标准输出流
            StreamReader sErr = myProcess.StandardError;//标准错误流 

            sIn.Write(MyDosComLine1 + System.Environment.NewLine);//第一条DOS命令

            sIn.Write(MyDosComLine2 + System.Environment.NewLine);//第二条DOS命令
            sIn.Write("exit" + System.Environment.NewLine);//第四条DOS命令，退出DOS窗口

            string s = sOut.ReadToEnd();//读取执行DOS命令后输出信息 
            string er = sErr.ReadToEnd();//读取执行DOS命令后错误信息 
            if (myProcess.HasExited == false)
            {
                myProcess.Kill();
                System.Windows.MessageBox.Show(er);
            }
            else
            {
                string x = s;
                System.Windows.MessageBox.Show(x);
            }
            sIn.Close();
            sOut.Close();
            sErr.Close();
            myProcess.Close();
        }

        private void Button_Struct_Click(object sender, RoutedEventArgs e)
        {
            ArrayList al = new ArrayList();
            StreamReader sr = new StreamReader(@"34.txt", Encoding.Default);
            string temp = null;
            string Protocol = null;
            //先分析协议 再新建相应的类  将协议以外的数据传进去
            List<string> PacketData= new List<string>();
            PacketData.Clear();
            while ((temp = sr.ReadLine()) != null)
            {
                if (temp.StartsWith("Frame "))
                {
                    while ((temp = sr.ReadLine()) != null)
                    {
                        if (temp.StartsWith("    [Protocols in frame:"))
                        {
                            Protocol = AnalyzeProtocol(temp);
                            break;
                        }
                    }
                    while ((temp = sr.ReadLine()) != null)
                    {
                        if (temp != "")
                            PacketData.Add(temp);
                        else
                            break;
                    }
                    if (Protocol == "tcp")
                    {
                        TCPHeader tcpheader = new TCPHeader();
                        tcpheader.AnalyzedData = new List<string>();
                        tcpheader.AnalyzedData = PacketData;
                        tcpheader.TCPRead();
                        al.Add(tcpheader);
                    }
                    else if (Protocol == "dns")
                    {
                        UDPHeader udpheader = new UDPHeader();
                        udpheader.AnalyzedData = new List<string>();
                        udpheader.AnalyzedData = PacketData;
                        udpheader.UDPRead();
                        al.Add(udpheader);
                    }
                    else if (Protocol == "arp")
                    {
                        ARPHeader arpheader = new ARPHeader();
                        arpheader.AnalyzedData = new List<string>();
                        arpheader.AnalyzedData = PacketData;
                        arpheader.ARPRead();
                        al.Add(arpheader);
                    }
                    else if (Protocol.Contains("http"))
                    {

                    }
                    PacketData.Clear();
                }
                else
                    continue;
            }
        }

        private void Button_Install_Click(object sender, RoutedEventArgs e)
        {
            string haha = "7465";
            int c;
            int.TryParse(haha,out c);
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            ARPHeader c = new ARPHeader();
            object x = (object)c;
            System.Windows.MessageBox.Show(x.GetType().ToString());
        }
    }
}
