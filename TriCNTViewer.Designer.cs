namespace TriCNES
{
    partial class TriCNTViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriCNTViewer));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cb_ForcePal0ToBackdrop = new System.Windows.Forms.CheckBox();
            this.cb_ScreenBoundary = new System.Windows.Forms.CheckBox();
            this.cb_OverlayScreen = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 480);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // cb_ForcePal0ToBackdrop
            // 
            this.cb_ForcePal0ToBackdrop.AutoSize = true;
            this.cb_ForcePal0ToBackdrop.Location = new System.Drawing.Point(13, 508);
            this.cb_ForcePal0ToBackdrop.Name = "cb_ForcePal0ToBackdrop";
            this.cb_ForcePal0ToBackdrop.Size = new System.Drawing.Size(121, 17);
            this.cb_ForcePal0ToBackdrop.TabIndex = 1;
            this.cb_ForcePal0ToBackdrop.Text = "Use Backdrop Color";
            this.cb_ForcePal0ToBackdrop.UseVisualStyleBackColor = true;
            // 
            // cb_ScreenBoundary
            // 
            this.cb_ScreenBoundary.AutoSize = true;
            this.cb_ScreenBoundary.Location = new System.Drawing.Point(12, 531);
            this.cb_ScreenBoundary.Name = "cb_ScreenBoundary";
            this.cb_ScreenBoundary.Size = new System.Drawing.Size(136, 17);
            this.cb_ScreenBoundary.TabIndex = 2;
            this.cb_ScreenBoundary.Text = "Draw Screen Boundary";
            this.cb_ScreenBoundary.UseVisualStyleBackColor = true;
            // 
            // cb_OverlayScreen
            // 
            this.cb_OverlayScreen.AutoSize = true;
            this.cb_OverlayScreen.Location = new System.Drawing.Point(12, 554);
            this.cb_OverlayScreen.Name = "cb_OverlayScreen";
            this.cb_OverlayScreen.Size = new System.Drawing.Size(99, 17);
            this.cb_OverlayScreen.TabIndex = 3;
            this.cb_OverlayScreen.Text = "Overlay Screen";
            this.cb_OverlayScreen.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(512, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenshotToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // screenshotToolStripMenuItem
            // 
            this.screenshotToolStripMenuItem.Name = "screenshotToolStripMenuItem";
            this.screenshotToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.screenshotToolStripMenuItem.Text = "Screenshot";
            this.screenshotToolStripMenuItem.Click += new System.EventHandler(this.screenshotToolStripMenuItem_Click);
            // 
            // TriCNTViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 579);
            this.Controls.Add(this.cb_OverlayScreen);
            this.Controls.Add(this.cb_ScreenBoundary);
            this.Controls.Add(this.cb_ForcePal0ToBackdrop);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(528, 618);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(528, 618);
            this.Name = "TriCNTViewer";
            this.Text = "Nametable Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox cb_ForcePal0ToBackdrop;
        private System.Windows.Forms.CheckBox cb_ScreenBoundary;
        private System.Windows.Forms.CheckBox cb_OverlayScreen;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenshotToolStripMenuItem;
    }
}