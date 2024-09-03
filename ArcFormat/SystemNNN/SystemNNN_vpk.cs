using GalArc;
using Interface;
using System.Text;
using Utility;
namespace ArcFormat.SystemNNN
{
    internal class SystemNNN_vpk
    {
        struct SystemNNN_vtb_entry
        {
            public int size { get; set; }
            public string filePath { get; set; }
        }

        public static int vpk_unpack(string filePath)
        {
            //init
            string vpkPath;
            string vtbPath;
            if (Path.GetExtension(filePath) == ".vpk")
            {
                vpkPath = filePath;
                vtbPath = Path.ChangeExtension(filePath, ".vtb");
            }
            else if (Path.GetExtension(filePath) == ".vtb")
            {
                vtbPath = filePath;
                vpkPath = Path.ChangeExtension(filePath, ".vpk");
            }
            else
            {
                return 1;
            }

            if (!File.Exists(vtbPath))
                return 3;

            int vtbSize = (int)new FileInfo(vtbPath).Length;
            int filecount = (vtbSize / 12) - 1;//file size = 12*file count + 12
            DisplayFace.displayUn(filecount, filePath);

            //open&make dir
            FileStream fs1 = new(vtbPath, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new(fs1);
            FileStream fs2 = new(vpkPath, FileMode.Open, FileAccess.Read);
            BinaryReader br2 = new(fs2);
            var folderPath = Path.GetDirectoryName(vtbPath) + "\\" + Path.GetFileName(vtbPath).Replace(".vtb", string.Empty);
            Directory.CreateDirectory(folderPath);


            //1~n-1
            for (int i = 1; i < filecount; i++)
            {
                SystemNNN_vtb_entry entry = new();
                entry.filePath = folderPath + "\\" + Encoding.UTF8.GetString(br1.ReadBytes(8)) + ".vaw";
                int size1 = br1.ReadInt32();
                fs1.Seek(8, SeekOrigin.Current);
                int size2 = br1.ReadInt32();
                entry.size = size2 - size1;
                byte[] buffer = br2.ReadBytes(entry.size);
                FileStream fw = new(entry.filePath, FileMode.Create, FileAccess.Write);
                fw.Flush();
                fw.Write(buffer, 0, entry.size);

                fs1.Seek(12 * i, SeekOrigin.Begin);


                fw.Close();
                main.Main.bar.PerformStep();
            }
            //the last
            SystemNNN_vtb_entry last = new();
            last.filePath = folderPath + "\\" + Encoding.UTF8.GetString(br1.ReadBytes(8)) + ".vaw";
            int vpksizeBefore = br1.ReadInt32();
            fs1.Seek(8, SeekOrigin.Current);//reserve

            int vpksize = br1.ReadInt32();
            last.size = vpksize - vpksizeBefore;
            byte[] buf = br2.ReadBytes(last.size);
            FileStream fwlast = new(last.filePath, FileMode.Create, FileAccess.Write);
            fwlast.Flush();
            fwlast.Write(buf, 0, last.size);

            fwlast.Close();
            main.Main.bar.PerformStep();

            fs1.Close();
            fs2.Close();
            br1.Close();
            br2.Close();
            return 0;
        }
        public static int vpk_pack(string folderPath)
        {
            //init
            int sizeToNow = 0;
            DirectoryInfo d = new(folderPath);
            int filecount = Util.GetFileCount_All(folderPath);
            string vtbPath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".vtb.new";
            string vpkPath = Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath) + ".vpk.new";
            FileStream fs1 = new(vtbPath, FileMode.Create, FileAccess.Write);
            FileStream fs2 = new(vpkPath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer1 = new(fs1);
            BinaryWriter writer2 = new(fs2);
            DisplayFace.displayPack(filecount);

            foreach (FileInfo fi in d.GetFiles())
            {
                if (Path.GetExtension(fi.FullName) != ".vaw" || Path.GetFileNameWithoutExtension(fi.FullName).Length != 8)
                {
                    fs1.Close();
                    fs2.Close();
                    FileInfo deleteVpk = new FileInfo(vpkPath);
                    FileInfo deleteVtb = new FileInfo(vtbPath);
                    deleteVpk.Delete();
                    deleteVtb.Delete();
                    return 1;
                }
                writer1.Write(Encoding.UTF8.GetBytes(Path.GetFileNameWithoutExtension(fi.FullName)));
                writer1.Write(sizeToNow);
                sizeToNow += (int)fi.Length;

                byte[] buffer = File.ReadAllBytes(fi.FullName);
                writer2.Write(buffer);
                main.Main.bar.PerformStep();
            }
            writer1.Write(0);
            writer1.Write(0);
            writer1.Write(sizeToNow);

            fs1.Close();
            fs2.Close();






            return 0;
        }
    }
}
