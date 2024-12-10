﻿using GalArc.Logs;
using System.IO;

namespace ArcFormats.Triangle
{
    public class SUD : ArchiveFormat
    {
        public override void Unpack(string filePath, string folderPath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            Directory.CreateDirectory(folderPath);
            int index = 1;
            while (fs.Position < fs.Length)
            {
                uint size = br.ReadUInt32();
                byte[] data = br.ReadBytes((int)size);
                File.WriteAllBytes(Path.Combine(folderPath, index.ToString("D6") + ".ogg"), data);
                data = null;
                index++;
            }
            fs.Dispose();
            br.Dispose();
        }

        public override void Pack(string folderPath, string filePath)
        {
            FileStream fw = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(fw);
            DirectoryInfo d = new DirectoryInfo(folderPath);
            FileInfo[] files = d.GetFiles("*.ogg");
            Logger.InitBar(files.Length);
            foreach (FileInfo file in files)
            {
                bw.Write((uint)file.Length);
                byte[] data = File.ReadAllBytes(file.FullName);
                bw.Write(data);
                data = null;
                Logger.UpdateBar();
            }
            fw.Dispose();
            bw.Dispose();
        }
    }
}