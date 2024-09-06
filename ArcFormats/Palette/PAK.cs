﻿using Log;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Utility;

namespace ArcFormats.Palette
{
    public class PAK
    {
        private static byte[] magic = { 0x05, 0x50, 0x41, 0x43, 0x4b, 0x32 };

        public static void Unpack(string filePath, string folderPath, Encoding encoding)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            if (!br.ReadBytes(6).SequenceEqual(magic))
            {
                LogUtility.Error_NotValidArchive();
            }
            int count = br.ReadInt32();
            LogUtility.InitBar(count);
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < count; i++)
            {
                byte nameLen = br.ReadByte();
                string name = ArcEncoding.Shift_JIS.GetString(Xor.xor(br.ReadBytes(nameLen), 0xff));
                int offset = br.ReadInt32();
                int size = br.ReadInt32();
                long pos = fs.Position;
                fs.Position = offset;
                File.WriteAllBytes(folderPath + "\\" + name, br.ReadBytes(size));
                fs.Position = pos;
                LogUtility.UpdateBar();
            }
            fs.Dispose();
            br.Dispose();
        }
        public static void Pack(string folderPath, string filePath, string version, Encoding encoding)
        {
            string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
            int nameLenSum = Utilities.GetNameLenSum(files, ArcEncoding.Shift_JIS);
            int fileCount = files.Length;
            int baseOffset = 10 + nameLenSum + 9 * fileCount;

            FileStream fs = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(magic);
            bw.Write(fileCount);
            LogUtility.InitBar(fileCount);

            foreach (string file in files)
            {
                byte[] name = ArcEncoding.Shift_JIS.GetBytes(Path.GetFileName(file));
                int fileSize = (int)new FileInfo(file).Length;
                bw.Write((byte)name.Length);
                bw.Write(Xor.xor(name, 0xff));
                bw.Write(baseOffset);
                baseOffset += fileSize;
                bw.Write(fileSize);
                LogUtility.UpdateBar();
            }
            foreach (string file in files)
            {
                bw.Write(File.ReadAllBytes(file));
            }
            fs.Dispose();
            bw.Dispose();
        }
    
    }
}
