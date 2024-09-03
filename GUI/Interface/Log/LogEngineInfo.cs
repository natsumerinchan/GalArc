using GalArc;

namespace Interface
{
    internal class LogEngineInfo
    {
        public static void AddEngineInfo(string SelectedText)
        {
            switch (SelectedText)
            {
                case "Artemis":
                    main.Main.txtlog.AppendText("Artemis:" + Environment.NewLine + "Encoding in Unicode default." + Environment.NewLine + "Attention:Some encoding might cause fatal error." + Environment.NewLine + Environment.NewLine);
                    return;
                case "SystemNNN":
                    main.Main.txtlog.AppendText("SystemNNN:" + Environment.NewLine + "Gpk file contains dwq image and vpk contains vaw audio." + Environment.NewLine + "To unpack them,gtb and vtb file with the same name is necessary." + Environment.NewLine + Environment.NewLine);
                    return;
                case "AdvHD":
                    main.Main.txtlog.AppendText("AdvHD:" + Environment.NewLine + "To pack pna file,you need:" + Environment.NewLine + "1.Unpack raw pna file using this tool." + Environment.NewLine + "2.Edit the png file in the folder.Don't change its size or name,and don't add or delete png file. " + Environment.NewLine + "3.Repack." + Environment.NewLine + Environment.NewLine);
                    return;
                case "InnocentGrey":
                    main.Main.txtlog.AppendText("InnocentGrey:" + Environment.NewLine + "The .s script file is already decrypted." + Environment.NewLine + "You can use IG_tools to extract and insert the scripts." + Environment.NewLine + Environment.NewLine);
                    return;
                case "NextonLikeC":
                    main.Main.txtlog.AppendText("NextonLikeC:" + Environment.NewLine + "You cound enter either the extensionless file or lst file." + Environment.NewLine + Environment.NewLine);
                    return;
                default:
                    return;
            }
        }
    }
}
