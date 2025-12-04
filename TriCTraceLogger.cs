using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TriCNES
{
    public partial class TriCTraceLogger : Form
    {
        public TriCNESGUI MainGUI;
        public bool Logging;
        public TriCTraceLogger()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(TriCTraceLogger_Closing);
        }

        private void TriCTraceLogger_Closing(Object sender, FormClosingEventArgs e)
        {
            MainGUI.TraceLogger = null;
        }

        public void Init()
        {
            rtb_TraceLog.SelectionTabs = new int[] { 0, 56, 56 * 2, 56 * 3, 56 * 4, 56 * 5, 56 * 6, 56 * 7, 56 * 8, 56 * 9, 56 * 10 };
        }
        String Log;
        public void Update()
        {
            if (MainGUI.EMU.DebugLog != null)
            {
                Log = MainGUI.EMU.DebugLog.ToString();
                MethodInvoker upd = delegate
                {
                    rtb_TraceLog.Text = Log;
                };
                this.BeginInvoke(upd);
            }
        }

        private void b_ToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            Logging = b_ToggleButton.Checked;
            b_ToggleButton.Text = Logging ? "Stop Logging" : "Start Logging";
        }

        private void cb_LogInRange_CheckedChanged(object sender, EventArgs e)
        {
            tb_RangeHigh.ReadOnly = !cb_LogInRange.Checked;
            tb_RangeHigh.Enabled = cb_LogInRange.Checked;
            tb_RangeLow.ReadOnly = !cb_LogInRange.Checked;
            tb_RangeLow.Enabled = cb_LogInRange.Checked;
        }

        private void tb_RangeLow_TextChanged(object sender, EventArgs e)
        {
            RangeLow = 0;
            ushort.TryParse(tb_RangeLow.Text, System.Globalization.NumberStyles.HexNumber, null, out RangeLow);
        }

        private void tb_RangeHigh_TextChanged(object sender, EventArgs e)
        {
            RangeHigh = 0xFFFF;
            ushort.TryParse(tb_RangeHigh.Text, System.Globalization.NumberStyles.HexNumber, null, out RangeHigh);
        }
        public ushort RangeLow;
        public ushort RangeHigh;

        public bool OnlyDebugInRange()
        {
            return cb_LogInRange.Checked;
        }
        public bool ClearEveryFrame()
        {
            return cb_ClearEveryFrame.Checked;
        }
    }
}
