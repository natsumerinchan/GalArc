using GalArc;
using Interface;
using System;
using System.Linq;
using System.Text;
using Utility;

namespace ArcFormat.AmuseCraft
{
    class AmuseCraft_pac
    {
        struct AmuseCraft_pac_entry
        {
            public string fileName { get; set; }
            public uint fileSize { get; set; }
            public uint offset { get; set; }
        }
        public static int pac_unpack(string filePath)
        {
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            if (Path.GetExtension(filePath) != ".pac" || Encoding.ASCII.GetString(br.ReadBytes(4)) != "PAC ")
                return 1;
            br.ReadInt32();
            uint fileCount = br.ReadUInt32();
            DisplayFace.displayUn(fileCount, filePath);
            fs.Position = 0x804;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath));
            for (int i = 0; i < fileCount; i++)
            {
                AmuseCraft_pac_entry entry = new();
                entry.fileName = ArcEncoding.Encodings(932).GetString(br.ReadBytes(32)).TrimEnd('\0');
                entry.fileSize = br.ReadUInt32();
                entry.offset = br.ReadUInt32();
                long pos = fs.Position;
                fs.Position = entry.offset;
                byte[] fileData = br.ReadBytes((int)entry.fileSize);
                File.WriteAllBytes(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "\\" + entry.fileName, fileData);
                fs.Position = pos;
                main.Main.bar.PerformStep();
            }
            return 0;
        }

        public static int pac_pack(string folderPath)
        {
            string filePath = folderPath + ".pac.new";
            uint fileCount = (uint)Util.GetFileCount_All(folderPath);
            DisplayFace.displayPack(fileCount);
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fw);
            //header
            bw.Write(Encoding.ASCII.GetBytes("PAC "));
            bw.Write(0);
            bw.Write(fileCount);
            bw.Write(new byte[532]);
            //bw.Write(fileCount);
            //for (int i = 0; i < 188; i++)
            //{
            //    bw.Write((ulong)fileCount);
            //}
            bw.Write(new byte[1508]);
            //entries
            DirectoryInfo d = new(folderPath);
            uint baseOffset = 2052 + 40 * fileCount;
            uint currentOffset = baseOffset;
            foreach (FileInfo fi in d.GetFiles())
            {
                bw.Write(ArcEncoding.Encodings(932).GetBytes(fi.Name.PadRight(32,'\0')));
                bw.Write((uint)fi.Length);
                bw.Write(currentOffset);
                currentOffset += (uint)fi.Length;
            }
            //data
            foreach (FileInfo fi in d.GetFiles())
            {
                byte[] fileData = File.ReadAllBytes(fi.FullName);
                bw.Write(fileData);
                main.Main.bar.PerformStep();
            }
            //end
            bw.Write(0);
            bw.Write(Encoding.ASCII.GetBytes("EOF "));
            fw.Close();

            return 0;
        }
    }
}
