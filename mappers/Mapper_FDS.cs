using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_FDS : Mapper
    {
        // The Famicom Disk System
        public byte[] FDS_BIOS;

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
                dataPinsAreNotFloating = true;                
                dataBus = FDS_BIOS[Address & 0x1FFF];
            }
            else if (Address >= 0x6000)
            {
                // read from the FDS PRG RAM
                dataPinsAreNotFloating = true;
                dataBus = Cart.PRGRAM[Address-0x6000];
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
    }
}
