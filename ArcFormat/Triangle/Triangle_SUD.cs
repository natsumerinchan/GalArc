using GalArc;
using Interface;
using System;
using System.Linq;

namespace ArcFormat.Triangle
{
    class Triangle_SUD
    {
        public static int SUD_unpack(string fileName)
        {
            main.Main.txtlog.AppendText("Unpacking……" + Environment.NewLine);
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            string dir = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName);
            Directory.CreateDirectory(dir);
            int index = 1;
            main.Main.bar.Value = 0;

            while (fs.Position < fs.Length)
            {
                uint size = br.ReadUInt32();
                byte[] data = br.ReadBytes((int)size);
                File.WriteAllBytes(dir + "\\" + index.ToString("D6") + ".ogg", data);
                index++;
            }
            main.Main.bar.Value = main.Main.bar.Maximum;
            fs.Close();

            return 0;
        }
        public static int SUD_pack(string folderName)
        {
            FileStream fw = new FileStream(Path.GetDirectoryName(folderName) + "\\" + Path.GetFileName(folderName) + ".SUD.new", FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fw);
            DirectoryInfo d = new(folderName);
            DisplayFace.displayPack(d.GetFiles("*.ogg", SearchOption.TopDirectoryOnly).Length);
            foreach (FileInfo file in d.GetFiles("*.ogg", SearchOption.TopDirectoryOnly))
            {
                bw.Write((uint)file.Length);
                bw.Write(File.ReadAllBytes(file.FullName));
                main.Main.bar.PerformStep();
            }
            fw.Close();

            return 0;
        }
    }
}