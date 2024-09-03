using GalArc;
using Interface;
using System;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Utility;

namespace ArcFormat.Kirikiri
{
    internal class KiriKiri_xp3
    {
        struct KiriKiri_xp3_header
        {
            public static byte[] magic = { 0x58, 0x50, 0x33, 0x0d, 0x0a, 0x20, 0x0a, 0x1a, 0x8b, 0x67, 0x01 };
            public uint indexOffset { get; set; }
            public int reserve { get; set; }
        }
        struct KiriKiri_xp3_entry
        {
            //public uint encrypted { get; set; }
            public ulong unpacked_size { get; set; }
            public ulong packed_size { get; set; }
            public ushort pathLen { get; set; }
            public bool isCompressed { get; set; }
            public ulong dataoffset { get; set; }
            public uint checksum { get; set; }
        }

        public static int xp3_unpack(string filePath)
        {
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            if (!br.ReadBytes(11).SequenceEqual(KiriKiri_xp3_header.magic))
            {
                return 1;
            }

            uint indexOffset = br.ReadUInt32();
            fs.Position = indexOffset;
            if (br.ReadByte() == 0)     //uncompressed
            {
                long indexSize = br.ReadInt64();        //skip index size
                string magic1, magic2, magic3;
                while (fs.Position < new FileInfo(filePath).Length)
                {
                    magic1 = Encoding.ASCII.GetString(br.ReadBytes(4));     //"File"
                    long otherLen = br.ReadInt64();
                    magic2 = Encoding.ASCII.GetString(br.ReadBytes(4));     //"info"
                    br.ReadBytes(8);
                    if (br.ReadUInt32() != 0)
                    {
                        main.Main.txtlog.AppendText("Encrypted file detected.Skip…………" + Environment.NewLine);
                        br.ReadBytes((int)(otherLen - 16));
                        continue;
                    }
                    ulong fileSize = br.ReadUInt64();
                    br.ReadUInt64();
                    ushort fileNameLen = br.ReadUInt16();
                    string path = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "\\" + Encoding.Unicode.GetString(br.ReadBytes(fileNameLen * 2));
                    magic3 = Encoding.ASCII.GetString(br.ReadBytes(4));     //"segm"
                    if (magic1 != "File" || magic2 != "info" || magic3 != "segm")
                    {
                        return 1;
                    }

                    fs.Position += 12;
                    long dataOffset = br.ReadInt64();
                    long pos = fs.Position;
                    fs.Position = dataOffset;
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllBytes(path, br.ReadBytes((int)fileSize));
                    fs.Position = pos + 32;
                }
            }
            fs.Close();
            return 0;


        }
        public static int xp3_pack(string folderPath)
        {
            string filePath = folderPath + ".xp3.new";
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fw);
            bw.Write(KiriKiri_xp3_header.magic);
            bw.Write((long)0);
            int fileCount = Util.GetFileCount_All(folderPath);
            DisplayFace.displayPack(fileCount);

            long sizeToNow = 0;
            DirectoryInfo d = new(folderPath);
            MemoryStream ms = new();
            BinaryWriter bwEntry = new(ms);
            foreach (FileInfo f in d.GetFiles("*", SearchOption.AllDirectories))
            {
                byte[] compressed = ZlibStream.ZlibCompress(File.ReadAllBytes(f.FullName), CompressionLevel.Fastest);
                //byte[] compressed = File.ReadAllBytes(f.FullName);
                long compressedLen = compressed.LongLength;
                bw.Write(compressed);
                //File
                bwEntry.Write(Encoding.ASCII.GetBytes("File"));
                string thisFilePath = f.FullName.Replace(folderPath + "\\", string.Empty);
                bwEntry.Write((long)(90 + 2 * thisFilePath.Length));
                //info
                bwEntry.Write(Encoding.ASCII.GetBytes("info"));
                bwEntry.Write((long)(22 + 2 * thisFilePath.Length));
                bwEntry.Write(0);           //no crypt
                bwEntry.Write(f.Length);
                bwEntry.Write(compressedLen);
                bwEntry.Write((ushort)thisFilePath.Length);
                bwEntry.Write(Encoding.Unicode.GetBytes(thisFilePath));
                //segment
                bwEntry.Write(Encoding.ASCII.GetBytes("segm"));
                bwEntry.Write((long)0x1c);  //fixed
                bwEntry.Write(1);
                bwEntry.Write(sizeToNow + 19);
                sizeToNow += compressedLen;
                bwEntry.Write(f.Length);
                bwEntry.Write(compressedLen);
                //adler
                bwEntry.Write(Encoding.ASCII.GetBytes("adlr"));
                bwEntry.Write((long)4);     //fixed
                bwEntry.Write(Adler32.Calculate(compressed));
                main.Main.bar.PerformStep();
            }
            bw.Write((byte)0);
            long uncomLen = ms.Length;
            //byte[] compressedIndex = ZlibStream.ZlibCompress_SharpZipLib(ms.ToArray());
            //long comLen = compressedIndex.LongLength;
            //bw.Write(comLen);
            bw.Write(uncomLen);
            //bw.Write(compressedIndex);
            bw.Write(ms.ToArray());
            //index offset
            long pos = fw.Position;
            fw.Position = 11;
            bw.Write(sizeToNow + 19);
            fw.Close();
            return 0;
        }
    }
}
