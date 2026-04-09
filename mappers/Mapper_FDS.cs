using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_FDS : Mapper
    {
        // The Famicom Disk System
        public byte[] FDS_BIOS;

        public byte FDS_4025_Control;

        public Mapper_FDS(byte[] fds_bios)
        {
            FDS_BIOS = fds_bios;
        }

        public override void FetchPRG(ushort Address, bool Observe)
        {
            bool notFloating = false;
            byte data = 0;
            if (!Observe) { dataPinsAreNotFloating = false; } else { observedDataPinsAreNotFloating = false; }
            // Observing can happen on a different thread, so we need to ensure that observing doesn't overwrite the data bus or floating pins status.

            if (Address >= 0xE000)
            {
                // read from the FDS BIOS
                notFloating = true;
                data = FDS_BIOS[Address & 0x1FFF];
            }
            else if (Address >= 0x6000)
            {
                // read from the FDS PRG RAM
                notFloating = true;
                data = Cart.PRGRAM[Address-0x6000];
            }
            else if (Address >= 4030 && Address <= 0x403F)
            {
                // Read from the FDS Registers
                Address &= 0xF;
                switch (Address)
                {
                    default: break;
                    case 0:
                        {
                            // FDS Status ($4030)

                        }
                        break;
                    case 1:
                        {
                            // Disk Data Input ($4031)
                            notFloating = true;
                            data = Cart.FDS.ShiftRegister;
                            Cart.Emu.IRQ_LevelDetector = false; //acknowledge the IRQ
                        }
                        break;
                    case 2:
                        {
                            // Disk Drive Status ($4032)

                        }
                        break;
                    case 3:
                        {
                            // External Connector Input ($4033)
                            notFloating = true;
                            data = 0x80; // The battery is good.
                        }
                        break;
                }
            }

            if (notFloating)
            {
                EndFetchPRG(Observe, data);
            }
            return;
        }
        public override byte FetchCHR(ushort Address, bool Observe)
        {
            return Cart.CHRRAM[Address];
        }

        public override void StorePRG(ushort Address, byte Input)
        {
            if (Address >= 0x6000 && Address < 0xE000)
            {
                Cart.PRGRAM[Address-0x6000] = Input;
                return;
            }
            else if (Address > 0x401F)
            {
                ushort tempo = (ushort)(Address & 0x40FF);
                switch (tempo)
                {
                    case 0x4025:
                        FDS_4025_Control = Input;
                        return;
                }
            }
        }

        public override void FDS_ByteTransferFlag()
        {
            if((FDS_4025_Control & 0x80) != 0)
            {
                Cart.Emu.IRQ_LevelDetector = true;
            }
        }

        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            foreach (Byte b in Cart.CHRRAM) { State.Add(b); }

            State.Add(FDS_4025_Control);
            State.Add((byte)Cart.FDS.clock);
            State.Add((byte)(Cart.FDS.clock >> 8));
            State.Add((byte)Cart.FDS.ShiftRegister);
            return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.CHRRAM.Length; i++) { Cart.CHRRAM[i] = State[p++]; }

            FDS_4025_Control = State[p++];
            Cart.FDS.clock = State[p++];
            Cart.FDS.clock |= (ushort)(State[p++] << 8);
            Cart.FDS.ShiftRegister = State[p++];

            exitIndex = p;
        }

    }
}
