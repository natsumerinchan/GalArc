﻿using Log;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utility;

namespace ArcFormats.Majiro
{
    public class ARC
    {
        private static readonly string magicV1 = "MajiroArcV1.000\x00";

        private static readonly string magicV2 = "MajiroArcV2.000\x00";

        private struct Entry
        {
            internal string name;
            internal uint dataOffset;
            internal uint size;
        }

        public static void Unpack(string filePath, string folderPath, Encoding encoding)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            string magic = Encoding.ASCII.GetString(br.ReadBytes(16));
            fs.Dispose();
            br.Dispose();
            if (magic == magicV1)
            {
                LogUtility.Info("Valid arc v1 archive detected.");
                arcV1_unpack(filePath, folderPath);
            }
            else if (magic == magicV2)
            {
                LogUtility.Info("Valid arc v2 archive detected.");
                arcV2_unpack(filePath, folderPath);
            }
            else
            {
                LogUtility.Error_NotValidArchive();
            }

        }
        private static void arcV1_unpack(string filePath, string folderPath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            fs.Position = 16;
            int fileCount = br.ReadInt32();
            uint nameOffset = br.ReadUInt32();
            uint dataOffset = br.ReadUInt32();

            uint indexLength = 8 * (uint)fileCount + 8;
            MemoryStream ms = new MemoryStream(br.ReadBytes((int)indexLength));
            BinaryReader brIndex = new BinaryReader(ms);

            List<Entry> entries = new List<Entry>();
            Directory.CreateDirectory(folderPath);
            LogUtility.InitBar(fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                Entry entry = new Entry();
                brIndex.ReadBytes(4);            //skip crc32
                entry.dataOffset = brIndex.ReadUInt32();
                entry.name = Utilities.ReadCString(br, ArcEncoding.Shift_JIS);
                entries.Add(entry);
            }
            Entry lastEntry = new Entry();
            brIndex.ReadBytes(4);            //skip crc32:0x00000000
            lastEntry.dataOffset = brIndex.ReadUInt32();
            entries.Add(lastEntry);

            for (int i = 0; i < fileCount; i++)
            {
                File.WriteAllBytes(folderPath + "\\" + entries[i].name, br.ReadBytes((int)(entries[i + 1].dataOffset - entries[i].dataOffset)));
                LogUtility.UpdateBar();
            }

            fs.Dispose();
            br.Dispose();
            brIndex.Dispose();
            ms.Dispose();
        }
        private static void arcV2_unpack(string filePath, string folderPath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            fs.Position = 16;
            int fileCount = br.ReadInt32();
            uint nameOffset = br.ReadUInt32();
            uint dataOffset = br.ReadUInt32();

            uint indexLength = 12 * (uint)fileCount;
            MemoryStream ms = new MemoryStream(br.ReadBytes((int)indexLength));
            BinaryReader brIndex = new BinaryReader(ms);

            Directory.CreateDirectory(folderPath);
            LogUtility.InitBar(fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                Entry entry = new Entry();
                brIndex.ReadBytes(4);            //skip crc32
                entry.dataOffset = brIndex.ReadUInt32();
                entry.size = brIndex.ReadUInt32();
                entry.name = Utilities.ReadCString(br, ArcEncoding.Shift_JIS);
                long pos = fs.Position;
                fs.Position = entry.dataOffset;
                File.WriteAllBytes(folderPath + "\\" + entry.name, br.ReadBytes((int)entry.size));
                fs.Position = pos;
                LogUtility.UpdateBar();
            }
            fs.Dispose();
            br.Dispose();
            brIndex.Dispose();
            ms.Dispose();
        }
        public static void Pack(string folderPath, string filePath, string version, Encoding encoding)
        {
            if (version == "1")
            {
                arcV1_pack(folderPath, filePath);
            }
            else if (version == "2")
            {
                arcV2_pack(folderPath, filePath);
            }
        }
        private static void arcV1_pack(string folderPath, string filePath)
        {
            FileStream fw = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(fw);
            int fileCount = Utilities.GetFileCount_TopOnly(folderPath);
            LogUtility.InitBar(fileCount);
            bw.Write(Encoding.ASCII.GetBytes(magicV1));
            bw.Write(fileCount);
            uint nameOffset = 28 + 8 * ((uint)fileCount + 1);
            uint dataOffset = 0;
            bw.Write(nameOffset);
            bw.Write(dataOffset);       // pos = 24

            // write name
            bw.BaseStream.Position = nameOffset;
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < fileCount; i++)
            {
                bw.Write(ArcEncoding.Shift_JIS.GetBytes(Path.GetFileName(files[i])));
                bw.Write('\0');
            }
            // write data
            dataOffset = (uint)fw.Position;
            for (int i = 0; i < fileCount; i++)
            {
                bw.Write(File.ReadAllBytes(files[i]));
            }
            uint maxOffset = (uint)fw.Position;
            // write index
            bw.BaseStream.Position = 24;
            bw.Write(dataOffset);
            for (int i = 0; i < fileCount; i++)
            {
                bw.Write(Crc32.Calculate(ArcEncoding.Shift_JIS.GetBytes(Path.GetFileName(files[i]))));
                bw.Write(dataOffset);
                dataOffset += (uint)new FileInfo(files[i]).Length;
                LogUtility.UpdateBar();
            }
            bw.Write(0);
            bw.Write(maxOffset);
            bw.Dispose();
            fw.Dispose();
        }
        private static void arcV2_pack(string folderPath, string filePath)
        {
            FileStream fw = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(fw);
            int fileCount = Utilities.GetFileCount_TopOnly(folderPath);
            LogUtility.InitBar(fileCount);
            bw.Write(Encoding.ASCII.GetBytes(magicV2));
            bw.Write(fileCount);
            uint nameOffset = 28 + 12 * (uint)fileCount;
            uint dataOffset = 0;
            bw.Write(nameOffset);
            bw.Write(dataOffset);       // pos = 24

            // write name
            bw.BaseStream.Position = nameOffset;
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < fileCount; i++)
            {
                bw.Write(ArcEncoding.Shift_JIS.GetBytes(Path.GetFileName(files[i])));
                bw.Write('\0');
            }
            // write data
            dataOffset = (uint)fw.Position;
            for (int i = 0; i < fileCount; i++)
            {
                bw.Write(File.ReadAllBytes(files[i]));
            }
            // write index
            bw.BaseStream.Position = 24;
            bw.Write(dataOffset);
            for (int i = 0; i < fileCount; i++)
            {
                bw.Write(Crc32.Calculate(ArcEncoding.Shift_JIS.GetBytes(Path.GetFileName(files[i]))));
                bw.Write(dataOffset);
                uint fileSize = (uint)new FileInfo(files[i]).Length;
                bw.Write(fileSize);
                dataOffset += fileSize;
                LogUtility.UpdateBar();
            }
            bw.Dispose();
            fw.Dispose();
        }

    }
}
