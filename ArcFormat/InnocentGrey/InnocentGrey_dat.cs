using GalArc;
using Interface;
using System;
using System.Linq;
using System.Text;
using Utility;

namespace ArcFormat.InnocentGrey
{
    class InnocentGrey_dat
    {
        struct InnocentGrey_dat_header
        {
            public string magic { get; set; }//"PACKDAT."
            public uint fileCount { get; set; }
            public uint fileCount1 { get; set; }

        }
        struct InnocentGrey_dat_entry
        {
            public string fileName { get; set; }//32 bytes
            public uint offset { get; set; }
            public uint fileType { get; set; }
            public uint fileSize_uncompressed { get; set; }
            public uint fileSize_compressed { get; set; }
            public bool isCompressed { get; set; }
        }

        public static int dat_unpack(string filePath)
        {
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            if (Encoding.ASCII.GetString(br.ReadBytes(7)) != "PACKDAT" || br.ReadByte() != '\x2e')
                return 1;
            InnocentGrey_dat_header header = new();
            header.fileCount = br.ReadUInt32();
            header.fileCount1 = br.ReadUInt32();
            if (header.fileCount != header.fileCount1)
                return 1;

            DisplayFace.displayUn(header.fileCount, filePath);
            List<InnocentGrey_dat_entry> entries = new();
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_unpacked");
            for (int i = 0; i < header.fileCount; i++)
            {
                InnocentGrey_dat_entry entry = new();
                entry.fileName = Encoding.ASCII.GetString(br.ReadBytes(32)).TrimEnd('\0');
                entry.offset = br.ReadUInt32();
                entry.fileType = br.ReadUInt32();
                entry.fileSize_uncompressed = br.ReadUInt32();
                entry.fileSize_compressed = br.ReadUInt32();
                if (entry.fileSize_compressed == entry.fileSize_uncompressed)
                    entry.isCompressed = false;
                else
                    entry.isCompressed = true;
                if (entry.isCompressed)//skip compressed data for now
                    return 3;
                entries.Add(entry);
            }

            for (int i = 0; i < header.fileCount; i++)
            {
                fs.Seek(entries[i].offset, SeekOrigin.Begin);//have to seek manually,or it will read the wrong data
                byte[] data = br.ReadBytes((int)entries[i].fileSize_uncompressed);
                byte key = (byte)(Path.GetExtension(entries[i].fileName) == ".s" ? 0xFF : 0);
                data = Xor.BytesXorByte(data, key);
                string fileName = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath)  + "_unpacked" + "\\" + entries[i].fileName;
                File.WriteAllBytes(fileName, data);
                main.Main.bar.PerformStep();
            }
            br.Close();
            fs.Close();
            return 0;
        }

        public static int dat_pack(string folderPath)
        {
            string filePath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath).Replace("_unpacked","") + ".dat.new";
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fw);
            DirectoryInfo d = new(folderPath);
            InnocentGrey_dat_header header = new();
            header.magic = "PACKDAT";
            header.fileCount = (uint)d.GetFiles("*.*", SearchOption.TopDirectoryOnly).Count();
            header.fileCount1 = header.fileCount;
            DisplayFace.displayPack(header.fileCount);
            bw.Write(Encoding.ASCII.GetBytes(header.magic));
            bw.Write('\x2e');
            bw.Write(header.fileCount);
            bw.Write(header.fileCount1);
            uint dataOffset = 16 + header.fileCount * 48;

            foreach (FileInfo file in d.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                bw.Write(Encoding.ASCII.GetBytes(file.Name.PadRight(32,'\0')));
                bw.Write(dataOffset);
                dataOffset += (uint)file.Length;
                bw.Write(0x20000000);
                bw.Write((uint)file.Length);
                bw.Write((uint)file.Length);
            }

            foreach (FileInfo file in d.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                byte[] data = File.ReadAllBytes(file.FullName);
                byte key = (byte)(Path.GetExtension(file.Name) == ".s" ? 0xFF : 0);
                data = Xor.BytesXorByte(data, key);
                bw.Write(data);
                main.Main.bar.PerformStep();
            }
            bw.Close();
            fw.Close();
            return 0;
        }

    }
}
