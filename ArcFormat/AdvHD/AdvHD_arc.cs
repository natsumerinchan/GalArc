using GalArc;
using Interface;
using System.Text;
using Utility;

namespace ArcFormat.AdvHD
{
    class AdvHD_arc
    {
        struct AdvHD_arc_v1_header
        {
            public uint typeCount { get; set; }
            public uint fileCountAll { get; set; }
        }
        struct AdvHD_arc_v1_type_header
        {
            public string ext { get; set; }
            public uint fileCount { get; set; }
            public uint indexOffset { get; set; }
        }
        struct AdvHD_arc_v1_entry
        {
            public string fileName { get; set; }
            public uint fileSize { get; set; }
            public uint offset { get; set; }
            public string filePath { get; set; }
        }
        public static int arc_v1_unpack(string filePath)
        {
            AdvHD_arc_v1_header header = new();
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            header.fileCountAll = 0;
            header.typeCount = br.ReadUInt32();
            List<AdvHD_arc_v1_type_header> typeHeaders = new();
            var folderPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileName(filePath).Replace(".arc", string.Empty);
            Directory.CreateDirectory(folderPath);
            DisplayFace.displayUn((int)header.fileCountAll, filePath);

            for (int i = 0; i < header.typeCount; i++)
            {
                AdvHD_arc_v1_type_header typeHeader = new();
                typeHeader.ext = Encoding.ASCII.GetString(br.ReadBytes(3));
                br.ReadByte();
                typeHeader.fileCount = br.ReadUInt32();
                typeHeader.indexOffset = br.ReadUInt32();
                typeHeaders.Add(typeHeader);
                header.fileCountAll += typeHeader.fileCount;
            }


            for (int i = 0; i < header.typeCount; i++)
            {
                for (int j = 0; j < typeHeaders[i].fileCount; j++)
                {
                    AdvHD_arc_v1_entry index = new();
                    index.fileName = Encoding.ASCII.GetString(br.ReadBytes(13)).Replace("\0", string.Empty);

                    index.fileSize = br.ReadUInt32();
                    index.offset = br.ReadUInt32();
                    long pos = fs.Position;
                    index.filePath = folderPath + "\\" + index.fileName + "." + typeHeaders[i].ext;
                    FileStream fw = new(index.filePath, FileMode.Create, FileAccess.Write);
                    fs.Seek(index.offset, SeekOrigin.Begin);
                    byte[] buffer = br.ReadBytes((int)index.fileSize);
                    fw.Write(buffer, 0, buffer.Length);
                    fw.Close();
                    fs.Seek(pos, SeekOrigin.Begin);
                    main.Main.bar.PerformStep();
                }
            }
            fs.Close();
            return 0;
        }
        public static int arc_v1_pack(string folderPath)
        {
            HashSet<string> uniqueExtension = new();

            AdvHD_arc_v1_header header = new();
            header.fileCountAll = (uint)Util.GetFileCount_All(folderPath);
            string[] ext = Util.GetUniqueExtension(folderPath);
            Util.InsertSort(ext);
            int extCount = ext.Length;
            DisplayFace.displayPack((int)header.fileCountAll);

            header.typeCount = (uint)extCount;
            int length = 13;
            DirectoryInfo d = new(folderPath);
            List<AdvHD_arc_v1_type_header> typeHeaders = new();
            int[] type_fileCount = new int[extCount];

            FileStream fw = new(Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".arc.new", FileMode.Create, FileAccess.Write);
            MemoryStream mshead = new();
            MemoryStream mstype = new();
            MemoryStream msentry = new();
            MemoryStream msdata = new();
            BinaryWriter bwhead = new(mshead);
            BinaryWriter bwtype = new(mstype);
            BinaryWriter bwentry = new(msentry);
            BinaryWriter bwdata = new(msdata);

            bwhead.Write(header.typeCount);
            uint pos = (uint)(12 * extCount + 4);

            for (int i = 0; i < extCount; i++)
            {
                foreach (FileInfo file in d.GetFiles("*" + ext[i]))
                {
                    type_fileCount[i]++;
                    bwentry.Write(Encoding.ASCII.GetBytes(file.Name.Replace("." + ext[i], string.Empty).PadRight(length, '\0')));
                    bwentry.Write((uint)file.Length);
                    bwentry.Write((uint)(4 + 12 * header.typeCount + 21 * header.fileCountAll + msdata.Length));
                    bwdata.Write(File.ReadAllBytes(file.FullName));
                    main.Main.bar.PerformStep();
                }
                bwtype.Write(Encoding.ASCII.GetBytes(ext[i]));
                bwtype.Write((byte)0);
                bwtype.Write((uint)type_fileCount[i]);
                bwtype.Write(pos);
                pos = (uint)(12 * extCount + 4 + msentry.Length);
            }
            mshead.WriteTo(fw);
            mstype.WriteTo(fw);
            msentry.WriteTo(fw);
            msdata.WriteTo(fw);

            mshead.Dispose();
            mstype.Dispose();
            msentry.Dispose();
            msdata.Dispose();
            fw.Close();
            return 0;
        }

        struct AdvHD_arc_v2_header
        {
            public uint fileCount { get; set; }
            public uint entrySize { get; set; }
        }
        struct AdvHD_arc_v2_entry
        {
            public uint fileSize { get; set; }
            public uint offset { get; set; }
            public string fileName { get; set; }
            public string filePath { get; set; }
        }
        public static int arc_v2_unpack(string filePath)
        {
            //init
            AdvHD_arc_v2_header header = new();
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new(fs);
            List<AdvHD_arc_v2_entry> l = new();

            string folderPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileName(filePath).Replace(".arc", string.Empty);
            Directory.CreateDirectory(folderPath);
            //read file
            header.fileCount = br1.ReadUInt32();
            header.entrySize = br1.ReadUInt32();
            DisplayFace.displayUn(header.fileCount, filePath);

            for (int i = 0; i < header.fileCount; i++)
            {
                AdvHD_arc_v2_entry entry = new();
                entry.fileSize = br1.ReadUInt32();
                entry.offset = br1.ReadUInt32() + 8 + header.entrySize;
                entry.fileName = Util.ReadToUnicode(br1);
                entry.filePath = folderPath + "\\" + entry.fileName;

                l.Add(entry);
            }

            for (int i = 0; i < header.fileCount; i++)
            {
                FileStream fw = new(l[i].filePath, FileMode.Create, FileAccess.Write);
                byte[] buffer = br1.ReadBytes((int)l[i].fileSize);
                fw.Write(buffer, 0, buffer.Length);
                fw.Close();
                main.Main.bar.PerformStep();
            }
            fs.Close();
            br1.Close();
            return 0;
        }
        public static int arc_v2_pack(string folderPath)
        {
            AdvHD_arc_v2_header header = new();
            List<AdvHD_arc_v2_entry> l = new();
            uint sizeToNow = 0;
            string arcPath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".arc.new";

            //make header
            DirectoryInfo d = new(folderPath);
            header.fileCount = (uint)Util.GetFileCount_All(folderPath);
            header.entrySize = 0;
            DisplayFace.displayPack((int)header.fileCount);

            foreach (FileInfo fi in d.GetFiles())
            {
                int nameLength = fi.Name.Length;
                header.entrySize = header.entrySize + (uint)nameLength * 2 + 2 + 8;
            }

            //make entry
            foreach (FileInfo fi in d.GetFiles())
            {
                AdvHD_arc_v2_entry entry = new();
                entry.fileName = fi.Name;
                entry.fileSize = (uint)new FileInfo(fi.FullName).Length;
                entry.offset = sizeToNow;
                sizeToNow += entry.fileSize;
                l.Add(entry);
            }

            //write init
            FileStream fs = new(arcPath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new(fs);

            //write header
            bw.Write(header.fileCount);
            bw.Write(header.entrySize);

            //write entry
            foreach (var file in l)
            {
                bw.Write(file.fileSize);
                bw.Write(file.offset);
                bw.Write(Encoding.Unicode.GetBytes(file.fileName));
                bw.Write('\0');
                bw.Write('\0');
            }

            //write data
            foreach (FileInfo fi in d.GetFiles())
            {
                byte[] buffer = File.ReadAllBytes(fi.FullName);
                bw.Write(buffer);
                main.Main.bar.PerformStep();
            }
            fs.Close();
            bw.Close();
            return 0;
        }

        public static int arc_ver(string filePath)
        {
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            br.ReadBytes(6);
            char a = br.ReadChar();
            if (a >= 'A')//extension
            {
                main.Main.txtlog.AppendText("Valid AdvHD v1 file detected." + Environment.NewLine);
                return arc_v1_unpack(filePath);
            }
            else
            {
                main.Main.txtlog.AppendText("Valid AdvHD v2 file detected." + Environment.NewLine);
                return arc_v2_unpack(filePath);
            }
        }
    }
}