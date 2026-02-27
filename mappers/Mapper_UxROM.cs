using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_UxROM : Mapper
    {
        // ines Mapper 2
        public byte Mapper_2_BankSelect;
        public override void FetchPRG(ushort Address, bool Observe)
        {
            bool notFloating = false;
            byte data = 0;
            if (!Observe) { dataPinsAreNotFloating = false; } else { observedDataPinsAreNotFloating = false; }
            // Observing can happen on a different thread, so we need to ensure that observing doesn't overwrite the data bus or floating pins status.

            if (Address >= 0x8000)
            {
                notFloating = true;
                if (Address >= 0xC000)
                {
                    ushort tempo = (ushort)(Address & 0x3FFF);
                    data = Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + tempo];
                }
                else
                {
                    ushort tempo = (ushort)(Address & 0x3FFF);
                    data = Cart.PRGROM[0x4000 * (Mapper_2_BankSelect & 0x0F) + tempo];
                }
            }

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
                Mapper_2_BankSelect = (byte)(Input & 0xF);
            }
        }
        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            foreach (Byte b in Cart.CHRRAM) { State.Add(b); }
            State.Add(Mapper_2_BankSelect);
            return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.CHRRAM.Length; i++) { Cart.CHRRAM[i] = State[p++]; }
            Mapper_2_BankSelect = State[p++];
            exitIndex = p;
        }
    }
}