namespace GalArc
{
    partial class main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main));
            selectFile = new OpenFileDialog();
            selectFolder = new FolderBrowserDialog();
            engine = new Label();
            selEngine = new ComboBox();
            format = new Label();
            showFormat = new ListBox();
            selEncoding = new ComboBox();
            encoding = new Label();
            packFormat = new Label();
            selPackFormat = new ComboBox();
            version = new Label();
            selVersion = new ComboBox();
            filePath = new TextBox();
            selFile = new Button();
            btUnpack = new Button();
            txtlog = new TextBox();
            folderPath = new TextBox();
            selFolder = new Button();
            btPack = new Button();
            clearlog = new Button();
            automatch = new CheckBox();
            clearpath = new Button();
            bar = new ProgressBar();
            operation = new GroupBox();
            log = new GroupBox();
            operation.SuspendLayout();
            log.SuspendLayout();
            SuspendLayout();
            // 
            // engine
            // 
            engine.AutoSize = true;
            engine.Location = new Point(31, 37);
            engine.Name = "engine";
            engine.Size = new Size(69, 24);
            engine.TabIndex = 12;
            engine.Text = "Engine";
            // 
            // selEngine
            // 
            selEngine.DropDownStyle = ComboBoxStyle.DropDownList;
            selEngine.FlatStyle = FlatStyle.System;
            selEngine.FormattingEnabled = true;
            selEngine.Items.AddRange(new object[] { "AdvHD", "Ai5Win", "AmuseCraft", "Artemis", "EntisGLS", "InnocentGrey", "KID", "Kirikiri", "NextonLikeC", "Nitro+", "NScripter", "Silky", "Softpal", "SystemNNN", "Triangle" });
            selEngine.Location = new Point(31, 72);
            selEngine.Name = "selEngine";
            selEngine.Size = new Size(145, 32);
            selEngine.TabIndex = 13;
            selEngine.SelectedIndexChanged += selEngine_SelectedIndexChanged;
            // 
            // format
            // 
            format.AutoSize = true;
            format.Location = new Point(31, 130);
            format.Name = "format";
            format.Size = new Size(72, 24);
            format.TabIndex = 14;
            format.Text = "Format";
            // 
            // showFormat
            // 
            showFormat.BorderStyle = BorderStyle.FixedSingle;
            showFormat.FormattingEnabled = true;
            showFormat.ItemHeight = 24;
            showFormat.Location = new Point(31, 167);
            showFormat.Name = "showFormat";
            showFormat.Size = new Size(145, 74);
            showFormat.TabIndex = 16;
            // 
            // selEncoding
            // 
            selEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            selEncoding.FlatStyle = FlatStyle.System;
            selEncoding.FormattingEnabled = true;
            selEncoding.Location = new Point(31, 304);
            selEncoding.Name = "selEncoding";
            selEncoding.Size = new Size(145, 32);
            selEncoding.TabIndex = 18;
            // 
            // encoding
            // 
            encoding.AutoSize = true;
            encoding.Location = new Point(31, 268);
            encoding.Name = "encoding";
            encoding.Size = new Size(91, 24);
            encoding.TabIndex = 19;
            encoding.Text = "Encoding";
            // 
            // packFormat
            // 
            packFormat.AutoSize = true;
            packFormat.Location = new Point(31, 367);
            packFormat.Name = "packFormat";
            packFormat.Size = new Size(112, 24);
            packFormat.TabIndex = 20;
            packFormat.Text = "PackFormat";
            // 
            // selPackFormat
            // 
            selPackFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            selPackFormat.FlatStyle = FlatStyle.System;
            selPackFormat.FormattingEnabled = true;
            selPackFormat.Location = new Point(31, 402);
            selPackFormat.Name = "selPackFormat";
            selPackFormat.Size = new Size(145, 32);
            selPackFormat.TabIndex = 21;
            selPackFormat.SelectedIndexChanged += selPackFormat_SelectedIndexChanged;
            // 
            // version
            // 
            version.AutoSize = true;
            version.Location = new Point(31, 469);
            version.Name = "version";
            version.Size = new Size(114, 24);
            version.TabIndex = 22;
            version.Text = "PackVersion";
            // 
            // selVersion
            // 
            selVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            selVersion.FlatStyle = FlatStyle.System;
            selVersion.FormattingEnabled = true;
            selVersion.Location = new Point(31, 505);
            selVersion.Name = "selVersion";
            selVersion.Size = new Size(145, 32);
            selVersion.TabIndex = 23;
            // 
            // filePath
            // 
            filePath.AllowDrop = true;
            filePath.BorderStyle = BorderStyle.FixedSingle;
            filePath.Location = new Point(38, 48);
            filePath.Name = "filePath";
            filePath.Size = new Size(816, 30);
            filePath.TabIndex = 0;
            filePath.TextChanged += filePath_TextChanged;
            // 
            // selFile
            // 
            selFile.Location = new Point(860, 48);
            selFile.Name = "selFile";
            selFile.Size = new Size(31, 30);
            selFile.TabIndex = 1;
            selFile.Text = "…";
            selFile.UseVisualStyleBackColor = true;
            selFile.Click += selFile_Click;
            // 
            // btUnpack
            // 
            btUnpack.BackColor = SystemColors.Window;
            btUnpack.FlatStyle = FlatStyle.System;
            btUnpack.Location = new Point(709, 83);
            btUnpack.Name = "btUnpack";
            btUnpack.Size = new Size(145, 41);
            btUnpack.TabIndex = 2;
            btUnpack.Text = "unpack";
            btUnpack.UseVisualStyleBackColor = false;
            btUnpack.Click += btUnpack_Click;
            // 
            // txtlog
            // 
            txtlog.BackColor = SystemColors.InfoText;
            txtlog.BorderStyle = BorderStyle.FixedSingle;
            txtlog.ForeColor = SystemColors.HighlightText;
            txtlog.Location = new Point(38, 40);
            txtlog.Multiline = true;
            txtlog.Name = "txtlog";
            txtlog.ReadOnly = true;
            txtlog.ScrollBars = ScrollBars.Vertical;
            txtlog.Size = new Size(853, 376);
            txtlog.TabIndex = 3;
            // 
            // folderPath
            // 
            folderPath.AllowDrop = true;
            folderPath.BorderStyle = BorderStyle.FixedSingle;
            folderPath.Location = new Point(38, 151);
            folderPath.Name = "folderPath";
            folderPath.Size = new Size(816, 30);
            folderPath.TabIndex = 4;
            // 
            // selFolder
            // 
            selFolder.Location = new Point(860, 151);
            selFolder.Name = "selFolder";
            selFolder.Size = new Size(31, 30);
            selFolder.TabIndex = 5;
            selFolder.Text = "…";
            selFolder.UseVisualStyleBackColor = true;
            selFolder.Click += selFolder_Click;
            // 
            // btPack
            // 
            btPack.BackColor = SystemColors.Window;
            btPack.FlatStyle = FlatStyle.System;
            btPack.Location = new Point(709, 186);
            btPack.Name = "btPack";
            btPack.Size = new Size(145, 41);
            btPack.TabIndex = 6;
            btPack.Text = "pack";
            btPack.UseVisualStyleBackColor = false;
            btPack.Click += btPack_Click;
            // 
            // clearlog
            // 
            clearlog.BackColor = SystemColors.Window;
            clearlog.FlatStyle = FlatStyle.System;
            clearlog.Location = new Point(763, 382);
            clearlog.Name = "clearlog";
            clearlog.Size = new Size(102, 34);
            clearlog.TabIndex = 8;
            clearlog.Text = "clear";
            clearlog.UseVisualStyleBackColor = false;
            clearlog.Click += clearlog_Click;
            // 
            // automatch
            // 
            automatch.AutoSize = true;
            automatch.Checked = true;
            automatch.CheckState = CheckState.Checked;
            automatch.FlatStyle = FlatStyle.System;
            automatch.Location = new Point(600, 193);
            automatch.Name = "automatch";
            automatch.Size = new Size(102, 29);
            automatch.TabIndex = 10;
            automatch.Text = "match";
            automatch.UseVisualStyleBackColor = true;
            automatch.CheckedChanged += automatch_CheckedChanged;
            // 
            // clearpath
            // 
            clearpath.BackColor = SystemColors.Window;
            clearpath.FlatStyle = FlatStyle.System;
            clearpath.Location = new Point(427, 186);
            clearpath.Name = "clearpath";
            clearpath.Size = new Size(145, 41);
            clearpath.TabIndex = 11;
            clearpath.Text = "clear";
            clearpath.UseVisualStyleBackColor = false;
            clearpath.Click += clearpath_Click;
            // 
            // bar
            // 
            bar.Location = new Point(38, 417);
            bar.Name = "bar";
            bar.Size = new Size(853, 30);
            bar.Step = 1;
            bar.TabIndex = 17;
            // 
            // operation
            // 
            operation.Anchor = AnchorStyles.Top;
            operation.Controls.Add(clearpath);
            operation.Controls.Add(automatch);
            operation.Controls.Add(btPack);
            operation.Controls.Add(selFolder);
            operation.Controls.Add(folderPath);
            operation.Controls.Add(btUnpack);
            operation.Controls.Add(selFile);
            operation.Controls.Add(filePath);
            operation.Location = new Point(207, 11);
            operation.Name = "operation";
            operation.Size = new Size(918, 251);
            operation.TabIndex = 24;
            operation.TabStop = false;
            // 
            // log
            // 
            log.Controls.Add(bar);
            log.Controls.Add(clearlog);
            log.Controls.Add(txtlog);
            log.Location = new Point(207, 268);
            log.Name = "log";
            log.Size = new Size(918, 472);
            log.TabIndex = 25;
            log.TabStop = false;
            // 
            // main
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1138, 757);
            Controls.Add(operation);
            Controls.Add(selVersion);
            Controls.Add(version);
            Controls.Add(selPackFormat);
            Controls.Add(packFormat);
            Controls.Add(encoding);
            Controls.Add(selEncoding);
            Controls.Add(showFormat);
            Controls.Add(format);
            Controls.Add(selEngine);
            Controls.Add(engine);
            Controls.Add(log);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GalArc";
            Load += main_Load;
            operation.ResumeLayout(false);
            operation.PerformLayout();
            log.ResumeLayout(false);
            log.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private OpenFileDialog selectFile;
        private FolderBrowserDialog selectFolder;
        private Label engine;
        private Label format;
        public ComboBox selEncoding;
        public ComboBox selEngine;
        public ListBox showFormat;
        public Label encoding;
        private Label packFormat;
        public ComboBox selPackFormat;
        private Label version;
        public ComboBox selVersion;
        public TextBox filePath;
        private Button selFile;
        private Button btUnpack;
        public TextBox txtlog;
        public TextBox folderPath;
        private Button selFolder;
        private Button btPack;
        private Button clearlog;
        private CheckBox automatch;
        private Button clearpath;
        public ProgressBar bar;
        private GroupBox operation;
        private GroupBox log;
    }
}
