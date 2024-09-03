using GalArc;
using System.Text;

namespace Utility
{
    class ArcEncoding//shiftjis:932,gb2312:936
    {
        public static Encoding Encodings()
        {
            if (main.Main.selEncoding.Enabled)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                if (main.Main.selEncoding.SelectedIndex == 0)
                {
                    return Encoding.UTF8;
                }
                else if (main.Main.selEncoding.SelectedIndex == 1)
                {
                    return Encoding.GetEncoding(932);
                }
                else
                {
                    return Encoding.GetEncoding(936);
                }
            }
            else
            {
                main.Main.txtlog.AppendText("No valid encoding selection detected.Use default." + Environment.NewLine);
                return Encoding.Default;
            }
        }
        public static Encoding Encodings(int codePage)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(codePage);
        }
    }
}
