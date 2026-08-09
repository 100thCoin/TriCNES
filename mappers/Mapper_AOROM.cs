using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_AOROM : Mapper
    {
        // ines Mapper 7
        public byte Mapper_7_BankSelect;
        public override void FetchCPU()
        {
            if ((Cart.Emu.ConnectorPinFloating[0] && Cart.Emu.ConnectorPinFloating[71]) || Cart.Emu.ConnectorPinFloating[35]) { return; } // If the cartridge is disconnected from power or ground, it cannot do anything.
            Connector_ReadCPUAddressPins();

            if (!Cart.Emu.SeventyTwoPinConnector[49]) // CPU /A15 + /M2
            {
                CPU_DataOut = Cart.PRGROM[(0x8000 * (Mapper_7_BankSelect & 0x07) + (CPU_AddressIn & 0x7FFF)) & (Cart.PRGROM.Length - 1)]; // Get the address from the ROM file. If the ROM only has $4000 bytes, this will make addresses > $BFFF mirrors of $8000 through $BFFF.
                Connector_SetUpCPUDataPins(CPU_DataOut);
            }

            return;
        }
        public override void StoreCPU(ushort Address, byte Input)
        {
            if (Address >= 0x8000)
            {
                Mapper_7_BankSelect = Input;
            }
        }
        public override byte SnoopCPU(ushort Address) // For debug purposes. It's a bit clunky.
        {
            if (Address >= 0x8000)
            {
                return Cart.PRGROM[(0x8000 * (Mapper_7_BankSelect & 0x07) + (Address & 0x7FFF)) & (Cart.PRGROM.Length - 1)]; // Get the address from the ROM file. If the ROM only has $4000 bytes, this will make addresses > $BFFF mirrors of $8000 through $BFFF.
            }
            return Cart.Emu.dataBus;
        }
        public override void Connector_CheckCIRAM()
        {
            if (TiltingCart)
            {
                if (!Cart.Emu.ConnectorPinFloating[56]) { Cart.Emu.SeventyTwoPinConnector[56] = Cart.Emu.SeventyTwoPinConnector[57]; }
                if (!Cart.Emu.ConnectorPinFloating[21])
                {
                    Cart.Emu.SeventyTwoPinConnector[21] = (Mapper_7_BankSelect & 0x10) != 0;
                }
            }
            else
            {
                Cart.Emu.SeventyTwoPinConnector[56] = (Cart.Emu.PPU_AddressBus & 0x2000) == 0;
                Cart.Emu.SeventyTwoPinConnector[21] = (Mapper_7_BankSelect & 0x10) != 0;
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
                Addr |= (ushort)(((Mapper_7_BankSelect & 0x10) == 0) ? 0 : 0x400);
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
            State.Add(Mapper_7_BankSelect);
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
            Mapper_7_BankSelect = State[p++];
            exitIndex = p;
        }
    }
}