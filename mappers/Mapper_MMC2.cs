using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_MMC2 : Mapper
    {
        // ines Mapper 9
        public byte Mapper_9_BankSelect;
        public byte Mapper_9_CHR0_FD;
        public byte Mapper_9_CHR0_FE;
        public byte Mapper_9_CHR1_FD;
        public byte Mapper_9_CHR1_FE;
        public bool Mapper_9_NametableMirroring;
        public bool Mapper_9_Latch0_FE;
        public bool Mapper_9_Latch1_FE; 
        public override void FetchPRG(ushort Address, bool Observe)
        {
            bool notFloating = false;
            byte data = 0;
            if (!Observe) { dataPinsAreNotFloating = false; } else { observedDataPinsAreNotFloating = false; }
            // Observing can happen on a different thread, so we need to ensure that observing doesn't overwrite the data bus or floating pins status.

            if (Address >= 0xA000)
            {
                notFloating = true;
                data = Cart.PRGROM[((Cart.PRG_Size - 2) << 14) | (Address & 0x7FFF)];
            }
            else if(Address >= 0x8000)
            {
                notFloating = true;
                data = Cart.PRGROM[(Mapper_9_BankSelect << 13) | (Address & 0x1FFF)];
            }

            if (notFloating)
            {
                EndFetchPRG(Observe, data);
            }
            return;
        }
        public override void StorePRG(ushort Address, byte Input)
        {
            if (Address < 0xA000)
            {
                // nothing
            }
            else if (Address < 0xB000) // PRG Bank select
            {
                Mapper_9_BankSelect = (byte)(Input & 0x0F);
            }
            else if (Address < 0xC000) // CHR0 Bank select
            {
                Mapper_9_CHR0_FD = (byte)(Input & 0x1F);
            }
            else if (Address < 0xD000) // CHR0 Bank select
            {
                Mapper_9_CHR0_FE = (byte)(Input & 0x1F);
            }
            else if (Address < 0xE000) // CHR1 Bank select
            {
                Mapper_9_CHR1_FD = (byte)(Input & 0x1F);
            }
            else if (Address < 0xF000) // CHR1 Bank select
            {
                Mapper_9_CHR1_FE = (byte)(Input & 0x1F);
            }
            else // Nametable mirroring
            {
                Mapper_9_NametableMirroring = (Input & 0x1) == 1;
            }
        }
        public override byte FetchCHR(ushort Address, bool Observe)
        {
            byte temp = 0;
            ushort Addr = Address;
            if (Address < 0x1000) { temp = Cart.CHRROM[(Mapper_9_Latch0_FE ? Mapper_9_CHR0_FE : Mapper_9_CHR0_FD) * 0x1000 + Addr]; }
            else { Addr &= 0xFFF; temp = Cart.CHRROM[(Mapper_9_Latch1_FE ? Mapper_9_CHR1_FE : Mapper_9_CHR1_FD) * 0x1000 + Addr]; }
            if (!Observe)
            {
                if (Address == 0x0FD8)
                {
                    Mapper_9_Latch0_FE = false;
                }
                else if (Address == 0x0FE8)
                {
                    Mapper_9_Latch0_FE = true;
                }
                else if (Address >= 0x1FD8 && Address <= 0x1FDF)
                {
                    Mapper_9_Latch1_FE = false;
                }
                else if (Address >= 0x1FE8 && Address <= 0x1FEF)
                {
                    Mapper_9_Latch1_FE = true;
                }
            }
            return temp;
        }
        public override ushort MirrorNametable(ushort Address)
        {
            if (Mapper_9_NametableMirroring) //horizontal
            {
                Address = (ushort)((Address & 0x33FF) | ((Address & 0x0800) >> 1)); // mask away $0C00, bit 10 becomes the former bit 11
            }
            else //vertical
            {
                Address &= 0x37FF; // mask away $0800
            }
            return Address;
        }
        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            foreach (Byte b in Cart.CHRRAM) { State.Add(b); }
            State.Add(Mapper_9_BankSelect);
            State.Add(Mapper_9_CHR0_FD);
            State.Add(Mapper_9_CHR0_FE);
            State.Add(Mapper_9_CHR1_FD);
            State.Add(Mapper_9_CHR1_FE);
            State.Add((byte)(Mapper_9_NametableMirroring ? 1 : 0));
            State.Add((byte)(Mapper_9_Latch0_FE ? 1 : 0));
            State.Add((byte)(Mapper_9_Latch1_FE ? 1 : 0)); return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.CHRRAM.Length; i++) { Cart.CHRRAM[i] = State[p++]; }
            Mapper_9_BankSelect = State[p++];
            Mapper_9_CHR0_FD = State[p++];
            Mapper_9_CHR0_FE = State[p++];
            Mapper_9_CHR1_FD = State[p++];
            Mapper_9_CHR1_FE = State[p++];
            Mapper_9_NametableMirroring = (State[p++] & 1) == 1;
            Mapper_9_Latch0_FE = (State[p++] & 1) == 1;
            Mapper_9_Latch1_FE = (State[p++] & 1) == 1;
            exitIndex = p;
        }
    }
}