namespace TriCNES
{
    partial class TriCHexEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriCHexEditor));
            this.pb_hexView = new System.Windows.Forms.PictureBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scopeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cPUAddressSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vRAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pPUAddressSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paletteRAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pb_hexView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb_hexView
            // 
            this.pb_hexView.Location = new System.Drawing.Point(12, 37);
            this.pb_hexView.Name = "pb_hexView";
            this.pb_hexView.Size = new System.Drawing.Size(312, 512);
            this.pb_hexView.TabIndex = 0;
            this.pb_hexView.TabStop = false;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.LargeChange = 32;
            this.vScrollBar1.Location = new System.Drawing.Point(327, 57);
            this.vScrollBar1.Maximum = 128;
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 492);
            this.vScrollBar1.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(356, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scopeToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // scopeToolStripMenuItem
            // 
            this.scopeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rAMToolStripMenuItem,
            this.cPUAddressSpaceToolStripMenuItem,
            this.vRAMToolStripMenuItem,
            this.pPUAddressSpaceToolStripMenuItem,
            this.oAMToolStripMenuItem,
            this.paletteRAMToolStripMenuItem});
            this.scopeToolStripMenuItem.Name = "scopeToolStripMenuItem";
            this.scopeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.scopeToolStripMenuItem.Text = "Scope";
            // 
            // rAMToolStripMenuItem
            // 
            this.rAMToolStripMenuItem.Checked = true;
            this.rAMToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rAMToolStripMenuItem.Name = "rAMToolStripMenuItem";
            this.rAMToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.rAMToolStripMenuItem.Text = "RAM";
            this.rAMToolStripMenuItem.Click += new System.EventHandler(this.rAMToolStripMenuItem_Click);
            // 
            // cPUAddressSpaceToolStripMenuItem
            // 
            this.cPUAddressSpaceToolStripMenuItem.Name = "cPUAddressSpaceToolStripMenuItem";
            this.cPUAddressSpaceToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.cPUAddressSpaceToolStripMenuItem.Text = "CPU Address Space";
            this.cPUAddressSpaceToolStripMenuItem.Click += new System.EventHandler(this.cPUAddressSpaceToolStripMenuItem_Click);
            // 
            // vRAMToolStripMenuItem
            // 
            this.vRAMToolStripMenuItem.Name = "vRAMToolStripMenuItem";
            this.vRAMToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.vRAMToolStripMenuItem.Text = "VRAM";
            this.vRAMToolStripMenuItem.Click += new System.EventHandler(this.vRAMToolStripMenuItem_Click);
            // 
            // pPUAddressSpaceToolStripMenuItem
            // 
            this.pPUAddressSpaceToolStripMenuItem.Name = "pPUAddressSpaceToolStripMenuItem";
            this.pPUAddressSpaceToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.pPUAddressSpaceToolStripMenuItem.Text = "PPU Address Space";
            this.pPUAddressSpaceToolStripMenuItem.Click += new System.EventHandler(this.pPUAddressSpaceToolStripMenuItem_Click);
            // 
            // oAMToolStripMenuItem
            // 
            this.oAMToolStripMenuItem.Name = "oAMToolStripMenuItem";
            this.oAMToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.oAMToolStripMenuItem.Text = "OAM";
            this.oAMToolStripMenuItem.Click += new System.EventHandler(this.oAMToolStripMenuItem_Click);
            // 
            // paletteRAMToolStripMenuItem
            // 
            this.paletteRAMToolStripMenuItem.Name = "paletteRAMToolStripMenuItem";
            this.paletteRAMToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.paletteRAMToolStripMenuItem.Text = "Palette RAM";
            this.paletteRAMToolStripMenuItem.Click += new System.EventHandler(this.paletteRAMToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToClipboardToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // copyToClipboardToolStripMenuItem
            // 
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            this.copyToClipboardToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyToClipboardToolStripMenuItem.Text = "Copy to Clipboard";
            this.copyToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem_Click);
            // 
            // TriCHexEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 561);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.pb_hexView);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(16, 135);
            this.Name = "TriCHexEditor";
            this.Text = "Hex Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pb_hexView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_hexView;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scopeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cPUAddressSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vRAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pPUAddressSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paletteRAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
    }
}