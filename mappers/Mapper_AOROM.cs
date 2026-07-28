using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_AOROM : Mapper
    {
        // ines Mapper 7
        public byte Mapper_7_BankSelect;
        public override void FetchPRG(ushort Address, bool Observe)
        {
            if (!Observe)
            {
                Address = Connector_ReadCPUAddressPins();
            }
            bool notFloating = false;
            byte data = 0;
            if (!Observe) { dataPinsAreNotFloating = false; } else { observedDataPinsAreNotFloating = false; }
            // Observing can happen on a different thread, so we need to ensure that observing doesn't overwrite the data bus or floating pins status.

            if (Address >= 0x8000)
            {
                ushort tempo = (ushort)(Address & 0x7FFF);
                data = Cart.PRGROM[(0x8000 * (Mapper_7_BankSelect & 0x07) + tempo) & (Cart.PRGROM.Length - 1)]; // Get the address from the ROM file. If the ROM only has $4000 bytes, this will make addresses > $BFFF mirrors of $8000 through $BFFF.
                notFloating = true;
            }
            //open bus

            if (notFloating)
            {
                EndFetchPRG(Observe, data);
            }
            return;
        }
        public override void StorePRG(ushort Address, byte Input)
        {
            if (Address >= 0x8000)
            {
                Mapper_7_BankSelect = Input;
            }
        }
        public override void Connector_CheckCIRAM()
        {
            if (!Cart.Emu.ConnectorPinFloating[56]) { Cart.Emu.SeventyTwoPinConnector[56] = !Cart.Emu.SeventyTwoPinConnector[57]; }
            if ((Mapper_7_BankSelect & 0x10) == 0) // show nametable 0
            {
                if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = false; }
            }
            else // show nametable 1
            {
                if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = true; }
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