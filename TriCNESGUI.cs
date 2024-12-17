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
using System.Threading;
using System.Security.Cryptography;

namespace TriCNES
{
    public partial class TriCNESGUI : Form
    {
        // This is the the main window for a user to interact with this emulator.
        // The logic for the emulator is contained entirely in 1 C# file, for easy use importing it into other projects.
        // this form here is intended to be used an an example.
        // The intended use for this emulator is to run your own code specifically to collect data, but do with it as you please.
        // Cheers! ~ Chris "100th_Coin" Siebert
        public TriCNESGUI()
        {
            InitializeComponent();
        }

        public Emulator EMU;
        Thread EmuClock;
        string CurrentROMFilePath;
        TASProperties TASPropertiesForm;
        TASProperties3ct TASPropertiesForm3ct;
        private object LockObject = new object();
        void ClockEmulator()
        {
            LockObject = pb_Screen;
            lock (LockObject)
            {
                while (true)
                {
                    EMU._CoreFrameAdvance();
                    if (pb_Screen.InvokeRequired)
                    {
                        pb_Screen.Invoke(new MethodInvoker(
                        delegate ()
                        {
                            pb_Screen.Image = EMU.Screen.Bitmap;
                        }));
                    }
                    else
                    {
                        pb_Screen.Image = EMU.Screen.Bitmap;
                    }

                }
            }
        }

        void ClockEmulator3CT()
        {
            Cartridge[] CartArray = TASPropertiesForm3ct.CartridgeArray;
            int[] CyclesToSwapOn = TASPropertiesForm3ct.CyclesToSwapOn.ToArray();
            int[] CartsToSwapIn = TASPropertiesForm3ct.CartsToSwapIn.ToArray();
            EMU.Cart = CartArray[0];
            lock (LockObject)
            {
                int i = 1; // what cycle is being executed next?
                int j = 0; // what step of the .3ct TAS is this?
                while(j < CyclesToSwapOn.Length)
                {
                    if(i == CyclesToSwapOn[j]) // if there's a cart swap on this cycle
                    {
                        EMU.Cart = CartArray[CartsToSwapIn[j]]; // swap the cartridge to the next one in the list
                        j++;
                    }
                    EMU._CoreCycleAdvance();
                    i++;
                }
                // once the .3ct TAS is completed, continue running the emulator with whatever cartridge is loaded last.
                while (true)
                {
                    EMU._CoreFrameAdvance();
                    pb_Screen.Image = EMU.Screen.Bitmap;
                }
            }
        }

        private void loadROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string InitDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"roms\"))
            {
                InitDirectory += @"roms\";
            }
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "",
                Filter = "NES ROM files (*.nes)|*.nes",
                Title = "Select file",
                InitialDirectory = InitDirectory
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (EmuClock != null)
                {
                    if (EmuClock.ThreadState != ThreadState.Stopped || EmuClock.ThreadState != ThreadState.Unstarted)
                    {
                        EmuClock.Abort();
                    }
                }
                CurrentROMFilePath = ofd.FileName;
                EMU = new Emulator();
                Cartridge Cart = new Cartridge(CurrentROMFilePath);
                EMU.Cart = Cart;
                EmuClock = new Thread(ClockEmulator);
                EmuClock.IsBackground = true;
                EmuClock.Start();
            }
        }

        private void loadTASToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string InitDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"tas\"))
            {
                InitDirectory += @"tas\";
            }
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "",
                Filter = 
                "All TAS Files (.bk2, .tasproj, .fm2, .fm3, .fmv, .r08)|*.bk2;*.tasproj;*.fm2;*.fm3;*.fmv;*.r08" +
                "|Bizhawk Movie (.bk2)|*.bk2" +
                "|Bizhawk TAStudio (.tasproj)|*.tasproj" +
                "|FCEUX Movie (.fm2)|*.fm2" +
                "|FCEUX TAS Editor (.fm3)|*.fm3" +
                "|Famtastia Movie (.fmv)|*.fmv" +
                "|Replay Device (.r08)|*.r08",
                Title = "Select file",
                InitialDirectory = InitDirectory
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TASPropertiesForm = new TASProperties();
                TASPropertiesForm.TasFilePath = ofd.FileName;
                TASPropertiesForm.MainGUI = this;
                TASPropertiesForm.Init();
                TASPropertiesForm.Show();
                TASPropertiesForm.Location = Location;
            }
        }

        public void StartTAS()
        {
            if (EmuClock != null)
            {
                if (EmuClock.ThreadState != ThreadState.Stopped || EmuClock.ThreadState != ThreadState.Unstarted)
                {
                    try
                    {
                        EmuClock.Abort();
                    }
                    catch(System.Threading.ThreadAbortException){}
                }
            }

            EMU = new Emulator();
            Cartridge Cart = new Cartridge(CurrentROMFilePath);
            EMU.Cart = Cart;
            EMU.TAS_ReadingTAS = true;
            EMU.TAS_InputLog = TASPropertiesForm.TasInputLog;
            EMU.ClockFiltering = TASPropertiesForm.SubframeInputs();
            EMU.PPUClock = TASPropertiesForm.GetPPUClockPhase();
            EMU.CPUClock = TASPropertiesForm.GetCPUClockPhase();
            EMU.TAS_InputSequenceIndex = 0;
            switch (TASPropertiesForm.extension)
            {
                case ".bk2":
                case ".tasproj":
                    {
                        int i = 0;
                        while (i < EMU.RAM.Length) //bizhawk RAM pattern
                        {
                            if ((i & 7) > 4)
                            {
                                EMU.RAM[i] = 0xFF;
                            }
                            else
                            {
                                EMU.RAM[i] = 0;
                            }
                            i++;
                        }
                    }
                    break;
                case ".fm2":
                case ".fm3":
                    {
                        // FCEUX incorrectly starts at the beginning of scanline 240, and cycle 0 is *after* the reset instruction.
                        // However, I think there's some other incorrect timing going on with FCEUX, and in order to sync TASes, I need to start at scanline 239, dot 312
                        EMU.PPU_Scanline = 239;
                        EMU.PPU_ScanCycle = 312;
                        // but of course, by starting here, the VBlank flag will be incorrectly set early.
                        EMU.SyncFM2 = true; // so this bool prevents that.
                        EMU.TAS_InputSequenceIndex--; // since this runs an extra vblank, this needs to be offset by 1
                        // FCEUX also starts with this RAM pattern
                        int i = 0;
                        while (i < EMU.RAM.Length) //bizhawk RAM pattern
                        {
                            if ((i & 7) > 4)
                            {
                                EMU.RAM[i] = 0xFF;
                            }
                            else
                            {
                                EMU.RAM[i] = 0;
                            }
                            i++;
                        }
                    }
                    break;
            }

            EmuClock = new Thread(ClockEmulator);
            EmuClock.IsBackground = true;
            EmuClock.Start();
        }

        public void Start3CTTAS()
        {
            if (EmuClock != null)
            {
                if (EmuClock.ThreadState != ThreadState.Stopped || EmuClock.ThreadState != ThreadState.Unstarted)
                {
                    try
                    {
                        EmuClock.Abort();
                    }
                    catch (System.Threading.ThreadAbortException) { }
                }
            }
            if (TASPropertiesForm3ct.FromRESET())
            {
                if(EMU == null)
                {
                    MessageBox.Show("The emulator needs to be powered on before running from RESET.");
                    return;
                }
                EMU.Reset();
            }
            else
            {
                EMU = new Emulator();
            }
            EmuClock = new Thread(ClockEmulator3CT);
            EmuClock.IsBackground = true;
            EmuClock.Start();
        }

        private void load3ctToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string InitDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"tas\"))
            {
                InitDirectory += @"tas\";
            }
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "",
                Filter =
                "3CT TAS Files (.3ct)|*.3ct",
                Title = "Select file",
                InitialDirectory = InitDirectory
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TASPropertiesForm3ct = new TASProperties3ct();
                TASPropertiesForm3ct.TasFilePath = ofd.FileName;
                TASPropertiesForm3ct.MainGUI = this;
                TASPropertiesForm3ct.Init();
                TASPropertiesForm3ct.Show();
                TASPropertiesForm3ct.Location = Location;
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EMU.Reset();
        }

        private void powerCycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Emulator Emu2 = new Emulator();
            Emu2.Cart = EMU.Cart;
            EMU = Emu2;
        }

        private void screenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(EMU.Screen.Bitmap);
        }
    }
}
