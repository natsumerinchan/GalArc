﻿using ArcFormats.Properties;
using GalArc.Database;
using GalArc.Database.NScripter;
using GalArc.Logs;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ArcFormats.NScripter
{
    public partial class UnpackNS2Options : UserControl
    {
        private NS2Scheme ImportedScheme { get; set; }

        public UnpackNS2Options()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            Type type = this.combSchemes.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.combSchemes, true, null);

            ImportKeys();
        }

        private void ImportKeys()
        {
            if (ImportedScheme == null)
            {
                ImportedScheme = Deserializer.ReadScheme(typeof(NS2Scheme)) as NS2Scheme;
                if (ImportedScheme != null)
                {
                    Logger.Debug(string.Format(Resources.logImportDataBaseSuccess, ImportedScheme.KnownKeys.Count));
                }
                combSchemes.Items.Add(Resources.combNoEncryption);
                combSchemes.Items.Add(Resources.combCustomScheme);
                combSchemes.SelectedIndex = 0;
                foreach (var key in ImportedScheme.KnownKeys)
                {
                    combSchemes.Items.Add(key.Key);
                }
            }
        }

        private void combSchemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (combSchemes.SelectedIndex)
            {
                case 0:
                    txtKey.Text = string.Empty;
                    txtKey.Enabled = false;
                    break;
                case 1:
                    txtKey.Text = string.Empty;
                    txtKey.Enabled = true;
                    break;
                default:
                    txtKey.Text = ImportedScheme.KnownKeys[combSchemes.SelectedItem.ToString()];
                    txtKey.Enabled = true;
                    break;
            }
        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            NS2.Key = txtKey.Text;
        }
    }
}
