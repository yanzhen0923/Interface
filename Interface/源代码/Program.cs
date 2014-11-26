using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace pcapToText
{

    public class Packet
    {
        public byte[] GMTime;
        public byte[] MicroTime;
        public byte[] CapLen;
        public byte[] len;
        public byte[] PacketText;
        static Packet() { }
    }
    public class Header
    {
        public byte[] magic;
        public byte[] MajorVersion;
        public byte[] MinorVersion;
        public byte[] ThisZone;
        public byte[] SigFigs;
        public byte[] SnapLen;
        public byte[] LinkType;
        static Header() { }
    }
    class Program
    {
        static int LenthAnalyze(byte[] Data)
        {
            int lenth;
            lenth = 0;
            for (int i = 3; i >= 0; --i)
            {
                if (Data[i] != 0)
                {
                    lenth *= 256;
                    lenth += Data[i];
                }
            }
            return lenth;
        }
        static void Main(string[] args)
        {
            List<Packet> packets = new List<Packet>();
            FileInfo fi = new FileInfo("34.pcap");
            long len = fi.Length;
            FileStream fs = new FileStream("34.pcap",FileMode.Open);
            byte[] bytes = new byte[len];
            fs.Read(bytes, 0, (int)len);
            fs.Close();
            Header header = new Header();
            byte[] temp = new byte[4];
            byte[] tmp = new byte[2];
            byte[] set = new byte[4];
            byte[] st = new byte[2];

            header.magic = new byte[4];
            Array.Copy(bytes, 0, header.magic, 0, 4);

            header.MajorVersion = new byte[2];
            Array.Copy(bytes, 4, header.MajorVersion, 0, 2);

            header.MinorVersion = new byte[2];
            Array.Copy(bytes, 6, header.MinorVersion, 0, 2);

            header.ThisZone = new byte[4];
            Array.Copy(bytes, 8, header.ThisZone, 0, 4);

            header.SigFigs = new byte[4];
            Array.Copy(bytes, 12, header.SigFigs, 0, 4);

            header.SnapLen = new byte[4];
            Array.Copy(bytes, 16, header.SnapLen, 0, 4);

            header.LinkType = new byte[4];
            Array.Copy(bytes, 20, header.LinkType, 0, 4);
            for (int i = 24; i < bytes.Length; )
            {
                Packet packet = new Packet();
                byte[] t = new byte[4];

                packet.GMTime = new byte[4];
                Array.Copy(bytes, i, packet.GMTime, 0, 4);
                i += 4;

                packet.MicroTime = new byte[4];
                Array.Copy(bytes, i, packet.MicroTime, 0, 4);
                i += 4;

                packet.CapLen = new byte[4];
                Array.Copy(bytes, i, packet.CapLen, 0, 4);
                i += 4;

                packet.len = new byte[4];
                Array.Copy(bytes, i, packet.len, 0, 4);
                i += 4;

                int lth = LenthAnalyze(packet.len);
                packet.PacketText = new byte[lth];
                Array.Copy(bytes, i, packet.PacketText, 0, lth);
                i += lth;

                packets.Add(packet);
            }
            Encoding gb2312 = Encoding.GetEncoding("GBK");
           // Encoding gb2312 = Encoding.Default;
            byte[] cc = new byte[1080];
            Array.Copy(packets[182].PacketText, 0, cc, 0, 1080);
            string ccc = gb2312.GetString(cc);
            Console.Write(ccc);
            Console.ReadLine();
        }
    }
}
