﻿using GalArc.Logs;
using System;
using System.IO;
using System.Windows.Forms;

namespace ArcFormats.Siglus
{
    public class GameexeDAT : ScenePCK
    {
        public static new UserControl UnpackExtraOptions = ScenePCK.UnpackExtraOptions;

        private const string UnpackedFileName = "Gameexe.ini";

        public override void Unpack(string filePath, string folderPath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            ScenePckEntry entry = new ScenePckEntry();
            uint reserve = br.ReadUInt32();
            if (reserve != 0)
            {
                Logger.ErrorInvalidArchive();
            }
            bool flag = br.ReadUInt32() == 1;
            entry.Data = br.ReadBytes((int)br.BaseStream.Length - 8);
            byte[] key = flag ? (TryEachKey ? TryAllSchemes(entry, 1) : SelectedScheme.Item2) : null;

            Logger.InitBar(1);
            SiglusUtils.DecryptWithKey(entry.Data, key);
            SiglusUtils.Decrypt(entry.Data, 1);

            entry.PackedLength = BitConverter.ToUInt32(entry.Data, 0);
            if (entry.PackedLength != entry.Data.Length)
            {
                Logger.Error(Siglus.logWrongKey);
            }
            entry.UnpackedLength = BitConverter.ToUInt32(entry.Data, 4);
            byte[] input = new byte[entry.PackedLength - 8];
            Array.Copy(entry.Data, 8, input, 0, input.Length);
            try
            {
                entry.Data = SiglusUtils.Decompress(input, entry.UnpackedLength);
            }
            catch
            {
                Logger.Error(Siglus.logWrongKey);
            }
            Directory.CreateDirectory(folderPath);
            using (FileStream fw = File.Create(Path.Combine(folderPath, UnpackedFileName)))
            {
                fw.Write(new byte[] { 0xff, 0xfe }, 0, 2);
                fw.Write(entry.Data, 0, entry.Data.Length);
            }
            Logger.UpdateBar();
            fs.Dispose();
            br.Dispose();
        }
    }
}
