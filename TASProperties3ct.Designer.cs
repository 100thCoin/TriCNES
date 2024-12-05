namespace TriCNES
{
    partial class TASProperties3ct
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TASProperties3ct));
            this.cb_CpuClock = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_ClockAlignment = new System.Windows.Forms.ComboBox();
            this.l_InputCount = new System.Windows.Forms.Label();
            this.b_BrowseFile = new System.Windows.Forms.Button();
            this.l_FilePath = new System.Windows.Forms.Label();
            this.tb_FilePath = new System.Windows.Forms.TextBox();
            this.b_RunTAS = new System.Windows.Forms.Button();
            this.b_LoadCartridges = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cb_CpuClock
            // 
            this.cb_CpuClock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_CpuClock.FormattingEnabled = true;
            this.cb_CpuClock.Items.AddRange(new object[] {
            "Phase 0",
            "Phase 1",
            "Phase 2",
            "Phase 3",
            "Phase 4",
            "Phase 5",
            "Phase 6",
            "Phase 7",
            "Phase 8",
            "Phase 9",
            "Phase 10",
            "Phase 11"});
            this.cb_CpuClock.Location = new System.Drawing.Point(177, 56);
            this.cb_CpuClock.Name = "cb_CpuClock";
            this.cb_CpuClock.Size = new System.Drawing.Size(65, 21);
            this.cb_CpuClock.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "CPU / Master clock alignment";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "PPU / Master clock alignment";
            // 
            // cb_ClockAlignment
            // 
            this.cb_ClockAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ClockAlignment.FormattingEnabled = true;
            this.cb_ClockAlignment.Items.AddRange(new object[] {
            "Phase 0",
            "Phase 1",
            "Phase 2",
            "Phase 3"});
            this.cb_ClockAlignment.Location = new System.Drawing.Point(177, 79);
            this.cb_ClockAlignment.Name = "cb_ClockAlignment";
            this.cb_ClockAlignment.Size = new System.Drawing.Size(65, 21);
            this.cb_ClockAlignment.TabIndex = 21;
            // 
            // l_InputCount
            // 
            this.l_InputCount.AutoSize = true;
            this.l_InputCount.Location = new System.Drawing.Point(27, 32);
            this.l_InputCount.Name = "l_InputCount";
            this.l_InputCount.Size = new System.Drawing.Size(44, 13);
            this.l_InputCount.TabIndex = 19;
            this.l_InputCount.Text = "0 inputs";
            // 
            // b_BrowseFile
            // 
            this.b_BrowseFile.Location = new System.Drawing.Point(266, 9);
            this.b_BrowseFile.Name = "b_BrowseFile";
            this.b_BrowseFile.Size = new System.Drawing.Size(59, 21);
            this.b_BrowseFile.TabIndex = 18;
            this.b_BrowseFile.Text = "Browse...";
            this.b_BrowseFile.UseVisualStyleBackColor = true;
            // 
            // l_FilePath
            // 
            this.l_FilePath.AutoSize = true;
            this.l_FilePath.Location = new System.Drawing.Point(27, 12);
            this.l_FilePath.Name = "l_FilePath";
            this.l_FilePath.Size = new System.Drawing.Size(23, 13);
            this.l_FilePath.TabIndex = 16;
            this.l_FilePath.Text = "File";
            // 
            // tb_FilePath
            // 
            this.tb_FilePath.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tb_FilePath.Location = new System.Drawing.Point(54, 9);
            this.tb_FilePath.Name = "tb_FilePath";
            this.tb_FilePath.ReadOnly = true;
            this.tb_FilePath.Size = new System.Drawing.Size(206, 20);
            this.tb_FilePath.TabIndex = 15;
            // 
            // b_RunTAS
            // 
            this.b_RunTAS.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.b_RunTAS.Enabled = false;
            this.b_RunTAS.Location = new System.Drawing.Point(25, 176);
            this.b_RunTAS.Name = "b_RunTAS";
            this.b_RunTAS.Size = new System.Drawing.Size(300, 40);
            this.b_RunTAS.TabIndex = 14;
            this.b_RunTAS.Text = "Run TAS";
            this.b_RunTAS.UseVisualStyleBackColor = true;
            this.b_RunTAS.Click += new System.EventHandler(this.b_RunTAS_Click);
            // 
            // b_LoadCartridges
            // 
            this.b_LoadCartridges.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.b_LoadCartridges.Location = new System.Drawing.Point(25, 118);
            this.b_LoadCartridges.Name = "b_LoadCartridges";
            this.b_LoadCartridges.Size = new System.Drawing.Size(300, 40);
            this.b_LoadCartridges.TabIndex = 25;
            this.b_LoadCartridges.Text = "Load Cartridges";
            this.b_LoadCartridges.UseVisualStyleBackColor = true;
            this.b_LoadCartridges.Click += new System.EventHandler(this.b_LoadCartridges_Click);
            // 
            // TASProperties3ct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 228);
            this.Controls.Add(this.b_LoadCartridges);
            this.Controls.Add(this.cb_CpuClock);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cb_ClockAlignment);
            this.Controls.Add(this.l_InputCount);
            this.Controls.Add(this.b_BrowseFile);
            this.Controls.Add(this.l_FilePath);
            this.Controls.Add(this.tb_FilePath);
            this.Controls.Add(this.b_RunTAS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TASProperties3ct";
            this.Text = "3CT TAS Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_CpuClock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_ClockAlignment;
        private System.Windows.Forms.Label l_InputCount;
        private System.Windows.Forms.Button b_BrowseFile;
        private System.Windows.Forms.Label l_FilePath;
        private System.Windows.Forms.TextBox tb_FilePath;
        private System.Windows.Forms.Button b_RunTAS;
        private System.Windows.Forms.Button b_LoadCartridges;
    }
}