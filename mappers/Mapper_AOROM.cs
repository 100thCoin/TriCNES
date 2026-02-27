using System;
using System.Collections.Generic;
using System.Net;

namespace TriCNES.mappers
{
    public class Mapper_AOROM : Mapper
    {
        // ines Mapper 7
        public byte Mapper_7_BankSelect;
        public override void FetchPRG(ushort Address, bool Observe)
        {
            bool notFloating = false;
            byte data = 0;
            if (!Observe) { dataPinsAreNotFloating = false; } else { observedDataPinsAreNotFloating = false; }
            // Observing can happen on a different thread, so we need to ensure that observing doesn't overwrite the data bus or floating pins status.

            if (Address >= 0x8000)
            {
                dataPinsAreNotFloating = true;
                ushort tempo = (ushort)(Address & 0x7FFF);
                dataBus = Cart.PRGROM[(0x8000 * (Mapper_7_BankSelect & 0x07) + tempo) & (Cart.PRGROM.Length - 1)];
            }
            // AOROM doesn't have any PRG RAM

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
        public override ushort MirrorNametable(ushort Address)
        {
            if ((Mapper_7_BankSelect & 0x10) == 0) // show nametable 0
            {
                Address &= 0x33FF;
            }
            else // show nametable 1
            {
                Address &= 0x33FF;
                Address |= 0x400;
            }
            return Address;
        }
        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            foreach (Byte b in Cart.CHRRAM) { State.Add(b); }
            State.Add(Mapper_7_BankSelect);
            return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.CHRRAM.Length; i++) { Cart.CHRRAM[i] = State[p++]; }
            Mapper_7_BankSelect = State[p++];
            exitIndex = p;
        }
    }
}