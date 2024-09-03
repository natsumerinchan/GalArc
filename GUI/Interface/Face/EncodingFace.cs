using GalArc;
namespace Interface
{
    internal class EncodingFace
    {
        public static void AddEncoding(string SelectedText)
        {
            main.Main.selEncoding.Items.Clear();

            switch (SelectedText)
            {
                case "Artemis":
                    main.Main.selEncoding.Enabled = true;

                    main.Main.selEncoding.Items.Add("UTF-8");
                    main.Main.selEncoding.Items.Add("Shift-JIS");
                    main.Main.selEncoding.Items.Add("GBK");
                    main.Main.selEncoding.SelectedIndex = 0;

                    return;
                default:
                    main.Main.selEncoding.Enabled = false;


                    return;
            }

        }

    }
}
