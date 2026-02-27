using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_MMC3 : Mapper
    {
        // ines Mapper 4
        public byte Mapper_4_8000;      // The value written to $8000 (or any even address between $8000 and $9FFE)
        public byte Mapper_4_BankA;     // The PRG bank between $A000 and $BFFF
        public byte Mapper_4_Bank8C;    // The PRG bank that could either be at $8000 through 9FFF, or $C000 through $DFFF
        public byte Mapper_4_CHR_2K0;
        public byte Mapper_4_CHR_2K8;
        public byte Mapper_4_CHR_1K0;
        public byte Mapper_4_CHR_1K4;
        public byte Mapper_4_CHR_1K8;
        public byte Mapper_4_CHR_1KC;
        public byte Mapper_4_IRQLatch;
        public byte Mapper_4_IRQCounter;
        public bool Mapper_4_EnableIRQ;
        public bool Mapper_4_ReloadIRQCounter;
        public bool Mapper_4_NametableMirroring; // MMC3 has it's own way of controlling how the nametables are mirrored.
        public byte Mapper_4_PRGRAMProtect;
        public byte Mapper_4_M2Filter;
        public override void FetchPRG(ushort Address, bool Observe)
        {
            bool notFloating = false;
            byte data = 0;
            if (!Observe) { dataPinsAreNotFloating = false; } else { observedDataPinsAreNotFloating = false; }
            // Observing can happen on a different thread, so we need to ensure that observing doesn't overwrite the data bus or floating pins status.

            if (Address >= 0xE000) // This bank is fixed the the final PRG bank of the ROM
            {
                notFloating = true;
                data = Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x3FFF)];
            }
            else if (Address >= 0xC000)
            {
                notFloating = true;
                if ((Mapper_4_8000 & 0x40) == 0x40)
                {
                    //$C000 swappable
                    data = Cart.PRGROM[(Mapper_4_Bank8C << 13) | (Address & 0x1FFF)];
                }
                else
                {
                    //$8000 swappable
                    data = Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x1FFF)];
                }
            }
            else if (Address >= 0xA000)
            {
                notFloating = true;
                //$8000 swappable
                data = Cart.PRGROM[(Mapper_4_BankA << 13) | (Address & 0x1FFF)];
            }
            else if (Address >= 0x8000)
            {
                notFloating = true;
                if ((Mapper_4_8000 & 0x40) == 0x40)
                {
                    //$8000 swappable
                    data = Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x1FFF)];
                }
                else
                {
                    //$C000 swappable
                    data = Cart.PRGROM[(Mapper_4_Bank8C << 13) | (Address & 0x1FFF)];
                }
            }
            else if (Address >= 0x6000)
            {
                if (Cart.SubMapper == 1) // MMC6
                {
                    if ((Mapper_4_8000 & 0x20) != 0)
                    {
                        // MMC6 differs from MMC3 since there's only 1Kib of PRG RAM
                        if (Address >= 0x7000 && Address <= 0x71FF)
                        {
                            if ((Mapper_4_PRGRAMProtect & 0x20) != 0)
                            {
                                notFloating = true;
                                data = Cart.PRGRAM[Address & 0x3FF];
                            }
                        }
                        else if (Address >= 0x7200 && Address <= 0x73FF)
                        {
                            if ((Mapper_4_PRGRAMProtect & 0x80) != 0)
                            {
                                notFloating = true;
                                data = Cart.PRGRAM[Address & 0x3FF];
                            }
                        }
                    }
                }
                else
                {
                    if ((Mapper_4_PRGRAMProtect & 0x80) != 0)
                    {
                        notFloating = true;
                        data = Cart.PRGRAM[Address & 0x1FFF];
                    }
                }
            }
            //else, open bus

            if (notFloating)
            {
                EndFetchPRG(Observe, data);
            }
            return;
        }
        public override void StorePRG(ushort Address, byte Input)
        {
            if (Address < 0x8000)
            {   //Battery backed RAM

                if (Cart.SubMapper == 1) // MMC6
                {
                    // MMC6 differs from MMC3 since there's only 1Kib of PRG RAM
                    if ((Mapper_4_8000 & 0x20) != 0)
                    {
                        if (Address >= 0x7000 && Address <= 0x71FF)
                        {
                            if ((Mapper_4_PRGRAMProtect & 0x10) != 0)
                            {
                                Cart.PRGRAM[Address & 0x3FF] = Input;

                            }
                        }
                        else if (Address >= 0x7200 && Address <= 0x73FF)
                        {
                            if ((Mapper_4_PRGRAMProtect & 0x40) != 0)
                            {
                                Cart.PRGRAM[Address & 0x3FF] = Input;
                            }
                        }
                    }
                }
                else if ((Mapper_4_PRGRAMProtect & 0xC0) != 0) // bit 7 enables PRG RAM, bit 6 enables writing there.
                {
                    Cart.PRGRAM[Address & 0x1FFF] = Input;
                }
                return;
            }
            else
            {
                ushort tempo = (ushort)(Address & 0xE001);
                switch (tempo)
                {
                    case 0x8000:
                        Mapper_4_8000 = Input;
                        return;
                    case 0x8001:
                        byte mode = (byte)(Mapper_4_8000 & 7);
                        switch (mode)
                        {
                            case 0: //PPU ($0000 - $07FF) ?+ $1000
                                Mapper_4_CHR_2K0 = (byte)(Input & 0xFE);
                                return;
                            case 1: //PPU ($0800 - $0FFF) ?+ $1000
                                Mapper_4_CHR_2K8 = (byte)(Input & 0xFE);
                                return;
                            case 2: //PPU ($1000 - $13FF) ?- $1000
                                Mapper_4_CHR_1K0 = Input;
                                return;
                            case 3: //PPU ($1400 - $17FF) ?- $1000
                                Mapper_4_CHR_1K4 = Input;
                                return;
                            case 4: //PPU ($1800 - $1BFF) ?- $1000
                                Mapper_4_CHR_1K8 = Input;
                                return;
                            case 5: //PPU ($1C00 - $1FFF) ?- $1000
                                Mapper_4_CHR_1KC = Input;
                                return;
                            case 6: //PRG ($8000 - $9FFF) ?+ 0x4000
                                Mapper_4_Bank8C = (byte)(Input & (Cart.PRG_Size * 2 - 1));
                                return;
                            case 7: //PRG ($A000 - $BFFF)
                                Mapper_4_BankA = (byte)(Input & (Cart.PRG_Size * 2 - 1));
                                return;
                        }
                        return;
                    case 0xA000:
                        Mapper_4_NametableMirroring = (Input & 1) == 1;
                        return;
                    case 0xA001:
                        Mapper_4_PRGRAMProtect = Input;
                        return;
                    case 0xC000:
                        Mapper_4_IRQLatch = Input;
                        return;
                    case 0xC001:
                        Mapper_4_IRQCounter = 0xFF;
                        Mapper_4_ReloadIRQCounter = true;
                        return;
                    case 0xE000:
                        Mapper_4_EnableIRQ = false;
                        Cart.Emu.IRQ_LevelDetector = false;
                        return;
                    case 0xE001:
                        Mapper_4_EnableIRQ = true;
                        return;
                }
            }
        }
        public override byte FetchCHR(ushort Address, bool Observe)
        {
            //Writes to $8000 determine the mode, writes to $8001 determine the banks
            if ((Mapper_4_8000 & 0x80) == 0) // bit 7 of the previous write to $8000 determines which pattern table is 2 2kb banks, and which is 4 1kb banks.
            {
                if (Address < 0x800) { return Cart.CHRROM[(Mapper_4_CHR_2K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0x1000) { Address &= 0x7FF; return Cart.CHRROM[(Mapper_4_CHR_2K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0x1400) { Address &= 0x3FF; return Cart.CHRROM[(Mapper_4_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0x1800) { Address &= 0x3FF; return Cart.CHRROM[(Mapper_4_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0x1C00) { Address &= 0x3FF; return Cart.CHRROM[(Mapper_4_CHR_1K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else { Address &= 0x3FF; return Cart.CHRROM[(Mapper_4_CHR_1KC * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
            }
            else
            {
                if (Address < 0x400) { return Cart.CHRROM[(Mapper_4_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0x800) { Address &= 0x3FF; return Cart.CHRROM[(Mapper_4_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0xC00) { Address &= 0x3FF; return Cart.CHRROM[(Mapper_4_CHR_1K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0x1000) { Address &= 0x3FF; return Cart.CHRROM[(Mapper_4_CHR_1KC * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else if (Address < 0x1800) { Address &= 0x7FF; return Cart.CHRROM[(Mapper_4_CHR_2K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                else { Address &= 0x7FF; return Cart.CHRROM[(Mapper_4_CHR_2K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
            }
        }
        public override ushort MirrorNametable(ushort Address)
        {
            if (!Cart.AlternativeNametableArrangement)
            {
                if (Mapper_4_NametableMirroring) //horizontal
                {
                    return (ushort)((Address & 0x33FF) | ((Address & 0x0800) >> 1)); // mask away $0C00, bit 10 becomes the former bit 11
                }
                else //vertical
                {
                    return (ushort)(Address & 0x37FF); // mask away $0800
                }
            }
            return Address;
        }
        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            foreach (Byte b in Cart.CHRRAM) { State.Add(b); }
            foreach (Byte b in Cart.PRGVRAM) { State.Add(b); }
            State.Add(Mapper_4_8000);
            State.Add(Mapper_4_BankA);
            State.Add(Mapper_4_Bank8C);
            State.Add(Mapper_4_CHR_2K0);
            State.Add(Mapper_4_CHR_2K8);
            State.Add(Mapper_4_CHR_1K0);
            State.Add(Mapper_4_CHR_1K4);
            State.Add(Mapper_4_CHR_1K8);
            State.Add(Mapper_4_CHR_1KC);
            State.Add(Mapper_4_IRQLatch);
            State.Add(Mapper_4_IRQCounter);
            State.Add((byte)(Mapper_4_EnableIRQ ? 1 : 0));
            State.Add((byte)(Mapper_4_ReloadIRQCounter ? 1 : 0));
            State.Add((byte)(Mapper_4_NametableMirroring ? 1 : 0));
            State.Add(Mapper_4_PRGRAMProtect);
            State.Add(Mapper_4_M2Filter);
            return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.CHRRAM.Length; i++) { Cart.CHRRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.PRGVRAM.Length; i++) { Cart.PRGVRAM[i] = State[p++]; }
            Mapper_4_8000 = State[p++];
            Mapper_4_BankA = State[p++];
            Mapper_4_Bank8C = State[p++];
            Mapper_4_CHR_2K0 = State[p++];
            Mapper_4_CHR_2K8 = State[p++];
            Mapper_4_CHR_1K0 = State[p++];
            Mapper_4_CHR_1K4 = State[p++];
            Mapper_4_CHR_1K8 = State[p++];
            Mapper_4_CHR_1KC = State[p++];
            Mapper_4_IRQLatch = State[p++];
            Mapper_4_IRQCounter = State[p++];
            Mapper_4_EnableIRQ = (State[p++] & 1) == 1;
            Mapper_4_ReloadIRQCounter = (State[p++] & 1) == 1;
            Mapper_4_NametableMirroring = (State[p++] & 1) == 1;
            Mapper_4_PRGRAMProtect = State[p++];
            Mapper_4_M2Filter = State[p++];
            exitIndex = p;
        }

        public override void PPUClock()
        {
            // if bit 12 of the ppu address bus (A12) changes:
            if (!Cart.Emu.PPU_A12_Prev && ((Cart.Emu.PPU_AddressBus & 0b0001000000000000) != 0) && Mapper_4_M2Filter == 3)
            {
                if (Mapper_4_ReloadIRQCounter)
                {
                    // If we're reloading the IRQ counter
                    Mapper_4_IRQCounter = Mapper_4_IRQLatch; // The latch is the reset value.
                    Mapper_4_ReloadIRQCounter = false;
                    if (Mapper_4_IRQCounter == 0)  // if the latch is set to 0, you need to enable the IRQ.
                    {
                        if (Mapper_4_EnableIRQ) // if setting the value to zero, run an IRQ
                        {
                            Cart.Emu.IRQ_LevelDetector = true;
                        }
                    }
                }
                else
                {
                    // decrement the counter
                    Mapper_4_IRQCounter--;
                    if (Mapper_4_IRQCounter == 0) // if decrementing the counter moved it to 0...
                    {
                        if (Mapper_4_EnableIRQ) // and the MMC3 IRQ is enabled...
                        {
                            Cart.Emu.IRQ_LevelDetector = true; // Run an IRQ!
                        }
                    }
                    else if (Mapper_4_IRQCounter == 255) // if the counter underflows...
                    {
                        Mapper_4_IRQCounter = Mapper_4_IRQLatch; // reset the irq counter
                        if (Mapper_4_IRQCounter == 0)  // if the latch is set to 0, you need to enable the IRQ... again
                        {
                            if (Mapper_4_EnableIRQ)
                            {
                                Cart.Emu.IRQ_LevelDetector = true;
                            }
                        }
                    }

                }
            }
            if ((Cart.Emu.PPU_AddressBus & 0b0001000000000000) != 0)
            {
                Mapper_4_M2Filter = 0;
            }
        }
        public override void CPUClockRise()
        {
            if ((Cart.Emu.PPU_AddressBus & 0b0001000000000000) == 0)
            {
                if (Mapper_4_M2Filter < 3)
                {
                    Mapper_4_M2Filter++;
                }
            }
        }
    }
}
