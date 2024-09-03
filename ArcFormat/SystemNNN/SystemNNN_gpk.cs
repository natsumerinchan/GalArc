using GalArc;
using Interface;
using System.Text;
using Utility;
namespace ArcFormat.SystemNNN
{
    internal class SystemNNN_gpk
    {
        struct SystemNNN_gtb_entry
        {
            public uint size { get; set; }
            public uint offset { get; set; }
            public string filePath { get; set; }
        }

        public static int gpk_unpack(string filePath)
        {
            //init
            string gpkPath;
            string gtbPath;

            if (Path.GetExtension(filePath) == ".gpk")
            {
                gpkPath = filePath;
                gtbPath = Path.ChangeExtension(filePath, ".gtb");
            }
            else if (Path.GetExtension(filePath) == ".gtb")
            {
                gtbPath = filePath;
                gpkPath = Path.ChangeExtension(filePath, ".gpk");
            }
            else
            {
                return 1;
            }

            if (!File.Exists(gtbPath)||!File.Exists(gpkPath))
                return 3;

            //open&make dir
            FileStream fs1 = new(gtbPath, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new(fs1);
            uint filecount = br1.ReadUInt32();
            DisplayFace.displayUn(filecount, filePath);

            FileStream fs2 = new(gpkPath, FileMode.Open, FileAccess.Read);
            BinaryReader br2 = new(fs2);

            //List<SystemNNN_entry> index = new();
            var folderPath = Path.GetDirectoryName(gtbPath) + "\\" + Path.GetFileName(gtbPath).Replace(".gtb", string.Empty);
            Directory.CreateDirectory(folderPath);

            uint thisPos = 0;
            uint maxPos = 0;
            //process
            //i~n-1
            for (int i = 1; i < filecount; i++)
            {
                SystemNNN_gtb_entry entry = new();

                //offset
                entry.offset = br1.ReadUInt32();

                fs1.Seek(4 * filecount - 4, SeekOrigin.Current);

                //size
                uint size1 = br1.ReadUInt32();
                uint size2 = br1.ReadUInt32();
                entry.size = size2 - size1;

                fs1.Seek(4 + 8 * filecount + entry.offset, SeekOrigin.Begin);

                entry.filePath = folderPath + "\\" + Encoding.UTF8.GetString(Util.ReadToAnsi(br1, 0x00)) + ".dwq";
                thisPos = (uint)fs1.Position;
                maxPos = Math.Max(thisPos, maxPos);

                //get file content
                byte[] buffer = br2.ReadBytes((int)entry.size);

                //write file
                FileStream fw = new(entry.filePath, FileMode.Create, FileAccess.Write);
                fw.Flush();
                fw.Write(buffer);
                fs1.Seek(4 + 4 * i, SeekOrigin.Begin);
                fw.Close();

                main.Main.bar.PerformStep();
            }

            uint offset = br1.ReadUInt32();
            uint gtbSize = (uint)new FileInfo(gtbPath).Length;
            uint gpkSize = (uint)new FileInfo(gpkPath).Length;
            fs1.Seek(8 * filecount, SeekOrigin.Begin);
            uint sizeWithoutLast = br1.ReadUInt32();
            fs1.Seek(offset + 4 + 8 * filecount, SeekOrigin.Begin);
            SystemNNN_gtb_entry last = new();
            last.offset = gtbSize - (offset + 4 + 8 * filecount) - 1;
            last.filePath = folderPath + "\\" + Encoding.UTF8.GetString(Util.ReadToAnsi(br1,0x00)) + ".dwq";
            last.size = gpkSize - sizeWithoutLast;

            thisPos = (uint)fs1.Position;
            maxPos = Math.Max(thisPos, maxPos);

            byte[] buf = br2.ReadBytes((int)last.size);

            //write file
            FileStream fwlast = new(last.filePath, FileMode.Create, FileAccess.Write);

            fwlast.Write(buf);
            fwlast.Close();
            main.Main.bar.PerformStep();
            if (maxPos == gtbSize)
                main.Main.txtlog.AppendText("Valid gpk v1 file detected." + Environment.NewLine);
            else
                main.Main.txtlog.AppendText("Valid gpk v2 file detected." + Environment.NewLine);

            fs1.Close();
            fs2.Close();
            br1.Close();
            br2.Close();

            return 0;
        }
        public static int gpk_pack(string folderPath)
        {

            DirectoryInfo d = new(folderPath);
            uint filecount = (uint)Util.GetFileCount_All(folderPath);
            main.Main.txtlog.AppendText(filecount + " files in total.Initializing……" + Environment.NewLine);
            string gtbPath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".gtb.new";
            string gpkPath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".gpk.new";
            main.Main.bar.Value = 0;
            main.Main.bar.Step = 1;
            main.Main.bar.Maximum = (int)(3 * filecount);

            FileStream fs1 = new(gtbPath, FileMode.Create, FileAccess.Write);
            FileStream fs2 = new(gpkPath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer1 = new(fs1);
            BinaryWriter writer2 = new(fs2);

            uint sizeToNow = 0;
            uint offsetToNow = 0;
            writer1.Write(filecount);

            foreach (FileInfo fi in d.GetFiles())
            {
                if (Path.GetExtension(fi.FullName) != ".dwq")
                {
                    fs1.Close();
                    fs2.Close();
                    FileInfo deleteVpk = new FileInfo(gpkPath);
                    FileInfo deleteVtb = new FileInfo(gtbPath);
                    deleteVpk.Delete();
                    deleteVtb.Delete();
                    return 1;
                }
                writer1.Write(offsetToNow);
                offsetToNow = offsetToNow + (uint)Path.GetFileNameWithoutExtension(fi.FullName).Length + 1;

                byte[] buffer = File.ReadAllBytes(fi.FullName);
                writer2.Write(buffer);
                main.Main.bar.PerformStep();
            }

            foreach (FileInfo fi in d.GetFiles())
            {
                writer1.Write(sizeToNow);
                sizeToNow += (uint)fi.Length;
                main.Main.bar.PerformStep();
            }

            foreach (FileInfo fi in d.GetFiles())
            {
                writer1.Write(Encoding.ASCII.GetBytes(Path.GetFileNameWithoutExtension(fi.FullName)));
                writer1.Write('\0');
                main.Main.bar.PerformStep();
            }

            while (fs1.Position % 16 != 0)
            {
                writer1.Write('\0');
            }

            if (main.Main.selVersion.Text == "1")
            {
                //skip this
            }
            else
            {
                ulong sizeToNowNew = 0;
                foreach (FileInfo fi in d.GetFiles())
                {
                    writer1.Write(sizeToNowNew);
                    sizeToNowNew += (ulong)fi.Length;
                }
                writer1.Write((ulong)0);
                writer1.Write(Encoding.ASCII.GetBytes("over2G!"));
                writer1.Write('\0');
            }

            fs1.Close();
            fs2.Close();

            return 0;
        }
    }
}
