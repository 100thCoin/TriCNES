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

namespace TriCNES;

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
            }

            break;
            case ".fm2":
            {
                cb_fceuxFrame0.Enabled = true;
                // change the alignment to use FCEUX's
                cb_CpuClock.SelectedIndex = 0;
                cb_CpuClock.Update();
            }

            break;
            case ".fm3":
            {

            }

            break;
            case ".fmv":
            {
                l_FamtasiaWarning.Visible = true;
            }

            break;
            case ".r08":
            {

            }

            break;
            case ".3c2":
            {

            }

            break;
            case ".3c3":
            {

            }

            break;

            // TODO: ask if the .tasd file format is a thing yet
        }

        TASInputs = MainGUI.ParseTasFile(TasFilePath);
        // okay cool, now we have the entire input log.
        TasInputLog = TASInputs.ToArray();
        l_InputCount.Text = TasInputLog.Length + " Inputs";
    }

    private void b_RunTAS_Click(object sender, EventArgs e)
    {
        MainGUI.StartTAS();

    }
}
