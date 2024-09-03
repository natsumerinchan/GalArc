using GalArc;
using Interface;
using System.Text;
using Utility;

namespace ArcFormat.AdvHD
{
    class AdvHD_pna
    {
        struct AdvHD_pna_header
        {
            public string magic { get; set; }
            public uint unknown1 { get; set; }
            public uint width { get; set; }
            public uint height { get; set; }
            public uint fileCount { get; set; }
        }
        struct AdvHD_pna_entry
        {
            public uint fileType { get; set; }
            public uint fileNumber { get; set; }
            public uint offsetX { get; set; }
            public uint offsetY { get; set; }
            public uint width { get; set; }
            public uint height { get; set; }
            public uint add1 { get; set; }
            public uint add2 { get; set; }
            public uint remark3 { get; set; }
            public uint fileSize { get; set; }
        }
        public static int pna_unpack(string filePath)
        {
            AdvHD_pna_header header = new();
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);

            header.magic = Encoding.UTF8.GetString(br.ReadBytes(4));
            if (header.magic != "PNAP" || Path.GetExtension(filePath) != ".pna")
                return 1;
            header.unknown1 = br.ReadUInt32();
            header.width = br.ReadUInt32();
            header.height = br.ReadUInt32();
            header.fileCount = br.ReadUInt32();
            DisplayFace.displayUn((int)header.fileCount, filePath);

            string folderPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);
            Directory.CreateDirectory(folderPath);
            List<AdvHD_pna_entry> l = new();

            for (int i = 0; i < header.fileCount; i++)
            {
                AdvHD_pna_entry entry = new();
                entry.fileType = br.ReadUInt32();
                entry.fileNumber = header.fileCount - br.ReadUInt32();
                entry.offsetX = br.ReadUInt32();
                entry.offsetY = br.ReadUInt32();
                entry.width = br.ReadUInt32();
                entry.height = br.ReadUInt32();
                entry.add1 = br.ReadUInt32();
                entry.add2 = br.ReadUInt32();
                entry.remark3 = br.ReadUInt32();
                entry.fileSize = br.ReadUInt32();
                l.Add(entry);
            }
            int validCount = 0;
            for (int i = 0; i < header.fileCount; i++)
            {
                main.Main.bar.PerformStep();
                if (l[i].fileType == 1 || l[i].fileType == 2)
                    continue;
                FileStream fw = new(folderPath + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_" + l[i].fileNumber.ToString("000") + ".png", FileMode.Create, FileAccess.Write);
                byte[] buffer = br.ReadBytes((int)l[i].fileSize);
                fw.Write(buffer, 0, buffer.Length);
                fw.Close();
                validCount++;

            }

            fs.Close();
            br.Close();
            main.Main.txtlog.AppendText(validCount.ToString() + " among them appears valid." + Environment.NewLine);
            return 0;
        }
        public static int pna_pack(string folderPath)
        {
            string spath = folderPath + ".pna";
            string tpath = spath + ".new";
            if (!File.Exists(spath))
                return 1;
            File.Copy(spath, tpath, true);

            int fileCount = Util.GetFileCount_All(folderPath);
            DisplayFace.displayPack(fileCount);

            FileStream fs = new(tpath, FileMode.Open, FileAccess.ReadWrite);
            BinaryWriter bw = new(fs);
            BinaryReader br = new(fs);
            fs.Seek(16, SeekOrigin.Begin);
            uint fileCountWithInvalid = br.ReadUInt32();

            //string[] pathString = new string[fileCountWithInvalid];
            //DirectoryInfo d = new(folderPath);
            //FileInfo[] arrFi = d.GetFiles("*.*", SearchOption.AllDirectories);
            //for (int i = 0; i < arrFi.Length; i++)
            //{
            //    pathString[i] = arrFi[i].FullName;
            //}

            DirectoryInfo dir = new DirectoryInfo(spath);
            foreach (FileInfo fi in dir.GetFiles())
            {

                while (br.ReadUInt32() != 0)
                {
                    fs.Seek(-4 + 40, SeekOrigin.Current);
                }
                fs.Seek(-4 + 36, SeekOrigin.Current);
                bw.Write((uint)new FileInfo(fi.FullName).Length);
                //fs.Seek(4, SeekOrigin.Current);
            }
            fs.Seek(20 + 40 * fileCountWithInvalid, SeekOrigin.Begin);

            foreach (FileInfo fi in dir.GetFiles())
            {
                byte[] buffer = File.ReadAllBytes(fi.FullName);
                bw.Write(buffer);
                main.Main.bar.PerformStep();
            }

            fs.Close();
            bw.Close();
            br.Close();
            return 0;
        }
    }
}
