using GalArc;
using Interface;
using System;
using System.Linq;
using Utility;

namespace ArcFormat.NScripter
{
    class NScripter_ns2
    {
        struct NScripter_ns2_entry
        {
            public string filePath { get; set; }
            public uint fileSize { get; set; }
            public string filePathDivided { get; set; }
        }

        public static int ns2_unpack(string filePath)
        {
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int dataOffset = br.ReadInt32();
            if (dataOffset > new FileInfo(filePath).Length || dataOffset <= 0)
                return 2;
            string dir = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
            Directory.CreateDirectory(dir);
            List<NScripter_ns2_entry> entries = new();

            while (fs.Position < dataOffset - 1)//dataOffset - 1 is the end of file path
            {
                NScripter_ns2_entry entry = new();
                br.ReadByte();//skip "
                entry.filePath = dir + "\\" + ArcEncoding.Encodings(932).GetString(Util.ReadToAnsi(br, 0x22));
                entry.fileSize = br.ReadUInt32();
                entries.Add(entry);
            }
            br.ReadByte(); //skip e

            DisplayFace.displayUn(entries.Count, filePath);

            foreach (NScripter_ns2_entry entry in entries)
            {
                byte[] data = br.ReadBytes((int)entry.fileSize);
                Directory.CreateDirectory(Path.GetDirectoryName(entry.filePath));
                File.WriteAllBytes(entry.filePath, data);
                main.Main.bar.PerformStep();
            }
            fs.Close();
            br.Close();
            return 0;
        }

        public static int ns2_pack(string folderPath)
        {
            int fileCount = Util.GetFileCount_All(folderPath);
            DisplayFace.displayPack(fileCount);
            uint dataOffset = 4;
            string filePath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".ns2.new";
            string[] pathString = new string[fileCount];
            DirectoryInfo d = new(folderPath);
            int i = 0;
            foreach (FileInfo file in d.GetFiles("*.*", SearchOption.AllDirectories))
            {
                pathString[i] = file.FullName.Replace(folderPath + "\\", string.Empty);
                i++;
            }
            Util.InsertSort(pathString);//cannot use getfiles,like pf8

            List<NScripter_ns2_entry> entries = new();

            for (int j = 0; j < fileCount; j++)
            {
                NScripter_ns2_entry entry = new();
                entry.filePathDivided = pathString[j];
                entry.filePath = folderPath + "\\" + pathString[j];
                entry.fileSize = (uint)new FileInfo(entry.filePath).Length;
                dataOffset += (uint)(ArcEncoding.Encodings(932).GetBytes(entry.filePathDivided).Length + 2);//to avoid japanese character length error
                dataOffset += 4;
                entries.Add(entry);
            }
            dataOffset += 1;//'e'
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fw);
            bw.Write(dataOffset);
            for (int j = 0; j < fileCount; j++)
            {
                bw.Write('\"');
                bw.Write(ArcEncoding.Encodings(932).GetBytes(entries[j].filePathDivided));
                bw.Write('\"');
                bw.Write(entries[j].fileSize);
            }
            bw.Write('e');

            for (int j = 0; j < fileCount; j++)
            {
                byte[] buffer = File.ReadAllBytes(entries[j].filePath);
                bw.Write(buffer);
                main.Main.bar.PerformStep();
            }
            fw.Close();
            bw.Close();
            return 0;
        }
    }
}
