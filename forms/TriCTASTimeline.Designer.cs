using System.Windows.Forms;

namespace TriCNES
{
    partial class TriCTASTimeline
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriCTASTimeline));
            this.pb_Timeline = new System.Windows.Forms.PictureBox();
            this.timelineScrollbar = new System.Windows.Forms.VScrollBar();
            this.b_FrameAdvance = new System.Windows.Forms.Button();
            this.b_FrameBack = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveWithSavestatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTor08ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellFrequencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perVBlankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perControllerStrobeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.b_play = new System.Windows.Forms.Button();
            this.contextMenuStrip_Timeline = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.truncateMovieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savestateThisFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.b_JumptoCursor = new System.Windows.Forms.Button();
            this.cb_FollowCursor = new System.Windows.Forms.CheckBox();
            this.tb_FollowDistance = new System.Windows.Forms.TrackBar();
            this.cb_SavestateEveryFrame = new System.Windows.Forms.CheckBox();
            this.tb_TempSavestates = new System.Windows.Forms.TextBox();
            this.label_TempSavestates = new System.Windows.Forms.Label();
            this.Tooltip_ = new System.Windows.Forms.ToolTip(this.components);
            this.label_AutoSavestateThreshold = new System.Windows.Forms.Label();
            this.tb_AutoSavestateThreshold = new System.Windows.Forms.TextBox();
            this.cb_RecordInputs = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Timeline)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip_Timeline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_FollowDistance)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_Timeline
            // 
            this.pb_Timeline.Location = new System.Drawing.Point(12, 27);
            this.pb_Timeline.Name = "pb_Timeline";
            this.pb_Timeline.Size = new System.Drawing.Size(337, 657);
            this.pb_Timeline.TabIndex = 7;
            this.pb_Timeline.TabStop = false;
            // 
            // timelineScrollbar
            // 
            this.timelineScrollbar.Location = new System.Drawing.Point(352, 27);
            this.timelineScrollbar.Name = "timelineScrollbar";
            this.timelineScrollbar.Size = new System.Drawing.Size(19, 657);
            this.timelineScrollbar.TabIndex = 2;
            // 
            // b_FrameAdvance
            // 
            this.b_FrameAdvance.Location = new System.Drawing.Point(601, 38);
            this.b_FrameAdvance.Name = "b_FrameAdvance";
            this.b_FrameAdvance.Size = new System.Drawing.Size(75, 23);
            this.b_FrameAdvance.TabIndex = 1;
            this.b_FrameAdvance.Text = ">";
            this.b_FrameAdvance.UseVisualStyleBackColor = true;
            this.b_FrameAdvance.Click += new System.EventHandler(this.b_FrameAdvance_Click);
            // 
            // b_FrameBack
            // 
            this.b_FrameBack.Location = new System.Drawing.Point(439, 38);
            this.b_FrameBack.Name = "b_FrameBack";
            this.b_FrameBack.Size = new System.Drawing.Size(75, 23);
            this.b_FrameBack.TabIndex = 2;
            this.b_FrameBack.Text = "<";
            this.b_FrameBack.UseVisualStyleBackColor = true;
            this.b_FrameBack.Click += new System.EventHandler(this.b_FrameBack_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tASToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(745, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tASToolStripMenuItem
            // 
            this.tASToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadTASToolStripMenuItem,
            this.saveTASToolStripMenuItem,
            this.saveWithSavestatesToolStripMenuItem,
            this.exportTor08ToolStripMenuItem});
            this.tASToolStripMenuItem.Name = "tASToolStripMenuItem";
            this.tASToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.tASToolStripMenuItem.Text = "File";
            // 
            // loadTASToolStripMenuItem
            // 
            this.loadTASToolStripMenuItem.Name = "loadTASToolStripMenuItem";
            this.loadTASToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadTASToolStripMenuItem.Text = "Load";
            this.loadTASToolStripMenuItem.Click += new System.EventHandler(this.loadTASToolStripMenuItem_Click);
            // 
            // saveTASToolStripMenuItem
            // 
            this.saveTASToolStripMenuItem.Name = "saveTASToolStripMenuItem";
            this.saveTASToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveTASToolStripMenuItem.Text = "Save";
            this.saveTASToolStripMenuItem.Click += new System.EventHandler(this.saveTASToolStripMenuItem_Click);
            // 
            // saveWithSavestatesToolStripMenuItem
            // 
            this.saveWithSavestatesToolStripMenuItem.Name = "saveWithSavestatesToolStripMenuItem";
            this.saveWithSavestatesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveWithSavestatesToolStripMenuItem.Text = "Save with savestates";
            this.saveWithSavestatesToolStripMenuItem.Click += new System.EventHandler(this.saveWithSavestatesToolStripMenuItem_Click);
            // 
            // exportTor08ToolStripMenuItem
            // 
            this.exportTor08ToolStripMenuItem.Name = "exportTor08ToolStripMenuItem";
            this.exportTor08ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportTor08ToolStripMenuItem.Text = "Export to .r08";
            this.exportTor08ToolStripMenuItem.Click += new System.EventHandler(this.exportTor08ToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cellFrequencyToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // cellFrequencyToolStripMenuItem
            // 
            this.cellFrequencyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.perVBlankToolStripMenuItem,
            this.perControllerStrobeToolStripMenuItem});
            this.cellFrequencyToolStripMenuItem.Name = "cellFrequencyToolStripMenuItem";
            this.cellFrequencyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cellFrequencyToolStripMenuItem.Text = "Cell Frequency";
            // 
            // perVBlankToolStripMenuItem
            // 
            this.perVBlankToolStripMenuItem.Checked = true;
            this.perVBlankToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.perVBlankToolStripMenuItem.Name = "perVBlankToolStripMenuItem";
            this.perVBlankToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.perVBlankToolStripMenuItem.Text = "Per VBlank";
            this.perVBlankToolStripMenuItem.Click += new System.EventHandler(this.perVBlankToolStripMenuItem_Click);
            // 
            // perControllerStrobeToolStripMenuItem
            // 
            this.perControllerStrobeToolStripMenuItem.Name = "perControllerStrobeToolStripMenuItem";
            this.perControllerStrobeToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.perControllerStrobeToolStripMenuItem.Text = "Per Controller Strobe";
            this.perControllerStrobeToolStripMenuItem.Click += new System.EventHandler(this.perControllerStrobeToolStripMenuItem_Click);
            // 
            // b_play
            // 
            this.b_play.Location = new System.Drawing.Point(521, 38);
            this.b_play.Name = "b_play";
            this.b_play.Size = new System.Drawing.Size(75, 23);
            this.b_play.TabIndex = 4;
            this.b_play.Text = "Paused";
            this.b_play.UseVisualStyleBackColor = true;
            this.b_play.Click += new System.EventHandler(this.b_play_Click);
            // 
            // contextMenuStrip_Timeline
            // 
            this.contextMenuStrip_Timeline.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteFrameToolStripMenuItem,
            this.insertFrameToolStripMenuItem,
            this.truncateMovieToolStripMenuItem,
            this.savestateThisFrameToolStripMenuItem});
            this.contextMenuStrip_Timeline.Name = "contextMenuStrip_Timeline";
            this.contextMenuStrip_Timeline.Size = new System.Drawing.Size(184, 92);
            // 
            // deleteFrameToolStripMenuItem
            // 
            this.deleteFrameToolStripMenuItem.Name = "deleteFrameToolStripMenuItem";
            this.deleteFrameToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.deleteFrameToolStripMenuItem.Text = "Delete Frame";
            this.deleteFrameToolStripMenuItem.Click += new System.EventHandler(this.deleteFrameToolStripMenuItem_Click);
            // 
            // insertFrameToolStripMenuItem
            // 
            this.insertFrameToolStripMenuItem.Name = "insertFrameToolStripMenuItem";
            this.insertFrameToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.insertFrameToolStripMenuItem.Text = "Insert Frame";
            this.insertFrameToolStripMenuItem.Click += new System.EventHandler(this.insertFrameToolStripMenuItem_Click);
            // 
            // truncateMovieToolStripMenuItem
            // 
            this.truncateMovieToolStripMenuItem.Name = "truncateMovieToolStripMenuItem";
            this.truncateMovieToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.truncateMovieToolStripMenuItem.Text = "Truncate Movie";
            this.truncateMovieToolStripMenuItem.Click += new System.EventHandler(this.truncateMovieToolStripMenuItem_Click);
            // 
            // savestateThisFrameToolStripMenuItem
            // 
            this.savestateThisFrameToolStripMenuItem.Name = "savestateThisFrameToolStripMenuItem";
            this.savestateThisFrameToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.savestateThisFrameToolStripMenuItem.Text = "Savestate This Frame";
            this.savestateThisFrameToolStripMenuItem.Click += new System.EventHandler(this.savestateThisFrameToolStripMenuItem_Click);
            // 
            // b_JumptoCursor
            // 
            this.b_JumptoCursor.Location = new System.Drawing.Point(236, 699);
            this.b_JumptoCursor.Name = "b_JumptoCursor";
            this.b_JumptoCursor.Size = new System.Drawing.Size(99, 23);
            this.b_JumptoCursor.TabIndex = 5;
            this.b_JumptoCursor.Text = "Jump to Cursor";
            this.b_JumptoCursor.UseVisualStyleBackColor = true;
            this.b_JumptoCursor.Click += new System.EventHandler(this.b_JumptoCursor_Click);
            // 
            // cb_FollowCursor
            // 
            this.cb_FollowCursor.AutoSize = true;
            this.cb_FollowCursor.Checked = true;
            this.cb_FollowCursor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_FollowCursor.Location = new System.Drawing.Point(26, 703);
            this.cb_FollowCursor.Name = "cb_FollowCursor";
            this.cb_FollowCursor.Size = new System.Drawing.Size(89, 17);
            this.cb_FollowCursor.TabIndex = 6;
            this.cb_FollowCursor.Text = "Follow Cursor";
            this.cb_FollowCursor.UseVisualStyleBackColor = true;
            // 
            // tb_FollowDistance
            // 
            this.tb_FollowDistance.LargeChange = 1;
            this.tb_FollowDistance.Location = new System.Drawing.Point(118, 699);
            this.tb_FollowDistance.Maximum = 39;
            this.tb_FollowDistance.Name = "tb_FollowDistance";
            this.tb_FollowDistance.Size = new System.Drawing.Size(104, 45);
            this.tb_FollowDistance.TabIndex = 8;
            this.tb_FollowDistance.Value = 20;
            this.tb_FollowDistance.Scroll += new System.EventHandler(this.tb_FollowDistance_Scroll);
            // 
            // cb_SavestateEveryFrame
            // 
            this.cb_SavestateEveryFrame.AutoSize = true;
            this.cb_SavestateEveryFrame.Location = new System.Drawing.Point(409, 112);
            this.cb_SavestateEveryFrame.Name = "cb_SavestateEveryFrame";
            this.cb_SavestateEveryFrame.Size = new System.Drawing.Size(136, 17);
            this.cb_SavestateEveryFrame.TabIndex = 9;
            this.cb_SavestateEveryFrame.Text = "Savestate Every Frame";
            this.cb_SavestateEveryFrame.UseVisualStyleBackColor = true;
            // 
            // tb_TempSavestates
            // 
            this.tb_TempSavestates.Location = new System.Drawing.Point(410, 132);
            this.tb_TempSavestates.MaxLength = 5;
            this.tb_TempSavestates.Name = "tb_TempSavestates";
            this.tb_TempSavestates.Size = new System.Drawing.Size(41, 20);
            this.tb_TempSavestates.TabIndex = 10;
            this.tb_TempSavestates.Text = "120";
            this.Tooltip_.SetToolTip(this.tb_TempSavestates, resources.GetString("tb_TempSavestates.ToolTip"));
            this.tb_TempSavestates.TextChanged += new System.EventHandler(this.tb_TempSavestates_TextChanged);
            this.tb_TempSavestates.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_FilterForNumbers);
            // 
            // label_TempSavestates
            // 
            this.label_TempSavestates.AutoSize = true;
            this.label_TempSavestates.Location = new System.Drawing.Point(457, 139);
            this.label_TempSavestates.Name = "label_TempSavestates";
            this.label_TempSavestates.Size = new System.Drawing.Size(139, 13);
            this.label_TempSavestates.TabIndex = 11;
            this.label_TempSavestates.Text = "Temporary Savestate Count";
            this.Tooltip_.SetToolTip(this.label_TempSavestates, resources.GetString("label_TempSavestates.ToolTip"));
            // 
            // Tooltip_
            // 
            this.Tooltip_.AutoPopDelay = 500000;
            this.Tooltip_.InitialDelay = 500;
            this.Tooltip_.ReshowDelay = 100;
            // 
            // label_AutoSavestateThreshold
            // 
            this.label_AutoSavestateThreshold.AutoSize = true;
            this.label_AutoSavestateThreshold.Location = new System.Drawing.Point(457, 165);
            this.label_AutoSavestateThreshold.Name = "label_AutoSavestateThreshold";
            this.label_AutoSavestateThreshold.Size = new System.Drawing.Size(130, 13);
            this.label_AutoSavestateThreshold.TabIndex = 13;
            this.label_AutoSavestateThreshold.Text = "Auto-Savestate Threshold";
            this.Tooltip_.SetToolTip(this.label_AutoSavestateThreshold, "Automatically make a savestate every \'n\' frames.\r\nA larger value will consume les" +
        "s RAM, but leave gaps that will need to be re-emulated when loading earlier fram" +
        "es.\r\n");
            // 
            // tb_AutoSavestateThreshold
            // 
            this.tb_AutoSavestateThreshold.Location = new System.Drawing.Point(410, 158);
            this.tb_AutoSavestateThreshold.MaxLength = 5;
            this.tb_AutoSavestateThreshold.Name = "tb_AutoSavestateThreshold";
            this.tb_AutoSavestateThreshold.Size = new System.Drawing.Size(41, 20);
            this.tb_AutoSavestateThreshold.TabIndex = 12;
            this.tb_AutoSavestateThreshold.Text = "500";
            this.Tooltip_.SetToolTip(this.tb_AutoSavestateThreshold, "Automatically make a savestate every \'n\' frames.\r\nA larger value will consume les" +
        "s RAM, but leave gaps that will need to be re-emulated when loading earlier fram" +
        "es.");
            this.tb_AutoSavestateThreshold.TextChanged += new System.EventHandler(this.tb_AutoSavestateThreshold_TextChanged);
            this.tb_AutoSavestateThreshold.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_FilterForNumbers);
            // 
            // cb_RecordInputs
            // 
            this.cb_RecordInputs.AutoSize = true;
            this.cb_RecordInputs.Location = new System.Drawing.Point(409, 89);
            this.cb_RecordInputs.Name = "cb_RecordInputs";
            this.cb_RecordInputs.Size = new System.Drawing.Size(93, 17);
            this.cb_RecordInputs.TabIndex = 14;
            this.cb_RecordInputs.Text = "Record Inputs";
            this.Tooltip_.SetToolTip(this.cb_RecordInputs, "If checked, the inputs you provide will be recorded on the timeline.\r\nThis will o" +
        "verwrite older frames if you rewind.");
            this.cb_RecordInputs.UseVisualStyleBackColor = true;
            // 
            // TriCTASTimeline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 743);
            this.Controls.Add(this.cb_RecordInputs);
            this.Controls.Add(this.label_AutoSavestateThreshold);
            this.Controls.Add(this.tb_AutoSavestateThreshold);
            this.Controls.Add(this.label_TempSavestates);
            this.Controls.Add(this.tb_TempSavestates);
            this.Controls.Add(this.cb_SavestateEveryFrame);
            this.Controls.Add(this.tb_FollowDistance);
            this.Controls.Add(this.timelineScrollbar);
            this.Controls.Add(this.pb_Timeline);
            this.Controls.Add(this.cb_FollowCursor);
            this.Controls.Add(this.b_JumptoCursor);
            this.Controls.Add(this.b_play);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.b_FrameBack);
            this.Controls.Add(this.b_FrameAdvance);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TriCTASTimeline";
            this.Text = "TAS Timeline";
            ((System.ComponentModel.ISupportInitialize)(this.pb_Timeline)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip_Timeline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tb_FollowDistance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button b_FrameAdvance;
        private System.Windows.Forms.VScrollBar timelineScrollbar;
        private System.Windows.Forms.Button b_FrameBack;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tASToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTASToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTASToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportTor08ToolStripMenuItem;
        private System.Windows.Forms.Button b_play;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Timeline;
        private System.Windows.Forms.ToolStripMenuItem deleteFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem truncateMovieToolStripMenuItem;
        private System.Windows.Forms.Button b_JumptoCursor;
        private System.Windows.Forms.CheckBox cb_FollowCursor;
        private System.Windows.Forms.PictureBox pb_Timeline;
        private System.Windows.Forms.TrackBar tb_FollowDistance;
        private System.Windows.Forms.ToolStripMenuItem savestateThisFrameToolStripMenuItem;
        private System.Windows.Forms.CheckBox cb_SavestateEveryFrame;
        private System.Windows.Forms.TextBox tb_TempSavestates;
        private System.Windows.Forms.Label label_TempSavestates;
        private System.Windows.Forms.ToolTip Tooltip_;
        private System.Windows.Forms.Label label_AutoSavestateThreshold;
        private System.Windows.Forms.TextBox tb_AutoSavestateThreshold;
        private System.Windows.Forms.ToolStripMenuItem saveWithSavestatesToolStripMenuItem;
        private CheckBox cb_RecordInputs;
        private ToolStripMenuItem cellFrequencyToolStripMenuItem;
        private ToolStripMenuItem perVBlankToolStripMenuItem;
        private ToolStripMenuItem perControllerStrobeToolStripMenuItem;
    }
}