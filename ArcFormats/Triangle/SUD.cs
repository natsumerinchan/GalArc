﻿using Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcFormats.Triangle
{
    public class SUD
    {
        public static void Unpack(string filePath, string folderPath, Encoding encoding)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Directory.CreateDirectory(folderPath);
            int index = 1;
            while (fs.Position < fs.Length)
            {
                uint size = br.ReadUInt32();
                byte[] data = br.ReadBytes((int)size);
                File.WriteAllBytes(folderPath + "\\" + index.ToString("D6") + ".ogg", data);
                index++;
            }
            fs.Dispose();
        }
        public static void Pack(string folderPath, string filePath, string version, Encoding encoding)
        {
            FileStream fw = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fw);
            DirectoryInfo d = new DirectoryInfo(folderPath);
            LogUtility.InitBar(d.GetFiles("*.ogg", SearchOption.TopDirectoryOnly).Length);
            foreach (FileInfo file in d.GetFiles("*.ogg", SearchOption.TopDirectoryOnly))
            {
                bw.Write((uint)file.Length);
                bw.Write(File.ReadAllBytes(file.FullName));
                LogUtility.UpdateBar();
            }
            fw.Dispose();
        }
    }
}