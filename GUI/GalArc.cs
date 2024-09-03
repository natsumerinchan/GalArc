using Interface;
using Utility;
namespace GalArc
{
    public partial class main : Form
    {
        public static main Main;
        public main()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.filePath.DragEnter += new DragEventHandler(filePath_DragEnter);
            this.filePath.DragDrop += new DragEventHandler(filePath_DragDrop);
            this.folderPath.DragEnter += new DragEventHandler(folderPath_DragEnter);
            this.folderPath.DragDrop += new DragEventHandler(folderPath_DragDrop);
            Main = this;
        }
        private void main_Load(object sender, EventArgs e)
        {
            Config.loadConfig();
            //ArcFormat.Formats.Engines.EngineInit();
        }

        private void selEngine_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormatFace.AddFormat(this.selEngine.Text);
            EncodingFace.AddEncoding(this.selEngine.Text);
            PackFormatFace.AddPackFormat(this.selEngine.Text);
            LogEngineInfo.AddEngineInfo(this.selEngine.Text);
            Config.saveConfig(this.selEngine.SelectedIndex);
            PackVersionFace.AddVersion(this.selEngine.Text);

            if (this.automatch.Checked && File.Exists(this.filePath.Text))
            {
                PathFace.folderPathSync();
            }

        }

        private void filePath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void filePath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                filePath.Lines = fileNames;
            }
        }
        private void folderPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void folderPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                folderPath.Lines = fileNames;
            }
        }

        private void selFile_Click(object sender, EventArgs e)
        {
            if (this.selectFile.ShowDialog() == DialogResult.OK)
            {
                this.filePath.Text = this.selectFile.FileName;
            }
        }
        private void selFolder_Click(object sender, EventArgs e)
        {
            if (this.selectFolder.ShowDialog() == DialogResult.OK)
            {
                this.folderPath.Text = this.selectFolder.SelectedPath;
            }
        }

        private void btUnpack_Click(object sender, EventArgs e)
        {
            if (this.filePath.Text.Length == 0)
            {
                txtlog.AppendText("Please specify file path." + Environment.NewLine + Environment.NewLine);
                return;
            }
            if (!File.Exists(this.filePath.Text))
            {
                txtlog.AppendText("Please check file path.Maybe it doesn't exist." + Environment.NewLine + Environment.NewLine);
                return;
            }
            Execute.ExecuteUnpack(this.selEngine.Text);
        }
        private void btPack_Click(object sender, EventArgs e)
        {
            if (this.folderPath.Text.Length == 0)
            {
                txtlog.AppendText("Please specify folder path." + Environment.NewLine + Environment.NewLine);
                return;
            }
            if (!Directory.Exists(this.folderPath.Text)&&!File.Exists(this.folderPath.Text))
            {
                txtlog.AppendText("Please check folder path.Maybe it doesn't exist." + Environment.NewLine + Environment.NewLine);
                return;
            }
            Execute.ExecutePack(this.selEngine.Text);
        }

        private void filePath_TextChanged(object sender, EventArgs e)
        {
            if (this.automatch.Checked && File.Exists(this.filePath.Text))
            {
                PathFace.folderPathSync();
            }
        }
        private void automatch_CheckedChanged(object sender, EventArgs e)
        {
            if (this.automatch.Checked && this.filePath.Text != string.Empty)
            {
                PathFace.folderPathSync();
            }
        }

        private void clearpath_Click(object sender, EventArgs e)
        {
            this.folderPath.Text = string.Empty;
            this.filePath.Text = string.Empty;
        }
        private void clearlog_Click(object sender, EventArgs e)
        {
            this.txtlog.Clear();
        }

        private void selPackFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            PathFace.folderPathSync();
            PackVersionFace.RemoveVersion(this.selEngine.Text);
        }
    }
}

