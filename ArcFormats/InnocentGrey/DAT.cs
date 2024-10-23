﻿using ArcFormats.Properties;
using Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static ArcFormats.Yuris.YPF;

namespace ArcFormats.InnocentGrey
{
    public class DAT
    {
        public static UserControl UnpackExtraOptions = IGA.UnpackExtraOptions;

        public static UserControl PackExtraOptions = IGA.PackExtraOptions;

        private static readonly string Magic = "PACKDAT.";

        private class Entry
        {
            public string FileName { get; set; }
            public uint Offset { get; set; }
            public uint FileType { get; set; }
            public uint UnpackedSize { get; set; }
            public uint PackedSize { get; set; }
            public bool IsCompressed { get; set; }
        }

        public void Unpack(string filePath, string folderPath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            if (Encoding.ASCII.GetString(br.ReadBytes(8)) != Magic)
            {
                LogUtility.ErrorInvalidArchive();
            }
            int fileCount = br.ReadInt32();
            br.BaseStream.Position += 4;

            LogUtility.InitBar(fileCount);
            List<Entry> entries = new List<Entry>();
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < fileCount; i++)
            {
                Entry entry = new Entry();
                entry.FileName = Encoding.ASCII.GetString(br.ReadBytes(32)).TrimEnd('\0');
                entry.Offset = br.ReadUInt32();
                entry.FileType = br.ReadUInt32();
                entry.UnpackedSize = br.ReadUInt32();
                entry.PackedSize = br.ReadUInt32();
                entry.IsCompressed = entry.PackedSize != entry.UnpackedSize;

                if (entry.IsCompressed)     //skip compressed data for now
                {
                    throw new NotImplementedException("Compressed data detected.Temporarily not supported.");
                }
                entries.Add(entry);
            }

            foreach (Entry entry in entries)
            {
                fs.Position = entry.Offset;
                byte[] data = br.ReadBytes((int)entry.UnpackedSize);
                if (UnpackIGAOptions.toDecryptScripts && Path.GetExtension(entry.FileName) == ".s")
                {
                    LogUtility.Debug(string.Format(Resources.logTryDecScr, entry.FileName));
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] ^= 0xFF;
                    }
                }
                File.WriteAllBytes(Path.Combine(folderPath, entry.FileName), data);
                data = null;
                LogUtility.UpdateBar();
            }
            br.Dispose();
            fs.Dispose();
        }

        public void Pack(string folderPath, string filePath)
        {
            FileStream fw = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(fw);
            DirectoryInfo d = new DirectoryInfo(folderPath);
            FileInfo[] files = d.GetFiles();
            int fileCount = files.Length;
            LogUtility.InitBar(fileCount);
            bw.Write(Encoding.ASCII.GetBytes(Magic));
            bw.Write(fileCount);
            bw.Write(fileCount);
            uint dataOffset = 16 + (uint)fileCount * 48;

            foreach (FileInfo file in files)
            {
                bw.Write(Encoding.ASCII.GetBytes(file.Name.PadRight(32, '\0')));
                bw.Write(dataOffset);
                uint size = (uint)file.Length;
                dataOffset += size;
                bw.Write(0x20000000);
                bw.Write(size);
                bw.Write(size);
            }

            foreach (FileInfo file in files)
            {
                byte[] data = File.ReadAllBytes(file.FullName);
                if (UnpackIGAOptions.toDecryptScripts && file.Extension == ".s")
                {
                    LogUtility.Debug(string.Format(Resources.logTryEncScr, file.Name));
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] ^= 0xFF;
                    }
                }
                bw.Write(data);
                data = null;
                LogUtility.UpdateBar();
            }
            bw.Dispose();
            fw.Dispose();
        }
    }
}