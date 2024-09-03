using GalArc;
using Interface;
using System.Text;
using Utility;
namespace ArcFormat.InnocentGrey
{
    internal class InnocentGrey_iga
    {
        struct InnocentGrey_iga_header
        {
            public byte[] magic { get; set; }//IGA0
            public uint unknown1 { get; set; }//checksum?
            public uint unknown2 { get; set; }//2
            public uint unknown3 { get; set; }//2
        }
        struct InnocentGrey_iga_entry
        {
            public uint nameOffset { get; set; }
            public uint dataOffset { get; set; }
            public uint fileSize { get; set; }
            public string fileName { get; set; }
            public uint nameLen { get; set; }
        }

        public static int InnocentGrey_iga_unpack(string filePath)
        {

            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            List<InnocentGrey_iga_entry> entries = new();
            List<InnocentGrey_iga_entry> entriesUpdate = new();
            if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "IGA0" || Path.GetExtension(filePath) != ".iga")
                return 1;

            fs.Position = 16;
            uint indexSize = VariableLength.UintUnpack(br);

            long endPos = fs.Position + indexSize;
            while (fs.Position < endPos)
            {
                var entry = new InnocentGrey_iga_entry();
                entry.nameOffset = VariableLength.UintUnpack(br);
                entry.dataOffset = VariableLength.UintUnpack(br);
                entry.fileSize = VariableLength.UintUnpack(br);
                entries.Add(entry);
            }

            DisplayFace.displayUn(entries.Count, filePath);
            uint nameIndexSize = VariableLength.UintUnpack(br);
            long endName = fs.Position + nameIndexSize;

            string dir = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_unpacked";
            Directory.CreateDirectory(dir);

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                uint nameLenThis;
                if (i + 1 < entries.Count)
                    nameLenThis = entries[i + 1].nameOffset - entries[i].nameOffset;
                else
                    nameLenThis = nameIndexSize - entries[i].nameOffset;
                entry.fileName = VariableLength.StringUnpack(br, nameLenThis);
                entry.dataOffset += (uint)endName;
                entriesUpdate.Add(entry);
            }

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entriesUpdate[i];
                fs.Position = entry.dataOffset;
                byte[] buffer = new byte[entry.fileSize];
                br.Read(buffer, 0, (int)entry.fileSize);
                int key = Path.GetExtension(entry.fileName) == ".s" ? 0xFF : 0;//decrypt script file,详见garbro
                for (uint j = 0; j < entry.fileSize; j++)
                {
                    buffer[j] ^= (byte)((j + 2) ^ key);
                }
                FileStream fw = new(dir + "\\" + entry.fileName, FileMode.Create, FileAccess.Write);
                fw.Write(buffer);
                fw.Close();
                main.Main.bar.PerformStep();
            }
            fs.Close();
            br.Close();
            return 0;
        }
        public static int InnocentGrey_iga_pack(string folderPath)
        {
            string filePath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath).Replace("_unpacked","") + ".iga.new";
            FileStream fw = new(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fw);
            InnocentGrey_iga_header header = new();
            header.magic = Encoding.ASCII.GetBytes("IGA0");
            bw.Write(header.magic);
            bw.Write(0);//don't know accurate value,but 0 seems valid,so just set it to 0
            bw.Write(2);
            bw.Write(2);
            List<InnocentGrey_iga_entry> l = new();

            DirectoryInfo dir = new(folderPath);
            uint nameOffset = 0;
            uint dataOffset = 0;
            foreach (FileInfo file in dir.GetFiles("*.*", searchOption: SearchOption.TopDirectoryOnly))
            {
                InnocentGrey_iga_entry entry = new();
                entry.fileName = file.Name;
                entry.nameLen = (uint)entry.fileName.Length;
                entry.nameOffset = nameOffset;
                entry.dataOffset = dataOffset;
                entry.fileSize = (uint)file.Length;
                l.Add(entry);
                nameOffset += entry.nameLen;
                dataOffset += entry.fileSize;
            }

            MemoryStream msEntry = new();
            BinaryWriter bwEntry = new(msEntry);
            MemoryStream msFileName = new();
            BinaryWriter bwFileName = new(msFileName);
            int fileCount = Util.GetFileCount_All(folderPath);
            DisplayFace.displayPack(fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                bwEntry.Write(VariableLength.UintPack(l[i].nameOffset));
                bwEntry.Write(VariableLength.UintPack(l[i].dataOffset));
                bwEntry.Write(VariableLength.UintPack(l[i].fileSize));
                bwFileName.Write(VariableLength.StringPack(l[i].fileName));
            }
            bw.Write(VariableLength.UintPack((uint)msEntry.Length));
            msEntry.WriteTo(fw);

            bw.Write(VariableLength.UintPack((uint)msFileName.Length));
            msFileName.WriteTo(fw);

            for (int i = 0; i < l.Count; i++)
            {
                byte[] buffer = File.ReadAllBytes(folderPath + "\\" + l[i].fileName);
                int key = Path.GetExtension(l[i].fileName) == ".s" ? 0xFF : 0;
                for (uint j = 0; j < l[i].fileSize; j++)
                {
                    buffer[j] ^= (byte)((j + 2) ^ key);
                }
                bw.Write(buffer);
                main.Main.bar.PerformStep();
            }

            fw.Close();
            return 0;
        }
    }
}
