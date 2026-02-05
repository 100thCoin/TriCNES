using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace TriCNES
{
    public partial class TriCTASTimeline : Form
    {
        public Font Font_Consolas;
        public Brush Brush_LeftColumn;
        public Brush Brush_LeftColumn_Saved;
        public Brush Brush_LeftColumn_TempSaved;
        public Brush Brush_HighlightedCell;
        public Brush Brush_WhiteCellP1;
        public Brush Brush_WhiteCellP2;
        public Brush Brush_GreenCellP1;
        public Brush Brush_GreenCellP2;
        public Brush Brush_GreenCellP1_Stale;
        public Brush Brush_GreenCellP2_Stale;
        public Brush Brush_RedCellP1;
        public Brush Brush_RedCellP2;
        public Brush Brush_RedCellP1_Stale;
        public Brush Brush_RedCellP2_Stale;

        struct Vector2
        {
            public int x;
            public int y;
            public Vector2(int X, int Y)
            {
                x = X;
                y = Y;
            }
            public override string ToString()
            {
                return x.ToString() + ", " + y.ToString();
            }
        }

        public struct TimelineCell
        {
            public bool Checked;
            public bool LagFrame;
            public bool Stale;
            public bool Emulated;
            public TimelineCell(bool t)
            {
                Checked = false;
                LagFrame = false;
                Stale = false;
                Emulated = false;
            }
        }
        public int SavestateLength;
        public int Rerecords;
        public TriCTASTimeline(TriCNESGUI Maingui)
        {
            MainGUI = Maingui;
            InitializeComponent();
            Inputs = new List<ushort>();
            Inputs.Add(0);
            Font_Consolas = new Font("Consolas", 8);
            Brush_LeftColumn = new SolidBrush(Color.LightGray);
            Brush_LeftColumn_Saved = new SolidBrush(Color.Wheat);
            Brush_LeftColumn_TempSaved = new SolidBrush(Color.LemonChiffon);
            Brush_HighlightedCell = new SolidBrush(Color.LightBlue);
            Brush_WhiteCellP1 = new SolidBrush(Color.WhiteSmoke);
            Brush_WhiteCellP2 = new SolidBrush(Color.FromArgb(255, 230, 230, 230));
            Brush_GreenCellP1 = new SolidBrush(Color.FromArgb(255, 200, 240, 200));
            Brush_GreenCellP2 = new SolidBrush(Color.FromArgb(255, 190, 232, 190));
            Brush_GreenCellP1_Stale = new SolidBrush(Color.FromArgb(255, 205, 220, 205));
            Brush_GreenCellP2_Stale = new SolidBrush(Color.FromArgb(255, 195, 215, 195));
            Brush_RedCellP1 = new SolidBrush(Color.FromArgb(255, 240, 200, 200));
            Brush_RedCellP2 = new SolidBrush(Color.FromArgb(255, 232, 190, 190));
            Brush_RedCellP1_Stale = new SolidBrush(Color.FromArgb(255, 220, 205, 205));
            Brush_RedCellP2_Stale = new SolidBrush(Color.FromArgb(255, 215, 195, 195));

            Start();

            timelineBitmap = new Bitmap(80 + 16 * 16 + 1, 41 * 16 + 1);
            G = Graphics.FromImage(timelineBitmap);
            pb_Timeline.Image = timelineBitmap;
            pb_Timeline.MouseDown += mouseDownEvent;
            pb_Timeline.MouseUp += mouseUpEvent;
            pb_Timeline.ContextMenuStrip = contextMenuStrip_Timeline;
            pb_Timeline.MouseWheel += mouseWheelEvent;
            timelineScrollbar.ValueChanged += timelineScrollbar_ValueChanged;

            //loop timer
            loopTimer = new System.Timers.Timer();
            loopTimer.Interval = 50;// interval in milliseconds
            loopTimer.Enabled = false;
            loopTimer.Elapsed += loopTimerEvent;
            loopTimer.AutoReset = true;
            Shown += TriCTASTimeline_Shown;
        }

        private void mouseWheelEvent(object sender, MouseEventArgs e)
        {
            int result = timelineScrollbar.Value - Math.Sign(e.Delta);
            if (result > timelineScrollbar.Maximum - timelineScrollbar.LargeChange + 1)
            {
                result = timelineScrollbar.Maximum - timelineScrollbar.LargeChange + 1;
            }
            else if (result < 0)
            {
                result = 0;
            }

            timelineScrollbar.Value = result;
        }

        void TriCTASTimeline_Shown(object sender, EventArgs e)
        {
            RefreshTopOfTimeline();
        }

        void RefreshTopOfTimeline()
        {
            Rectangle[] GridOverlay = new Rectangle[]
                {
                new Rectangle(0,0,80,16),
                new Rectangle(80 + 16*0,0,16,16),
                new Rectangle(80 + 16*1,0,16,16),
                new Rectangle(80 + 16*2,0,16,16),
                new Rectangle(80 + 16*3,0,16,16),
                new Rectangle(80 + 16*4,0,16,16),
                new Rectangle(80 + 16*5,0,16,16),
                new Rectangle(80 + 16*6,0,16,16),
                new Rectangle(80 + 16*7,0,16,16),
                new Rectangle(80 + 16*8,0,16,16),
                new Rectangle(80 + 16*9,0,16,16),
                new Rectangle(80 + 16*10,0,16,16),
                new Rectangle(80 + 16*11,0,16,16),
                new Rectangle(80 + 16*12,0,16,16),
                new Rectangle(80 + 16*13,0,16,16),
                new Rectangle(80 + 16*14,0,16,16),
                new Rectangle(80 + 16*15,0,16,16)
                };
            G.FillRectangle(Brush_LeftColumn, new Rectangle(0, 0, 80 + 16 * 16, 16));
            G.DrawRectangles(Pens.Black, GridOverlay);
            G.DrawString(ClockFiltering ? "Input #" : "Frame #", Font_Consolas, Brushes.Black, 0, 0);
            G.DrawString("A", Font_Consolas, Brushes.Black, 80 + 16 * 0, 0);
            G.DrawString("B", Font_Consolas, Brushes.Black, 80 + 16 * 1, 0);
            G.DrawString("s", Font_Consolas, Brushes.Black, 80 + 16 * 2, 0);
            G.DrawString("S", Font_Consolas, Brushes.Black, 80 + 16 * 3, 0);
            G.DrawString("U", Font_Consolas, Brushes.Black, 80 + 16 * 4, 0);
            G.DrawString("D", Font_Consolas, Brushes.Black, 80 + 16 * 5, 0);
            G.DrawString("L", Font_Consolas, Brushes.Black, 80 + 16 * 6, 0);
            G.DrawString("R", Font_Consolas, Brushes.Black, 80 + 16 * 7, 0);
            G.DrawString("A", Font_Consolas, Brushes.Black, 80 + 16 * 8, 0);
            G.DrawString("B", Font_Consolas, Brushes.Black, 80 + 16 * 9, 0);
            G.DrawString("s", Font_Consolas, Brushes.Black, 80 + 16 * 10, 0);
            G.DrawString("S", Font_Consolas, Brushes.Black, 80 + 16 * 11, 0);
            G.DrawString("U", Font_Consolas, Brushes.Black, 80 + 16 * 12, 0);
            G.DrawString("D", Font_Consolas, Brushes.Black, 80 + 16 * 13, 0);
            G.DrawString("L", Font_Consolas, Brushes.Black, 80 + 16 * 14, 0);
            G.DrawString("R", Font_Consolas, Brushes.Black, 80 + 16 * 15, 0);
            RefreshTimeline();
        }

        public Graphics G;
        public Bitmap timelineBitmap;

        private static System.Timers.Timer loopTimer;
        private void loopTimerEvent(Object source, ElapsedEventArgs e)
        {
            //this does whatever you want to happen while clicking on the button
            MainGUI.Timeline_PendingMouseHeld = true;
        }

        public void TimelineMouseHeldEvent()
        {
            MethodInvoker upd = delegate
            {
                int mouseX = MousePosition.X - Left - pb_Timeline.Left - 8;
                int mouseY = MousePosition.Y - Top - pb_Timeline.Top - 48;

                int Column = mouseX >= 80 ? (mouseX - 80) / 16 : -1;
                if (Column > 15)
                {
                    Column = 15;
                }

                int Row = mouseY >= 0 ? mouseY / 16 : -1;
                if (Row > 39)
                {
                    Row = 39;
                }

                Vector2 mousePos = new Vector2(Column, Row);

                if (mouseHeld_initPos.y >= 0)
                {
                    if (mouseHeld_initPos.x >= 0)
                    {
                        // we clicked on a cell
                        int spos = Math.Min(mousePos.y, mouseHeld_initPos.y);
                        int tpos = Math.Max(mousePos.y, mouseHeld_initPos.y);
                        if (spos < 0)
                        {
                            spos = 0;
                        }

                        for (int i = spos; i <= tpos; i++)
                        {
                            bool state = GetCellInputStatus(new Vector2(mouseHeld_initPos.x, i));
                            if (state != mouseHeld_setInput)
                            {
                                ushort input = SetCellInputStatus(new Vector2(mouseHeld_initPos.x, i), mouseHeld_setInput);
                                TimelineGrid[i][mouseHeld_initPos.x + 1].Checked = mouseHeld_setInput;

                                RecalculateTimelineRow(i, input);
                                RedrawTimelineRow(i, false);
                            }
                        }

                        int checkStale = spos + TopFrame;
                        if (checkStale < frameEmulated)
                        {
                            MarkStale(checkStale);
                        }
                    }
                    else
                    {
                        // we clicked on the frame number.

                    }
                }
            };
            this.Invoke(upd);
        }

        public int TopFrame = 0;

        Vector2 mouseHeld_initPos;
        bool mouseHeld_setInput;
        private void mouseDownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            loopTimer.Enabled = true;
            int mouseX = MousePosition.X - Left - pb_Timeline.Left - 8;
            int mouseY = MousePosition.Y - Top - pb_Timeline.Top - 48;

            int Column = mouseX >= 80 ? (mouseX - 80) / 16 : -1;
            if (Column > 15)
            {
                Column = 15;
            }

            int Row = mouseY >= 0 ? mouseY / 16 : -1;
            if (Row > 39)
            {
                Row = 39;
            }

            mouseHeld_initPos = new Vector2(Column, Row);

            MainGUI.Timeline_PendingMouseDown = true;
        }

        public void TimelineMouseDownEvent()
        {
            MethodInvoker upd = delegate
            {
                if (mouseHeld_initPos.y >= 0)
                {
                    if (mouseHeld_initPos.x >= 0)
                    {
                        // we clicked on a cell
                        // calculate if that cell had an input or not.
                        mouseHeld_setInput = !GetCellInputStatus(mouseHeld_initPos);
                        ushort input = SetCellInputStatus(mouseHeld_initPos, mouseHeld_setInput);
                        TimelineGrid[mouseHeld_initPos.y][mouseHeld_initPos.x + 1].Checked = mouseHeld_setInput;

                        RecalculateTimelineRow(mouseHeld_initPos.y, input);
                        RedrawTimelineRow(mouseHeld_initPos.y, false);

                        int frameClicked = TopFrame + mouseHeld_initPos.y;
                        if (frameClicked < frameEmulated)
                        {
                            MarkStale(frameClicked);
                        }
                    }
                    else
                    {
                        // we clicked on the frame number.
                        // TODO: Move the cursor to this frame.
                        // - If there exists a savestate for this frame, then simply laod it.
                        // - Otherwise, we haven't seen this frame yet. Load the last savestate in the list, then emulate to this frame.
                        int frameClicked = TopFrame + mouseHeld_initPos.y;
                        if (frameClicked == frameIndex)
                        {
                            return; // we're already on that frame!
                        }

                        if (frameClicked < frameEmulated && (TimelineSavestates[frameClicked].Count == SavestateLength || TimelineTempSavestates[frameClicked].Count == SavestateLength))
                        {
                            int prevrow = frameIndex - TopFrame;
                            frameIndex = frameClicked - 1; // and we're good to go.
                            if (frameIndex == -1)
                            {
                                frameIndex = 0;
                                MainGUI.Timeline_LoadState = TimelineSavestates[0];
                                MainGUI.Timeline_PendingLoadState = true;
                                MainGUI.Timeline_PendingFrameNumber = 0;
                                UpdateTimelineRowStatus(prevrow);
                                RedrawTimelineRow(prevrow, false);
                                UpdateTimelineRowStatus(0);
                                RedrawTimelineRow(0, false);
                                MainGUI.Timeline_PendingResetScreen = true;
                            }
                            else
                            {
                                MainGUI.Timeline_LoadState = GrabMostRecentSavestate(); // This function updates frameIndex
                                MainGUI.Timeline_PendingLoadState = true;
                                MainGUI.Timeline_PendingFrameNumber = frameIndex;
                                MainGUI.Timeline_PendingFrameAdvance = true;

                                UpdateTimelineRowStatus(prevrow);
                                RedrawTimelineRow(prevrow, false);
                            }
                        }
                        else
                        {
                            int row = frameIndex - TopFrame;
                            int prevFrame = frameIndex;
                            frameIndex = frameClicked;
                            if (frameIndex > frameEmulated)
                            {
                                frameIndex = frameEmulated;
                            }

                            UpdateTimelineRowStatus(row);
                            RedrawTimelineRow(row, false);
                            if (frameIndex < 0)
                            {
                                frameIndex = 0;
                            }
                            else
                            {
                                bool Unneeded = false;
                                MainGUI.Timeline_LoadState = GrabMostRecentSavestate(prevFrame, out Unneeded); // This function updates frameIndex
                                if (!Unneeded)
                                {
                                    MainGUI.Timeline_PendingLoadState = true;
                                    MainGUI.Timeline_PendingFrameNumber = frameIndex;
                                }
                            }
                            // Now we need to emulate all the way until we reach the target frame.
                            MainGUI.Timeline_AutoPlayUntilTarget = true;
                            MainGUI.Timeline_AutoPlayTarget = frameClicked;
                            row = frameIndex - TopFrame;

                            UpdateTimelineRowStatus(row);
                            RedrawTimelineRow(row, false);

                            if (frameIndex > frameEmulated)
                            {
                                frameEmulated = frameIndex;
                            }

                            if (frameIndex > highestFrameEmulatedEver)
                            {
                                highestFrameEmulatedEver = frameIndex;
                            }

                            while (frameIndex >= Inputs.Count)
                            {
                                Inputs.Add(0);
                            }

                            if (Inputs.Count + 39 > timelineScrollbar.Maximum)
                            {
                                timelineScrollbar.Maximum = Inputs.Count + 38;
                            }
                        }
                    }
                }
            };
            this.Invoke(upd); // Use delegates here because this could otherwise modify the state of the timeline mid-rendering the timeline. (causing runtime errors)
        }

        public void MarkStale(int Frame)
        {
            frameEmulated = Frame; // mark everything after this as "stale"
            if (frameIndex >= Frame)
            {
                Rerecords++;
                frameIndex = Frame;
                TimelineSavestates.RemoveRange(frameIndex + 1, TimelineSavestates.Count - (frameIndex + 1));
                TimelineTempSavestates.RemoveRange(frameIndex + 1, TimelineTempSavestates.Count - (frameIndex + 1));
                //TEMPRerecordTracker.RemoveRange(frameIndex + 1, TEMPRerecordTracker.Count - (frameIndex + 1));
                MainGUI.Timeline_LoadState = GrabMostRecentSavestate(); // This function updates frameIndex
                System.GC.Collect();
                MainGUI.Timeline_PendingLoadState = true;
                MainGUI.Timeline_PendingFrameNumber = frameIndex;
            }

            RefreshTimeline();
        }
        private void mouseUpEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            loopTimer.Enabled = false;
        }

        bool GetCellInputStatus(Vector2 pos)
        {
            if (pos.x < 0 || pos.y < 0)
            {
                return false; // should never happen
            }
            // get frame index
            int frame = pos.y + TopFrame;
            if (frame >= Inputs.Count)
            {
                return false;
            }

            int shift = (7 - (pos.x & 7)) | (pos.x & 8);
            return ((Inputs[frame] >> shift) & 1) == 1;
        }
        ushort SetCellInputStatus(Vector2 pos, bool state)
        {
            if (pos.x < 0 || pos.y < 0)
            {
                return 0; // should never happen
            }
            // get frame index
            int frame = pos.y + TopFrame;
            int shift = (7 - (pos.x & 7)) | (pos.x & 8);
            int prevInputs = Inputs.Count;
            while (frame >= Inputs.Count)
            {
                // Add new inputs until you reach this frame
                if (state)
                {
                    //Inputs.Add((ushort)(1 << shift));
                    Inputs.Add(0);
                }
                else
                {
                    Inputs.Add(0);
                }
            }

            if ((Inputs.Count > prevInputs))
            {
                timelineScrollbar.Maximum = Inputs.Count + 38;
            }

            if (state)
            {
                Inputs[frame] |= (ushort)(1 << shift);
            }
            else
            {
                Inputs[frame] &= (ushort)(0xFFFF ^ (1 << shift));
            }

            return Inputs[frame];
        }

        public TriCNESGUI MainGUI;

        public static List<bool> LagFrames;
        public static List<TimelineCell[]> TimelineGrid;
        public static List<List<byte>> TimelineSavestates;
        public static List<List<byte>> TimelineTempSavestates;
        public static List<int> TEMPRerecordTracker;

        public static List<ushort> Inputs; // high byte = controller 2

        public int frameIndex;
        public int highestFrameEmulatedEver; // highest emulated frame
        public int frameEmulated; // highest emulated frame for the purposes of tracking stale frames

        int AutoSavestateThreshold = 500;
        int TempSavestates = 120;

        public void Start()
        {
            TimelineGrid = new List<TimelineCell[]>();
            TimelineSavestates = new List<List<byte>>();
            TimelineTempSavestates = new List<List<byte>>();
            LagFrames = new List<bool>();
            //TEMPRerecordTracker = new List<int>();

            for (int i = 0; i < 40; i++)
            {
                TimelineCell[] t = new TimelineCell[17];
                for (int j = 0; j < t.Length; j++)
                {
                    t[0] = new TimelineCell(false);
                }

                TimelineGrid.Add(t);
            }

            Rerecords = 0;
            frameIndex = 0;

            timelineScrollbar.Maximum = Inputs.Count + 38;
            timelineScrollbar.LargeChange = 40;

            MainGUI.CreateTASTimelineEmulator();

            List<byte> state = MainGUI.EMU.SaveState();
            TimelineSavestates.Add(state);
            TimelineTempSavestates.Add(new List<byte>());
            SavestateLength = state.Count;
            //TEMPRerecordTracker.Add(Rerecords);
        }

        int ScrollbarValue = 0;
        private void timelineScrollbar_ValueChanged(object sender, EventArgs e)
        {
            TopFrame = timelineScrollbar.Value;
            if (TopFrame != ScrollbarValue)
            {
                ScrollbarValue = TopFrame;
                RefreshTimeline();
            }
        }

        private void b_FrameAdvance_Click(object sender, EventArgs e)
        {
            MainGUI.Timeline_PendingFrameAdvance = true;
        }

        private void b_FrameBack_Click(object sender, EventArgs e)
        {
            FrameRewind();
        }

        public void FrameRewind()
        {
            int row = frameIndex - TopFrame;
            int rowm1 = row - 1;
            frameIndex -= 2;
            if (frameIndex < 0)
            {
                frameIndex = 0;
            }

            UpdateTimelineRowStatus(row);
            RedrawTimelineRow(row, false);

            MainGUI.Timeline_LoadState = GrabMostRecentSavestate(); // This function updates frameIndex
            MainGUI.Timeline_PendingLoadState = true;
            MainGUI.Timeline_PendingFrameNumber = frameIndex;
            if (frameIndex == 0)
            {
                UpdateTimelineRowStatus(0);
                RedrawTimelineRow(0, false);
                MainGUI.Timeline_PendingResetScreen = true;
            }
            else
            {
                MainGUI.Timeline_PendingFrameNumber = frameIndex;
                MainGUI.Timeline_PendingFrameAdvance = true;
            }

            if (frameIndex != rowm1)
            {
                MainGUI.Timeline_AutoPlayUntilTarget = true;
                MainGUI.Timeline_AutoPlayTarget = rowm1;
            }
        }

        public void FrameAdvance()
        {
            frameIndex++;
            if (frameIndex > LagFrames.Count)
            {
                LagFrames.Add(MainGUI.EMU.LagFrame);
            }
            else
            {
                LagFrames[frameIndex - 1] = MainGUI.EMU.LagFrame;
            }

            if (frameIndex > frameEmulated)
            {
                frameEmulated = frameIndex;
            }

            if (frameIndex > highestFrameEmulatedEver)
            {
                highestFrameEmulatedEver = frameIndex;
            }

            if (frameIndex == TimelineSavestates.Count)
            {
                // create a savestate for the previous frame.
                if (cb_SavestateEveryFrame.Checked || frameIndex % AutoSavestateThreshold == 0)
                {
                    List<byte> state = MainGUI.EMU.SaveState();
                    TimelineSavestates.Add(state);
                }
                else
                {
                    List<byte> state = new List<byte>();
                    TimelineSavestates.Add(state);
                }

                if (TimelineSavestates[frameIndex].Count > 0)
                {
                    // if this savestate is not empty
                    List<byte> state = new List<byte>();
                    TimelineTempSavestates.Add(state);
                }
                else
                {
                    // if this savestate is empty
                    List<byte> state = MainGUI.EMU.SaveState();
                    TimelineTempSavestates.Add(state);
                }

                //TEMPRerecordTracker.Add(Rerecords);

            }
            else if (TimelineSavestates[frameIndex].Count != SavestateLength && cb_SavestateEveryFrame.Checked)
            {
                List<byte> state = MainGUI.EMU.SaveState();
                TimelineSavestates[frameIndex] = state;
                //TEMPRerecordTracker[frameIndex] = Rerecords;
            }

            TrimTempSavestates();

            while (frameIndex >= Inputs.Count)
            {
                Inputs.Add(0);
                MethodInvoker upd = delegate
                {
                    timelineScrollbar.Maximum = Inputs.Count + 38;
                };
                this.BeginInvoke(upd);
            }

            int row = frameIndex - TopFrame;

            bool didFullRefresh = false;

            if (cb_FollowCursor.Checked)
            {
                if (row >= TimelineGrid.Count || row > FollowDistance)
                {
                    TopFrame = frameIndex - FollowDistance;
                    if (TopFrame != ScrollbarValue && TopFrame >= 0)
                    {
                        didFullRefresh = true;
                        RefreshTimeline();
                        MethodInvoker upd = delegate
                        {
                            ScrollbarValue = TopFrame;
                            timelineScrollbar.Value = TopFrame;
                        };
                        this.Invoke(upd);
                    }
                }
            }

            if (!didFullRefresh)
            {
                UpdateTimelineRowStatus(row - 1);
                RedrawTimelineRow(row - 1, false);
                UpdateTimelineRowStatus(row);
                RedrawTimelineRow(row, false);
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
                "TriCNES TAS File (.3c2, .3c3)|*.3c2;*.3c3" +
                "|Bizhawk Movie (.bk2)|*.bk2" +
                "|Bizhawk TAStudio (.tasproj)|*.tasproj" +
                "|FCEUX Movie (.fm2)|*.fm2" +
                "|FCEUX TAS Editor (.fm3)|*.fm3" +
                "|Famtastia Movie (.fmv)|*.fmv" +
                "|Replay Device (.r08)|*.r08" +
                "|All TAS Files (.3c2, .3c3, .bk2, .tasproj, .fm2, .fm3, .fmv, .r08)|*.3c2;*.3c3;*.bk2;*.tasproj;*.fm2;*.fm3;*.fmv;*.r08",
                Title = "Select file",
                InitialDirectory = InitDirectory
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                frameIndex = 0;
                Inputs = MainGUI.ParseTasFile(ofd.FileName);
                string extension = Path.GetExtension(ofd.FileName);
                timelineScrollbar.Maximum = Inputs.Count + 38;
                MainGUI.Timeline_PendingHardReset = true;

                if (extension != ".3c3")
                {
                    LagFrames = new List<bool>();
                    TimelineSavestates = new List<List<byte>>();
                    TimelineTempSavestates = new List<List<byte>>();
                    // savestates are initialized in the Timeline_PendingArbitrarySavestate
                    MainGUI.Timeline_PendingArbitrarySavestate = true;

                    ScrollbarValue = 0;
                    timelineScrollbar.Value = 0;
                    TopFrame = 0;
                    highestFrameEmulatedEver = 0;
                    frameEmulated = 0;
                }

                RefreshTimeline();
                GC.Collect();
            }
        }

        private void saveTASToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string InitDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"tas\"))
            {
                InitDirectory += @"tas\";
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = "",
                Filter = "TriCNES TAS File (.3c2)|*.3c2",
                Title = "Save a .3c2 TAS File",
                InitialDirectory = InitDirectory
            };
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                FileStream fs = (FileStream)sfd.OpenFile();
                for (int i = 0; i < Inputs.Count; i++)
                {
                    fs.WriteByte((byte)Inputs[i]);
                    fs.WriteByte((byte)(Inputs[i] >> 8));
                }

                fs.Close();
            }
        }

        private void saveWithSavestatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string InitDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"tas\"))
            {
                InitDirectory += @"tas\";
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = "",
                Filter = "TriCNES TAS File (.3c3)|*.3c3",
                Title = "Save a .3c3 TAS File",
                InitialDirectory = InitDirectory
            };
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                FileStream fs = (FileStream)sfd.OpenFile();
                // save the length of the savestates
                fs.WriteByte((byte)SavestateLength);
                fs.WriteByte((byte)(SavestateLength >> 8));
                fs.WriteByte((byte)(SavestateLength >> 16));
                fs.WriteByte((byte)(SavestateLength >> 24));

                // save the number of rerecords. (I personally don't care about this statistic, but other people do, so I'll add it!)
                fs.WriteByte((byte)Rerecords);
                fs.WriteByte((byte)(Rerecords >> 8));
                fs.WriteByte((byte)(Rerecords >> 16));
                fs.WriteByte((byte)(Rerecords >> 24));

                // save the length of the TAS
                fs.WriteByte((byte)Inputs.Count);
                fs.WriteByte((byte)(Inputs.Count >> 8));
                fs.WriteByte((byte)(Inputs.Count >> 16));
                fs.WriteByte((byte)(Inputs.Count >> 24));

                for (int i = 0; i < Inputs.Count; i++)
                {
                    fs.WriteByte((byte)Inputs[i]);
                    fs.WriteByte((byte)(Inputs[i] >> 8));
                    fs.WriteByte((byte)(i < LagFrames.Count ? (LagFrames[i] ? 1 : 0) : 0));
                }

                for (int i = 0; i < TimelineSavestates.Count; i++)
                {
                    if (TimelineSavestates[i].Count != 0)
                    {
                        // save the frame index
                        fs.WriteByte((byte)i);
                        fs.WriteByte((byte)(i >> 8));
                        fs.WriteByte((byte)(i >> 16));
                        fs.WriteByte((byte)(i >> 24));
                        // save every byte of the savestate
                        for (int j = 0; j < SavestateLength; j++)
                        {
                            fs.WriteByte(TimelineSavestates[i][j]);
                        }
                    }
                }

                fs.Close();
            }
        }

        private void exportTor08ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string InitDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"tas\"))
            {
                InitDirectory += @"tas\";
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = "",
                Filter = "Replay Device (.r08)|*.r08",
                Title = "Save a .r08 TAS File",
                InitialDirectory = InitDirectory
            };
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                FileStream fs = (FileStream)sfd.OpenFile();

                if (ClockFiltering)
                {
                    // the .r08 file format is identical to my input list.
                    for (int i = 0; i < Inputs.Count; i++)
                    {
                        fs.WriteByte((byte)Inputs[i]);
                        fs.WriteByte((byte)(Inputs[i] >> 8));
                    }
                }
                else
                {
                    if (LagFrames.Count < Inputs.Count)
                    {
                        MessageBox.Show("The .r08 exporter needs to know which frames are lag frames.\nOnly frames that have been emulated on the timeline will be exported.");
                    }

                    for (int i = 0; i < Inputs.Count; i++)
                    {
                        // the .r08 file format doesn't include lag frames.
                        if (i < LagFrames.Count && !LagFrames[i])
                        {
                            fs.WriteByte((byte)Inputs[i]);
                            fs.WriteByte((byte)(Inputs[i] >> 8));
                        }
                    }
                }

                fs.Close();
            }
        }

        bool Paused = true;

        private void b_play_Click(object sender, EventArgs e)
        {
            Paused = !Paused;
            b_play.Text = Paused ? "Paused" : "Running";
            MainGUI.Timeline_PendingResume = !Paused;
            MainGUI.Timeline_PendingPause = Paused;
            if (Paused)
            {
                GC.Collect();
            }
        }

        private void deleteFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Inputs.RemoveAt(frameIndex);
            int HighlightedFrame = frameIndex;
            if (HighlightedFrame < frameEmulated)
            {
                MarkStale(HighlightedFrame);
            }

            timelineScrollbar.Maximum = Inputs.Count + 38;
            RefreshTimeline();
        }

        private void insertFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inputs.Insert(frameIndex, 0);
            int HighlightedFrame = frameIndex;
            if (HighlightedFrame < frameEmulated)
            {
                MarkStale(HighlightedFrame);
            }

            timelineScrollbar.Maximum = Inputs.Count + 38;
            RefreshTimeline();
        }

        private void truncateMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inputs.RemoveRange(frameIndex + 1, Inputs.Count - (frameIndex + 1));
            if (TimelineSavestates.Count > frameIndex)
            {
                TimelineSavestates.RemoveRange(frameIndex + 1, TimelineSavestates.Count - (frameIndex + 1));
            }

            if (TimelineTempSavestates.Count > frameIndex)
            {
                TimelineTempSavestates.RemoveRange(frameIndex + 1, TimelineTempSavestates.Count - (frameIndex + 1));
            }

            if (LagFrames.Count > frameIndex)
            {
                LagFrames.RemoveRange(frameIndex, LagFrames.Count - (frameIndex));
            }

            System.GC.Collect();
            highestFrameEmulatedEver = frameIndex;
            frameEmulated = frameIndex;
            timelineScrollbar.Maximum = Inputs.Count + 38;
            RefreshTimeline();
        }

        private void b_JumptoCursor_Click(object sender, EventArgs e)
        {
            int scroll = frameIndex - FollowDistance;
            if (scroll < 0)
            {
                scroll = 0;
            }

            timelineScrollbar.Value = scroll;
            TopFrame = scroll;
            RefreshTimeline();
        }

        public void RefreshTimeline()
        {
            // redraw the entire timeline's bitmap.
            for (int i = 0; i < 40; i++)
            {
                int frame = TopFrame + i;
                RecalculateTimelineRow(i, frame < Inputs.Count ? Inputs[frame] : (ushort)0);
                RedrawTimelineRow(i, true);
            }

            MethodInvoker upd = delegate
            {
                pb_Timeline.Image = timelineBitmap;
                pb_Timeline.Update();
            };
            this.Invoke(upd);
        }
        public void RedrawTimelineRow(int row, bool batch)
        {
            if (row < 0 || row >= TimelineGrid.Count)
            {
                return;
            }

            MethodInvoker upd = delegate
            {
                int rowp1 = row + 1;
                Rectangle[] GridOverlay = new Rectangle[]
                    {
                        new Rectangle(0,rowp1*16,80,16),
                        new Rectangle(80 + 16*0,rowp1*16,16,16),
                        new Rectangle(80 + 16*1,rowp1*16,16,16),
                        new Rectangle(80 + 16*2,rowp1*16,16,16),
                        new Rectangle(80 + 16*3,rowp1*16,16,16),
                        new Rectangle(80 + 16*4,rowp1*16,16,16),
                        new Rectangle(80 + 16*5,rowp1*16,16,16),
                        new Rectangle(80 + 16*6,rowp1*16,16,16),
                        new Rectangle(80 + 16*7,rowp1*16,16,16),
                        new Rectangle(80 + 16*8,rowp1*16,16,16),
                        new Rectangle(80 + 16*9,rowp1*16,16,16),
                        new Rectangle(80 + 16*10,rowp1*16,16,16),
                        new Rectangle(80 + 16*11,rowp1*16,16,16),
                        new Rectangle(80 + 16*12,rowp1*16,16,16),
                        new Rectangle(80 + 16*13,rowp1*16,16,16),
                        new Rectangle(80 + 16*14,rowp1*16,16,16),
                        new Rectangle(80 + 16*15,rowp1*16,16,16)
                    };
                if (frameIndex - TopFrame == row)
                {
                    G.FillRectangle(Brush_HighlightedCell, new Rectangle(0, rowp1 * 16, 80 + 16 * 16, 16));
                }
                else if (!TimelineGrid[row][0].Emulated)
                {
                    G.FillRectangle(Brush_LeftColumn, new Rectangle(0, rowp1 * 16, 80, 16));
                    G.FillRectangle(Brush_WhiteCellP1, new Rectangle(80, rowp1 * 16, 16 * 8, 16));
                    G.FillRectangle(Brush_WhiteCellP2, new Rectangle(80 + 16 * 8, rowp1 * 16, 16 * 8, 16));
                }
                else if (TimelineGrid[row][0].Stale)
                {
                    G.FillRectangle(Brush_LeftColumn, new Rectangle(0, rowp1 * 16, 80, 16));
                    G.FillRectangle(TimelineGrid[row][0].LagFrame ? Brush_RedCellP1_Stale : Brush_GreenCellP1_Stale, new Rectangle(80, rowp1 * 16, 16 * 8, 16));
                    G.FillRectangle(TimelineGrid[row][0].LagFrame ? Brush_RedCellP2_Stale : Brush_GreenCellP2_Stale, new Rectangle(80 + 16 * 8, rowp1 * 16, 16 * 8, 16));
                }
                else
                {
                    G.FillRectangle(TimelineSavestates[row + TopFrame].Count == SavestateLength ? Brush_LeftColumn_Saved : TimelineTempSavestates[row + TopFrame].Count == SavestateLength ? Brush_LeftColumn_TempSaved : Brush_LeftColumn, new Rectangle(0, rowp1 * 16, 80, 16));
                    G.FillRectangle(TimelineGrid[row][0].LagFrame ? Brush_RedCellP1 : Brush_GreenCellP1, new Rectangle(80, rowp1 * 16, 16 * 8, 16));
                    G.FillRectangle(TimelineGrid[row][0].LagFrame ? Brush_RedCellP2 : Brush_GreenCellP2, new Rectangle(80 + 16 * 8, rowp1 * 16, 16 * 8, 16));
                }

                G.DrawRectangles(Pens.Black, GridOverlay);
                string rownum = (row + TopFrame).ToString();
                //if(row + TopFrame < TEMPRerecordTracker.Count)
                //{
                //    rownum += "   (" + TEMPRerecordTracker[row + TopFrame].ToString() + ")";
                //}
                G.DrawString(rownum, Font_Consolas, Brushes.Black, 0, rowp1 * 16);
                if (TimelineGrid[row][1].Checked)
                {
                    G.DrawString("A", Font_Consolas, Brushes.Black, 80 + 16 * 0, rowp1 * 16);
                }

                if (TimelineGrid[row][2].Checked)
                {
                    G.DrawString("B", Font_Consolas, Brushes.Black, 80 + 16 * 1, rowp1 * 16);
                }

                if (TimelineGrid[row][3].Checked)
                {
                    G.DrawString("s", Font_Consolas, Brushes.Black, 80 + 16 * 2, rowp1 * 16);
                }

                if (TimelineGrid[row][4].Checked)
                {
                    G.DrawString("S", Font_Consolas, Brushes.Black, 80 + 16 * 3, rowp1 * 16);
                }

                if (TimelineGrid[row][5].Checked)
                {
                    G.DrawString("U", Font_Consolas, Brushes.Black, 80 + 16 * 4, rowp1 * 16);
                }

                if (TimelineGrid[row][6].Checked)
                {
                    G.DrawString("D", Font_Consolas, Brushes.Black, 80 + 16 * 5, rowp1 * 16);
                }

                if (TimelineGrid[row][7].Checked)
                {
                    G.DrawString("L", Font_Consolas, Brushes.Black, 80 + 16 * 6, rowp1 * 16);
                }

                if (TimelineGrid[row][8].Checked)
                {
                    G.DrawString("R", Font_Consolas, Brushes.Black, 80 + 16 * 7, rowp1 * 16);
                }

                if (TimelineGrid[row][9].Checked)
                {
                    G.DrawString("A", Font_Consolas, Brushes.Black, 80 + 16 * 8, rowp1 * 16);
                }

                if (TimelineGrid[row][10].Checked)
                {
                    G.DrawString("B", Font_Consolas, Brushes.Black, 80 + 16 * 9, rowp1 * 16);
                }

                if (TimelineGrid[row][11].Checked)
                {
                    G.DrawString("s", Font_Consolas, Brushes.Black, 80 + 16 * 10, rowp1 * 16);
                }

                if (TimelineGrid[row][12].Checked)
                {
                    G.DrawString("S", Font_Consolas, Brushes.Black, 80 + 16 * 11, rowp1 * 16);
                }

                if (TimelineGrid[row][13].Checked)
                {
                    G.DrawString("U", Font_Consolas, Brushes.Black, 80 + 16 * 12, rowp1 * 16);
                }

                if (TimelineGrid[row][14].Checked)
                {
                    G.DrawString("D", Font_Consolas, Brushes.Black, 80 + 16 * 13, rowp1 * 16);
                }

                if (TimelineGrid[row][15].Checked)
                {
                    G.DrawString("L", Font_Consolas, Brushes.Black, 80 + 16 * 14, rowp1 * 16);
                }

                if (TimelineGrid[row][16].Checked)
                {
                    G.DrawString("R", Font_Consolas, Brushes.Black, 80 + 16 * 15, rowp1 * 16);
                }

                if (!batch)
                {
                    pb_Timeline.Image = timelineBitmap;
                    pb_Timeline.Update();
                }
            };
            this.BeginInvoke(upd);
        }
        public void RecalculateTimelineRow(int row, ushort input)
        {
            // redraw one row of the timeline
            if (row < 0 || row >= TimelineGrid.Count)
            {
                return;
            }

            TimelineGrid[row][8].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][7].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][6].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][5].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][4].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][3].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][2].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][1].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][16].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][15].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][14].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][13].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][12].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][11].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][10].Checked = (input & 1) == 1;
            input >>= 1;
            TimelineGrid[row][9].Checked = (input & 1) == 1;

            UpdateTimelineRowStatus(row);
        }
        public void UpdateTimelineRowStatus(int row)
        {
            if (row < 0 || row >= TimelineGrid.Count)
            {
                return;
            }

            // redraw one row of the timeline
            int frame = (TopFrame + row);

            bool Emulated = frame < highestFrameEmulatedEver;
            bool Stale = Emulated && (frame > frameEmulated);
            bool LagFrame = false;
            if (LagFrames.Count > frame)
            {
                LagFrame = Emulated && (LagFrames[frame]);
            }

            TimelineGrid[row][0].Emulated = Emulated;
            TimelineGrid[row][0].Stale = Stale;
            TimelineGrid[row][0].LagFrame = LagFrame;
        }
        int FollowDistance = 20;
        private void tb_FollowDistance_Scroll(object sender, EventArgs e)
        {
            FollowDistance = tb_FollowDistance.Value;
        }

        public bool SavestateEveryFrame()
        {
            return cb_SavestateEveryFrame.Checked;
        }
        public bool RecordInputs()
        {
            return cb_RecordInputs.Checked;
        }
        List<byte> GrabMostRecentSavestate()
        {
            int l = TimelineSavestates[frameIndex].Count;
            if (l != SavestateLength)
            {
                l = TimelineTempSavestates[frameIndex].Count;
                if (l == SavestateLength)
                {
                    return TimelineTempSavestates[frameIndex];
                }
            }

            bool temp = false;
            while (l != SavestateLength)
            {
                frameIndex--;
                l = TimelineSavestates[frameIndex].Count;
                if (l != SavestateLength)
                {
                    l = TimelineTempSavestates[frameIndex].Count;
                    if (l == SavestateLength)
                    {
                        temp = true;
                        break;
                    }
                }
            }

            return temp ? TimelineTempSavestates[frameIndex] : TimelineSavestates[frameIndex];
        }

        List<byte> GrabMostRecentSavestate(int TargetFrame, out bool HitTargetFrame)
        {
            if (frameIndex == TargetFrame)
            {
                HitTargetFrame = true;
                return null;
            }

            bool temp = false;
            int l = TimelineSavestates[frameIndex].Count;
            if (l != SavestateLength)
            {
                l = TimelineTempSavestates[frameIndex].Count;
                if (l == SavestateLength)
                {
                    HitTargetFrame = false;
                    return TimelineTempSavestates[frameIndex];
                }
            }

            while (l != SavestateLength)
            {
                frameIndex--;
                if (frameIndex == TargetFrame)
                {
                    HitTargetFrame = true;
                    return null;
                }

                l = TimelineSavestates[frameIndex].Count;
                if (l != SavestateLength)
                {
                    l = TimelineTempSavestates[frameIndex].Count;
                    if (l == SavestateLength)
                    {
                        temp = true;
                        break;
                    }
                }
            }

            HitTargetFrame = false;
            return temp ? TimelineTempSavestates[frameIndex] : TimelineSavestates[frameIndex];
        }

        private void savestateThisFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frameIndex == TimelineSavestates.Count)
            {
                List<byte> state = MainGUI.EMU.SaveState();
                TimelineSavestates.Add(state);
                if (TimelineTempSavestates[frameIndex].Count > 0)
                {
                    TimelineTempSavestates[frameIndex] = new List<byte>(); // remove temp savestate for this frame
                }
            }
            else
            {
                List<byte> state = MainGUI.EMU.SaveState();
                TimelineSavestates[frameIndex] = state;
                if (TimelineTempSavestates[frameIndex].Count > 0)
                {
                    TimelineTempSavestates[frameIndex] = new List<byte>(); // remove temp savestate for this frame
                }

                GC.Collect();
            }
        }

        public void TrimTempSavestates()
        {
            int deletion = frameIndex - TempSavestates;
            if (deletion >= 0 && deletion < TimelineTempSavestates.Count && TimelineTempSavestates[deletion].Count > 0)
            {
                TimelineTempSavestates[deletion] = new List<byte>(); // remove temp savestate for this frame
                int row = deletion - TopFrame;
                if (row >= 0 && row < 40)
                {
                    UpdateTimelineRowStatus(row);
                    RedrawTimelineRow(row, false);
                }
            }
        }

        private void tb_FilterForNumbers(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tb_AutoSavestateThreshold_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (int.TryParse(tb_AutoSavestateThreshold.Text, out i))
            {
                AutoSavestateThreshold = i;
            }
        }

        private void tb_TempSavestates_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (int.TryParse(tb_TempSavestates.Text, out i))
            {
                TempSavestates = i;
            }
        }

        public void ChangePlayPauseButtonText(string str)
        {
            MethodInvoker upd = delegate
            {
                b_play.Text = str;
                b_play.Update();
            };
            this.Invoke(upd);
        }

        public bool ClockFiltering;
        private void perVBlankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            perVBlankToolStripMenuItem.Checked = true;
            perControllerStrobeToolStripMenuItem.Checked = false;
            ClockFiltering = false;
            // reset the TAS and mark everything as stale!
            ResetTASAndMarkEverythingStale();
            MainGUI.Timeline_PendingClockFiltering = false;
            RefreshTopOfTimeline();
        }

        private void perControllerStrobeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            perVBlankToolStripMenuItem.Checked = false;
            perControllerStrobeToolStripMenuItem.Checked = true;
            ClockFiltering = true;
            ResetTASAndMarkEverythingStale();
            MainGUI.Timeline_PendingClockFiltering = true;
            RefreshTopOfTimeline();
        }

        void ResetTASAndMarkEverythingStale()
        {
            frameIndex = 0;
            timelineScrollbar.Maximum = Inputs.Count + 38;
            MainGUI.Timeline_PendingHardReset = true;

            LagFrames = new List<bool>();
            TimelineSavestates = new List<List<byte>>();
            TimelineTempSavestates = new List<List<byte>>();
            // savestates are initialized in the Timeline_PendingArbitrarySavestate
            MainGUI.Timeline_PendingArbitrarySavestate = true;

            ScrollbarValue = 0;
            timelineScrollbar.Value = 0;
            TopFrame = 0;
            highestFrameEmulatedEver = 0;
            frameEmulated = 0;

            RefreshTimeline();
        }
    }
}
