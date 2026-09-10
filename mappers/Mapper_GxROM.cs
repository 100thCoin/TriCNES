using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_GxROM : Mapper
    {
        // ines Mapper 66
        public byte Mapper_66_BankSelect_CHR;
        public byte Mapper_66_BankSelect_PRG;
        public override int FetchPatternAddress(ushort Address)
        {
            return (Mapper_66_BankSelect_CHR * 0x2000 + Address) & (Cart.CHRROM.Length - 1);
        }
        public override void FetchCPU()
        {
            if ((Cart.Emu.ConnectorPinFloating[0] && Cart.Emu.ConnectorPinFloating[71]) || Cart.Emu.ConnectorPinFloating[35]) { return; } // If the cartridge is disconnected from power or ground, it cannot do anything.
            Connector_ReadCPUAddressPins();

            if (CPU_AddressIn >= 0x8000)
            {
                CPU_DataOut = Cart.PRGROM[0x8000 * (Mapper_66_BankSelect_PRG) + (CPU_AddressIn & 0x7FFF)];
                Connector_SetUpCPUDataPins(CPU_DataOut);
            }

            return;
        }
        public override void StoreCPU(ushort Address, byte Input)
        {
            if (Address >= 0x8000)
            {
                Mapper_66_BankSelect_CHR = (byte)(Input & 0x3);
                Mapper_66_BankSelect_PRG = (byte)((Input & 0x30) >> 4);
            }
        }
        public override byte SnoopCPU(ushort Address) // For debug purposes. It's a bit clunky.
        {
            if (Address >= 0x8000)
            {
                return Cart.PRGROM[0x8000 * (Mapper_66_BankSelect_PRG) + (Address & 0x7FFF)];
            }
            return Cart.Emu.dataBus;
        }
        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            if (Cart.UsingCHRRAM)
            {
                foreach (Byte b in Cart.CHRROM) { State.Add(b); }
            }
            State.Add(Mapper_66_BankSelect_CHR);
            State.Add(Mapper_66_BankSelect_PRG);
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
            Mapper_66_BankSelect_CHR = State[p++];
            Mapper_66_BankSelect_PRG = State[p++];
            exitIndex = p;
        }
    }
}