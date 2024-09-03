using GalArc;
using Interface;
using System;
using System.Linq;
using Utility;

namespace ArcFormat.NitroPlus
{
    class NitroPlus_pak
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
        public static int pak_unpack(string filePath)
        {
            if (Path.GetExtension(filePath) != ".pak")
                return 1;
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            fs.Position = 4;
            int fileCount = br.ReadInt32();
            DisplayFace.displayUn(fileCount, filePath);
            fs.Position += 4;
            int comSize = br.ReadInt32();
            fs.Position = 0x114;
            byte[] comData = br.ReadBytes(comSize);
            byte[] uncomData = ZlibStream.ZlibDecompress(comData);

            MemoryStream ms = new(uncomData);
            BinaryReader brEntry = new(ms);
            int dataOffset = 276 + comSize;
            fs.Position = dataOffset;

            while (ms.Position != ms.Length)
            {
                NitroPlus_pak_entry entry = new();
                entry.pathLen = brEntry.ReadUInt32();
                entry.path = ArcEncoding.Encodings(932).GetString(brEntry.ReadBytes((int)entry.pathLen));
                entry.offset = brEntry.ReadUInt32() + (uint)dataOffset;
                entry.size = brEntry.ReadUInt32();
                entry.fullPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "\\" + entry.path;
                Directory.CreateDirectory(Path.GetDirectoryName(entry.fullPath));
                File.WriteAllBytes(entry.fullPath, br.ReadBytes((int)entry.size));
                ms.Position += 12;
                main.Main.bar.PerformStep();
            }
            fs.Close();
            ms.Close();
            return 0;
        }

        public static int pak_pack(string folderPath)
        {
            string filePath = folderPath + ".pak.new";
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fw);
            bw.Write(2);
            int fileCount = Util.GetFileCount_All(folderPath);
            bw.Write(fileCount);
            DisplayFace.displayPack(fileCount);

            string[] filePaths = new string[fileCount];
            DirectoryInfo d = new(folderPath);
            int pathLenSum = 0;
            int i = 0;
            foreach (FileInfo file in d.GetFiles("*.*", SearchOption.AllDirectories))
            {
                filePaths[i] = file.FullName.Replace(folderPath + "\\", string.Empty);
                pathLenSum += ArcEncoding.Encodings(932).GetByteCount(filePaths[i]);
                i++;
            }
            Util.InsertSort(filePaths);

            MemoryStream memoryStream = new();
            BinaryWriter bwIndex = new(memoryStream);
            int offset = 0;
            for (int j = 0; j < fileCount; j++)
            {
                string path = folderPath + "\\" + filePaths[j];
                bwIndex.Write(ArcEncoding.Encodings(932).GetByteCount(filePaths[j]));
                bwIndex.Write(ArcEncoding.Encodings(932).GetBytes(filePaths[j]));
                bwIndex.Write(offset);
                int fileSize = (int)new FileInfo(path).Length;
                bwIndex.Write(fileSize);
                bwIndex.Write(fileSize);
                bwIndex.Write((long)0);
                offset += fileSize;
            }

            byte[] uncomIndex = memoryStream.ToArray();
            bw.Write(uncomIndex.Length);
            byte[] comIndex = ZlibStream.ZlibCompress_SharpZipLib(uncomIndex);//failed on 沙耶の唄 while using .net zlib
            bw.Write(comIndex.Length);

            FileStream fs = new(folderPath + ".pak", FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            fs.Position = 16;
            byte[] reserve = br.ReadBytes(260);
            fs.Close();
            bw.Write(reserve);
            bw.Write(comIndex);

            for (int j = 0; j < fileCount; j++)
            {
                string path = folderPath + "\\" + filePaths[j];
                bw.Write(File.ReadAllBytes(path));
                main.Main.bar.PerformStep();
            }
            fw.Close();
            return 0;

        }
    }
}
