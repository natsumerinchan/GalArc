using GalArc;
using System;
using System.Linq;
namespace Interface
{
    class PathFace
    {
        public static void folderPathSync()
        {
            if (main.Main.filePath.Text != "")
            {
                switch (main.Main.selEngine.Text)
                {
                    case "Artemis":
                        main.Main.folderPath.Text = Path.GetDirectoryName(main.Main.filePath.Text) + "\\" + Path.GetFileName(main.Main.filePath.Text).Replace(".", "_");
                        break;
                    case "NextonLikeC":
                        main.Main.folderPath.Text = Path.GetDirectoryName(main.Main.filePath.Text) + "\\" + Path.GetFileNameWithoutExtension(main.Main.filePath.Text) + "_unpacked";
                        break;
                    case "InnocentGrey":
                        main.Main.folderPath.Text = Path.GetDirectoryName(main.Main.filePath.Text) + "\\" + Path.GetFileNameWithoutExtension(main.Main.filePath.Text) + "_unpacked";
                        break;
                    case "NScripter":
                        if (main.Main.selPackFormat.Text == ".dat")
                        {
                            main.Main.folderPath.Text = main.Main.filePath.Text + ".out";
                            break;
                        }
                        else
                        {
                            main.Main.folderPath.Text = Path.GetDirectoryName(main.Main.filePath.Text) + "\\" + Path.GetFileNameWithoutExtension(main.Main.filePath.Text);
                            break;
                        }
                    default:
                        main.Main.folderPath.Text = Path.GetDirectoryName(main.Main.filePath.Text) + "\\" + Path.GetFileNameWithoutExtension(main.Main.filePath.Text);
                        break;

                }
            }
        }

    }
}
