using SDL2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace TriCNES
{
    public partial class TriCNESGUI : Form
    {
        // This is the the main window for a user to interact with this emulator.
        // The logic for the emulator is contained entirely in a single C# file, for easy use importing it into other projects.
        // this form here is intended to be used an an example.
        // The intended use for this emulator is to run your own code specifically to collect data, but do with it as you please.
        // Cheers! ~ Chris "100th_Coin" Siebert
        public TriCNESGUI()
        {
            InitializeComponent();
            pb_Screen.DragEnter += new DragEventHandler(pb_Screen_DragEnter);
            pb_Screen.DragDrop += new DragEventHandler(pb_Screen_DragDrop);
            FormClosing += new FormClosingEventHandler(TriCNESGUI_Closing);
            SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER);
            SDL.SDL_GameControllerEventState(SDL.SDL_ENABLE);
            SDL.SDL_GameControllerUpdate();
            int c = SDL.SDL_NumJoysticks();
            if (c != 0)
            {
                joystickptr = SDL.SDL_JoystickOpen(0);
                gameControllerPrt = SDL.SDL_GameControllerOpen(0);
            }
        }
        IntPtr joystickptr;
        IntPtr gameControllerPrt;

        bool settings_ntsc;
        bool settings_ntscRaw;
        bool settings_border;
        byte settings_alignment;

        public Emulator EMU;
        public Thread EmuClock;
        string filePath;
        TASProperties TASPropertiesForm;
        TASProperties3ct TASPropertiesForm3ct;
        public TriCTraceLogger? TraceLogger;
        public TriCNTViewer? NametableViewer;
        public TriCTASTimeline? TasTimeline;
        public TriCHexEditor? HexExditor;

        void RunUpkeep()
        {
            if (PendingScreenshot)
            {
                PendingScreenshot = false;
                if (EMU.PPU_DecodeSignal)
                {
                    if (EMU.PPU_ShowScreenBorders)
                    {
                        Clipboard.SetImage(EMU.BorderedNTSCScreen.Bitmap);
                    }
                    else
                    {
                        Clipboard.SetImage(EMU.NTSCScreen.Bitmap);
                    }
                }
                else
                {
                    if (EMU.PPU_ShowScreenBorders)
                    {
                        Clipboard.SetImage(EMU.BorderedScreen.Bitmap);
                    }
                    else
                    {
                        Clipboard.SetImage(EMU.Screen.Bitmap);
                    }
                }
            }
            if (Pending_ShowScreenBorders)
            {
                Pending_ShowScreenBorders = false;
                EMU.PPU_ShowScreenBorders = true;
                BeginInvoke(new MethodInvoker(delegate () { ResizeWindow(ScreenMult); }));
            }
            if (Pending_HideScreenBorders)
            {
                Pending_HideScreenBorders = false;
                EMU.PPU_ShowScreenBorders = false;
                BeginInvoke(new MethodInvoker(delegate () { ResizeWindow(ScreenMult); }));
            }
            if (PendingSaveState)
            {
                PendingSaveState = false;
                Savestate = EMU.SaveState();
            }
            if (PendingLoadState && Savestate != null && Savestate.Count > 0)
            {
                PendingLoadState = false;
                EMU.LoadState(Savestate);
            }
            if (TraceLogger != null)
            {
                EMU.Logging = TraceLogger.Logging;
                if (EMU.DebugLog == null)
                {
                    EMU.DebugLog = new StringBuilder();
                }
                EMU.DebugRange_Low = TraceLogger.RangeLow;
                EMU.DebugRange_High = TraceLogger.RangeHigh;
                EMU.OnlyDebugInRange = TraceLogger.OnlyDebugInRange();
                EMU.LoggingPPU = TraceLogger.LogPPUCycles();
            }
            else if(EMU.Logging)
            {
                EMU.Logging = false;
                EMU.DebugLog = new StringBuilder();
            }
            if(HexExditor != null)
            {
                HexExditor.Update();
            }
        }

        void RunPostFramePhase()
        {
            if (TraceLogger != null)
            {
                if (TraceLogger.Logging)
                {
                    TraceLogger.Update();
                    if (TraceLogger.ClearEveryFrame())
                    {
                        EMU.DebugLog = new StringBuilder();
                    }
                }
            }
            if (NametableViewer != null && !NametableViewer.IsDisposed)
            {
                RenderNametable();
                NametableViewer.Update(NametableBitmap.Bitmap);
            }
            if (pb_Screen.InvokeRequired)
            {
                pb_Screen.BeginInvoke(new MethodInvoker(
                delegate ()
                {
                    if (EMU.PPU_DecodeSignal)
                    {
                        if (EMU.PPU_ShowScreenBorders)
                        {
                            pb_Screen.Image = EMU.BorderedNTSCScreen.Bitmap;
                        }
                        else
                        {
                            pb_Screen.Image = EMU.NTSCScreen.Bitmap;
                        }
                    }
                    else
                    {
                        if (EMU.PPU_ShowScreenBorders)
                        {
                            pb_Screen.Image = EMU.BorderedScreen.Bitmap;
                        }
                        else
                        {
                            pb_Screen.Image = EMU.Screen.Bitmap;
                        }
                    }
                    pb_Screen.Update();
                }));
            }
            else
            {
                if (EMU.PPU_DecodeSignal)
                {
                    if (EMU.PPU_ShowScreenBorders)
                    {
                        pb_Screen.Image = EMU.BorderedNTSCScreen.Bitmap;
                    }
                    else
                    {
                        pb_Screen.Image = EMU.NTSCScreen.Bitmap;
                    }
                }
                else
                {
                    if (EMU.PPU_ShowScreenBorders)
                    {
                        pb_Screen.Image = EMU.BorderedScreen.Bitmap;
                    }
                    else
                    {
                        pb_Screen.Image = EMU.Screen.Bitmap;
                    }
                }
                pb_Screen.Update();
            }
            
        }

        bool[] ControllerInputs()
        {
            bool[] joystickButtons = new bool[8];
            
            int c = SDL.SDL_NumJoysticks();
            if (c != 0)
            {
                SDL.SDL_GameControllerUpdate();

                joystickButtons[0] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT) != 0;
                joystickButtons[1] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT) != 0;
                joystickButtons[2] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN) != 0;
                joystickButtons[3] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP) != 0;
                joystickButtons[4] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START) != 0;
                joystickButtons[5] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK) != 0;
                joystickButtons[6] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X) != 0;
                joystickButtons[7] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A) != 0;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    joystickButtons[i] = false;
                }
            }
            return joystickButtons;
        }

        byte RealtimeInputs()
        {
            bool[] joystickButtons = ControllerInputs();

            byte controller1 = 0;
            if (joystickButtons[7] || Keyboard.IsKeyDown(Key.X)) { controller1 |= 0x80; }
            if (joystickButtons[6] || Keyboard.IsKeyDown(Key.Z)) { controller1 |= 0x40; }
            if (joystickButtons[5] || Keyboard.IsKeyDown(Key.RightShift)) { controller1 |= 0x20; }
            if (joystickButtons[4] || Keyboard.IsKeyDown(Key.Enter)) { controller1 |= 0x10; }
            if (joystickButtons[3] || Keyboard.IsKeyDown(Key.Up)) { controller1 |= 0x08; }
            if (joystickButtons[2] || Keyboard.IsKeyDown(Key.Down)) { controller1 |= 0x04; }
            if (joystickButtons[1] || Keyboard.IsKeyDown(Key.Left)) { controller1 |= 0x02; }
            if (joystickButtons[0] || Keyboard.IsKeyDown(Key.Right)) { controller1 |= 0x01; }
            return controller1;
        }

        CancellationTokenSource cancel;
        void ClockEmulator(CancellationToken ct)
        {
            int frameCount = 0;
            
            while (!ct.IsCancellationRequested)
            {
                if (Form.ActiveForm != null)
                {
                    if (Keyboard.IsKeyDown(Key.Q)) { PendingSaveState = true; }
                    if (Keyboard.IsKeyDown(Key.W)) { PendingLoadState = true; }
                                        
                    EMU.ControllerPort1 = RealtimeInputs();
                }
                RunUpkeep();
                EMU._CoreFrameAdvance();
                RunPostFramePhase();
                frameCount++;
            }            
        }

        DirectBitmap NametableBitmap;
        public Bitmap RenderNametable()
        {


            if (NametableBitmap != null)
            {
                NametableBitmap.Dispose();
            }
            NametableBitmap = new DirectBitmap(512, 480);
            if (EMU.Cart == null)
            {
                return NametableBitmap.Bitmap;
            }

            int tx = 0;
            int ty = 0;
            int x = 0;
            int y = 0;
            int px = 0;
            int py = 0;

            int PatternTile;
            int pal = 0;

            bool ForceBackdropOnIndex0 = NametableViewer.UseBackdrop();

            while (ty < 2)
            {
                while (tx < 2)
                {
                    while (y < 30)
                    {
                        while (x < 32)
                        {
                            PatternTile = EMU.FetchPPU((ushort)(0x2000 + 0x400 * tx + 0x800 * ty + x + y * 32));
                            pal = EMU.FetchPPU((ushort)(0x2000 + 0x400 * (tx + 1) + 0x800 * ty - 0x40 + x / 4 + (y / 4) * 8));
                            if ((x & 3) >= 2)
                            {
                                pal = pal >> 2;
                            }
                            if ((y & 3) >= 2)
                            {
                                pal = pal >> 4;
                            }
                            pal = pal & 3;
                            while (py < 8)
                            {
                                while (px < 8)
                                {

                                    int k = ((EMU.FetchPPU((ushort)(py + PatternTile * 16 + (!EMU.PPU_PatternSelect_Background ? 0 : 0x1000))) >> (7 - px)) & 1) + 2 * ((EMU.FetchPPU((ushort)(py + 8 + PatternTile * 16 + (!EMU.PPU_PatternSelect_Background ? 0 : 0x1000))) >> (7 - px)) & 1);
                                    if (k == 0 && ForceBackdropOnIndex0)
                                    {
                                        k = EMU.FetchPPU(0x3F00);
                                    }
                                    else
                                    {
                                        k = EMU.FetchPPU((ushort)(0x3F00 + k + pal * 4));
                                    }
                                    int col = unchecked((int)Emulator.NesPalInts[k & 0x3F]);
                                    NametableBitmap.SetPixel(tx * 0x100 + x * 8 + px, ty * 0xF0 + y * 8 + py, col);
                                    px++;
                                }
                                px = 0;
                                py++;
                            }
                            py = 0;
                            x++;
                        }

                        x = 0;
                        y++;
                    }
                    y = 0;
                    tx++;
                }
                tx = 0;
                ty++;
            }

            bool DrawScreenBoundary = NametableViewer.DrawBoundary();
            if (DrawScreenBoundary)
            {
                // convert the t register into X,Y coordinates
                /*
                The v and t registers are 15 bits:
                yyy NN YYYYY XXXXX
                ||| || ||||| +++++-- coarse X scroll
                ||| || +++++-------- coarse Y scroll
                ||| ++-------------- nametable select
                +++----------------- fine Y scroll
                */
                int X = ((EMU.PPU_TempVRAMAddress & 0b11111) << 3) | EMU.PPU_FineXScroll | ((EMU.PPU_TempVRAMAddress & 0b10000000000) >> 2);
                int Y = ((EMU.PPU_TempVRAMAddress & 0b1111100000) >> 2) | ((EMU.PPU_TempVRAMAddress & 0b111000000000000) >> 12) | ((EMU.PPU_TempVRAMAddress & 0b100000000000) >> 4);
                int i = 0;
                while (i <= 257)
                {
                    NametableBitmap.SetPixel((X + 511 + i) & 511, (Y + 479) % 480, Color.White);
                    NametableBitmap.SetPixel((X + 511 + i) & 511, (Y + 240) % 480, Color.White);
                    i++;
                }
                i = 0;
                while (i <= 241)
                {
                    NametableBitmap.SetPixel((X + 511) & 511, (Y + 479 + i) % 480, Color.White);
                    NametableBitmap.SetPixel((X + 256) & 511, (Y + 479 + i) % 480, Color.White);
                    i++;
                }
            }
            if (NametableViewer.OverlayScreen())
            {
                int X = ((EMU.PPU_TempVRAMAddress & 0b11111) << 3) | EMU.PPU_FineXScroll | ((EMU.PPU_TempVRAMAddress & 0b10000000000) >> 2);
                int Y = ((EMU.PPU_TempVRAMAddress & 0b1111100000) >> 2) | ((EMU.PPU_TempVRAMAddress & 0b111000000000000) >> 12) | ((EMU.PPU_TempVRAMAddress & 0b100000000000) >> 4);
                for (int xx = 0; xx < 256; xx++)
                {
                    for (int yy = 0; yy < 240; yy++)
                    {
                        NametableBitmap.SetPixel((X + xx) & 511, (Y + yy) % 480, EMU.Screen.GetPixel(xx, yy));
                    }
                }
            }
            return NametableBitmap.Bitmap;
        }

        void ClockEmulator3CT(CancellationToken ct)
        {
            Cartridge[] CartArray = TASPropertiesForm3ct.CartridgeArray;
            int[] CyclesToSwapOn = TASPropertiesForm3ct.CyclesToSwapOn.ToArray();
            int[] CartsToSwapIn = TASPropertiesForm3ct.CartsToSwapIn.ToArray();
            EMU.Cart = CartArray[0];

            int i = 1; // what cycle is being executed next?
            int j = 0; // what step of the .3ct TAS is this?
            while (j < CyclesToSwapOn.Length)
            {
                if (i == CyclesToSwapOn[j]) // if there's a cart swap on this cycle
                {
                    EMU.Cart = CartArray[CartsToSwapIn[j]]; // swap the cartridge to the next one in the list
                    j++;
                }
                EMU._CoreCycleAdvance();
                i++;
            }
            // once the .3ct TAS is completed, continue running the emulator with whatever cartridge is loaded last.
            while (!ct.IsCancellationRequested)
            {
                RunUpkeep();
                EMU._CoreFrameAdvance();
                RunPostFramePhase();
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
                    cancel.Cancel();
                    EmuClock.Join();
                }
                if (EMU != null)
                {
                    EMU.Dispose();
                    GC.Collect();
                }
                filePath = ofd.FileName;
                EMU = new Emulator();
                EMU.PPU_DecodeSignal = settings_ntsc;
                EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
                EMU.PPU_ShowScreenBorders = settings_border;
                EMU.PPUClock = settings_alignment;
                Cartridge Cart = new Cartridge(filePath);
                EMU.Cart = Cart;
                cancel = new CancellationTokenSource();
                EmuClock = new Thread(() => ClockEmulator(cancel.Token));
                EmuClock.SetApartmentState(ApartmentState.STA);
                EmuClock.IsBackground = true;
                EmuClock.Start();
                GC.Collect();
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
                "All TAS Files (.3c2, .3c3, .bk2, .tasproj, .fm2, .fm3, .fmv, .r08)|*.3c2;*.3c3;*.bk2;*.tasproj;*.fm2;*.fm3;*.fmv;*.r08" +
                "|TriCNES TAS File (.3c2, .3c3)|*.3c2;*.3c3" +
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
                if (TASPropertiesForm != null)
                {
                    TASPropertiesForm.Close();
                    TASPropertiesForm.Dispose();
                }
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
            if (filePath == "" || filePath == null)
            {
                MessageBox.Show("You need to select a ROM before running a TAS.");
                return;
            }

            if (EmuClock != null)
            {
                cancel.Cancel();
                EmuClock.Join();
            }

            if (EMU != null)
            {
                EMU.Dispose();
                GC.Collect();
            }

            EMU = new Emulator();
            EMU.PPU_DecodeSignal = settings_ntsc;
            EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
            EMU.PPU_ShowScreenBorders = settings_border;

            Cartridge Cart = new Cartridge(filePath);
            EMU.Cart = Cart;
            EMU.TAS_ReadingTAS = true;
            EMU.TAS_InputLog = TASPropertiesForm.TasInputLog;
            EMU.TAS_ResetLog = TASPropertiesForm.TasResetLog;
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
                        if (TASPropertiesForm.UseFCEUXFrame0Timing())
                        {
                            // FCEUX incorrectly starts at the beginning of scanline 240, and cycle 0 is *after* the reset instruction.
                            // However, I think there's some other incorrect timing going on with FCEUX, and in order to sync TASes, I need to start at scanline 239, dot 312
                            EMU.PPU_Scanline = 239;
                            EMU.PPU_Dot = 312;
                            // but of course, by starting here, the VBlank flag will be incorrectly set early.
                            EMU.SyncFM2 = true; // so this bool prevents that.
                            EMU.TAS_InputSequenceIndex--; // since this runs an extra vblank, this needs to be offset by 1
                        }
                        else
                        {
                            EMU.TAS_InputSequenceIndex++;
                            EMU.PPU_Dot = 0;
                        }
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
                case ".r08":
                    {
                        // This following comment block can be removed if you want to set up RAM for the Bad Apple TAS's .r08 file.
                        /*
                        string s = "0000000000000C000000000000000000E2000000001D1E000000000001000000984820BEFE68A8A5F7A6F8600000000010400000000000000000000000000000A2A58EFF07A216EA8EFD07020000000020200091318A11319131C8C430D0F14C40000000000000000101030000000000000000000000000000000000000000000000000000F000000000020000A0A000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000101000000000000000000000000000100000000000000000000000000000000000035000000008E002001008A4820BEFE68AA0C000000004C4000000001A804D9B4B4070004DAB4B4030004DBB4B4030005DCB4B4030004DDB4B4030004DEB4B4030004DFB4B4030004E0B4B4030004E1B4B4030004E2B4B4030004E3B4B4030004E4B4B4030004E5B4B4030004E6B4C886A080F5D000D00B00003F2FC7F8C8FE0024000F5200FB0400A9018D164085C04A8D1640AD16404A26C090F8A5C060A202206B0195C1CA10F8A000206B0191C2C8C4C190F6206B01F0E5206B0185C3206B0185C26CC200FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB00FB003AFB00FB00FB00FB10D2A27DA07DF50400040004D93525D8F70000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8410000F8410000F8250000F8250000F8410000F8410000F8250000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000F8010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000D900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000787A2021047F1918470000000000000000000000000000000000000000040400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000F722CC891000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001600A5";
                        int i = 0;
                        while (i < 0x800)
                        {
                            EMU.RAM[i] = byte.Parse(s.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                            i++;
                        }
                        */
                        break;
                    }
            }

            cancel = new CancellationTokenSource();
            EmuClock = new Thread(() => ClockEmulator(cancel.Token));
            EmuClock.SetApartmentState(ApartmentState.STA);
            EmuClock.IsBackground = true;
            EmuClock.Start();
            GC.Collect();
        }

        public void Start3CTTAS()
        {
            if (EmuClock != null)
            {
                cancel.Cancel();
                EmuClock.Join();
                EMU.Dispose();
            }
            if (TASPropertiesForm3ct.FromRESET())
            {
                if (EMU == null)
                {
                    MessageBox.Show("The emulator needs to be powered on before running from RESET.");
                    return;
                }
                EMU.Reset();
            }
            else
            {
                if (EMU != null)
                {
                    EMU.Dispose();
                    GC.Collect();
                }
                EMU = new Emulator();
                EMU.PPU_DecodeSignal = settings_ntsc;
                EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
                EMU.PPU_ShowScreenBorders = settings_border;
                EMU.PPUClock = settings_alignment;
            }
            cancel = new CancellationTokenSource();
            EmuClock = new Thread(() => ClockEmulator3CT(cancel.Token));
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
                if (TASPropertiesForm3ct != null)
                {
                    TASPropertiesForm3ct.Close();
                    TASPropertiesForm3ct.Dispose();
                }
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
            if (EMU != null)
            {
                EMU.Reset();
            }
        }

        private void powerCycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EMU != null)
            {
                Emulator Emu2 = new Emulator();
                Emu2.PPU_DecodeSignal = settings_ntsc;
                EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
                Emu2.PPU_ShowScreenBorders = settings_border;
                Emu2.PPUClock = settings_alignment;
                Emu2.Cart = EMU.Cart;
                EMU = Emu2;
            }
        }

        bool PendingScreenshot;
        private void screenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PendingScreenshot = true;
        }

        private void pb_Screen_DragEnter(object sender, DragEventArgs e)
        {
            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (Path.GetExtension(filenames[0]) == ".nes" || Path.GetExtension(filenames[0]) == ".NES") e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void pb_Screen_DragDrop(object sender, DragEventArgs e)
        {
            if (EmuClock != null)
            {
                cancel.Cancel();
                EmuClock.Join();
            }

            if (EMU != null)
            {
                EMU.Dispose();
                GC.Collect();
            }

            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string filename = filenames[0];
            filePath = filename;
            EMU = new Emulator();
            EMU.PPU_DecodeSignal = settings_ntsc;
            EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
            EMU.PPU_ShowScreenBorders = settings_border;
            EMU.PPUClock = settings_alignment;

            Cartridge Cart = new Cartridge(filePath);

            EMU.Cart = Cart;
            cancel = new CancellationTokenSource();
            EmuClock = new Thread(() => ClockEmulator(cancel.Token));
            EmuClock.SetApartmentState(ApartmentState.STA);
            EmuClock.IsBackground = true;
            EmuClock.Start();
            GC.Collect();

        }
        private void TriCNESGUI_Closing(Object sender, FormClosingEventArgs e)
        {
            if (EmuClock != null)
            {
                cancel.Cancel();
                EmuClock.Join();
            }
            if (TASPropertiesForm != null)
            {
                TASPropertiesForm.Dispose();
            }
            if (TASPropertiesForm3ct != null)
            {
                TASPropertiesForm3ct.Dispose();
            }
            if (TraceLogger != null)
            {
                TraceLogger.Dispose();
            }
            if (NametableViewer != null)
            {
                NametableViewer.Dispose();
            }
            if(TasTimeline != null)
            {
                TasTimeline.Dispose();
            }
            if (HexExditor != null)
            {
                HexExditor.Dispose();
            }
            Application.Exit();
        }

        private void phase0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            phase0ToolStripMenuItem.Checked = true;
            phase1ToolStripMenuItem.Checked = false;
            phase2ToolStripMenuItem.Checked = false;
            phase3ToolStripMenuItem.Checked = false;
            RebootWithAlignment(0);
        }

        private void phase1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            phase0ToolStripMenuItem.Checked = false;
            phase1ToolStripMenuItem.Checked = true;
            phase2ToolStripMenuItem.Checked = false;
            phase3ToolStripMenuItem.Checked = false;
            RebootWithAlignment(1);
        }

        private void phase2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            phase0ToolStripMenuItem.Checked = false;
            phase1ToolStripMenuItem.Checked = false;
            phase2ToolStripMenuItem.Checked = true;
            phase3ToolStripMenuItem.Checked = false;
            RebootWithAlignment(2);
        }

        private void phase3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            phase0ToolStripMenuItem.Checked = false;
            phase1ToolStripMenuItem.Checked = false;
            phase2ToolStripMenuItem.Checked = false;
            phase3ToolStripMenuItem.Checked = true;
            RebootWithAlignment(3);
        }

        private void RebootWithAlignment(byte Alignment)
        {
            if (EMU != null)
            {
                Emulator Emu2 = new Emulator();
                Emu2.Cart = EMU.Cart;
                EMU = Emu2;
                EMU.PPUClock = Alignment;
                EMU.CPUClock = 0;
                EMU.PPU_DecodeSignal = settings_ntsc;
                EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
                EMU.PPU_ShowScreenBorders = settings_border;
            }
            settings_alignment = Alignment;
        }

        private void trueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            falseToolStripMenuItem.Checked = false;
            showRawSignalsToolStripMenuItem.Checked = false;
            trueToolStripMenuItem.Checked = true;
            if (EMU != null)
            {
                EMU.PPU_DecodeSignal = true;
                EMU.PPU_ShowRawNTSCSignal = false;
            }
            settings_ntsc = true;
            settings_ntscRaw = false;
        }

        private void falseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trueToolStripMenuItem.Checked = false;
            showRawSignalsToolStripMenuItem.Checked = false;
            falseToolStripMenuItem.Checked = true;
            if (EMU != null)
            {
                EMU.PPU_DecodeSignal = false;
                EMU.PPU_ShowRawNTSCSignal = false;
            }
            settings_ntsc = false;
            settings_ntscRaw = false;
        }

        private void showRawSignalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trueToolStripMenuItem.Checked = false;
            showRawSignalsToolStripMenuItem.Checked = true;
            falseToolStripMenuItem.Checked = false;
            if (EMU != null)
            {
                EMU.PPU_DecodeSignal = true;
                EMU.PPU_ShowRawNTSCSignal = true;
            }
            settings_ntsc = true;
            settings_ntscRaw = true;
        }

        public void ResizeWindow(int scale)
        {
            int w = 256;
            int h = 240;
            if (EMU != null)
            {
                if (EMU.PPU_ShowScreenBorders)
                {
                    w = 341;
                    h = 262;
                }
            }

            Size pbs = new Size();
            pbs.Width = w * scale;
            pbs.Height = h * scale;
            Size ws = new Size();
            ws.Width = w * scale + 16;
            ws.Height = h * scale + 66;
            MinimumSize = ws;
            MaximumSize = ws;
            pb_Screen.Size = pbs;
            Width = ws.Width;
            Height = ws.Height;
        }

        int ScreenMult = 1;
        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScreenMult = 1;
            ResizeWindow(1);
        }

        private void xToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ScreenMult = 2;
            ResizeWindow(2);
        }

        private void xToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ScreenMult = 3;
            ResizeWindow(3);
        }

        private void xToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ScreenMult = 4;
            ResizeWindow(4);
        }

        private void xToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            ScreenMult = 5;
            ResizeWindow(5);
        }

        private void xToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            ScreenMult = 6;
            ResizeWindow(6);
        }

        private void xToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            ScreenMult = 7;
            ResizeWindow(7);
        }

        private void xToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            ScreenMult = 8;
            ResizeWindow(8);
        }

        private void traceLoggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(TraceLogger != null)
            {
                TraceLogger.Focus();
                return;
            }
            TraceLogger = new TriCTraceLogger();
            TraceLogger.MainGUI = this;
            TraceLogger.Init();
            TraceLogger.Show();
            TraceLogger.Location = Location;
        }
        bool Pending_ShowScreenBorders;
        private void trueToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolstrip_ViewBorders_False.Checked = false;
            toolstrip_ViewBorders_True.Checked = true;
            if (EMU != null)
            {
                Pending_ShowScreenBorders = true;
            }
            settings_border = true;
        }
        bool Pending_HideScreenBorders;
        private void falseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolstrip_ViewBorders_False.Checked = true;
            toolstrip_ViewBorders_True.Checked = false;
            if (EMU != null)
            {
                Pending_HideScreenBorders = true;
            }
            settings_border = false;
        }

        private void nametableViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(NametableViewer != null)
            {
                NametableViewer.Focus();
                return;
            }
            NametableViewer = new TriCNTViewer();
            NametableViewer.MainGUI = this;
            NametableViewer.Show();
            NametableViewer.Location = Location;
        }

        private void hexEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HexExditor != null)
            {
                HexExditor.Focus();
                return;
            }
            HexExditor = new TriCHexEditor();
            HexExditor.MainGUI = this;
            HexExditor.Show();
            HexExditor.Location = Location;
        }

        List<Byte> Savestate = new List<byte>();
        bool PendingSaveState = false;
        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PendingSaveState = true;
        }
        bool PendingLoadState = false;
        private void loadStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TasTimeline == null) // this would cause a desync otherwise, so forcefully prevent this.
            {
                PendingLoadState = true;
            }
        }

        private void tASTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(TasTimeline != null)
            {
                TasTimeline.Focus();
                return;
            }
            bool EMUExists = (EMU != null);
            if (!EMUExists)
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
                        cancel.Cancel();
                        EmuClock.Join();
                    }
                    filePath = ofd.FileName;
                    TasTimeline = new TriCTASTimeline(this);
                    TasTimeline.Show();
                    TasTimeline.Location = Location;
                }
            }
            else
            {
                TasTimeline = new TriCTASTimeline(this);
                TasTimeline.Show();
                TasTimeline.Location = Location;
            }
        }

        public List<ushort> ParseTasFile(string TasFilePath, out List<bool> Resets)
        {
            // determine file type
            string extension = Path.GetExtension(TasFilePath);
            // create list of inputs from the tas file, and make any settings changes if needed.
            byte[] ByteArray = File.ReadAllBytes(TasFilePath);
            List<ushort> TASInputs = new List<ushort>(); // Low byte is player 1, High byte is player 2.
            List<bool> TASResets = new List<bool>();

            switch (extension)
            {
                case ".bk2":
                case ".tasproj":
                    {
                        // .bk2 files are actually just .zip files!
                        // Let's yoink "Input Log.txt" from this .bk2 file
                        StringReader InputLog = new StringReader(new string(new StreamReader(ZipFile.OpenRead(TasFilePath).Entries.Where(x => x.Name.Equals("Input Log.txt", StringComparison.InvariantCulture)).FirstOrDefault().Open(), Encoding.UTF8).ReadToEnd().ToArray()));
                        // now to parse the input log!
                        InputLog.ReadLine(); // "[Input]"
                        string key = InputLog.ReadLine(); // "LogKey: ... "
                        bool Bk2_Port1 = key.Contains("P1");
                        bool Bk2_Port2 = key.Contains("P2");
                        string ln = InputLog.ReadLine();
                        ushort u = 0;
                        while (ln != null && ln.Length > 3)
                        {
                            int pipeIndex = ln.Substring(1, ln.Length - 1).IndexOf('|') + 1;
                            char[] lnCharArray = ln.ToCharArray();
                            bool reset = lnCharArray[pipeIndex - 1] == 'r';
                            u = 0;
                            if (Bk2_Port1)
                            {
                                u |= (ushort)(lnCharArray[pipeIndex + 1] == 'U' ? 0x08 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 2] == 'D' ? 0x04 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 3] == 'L' ? 0x02 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 4] == 'R' ? 0x01 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 5] == 'S' ? 0x10 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 6] == 's' ? 0x20 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 7] == 'B' ? 0x40 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 8] == 'A' ? 0x80 : 0);
                            }
                            else if (Bk2_Port2) // Are there any NES TASes that only feature controller 2?
                            {
                                u |= (ushort)(lnCharArray[pipeIndex + 1] == 'U' ? 0x0800 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 2] == 'D' ? 0x0400 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 3] == 'L' ? 0x0200 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 4] == 'R' ? 0x0100 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 5] == 'S' ? 0x1000 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 6] == 's' ? 0x2000 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 7] == 'B' ? 0x4000 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 8] == 'A' ? 0x8000 : 0);
                            }
                            if (Bk2_Port1 && Bk2_Port2)
                            {
                                pipeIndex = ln.Substring(pipeIndex + 1, ln.Length - 1 - pipeIndex).IndexOf('|') + pipeIndex + 1;
                                u |= (ushort)(lnCharArray[pipeIndex + 1] == 'U' ? 0x0800 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 2] == 'D' ? 0x0400 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 3] == 'L' ? 0x0200 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 4] == 'R' ? 0x0100 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 5] == 'S' ? 0x1000 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 6] == 's' ? 0x2000 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 7] == 'B' ? 0x4000 : 0);
                                u |= (ushort)(lnCharArray[pipeIndex + 8] == 'A' ? 0x8000 : 0);
                            }
                            TASInputs.Add(u);
                            TASResets.Add(reset);
                            ln = InputLog.ReadLine();
                            if (ln == "[/Input]")
                            {
                                break;
                            }
                        }
                    }
                    break;
                case ".fm2":
                    {
                        // change the alignment to use FCEUX's
                        // header info of varying size
                        // Every line of a header ends in $0A
                        // Every header section is named. Example: $0A "romFileName"
                        // Since the input log begins with "|" and none of the header section names begin with "|", I can assume $0A"|" is the start of the input log
                        bool fm2_UsePort0 = false;
                        bool fm2_UsePort1 = false;

                        int i = 0;
                        while (i < ByteArray.Length)
                        {
                            // parse for "port0 ?"
                            if (ByteArray[i] == 0x0A &&
                                ByteArray[i + 1] == 0x70 &&
                                ByteArray[i + 2] == 0x6F &&
                                ByteArray[i + 3] == 0x72 &&
                                ByteArray[i + 4] == 0x74 &&
                                ByteArray[i + 5] == 0x30 &&
                                ByteArray[i + 6] == 0x20
                                )
                            {
                                fm2_UsePort0 = ByteArray[i + 7] == 0x31;
                            }
                            // parse for "port1 ?"
                            if (ByteArray[i] == 0x0A &&
                                ByteArray[i + 1] == 0x70 &&
                                ByteArray[i + 2] == 0x6F &&
                                ByteArray[i + 3] == 0x72 &&
                                ByteArray[i + 4] == 0x74 &&
                                ByteArray[i + 5] == 0x31 &&
                                ByteArray[i + 6] == 0x20
                                )
                            {
                                fm2_UsePort1 = ByteArray[i + 7] == 0x31;
                            }

                            if (ByteArray[i] == 0x0A && ByteArray[i + 1] == 0x7C)
                            {
                                break;
                            }
                            i++;
                        }

                        ushort u = 0;

                        int Port0Index = 0;
                        int Port1Index = 0;

                        while (i < ByteArray.Length)
                        {
                            if (ByteArray[i] == 0x0A)
                            {
                                if (i == ByteArray.Length - 1)
                                {
                                    break;
                                }
                                if (ByteArray[i + 1] == 0x0A)
                                {
                                    // The .fm2 TAS file format supports empty rows. Formatting quirk?
                                    i++;
                                    continue;
                                }
                                if (ByteArray[i + 1] == 0x23)
                                {
                                    // The .fm2 TAS file format supports comments, in the following format:
                                    //\n### Comment
                                    // so basically, check for `#` as the next character.

                                    // And now we skip until the next new line.
                                    i++;
                                    continue;
                                }
                                bool reset = (ByteArray[i + 2] & 1) == 1;
                                if (fm2_UsePort0)
                                {
                                    Port0Index = i + 4;
                                    if (fm2_UsePort1)
                                    {
                                        Port1Index = i + 0xD;
                                    }
                                }
                                else if (fm2_UsePort1)
                                {
                                    Port1Index = i + 0x6;
                                }

                                u = 0;
                                if (fm2_UsePort0)
                                {
                                    u |= (ushort)(ByteArray[Port0Index] == 0x2E ? 0 : 1);
                                    u |= (ushort)(ByteArray[Port0Index + 1] == 0x2E ? 0 : 2);
                                    u |= (ushort)(ByteArray[Port0Index + 2] == 0x2E ? 0 : 4);
                                    u |= (ushort)(ByteArray[Port0Index + 3] == 0x2E ? 0 : 8);
                                    u |= (ushort)(ByteArray[Port0Index + 4] == 0x2E ? 0 : 0x10);
                                    u |= (ushort)(ByteArray[Port0Index + 5] == 0x2E ? 0 : 0x20);
                                    u |= (ushort)(ByteArray[Port0Index + 6] == 0x2E ? 0 : 0x40);
                                    u |= (ushort)(ByteArray[Port0Index + 7] == 0x2E ? 0 : 0x80);
                                }
                                if (fm2_UsePort1)
                                {
                                    u |= (ushort)(ByteArray[Port1Index] == 0x2E ? 0 : 0x100);
                                    u |= (ushort)(ByteArray[Port1Index + 1] == 0x2E ? 0 : 0x200);
                                    u |= (ushort)(ByteArray[Port1Index + 2] == 0x2E ? 0 : 0x400);
                                    u |= (ushort)(ByteArray[Port1Index + 3] == 0x2E ? 0 : 0x800);
                                    u |= (ushort)(ByteArray[Port1Index + 4] == 0x2E ? 0 : 0x1000);
                                    u |= (ushort)(ByteArray[Port1Index + 5] == 0x2E ? 0 : 0x2000);
                                    u |= (ushort)(ByteArray[Port1Index + 6] == 0x2E ? 0 : 0x4000);
                                    u |= (ushort)(ByteArray[Port1Index + 7] == 0x2E ? 0 : 0x8000);
                                }
                                TASInputs.Add(u);
                                TASResets.Add(reset);

                            }
                            i++;
                        }
                    }
                    break;
                case ".fm3":
                    {
                        // similar to fm2, this has a header of varying length.
                        // But it also contains significantly more metadata after the input log.
                        // we need to parse $0A"length "
                        bool fm3_UsePort0 = false;
                        bool fm3_UsePort1 = false;
                        int i = 0;
                        while (i < ByteArray.Length)
                        {
                            if (ByteArray[i] == 0x0A)
                            {
                                if (ByteArray[i] == 0x0A &&
                                ByteArray[i + 1] == 0x70 &&
                                ByteArray[i + 2] == 0x6F &&
                                ByteArray[i + 3] == 0x72 &&
                                ByteArray[i + 4] == 0x74 &&
                                ByteArray[i + 5] == 0x30 &&
                                ByteArray[i + 6] == 0x20
                                )
                                {
                                    fm3_UsePort0 = ByteArray[i + 7] == 0x31;
                                }
                                // parse for "port1 ?"
                                if (ByteArray[i] == 0x0A &&
                                    ByteArray[i + 1] == 0x70 &&
                                    ByteArray[i + 2] == 0x6F &&
                                    ByteArray[i + 3] == 0x72 &&
                                    ByteArray[i + 4] == 0x74 &&
                                    ByteArray[i + 5] == 0x31 &&
                                    ByteArray[i + 6] == 0x20
                                    )
                                {
                                    fm3_UsePort1 = ByteArray[i + 7] == 0x31;
                                }
                                // check if this is the header info for "length"
                                if (ByteArray[i] == 0x0A)
                                {
                                    if (ByteArray[i + 1] == 0x6C &&
                                        ByteArray[i + 2] == 0x65 &&
                                        ByteArray[i + 3] == 0x6E &&
                                        ByteArray[i + 4] == 0x67 &&
                                        ByteArray[i + 5] == 0x74 &&
                                        ByteArray[i + 6] == 0x68 &&
                                        ByteArray[i + 7] == 0x20)
                                    {
                                        // okay, so the length is in ascii...
                                        // let's figure out where the next $0A character is
                                        int next0A = i + 8;
                                        while (next0A < ByteArray.Length)
                                        {
                                            if (ByteArray[next0A] == 0x0A)
                                            {
                                                break;
                                            }
                                            next0A++;
                                        }
                                        // okay, so the string from i+8 though next0A is the length.
                                        byte[] StringArray = new byte[next0A - (i + 8)];
                                        Array.Copy(ByteArray, i + 8, StringArray, 0, StringArray.Length);
                                        int InputLogLength = int.Parse(Encoding.Default.GetString(StringArray));
                                        i = next0A + 2;
                                        int tempMul = 1;
                                        if (fm3_UsePort0) { tempMul++; }
                                        if (fm3_UsePort1) { tempMul++; }
                                        int InputLogByteLength = InputLogLength * tempMul;
                                        // first byte is always zero?
                                        // next byte is controller 1 (if enabled)
                                        // next byte is controller 2 (if enabled)
                                        ushort u = 0;
                                        while (i < next0A + 2 + InputLogByteLength)
                                        {
                                            bool reset = (ByteArray[i] & 1) == 1;
                                            i++;// dummy byte (?)
                                            u = 0;
                                            if (fm3_UsePort0) { u = ByteArray[i]; i++; }
                                            if (fm3_UsePort1) { u |= (ushort)(ByteArray[i] << 8); i++; }
                                            TASInputs.Add(u);
                                            TASResets.Add(reset);
                                        }

                                    }

                                }

                            }
                            i++;

                        }



                    }
                    break;
                case ".fmv":
                    {
                        int i = 0x90; // there's a 144 byte header
                        bool fmv_UseController2 = (ByteArray[5] & 0b00010000) != 0;
                        if (fmv_UseController2)
                        {
                            while (i < ByteArray.Length)
                            {
                                ushort u = (ushort)(FamtasiaInput2Standard(ByteArray[i]) | (FamtasiaInput2Standard(ByteArray[i + 1]) << 8));
                                TASInputs.Add(u);
                                i += 2;
                            }
                        }
                        else
                        {
                            while (i < ByteArray.Length)
                            {
                                TASInputs.Add(FamtasiaInput2Standard(ByteArray[i]));
                                i++;
                            }
                        }
                    }
                    break;
                case ".3c2":
                    {
                        // The .3c2 format is pretty much identical to the .r08 file format, but with a 1-byte header.
                        // Bit 0: 0 = Latch Filtering. 1 = Clock Filtering.
                        // Bit 1: 0 = Only controller 1. 1 = Controller 1 and controller 2.
                        // Bit 2: 0 = No reset button. 1 = The reset button is used in this TAS.

                        bool UseController2 = (ByteArray[0] & 2) != 0;
                        bool UseReset = (ByteArray[0] & 4) != 0;

                        byte b = 0;
                        byte b2 = 0;
                        int i = 1;
                        while (i < ByteArray.Length)
                        {
                            b = ByteArray[i];
                            i++;
                            if (UseController2)
                            {
                                b2 = ByteArray[i];
                                i++;
                            }
                            TASInputs.Add((ushort)(b | (b2 << 8)));
                            if (UseReset)
                            {
                                bool res = (ByteArray[i] & 0x80) == 0x80; // I use bit 7 for the reset button. (bit 0 is for lag frames in the .3c3 format.)
                                TASResets.Add(res);
                                i++;
                            }
                        }
                        TASInputs.Add(0); // append a zero to the end for safe measure.
                    }
                    break;
                case ".r08":
                    {
                        // the .r08 file format is conveniently already in the format I want for my emulator.
                        byte b = 0;
                        byte b2 = 0;
                        int i = 0;
                        while (i < ByteArray.Length)
                        {
                            b = ByteArray[i];
                            b2 = ByteArray[i + 1];
                            TASInputs.Add((ushort)(b | (b2 << 8)));
                            i += 2;
                        }
                        TASInputs.Add(0); // append a zero to the end for safe measure.
                    }
                    break;
                case ".3c3":
                    {
                        // .3c3 is the format for my TAS timeline.
                        // The big differences are:
                        // - .3c3 saves the savestate information
                        // - .3c3 saves the "lag frame" information as well. (So every frame is 3 bytes now.)

                        // .3c3 has a 16 byte header.
                        // It's just little-endian 32-bit integers, and the same 1-byte header used in .3c2's.
                        // The first one determines how many bytes are in every savestate.
                        // the second one determinines how many frames there are in this TAS.
                        // I guess that means there's a limit of 2,147,483,647 frames in a .3c3 TAS file. God help me if I ever feel compelled to challenge this.
                        // Then there's a handful of unused bytes. ByteArray[15] is the same format as the 1-byte header used in 3c2's.
                        // Bit 0: 0 = Latch Filtering. 1 = Clock Filtering.
                        // Bit 1: 0 = Only controller 1. 1 = Controller 1 and controller 2.
                        // Bit 2: 0 = No reset button. 1 = The reset button is used in this TAS.


                        int SavestateLength = ByteArray[0] | (ByteArray[1] << 8) | (ByteArray[2] << 16) | (ByteArray[3] << 24);
                        int rerecords = ByteArray[4] | (ByteArray[5] << 8) | (ByteArray[6] << 16) | (ByteArray[7] << 24);
                        int frameCount = ByteArray[8] | (ByteArray[9] << 8) | (ByteArray[10] << 16) | (ByteArray[11] << 24);

                        bool UseController2 = (ByteArray[15] & 2) != 0;
                        bool UseReset = (ByteArray[15] & 4) != 0;

                        List<List<byte>> saveStates = new List<List<byte>>();
                        List<List<byte>> saveStates2 = new List<List<byte>>();
                        List<bool> lagFrames = new List<bool>();

                        byte b = 0;
                        byte b2 = 0;
                        int i = 16;
                        while (i < frameCount * 3 + 16)
                        {
                            b = ByteArray[i];
                            i++;
                            if (UseController2)
                            {
                                b2 = ByteArray[i];
                                i++;
                            }
                            TASInputs.Add((ushort)(b | (b2 << 8)));
                            bool lagframe = (ByteArray[i] & 1) == 1; // I use bit 0 for the lag frame info.
                            lagFrames.Add(lagframe);
                            if (UseReset)
                            {
                                bool res = (ByteArray[i] & 0x80) == 0x80; // I use bit 7 for the reset button.
                                TASResets.Add(res);
                            }
                            i++;
                            saveStates.Add(new List<byte>());
                            saveStates2.Add(new List<byte>());

                        }

                        // and from here until you reach the end of the file, the data is arranged in the following format:
                        // [32-bit int declaring the frame number, and 'n' bytes for the save state at that frame.]

                        while (i < ByteArray.Length)
                        {
                            // read the 4 byte header.
                            int frameIndex = ByteArray[i] | (ByteArray[i + 1] << 8) | (ByteArray[i + 2] << 16) | (ByteArray[i + 3] << 24);
                            i += 4;
                            int j = 0;
                            while (j < SavestateLength)
                            {
                                saveStates[frameIndex].Add(ByteArray[i]);
                                i++;
                                j++;
                            }
                        }

                        if (TasTimeline != null)
                        {
                            TriCTASTimeline.TimelineSavestates = saveStates;
                            TriCTASTimeline.TimelineTempSavestates = saveStates2; // the empty list.
                            TriCTASTimeline.LagFrames = lagFrames;
                            TasTimeline.highestFrameEmulatedEver = frameCount - 1;
                            TasTimeline.frameEmulated = frameCount - 1;
                            TasTimeline.Rerecords = rerecords;
                        }
                    }
                    break;
                    // TODO: ask if the .tasd file format is a thing yet
            }
            if (TASResets.Count == 0) // If not using Resets, we still want to initialize the Resets list, in case they are added to the TAS timeline at a later point.
            {
                TASResets = new List<bool>(new bool[TASInputs.Count]);
            }

            Resets = TASResets;
            return TASInputs;
        }
        byte FamtasiaInput2Standard(byte input)
        {
            //famtasia format is SsABDULR
            byte b0 = (byte)(input & 0x8);
            byte b1 = (byte)(input & 0x4);
            byte b2 = (byte)(input & 0xC0);
            byte b3 = (byte)(input & 0x30);
            b0 >>= 1;
            b1 <<= 1;
            byte b4 = (byte)(b2 & 0x80);
            byte b5 = (byte)(b2 & 0x40);
            b4 >>= 1;
            b5 <<= 1;
            b2 = (byte)(b4 | b5);
            b2 >>= 2;
            b3 <<= 2;
            byte b = (byte)(b2 | b3 | b0 | b1 | (input & 0x3));
            return b;
        }

        public void CreateTASTimelineEmulator()
        {
            if (EmuClock != null)
            {
                cancel.Cancel();
                EmuClock.Join();
            }
            if (EMU != null)
            {
                EMU.Dispose();
                GC.Collect();
            }
            Timeline_Paused = true;
            EMU = new Emulator();
            EMU.PPU_DecodeSignal = settings_ntsc;
            EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
            EMU.PPU_ShowScreenBorders = settings_border;
            EMU.PPUClock = settings_alignment;
            Cartridge Cart = new Cartridge(filePath);
            EMU.Cart = Cart;
            cancel = new CancellationTokenSource();
            EmuClock = new Thread(() => ClockTimelineEmulator(cancel.Token));
            EmuClock.SetApartmentState(ApartmentState.STA);
            EmuClock.IsBackground = true;
            EmuClock.Start();
        }

        public bool Timeline_PendingPause;
        public bool Timeline_PendingResume;
        public bool Timeline_Paused;
        public bool Timeline_PendingFrameAdvance;
        public bool Timeline_PendingLoadState;
        public int Timeline_PendingFrameNumber;
        public bool Timeline_PendingHardReset;
        public bool Timeline_PendingArbitrarySavestate;
        public bool Timeline_AutoPlayUntilTarget;
        public int Timeline_AutoPlayTarget;
        public bool Timeline_PendingMouseDown;
        public bool Timeline_PendingMouseHeld;
        public bool Timeline_PendingResetScreen;
        public bool Timeline_PendingClockFiltering;

        public bool Timeline_HotkeyHeld_RShoulder;
        public bool Timeline_HotkeyHeld_RTrigger;
        public bool Timeline_HotkeyHeld_LTrigger;

        public List<byte> Timeline_LoadState;
        void ClockTimelineEmulator(CancellationToken ct)
        {

            while (!ct.IsCancellationRequested)
            {
                if (Timeline_PendingHardReset)
                {
                    Timeline_PendingHardReset = false;
                    if (EMU != null)
                    {
                        EMU.Dispose();
                        GC.Collect();
                    }
                    EMU = new Emulator();
                    EMU.PPU_DecodeSignal = settings_ntsc;
                    EMU.PPU_ShowRawNTSCSignal = settings_ntscRaw;
                    EMU.PPU_ShowScreenBorders = settings_border;
                    EMU.PPUClock = settings_alignment;
                    Cartridge Cart = new Cartridge(filePath);
                    EMU.Cart = Cart;
                    if (Timeline_PendingClockFiltering)
                    {
                        Timeline_PendingClockFiltering = false;
                        EMU.TASTimelineClockFiltering = true;
                    }
                }
                if (Timeline_PendingArbitrarySavestate)
                {
                    Timeline_PendingArbitrarySavestate = false; // pretty much only ever set when loading a TAS.
                    List<byte> state = EMU.SaveState();
                    TriCTASTimeline.TimelineSavestates.Add(state);
                    TriCTASTimeline.TimelineTempSavestates.Add(new List<byte>());
                }

                bool[] ControllerHotkeys = OtherControllerHotkeys();

                if (!Timeline_HotkeyHeld_RShoulder && ControllerHotkeys[0])
                {
                    Timeline_PendingPause = !Timeline_Paused;
                    Timeline_PendingResume = Timeline_Paused;
                }

                if ((ControllerHotkeys[2] && ControllerHotkeys[1]) || (!Timeline_HotkeyHeld_RTrigger && ControllerHotkeys[2]))
                {
                    Timeline_PendingFrameAdvance = true;
                    Timeline_PendingPause = !Timeline_Paused;
                }
                bool rewinding = false;
                if ((ControllerHotkeys[3] && ControllerHotkeys[1]) || (!Timeline_HotkeyHeld_LTrigger && ControllerHotkeys[3]))
                {
                    TasTimeline.FrameRewind();
                    Timeline_PendingPause = !Timeline_Paused;
                    rewinding = true;
                }

                Timeline_HotkeyHeld_RShoulder = ControllerHotkeys[0];
                Timeline_HotkeyHeld_RTrigger = ControllerHotkeys[2];
                Timeline_HotkeyHeld_LTrigger = ControllerHotkeys[3];

                if (Timeline_PendingPause)
                {
                    Timeline_PendingPause = false;
                    Timeline_Paused = true;
                    TasTimeline.ChangePlayPauseButtonText("Paused");
                }
                if (Timeline_PendingResume)
                {
                    Timeline_PendingResume = false;
                    Timeline_Paused = false;
                    TasTimeline.ChangePlayPauseButtonText("Running");
                }
                if (Timeline_PendingMouseDown)
                {
                    Timeline_PendingMouseDown = false;
                    TasTimeline.TimelineMouseDownEvent();
                }
                if (Timeline_PendingMouseHeld)
                {
                    Timeline_PendingMouseHeld = false;
                    TasTimeline.TimelineMouseHeldEvent();
                }
                if (Timeline_PendingLoadState)
                {
                    Timeline_PendingLoadState = false;
                    PendingLoadState = true;
                    Savestate = Timeline_LoadState;
                    TasTimeline.frameIndex = Timeline_PendingFrameNumber;
                }
                if (Timeline_PendingResetScreen)
                {
                    Timeline_PendingResetScreen = false;
                    pb_Screen.Invoke(new MethodInvoker(
                    delegate ()
                    {
                        Bitmap b = new Bitmap(pb_Screen.Image);
                        for (int x = 0; x < b.Width; x++) { for (int y = 0; y < b.Height; y++) { b.SetPixel(x, y, Color.Black); } }
                        pb_Screen.Image = b;
                        pb_Screen.Update();
                    }));
                }
                bool FrameAdvance = false;
                if (Timeline_PendingFrameAdvance)
                {
                    Timeline_PendingFrameAdvance = false;
                    FrameAdvance = true;
                }

                if (Timeline_AutoPlayUntilTarget && TasTimeline.frameIndex >= Timeline_AutoPlayTarget)
                {
                    Timeline_AutoPlayUntilTarget = false;
                }

                RunUpkeep();
                if (Timeline_Paused && !FrameAdvance && !Timeline_AutoPlayUntilTarget)
                {
                    Thread.Sleep(50);
                }
                else
                {
                    if (TasTimeline.frameIndex == TriCTASTimeline.TimelineSavestates.Count)
                    {
                        // create a savestate for the previous frame.
                        if (TasTimeline.SavestateEveryFrame())
                        {
                            List<byte> state = EMU.SaveState();
                            TriCTASTimeline.TimelineSavestates.Add(state);
                            TasTimeline.SavestateLength = state.Count;
                        }
                        else
                        {
                            List<byte> state = new List<byte>();
                            TriCTASTimeline.TimelineSavestates.Add(state);
                        }
                        if (TriCTASTimeline.TimelineSavestates[TasTimeline.frameIndex].Count > 0)
                        {
                            // if this savestate is not empty
                            List<byte> state = new List<byte>();
                            TriCTASTimeline.TimelineTempSavestates.Add(state);
                            TasTimeline.TrimTempSavestates();
                            //TriCTASTimeline.TEMPRerecordTracker.Add(TasTimeline.Rerecords);
                        }
                        else
                        {
                            // if this savestate is empty
                            List<byte> state = EMU.SaveState();
                            TriCTASTimeline.TimelineTempSavestates.Add(state);
                            TasTimeline.TrimTempSavestates();
                            //TriCTASTimeline.TEMPRerecordTracker.Add(TasTimeline.Rerecords);
                        }
                    }
                    else if (TasTimeline.frameIndex < TriCTASTimeline.TimelineTempSavestates.Count && TriCTASTimeline.TimelineTempSavestates[TasTimeline.frameIndex].Count == 0)
                    {
                        List<byte> state = EMU.SaveState();
                        TriCTASTimeline.TimelineTempSavestates[TasTimeline.frameIndex] = state;
                        TasTimeline.TrimTempSavestates();
                    }

                    if (TasTimeline.RecordInputs() && !rewinding)
                    {
                        byte realtimeInputs = RealtimeInputs();
                        if (TasTimeline.Player2())
                        {
                            EMU.ControllerPort2 = realtimeInputs;
                            EMU.ControllerPort1 = 0;
                        }
                        else
                        {
                            EMU.ControllerPort1 = realtimeInputs;
                            EMU.ControllerPort2 = 0;
                        }
                        ushort rimputs = (ushort)((EMU.ControllerPort2 << 8) | EMU.ControllerPort1);
                        TriCTASTimeline.Inputs[TasTimeline.frameIndex] = rimputs;
                        int row = TasTimeline.frameIndex - TasTimeline.TopFrame;
                        if (row >= 0 && row < 40)
                        {
                            TasTimeline.RecalculateTimelineRow(row, rimputs);
                            TasTimeline.RedrawTimelineRow(row, false);
                        }
                        if (TasTimeline.frameIndex < TasTimeline.frameEmulated)
                        {
                            TasTimeline.MarkStale(TasTimeline.frameIndex);
                            Timeline_PendingLoadState = false; // the MarkStale() function typically loads a savestate, but we don't want that here.
                        }
                    }
                    else
                    {
                        EMU.ControllerPort1 = (byte)(TriCTASTimeline.Inputs[TasTimeline.frameIndex] & 0xFF);
                        EMU.ControllerPort2 = (byte)((TriCTASTimeline.Inputs[TasTimeline.frameIndex] & 0xFF00) >> 8);
                    }

                    EMU._CoreFrameAdvance();
                    RunPostFramePhase();
                    if (TasTimeline.frameIndex < TriCTASTimeline.Resets.Count && TriCTASTimeline.Resets[TasTimeline.frameIndex])
                    {
                        EMU.Reset();
                    }

                    if (!EMU.TASTimelineClockFiltering || !EMU.LagFrame)
                    {
                        TasTimeline.FrameAdvance();
                    }
                    else
                    {
                        //Timeline_PendingFrameAdvance = true; // keep running until a non-lag frame.
                    }
                }
            }
        }

        public bool[] OtherControllerHotkeys()
        {
            bool[] joystickButtons = new bool[4];

            int c = SDL.SDL_NumJoysticks();
            if (c != 0)
            {
                SDL.SDL_GameControllerUpdate();

                joystickButtons[0] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER) != 0;
                joystickButtons[1] = SDL.SDL_GameControllerGetButton(gameControllerPrt, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER) != 0;
                joystickButtons[2] = SDL.SDL_GameControllerGetAxis(gameControllerPrt, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT) > 0.2;
                joystickButtons[3] = SDL.SDL_GameControllerGetAxis(gameControllerPrt, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT) > 0.2;

            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    joystickButtons[i] = false;
                }
            }
            return joystickButtons;
        }

    }

    /// <summary>
    /// Inherits from PictureBox; adds Interpolation Mode Setting
    /// </summary>
    public class PictureBoxWithInterpolationMode : PictureBox
    {
        public InterpolationMode InterpolationMode { get; set; }
        public PictureBoxWithInterpolationMode()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);
        }
    }

}
