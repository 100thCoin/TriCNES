using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_FDS : Mapper
    {
        // The Famicom Disk System
        public byte[] FDS_BIOS;

        public byte FDS_4023_IOEnable;
        public byte FDS_4025_Control;

        public Mapper_FDS(byte[] fds_bios)
        {
            FDS_BIOS = fds_bios;
        }

        public override void FetchCPU()
        {
            if ((Cart.Emu.ConnectorPinFloating[0] && Cart.Emu.ConnectorPinFloating[71]) || Cart.Emu.ConnectorPinFloating[35]) { return; } // If the cartridge is disconnected from power or ground, it cannot do anything.
            Connector_ReadCPUAddressPins();
            ushort Address = CPU_AddressIn;

            if (CPU_AddressIn >= 0xE000)
            {
                // read from the FDS BIOS
                CPU_DataOut = FDS_BIOS[Address & 0x1FFF];
                Connector_SetUpCPUDataPins(CPU_DataOut);
            }
            else if (Address >= 0x6000)
            {
                // read from the FDS PRG RAM
                CPU_DataOut = Cart.PRGRAM[Address-0x6000];
                Connector_SetUpCPUDataPins(CPU_DataOut);
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
                            CPU_DataOut = 0;
                            CPU_DataOut |= (byte)((FDS_4025_Control >> 3) & 1); // 4030.3 = 4025.3
                            CPU_DataOut |= (byte)((Cart.FDS.DiskAddress >= Cart.FDS.Disk.Length) ? 0x40 : 0); // 4030.6 = End of Disk
                            CPU_DataOut |= (byte)(Cart.FDS.Status_ByteTransferFlag ? 0x80 : 0); // 4030.7 = Byte Transfer Flag
                            Connector_SetUpCPUDataPins(CPU_DataOut);
                        }
                        break;
                    case 1:
                        {
                            // Disk Data Input ($4031)
                            CPU_DataOut = Cart.FDS.ShiftRegisterLatch;
                            Connector_SetUpCPUDataPins(CPU_DataOut);
                            Cart.FDS.Status_ByteTransferFlag = false;
                            Connector_IRQPin(false); //acknowledge the IRQ
                        }
                        break;
                    case 2:
                        {
                            // Disk Drive Status ($4032)
                            CPU_DataOut = 0;
                            if(Cart.FDS.CurrentState == DiskDrive.RamAdapterState.INSERTING)
                            {
                                CPU_DataOut |= 1;
                            }
                            if (!(((FDS_4025_Control & 2) == 0) && (Cart.FDS.CurrentState == DiskDrive.RamAdapterState.RUNNING || Cart.FDS.CurrentState == DiskDrive.RamAdapterState.IDLE)))
                            {
                                CPU_DataOut |= 2;
                            }
                            Connector_SetUpCPUDataPins(CPU_DataOut);
                        }
                        break;
                    case 3:
                        {
                            // External Connector Input ($4033)
                            CPU_DataOut = 0x80; // The battery is good.
                            Connector_SetUpCPUDataPins(CPU_DataOut);
                        }
                        break;
                }
            }

            return;
        }
        public override int FetchPatternAddress(ushort Address)
        {
            return Address;
        }

        public override void StoreCPU(ushort Address, byte Input)
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
                    case 0x4023:
                        FDS_4023_IOEnable = Input;
                        if((FDS_4023_IOEnable & 1) == 0)
                        {
                            // Disable disk I/O registers
                            Connector_IRQPin(false); //acknowledge the IRQ
                            FDS_4025_Control &= 0xF3;
                            FDS_4025_Control |= 6;
                        }
                        return;
                    case 0x4024:
                        Cart.FDS.Status_ByteTransferFlag = false;
                        return;
                    case 0x4025:
                        if((FDS_4025_Control & 0x40) == 0 && (Input & 0x40) != 0)
                        {
                            Cart.FDS.lookingForEndOfGap = true;
                        }
                        FDS_4025_Control = Input;
                        if ((Input & 1) != 0)
                        {
                            if (Cart.FDS.CurrentState == DiskDrive.RamAdapterState.IDLE)
                            {
                                Cart.FDS.CurrentState = DiskDrive.RamAdapterState.SPINUP;
                            }
                        }
                        if((FDS_4025_Control & 2) != 0)
                        {
                            // debugging: put breakpoint here
                        }
                        if ((FDS_4025_Control & 2) == 0)
                        {
                            // debugging: put breakpoint here
                        }
                        return;
                }
            }
        }

        public override byte SnoopCPU(ushort Address) // For debug purposes. It's a bit clunky.
        {
            if (Address >= 0xE000)
            {
                // read from the FDS BIOS
                return FDS_BIOS[Address & 0x1FFF];
            }
            else if (Address >= 0x6000)
            {
                // read from the FDS PRG RAM
                return Cart.PRGRAM[Address - 0x6000];
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
                            byte t = 0;
                            t |= (byte)((FDS_4025_Control >> 3) & 1); // 4030.3 = 4025.3
                            t |= (byte)((Cart.FDS.DiskAddress >= Cart.FDS.Disk.Length) ? 0x40 : 0); // 4030.6 = End of Disk
                            t |= (byte)(Cart.FDS.Status_ByteTransferFlag ? 0x80 : 0); // 4030.7 = Byte Transfer Flag
                            return t;
                        }
                        break;
                    case 1:
                        {
                            // Disk Data Input ($4031)
                            return Cart.FDS.ShiftRegisterLatch;
                        }
                        break;
                    case 2:
                        {
                            // Disk Drive Status ($4032)
                            byte t = 0;
                            if (Cart.FDS.CurrentState == DiskDrive.RamAdapterState.INSERTING)
                            {
                                t |= 1;
                            }
                            if (!(((FDS_4025_Control & 2) == 0) && (Cart.FDS.CurrentState == DiskDrive.RamAdapterState.RUNNING || Cart.FDS.CurrentState == DiskDrive.RamAdapterState.IDLE)))
                            {
                                t |= 2;
                            }
                            return t;
                        }
                        break;
                    case 3:
                        {
                            // External Connector Input ($4033)
                            return 0x80; // The battery is good.
                        }
                        break;
                }
            }
            return Cart.Emu.dataBus;
        }

        public override void Connector_CheckCIRAM()
        {
            if (TiltingCart)
            {
                if (!Cart.Emu.ConnectorPinFloating[56]) { Cart.Emu.SeventyTwoPinConnector[56] = Cart.Emu.SeventyTwoPinConnector[57]; }
                if (((FDS_4025_Control >> 3) & 1) == 1) //horizontal
                {
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = Cart.Emu.SeventyTwoPinConnector[61]; }
                }
                else //vertical
                {
                    if (!Cart.Emu.ConnectorPinFloating[21]) { Cart.Emu.SeventyTwoPinConnector[21] = Cart.Emu.SeventyTwoPinConnector[62]; }
                }
            }
            else
            {
                Cart.Emu.SeventyTwoPinConnector[56] = (Cart.Emu.PPU_AddressBus & 0x2000) == 0;
                Cart.Emu.SeventyTwoPinConnector[21] = (((FDS_4025_Control >> 3) & 1) == 1) ? ((Cart.Emu.PPU_AddressBus & 0x800) != 0) : ((Cart.Emu.PPU_AddressBus & 0x400) != 0);
            }
        }
        public override byte SnoopPPU(ushort Address) // For debug purposes. It's a bit clunky having to set this up for every mapper with a non-NROM CIRAM setup.
        {
            if (Address < 0x2000)
            {
                int CHR_Address = Cart.MapperChip.FetchPatternAddress(Address);
                return Cart.CHRROM[CHR_Address];
            }
            else
            {
                ushort Addr = (ushort)(Address & 0x3FF);
                Addr |= (ushort)(((((FDS_4025_Control >> 3) & 1) == 1) ? ((Address & 0x800) != 0) : ((Address & 0x400) != 0)) ? 0x400 : 0);
                return Cart.Emu.VRAM[Addr];
            }
        }

        public override void FDS_ByteTransferFlag()
        {
            if((FDS_4025_Control & 0x80) != 0)
            {
                Connector_IRQPin(true); // Run an IRQ!
            }
        }
        public override byte FDS_Get4025()
        {
            return FDS_4025_Control;
        }

        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            if (Cart.UsingCHRRAM)
            {
                foreach (Byte b in Cart.CHRROM) { State.Add(b); }
            }
            State.Add(FDS_4025_Control);
            State.Add((byte)Cart.FDS.clock);
            State.Add((byte)(Cart.FDS.clock >> 8));
            State.Add((byte)(Cart.FDS.clock >> 16));
            State.Add((byte)(Cart.FDS.clock >> 24));
            State.Add((byte)Cart.FDS.CurrentState);
            State.Add(Cart.FDS.ShiftRegister);
            State.Add(Cart.FDS.ShiftRegisterLatch);            
            State.Add((byte)Cart.FDS.DiskAddress);
            State.Add((byte)(Cart.FDS.DiskAddress >> 8));
            State.Add((byte)(Cart.FDS.DiskAddress >> 16));
            State.Add((byte)(Cart.FDS.DiskAddress >> 24));
            State.Add(Cart.FDS.DiskAddressFine);
            State.Add((byte)(Cart.FDS.Status_ByteTransferFlag ? 1 : 0));
            State.Add((byte)(Cart.FDS.lookingForEndOfGap ? 1 : 0));

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
            FDS_4025_Control = State[p++];
            Cart.FDS.clock = State[p++];
            Cart.FDS.clock |= (ushort)(State[p++] << 8);
            Cart.FDS.clock |= (ushort)(State[p++] << 16);
            Cart.FDS.clock |= (ushort)(State[p++] << 24);
            Cart.FDS.CurrentState = (DiskDrive.RamAdapterState)State[p++];
            Cart.FDS.ShiftRegister = State[p++];
            Cart.FDS.ShiftRegisterLatch = State[p++];
            Cart.FDS.DiskAddress = State[p++];
            Cart.FDS.DiskAddress |= (ushort)(State[p++] << 8);
            Cart.FDS.DiskAddress |= (ushort)(State[p++] << 16);
            Cart.FDS.DiskAddress |= (ushort)(State[p++] << 24);
            Cart.FDS.DiskAddressFine = State[p++];
            Cart.FDS.Status_ByteTransferFlag = State[p++] == 1;
            Cart.FDS.lookingForEndOfGap = State[p++] == 1;

            exitIndex = p;
        }

    }
}
