using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TriCNES.forms
{
    public partial class TriC72PinConnector : Form
    {
        public TriCNESGUI MainGUI;
        CheckBox[] cb_PinsFloating;
        public void TriC72PinConnector_Closing(object sender, FormClosingEventArgs e)
        {
            if (MainGUI != null)
            {
                MainGUI.CartConnector = null;
            }
            Dispose();
        }
        public TriC72PinConnector()
        {
            InitializeComponent();
            cb_PinsFloating = new CheckBox[72];

            for(int i = 0; i < 72; i++)
            {
                cb_PinsFloating[i] = new CheckBox();
                cb_PinsFloating[i].Location = new Point(32 + ((i >= 36) ? 128 : 0), (36 - (i % 36)) * 18);
                cb_PinsFloating[i].Text = PinNames[i];
                cb_PinsFloating[i].RightToLeft = (i >= 36) ? RightToLeft.Yes : RightToLeft.No;
                Controls.Add(cb_PinsFloating[i]);
            }

            FormClosing += TriC72PinConnector_Closing;
        }

        String[] PinNames = new String[] {
            "GND",
            "CPU A11",
            "CPU A10",
            "CPU A9",
            "CPU A8",
            "CPU A7",
            "CPU A6",
            "CPU A5",
            "CPU A4",
            "CPU A3",
            "CPU A2",
            "CPU A1",
            "CPU A0",
            "CPU R/W",
            "/IRQ",
            "EXP 0",
            "EXP 1",
            "EXP 2",
            "EXP 3",
            "EXP 4",
            "PPU /RD",
            "CIRAM A10",
            "PPU A6",
            "PPU A5",
            "PPU A4",
            "PPU A3",
            "PPU A2",
            "PPU A1",
            "PPU A0",
            "PPU D0",
            "PPU D1",
            "PPU D2",
            "PPU D3",
            "CIC toPak",
            "CIC toMB",
            "+5V",
            "SYSTEM CLK",
            "M2",
            "CPU A12",
            "CPU A13",
            "CPU A14",
            "CPU D7",
            "CPU D6",
            "CPU D5",
            "CPU D4",
            "CPU D3",
            "CPU D2",
            "CPU D1",
            "CPU D0",
            "CPU /A15",
            "EXP 9",
            "EXP 8",
            "EXP 7",
            "EXP 6",
            "EXP 5",
            "PPU /WR",
            "CIRAM /CE",
            "PPU /A13",
            "PPU A7",
            "PPU A8",
            "PPU A9",
            "PPU A11",
            "PPU A10",
            "PPU A12",
            "PPU A13",
            "PPU D7",
            "PPU D6",
            "PPU D5",
            "PPU D4",
            "CIC +RST",
            "CIC CLK",
            "GND",
        };

        public void Update72PinConnector()
        {
            if(MainGUI.EMU != null)
            {
                if (MainGUI.EMU.ConnectorPinFloating != null)
                {
                    for(int i = 0; i < 72; i++)
                    {
                        MainGUI.EMU.ConnectorPinFloating[i] = cb_PinsFloating[i].Checked;
                    }
                }
            }
        }

    }
}
