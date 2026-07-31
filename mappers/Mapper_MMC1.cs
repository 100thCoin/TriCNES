using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_MMC1 : Mapper
    {
        // ines Mapper 1
        public byte Mapper_1_ShiftRegister;
        public byte Mapper_1_Control = 0x0C;    //0x8000
        public byte Mapper_1_CHR0;              //0xA000
        public byte Mapper_1_CHR1;              //0xC000
        public byte Mapper_1_PRG;               //0xE000
        public bool Mapper_1_PB;
        public override void FetchCPU()
        {
            if ((Cart.Emu.ConnectorPinFloating[0] && Cart.Emu.ConnectorPinFloating[75]) || Cart.Emu.ConnectorPinFloating[35]) { return; } // If the cartridge is disconnected from power or ground, it cannot do anything.
            Connector_ReadCPUAddressPins();

            if (CPU_AddressIn >= 0x8000)
            {
                // The bank mode for MMC1:
                byte MMC1PRGROMBankMode = (byte)((Mapper_1_Control & 0b01100) >> 2);
                switch (MMC1PRGROMBankMode)
                {
                    case 0:
                    case 1:
                        {
                            // switch 32 KB at $8000, ignoring low bit of bank number
                            ushort tempo = (ushort)(CPU_AddressIn & 0x7FFF);
                            CPU_DataOut = Cart.PRGROM[(0x8000 * (Mapper_1_PRG & 0x0E) + tempo) % Cart.PRGROM.Length];
                        }
                        break;
                    case 2:
                        // fix first bank at $8000 and switch 16 KB bank at $C000
                        if (CPU_AddressIn >= 0xC000)
                        {
                            ushort tempo = (ushort)(CPU_AddressIn & 0x3FFF);
                            CPU_DataOut = Cart.PRGROM[0x4000 * (Mapper_1_PRG) + tempo];
                        }
                        else
                        {
                            ushort tempo = (ushort)(CPU_AddressIn & 0x3FFF);
                            CPU_DataOut = Cart.PRGROM[tempo];
                        }
                        break;
                    case 3:
                        // fix last bank at $C000 and switch 16 KB bank at $8000
                        if (CPU_AddressIn >= 0xC000)
                        {
                            ushort tempo = (ushort)(CPU_AddressIn & 0x3FFF);
                            CPU_DataOut = Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + tempo];
                        }
                        else
                        {
                            ushort tempo = (ushort)(CPU_AddressIn & 0x3FFF);
                            CPU_DataOut = Cart.PRGROM[(0x4000 * (Mapper_1_PRG & 0x0F) + tempo) & (Cart.PRGROM.Length - 1)];
                        }
                        break;
                }
                Connector_SetUpCPUDataPins(CPU_DataOut);
            }
            else // if the address is < $8000
            {
                if (((Mapper_1_PRG & 0x10) == 0) && CPU_AddressIn >= 0x6000) // if Work RAM is enabled
                {
                    CPU_DataOut = Cart.PRGRAM[CPU_AddressIn & 0x1FFF];
                    Connector_SetUpCPUDataPins(CPU_DataOut);
                }
                // else, open bus.
            }

            return;
        }
        public override void StoreCPU(ushort Address, byte Input)
        {
            if (Address < 0x8000) //WRAM not available on MMC1A
            {
                if (((Mapper_1_PRG & 0x10) == 0) /*&& Mapper != 1*/)
                {
                    //Battery backed RAM
                    Cart.PRGRAM[Address & 0x1FFF] = Input;
                    return;
                }
                else
                {
                    return; //do nothing
                }
            }
            else
            {   // shift the shirftRegister and add the new bit
                Mapper_1_PB = (Mapper_1_ShiftRegister & 1) == 1;
                Mapper_1_ShiftRegister >>= 1;
                Mapper_1_ShiftRegister |= (byte)((Input & 1) << 4);
            }
            if (Mapper_1_PB) // if the '1' that was initialized in bit 4 is shifted into the bus
            {
                // copy shift register to the desired internal register.
                switch (Address & 0xE000)
                {
                    case 0x8000: //control
                        Mapper_1_Control = Mapper_1_ShiftRegister;
                        break;
                    case 0xA000: //CHR0
                        Mapper_1_CHR0 = Mapper_1_ShiftRegister;
                        break;
                    case 0xC000: //CHR1
                        Mapper_1_CHR1 = Mapper_1_ShiftRegister;
                        break;
                    case 0xE000: //PRG
                        Mapper_1_PRG = Mapper_1_ShiftRegister;
                        break;
                }
                Mapper_1_ShiftRegister = 0b10000;
            }
            if ((Input & 0b10000000) != 0)
            {
                Mapper_1_ShiftRegister = 0b10000;
                Mapper_1_Control |= 0b01100;
            }
        }
        public override byte SnoopCPU(ushort Address) // For debug purposes. It's a bit clunky.
        {
            if (Address >= 0x8000)
            {
                // The bank mode for MMC1:
                byte MMC1PRGROMBankMode = (byte)((Mapper_1_Control & 0b01100) >> 2);
                switch (MMC1PRGROMBankMode)
                {
                    case 0:
                    case 1:
                        {
                            // switch 32 KB at $8000, ignoring low bit of bank number
                            ushort tempo = (ushort)(Address & 0x7FFF);
                            return Cart.PRGROM[(0x8000 * (Mapper_1_PRG & 0x0E) + tempo) % Cart.PRGROM.Length];
                        }
                        break;
                    case 2:
                        // fix first bank at $8000 and switch 16 KB bank at $C000
                        if (Address >= 0xC000)
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            return Cart.PRGROM[0x4000 * (Mapper_1_PRG) + tempo];
                        }
                        else
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            return Cart.PRGROM[tempo];
                        }
                        break;
                    case 3:
                        // fix last bank at $C000 and switch 16 KB bank at $8000
                        if (Address >= 0xC000)
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            return Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + tempo];
                        }
                        else
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            return Cart.PRGROM[(0x4000 * (Mapper_1_PRG & 0x0F) + tempo) & (Cart.PRGROM.Length - 1)];
                        }
                        break;
                }
            }
            else // if the address is < $8000
            {
                if (((Mapper_1_PRG & 0x10) == 0) && Address >= 0x6000) // if Work RAM is enabled
                {
                    return Cart.PRGRAM[Address & 0x1FFF];
                }
                // else, open bus.
            }
            return Cart.Emu.dataBus;
        }
        public override int FetchPatternAddress(ushort Address)
        {
            // bit 4 of Mapper_1_Control controls how the pattern tables are swapped. if set, 2 banks of 4Kib. Otherwise, 1 8Kib bank
            if ((Mapper_1_Control & 0x10) != 0)
            {
                // with the MMC1 chip, you can swap out the pattern tables.
                // address < 0x1000 is the first pattern table, else, the second pattern table.
                // if the final write for the MMC1 shift register was in the $A000 - $BFFF, this updates Mapper_1_CHR0
                // if the final write for the MMC1 shift register was in the $B000 - $CFFF, this updates Mapper_1_CHR1
                if (Address < 0x1000) { return ((Mapper_1_CHR0 & 0x1F) * 0x1000 + Address) & (Cart.CHRROM.Length - 1); }
                else { Address &= 0xFFF; return ((Mapper_1_CHR1 & 0x1F) * 0x1000 + Address) & (Cart.CHRROM.Length - 1); }
            }
            else // one swappable bank that changes both pattern tables.
            {
                // this uses the value written to Mapper_1_CHR0
                return ((Mapper_1_CHR0 & 0b11111110) * 0x2000 + Address) & (Cart.CHRROM.Length - 1);
            }
        }
        public override void Connector_CheckCIRAM()
        {
            if (!Cart.Emu.ConnectorPinFloating[56]) { Cart.Emu.SeventyTwoPinConnector[56] = !Cart.Emu.SeventyTwoPinConnector[57]; }
            switch (Mapper_1_Control & 3)
            {
                case 0: //one screen, low
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = false; }
                    break;
                case 1: //one screen, high
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = true; }
                    break;
                case 2: //vertical
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = Cart.Emu.SeventyTwoPinConnector[62]; }
                    break;
                case 3: //horizontal
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = Cart.Emu.SeventyTwoPinConnector[61]; }
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
                switch (Mapper_1_Control & 3)
                {
                    case 0: //one screen, low
                        break;
                    case 1: //one screen, high
                        Addr |= 0x400;
                        break;
                    case 2: //vertical
                        Addr |= (ushort)(((Address & 0x400) != 0) ? 0x400 : 0);
                        break;
                    case 3: //horizontal
                        Addr |= (ushort)(((Address & 0x800) != 0) ? 0x400 : 0);
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
            State.Add(Mapper_1_ShiftRegister);
            State.Add(Mapper_1_Control);
            State.Add(Mapper_1_CHR0);
            State.Add(Mapper_1_CHR1);
            State.Add(Mapper_1_ShiftRegister);
            State.Add((byte)(Mapper_1_PB ? 1 : 0));
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
            Mapper_1_ShiftRegister = State[p++];
            Mapper_1_Control = State[p++];
            Mapper_1_CHR0 = State[p++];
            Mapper_1_CHR1 = State[p++];
            Mapper_1_ShiftRegister = State[p++];
            Mapper_1_PB = (State[p++] & 1) == 1;
            exitIndex = p;
        }
    }
}
