using GalArc;
using System;
using System.Linq;
namespace Interface
{
    class LogUnpackInfo
    {
        public static void Unpack(int result)
        {
            switch (result)
            {
                case 0://finished
                    main.Main.txtlog.AppendText("Unpack finished." + Environment.NewLine + Environment.NewLine);
                    return;
                case 1://magic or extension cannot match.
                    main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                    return;
                case 2:
                    main.Main.txtlog.AppendText("Unpack failed:Encrypted file detected." + Environment.NewLine + Environment.NewLine);
                    return;
                default://unknown reason
                    main.Main.txtlog.AppendText("Unpack failed." + Environment.NewLine + Environment.NewLine);
                    return;

            }
        }
        public static void Unpack(int result, string ext1, string ext2)
        {
            switch (result)
            {
                case 0://finished
                    main.Main.txtlog.AppendText("Unpack finished." + Environment.NewLine + Environment.NewLine);
                    return;
                case 1://magic or extension cannot match.
                    main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                    return;
                case 2://encrypted not supported
                    main.Main.txtlog.AppendText("Unpack failed:Encrypted file detected." + Environment.NewLine + Environment.NewLine);
                    return;
                case 3://no same name file detected
                    main.Main.txtlog.AppendText("Unpack failed:" + ext1 + " and " + ext2 + " file with the same name should be in the same directory." + Environment.NewLine + Environment.NewLine);
                    return;

                default://unknown reason
                    main.Main.txtlog.AppendText("Unpack failed." + Environment.NewLine + Environment.NewLine);
                    return;
            }

        }
    }
}
