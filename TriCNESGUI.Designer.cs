using System.Threading;
using System.Windows.Forms;

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
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerCycleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.load3ctToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pPUClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phase0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phase1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phase2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phase3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decodeNTSCSignalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.falseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.traceLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pb_Screen = new PictureBoxWithInterpolationMode();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Screen)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consoleToolStripMenuItem,
            this.tASToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolsToolStripMenuItem});
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
            this.resetToolStripMenuItem,
            this.powerCycleToolStripMenuItem,
            this.screenshotToolStripMenuItem});
            this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
            this.consoleToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.consoleToolStripMenuItem.Text = "Console";
            // 
            // loadROMToolStripMenuItem
            // 
            this.loadROMToolStripMenuItem.Name = "loadROMToolStripMenuItem";
            this.loadROMToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.loadROMToolStripMenuItem.Text = "Load ROM";
            this.loadROMToolStripMenuItem.Click += new System.EventHandler(this.loadROMToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // powerCycleToolStripMenuItem
            // 
            this.powerCycleToolStripMenuItem.Name = "powerCycleToolStripMenuItem";
            this.powerCycleToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.powerCycleToolStripMenuItem.Text = "Power Cycle";
            this.powerCycleToolStripMenuItem.Click += new System.EventHandler(this.powerCycleToolStripMenuItem_Click);
            // 
            // screenshotToolStripMenuItem
            // 
            this.screenshotToolStripMenuItem.Name = "screenshotToolStripMenuItem";
            this.screenshotToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.screenshotToolStripMenuItem.Text = "Screenshot";
            this.screenshotToolStripMenuItem.Click += new System.EventHandler(this.screenshotToolStripMenuItem_Click);
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
            this.loadTASToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.loadTASToolStripMenuItem.Text = "Load TAS";
            this.loadTASToolStripMenuItem.Click += new System.EventHandler(this.loadTASToolStripMenuItem_Click);
            // 
            // load3ctToolStripMenuItem
            // 
            this.load3ctToolStripMenuItem.Name = "load3ctToolStripMenuItem";
            this.load3ctToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.load3ctToolStripMenuItem.Text = "Load .3ct TAS";
            this.load3ctToolStripMenuItem.Click += new System.EventHandler(this.load3ctToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pPUClockToolStripMenuItem,
            this.decodeNTSCSignalsToolStripMenuItem,
            this.viewScaleToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // pPUClockToolStripMenuItem
            // 
            this.pPUClockToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.phase0ToolStripMenuItem,
            this.phase1ToolStripMenuItem,
            this.phase2ToolStripMenuItem,
            this.phase3ToolStripMenuItem});
            this.pPUClockToolStripMenuItem.Name = "pPUClockToolStripMenuItem";
            this.pPUClockToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.pPUClockToolStripMenuItem.Text = "PPU Clock";
            // 
            // phase0ToolStripMenuItem
            // 
            this.phase0ToolStripMenuItem.Checked = true;
            this.phase0ToolStripMenuItem.CheckOnClick = true;
            this.phase0ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.phase0ToolStripMenuItem.Name = "phase0ToolStripMenuItem";
            this.phase0ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.phase0ToolStripMenuItem.Text = "Phase 0";
            this.phase0ToolStripMenuItem.Click += new System.EventHandler(this.phase0ToolStripMenuItem_Click);
            // 
            // phase1ToolStripMenuItem
            // 
            this.phase1ToolStripMenuItem.CheckOnClick = true;
            this.phase1ToolStripMenuItem.Name = "phase1ToolStripMenuItem";
            this.phase1ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.phase1ToolStripMenuItem.Text = "Phase 1";
            this.phase1ToolStripMenuItem.Click += new System.EventHandler(this.phase1ToolStripMenuItem_Click);
            // 
            // phase2ToolStripMenuItem
            // 
            this.phase2ToolStripMenuItem.CheckOnClick = true;
            this.phase2ToolStripMenuItem.Name = "phase2ToolStripMenuItem";
            this.phase2ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.phase2ToolStripMenuItem.Text = "Phase 2";
            this.phase2ToolStripMenuItem.Click += new System.EventHandler(this.phase2ToolStripMenuItem_Click);
            // 
            // phase3ToolStripMenuItem
            // 
            this.phase3ToolStripMenuItem.CheckOnClick = true;
            this.phase3ToolStripMenuItem.Name = "phase3ToolStripMenuItem";
            this.phase3ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.phase3ToolStripMenuItem.Text = "Phase 3";
            this.phase3ToolStripMenuItem.Click += new System.EventHandler(this.phase3ToolStripMenuItem_Click);
            // 
            // decodeNTSCSignalsToolStripMenuItem
            // 
            this.decodeNTSCSignalsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trueToolStripMenuItem,
            this.falseToolStripMenuItem});
            this.decodeNTSCSignalsToolStripMenuItem.Name = "decodeNTSCSignalsToolStripMenuItem";
            this.decodeNTSCSignalsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.decodeNTSCSignalsToolStripMenuItem.Text = "Decode NTSC Signals";
            // 
            // trueToolStripMenuItem
            // 
            this.trueToolStripMenuItem.Name = "trueToolStripMenuItem";
            this.trueToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.trueToolStripMenuItem.Text = "True";
            this.trueToolStripMenuItem.Click += new System.EventHandler(this.trueToolStripMenuItem_Click);
            // 
            // falseToolStripMenuItem
            // 
            this.falseToolStripMenuItem.Checked = true;
            this.falseToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.falseToolStripMenuItem.Name = "falseToolStripMenuItem";
            this.falseToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.falseToolStripMenuItem.Text = "False";
            this.falseToolStripMenuItem.Click += new System.EventHandler(this.falseToolStripMenuItem_Click);
            // 
            // viewScaleToolStripMenuItem
            // 
            this.viewScaleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xToolStripMenuItem,
            this.xToolStripMenuItem1,
            this.xToolStripMenuItem2,
            this.xToolStripMenuItem3,
            this.xToolStripMenuItem4,
            this.xToolStripMenuItem5,
            this.xToolStripMenuItem6,
            this.xToolStripMenuItem7});
            this.viewScaleToolStripMenuItem.Name = "viewScaleToolStripMenuItem";
            this.viewScaleToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.viewScaleToolStripMenuItem.Text = "View Scale";
            // 
            // xToolStripMenuItem
            // 
            this.xToolStripMenuItem.Name = "xToolStripMenuItem";
            this.xToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem.Text = "1x";
            this.xToolStripMenuItem.Click += new System.EventHandler(this.xToolStripMenuItem_Click);
            // 
            // xToolStripMenuItem1
            // 
            this.xToolStripMenuItem1.Name = "xToolStripMenuItem1";
            this.xToolStripMenuItem1.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem1.Text = "2x";
            this.xToolStripMenuItem1.Click += new System.EventHandler(this.xToolStripMenuItem1_Click);
            // 
            // xToolStripMenuItem2
            // 
            this.xToolStripMenuItem2.Name = "xToolStripMenuItem2";
            this.xToolStripMenuItem2.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem2.Text = "3x";
            this.xToolStripMenuItem2.Click += new System.EventHandler(this.xToolStripMenuItem2_Click);
            // 
            // xToolStripMenuItem3
            // 
            this.xToolStripMenuItem3.Name = "xToolStripMenuItem3";
            this.xToolStripMenuItem3.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem3.Text = "4x";
            this.xToolStripMenuItem3.Click += new System.EventHandler(this.xToolStripMenuItem3_Click);
            // 
            // xToolStripMenuItem4
            // 
            this.xToolStripMenuItem4.Name = "xToolStripMenuItem4";
            this.xToolStripMenuItem4.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem4.Text = "5x";
            this.xToolStripMenuItem4.Click += new System.EventHandler(this.xToolStripMenuItem4_Click);
            // 
            // xToolStripMenuItem5
            // 
            this.xToolStripMenuItem5.Name = "xToolStripMenuItem5";
            this.xToolStripMenuItem5.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem5.Text = "6x";
            this.xToolStripMenuItem5.Click += new System.EventHandler(this.xToolStripMenuItem5_Click);
            // 
            // xToolStripMenuItem6
            // 
            this.xToolStripMenuItem6.Name = "xToolStripMenuItem6";
            this.xToolStripMenuItem6.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem6.Text = "7x";
            this.xToolStripMenuItem6.Click += new System.EventHandler(this.xToolStripMenuItem6_Click);
            // 
            // xToolStripMenuItem7
            // 
            this.xToolStripMenuItem7.Name = "xToolStripMenuItem7";
            this.xToolStripMenuItem7.Size = new System.Drawing.Size(86, 22);
            this.xToolStripMenuItem7.Text = "8x";
            this.xToolStripMenuItem7.Click += new System.EventHandler(this.xToolStripMenuItem7_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.traceLoggerToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // traceLoggerToolStripMenuItem
            // 
            this.traceLoggerToolStripMenuItem.Name = "traceLoggerToolStripMenuItem";
            this.traceLoggerToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.traceLoggerToolStripMenuItem.Text = "TraceLogger";
            this.traceLoggerToolStripMenuItem.Click += new System.EventHandler(this.traceLoggerToolStripMenuItem_Click);
            // 
            // pb_Screen
            // 
            this.pb_Screen.AllowDrop = true;
            this.pb_Screen.BackColor = System.Drawing.Color.Black;
            this.pb_Screen.Location = new System.Drawing.Point(0, 27);
            this.pb_Screen.Name = "pb_Screen";
            this.pb_Screen.Size = new System.Drawing.Size(256, 240);
            this.pb_Screen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Screen.TabIndex = 1;
            this.pb_Screen.TabStop = false;
            this.pb_Screen.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            // 
            // TriCNESGUI
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(256, 267);
            this.Controls.Add(this.pb_Screen);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
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
        private PictureBoxWithInterpolationMode pb_Screen;
        private System.Windows.Forms.ToolStripMenuItem loadROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTASToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem load3ctToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem powerCycleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenshotToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem pPUClockToolStripMenuItem;
        private ToolStripMenuItem phase0ToolStripMenuItem;
        private ToolStripMenuItem phase1ToolStripMenuItem;
        private ToolStripMenuItem phase2ToolStripMenuItem;
        private ToolStripMenuItem phase3ToolStripMenuItem;
        private ToolStripMenuItem decodeNTSCSignalsToolStripMenuItem;
        private ToolStripMenuItem trueToolStripMenuItem;
        private ToolStripMenuItem falseToolStripMenuItem;
        private ToolStripMenuItem viewScaleToolStripMenuItem;
        private ToolStripMenuItem xToolStripMenuItem;
        private ToolStripMenuItem xToolStripMenuItem1;
        private ToolStripMenuItem xToolStripMenuItem2;
        private ToolStripMenuItem xToolStripMenuItem3;
        private ToolStripMenuItem xToolStripMenuItem4;
        private ToolStripMenuItem xToolStripMenuItem5;
        private ToolStripMenuItem xToolStripMenuItem6;
        private ToolStripMenuItem xToolStripMenuItem7;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem traceLoggerToolStripMenuItem;
    }
}

