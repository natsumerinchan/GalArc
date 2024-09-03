using GalArc;

namespace Interface
{
    internal class FormatFace
    {
        public static void AddFormat(string SelectedText)
        {
            main.Main.showFormat.Items.Clear();
            switch (SelectedText)
            {
                case "Artemis":
                    main.Main.showFormat.Items.Add(".pfs");
                    return;
                case "SystemNNN":
                    main.Main.showFormat.Items.Add(".gpk");
                    main.Main.showFormat.Items.Add(".gtb");
                    main.Main.showFormat.Items.Add(".vpk");
                    main.Main.showFormat.Items.Add(".vtb");
                    return;
                case "AdvHD":
                    main.Main.showFormat.Items.Add(".arc");
                    main.Main.showFormat.Items.Add(".pna");
                    return;
                case "InnocentGrey":
                    main.Main.showFormat.Items.Add(".iga");
                    main.Main.showFormat.Items.Add(".dat");
                    return;
                case "EntisGLS":
                    main.Main.showFormat.Items.Add(".noa");
                    return;
                case "Ai5Win":
                    main.Main.showFormat.Items.Add(".VSD");
                    return;
                case "Silky":
                    main.Main.showFormat.Items.Add(".arc");
                    return;
                case "NextonLikeC":
                    main.Main.showFormat.Items.Add(string.Empty);
                    main.Main.showFormat.Items.Add(".lst");
                    return;
                case "Triangle":
                    main.Main.showFormat.Items.Add(".CG");
                    main.Main.showFormat.Items.Add(".SUD");
                    return;
                case "NScripter":
                    main.Main.showFormat.Items.Add(".ns2");
                    main.Main.showFormat.Items.Add(".dat");
                    return;
                case "AmuseCraft":
                    main.Main.showFormat.Items.Add(".pac");
                    return;
                case "Nitro+":
                    main.Main.showFormat.Items.Add(".pak");
                    return;
                case "Softpal":
                    main.Main.showFormat.Items.Add(".pac");
                    return;
                case "KID":
                    main.Main.showFormat.Items.Add(".dat");
                    return;
                case "Kirikiri":
                    main.Main.showFormat.Items.Add(".xp3");
                    return;
            }

        }
    }
}
