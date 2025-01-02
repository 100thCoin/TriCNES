using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace TriCNES
{
    public partial class TASProperties : Form
    {
        public TASProperties()
        {
            InitializeComponent();
        }

        public string TasFilePath;
        public ushort[] TasInputLog;
        public TriCNESGUI MainGUI;

        public bool SubframeInputs()
        {
            return rb_ClockFiltering.Checked;
        }

        public bool UseFCEUXFrame0Timing() // this only applies to TASes using the .fm2 or .fm3 file format.
        {
            return cb_fceuxFrame0.Checked;
        }

        public byte GetPPUClockPhase()
        {
            return (byte)cb_ClockAlignment.SelectedIndex;
        }

        public byte GetCPUClockPhase()
        {
            return (byte)cb_CpuClock.SelectedIndex;
        }

        public string extension;

        public void Init()
        {
            tb_FilePath.Text = TasFilePath;
            // determine file type
            extension = Path.GetExtension(TasFilePath);
            // create list of inputs from the tas file, and make any settings changes if needed.
            byte[] ByteArray = File.ReadAllBytes(TasFilePath);
            List<ushort> TASInputs = new List<ushort>(); // Low byte is player 1, High byte is player 2.

            rb_ClockFiltering.Checked = false;
            rb_LatchFiltering.Checked = true;
            l_FamtasiaWarning.Visible = false;
            cb_ClockAlignment.SelectedIndex = 0;
            cb_ClockAlignment.Update();
            cb_CpuClock.SelectedIndex = 0;
            cb_CpuClock.Update();
            cb_fceuxFrame0.Enabled = false;
            switch (extension)
            {
                case ".bk2":
                case ".tasproj":
                    {
                        cb_CpuClock.SelectedIndex = 8;
                        cb_CpuClock.Update();
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
                        while(ln != null && ln.Length > 3)
                        {
                            int pipeIndex = ln.Substring(1,ln.Length-1).IndexOf('|')+1;
                            char[] lnCharArray = ln.ToCharArray();
                            bool reset = lnCharArray[pipeIndex - 1] == 'r';
                            u = 0;
                            if(Bk2_Port1)
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
                            else if(Bk2_Port2) // Are there any NES TASes that only feature controller 2?
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
                            if(Bk2_Port1 && Bk2_Port2)
                            {
                                pipeIndex = ln.Substring(pipeIndex+1, ln.Length - 1 - pipeIndex).IndexOf('|')+pipeIndex+1;
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
                            ln = InputLog.ReadLine();
                            if(ln == "[/Input]")
                            {
                                break;
                            }
                        }
                    }
                    break;
                case ".fm2":
                    {
                        cb_fceuxFrame0.Enabled = true;
                        // change the alignment to use FCEUX's
                        cb_CpuClock.SelectedIndex = 0;
                        cb_CpuClock.Update();
                        // header info of varying size
                        // Every line of a header ends in $0A
                        // Every header section is named. Example: $0A "romFileName"
                        // Since the input log begins with "|" and none of the header section names begin with "|", I can assume $0A"|" is the start of the input log
                        bool fm2_UsePort0 = false;
                        bool fm2_UsePort1 = false;

                        int i = 0;
                        while(i < ByteArray.Length)
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

                        int pipecount = 0;
                        int bitcount = 0;
                        ushort u = 0;

                        int Port0Index = 0;
                        int Port1Index = 0;

                        while (i < ByteArray.Length)
                        {
                            if (ByteArray[i] == 0x0A)
                            {
                                if(i == ByteArray.Length-1)
                                {
                                    break;
                                }
                                if(fm2_UsePort0)
                                {
                                    Port0Index = i + 4;
                                    if(fm2_UsePort1)
                                    {
                                        Port1Index = i + 0xD;
                                    }
                                }
                                else if (fm2_UsePort1)
                                {
                                    Port1Index = i + 0x6;
                                }

                                u = 0;
                                if(fm2_UsePort0)
                                {
                                    u |= (ushort)(ByteArray[Port0Index] == 0x2E ? 0 : 1);
                                    u |= (ushort)(ByteArray[Port0Index+1] == 0x2E ? 0 : 2);
                                    u |= (ushort)(ByteArray[Port0Index+2] == 0x2E ? 0 : 4);
                                    u |= (ushort)(ByteArray[Port0Index+3] == 0x2E ? 0 : 8);
                                    u |= (ushort)(ByteArray[Port0Index+4] == 0x2E ? 0 : 0x10);
                                    u |= (ushort)(ByteArray[Port0Index+5] == 0x2E ? 0 : 0x20);
                                    u |= (ushort)(ByteArray[Port0Index+6] == 0x2E ? 0 : 0x40);
                                    u |= (ushort)(ByteArray[Port0Index+7] == 0x2E ? 0 : 0x80);
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
                        int fm3_length = 0;
                        bool fm3_UsePort0 = false;
                        bool fm3_UsePort1 = false;
                        int i = 0;
                        while(i < ByteArray.Length)
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
                                    if(ByteArray[i+1] == 0x6C &&
                                        ByteArray[i+2] == 0x65 &&
                                        ByteArray[i+3] == 0x6E &&
                                        ByteArray[i+4] == 0x67 &&
                                        ByteArray[i+5] == 0x74 &&
                                        ByteArray[i+6] == 0x68 &&
                                        ByteArray[i+7] == 0x20)
                                    {
                                        // okay, so the length is in ascii...
                                        // let's figure out where the next $0A character is
                                        int next0A = i + 8;
                                        while(next0A < ByteArray.Length)
                                        {
                                            if (ByteArray[next0A] == 0x0A)
                                            {
                                                break;
                                            }
                                            next0A++;
                                        }
                                        // okay, so the string from i+8 though next0A is the length.
                                        byte[] StringArray = new byte[next0A - (i + 8)];
                                        Array.Copy(ByteArray, i + 8,StringArray,0, StringArray.Length);
                                        int InputLogLength = int.Parse(Encoding.Default.GetString(StringArray));
                                        i = next0A + 2;
                                        int tempMul = 1;
                                        if (fm3_UsePort0) { tempMul++; }
                                        if (fm3_UsePort1) { tempMul++; }
                                        int InputLogByteLength = InputLogLength*tempMul;
                                        // first byte is always zero?
                                        // next byte is controller 1 (if enabled)
                                        // next byte is controller 2 (if enabled)
                                        ushort u = 0;
                                        while(i < next0A+2+ InputLogByteLength)
                                        {
                                            i++;// dummy byte (?)
                                            u = 0;
                                            if (fm3_UsePort0) { u = ByteArray[i]; i++; }
                                            if (fm3_UsePort1) { u |= (ushort)(ByteArray[i]<<8); i++; }
                                            TASInputs.Add(u);
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
                        l_FamtasiaWarning.Visible = true;

                        int i = 0x90; // there's a 144 byte header
                        bool fmv_UseController2 = (ByteArray[5] & 0b00010000) != 0;
                        if (fmv_UseController2)
                        {
                            while (i < ByteArray.Length)
                            {
                                ushort u = (ushort)(FamtasiaInput2Standard(ByteArray[i]) | (FamtasiaInput2Standard(ByteArray[i + 1]) << 8));
                                TASInputs.Add(u);
                                i+=2;
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
                            TASInputs.Add((ushort)(b | (b2<<8)));
                            i += 2;
                        }
                        TASInputs.Add(0); // append a zero to the end for safe measure.
                    }
                    break;

                    // TODO: ask if the .tasd file format is a thing yet
            }

            // okay cool, now we have the entire input log.
            TasInputLog = TASInputs.ToArray();
            l_InputCount.Text = TasInputLog.Length + " Inputs";
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

        private void b_RunTAS_Click(object sender, EventArgs e)
        {
            MainGUI.StartTAS();


        }
    }
}
