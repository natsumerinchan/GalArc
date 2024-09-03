using GalArc;
using System;
using System.Linq;

namespace Interface
{
    class LogPackInfo
    {
        public static void Pack(int result)
        {
            switch (result)
            {
                case 0://finished
                    main.Main.txtlog.AppendText("Pack finished." + Environment.NewLine + Environment.NewLine);
                    return;
                default://unknown reason
                    main.Main.txtlog.AppendText("Pack failed." + Environment.NewLine + Environment.NewLine);
                    return;
            }

        }
    }
}
