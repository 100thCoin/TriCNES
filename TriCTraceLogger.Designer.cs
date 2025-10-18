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
            // TriCTraceLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 532);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.b_ToggleButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TriCTraceLogger";
            this.Text = "TriCTraceLogger";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox b_ToggleButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtb_TraceLog;
    }
}