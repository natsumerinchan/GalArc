using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcFormat.KaGuYa
{
    class KaGuYa_arc
    {
        struct KaGuYa_arc_header
        {
            public string magic { get; set; }
            public int version { get; set; }
            public ushort reserve { get; set; }
            public int arcNameLen { get; set; }
            public string arcName { get; set; }
        }
        struct KaGuYa_arc_entry
        {
            public uint entrySize { get; set; }
            public byte[] unknown { get; set; }
            public ushort fileNameLen { get; set; }
            public string fileName { get; set; }
            public byte[] data { get; set; }
        }
        public static int arc_unpack(string filePath)
        {
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            if (Path.GetExtension(filePath) != ".arc")                
                return 1;
            if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "LINK")
                return 1;
            if (Convert.ToChar(br.ReadByte()) != '6')
                return 1;
            fs.Position += 2;
            int len = br.ReadChar();
            fs.Position += len;

            Directory.CreateDirectory(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath));
            while(br.ReadUInt32() != 0)
            {
                fs.Position -= 4;
                KaGuYa_arc_entry entry = new();
                long pos1 = fs.Position;
                entry.entrySize = br.ReadUInt32();
                fs.Position += 9;
                entry.fileNameLen = br.ReadUInt16();
                entry.fileName = Encoding.Unicode.GetString(br.ReadBytes(entry.fileNameLen));
                long pos2 = fs.Position;
                uint fileSize = entry.entrySize - (uint)(pos2 - pos1);
                entry.data = br.ReadBytes((int)fileSize);
                File.WriteAllBytes(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "\\" + entry.fileName, entry.data);

            }
            fs.Close();
            return 0;
        }

    }
}
