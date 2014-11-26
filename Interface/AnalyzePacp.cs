using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public class AnalyzePacp
    {
        public string Path;
        public List<PacketData> PacketList;
        public AnalyzePacp(string PacpPath)
        {
            Path = PacpPath;
        }

        public class PacketData
        {
            public byte[] newbuffer;
        }

        public void Run()
        {
            PacketList = new List<PacketData>();
            FileInfo fi = new FileInfo(Path);
            long len = fi.Length;
            FileStream fs = new FileStream(Path, FileMode.Open);
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, (int)len);
            fs.Close();
            long i, length;
            for (i = 24; i < len; )
            {
                length = ((buffer[i + 14] << 16) | (buffer[i + 13] << 8) | (buffer[i + 12]));
                PacketData b = new PacketData();
                b.newbuffer = new byte[length];
                if(length < len - i - 16)
                Array.Copy(buffer, i + 16, b.newbuffer, 0, length);
                PacketList.Add(b);
                i = i + 16 + length;
            }
            return;
        }
    }
}
