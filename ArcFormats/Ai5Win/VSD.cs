﻿using Log;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ArcFormats.Ai5Win
{
    public class VSD
    {
        public static void Unpack(string filePath, string folderPath, Encoding encoding)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath));
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "VSD1" || !File.Exists(filePath))
            {
                LogUtility.Error_NotValidArchive();
            }
            LogUtility.InitBar(1);
            uint offset = br.ReadUInt32() + 8;
            uint size = (uint)new FileInfo(filePath).Length - offset;
            fs.Seek(offset, SeekOrigin.Begin);
            byte[] buffer = br.ReadBytes((int)size);
            File.WriteAllBytes(folderPath + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".mpg", buffer);
            LogUtility.UpdateBar();
            fs.Dispose();
        }
    }
}

