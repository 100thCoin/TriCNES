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
using TriCNES.mappers;

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

        public bool FromRESET()
        {
            return rb_FromRES.Checked;
        }

        public Cartridge[] CartridgeArray;

        public void Init()
        {
            tb_FilePath.Text = TasFilePath;
            cb_ClockAlignment.SelectedIndex = 0;
            cb_ClockAlignment.Update();
            cb_CpuClock.SelectedIndex = 0;
            cb_CpuClock.Update();
            rb_FromPOW.Checked = true;
            rb_FromPOW.Update();
        }

        Cartridge BackupCart;

        private void b_RunTAS_Click(object sender, EventArgs e)
        {
            if (rb_FromPOW.Checked)
            {
                int i = 0;
                while (i < CartridgeArray.Length)
                {
                    CartridgeArray[i].PRGRAM = new byte[0x2000];
                    CartridgeArray[i].CHRRAM = new byte[0x2000];
                    Mapper MapperChip;
                    // clear all mapper stuff.
                    switch (CartridgeArray[i].MemoryMapper)
                    {
                        default:
                        case 0: MapperChip = new Mapper_NROM(); break;
                        case 1: MapperChip = new Mapper_MMC1(); break;
                        case 2: MapperChip = new Mapper_UxROM(); break;
                        case 3: MapperChip = new Mapper_CNROM(); break;
                        case 4: MapperChip = new Mapper_MMC3(); break;
                        case 7: MapperChip = new Mapper_AOROM(); break;
                        case 9: MapperChip = new Mapper_MMC2(); break;
                        case 69: MapperChip = new Mapper_FME7(); break;
                    }
                    MapperChip.Cart = CartridgeArray[i];
                    CartridgeArray[i].MapperChip = MapperChip;
                    i++;
                }
            }
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
                    if(i ==0)
                    {
                        BackupCart = new Cartridge(Dir + l);
                    }
                    if (MainGUI.EMU != null && MainGUI.EMU.Cart.Name == (Dir + l))
                    {
                        CartridgeArray[i] = MainGUI.EMU.Cart; // If running a TAS from RESET, we want to use the currently loaded cartridge
                    }
                    else
                    {
                        CartridgeArray[i] = new Cartridge(Dir + l);
                    }
                }
                else
                {
                    MessageBox.Show("TriCNES roms folder is missing a required ROM for this TAS!\n\nMissing ROM: \"" + l + "\"");
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
