using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Interface
{
    public partial class AnalyzeData
    {

        public AnalyzePacp[] ap = new AnalyzePacp[10];
        public List<cab>[] PacketData = new List<cab>[10];
        public int[] Total = new int[10];
        public double finalres;
        public double K;               //拟合直线的斜率
        public double R;               //拟合直线的截距
        public string LocalIP;
        //最小二乘法 10个数-----------------------------------------------------------------------------
        /// <summary>
        /// 平均和
        /// </summary>
        /// <param name="d">double数据数组</param>
        /// <param name="len">数据个数</param>
        /// <returns>平均和</returns>
        double Sum_Average(double[] d, int len)
        {
            int i = 0;
            double z = 0.0;
            for (i = 0; i < len; i++)
            {
                z += d[i];
            }
            z = z / 10;
            return z;
        }

        double X_Y_By(double[] m, double[] n, int len)
        {
            int i = 0;
            double z = 0.0;
            for (i = 0; i < len; i++)
            {
                z += m[i] * n[i];
            }
            return z;
        }

        double Squre_sum(double[] c, int len)
        {
            int i = 0;
            double z = 0.0;
            for (i = 0; i < len; i++)
            {
                z += c[i] * c[i];
            }
            return z;
        }

        /// <summary>
        /// 直线拟合函数无输入值
        /// </summary>
        /// <returns>直线拟合度</returns>
        public double LineFit()
        {

            double x_sum_average;   //数组 X[N] 个元素求和 并求平均值
            double y_sum_average;   //数组 Y[N] 个元素求和 并求平均值
            double x_square_sum;    //数组 X[N] 个个元素的平均值
            double x_multiply_y;    //数组 X[N]和Y[N]对应元素的乘
            const int sta = 1;
            const double pi = 3.141592653589;
            int i;
            double[] X = new double[10];
            double[] Y = new double[10];
            for (i = 0; i < 10; ++i)
            {
                X[i] = i + 1;
                Y[i] = Total[i] / 1448;
            }
            x_sum_average = Sum_Average(X, 10);
            y_sum_average = Sum_Average(Y, 10);
            x_square_sum = Squre_sum(X, 10);
            x_multiply_y = X_Y_By(X, Y, 10);
            K = (x_multiply_y - 10 * x_sum_average * y_sum_average) / (x_square_sum - 10 * x_sum_average * x_sum_average);
            R = y_sum_average - K * x_sum_average;
            //textBox3.AppendText(K.ToString());
            //textBox3.AppendText(R.ToString());
            if (Math.Atan(K) <= 0) return 0;
            if (K > sta) return ((1 - Math.Abs(Math.Atan(K) - Math.Atan(sta)) / (pi / 2 - Math.Atan(sta))) * 0.3 + 0.7) * 100;
            else return (1 - Math.Abs(Math.Atan(K) - Math.Atan(sta)) / Math.Atan(sta)) * 100;
        }
        //----------------------------------------------------------------------------------------------



        public string GetType(string Range)
        {
            int i;
            if (Range.Contains("http"))
            {
                return "http";
            }
            for (i = Range.Length - 2; i >= 0; --i)
            {
                if (Range[i] == ':' || Range[i] == ' ')
                    break;
            }
            return (Range.Substring(i + 1, Range.Length - i - 2));
        }

        public class cab
        {
            public int type = -1;
            public int Totallen;
            public int Aimlen = 0;
            public int Optionlen = 0;
            public int id;
            public string Protocol = null;
            public string TargetIP;
            public void Pro2type()
            {//0 for TCP,1 for HTTP,2 for SSL,-1 for others
                if (Protocol == "tcp") this.type = 0;
                else if (Protocol == "http") this.type = 1;
                else if (Protocol == "ssl") this.type = 2;
                else if (Protocol == "data") this.type = 3;
                else this.type = -1;
            }
            public void GetidTotallen(string line)
            {
                var reg = new Regex(@"[0-9]+");
                var ms = reg.Matches(line);
                this.id = int.Parse(ms[0].Value);
                this.Totallen = int.Parse(ms[1].Value);
            }
        }
        public int GetOptionLen(string line)
        {
            int len;
            var reg = new Regex(@"[0-9]+");
            var ms = reg.Match(line);
            string num = null;
            num = ms.Value;
            len = int.Parse(num);
            return len;
        }

        public int Within(string aim, List<string> a)
        {
            for (int i = 0, cou = a.Count; i < cou; ++i)
            {
                if (a[i].StartsWith(aim))
                {
                    return GetOptionLen(a[i]);
                }
            }
            return -1;
        }

        public void Run()
        {

            int finame = 0;
            for (int fi = 0; fi < 10; ++fi)
            {
                if (fi != 0) finame++;
                StreamReader sr = new StreamReader(@finame.ToString() + ".py", Encoding.Default);
                ap[fi] = new AnalyzePacp(@finame.ToString() + ".pcap");
                ap[fi].Run();

                List<string> Onecab = new List<string>();
                string trystr = null;
                int total;
                PacketData[fi] = new List<cab>();
                total = 0;
                while ((trystr = sr.ReadLine()) != null)
                {
                    Onecab.Clear();
                    Onecab.Add(trystr);
                    while ((trystr = sr.ReadLine()) != "")
                    {
                        Onecab.Add(trystr);
                    }
                    cab now = new cab();
                    if (Onecab[0].StartsWith("Frame ")) now.GetidTotallen(Onecab[0]);
                    for (int i = 0, cou = Onecab.Count; i < cou; ++i)
                    {
                        string temp = Onecab[i];
                        if (temp.StartsWith("    [Protocols in frame:"))//获取包的格式的
                        {
                            now.Protocol = GetType(temp);
                            now.Pro2type();
                            if (now.type == -1) break;
                        }

                        else if (temp.StartsWith("Internet Protocol Version 4, Src:"))//这里是获取是上行还是下行流量的需要改动
                        {
                            var reg = new Regex(@"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}");
                            var reg1 = new Regex("[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}");
                            var ms = reg.Matches(temp);
                            now.TargetIP = ms[ms.Count - 1].Value;
                            var ms1 = reg1.Matches(LocalIP);
                            //if (((int.Parse(ms[1].Value) == 192 && int.Parse(ms[2].Value) == 168) || (int.Parse(ms[1].Value) == 10 && int.Parse(ms[2].Value) == 143)) == false)
                            //{
                            //    now.type = -1;
                            //    break;
                            //}
                            if (ms[0].Value != ms1[0].Value)
                            {
                                now.type = -1;
                                break;
                            }

                            if (now.type == 0)//tcp部分
                            {
                                for (; i < cou; ++i)
                                {
                                    if (Onecab[i].StartsWith("    Options:"))
                                    {
                                        now.Optionlen = GetOptionLen(Onecab[i]);
                                    }
                                }
                            }//以上tcp部分

                            else if (now.type == 1)//http部分
                            {
                                int with = Within("    TCP segment data", Onecab);
                                if (with != -1)
                                {
                                    now.Aimlen = with;
                                }
                                else for (int j = ap[fi].PacketList[now.id - 1].newbuffer.Length - 1; j >= 3; --j)
                                    {
                                        if (ap[fi].PacketList[now.id - 1].newbuffer[j] == 10 && ap[fi].PacketList[now.id - 1].newbuffer[j - 1] == 13 && ap[fi].PacketList[now.id - 1].newbuffer[j - 2] == 10 && ap[fi].PacketList[now.id - 1].newbuffer[j - 3] == 13)
                                        {
                                            now.Aimlen = ap[fi].PacketList[now.id - 1].newbuffer.Length - j - 1;
                                            break;
                                        }
                                    }
                            }
                            else if (now.type == 2)//ssl部分
                            {
                                for (int j = cou - 1; j >= i; --j)
                                {
                                    temp = Onecab[j];
                                    if (temp.StartsWith("        Content Type:"))
                                    {
                                        if (temp.StartsWith("        Content Type: Application Data"))
                                        {
                                            for (int k = j; k < cou; ++k)
                                            {
                                                if (Onecab[k].StartsWith("        Length:"))
                                                {
                                                    now.Aimlen = GetOptionLen(Onecab[cou - 2]);
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (now.type == 3)//udp
                            {
                                for (; i < cou; ++i)
                                {
                                    if (Onecab[i].StartsWith("Data ("))
                                    {
                                        now.Aimlen = GetOptionLen(Onecab[i]);
                                    }
                                }
                            }

                        }

                    }

                    if (now.type == -1) continue;
                    if (now.type == 0) now.Aimlen = now.Totallen - now.Optionlen - 54;
                    PacketData[fi].Add(now);
                }
                total = 0;
                foreach (cab tempcab in PacketData[fi])
                {
                    total += tempcab.Aimlen;
                }

                Total[fi] = total;
            }
            finalres = LineFit();
        }
    }
}
