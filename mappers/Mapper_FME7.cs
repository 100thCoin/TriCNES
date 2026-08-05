using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_FME7 : Mapper
    {
        // ines Mapper 69
        public byte Mapper_69_CMD;
        public byte Mapper_69_CHR_1K0;
        public byte Mapper_69_CHR_1K1;
        public byte Mapper_69_CHR_1K2;
        public byte Mapper_69_CHR_1K3;
        public byte Mapper_69_CHR_1K4;
        public byte Mapper_69_CHR_1K5;
        public byte Mapper_69_CHR_1K6;
        public byte Mapper_69_CHR_1K7;
        public byte Mapper_69_Bank_6;
        public bool Mapper_69_Bank_6_isRAM;
        public bool Mapper_69_Bank_6_isRAMEnabled;
        public byte Mapper_69_Bank_8;
        public byte Mapper_69_Bank_A;
        public byte Mapper_69_Bank_C;
        public byte Mapper_69_NametableMirroring; // 0 = Vertical              1 = Horizontal            2 = One Screen Mirroring from $2000 ("1ScA")            3 = One Screen Mirroring from $2400 ("1ScB")
        public bool Mapper_69_EnableIRQ;
        public bool Mapper_69_EnableIRQCounterDecrement;
        public ushort Mapper_69_IRQCounter; // When enabled the 16-bit IRQ counter is decremented once per CPU cycle. When the IRQ counter is decremented from $0000 to $FFFF an IRQ is generated.
        public override void FetchCPU()
        {
            if ((Cart.Emu.ConnectorPinFloating[0] && Cart.Emu.ConnectorPinFloating[71]) || Cart.Emu.ConnectorPinFloating[35]) { return; } // If the cartridge is disconnected from power or ground, it cannot do anything.
            Connector_ReadCPUAddressPins();

            if (CPU_AddressIn >= 0x6000)
            {
                ushort tempo = (ushort)(CPU_AddressIn % 0x2000);
                if (CPU_AddressIn >= 0x6000)
                {
                    //actions
                    if (CPU_AddressIn < 0x8000)
                    {
                        if (Mapper_69_Bank_6_isRAM)
                        {
                            if (Mapper_69_Bank_6_isRAMEnabled)
                            {
                                CPU_DataOut = Cart.PRGRAM[CPU_AddressIn & 0x1FFF];
    
                            }
                        }
                        else
                        {   //read from ROM
                            CPU_DataOut = Cart.PRGROM[(Mapper_69_Bank_6 * 0x2000 + tempo) % Cart.PRGROM.Length];

                        }
                    }
                    else if (CPU_AddressIn < 0xA000)
                    {
                        CPU_DataOut = Cart.PRGROM[(Mapper_69_Bank_8 * 0x2000 + tempo) % Cart.PRGROM.Length];
                        Connector_SetUpCPUDataPins(CPU_DataOut);
                    }
                    else if (CPU_AddressIn < 0xC000)
                    {
                        CPU_DataOut = Cart.PRGROM[(Mapper_69_Bank_A * 0x2000 + tempo) % Cart.PRGROM.Length];
                        Connector_SetUpCPUDataPins(CPU_DataOut);
                    }
                    else if (CPU_AddressIn < 0xE000)
                    {
                        CPU_DataOut = Cart.PRGROM[(Mapper_69_Bank_C * 0x2000 + tempo) % Cart.PRGROM.Length];
                        Connector_SetUpCPUDataPins(CPU_DataOut);
                    }
                    else
                    {
                        CPU_DataOut = Cart.PRGROM[Cart.PRGROM.Length - 0x2000 + tempo];
                        Connector_SetUpCPUDataPins(CPU_DataOut);
                    }
                }
            }

            return;
        }
        public override void StoreCPU(ushort Address, byte Input)
        {
            if (Address >= 0x6000)
            {
                //actions
                if (Address < 0x8000)
                {
                    if (Mapper_69_Bank_6_isRAM)
                    {
                        if (Mapper_69_Bank_6_isRAMEnabled)
                        {
                            //writing to RAM
                            Cart.PRGRAM[Address & 0x1FFF] = Input;
                        } //else, writing to open bus
                    } //else it's ROM. writing here does nothing.
                }
                else if (Address < 0xA000)
                {
                    Mapper_69_CMD = (byte)(Input & 0x0F);
                }
                else if (Address < 0xC000)
                {
                    switch (Mapper_69_CMD)
                    {
                        case 0: Mapper_69_CHR_1K0 = Input; break;
                        case 1: Mapper_69_CHR_1K1 = Input; break;
                        case 2: Mapper_69_CHR_1K2 = Input; break;
                        case 3: Mapper_69_CHR_1K3 = Input; break;
                        case 4: Mapper_69_CHR_1K4 = Input; break;
                        case 5: Mapper_69_CHR_1K5 = Input; break;
                        case 6: Mapper_69_CHR_1K6 = Input; break;
                        case 7: Mapper_69_CHR_1K7 = Input; break;
                        case 8: Mapper_69_Bank_6 = (byte)(Input & 0x3F); Mapper_69_Bank_6_isRAM = (Input & 0x40) != 0; Mapper_69_Bank_6_isRAMEnabled = (Input & 0x80) != 0; break;
                        case 9: Mapper_69_Bank_8 = (byte)(Input & 0x3F); break;
                        case 10: Mapper_69_Bank_A = (byte)(Input & 0x3F); break;
                        case 11: Mapper_69_Bank_C = (byte)(Input & 0x3F); break;
                        case 12: Mapper_69_NametableMirroring = (byte)(Input & 0x3); break;
                        case 13: Mapper_69_EnableIRQ = (Input & 0x1) != 0; Mapper_69_EnableIRQCounterDecrement = (Input & 0x80) != 0; Connector_IRQPin(false); break;
                        case 14: Mapper_69_IRQCounter = (ushort)((Mapper_69_IRQCounter & 0xFF00) | Input); break;
                        case 15: Mapper_69_IRQCounter = (ushort)((Mapper_69_IRQCounter & 0xFF) | (Input << 8)); break;
                    }
                } // else do nothing
            }
        }
        public override byte SnoopCPU(ushort Address) // For debug purposes. It's a bit clunky.
        {
            if (Address >= 0x6000)
            {
                ushort tempo = (ushort)(Address % 0x2000);
                if (Address >= 0x6000)
                {
                    //actions
                    if (Address < 0x8000)
                    {
                        if (Mapper_69_Bank_6_isRAM)
                        {
                            if (Mapper_69_Bank_6_isRAMEnabled)
                            {
                                return Cart.PRGRAM[Address & 0x1FFF];    
                            }
                        }
                        else
                        {   //read from ROM
                            return Cart.PRGROM[(Mapper_69_Bank_6 * 0x2000 + tempo) % Cart.PRGROM.Length];
                        }
                    }
                    else if (Address < 0xA000)
                    {
                        return Cart.PRGROM[(Mapper_69_Bank_8 * 0x2000 + tempo) % Cart.PRGROM.Length];
                    }
                    else if (Address < 0xC000)
                    {
                        return Cart.PRGROM[(Mapper_69_Bank_A * 0x2000 + tempo) % Cart.PRGROM.Length];
                    }
                    else if (Address < 0xE000)
                    {
                        return Cart.PRGROM[(Mapper_69_Bank_C * 0x2000 + tempo) % Cart.PRGROM.Length];
                    }
                    else
                    {
                        return Cart.PRGROM[Cart.PRGROM.Length - 0x2000 + tempo];
                    }
                }
            }
            return Cart.Emu.dataBus;
        }
        public override int FetchPatternAddress(ushort Address)
        {
            if (Address < 0x400) { return (Mapper_69_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
            else if (Address < 0x800) { Address &= 0x3FF; return (Mapper_69_CHR_1K1 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
            else if (Address < 0xC00) { Address &= 0x3FF; return (Mapper_69_CHR_1K2 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
            else if (Address < 0x1000) { Address &= 0x3FF; return (Mapper_69_CHR_1K3 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
            else if (Address < 0x1400) { Address &= 0x3FF; return (Mapper_69_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
            else if (Address < 0x1800) { Address &= 0x3FF; return (Mapper_69_CHR_1K5 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
            else if (Address < 0x1C00) { Address &= 0x3FF; return (Mapper_69_CHR_1K6 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
            else { Address &= 0x3FF; return (Mapper_69_CHR_1K7 * 0x400 + Address) & (Cart.CHRROM.Length - 1); }
        }
        public override void Connector_CheckCIRAM()
        {
            if (!Cart.Emu.ConnectorPinFloating[56]) { Cart.Emu.SeventyTwoPinConnector[56] = Cart.Emu.SeventyTwoPinConnector[57]; }
            switch (Mapper_69_NametableMirroring)
            {
                case 0: //vertical
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = Cart.Emu.SeventyTwoPinConnector[62]; }
                    break;
                case 1: //horizontal
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = Cart.Emu.SeventyTwoPinConnector[61]; }
                    break;
                case 2: //one-screen A
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = false; }
                    break;
                case 3: //one-screen B
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = true; }
                    break;
            }
        }
        public override byte SnoopPPU(ushort Address) // For debug purposes. It's a bit clunky having to set this up for every mapper with a non-NROM CIRAM setup.
        {
            if (Address < 0x2000)
            {
                int CHR_Address = Cart.MapperChip.FetchPatternAddress(Address);
                return Cart.CHRROM[CHR_Address];
            }
            else
            {
                ushort Addr = (ushort)(Address & 0x3FF);
                switch (Mapper_69_NametableMirroring)
                {
                    case 0: //vertical
                        Addr |= (ushort)(((Address & 0x400) != 0) ? 0x400 : 0);
                        break;
                    case 1: //horizontal
                        Addr |= (ushort)(((Address & 0x800) != 0) ? 0x400 : 0);
                        break;
                    case 2: //one-screen A
                        break;
                    case 3: //one-screen B
                        Addr |= 0x400;
                        break;
                }
                return Cart.Emu.VRAM[Addr];
            }
        }

        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            if (Cart.UsingCHRRAM)
            {
                foreach (Byte b in Cart.CHRROM) { State.Add(b); }
            }
            State.Add(Mapper_69_CMD);
            State.Add(Mapper_69_CHR_1K0);
            State.Add(Mapper_69_CHR_1K1);
            State.Add(Mapper_69_CHR_1K2);
            State.Add(Mapper_69_CHR_1K3);
            State.Add(Mapper_69_CHR_1K4);
            State.Add(Mapper_69_CHR_1K5);
            State.Add(Mapper_69_CHR_1K6);
            State.Add(Mapper_69_CHR_1K7);
            State.Add(Mapper_69_Bank_6);
            State.Add((byte)(Mapper_69_Bank_6_isRAM ? 1 : 0));
            State.Add((byte)(Mapper_69_Bank_6_isRAMEnabled ? 1 : 0));
            State.Add(Mapper_69_Bank_8);
            State.Add(Mapper_69_Bank_A);
            State.Add(Mapper_69_Bank_C);
            State.Add(Mapper_69_NametableMirroring);
            State.Add((byte)(Mapper_69_EnableIRQ ? 1 : 0));
            State.Add((byte)(Mapper_69_EnableIRQCounterDecrement ? 1 : 0));
            State.Add((byte)Mapper_69_IRQCounter);
            State.Add((byte)(Mapper_69_IRQCounter >> 8));
            return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            if (Cart.UsingCHRRAM)
            {
                for (int i = 0; i < Cart.CHRROM.Length; i++) { Cart.CHRROM[i] = State[p++]; }
            }
            Mapper_69_CMD = State[p++];
            Mapper_69_CHR_1K0 = State[p++];
            Mapper_69_CHR_1K1 = State[p++];
            Mapper_69_CHR_1K2 = State[p++];
            Mapper_69_CHR_1K3 = State[p++];
            Mapper_69_CHR_1K4 = State[p++];
            Mapper_69_CHR_1K5 = State[p++];
            Mapper_69_CHR_1K6 = State[p++];
            Mapper_69_CHR_1K7 = State[p++];
            Mapper_69_Bank_6 = State[p++];
            Mapper_69_Bank_6_isRAM = (State[p++] & 1) == 1;
            Mapper_69_Bank_6_isRAMEnabled = (State[p++] & 1) == 1;
            Mapper_69_Bank_8 = State[p++];
            Mapper_69_Bank_A = State[p++];
            Mapper_69_Bank_C = State[p++];
            Mapper_69_NametableMirroring = State[p++];
            Mapper_69_EnableIRQ = (State[p++] & 1) == 1;
            Mapper_69_EnableIRQCounterDecrement = (State[p++] & 1) == 1;
            Mapper_69_IRQCounter = State[p++];
            Mapper_69_IRQCounter |= (ushort)(State[p++] << 8); 
            exitIndex = p;
        }
        public override void CPUClock()
        {
            // The sunsoft FME-7 mapper chip has an IRQ counter that ticks down once per CPU cycle.
            if (Mapper_69_EnableIRQCounterDecrement)
            {
                ushort temp = Mapper_69_IRQCounter;
                Mapper_69_IRQCounter--;
                if (Mapper_69_EnableIRQ && temp < Mapper_69_IRQCounter)
                {
                    Connector_IRQPin(true); // Run an IRQ!
                }
            }
        }

    }
}