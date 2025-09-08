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
                    // clear all mapper stuff.
                    CartridgeArray[i].Mapper_1_ShiftRegister = 0;
                    CartridgeArray[i].Mapper_1_Control = 0x0C;    //0x8000
                    CartridgeArray[i].Mapper_1_CHR0 = 0;              //0xA000
                    CartridgeArray[i].Mapper_1_CHR1 = 0;              //0xC000
                    CartridgeArray[i].Mapper_1_PRG = 0;               //0xE000
                    CartridgeArray[i].Mapper_1_PB=false;

                    // Mapper 2, UxROM
                    CartridgeArray[i].Mapper_2_Bank = 0; // any write to ROM

                    // Mapper 3, CNROM
                    CartridgeArray[i].Mapper_3_CHRBank=0; // any write to ROM

                    // Mapper 4, MMC3
                    CartridgeArray[i].Mapper_4_8000 = 0;      // The value written to $8000 (or any even address between $8000 and $9FFE)
                    CartridgeArray[i].Mapper_4_BankA = 0;     // The PRG bank between $A000 and $BFFF
                    CartridgeArray[i].Mapper_4_Bank8C = 0;    // The PRG bank that could either be at $8000 through 9FFF, or $C000 through $DFFF
                    CartridgeArray[i].Mapper_4_CHR_2K0 = 0;
                    CartridgeArray[i].Mapper_4_CHR_2K8 = 0;
                    CartridgeArray[i].Mapper_4_CHR_1K0 = 0;
                    CartridgeArray[i].Mapper_4_CHR_1K4 = 0;
                    CartridgeArray[i].Mapper_4_CHR_1K8 = 0;
                    CartridgeArray[i].Mapper_4_CHR_1KC = 0;
                    CartridgeArray[i].Mapper_4_IRQLatch = 0;
                    CartridgeArray[i].Mapper_4_IRQCounter=0;
                    CartridgeArray[i].Mapper_4_EnableIRQ = false;
                    CartridgeArray[i].Mapper_4_ReloadIRQCounter = false;
                    CartridgeArray[i].Mapper_4_NametableMirroring = false; // MMC3 has it's own way of controlling how the nametables are mirrored.
                    CartridgeArray[i].Mapper_4_PRGRAMProtect = 0;

                    // Mapper 7, AOROM
                    CartridgeArray[i].Mapper_7_BankSelect = 0;

                    // Mapper 69, Sunsoft FME-7
                    CartridgeArray[i].Mapper_69_CMD = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K0 = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K1 = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K2 = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K3 = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K4 = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K5 = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K6 = 0;
                    CartridgeArray[i].Mapper_69_CHR_1K7 = 0;
                    CartridgeArray[i].Mapper_69_Bank_6 = 0;
                    CartridgeArray[i].Mapper_69_Bank_6_isRAM=false;
                    CartridgeArray[i].Mapper_69_Bank_6_isRAMEnabled=false;
                    CartridgeArray[i].Mapper_69_Bank_8=0;
                    CartridgeArray[i].Mapper_69_Bank_A=0;
                    CartridgeArray[i].Mapper_69_Bank_C=0;
                    CartridgeArray[i].Mapper_69_NametableMirroring = 0; // 0 = Vertical              1 = Horizontal            2 = One Screen Mirroring from $2000 ("1ScA")            3 = One Screen Mirroring from $2400 ("1ScB")
                    CartridgeArray[i].Mapper_69_EnableIRQ = false;
                    CartridgeArray[i].Mapper_69_EnableIRQCounterDecrement = false;
                    CartridgeArray[i].Mapper_69_IRQCounter =0; // When enabled the 16-bit IRQ counter is decremented once per CPU cycle. When the IRQ counter is decremented from $0000 to $FFFF an IRQ is generated.

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
