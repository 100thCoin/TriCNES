using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        }

        public void Init()
        {
            rtb_TraceLog.SelectionTabs = new int[] { 0, 56, 56 * 2, 56 * 3, 56 * 4, 56 * 5, 56 * 6, 56 * 7, 56 * 8, 56 * 9, 56 * 10 };
        }

        public void Update()
        {
            if (MainGUI.EMU.DebugLog != null)
            {
                MethodInvoker upd = delegate
                {
                    rtb_TraceLog.Text = MainGUI.EMU.DebugLog.ToString();
                };
                this.Invoke(upd);
            }
        }

        private void b_ToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            Logging = b_ToggleButton.Checked;
            b_ToggleButton.Text = Logging ? "Stop Logging" : "Start Logging";
        }
    }
}
