using GalArc;
namespace Interface
{
    class DisplayFace
    {
        public static void displayUn(int fileCount, string filePath)
        {
            main.Main.bar.Value = 0;
            main.Main.bar.Step = 1;
            main.Main.txtlog.AppendText(fileCount.ToString() + " files inside " + filePath + Environment.NewLine + "Unpacking……" + Environment.NewLine);
            main.Main.bar.Maximum = fileCount;
        }
        public static void displayUn(uint fileCount, string filePath)
        {
            main.Main.bar.Value = 0;
            main.Main.bar.Step = 1;
            main.Main.txtlog.AppendText(fileCount.ToString() + " files inside " + filePath + Environment.NewLine + "Unpacking……" + Environment.NewLine);
            main.Main.bar.Maximum = (int)fileCount;
        }

        public static void displayPack(int fileCount)
        {
            main.Main.bar.Value = 0;
            main.Main.bar.Step = 1;
            main.Main.txtlog.AppendText(fileCount + " files in total.Initializing……" + Environment.NewLine);
            main.Main.bar.Maximum = fileCount;
        }
        public static void displayPack(uint fileCount)
        {
            main.Main.bar.Value = 0;
            main.Main.bar.Step = 1;
            main.Main.txtlog.AppendText(fileCount + " files in total.Initializing……" + Environment.NewLine);
            main.Main.bar.Maximum = (int)fileCount;
        }

    }
}
