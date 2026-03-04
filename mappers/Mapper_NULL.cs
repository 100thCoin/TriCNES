using System;
using System.Collections.Generic;

namespace TriCNES.mappers
{
    public class Mapper_NULL : Mapper
    {
        // There is not a cartridge inserted in the console.

        public override void FetchPRG(ushort Address, bool Observe)
        {
            dataPinsAreNotFloating = false;
            // the data pins are always floating. There's no cartridge inserted!
            return;
        }

        public override byte FetchCHR(ushort Address, bool Observe)
        {
            // there's no cartridge. TODO: Look into this. Supposedly this would likely be the lower 8 bits of the address bus, but CIRAM enable is also floating.
            return 0;
        }
        public override ushort MirrorNametable(ushort Address)
        {
            return Address;
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
    }
}
