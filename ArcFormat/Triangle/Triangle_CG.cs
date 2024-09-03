using GalArc;
using Interface;
using System;
using System.Linq;
using Utility;

namespace ArcFormat.Triangle
{
    class Triangle_CG
    {
        struct Triangle_CG_entry
        {
            public string name { get; set; }
            public uint offset { get; set; }
            public uint size { get; set; }
        }
        public static int CG_unpack(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            List<Triangle_CG_entry> entries = new List<Triangle_CG_entry>();
            uint fileCount = br.ReadUInt32();
            DisplayFace.displayUn(fileCount, filePath);
            string dir = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
            Directory.CreateDirectory(dir);

            for (int i = 0; i < fileCount; i++)
            {
                Triangle_CG_entry entry = new();
                entry.name = ArcEncoding.Encodings(932).GetString(br.ReadBytes(16)).TrimEnd('\0');
                //fs.Position = 4 + 20 * i + 16;
                entry.offset = br.ReadUInt32();
                entries.Add(entry);
            }

            for (int i = 0; i < entries.Count - 1; i++)
            {
                byte[] data = br.ReadBytes((int)(entries[i + 1].offset - entries[i].offset));
                string fileName = dir + "\\" + entries[i].name;
                File.WriteAllBytes(fileName, data);
                main.Main.bar.PerformStep();
            }
            byte[] dataLast = br.ReadBytes((int)(fs.Length - entries[entries.Count - 1].offset));
            string fileNameLast = dir + "\\" + entries[entries.Count - 1].name;
            File.WriteAllBytes(fileNameLast, dataLast);
            main.Main.bar.PerformStep();
            return 0;
        }

        public static int CG_pack(string folderPath)
        {
            string filePath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".CG.new";
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            DirectoryInfo dir = new(folderPath);
            uint fileCount = (uint)dir.GetFiles("*.*", SearchOption.TopDirectoryOnly).Count();
            bw.Write(fileCount);
            uint dataOffset = (uint)(4 + 20 * Util.GetFileCount_All(folderPath));
            DisplayFace.displayPack(fileCount);

            foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                bw.Write(ArcEncoding.Encodings(932).GetBytes(file.Name.PadRight(16,'\0')));
                bw.Write(dataOffset);
                dataOffset += (uint)file.Length;
            }

            foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                byte[] data = File.ReadAllBytes(file.FullName);
                bw.Write(data);
                main.Main.bar.PerformStep();
            }
            fs.Close();
            bw.Close();
            return 0;
        }
    }
}
