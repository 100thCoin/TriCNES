namespace TriCNES
{
    partial class TriCNESGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriCNESGUI));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.consoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.load3ctToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pb_Screen = new System.Windows.Forms.PictureBox();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Screen)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consoleToolStripMenuItem,
            this.tASToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(256, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // consoleToolStripMenuItem
            // 
            this.consoleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadROMToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
            this.consoleToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.consoleToolStripMenuItem.Text = "Console";
            // 
            // loadROMToolStripMenuItem
            // 
            this.loadROMToolStripMenuItem.Name = "loadROMToolStripMenuItem";
            this.loadROMToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadROMToolStripMenuItem.Text = "Load ROM";
            this.loadROMToolStripMenuItem.Click += new System.EventHandler(this.loadROMToolStripMenuItem_Click);
            // 
            // tASToolStripMenuItem
            // 
            this.tASToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadTASToolStripMenuItem,
            this.load3ctToolStripMenuItem});
            this.tASToolStripMenuItem.Name = "tASToolStripMenuItem";
            this.tASToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.tASToolStripMenuItem.Text = "TAS";
            // 
            // loadTASToolStripMenuItem
            // 
            this.loadTASToolStripMenuItem.Name = "loadTASToolStripMenuItem";
            this.loadTASToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadTASToolStripMenuItem.Text = "Load TAS";
            this.loadTASToolStripMenuItem.Click += new System.EventHandler(this.loadTASToolStripMenuItem_Click);
            // 
            // load3ctToolStripMenuItem
            // 
            this.load3ctToolStripMenuItem.Name = "load3ctToolStripMenuItem";
            this.load3ctToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.load3ctToolStripMenuItem.Text = "Load .3ct TAS";
            this.load3ctToolStripMenuItem.Click += new System.EventHandler(this.load3ctToolStripMenuItem_Click);
            // 
            // pb_Screen
            // 
            this.pb_Screen.BackColor = System.Drawing.Color.Black;
            this.pb_Screen.Location = new System.Drawing.Point(0, 27);
            this.pb_Screen.Name = "pb_Screen";
            this.pb_Screen.Size = new System.Drawing.Size(256, 240);
            this.pb_Screen.TabIndex = 1;
            this.pb_Screen.TabStop = false;
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // TriCNESGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 267);
            this.Controls.Add(this.pb_Screen);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(272, 306);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(272, 306);
            this.Name = "TriCNESGUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TriCNES GUI";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Screen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem consoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tASToolStripMenuItem;
        private System.Windows.Forms.PictureBox pb_Screen;
        private System.Windows.Forms.ToolStripMenuItem loadROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTASToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem load3ctToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
    }
}

