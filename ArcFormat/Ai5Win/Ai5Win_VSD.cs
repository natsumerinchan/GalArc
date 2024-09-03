using Interface;
using System.Text;
namespace ArcFormat.Ai5Win
{
    class Ai5Win_VSD
    {
        public static int vsd_unpack(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath));
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "VSD1" || !File.Exists(filePath))
                return 1;
            int fileCount = 1;
            DisplayFace.displayUn(fileCount, filePath);
            uint offset = br.ReadUInt32() + 8;
            uint size = (uint)new FileInfo(filePath).Length - offset;
            fs.Seek(offset, SeekOrigin.Begin);
            byte[] buffer = br.ReadBytes((int)size);
            FileStream fw = new(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".mpg", FileMode.Create, FileAccess.Write);
            fw.Write(buffer, 0, buffer.Length);
            fw.Close();
            fs.Close();
            return 0;
        }
    }
}
