using GalArc;
using Interface;
using Utility;

namespace ArcFormat.NextonLikeC
{
    internal class NextonLikeC_lst
    {
        struct NextonLikeC_lst_entry
        {
            public uint fileOffset { get; set; }
            public uint fileSize { get; set; }
            public string fileName { get; set; }
            public int fileType { get; set; }
            //1:script 2,3:image 4,5:audio
            //1:SNX 3:PNG 4,5:OGG
        }
        public static int NextonLikeC_lst_unpack(string filePath)
        {
            string arcPath;
            string lstPath;

            //judge
            if (Path.GetExtension(filePath) == string.Empty)
            {
                arcPath = filePath;
                lstPath = filePath + ".lst";
                if (!File.Exists(lstPath))
                    return 3;
            }
            else if (Path.GetExtension(filePath) == ".lst")
            {
                lstPath = filePath;
                arcPath = Path.ChangeExtension(filePath,"");
                if (!File.Exists(arcPath))
                    return 3;
            }
            else
            {
                return 1;
            }

            FileStream fsLst = new(lstPath, FileMode.Open, FileAccess.Read);
            BinaryReader brtemp = new BinaryReader(fsLst);
            fsLst.Position = 3;
            byte keyLst = brtemp.ReadByte();
            fsLst.Dispose();
            brtemp.Dispose();

            byte[] lst = Xor.BytesXorByte(File.ReadAllBytes(lstPath), keyLst);
            MemoryStream ms = new MemoryStream(lst);
            BinaryReader brLst = new(ms);

            uint fileCount = brLst.ReadUInt32();
            //main.Main.txtlog.AppendText(fileCount.ToString());
            List<NextonLikeC_lst_entry> l = new();
            DisplayFace.displayUn(fileCount, filePath);

            for (int i = 0; i < (int)fileCount; i++)
            {
                NextonLikeC_lst_entry entry = new();
                entry.fileOffset = brLst.ReadUInt32();
                entry.fileSize = brLst.ReadUInt32();
                entry.fileName = ArcEncoding.Encodings(932).GetString(brLst.ReadBytes(64)).TrimEnd('\x02');
                entry.fileType = Xor.ByteXorByte(brLst.ReadByte(), keyLst);//only read one byte to convert to int
                brLst.ReadBytes(3);//000000
                l.Add(entry);
            }
            string dir = Path.GetDirectoryName(arcPath) + "\\" + Path.GetFileNameWithoutExtension(arcPath) + "_unpack";
            Directory.CreateDirectory(dir);

            FileStream fsArc = new(arcPath, FileMode.Open, FileAccess.Read);
            BinaryReader brArc = new(fsArc);
            fsArc.Position = 3;
            byte keyArc = brArc.ReadByte();
            fsArc.Position = 0;
            for (int i = 0; i < (int)fileCount; i++)
            {
                switch (l[i].fileType)
                {
                    case 1://script
                        byte[] bufferSCR = Xor.BytesXorByte(brArc.ReadBytes((int)l[i].fileSize), keyArc);
                        FileStream fwSCR = new(dir + "\\" + l[i].fileName + ".SNX", FileMode.Create, FileAccess.Write);
                        fwSCR.Write(bufferSCR);
                        fwSCR.Close();
                        break;
                    case 2:
                    case 3://image
                        byte[] bufferIMG = brArc.ReadBytes((int)l[i].fileSize);
                        FileStream fwIMG = new(dir + "\\" + l[i].fileName + ".PNG", FileMode.Create, FileAccess.Write);
                        fwIMG.Write(bufferIMG);
                        fwIMG.Close();
                        break;
                    case 4://audio
                        byte[] bufferAUD_WAV = brArc.ReadBytes((int)l[i].fileSize);
                        FileStream fwAUD_WAV = new(dir + "\\" + l[i].fileName + ".WAV", FileMode.Create, FileAccess.Write);
                        fwAUD_WAV.Write(bufferAUD_WAV);
                        fwAUD_WAV.Close();
                        break;
                    case 5:
                        byte[] bufferAUD_OGG = brArc.ReadBytes((int)l[i].fileSize);
                        FileStream fwAUD_OGG = new(dir + "\\" + l[i].fileName + ".OGG", FileMode.Create, FileAccess.Write);
                        fwAUD_OGG.Write(bufferAUD_OGG);
                        fwAUD_OGG.Close();
                        break;
                    default:
                        main.Main.txtlog.AppendText("Unrecognized file detected:" + l[i].fileName + "Skip." + Environment.NewLine);
                        break;
                }
                main.Main.bar.PerformStep();
            }
            return 0;
        }
    }
}
