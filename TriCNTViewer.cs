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
    public partial class TriCNTViewer : Form
    {
        public TriCNESGUI MainGUI;
        public TriCNTViewer()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(TriCNTViewer_Closing);
        }

        private void TriCNTViewer_Closing(Object sender, FormClosingEventArgs e)
        {
            MainGUI.NametableViewer = null;
            this.Dispose();
        }

        public void Update(Bitmap b)
        {
            MethodInvoker upd = delegate
            {
                pictureBox1.Image = b;
                pictureBox1.Update();
            };
            this.Invoke(upd);
        }

        public bool UseBackdrop()
        {
            return cb_ForcePal0ToBackdrop.Checked;
        }

        public bool DrawBoundary()
        {
            return cb_ScreenBoundary.Checked;
        }
        public bool OverlayScreen()
        {
            return cb_OverlayScreen.Checked;
        }

        private void screenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(pictureBox1.Image);
        }
    }
}
