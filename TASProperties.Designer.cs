namespace TriCNES
{
    partial class TASProperties
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TASProperties));
            this.b_RunTAS = new System.Windows.Forms.Button();
            this.tb_FilePath = new System.Windows.Forms.TextBox();
            this.l_FilePath = new System.Windows.Forms.Label();
            this.rb_LatchFiltering = new System.Windows.Forms.RadioButton();
            this.rb_ClockFiltering = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TASPropTooltips = new System.Windows.Forms.ToolTip(this.components);
            this.cb_ClockAlignment = new System.Windows.Forms.ComboBox();
            this.cb_CpuClock = new System.Windows.Forms.ComboBox();
            this.b_BrowseFile = new System.Windows.Forms.Button();
            this.l_InputCount = new System.Windows.Forms.Label();
            this.l_FamtasiaWarning = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // b_RunTAS
            // 
            this.b_RunTAS.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.b_RunTAS.Location = new System.Drawing.Point(24, 255);
            this.b_RunTAS.Name = "b_RunTAS";
            this.b_RunTAS.Size = new System.Drawing.Size(300, 40);
            this.b_RunTAS.TabIndex = 0;
            this.b_RunTAS.Text = "Run TAS";
            this.b_RunTAS.UseVisualStyleBackColor = true;
            this.b_RunTAS.Click += new System.EventHandler(this.b_RunTAS_Click);
            // 
            // tb_FilePath
            // 
            this.tb_FilePath.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tb_FilePath.Location = new System.Drawing.Point(53, 6);
            this.tb_FilePath.Name = "tb_FilePath";
            this.tb_FilePath.ReadOnly = true;
            this.tb_FilePath.Size = new System.Drawing.Size(206, 20);
            this.tb_FilePath.TabIndex = 1;
            // 
            // l_FilePath
            // 
            this.l_FilePath.AutoSize = true;
            this.l_FilePath.Location = new System.Drawing.Point(26, 9);
            this.l_FilePath.Name = "l_FilePath";
            this.l_FilePath.Size = new System.Drawing.Size(23, 13);
            this.l_FilePath.TabIndex = 2;
            this.l_FilePath.Text = "File";
            // 
            // rb_LatchFiltering
            // 
            this.rb_LatchFiltering.AutoSize = true;
            this.rb_LatchFiltering.Location = new System.Drawing.Point(5, 3);
            this.rb_LatchFiltering.Name = "rb_LatchFiltering";
            this.rb_LatchFiltering.Size = new System.Drawing.Size(91, 17);
            this.rb_LatchFiltering.TabIndex = 3;
            this.rb_LatchFiltering.TabStop = true;
            this.rb_LatchFiltering.Text = "Latch Filtering";
            this.TASPropTooltips.SetToolTip(this.rb_LatchFiltering, "Latch filtering will provide a single input per frame.");
            this.rb_LatchFiltering.UseVisualStyleBackColor = true;
            // 
            // rb_ClockFiltering
            // 
            this.rb_ClockFiltering.AutoSize = true;
            this.rb_ClockFiltering.Location = new System.Drawing.Point(5, 26);
            this.rb_ClockFiltering.Name = "rb_ClockFiltering";
            this.rb_ClockFiltering.Size = new System.Drawing.Size(91, 17);
            this.rb_ClockFiltering.TabIndex = 4;
            this.rb_ClockFiltering.TabStop = true;
            this.rb_ClockFiltering.Text = "Clock Filtering";
            this.TASPropTooltips.SetToolTip(this.rb_ClockFiltering, "Clock filtering will provide multiple inputs per frame. This is used in \"subframe" +
        "\" TASes.");
            this.rb_ClockFiltering.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rb_LatchFiltering);
            this.panel1.Controls.Add(this.rb_ClockFiltering);
            this.panel1.Location = new System.Drawing.Point(24, 200);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(100, 49);
            this.panel1.TabIndex = 6;
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
            this.cb_ClockAlignment.Location = new System.Drawing.Point(176, 174);
            this.cb_ClockAlignment.Name = "cb_ClockAlignment";
            this.cb_ClockAlignment.Size = new System.Drawing.Size(65, 21);
            this.cb_ClockAlignment.TabIndex = 10;
            this.TASPropTooltips.SetToolTip(this.cb_ClockAlignment, "Some runs may desync depending on alignment. Different emulators use different al" +
        "ignments. Only change this if you know what you are doing.");
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
            this.cb_CpuClock.Location = new System.Drawing.Point(176, 151);
            this.cb_CpuClock.Name = "cb_CpuClock";
            this.cb_CpuClock.Size = new System.Drawing.Size(65, 21);
            this.cb_CpuClock.TabIndex = 13;
            this.TASPropTooltips.SetToolTip(this.cb_CpuClock, "Some runs may desync depending on alignment. Different emulators use different al" +
        "ignments. Only change this if you know what you are doing.");
            // 
            // b_BrowseFile
            // 
            this.b_BrowseFile.Location = new System.Drawing.Point(265, 6);
            this.b_BrowseFile.Name = "b_BrowseFile";
            this.b_BrowseFile.Size = new System.Drawing.Size(59, 21);
            this.b_BrowseFile.TabIndex = 7;
            this.b_BrowseFile.Text = "Browse...";
            this.b_BrowseFile.UseVisualStyleBackColor = true;
            // 
            // l_InputCount
            // 
            this.l_InputCount.AutoSize = true;
            this.l_InputCount.Location = new System.Drawing.Point(26, 29);
            this.l_InputCount.Name = "l_InputCount";
            this.l_InputCount.Size = new System.Drawing.Size(44, 13);
            this.l_InputCount.TabIndex = 8;
            this.l_InputCount.Text = "0 inputs";
            // 
            // l_FamtasiaWarning
            // 
            this.l_FamtasiaWarning.AutoSize = true;
            this.l_FamtasiaWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_FamtasiaWarning.Location = new System.Drawing.Point(12, 94);
            this.l_FamtasiaWarning.Name = "l_FamtasiaWarning";
            this.l_FamtasiaWarning.Size = new System.Drawing.Size(325, 26);
            this.l_FamtasiaWarning.TabIndex = 9;
            this.l_FamtasiaWarning.Text = "Warning!\r\nFamtasia is an old emulator, and the TAS is not guaranteed to sync!";
            this.l_FamtasiaWarning.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "PPU / Master clock alignment";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "CPU / Master clock alignment";
            // 
            // TASProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 307);
            this.Controls.Add(this.cb_CpuClock);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cb_ClockAlignment);
            this.Controls.Add(this.l_FamtasiaWarning);
            this.Controls.Add(this.l_InputCount);
            this.Controls.Add(this.b_BrowseFile);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.l_FilePath);
            this.Controls.Add(this.tb_FilePath);
            this.Controls.Add(this.b_RunTAS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TASProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TAS Properties";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button b_RunTAS;
        private System.Windows.Forms.TextBox tb_FilePath;
        private System.Windows.Forms.Label l_FilePath;
        private System.Windows.Forms.RadioButton rb_LatchFiltering;
        private System.Windows.Forms.RadioButton rb_ClockFiltering;
        private System.Windows.Forms.ToolTip TASPropTooltips;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button b_BrowseFile;
        private System.Windows.Forms.Label l_InputCount;
        private System.Windows.Forms.Label l_FamtasiaWarning;
        private System.Windows.Forms.ComboBox cb_ClockAlignment;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cb_CpuClock;
    }
}