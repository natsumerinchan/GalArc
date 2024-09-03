using GalArc;

namespace Interface
{
    internal class PackFormatFace
    {
        public static void AddPackFormat(string SelectedText)
        {
            main.Main.selPackFormat.Items.Clear();
            main.Main.selPackFormat.Enabled = true;
            switch (SelectedText)
            {
                default:
                    break;
                case "SystemNNN":
                    main.Main.selPackFormat.Items.Add(".gpk");
                    main.Main.selPackFormat.Items.Add(".vpk");
                    break;
                case "AdvHD":
                    main.Main.selPackFormat.Items.Add(".arc");
                    main.Main.selPackFormat.Items.Add(".pna");
                    break;
                case "EntisGLS":
                    main.Main.selPackFormat.Items.Add(".noa");
                    break;
                case "Artemis":
                    main.Main.selPackFormat.Items.Add(".pfs");
                    break;
                case "InnocentGrey":
                    main.Main.selPackFormat.Items.Add(".iga");
                    main.Main.selPackFormat.Items.Add(".dat");
                    break;
                case "Triangle":
                    main.Main.selPackFormat.Items.Add(".CG");
                    main.Main.selPackFormat.Items.Add(".SUD");
                    break;
                case "NScripter":
                    main.Main.selPackFormat.Items.Add(".ns2");
                    main.Main.selPackFormat.Items.Add(".dat");
                    break;
                case "Nitro+":
                    main.Main.selPackFormat.Items.Add(".pak");
                    break;
                case "Softpal":
                    main.Main.selPackFormat.Items.Add(".pac");
                    break;
                case "KID":
                    main.Main.selPackFormat.Items.Add(".dat");
                    break;
                case "AmuseCraft":
                    main.Main.selPackFormat.Items.Add(".pac");
                    break;
                case "Kirikiri":
                    main.Main.selPackFormat.Items.Add(".xp3");
                    break;
            }
            if (main.Main.selPackFormat.Items.Count != 0)
                main.Main.selPackFormat.SelectedIndex = 0;
            return;
        }
    }
}
