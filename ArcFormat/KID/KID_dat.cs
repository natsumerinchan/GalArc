using GalArc;
using Interface;
using System;
using System.Linq;
using System.Text;
using Utility;

namespace ArcFormat.KID
{
    internal class KID_dat
    {
        struct KID_dat_header
        {
            public string magic { get; set; }
            public uint fileCount { get; set; }
            public long reserve { get; set; }
        }
        struct KID_dat_entry
        {
            public uint offset { get; set; }
            public uint size { get; set; }
            public string name { get; set; }
        }

        public static int dat_unpack(string filePath)
        {
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            if (Encoding.ASCII.GetString(br.ReadBytes(4)).TrimEnd('\0') != "LNK")
                return 1;
            string folderPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
            int fileCount = br.ReadInt32();
            br.ReadBytes(8);
            uint dataOffset = 16 + 32 * (uint)fileCount;
            Directory.CreateDirectory(folderPath);
            DisplayFace.displayUn(fileCount, filePath);
            long thisPos = 0;
            for (int i = 0; i < fileCount; i++)
            {
                KID_dat_entry entry = new();
                entry.offset = br.ReadUInt32() + dataOffset;
                entry.size = br.ReadUInt32() >> 1;
                entry.name = ArcEncoding.Encodings(932).GetString(br.ReadBytes(24)).TrimEnd('\0');
                thisPos = fs.Position;
                fs.Position = entry.offset;
                byte[] data = br.ReadBytes((int)entry.size);
                File.WriteAllBytes(folderPath + "\\" + entry.name, data);
                fs.Position = thisPos;
                main.Main.bar.PerformStep();
            }
            fs.Close();
            return 0;
        }

        public static int dat_pack(string folderPath)
        {
            string filePath = folderPath + ".dat.new";
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fw);
            bw.Write(Encoding.ASCII.GetBytes("LNK"));
            bw.Write((byte)0);
            int fileCount = Util.GetFileCount_TopOnly(folderPath);
            bw.Write(fileCount);
            bw.Write((long)0);
            DirectoryInfo d = new(folderPath);
            DisplayFace.displayPack(fileCount);
            uint offset = 0;
            foreach (var file in d.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                bw.Write(offset);
                bw.Write((uint)file.Length << 1);
                bw.Write(ArcEncoding.Encodings(932).GetBytes(file.Name.PadRight(24, '\0')));
                offset += (uint)file.Length;
            }
            foreach (var file in d.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                bw.Write(File.ReadAllBytes(file.FullName));
                main.Main.bar.PerformStep();
            }
            fw.Close();
            return 0;
        }
    }
}
