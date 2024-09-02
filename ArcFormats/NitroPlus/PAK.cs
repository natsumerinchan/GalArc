﻿using Log;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Utility;
using Utility.Compression;

namespace ArcFormats.NitroPlus
{
    public class PAK
    {
        struct NitroPlus_pak_entry
        {
            public uint pathLen { get; set; }
            public string path { get; set; }
            public uint offset { get; set; }
            public uint size { get; set; }
            public uint reserve { get; set; }
            public string fullPath { get; set; }
        }
        public static void Unpack(string filePath, string folderPath, Encoding encoding)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Position = 4;
            int fileCount = br.ReadInt32();
            LogUtility.InitBar(fileCount);
            fs.Position += 4;
            int comSize = br.ReadInt32();
            fs.Position = 0x114;
            byte[] comData = br.ReadBytes(comSize);
            byte[] uncomData = Zlib.DecompressBytes(comData);

            MemoryStream ms = new MemoryStream(uncomData);
            BinaryReader brEntry = new BinaryReader(ms);
            int dataOffset = 276 + comSize;
            fs.Position = dataOffset;

            while (ms.Position != ms.Length)
            {
                NitroPlus_pak_entry entry = new NitroPlus_pak_entry();
                entry.pathLen = brEntry.ReadUInt32();
                entry.path = ArcEncoding.Shift_JIS.GetString(brEntry.ReadBytes((int)entry.pathLen));
                entry.offset = brEntry.ReadUInt32() + (uint)dataOffset;
                entry.size = brEntry.ReadUInt32();
                entry.fullPath = folderPath + "\\" + entry.path;
                Directory.CreateDirectory(Path.GetDirectoryName(entry.fullPath));
                File.WriteAllBytes(entry.fullPath, br.ReadBytes((int)entry.size));
                ms.Position += 12;
                LogUtility.UpdateBar();
            }
            fs.Dispose();
            ms.Dispose();
        }

        public static void Pack(string folderPath, string filePath, string version, Encoding encoding)
        {
            FileStream fw = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fw);
            bw.Write(2);
            int fileCount = Utilities.GetFileCount_All(folderPath);
            bw.Write(fileCount);
            LogUtility.InitBar(fileCount);

            string[] filePaths = new string[fileCount];
            DirectoryInfo d = new DirectoryInfo(folderPath);
            int pathLenSum = 0;
            int i = 0;
            foreach (FileInfo file in d.GetFiles("*.*", SearchOption.AllDirectories))
            {
                filePaths[i] = file.FullName.Replace(folderPath + "\\", string.Empty);
                pathLenSum += ArcEncoding.Shift_JIS.GetByteCount(filePaths[i]);
                i++;
            }
            Utilities.InsertSort(filePaths);

            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter bwIndex = new BinaryWriter(memoryStream);
            int offset = 0;
            for (int j = 0; j < fileCount; j++)
            {
                string path = folderPath + "\\" + filePaths[j];
                bwIndex.Write(ArcEncoding.Shift_JIS.GetByteCount(filePaths[j]));
                bwIndex.Write(ArcEncoding.Shift_JIS.GetBytes(filePaths[j]));
                bwIndex.Write(offset);
                int fileSize = (int)new FileInfo(path).Length;
                bwIndex.Write(fileSize);
                bwIndex.Write(fileSize);
                bwIndex.Write((long)0);
                offset += fileSize;
            }

            byte[] uncomIndex = memoryStream.ToArray();
            bw.Write(uncomIndex.Length);
            byte[] comIndex = Zlib.CompressBytes(uncomIndex);
            bw.Write(comIndex.Length);

            FileStream fs = new FileStream(folderPath + ".pak", FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Position = 16;
            byte[] reserve = br.ReadBytes(260);
            fs.Dispose();

            bw.Write(reserve);
            bw.Write(comIndex);

            for (int j = 0; j < fileCount; j++)
            {
                string path = folderPath + "\\" + filePaths[j];
                bw.Write(File.ReadAllBytes(path));
                LogUtility.UpdateBar();
            }
            fw.Dispose();
        }
    }
}
