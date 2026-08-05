using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_UxROM : Mapper
    {
        // ines Mapper 2
        public byte Mapper_2_BankSelect;
        public override void FetchCPU()
        {
            if ((Cart.Emu.ConnectorPinFloating[0] && Cart.Emu.ConnectorPinFloating[71]) || Cart.Emu.ConnectorPinFloating[35]) { return; } // If the cartridge is disconnected from power or ground, it cannot do anything.
            Connector_ReadCPUAddressPins();

            if (CPU_AddressIn >= 0x8000)
            {
                if (CPU_AddressIn >= 0xC000)
                {
                    CPU_DataOut = Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + (CPU_AddressIn & 0x3FFF)];
                }
                else
                {
                    CPU_DataOut = Cart.PRGROM[0x4000 * (Mapper_2_BankSelect & 0x0F) + (CPU_AddressIn & 0x3FFF)];
                }
                Connector_SetUpCPUDataPins(CPU_DataOut);
            }

            return;
        }
        public override void StoreCPU(ushort Address, byte Input)
        {
            if (Address >= 0x8000)
            {
                Mapper_2_BankSelect = (byte)(Input & 0xF);
            }
        }
        public override byte SnoopCPU(ushort Address) // For debug purposes. It's a bit clunky.
        {
            if (Address >= 0x8000)
            {
                if (CPU_AddressIn >= 0xC000)
                {
                    return Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + (Address & 0x3FFF)];
                }
                return Cart.PRGROM[0x4000 * (Mapper_2_BankSelect & 0x0F) + (Address & 0x3FFF)];
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
            State.Add(Mapper_2_BankSelect);
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
            Mapper_2_BankSelect = State[p++];
            exitIndex = p;
        }
    }
}