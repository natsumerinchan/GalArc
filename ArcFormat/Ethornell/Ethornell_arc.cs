using System;
using System.Linq;
using System.Text;

namespace ArcFormat.Ethornell
{
    internal class Ethornell_arc
    {
        struct Ethornell_arc_header
        {
            public string magic { get; set; }
            public uint fileCount { get; set; }
        }
        struct Ethornell_arc_entry
        {
            public string fileName { get; set; }
            public uint offset { get; set; }
            public uint size { get; set; }
            public ulong reserve1 { get; set; }
            public ulong reserve2 { get; set; }
            public ulong reserve3 { get; set; }
        }
        public static int arc_unpack(string filePath)
        {
            Ethornell_arc_header header = new();
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            header.magic = Encoding.ASCII.GetString(br.ReadBytes(12));
            if (header.magic != "BURIKO ARC20")
                return 1;
            header.fileCount = br.ReadUInt32();
            List<Ethornell_arc_entry> entries = new();
            uint offsetBase = 128 * header.fileCount + 16;
            for (int i = 0; i < header.fileCount; i++)
            {
                Ethornell_arc_entry entry = new();
                entry.fileName = Encoding.ASCII.GetString(br.ReadBytes(96)).TrimEnd('\0');
                entry.offset = br.ReadUInt32() + offsetBase;
                entry.size = br.ReadUInt32();
                entries.Add(entry);
            }
            return 0;
        }
    }
}
