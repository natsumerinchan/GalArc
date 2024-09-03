using GalArc;
using Interface;
using System;
using System.Linq;

namespace ArcFormat.NScripter
{
    class Nscripter_dat
    {
        public static int dat_unpack(string filePath)
        {
            DisplayFace.displayUn(1,filePath);
            byte[] data = File.ReadAllBytes(filePath);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= 0x84;
            }
            main.Main.bar.PerformStep();
            File.WriteAllBytes(filePath + ".out", data);
            return 0;
        }
        public static int dat_pack(string filePath)
        {
            DisplayFace.displayPack(1);
            byte[] data = File.ReadAllBytes(filePath);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= 0x84;
            }
            main.Main.bar.PerformStep();
            File.WriteAllBytes(Path.ChangeExtension(filePath, ".new"), data);
            return 0;
        }
    }
}
