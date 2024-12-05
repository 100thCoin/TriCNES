using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TriCNES
{
    public partial class TASProperties3ct : Form
    {
        public TASProperties3ct()
        {
            InitializeComponent();
        }

        public string TasFilePath;
        public ushort[] TasInputLog;
        public TriCNESGUI MainGUI;

        public byte GetPPUClockPhase()
        {
            return (byte)cb_ClockAlignment.SelectedIndex;
        }

        public byte GetCPUClockPhase()
        {
            return (byte)cb_CpuClock.SelectedIndex;
        }

        public Cartridge[] CartridgeArray;

        public void Init()
        {
            tb_FilePath.Text = TasFilePath;
            cb_ClockAlignment.SelectedIndex = 0;
            cb_ClockAlignment.Update();
            cb_CpuClock.SelectedIndex = 0;
            cb_CpuClock.Update();
        }


        private void b_RunTAS_Click(object sender, EventArgs e)
        {
            MainGUI.Start3CTTAS();
        }

        public List<int> CyclesToSwapOn;
        public List<int> CartsToSwapIn;
        private void b_LoadCartridges_Click(object sender, EventArgs e)
        {
            bool error = false;
            // check if rom folder is empty
            string Dir = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"roms\"))
            {
                Dir += @"roms\";
                if(Directory.GetFiles(Dir).Length == 0)
                {
                    MessageBox.Show("Loading a .3ct TAS requires your roms to be located in the TriCNES roms folder.");
                    return;
                }
            }
            // rom folder isn't empty!

            StringReader SR = new StringReader(File.ReadAllText(tb_FilePath.Text));
            string l = SR.ReadLine();
            int count = int.Parse(l);
            CartridgeArray = new Cartridge[count];
            int i = 0;
            while(i < count)
            {
                l = SR.ReadLine();
                if(File.Exists(Dir+l))
                {
                    CartridgeArray[i] = new Cartridge(Dir + l);
                }
                else
                {
                    MessageBox.Show("TriCNES roms folder is smissing a required ROM for this TAS!\n\nMissing ROM: \"" + l + "\"");
                    return;
                }
                i++;
            }
            // if all carts are now loaded.
            // let's also prepare the cycles to swap on, and the carts to swap in
            CyclesToSwapOn = new List<int>();
            CartsToSwapIn = new List<int>();

            l = SR.ReadLine();
            while (l != null)
            {
                // the format here is:
                //x y
                //x and y could be any length, but there's a space between them.

                string s = l.Substring(0, l.IndexOf(" "));
                CyclesToSwapOn.Add(int.Parse(s));
                s = l.Remove(0,s.Length+1);
                CartsToSwapIn.Add(int.Parse(s));
                l = SR.ReadLine();
            }


            b_RunTAS.Enabled = true;
        }
    }
}
