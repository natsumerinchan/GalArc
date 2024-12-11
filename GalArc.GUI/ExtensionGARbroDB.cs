﻿using GalArc.Extensions.GARbroDB;
using GalArc.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GalArc.GUI
{
    public partial class ExtensionGARbroDB : UserControl
    {
        private static ExtensionGARbroDB instance;

        public static ExtensionGARbroDB Instance
        {
            get
            {
                return instance ?? (instance = new ExtensionGARbroDB());
            }
        }

        public ExtensionGARbroDB()
        {
            InitializeComponent();
            this.txtDBInfo.BackColor = Color.FromArgb(249, 249, 249);
        }

        private void ExtensionGARbroDB_Load(object sender, EventArgs e)
        {
            this.txtJsonPath.Text = GARbroDBConfig.Path;
            this.chkbxEnableGARbroDB.Checked = BaseSettings.Default.IsGARbroDBEnabled;
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.txtJsonPath.Text = openFileDialog.FileName;
                    GARbroDBConfig.Path = openFileDialog.FileName;
                }
            }
        }

        private void txtJsonPath_TextChanged(object sender, EventArgs e)
        {
            this.txtDBInfo.Text = Deserializer.GetJsonInfo(this.txtJsonPath.Text).Trim();
            GARbroDBConfig.Path = this.txtJsonPath.Text;
        }

        private void chkbxEnableGARbroDB_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.Default.IsGARbroDBEnabled = chkbxEnableGARbroDB.Checked;
            BaseSettings.Default.Save();
            this.panel.Enabled = chkbxEnableGARbroDB.Checked;
        }
    }
}
