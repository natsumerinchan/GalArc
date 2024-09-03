using GalArc;
namespace Interface
{
    internal class PackVersionFace
    {
        public static void AddVersion(string SelectedText)
        {
            main.Main.selVersion.Items.Clear();
            switch (SelectedText)
            {
                default:
                    main.Main.selVersion.Enabled = false;
                    return;
                case "Artemis":
                    main.Main.selVersion.Enabled = true;
                    main.Main.selVersion.Items.Add("8");
                    main.Main.selVersion.Items.Add("6");
                    main.Main.selVersion.Items.Add("2");
                    main.Main.selVersion.SelectedIndex = 0;
                    return;
                case "AdvHD":
                    main.Main.selVersion.Enabled = true;
                    main.Main.selVersion.Items.Add("1");
                    main.Main.selVersion.Items.Add("2");
                    main.Main.selVersion.SelectedIndex = 0;
                    return;
                case "SystemNNN":
                    main.Main.selVersion.Enabled = true;
                    main.Main.selVersion.Items.Add("1");
                    main.Main.selVersion.Items.Add("2");
                    main.Main.selVersion.SelectedIndex = 0;
                    return;
            }

        }

        public static void RemoveVersion(string SelectedText)
        {
            if (SelectedText == "AdvHD")
            {
                main.Main.selVersion.Items.Clear();
                if (main.Main.selPackFormat.SelectedIndex == 0)
                {
                    main.Main.selVersion.Enabled = true;
                    main.Main.selVersion.Items.Add("1");
                    main.Main.selVersion.Items.Add("2");
                    main.Main.selVersion.SelectedIndex = 0;
                    return;
                }
                else
                {
                    main.Main.selVersion.Enabled = false;
                    return;
                }
            }
            if (SelectedText == "SystemNNN")
            {
                main.Main.selVersion.Items.Clear();
                if (main.Main.selPackFormat.SelectedIndex == 0)
                {
                    main.Main.selVersion.Enabled = true;
                    main.Main.selVersion.Items.Add("1");
                    main.Main.selVersion.Items.Add("2");
                    main.Main.selVersion.SelectedIndex = 0;
                    return;
                }
                else
                {
                    main.Main.selVersion.Enabled = false;
                    return;
                }
            }
        }
    }
}
