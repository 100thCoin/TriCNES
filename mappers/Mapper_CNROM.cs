using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_CNROM : Mapper
    {
        // ines Mapper 3
        public byte Mapper_3_CHRBank;
        public override void StorePRG(ushort Address, byte Input)
        {
            if (Address >= 0x8000)
            {
                Mapper_3_CHRBank = (byte)(Input & 0x3);
            }
        }
        public override byte FetchCHR(ushort Address, bool Observe)
        {
            return Cart.CHRROM[(Mapper_3_CHRBank * 0x2000 + Address) & (Cart.CHRROM.Length - 1)];
        }
        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            foreach (Byte b in Cart.CHRRAM) { State.Add(b); }
            State.Add(Mapper_3_CHRBank);
            return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.CHRRAM.Length; i++) { Cart.CHRRAM[i] = State[p++]; }
            Mapper_3_CHRBank = State[p++];
            exitIndex = p;
        }
    }
}