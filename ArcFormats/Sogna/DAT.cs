using GalArc.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Extensions;

namespace ArcFormats.Sogna
{
    internal class DAT
    {
        private string Magic => "SGS.DAT 1.00";
        public class Entry
        {
            public string Name { get; set; }
            public uint PackedSize { get; set; }
            public long Offset { get; set; }
            public uint UnpackedSize { get; set; }
            public bool IsPacked { get; set; }
        }
        public void Unpack(string filePath, string folderPath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);

            if (Encoding.ASCII.GetString(br.ReadBytes(12)) != Magic)
            {
                Logger.ErrorInvalidArchive();
            }

            br.BaseStream.Position = 12;
            uint fileCount = br.ReadUInt32();
            uint indexOffset = 0x10;
            var entries = new List<Entry>((int)fileCount);
            for (int i = 0; i < fileCount; ++i)
            {
                var entry = new Entry();
                br.BaseStream.Position = indexOffset;
                entry.Name = ArcEncoding.Shift_JIS.GetString(br.ReadBytes(0x10)).Trim('\0');
                br.BaseStream.Position = indexOffset + 0x13;
                entry.IsPacked = br.ReadByte() != 0;
                br.BaseStream.Position = indexOffset + 0x14;
                entry.PackedSize = br.ReadUInt32();
                br.BaseStream.Position = indexOffset + 0x18;
                entry.UnpackedSize = br.ReadUInt32();
                br.BaseStream.Position = indexOffset + 0x1C;
                entry.Offset = br.ReadUInt32();
                entries.Add(entry);
                indexOffset += 0x20;
            }

            Logger.InitBar(fileCount);
            Directory.CreateDirectory(folderPath);
            foreach (Entry entry in entries)
            {
                br.BaseStream.Position = entry.Offset;
                string fileName = Path.Combine(folderPath, entry.Name);
                string directoryPath = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                if (entry.IsPacked)
                {
                    br.BaseStream.Position = entry.Offset;
                    byte[] compressedData = br.ReadBytes((int)entry.PackedSize);
                    byte[] uncompressedData = new byte[entry.UnpackedSize];
                    compressedData = LzUnpack(compressedData, uncompressedData);
                    File.WriteAllBytes(fileName, compressedData);
                }
                else
                {
                    byte[] data = br.ReadBytes((int)entry.UnpackedSize);
                    File.WriteAllBytes(fileName, data);
                }

            }
            Logger.Debug($"Extracted {entries.Count} files.");

            
            fs.Dispose();
            br.Dispose();
        }
        public void Pack(string folderPath, string filePath)
        {
            FileStream fw = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(fw);

            FileInfo[] files = new DirectoryInfo(folderPath).GetFiles("*", SearchOption.AllDirectories);
            Logger.InitBar(files.Length);

            bw.Write(Encoding.ASCII.GetBytes(Magic));
            bw.BaseStream.Position = 12;
            bw.Write(files.Length);

            long indexOffset = 0x10;
            long dataOffset = 0x10 + (files.Length * 0x20);

            foreach (FileInfo file in files)
            {
                string relativePath = file.FullName.Substring(folderPath.Length + 1);

                // Write index entry
                bw.BaseStream.Position = indexOffset;
                bw.WritePaddedString(relativePath, 0x10);
                bw.BaseStream.Position = indexOffset + 0x13;
                bw.Write((byte)0); // Not packed
                bw.BaseStream.Position = indexOffset + 0x14;
                bw.Write((uint)file.Length);
                bw.BaseStream.Position = indexOffset + 0x18;
                bw.Write((uint)file.Length);
                bw.BaseStream.Position = indexOffset + 0x1C;
                bw.Write((uint)dataOffset);
                indexOffset += 0x20;

                // Write file data
                bw.BaseStream.Position = dataOffset;
                byte[] fileData = File.ReadAllBytes(file.FullName);
                bw.Write(fileData);
                dataOffset += file.Length;
            }

            fw.Dispose();
            bw.Dispose();
        }

        byte[] LzUnpack(byte[] input, byte[] output)
        {
            using (var reader = new BinaryReader(new MemoryStream(input)))
            {
                int dst = 0;
                int bits = 0;
                byte mask = 0;
                while (dst < output.Length)
                {
                    mask >>= 1;
                    if (0 == mask)
                    {
                        bits = reader.ReadByte();
                        if (-1 == bits)
                            break;
                        mask = 0x80;
                    }
                    if ((mask & bits) != 0)
                    {
                        int offset = reader.ReadUInt16();
                        int count = (offset >> 12) + 1;
                        offset &= 0xFFF;
                        Binary.CopyOverlapped(output, dst - offset, dst, count);
                        dst += count;
                    }
                    else
                    {
                        output[dst++] = reader.ReadByte();
                    }

                }
                return output;
            }
        }
    }
}
