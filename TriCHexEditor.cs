using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TriCNES;

public partial class TriCHexEditor : Form
{
    public TriCHexEditor()
    {
        InitializeComponent();
        hexBitmap = new Bitmap(312, 512);
        G = Graphics.FromImage(hexBitmap);
        Font_Consolas = new Font("Consolas", 8);
        Scope = "RAM";
        Resize += TriCHexEditor_Resize;
        vScrollBar1.ValueChanged += Scrollbar_ValueChanged;
    }

    public TriCNESGUI MainGUI;
    public Graphics G;
    public Bitmap hexBitmap;
    public Font Font_Consolas;
    public int Scroll;
    public string Scope;
    ScopeType scopeType = ScopeType.RAM;

    public int MaxRows = 32;

    private void TriCHexEditor_Resize(object sender, EventArgs e)
    {
        MaxRows = (Size.Height - 115) / 15;
        hexBitmap = new Bitmap(312, Size.Height - 88);
        pb_hexView.Size = new Size(312, Size.Height - 88);
        vScrollBar1.Size = new Size(vScrollBar1.Width, Size.Height - 108);
        vScrollBar1.LargeChange = MaxRows;
        G = Graphics.FromImage(hexBitmap);
        RefreshEntireHexView();
    }

    private void Scrollbar_ValueChanged(object sender, EventArgs e)
    {
        Scroll = vScrollBar1.Value;
        RefreshEntireHexView();
    }

    enum ScopeType
    {
        RAM,
        CPU_Address_Space,
        VRAM,
        PPU_Address_Space,
        OAM,
        Palette_RAM
    };

    public void Update()
    {
        MethodInvoker upd = delegate
        {
            RefreshEntireHexView();
        };
        try
        {
            this.Invoke(upd);
        }
        catch (Exception e)
        {

        }
    }

    public void RefreshEntireHexView()
    {
        if (MainGUI.EMU == null)
        {
            return;
        }

        vScrollBar1.Enabled = MaxRows < vScrollBar1.Maximum;
        vScrollBar1.Update();

        G.FillRectangle(Brushes.WhiteSmoke, new Rectangle(0, 0, 312, Size.Height - 88));
        G.DrawString(Scope + ":", Font_Consolas, Brushes.Black, new Point(0, 0));
        for (int x = 0; x < 0x10; x++)
        {
            G.DrawString(" " + x.ToString("X"), Font_Consolas, Brushes.Black, new Point(42 + x * 15, 16));
        }

        switch (scopeType)
        {
            case ScopeType.RAM:
            {
                int i = Scroll * 0x10;
                int y = 0;
                while (i < 0x800 && y < MaxRows)
                {
                    // print $xy0:
                    G.DrawString("$" + (i).ToString("X3") + ":", Font_Consolas, Brushes.Black, new Point(0, 32 + y * 15));
                    for (int x = 0; x < 0x10; x++)
                    {
                        G.DrawString(MainGUI.EMU.RAM[i].ToString("X2"), Font_Consolas, Brushes.Black, new Point(42 + x * 15, 32 + y * 15));
                        i++;
                    }

                    y++;
                }
            }

            break;
            case ScopeType.CPU_Address_Space:
            {
                int i = Scroll * 0x10;
                int y = 0;
                while (i < 0x10000 && y < MaxRows)
                {
                    // print $xyz0:
                    G.DrawString("$" + (i).ToString("X4") + ":", Font_Consolas, Brushes.Black, new Point(0, 32 + y * 15));

                    for (int x = 0; x < 0x10; x++)
                    {
                        G.DrawString(MainGUI.EMU.Observe((ushort)i).ToString("X2"), Font_Consolas, Brushes.Black, new Point(42 + x * 15, 32 + y * 15));
                        i++;
                    }

                    y++;
                }
            }

            break;
            case ScopeType.VRAM:
            {
                int i = Scroll * 0x10;
                int y = 0;
                while (i < 0x800 && y < MaxRows)
                {
                    // print $xy0:
                    G.DrawString("$" + (i).ToString("X3") + ":", Font_Consolas, Brushes.Black, new Point(0, 32 + y * 15));
                    for (int x = 0; x < 0x10; x++)
                    {
                        G.DrawString(MainGUI.EMU.VRAM[i].ToString("X2"), Font_Consolas, Brushes.Black, new Point(42 + x * 15, 32 + y * 15));
                        i++;
                    }

                    y++;
                }
            }

            break;
            case ScopeType.PPU_Address_Space:
            {
                int i = Scroll * 0x10;
                int y = 0;
                while (i < 0x4000 && y < MaxRows)
                {
                    // print $xyz0:
                    G.DrawString("$" + (i).ToString("X4") + ":", Font_Consolas, Brushes.Black, new Point(0, 32 + y * 15));

                    for (int x = 0; x < 0x10; x++)
                    {
                        G.DrawString(MainGUI.EMU.ObservePPU((ushort)i).ToString("X2"), Font_Consolas, Brushes.Black, new Point(42 + x * 15, 32 + y * 15));
                        i++;
                    }

                    y++;
                }
            }

            break;
            case ScopeType.OAM:
            {
                int i = Scroll * 0x10;
                int y = 0;
                while (i < 0x100 && y < MaxRows)
                {
                    // print $xy0:
                    G.DrawString("$" + (i).ToString("X3") + ":", Font_Consolas, Brushes.Black, new Point(0, 32 + y * 15));
                    for (int x = 0; x < 0x10; x++)
                    {
                        G.DrawString(MainGUI.EMU.OAM[i].ToString("X2"), Font_Consolas, Brushes.Black, new Point(42 + x * 15, 32 + y * 15));
                        i++;
                    }

                    y++;
                }
            }

            break;
            case ScopeType.Palette_RAM:
            {
                int i = Scroll * 0x10;
                int y = 0;
                while (i < 0x20 && y < MaxRows)
                {
                    // print $xy0:
                    G.DrawString("$" + (i).ToString("X3") + ":", Font_Consolas, Brushes.Black, new Point(0, 32 + y * 15));
                    for (int x = 0; x < 0x10; x++)
                    {
                        G.DrawString(MainGUI.EMU.PaletteRAM[i].ToString("X2"), Font_Consolas, Brushes.Black, new Point(42 + x * 15, 32 + y * 15));
                        i++;
                    }

                    y++;
                }
            }

            break;
        }

        pb_hexView.Image = hexBitmap;
        pb_hexView.Update();
    }

    void ChangeScope(ScopeType st)
    {
        Scroll = 0;
        vScrollBar1.Value = 0;
        vScrollBar1.Update();
        rAMToolStripMenuItem.Checked = st == ScopeType.RAM;
        cPUAddressSpaceToolStripMenuItem.Checked = st == ScopeType.CPU_Address_Space;
        vRAMToolStripMenuItem.Checked = st == ScopeType.VRAM;
        pPUAddressSpaceToolStripMenuItem.Checked = st == ScopeType.PPU_Address_Space;
        oAMToolStripMenuItem.Checked = st == ScopeType.OAM;
        paletteRAMToolStripMenuItem.Checked = st == ScopeType.Palette_RAM;
        scopeType = st;
        switch (st)
        {
            case ScopeType.RAM:
                vScrollBar1.Maximum = 0x80; Scope = "RAM"; break;
            case ScopeType.CPU_Address_Space:
                vScrollBar1.Maximum = 0x1000; Scope = "CPU Address Space"; break;
            case ScopeType.VRAM:
                vScrollBar1.Maximum = 0x80; Scope = "VRAM"; break;
            case ScopeType.PPU_Address_Space:
                vScrollBar1.Maximum = 0x400; Scope = "PPU Address Space"; break;
            case ScopeType.OAM:
                vScrollBar1.Maximum = 0x10; Scope = "OAM"; break;
            case ScopeType.Palette_RAM:
                vScrollBar1.Maximum = 0x2; Scope = "Palette RAM"; break;
        }

        RefreshEntireHexView();
    }

    private void rAMToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ChangeScope(ScopeType.RAM);
    }

    private void cPUAddressSpaceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ChangeScope(ScopeType.CPU_Address_Space);
    }

    private void vRAMToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ChangeScope(ScopeType.VRAM);
    }

    private void pPUAddressSpaceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ChangeScope(ScopeType.PPU_Address_Space);
    }

    private void oAMToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ChangeScope(ScopeType.OAM);
    }

    private void paletteRAMToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ChangeScope(ScopeType.Palette_RAM);
    }
}
