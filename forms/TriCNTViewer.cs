using System;
using System.Drawing;
using System.Windows.Forms;

namespace TriCNES
{
    public partial class TriCNTViewer : Form
    {
        public TriCNESGUI MainGUI;
        public TriCNTViewer()
        {
            InitializeComponent();
            FormClosing += TriCNTViewer_Closing;
        }

        private void TriCNTViewer_Closing(Object sender, FormClosingEventArgs e)
        {
            if (MainGUI != null)
            {
                MainGUI.NametableViewer = null;
            }
            Dispose();
        }

        public void Update(Bitmap b)
        {
            MethodInvoker upd = delegate
            {
                pictureBox1.Image = b;
                pictureBox1.Update();
            };
            try
            {
                this.Invoke(upd);
            }
            catch (Exception e)
            {

            }
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
