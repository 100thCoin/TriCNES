using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_NULL : Mapper
    {
        // There is not a cartridge inserted in the console.

        public override void FetchCPU()
        {
            // the data pins are always floating. There's no cartridge inserted!
            return;
        }

        public override byte SnoopCPU(ushort Address) // For debug purposes. It's a bit clunky.
        {
            return Cart.Emu.dataBus; // There is nothing inserted here. The CPU *must* see open bus.
        }

        public override void AccessPPU()
        {
            // the data pins are always floating. There's no cartridge inserted!
            return;
        }

        public override byte SnoopPPU(ushort Address) // For debug purposes. It's a bit clunky having to set this up for every mapper with a non-NROM CIRAM setup.
        {
            return (byte)Cart.Emu.addressBus; // There is nothing inserted here. The PPU *must* see open bus.
        }

        public override int FetchPatternAddress(ushort Address)
        {
            // there's no cartridge. TODO: Look into this. Supposedly this would likely be the lower 8 bits of the address bus, but CIRAM enable is also floating.
            return 0;
        }
        public override void Connector_CheckCIRAM()
        {
            // CIRAM is left floating. There's no cartridge inserted!
        }
        public override List<byte> SaveMapperRegisters()
        {
            List<byte> State = new List<byte>();
            return State;
        }
        public override void LoadMapperRegisters(List<byte> State, int startIndex, out int exitIndex)
        {
            int p = startIndex;
            exitIndex = p;
        }
        public override bool CheckCIC()
        {
            return false; // There's no CIC chip!
        }
    }
}
