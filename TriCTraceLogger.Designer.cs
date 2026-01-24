namespace TriCNES
{
    partial class TriCTraceLogger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriCTraceLogger));
            this.b_ToggleButton = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtb_TraceLog = new System.Windows.Forms.RichTextBox();
            this.cb_LogInRange = new System.Windows.Forms.CheckBox();
            this.tb_RangeLow = new System.Windows.Forms.TextBox();
            this.tb_RangeHigh = new System.Windows.Forms.TextBox();
            this.cb_ClearEveryFrame = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // b_ToggleButton
            // 
            this.b_ToggleButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.b_ToggleButton.AutoSize = true;
            this.b_ToggleButton.Location = new System.Drawing.Point(12, 497);
            this.b_ToggleButton.Name = "b_ToggleButton";
            this.b_ToggleButton.Size = new System.Drawing.Size(80, 23);
            this.b_ToggleButton.TabIndex = 0;
            this.b_ToggleButton.Text = "Start Logging";
            this.b_ToggleButton.UseVisualStyleBackColor = true;
            this.b_ToggleButton.CheckedChanged += new System.EventHandler(this.b_ToggleButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtb_TraceLog);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(931, 464);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trace Log";
            // 
            // rtb_TraceLog
            // 
            this.rtb_TraceLog.DetectUrls = false;
            this.rtb_TraceLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_TraceLog.Location = new System.Drawing.Point(7, 20);
            this.rtb_TraceLog.Name = "rtb_TraceLog";
            this.rtb_TraceLog.Size = new System.Drawing.Size(918, 438);
            this.rtb_TraceLog.TabIndex = 56;
            this.rtb_TraceLog.Text = "";
            this.rtb_TraceLog.WordWrap = false;
            // 
            // cb_LogInRange
            // 
            this.cb_LogInRange.AutoSize = true;
            this.cb_LogInRange.Location = new System.Drawing.Point(165, 498);
            this.cb_LogInRange.Name = "cb_LogInRange";
            this.cb_LogInRange.Size = new System.Drawing.Size(139, 17);
            this.cb_LogInRange.TabIndex = 2;
            this.cb_LogInRange.Text = "Only Log Within Range:";
            this.cb_LogInRange.UseVisualStyleBackColor = true;
            this.cb_LogInRange.CheckedChanged += new System.EventHandler(this.cb_LogInRange_CheckedChanged);
            // 
            // tb_RangeLow
            // 
            this.tb_RangeLow.Enabled = false;
            this.tb_RangeLow.Location = new System.Drawing.Point(298, 495);
            this.tb_RangeLow.Name = "tb_RangeLow";
            this.tb_RangeLow.ReadOnly = true;
            this.tb_RangeLow.Size = new System.Drawing.Size(32, 20);
            this.tb_RangeLow.TabIndex = 3;
            this.tb_RangeLow.Text = "0000";
            this.tb_RangeLow.TextChanged += new System.EventHandler(this.tb_RangeLow_TextChanged);
            // 
            // tb_RangeHigh
            // 
            this.tb_RangeHigh.Enabled = false;
            this.tb_RangeHigh.Location = new System.Drawing.Point(336, 495);
            this.tb_RangeHigh.Name = "tb_RangeHigh";
            this.tb_RangeHigh.ReadOnly = true;
            this.tb_RangeHigh.Size = new System.Drawing.Size(32, 20);
            this.tb_RangeHigh.TabIndex = 4;
            this.tb_RangeHigh.Text = "FFFF";
            this.tb_RangeHigh.TextChanged += new System.EventHandler(this.tb_RangeHigh_TextChanged);
            // 
            // cb_ClearEveryFrame
            // 
            this.cb_ClearEveryFrame.AutoSize = true;
            this.cb_ClearEveryFrame.Checked = true;
            this.cb_ClearEveryFrame.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ClearEveryFrame.Location = new System.Drawing.Point(165, 521);
            this.cb_ClearEveryFrame.Name = "cb_ClearEveryFrame";
            this.cb_ClearEveryFrame.Size = new System.Drawing.Size(133, 17);
            this.cb_ClearEveryFrame.TabIndex = 5;
            this.cb_ClearEveryFrame.Text = "Clear Log Every Frame";
            this.cb_ClearEveryFrame.UseVisualStyleBackColor = true;
            // 
            // TriCTraceLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 577);
            this.Controls.Add(this.cb_ClearEveryFrame);
            this.Controls.Add(this.tb_RangeHigh);
            this.Controls.Add(this.tb_RangeLow);
            this.Controls.Add(this.cb_LogInRange);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.b_ToggleButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(969, 616);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(969, 616);
            this.Name = "TriCTraceLogger";
            this.Text = "Trace Logger";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox b_ToggleButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtb_TraceLog;
        private System.Windows.Forms.CheckBox cb_LogInRange;
        private System.Windows.Forms.TextBox tb_RangeLow;
        private System.Windows.Forms.TextBox tb_RangeHigh;
        private System.Windows.Forms.CheckBox cb_ClearEveryFrame;
    }
}