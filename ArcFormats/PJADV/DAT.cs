﻿using ArcFormats.Properties;
using GalArc.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utility;
using Utility.Extensions;

namespace ArcFormats.PJADV
{
    public class DAT : ArchiveFormat
    {
        public static UserControl UnpackExtraOptions = new UnpackDATOptions();

        public static UserControl PackExtraOptions = new PackDATOptions();

        private readonly string Magic = "GAMEDAT PAC";

        public override void Unpack(string filePath, string folderPath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);

            if (Encoding.ASCII.GetString(br.ReadBytes(11)) != Magic)
            {
                Logger.ErrorInvalidArchive();
            }
            int version = br.ReadByte();
            int nameLen;
            if (version == 'K')
            {
                nameLen = 16;
                Logger.ShowVersion("dat", 1);
            }
            else if (version == '2')
            {
                nameLen = 32;
                Logger.ShowVersion("dat", 2);
            }
            else
            {
                Logger.Error(string.Format(Resources.logErrorNotSupportedVersion, "dat", version));
                return;
            }

            int count = br.ReadInt32();
            uint baseOffset = 0x10 + (uint)(count * (nameLen + 8));
            List<Entry> entries = new List<Entry>();
            Logger.InitBar(count);
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < count; i++)
            {
                Entry entry = new Entry();
                entry.Name = ArcEncoding.Shift_JIS.GetString(br.ReadBytes(nameLen)).TrimEnd('\0');
                entry.Path = Path.Combine(folderPath, entry.Name);
                entries.Add(entry);
            }
            for (int i = 0; i < count; i++)
            {
                entries[i].Offset = br.ReadUInt32() + baseOffset;
                entries[i].Size = br.ReadUInt32();
            }
            foreach (Entry entry in entries)
            {
                fs.Position = entry.Offset;
                byte[] buffer = br.ReadBytes((int)entry.Size);
                if (UnpackDATOptions.toDecryptScripts && entry.Name.Contains("textdata") && buffer.Take(5).ToArray().SequenceEqual(new byte[] { 0x95, 0x6b, 0x3c, 0x9d, 0x63 }))
                {
                    Logger.Debug(string.Format(Resources.logTryDecScr, entry.Name));
                    DecryptScript(buffer);
                }
                File.WriteAllBytes(entry.Path, buffer);
                buffer = null;
                Logger.UpdateBar();
            }
            fs.Dispose();
            br.Dispose();
        }

        public override void Pack(string folderPath, string filePath)
        {
            FileStream fs = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(Encoding.ASCII.GetBytes(Magic));
            int nameLength = ArcSettings.Version == "1" ? 16 : 32;
            bw.Write(ArcSettings.Version == "1" ? (byte)'K' : (byte)'2');
            DirectoryInfo d = new DirectoryInfo(folderPath);
            FileInfo[] files = d.GetFiles();

            bw.Write(files.Length);
            List<Entry> entries = new List<Entry>();
            Logger.InitBar(files.Length);
            uint thisOffset = 0;

            foreach (FileInfo file in files)
            {
                Entry entry = new Entry();
                entry.Name = file.Name;
                entry.Path = file.FullName;
                entry.Size = (uint)file.Length;
                entry.Offset = thisOffset;
                thisOffset += entry.Size;
                entries.Add(entry);
            }
            foreach (Entry entry in entries)
            {
                bw.WritePaddedString(entry.Name, nameLength);
            }
            foreach (Entry entry in entries)
            {
                bw.Write(entry.Offset);
                bw.Write(entry.Size);
            }
            foreach (Entry entry in entries)
            {
                byte[] buffer = File.ReadAllBytes(entry.Path);
                if (PackDATOptions.toEncryptScripts && entry.Name.Contains("textdata") && buffer.Take(5).ToArray().SequenceEqual(new byte[] { 0x95, 0x6b, 0x3c, 0x9d, 0x63 }))
                {
                    Logger.Debug(string.Format(Resources.logTryEncScr, entry.Name));
                    DecryptScript(buffer);
                }
                bw.Write(buffer);
                buffer = null;
                Logger.UpdateBar();
            }
            fs.Dispose();
            bw.Dispose();
        }

        private void DecryptScript(byte[] data)
        {
            byte key = 0xC5;
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] ^= key;
                key += 0x5C;
            }
        }
    }

    public class PAK : DAT
    {
        public static new UserControl UnpackExtraOptions = DAT.UnpackExtraOptions;

        public static new UserControl PackExtraOptions = DAT.PackExtraOptions;
    }
}
