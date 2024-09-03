using GalArc;
using Interface;
using System;
using System.Linq;
using System.Text;
using Utility;

namespace ArcFormat.Softpal
{
    class Softpal_pac
    {
        struct Softpal_pac_entry
        {
            public string fileName { get; set; }
            public uint fileSize { get; set; }
            public uint offset { get; set; }
        }
        public static int pac_unpack(string filePath)
        {
            if (Path.GetExtension(filePath) != ".pac")
                return 1;
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            int fileCount = br.ReadInt32();
            fs.Position = 0x3fe;
            List<Softpal_pac_entry> entries = new();
            DisplayFace.displayUn(fileCount, filePath);
            string folderPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < fileCount; i++)
            {
                Softpal_pac_entry entry = new();
                entry.fileName = ArcEncoding.Encodings(932).GetString(br.ReadBytes(32)).TrimEnd('\0');
                entry.fileSize = br.ReadUInt32();
                entry.offset = br.ReadUInt32();
                entries.Add(entry);
            }
            for (int i = 0; i < fileCount; i++)
            {
                byte[] data = br.ReadBytes((int)entries[i].fileSize);
                File.WriteAllBytes(folderPath + "\\" + entries[i].fileName, data);
                main.Main.bar.PerformStep();
            }
            return 0;
        }
        public static int pac_pack(string folderPath)
        {
            string filePath = folderPath + ".pac.new";
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fw);
            int fileCount = Util.GetFileCount_TopOnly(folderPath);
            DisplayFace.displayPack(fileCount);
            bw.Write(fileCount);
            bw.Write(new byte[1018]);
            DirectoryInfo d = new(folderPath);
            uint offset = 0x3fe + (uint)(40 * fileCount);
            foreach (var file in d.GetFiles("*.*", System.IO.SearchOption.TopDirectoryOnly))
            {
                bw.Write(ArcEncoding.Encodings(932).GetBytes(file.Name.PadRight(32,'\0')));
                bw.Write((int)file.Length);
                bw.Write(offset);
                offset += (uint)file.Length;
            }
            foreach (var file in d.GetFiles("*.*", System.IO.SearchOption.TopDirectoryOnly))
            {
                bw.Write(File.ReadAllBytes(file.FullName));
            }
            bw.Write(Encoding.ASCII.GetBytes("EOF "));
            fw.Close();
            return 0;
        }
    }
}
