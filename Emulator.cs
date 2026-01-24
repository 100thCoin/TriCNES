using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace TriCNES
{
    // Coin's Contrabulous Cartswapulator!
    public class Cartridge
    {
        // Since I made this emulator with mid-instruction cartridge swapping in mind, the cartridge class holds information about the cartridge that would persist when swapped in and out.

        public string Name;         // For debugging
        public byte[] ROM;          // The entire .nes file

        public byte[] PRGROM;       // The entire program rom portion of the .nes file
        public byte[] CHRROM;       // The entire character rom portion of the .nes file

        public byte MemoryMapper;   // Header info: what mapper chip is this cartridge using?
        public byte PRG_Size;       // Header info: how many kb of PRG data does this cartridge have?
        public byte CHR_Size;       // Header info: how many kb of CHR data does this cartridge have?
        public byte PRG_SizeMinus1; // PRG_Size-1; This is frequently used when grabbing data from PRG banks

        public byte[] CHRRAM;       // If this cartridge has character RAM, this array is used.
        public bool UsingCHRRAM;    // Header info: CHR RAM doesn't exist on all cartridges.

        public byte[] PRGRAM;         // PRG RAM / Battery backed save RAM.

        public Cartridge(String filepath) // Constructor from file path
        {
            ROM = File.ReadAllBytes(filepath); // Reads the file from the provided file path, and stores every byte into an array.

            // The iNES header isn't actually part of the physical cartridge.
            // Rather, the values of the iNES header are manually added to provide extra information to emulators.
            // Info such as "what mapper chip", "how many CHR banks?" and even "how should we mirror the nametables?" are part of this header.

            MemoryMapper = (byte)(ROM[7] & 0xF0);   // Parsing the iNES header to determine what mapper chip this cartridge uses.
            MemoryMapper |= (byte)(ROM[6] >> 4);    // The upper nybble of byte 6, bitwise OR with the upper nybble of byte 7.

            PRG_Size = ROM[4];  // Parsing the iNES header to determine how many kb of PRG data exists on this cartridge.
            CHR_Size = ROM[5];  // Parsing the iNES header to determine how many kb of CHR data exists on this cartridge.

            PRG_SizeMinus1 = (byte)(PRG_Size - 1); // This value is occasionally used whenever a mapper has a fixed bank from the end of the PRG data, like address $E000 in the MMC3 chip.

            UsingCHRRAM = CHR_Size == 0; // If CHR_Size == 0, this is using CHR RAM


            PRGROM = new byte[PRG_Size * 0x4000]; // 0x4000 bytes of PRG ROM, multiplied by byte 4 of the iNES header.
            CHRROM = new byte[CHR_Size * 0x2000]; // 0x2000 bytes of CHR ROM, multiplied by byte 5 of the iNES header.
            CHRRAM = new byte[0x2000];            // CHR RAM always has 2 kibibytes

            NametableHorizontalMirroring = ((ROM[6] & 1) == 0); // The style in which the nametable is mirrored is part of the iNES header.

            Array.Copy(ROM, 0x10, PRGROM, 0, PRGROM.Length); // This sets up the PRG ROM array with the values from the .nes file
            Array.Copy(ROM, 0x10 + PRGROM.Length, CHRROM, 0, CHRROM.Length); // This sets up the CHR ROM array with the values from the .nes file

            // at this point, the ROM byte array is no longer needed, so null it to free up its memory.
            ROM = null;

            PRGRAM = new byte[0x2000]; // PRG RAM probably has different lengths depending on the mapper, but this emulator doesn't yet support any mappers in which that length isn't 2 kibibytes.

            Name = filepath; // For debugging, it's nice to see the file name sometimes.
        }

        public bool NametableHorizontalMirroring;


        // Mapper stuff

        // I should probably refactor this.
        // Since each cart can only have 1 mapper, there's no need for every mapper's variables to coexist.


        // Mapper 0, NROM doesn't have any registers.

        // Mapper 1, MMC1
        public byte Mapper_1_ShiftRegister;
        public byte Mapper_1_Control = 0x0C;    //0x8000
        public byte Mapper_1_CHR0;              //0xA000
        public byte Mapper_1_CHR1;              //0xC000
        public byte Mapper_1_PRG;               //0xE000
        public bool Mapper_1_PB;

        // Mapper 2, UxROM
        public byte Mapper_2_BankSelect; // any write to ROM

        // Mapper 3, CNROM
        public byte Mapper_3_CHRBank; // any write to ROM

        // Mapper 4, MMC3
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

        // Mapper 7, AOROM
        public byte Mapper_7_BankSelect;

        // Mapper 9, MMC2
        public byte Mapper_9_BankSelect;
        public byte Mapper_9_CHR0_FD;
        public byte Mapper_9_CHR0_FE;
        public byte Mapper_9_CHR1_FD;
        public byte Mapper_9_CHR1_FE;
        public bool Mapper_9_NametableMirroring;
        public bool Mapper_9_Latch0_FE;
        public bool Mapper_9_Latch1_FE;

        // Mapper 69, Sunsoft FME-7
        public byte Mapper_69_CMD;
        public byte Mapper_69_CHR_1K0;
        public byte Mapper_69_CHR_1K1;
        public byte Mapper_69_CHR_1K2;
        public byte Mapper_69_CHR_1K3;
        public byte Mapper_69_CHR_1K4;
        public byte Mapper_69_CHR_1K5;
        public byte Mapper_69_CHR_1K6;
        public byte Mapper_69_CHR_1K7;
        public byte Mapper_69_Bank_6;
        public bool Mapper_69_Bank_6_isRAM;
        public bool Mapper_69_Bank_6_isRAMEnabled;
        public byte Mapper_69_Bank_8;
        public byte Mapper_69_Bank_A;
        public byte Mapper_69_Bank_C;
        public byte Mapper_69_NametableMirroring; // 0 = Vertical              1 = Horizontal            2 = One Screen Mirroring from $2000 ("1ScA")            3 = One Screen Mirroring from $2400 ("1ScB")
        public bool Mapper_69_EnableIRQ;
        public bool Mapper_69_EnableIRQCounterDecrement;
        public ushort Mapper_69_IRQCounter; // When enabled the 16-bit IRQ counter is decremented once per CPU cycle. When the IRQ counter is decremented from $0000 to $FFFF an IRQ is generated.



    }

    public class Emulator
    {

        public Cartridge Cart;  // The idea behind this emulator is that this value could be changed at any time if you so desire.
        public byte PPUClock;    // Counts down from 4. When it's 0, a PPU cycle occurs.
        public byte CPUClock;    // Counts down from 12. When it's 0, a CPU cycle occurs.
        public byte APUClock;    // Counts down from 12. Technically an APU cycle is 24 master clock cycles, but certain actions happen when this clock goes low and when it goes high.
        public byte MasterClock; // Counts up every master clock cycle. Resets at 24.

        public bool APU_PutCycle = false; // The APU needs to know if this is a "get" or "put" cycle.

        public byte[] OAM = new byte[0x100];         // Object Attribute Memory is 256 bytes.
        public byte[] OAM2 = new byte[32];   // Secondary OAM is specifically the 8 objects being rendered on the current scanline.
        public byte SecondaryOAMSize = 0;            // This is a count of how many objects are currently in secondary OAM.
        public byte OAM2Address = 0;         // During sprite evaluation, the current SecondaryOAM Address is used to track what byte is set of a given dot.
        public bool SecondaryOAMFull = false;        // If full and another object exists in the same scanline, the PPU Sprite OVerflow flag is set.
        public byte SpriteEvaluationTick = 0;        // During sprite evaluation, there's a switch statement that determines what to do on a given dot. This determines which action to take.
        public bool OAMAddressOverflowedDuringSpriteEvaluation = false; // If the OAM address overflows during sprite evaluation, there's a few bugs that can occur.

        public byte[] RAM = new byte[0x800];    // There are 0x800 bytes of RAM
        public byte[] VRAM = new byte[0x800];   // There are 0x800 bytes of VRAM
        public byte[] PaletteRAM = new byte[0x20]; // there are 0x20 bytes of palette RAM

        public ushort programCounter = 0;   // The PC. What address is currently being executed?
        public byte opCode = 0; // The first CPU cycle of an instruction will read the opcode. This determines how the rest of the cycles will behave.

        public int totalCycles; // For debugging. This is just a count of how many CPU cycles have occurred since the console booted up.

        public byte stackPointer = 0x00; // The Stack pointer is used during pushing/popping values with the stack. This determines which address will be read or written to.

        public bool flag_Carry;      // The Carry flag is used in BCC and BCS instructions, and is set when the result of an operation over/underflows.
        public bool flag_Zero;       // The Zero flag is used in BNE and BEQ instructions, and is set when the result of an operation is zero.
        public bool flag_Interrupt;  // The Interrupt suppression flag will suppress IRQ's. 
        public bool flag_Decimal;    // The NES doesn't use this flag.
        public bool flag_Overflow;   // The Carry flag is used in BVC and BVS instructions, and is set when the result of an operation over/underflows and the sign of the result is the same as the value before the operation.
        public bool flag_Negative;   // The Zero flag is used in BPL and BMI instructions, and is set when the result of an operation is negative. (bit 7 is set)
        byte status = 0;             // This is a byte representation of all the flags.
        public byte A = 0;           // The Accumulator, or "A Register"
        public byte X = 0;           // The X Register
        public byte Y = 0;           // The Y Register
        public byte H = 0;           // The High byte of the target address. A couple unofficial instructions use this value.
        public bool IgnoreH;         // However, with a well-timed DMA, the H register isn't actually part of the equation on some of those.
        public byte dataBus = 0;     // The Data Bus.
        public ushort addressBus = 0;// The Address Bus. "Where are we reading/writing"
        public byte specialBus = 0;  // The Special Bus is used in certain instructions. //TODO: What's the actual use for this bus??
        public byte dl = 0;          // Data Latch. This holds values between CPU cycles that are used in later cycles within an instruction.


        public byte operationCycle = 0; // This tracks what cycle of a given instruction is being emulated. Cycle 0 fetches the opcode, and all cycles after that have specific logic depending on which cycle needs emulated next.
        public bool operationComplete = false; // When an instruction is complete, I use this to reset operationCycle.

        public ushort temporaryAddress; // I use this to temporarily modify the value of the address bus for some if statements. This is mostly for checking if the low byte under/over flows.


        public static uint[] NesPalInts = {
            // each uint represents the ARGB components of a color.
            // there's 64 colors, but this is also how I implement specific values for the PPU's emphasis bits.
            // default palette:
            0xFF656565, 0xFF002A84, 0xFF1513A2, 0xFF3A019E, 0xFF59007A, 0xFF6A003E, 0xFF680800, 0xFF531D00, 0xFF323400, 0xFF0D4600, 0xFF004F00, 0xFF004C09, 0xFF003F4B, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFAEAEAE, 0xFF175FD6, 0xFF4341FF, 0xFF7529FA, 0xFF9E1DCA, 0xFFB4207B, 0xFFB13322, 0xFF964E00, 0xFF6A6C00, 0xFF398400, 0xFF0F9000, 0xFF008D33, 0xFF007B8C, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFFEFFFF, 0xFF66AFFF, 0xFF9390FF, 0xFFC578FF, 0xFFEE6CFF, 0xFFFF6FCA, 0xFFFF8271, 0xFFE69E25, 0xFFBABC00, 0xFF88D501, 0xFF5EE132, 0xFF47DD82, 0xFF4ACBDC, 0xFF4E4E4E, 0xFF000000, 0xFF000000,
            0xFFFEFFFF, 0xFFC0DEFF, 0xFFD2D1FF, 0xFFE7C7FF, 0xFFF8C2FF, 0xFFFFC3E9, 0xFFFFCBC4, 0xFFF5D7A5, 0xFFE2E394, 0xFFCEED96, 0xFFBCF2AA, 0xFFB3F1CB, 0xFFB4E9F0, 0xFFB6B6B6, 0xFF000000, 0xFF000000,
            // emphasize red:
            0xFF66423E, 0xFF000D58, 0xFF150075, 0xFF380075, 0xFF560058, 0xFF670027, 0xFF680000, 0xFF530D00, 0xFF341E00, 0xFF102B00, 0xFF003000, 0xFF002B00, 0xFF001C24, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFAF7E78, 0xFF19379A, 0xFF4320C1, 0xFF720FC1, 0xFF9A089A, 0xFFB10F59, 0xFFB2220F, 0xFF963700, 0xFF6C4D00, 0xFF3D5F00, 0xFF166500, 0xFF005F0C, 0xFF004B55, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFFFC0B8, 0xFF6878DB, 0xFF9361FF, 0xFFC24FFF, 0xFFEA49DB, 0xFFFF4F99, 0xFFFF634E, 0xFFE77808, 0xFFBC8F00, 0xFF8DA000, 0xFF65A708, 0xFF4DA04A, 0xFF4C8D95, 0xFF4F2F2B, 0xFF000000, 0xFF000000,
            0xFFFFC0B8, 0xFFC1A2C6, 0xFFD399D6, 0xFFE792D6, 0xFFF78FC6, 0xFFFF92AB, 0xFFFF9A8C, 0xFFF6A26F, 0xFFE4AC5F, 0xFFD1B35F, 0xFFC0B66F, 0xFFB7B38B, 0xFFB6ABA9, 0xFFB7857E, 0xFF000000, 0xFF000000,
            // emphasize green:
            0xFF395D2C, 0xFF002452, 0xFF000D6A, 0xFF140064, 0xFF2D0041, 0xFF3E0010, 0xFF3F0300, 0xFF301800, 0xFF162F00, 0xFF004200, 0xFF004C00, 0xFF004700, 0xFF003924, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFF71A360, 0xFF005691, 0xFF1939B1, 0xFF4020A9, 0xFF61127B, 0xFF78183A, 0xFF792C00, 0xFF654800, 0xFF426600, 0xFF1B7E00, 0xFF008D00, 0xFF00860A, 0xFF007254, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFAEF099, 0xFF32A3CB, 0xFF5684EB, 0xFF7E6BE3, 0xFF9E5DB5, 0xFFB66472, 0xFFB77728, 0xFFA39400, 0xFF7FB200, 0xFF57CB00, 0xFF37D900, 0xFF1FD342, 0xFF1EBF8D, 0xFF27471C, 0xFF000000, 0xFF000000,
            0xFFAEF099, 0xFF7BD0AD, 0xFF8AC3BA, 0xFF9AB9B7, 0xFFA8B3A4, 0xFFB1B689, 0xFFB2BE6A, 0xFFAACA50, 0xFF9BD643, 0xFF8BE146, 0xFF7DE65A, 0xFF74E475, 0xFF73DC94, 0xFF77AA65, 0xFF000000, 0xFF000000,
            // emphasize red + green:
            0xFF3F3F25, 0xFF000B46, 0xFF00005D, 0xFF18005A, 0xFF2F003F, 0xFF40000E, 0xFF410000, 0xFF320A00, 0xFF191A00, 0xFF002800, 0xFF002F00, 0xFF002A00, 0xFF001B1C, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFF797A55, 0xFF003581, 0xFF201F9F, 0xFF450D9C, 0xFF640478, 0xFF7B0A36, 0xFF7C1E00, 0xFF683200, 0xFF474900, 0xFF225B00, 0xFF036400, 0xFF005D00, 0xFF004A4A, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFBABB8B, 0xFF3E75B7, 0xFF605ED6, 0xFF854CD2, 0xFFA443AE, 0xFFBB4A6C, 0xFFBD5D21, 0xFFA87200, 0xFF878900, 0xFF619B00, 0xFF42A400, 0xFF2B9D34, 0xFF2A8A7F, 0xFF2C2D15, 0xFF000000, 0xFF000000,
            0xFFBABB8B, 0xFF879E9D, 0xFF9595AA, 0xFFA48DA8, 0xFFB18999, 0xFFBB8C7E, 0xFFBB945F, 0xFFB39D48, 0xFFA5A63B, 0xFF96AE3D, 0xFF89B14C, 0xFF7FAF67, 0xFF7FA686, 0xFF80805A, 0xFF000000, 0xFF000000,
            // emphasize blue:
            0xFF47477C, 0xFF001A8C, 0xFF0B0AA9, 0xFF2900A3, 0xFF410081, 0xFF4D004A, 0xFF49000D, 0xFF340400, 0xFF141500, 0xFF002800, 0xFF003300, 0xFF00331B, 0xFF002A58, 0xFF000000, 0xFF00000A, 0xFF00000A,
            0xFF8584CD, 0xFF0B49E2, 0xFF3533FF, 0xFF5D1AFF, 0xFF7D0CD4, 0xFF8D0B8B, 0xFF86173A, 0xFF6B2C00, 0xFF414200, 0xFF195B00, 0xFF006904, 0xFF006A4C, 0xFF005E9E, 0xFF00000A, 0xFF00000A, 0xFF00000A,
            0xFFC9C8FF, 0xFF4E8CFF, 0xFF7876FF, 0xFFA05CFF, 0xFFC14EFF, 0xFFD14DE4, 0xFFCB5A92, 0xFFAF6E4C, 0xFF848525, 0xFF5C9E2D, 0xFF3BAD5B, 0xFF2BADA5, 0xFF32A1F7, 0xFF343362, 0xFF00000A, 0xFF00000A,
            0xFFC9C8FF, 0xFF96AFFF, 0xFFA8A6FF, 0xFFB89BFF, 0xFFC696FF, 0xFFCC95FF, 0xFFCA9AEA, 0xFFBEA3CD, 0xFFACACBD, 0xFF9CB7C0, 0xFF8FBDD3, 0xFF88BDF2, 0xFF8BB8FF, 0xFF8B8AD6, 0xFF00000A, 0xFF00000A,
            // emphasize red + blue:
            0xFF46344C, 0xFF00085C, 0xFF0B007A, 0xFF260077, 0xFF3D005C, 0xFF4A0030, 0xFF480000, 0xFF340000, 0xFF140F00, 0xFF001D00, 0xFF002400, 0xFF002200, 0xFF001829, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFF846B8C, 0xFF0A30A1, 0xFF3419C8, 0xFF5907C5, 0xFF7800A1, 0xFF880166, 0xFF860E23, 0xFF6B2300, 0xFF403900, 0xFF1C4C00, 0xFF005400, 0xFF00521A, 0xFF00445C, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFC7A7D2, 0xFF4C6BE8, 0xFF7754FF, 0xFF9C42FF, 0xFFBB39E7, 0xFFCC3CAB, 0xFFCA4968, 0xFFAE5E23, 0xFF837500, 0xFF5E8700, 0xFF3F9023, 0xFF2E8E5F, 0xFF3080A2, 0xFF332338, 0xFF000000, 0xFF000000,
            0xFFC7A7D2, 0xFF948EDB, 0xFFA685EB, 0xFFB57DEA, 0xFFC27ADB, 0xFFC97BC2, 0xFFC880A7, 0xFFBD898A, 0xFFAB927A, 0xFF9C9A7B, 0xFF8F9D8A, 0xFF889CA3, 0xFF8997BE, 0xFF8A7093, 0xFF000000, 0xFF000000,
            // emphasize green + blue:
            0xFF304144, 0xFF00155A, 0xFF000471, 0xFF11006B, 0xFF2A0049, 0xFF36001C, 0xFF350000, 0xFF250300, 0xFF0C1300, 0xFF002600, 0xFF003100, 0xFF002F00, 0xFF002531, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFF647D80, 0xFF00429E, 0xFF152CBC, 0xFF3C13B4, 0xFF5C0586, 0xFF6D074B, 0xFF6B1509, 0xFF572900, 0xFF364000, 0xFF0E5900, 0xFF006700, 0xFF006424, 0xFF005766, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFF9EBEC3, 0xFF2D83E1, 0xFF4E6CFF, 0xFF7653F8, 0xFF9745C9, 0xFFA7478D, 0xFFA5554A, 0xFF916A12, 0xFF6F8100, 0xFF479A00, 0xFF27A82A, 0xFF16A566, 0xFF1898A9, 0xFF1F2E30, 0xFF000000, 0xFF000000,
            0xFF9EBEC3, 0xFF6FA6CF, 0xFF7D9CDC, 0xFF8E92D8, 0xFF9B8CC5, 0xFFA28DAD, 0xFFA19391, 0xFF999C7A, 0xFF8BA56D, 0xFF7AAF70, 0xFF6DB584, 0xFF66B49C, 0xFF67AEB8, 0xFF6A8386, 0xFF000000, 0xFF000000,
            // emphasize red + green + blue:
            0xFF343434, 0xFF00084B, 0xFF000061, 0xFF14005F, 0xFF2B0044, 0xFF380017, 0xFF360000, 0xFF270000, 0xFF0E0F00, 0xFF001D00, 0xFF002400, 0xFF002200, 0xFF001721, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFF6A6A6A, 0xFF003088, 0xFF1B19A7, 0xFF4007A3, 0xFF5F007F, 0xFF6F0144, 0xFF6D0E02, 0xFF592300, 0xFF383900, 0xFF134B00, 0xFF005400, 0xFF00520F, 0xFF004451, 0xFF000000, 0xFF000000, 0xFF000000,
            0xFFA6A6A6, 0xFF356BC5, 0xFF5654E3, 0xFF7B42E0, 0xFF9B39BB, 0xFFAB3C80, 0xFFA9493D, 0xFF955E04, 0xFF737500, 0xFF4E8700, 0xFF2F900E, 0xFF1E8E4A, 0xFF20808D, 0xFF232323, 0xFF000000, 0xFF000000,
            0xFFA6A6A6, 0xFF788EB3, 0xFF8585C0, 0xFF957DBE, 0xFFA279AF, 0xFFA87A96, 0xFFA8807B, 0xFF9F8964, 0xFF919257, 0xFF829A59, 0xFF759D68, 0xFF6E9C80, 0xFF6F979C, 0xFF707070, 0xFF000000, 0xFF000000,
            // colorburst
            0xFF010900
        };

        int chosenColor; // During screen rendering, this value is the index into the color array.
        public DirectBitmap Screen = new DirectBitmap(256, 240); // This uses a class called "DirectBitmap". It's pretty much just the same as Bitmap, but I don't need to unlock/lock the bits, so it's faster.
        public DirectBitmap NTSCScreen = new DirectBitmap(256 * 8, 240); // This uses a class called "DirectBitmap". It's pretty much just the same as Bitmap, but I don't need to unlock/lock the bits, so it's faster.
        public DirectBitmap BorderedScreen = new DirectBitmap(341, 262); // This uses a class called "DirectBitmap". It's pretty much just the same as Bitmap, but I don't need to unlock/lock the bits, so it's faster.
        public DirectBitmap BorderedNTSCScreen = new DirectBitmap(341 * 8, 262); // This uses a class called "DirectBitmap". It's pretty much just the same as Bitmap, but I don't need to unlock/lock the bits, so it's faster.

        //Debugging
        public bool Logging;    // If set, the tracelogger will record all instructions ran.
        public StringBuilder DebugLog; // This is where the tracelogger is recording.

        public Emulator() // The instantiator for this class
        {
            RAM = new byte[0x800];
            A = 0;  // The A, X, and Y registers are all initialized with 0 when the console boots up.
            X = 0;
            Y = 0;
            VRAM = new byte[0x800];
            OAM = new byte[0x100];
            OAM2 = new byte[32];
            for(int oam2_init = 0; oam2_init < OAM2.Length; oam2_init++)
            {
                OAM2[oam2_init] = 0xFF;
            }

            // set up RAM and PPU RAM Pattern
            int i = 0;
            while (i < 0x800)
            {
                int j = i & 0x2;
                bool swap = (i & 0x1F) >= 0x10;
                if (j < 0x2 == !swap)
                {
                    VRAM[i] = 0xF0;
                    RAM[i] = 0xF0;
                }
                else
                {
                    VRAM[i] = 0x0F;
                    RAM[i] = 0x0F;
                }
                i++;
            }

            bool BlarggPalette = false; // There's a PPU test cartridge that expects a very specific palette when you power on the console.
            if (BlarggPalette)
            {
                //use the palette that Blargg's NES uses
                PaletteRAM[0x00] = 0x09;
                PaletteRAM[0x01] = 0x01;
                PaletteRAM[0x02] = 0x00;
                PaletteRAM[0x03] = 0x01;
                PaletteRAM[0x04] = 0x00;
                PaletteRAM[0x05] = 0x02;
                PaletteRAM[0x06] = 0x02;
                PaletteRAM[0x07] = 0x0D;
                PaletteRAM[0x08] = 0x08;
                PaletteRAM[0x09] = 0x10;
                PaletteRAM[0x0A] = 0x08;
                PaletteRAM[0x0B] = 0x24;
                PaletteRAM[0x0C] = 0x00;
                PaletteRAM[0x0D] = 0x00;
                PaletteRAM[0x0E] = 0x04;
                PaletteRAM[0x0F] = 0x2C;
                PaletteRAM[0x10] = 0x09;
                PaletteRAM[0x11] = 0x01;
                PaletteRAM[0x12] = 0x34;
                PaletteRAM[0x13] = 0x03;
                PaletteRAM[0x14] = 0x00;
                PaletteRAM[0x15] = 0x04;
                PaletteRAM[0x16] = 0x00;
                PaletteRAM[0x17] = 0x14;
                PaletteRAM[0x18] = 0x08;
                PaletteRAM[0x19] = 0x3A;
                PaletteRAM[0x1A] = 0x00;
                PaletteRAM[0x1B] = 0x02;
                PaletteRAM[0x1C] = 0x00;
                PaletteRAM[0x1D] = 0x20;
                PaletteRAM[0x1E] = 0x2C;
                PaletteRAM[0x1F] = 0x08;
            }
            else // Except my actual console has a different palette than Blargg, so I use this palette instead.
            {
                // use the palette that my NES uses
                PaletteRAM[0x00] = 0x00;
                PaletteRAM[0x01] = 0x00;
                PaletteRAM[0x02] = 0x28;
                PaletteRAM[0x03] = 0x00;
                PaletteRAM[0x04] = 0x00;
                PaletteRAM[0x05] = 0x08;
                PaletteRAM[0x06] = 0x00;
                PaletteRAM[0x07] = 0x00;
                PaletteRAM[0x08] = 0x00;
                PaletteRAM[0x09] = 0x01;
                PaletteRAM[0x0A] = 0x01;
                PaletteRAM[0x0B] = 0x20;
                PaletteRAM[0x0C] = 0x00;
                PaletteRAM[0x0D] = 0x08;
                PaletteRAM[0x0E] = 0x00;
                PaletteRAM[0x0F] = 0x02;
                PaletteRAM[0x10] = 0x00;
                PaletteRAM[0x11] = 0x00;
                PaletteRAM[0x12] = 0x00;
                PaletteRAM[0x13] = 0x00;
                PaletteRAM[0x14] = 0x00;
                PaletteRAM[0x15] = 0x02;
                PaletteRAM[0x16] = 0x21;
                PaletteRAM[0x17] = 0x00;
                PaletteRAM[0x18] = 0x00;
                PaletteRAM[0x19] = 0x00;
                PaletteRAM[0x1A] = 0x00;
                PaletteRAM[0x1B] = 0x00;
                PaletteRAM[0x1C] = 0x00;
                PaletteRAM[0x1D] = 0x10;
                PaletteRAM[0x1E] = 0x00;
                PaletteRAM[0x1F] = 0x00;
            }

            programCounter = 0xFFFF; // Technically, this value is nondeterministic. It also doesn't matter where it is, as it will be initialized in the RESET instruction.
            PPU_Scanline = 0;        // The PPU begins on dot 0 of scanline 0
            PPU_Dot = 7;       // Shouldn't this be 0? I don't know why, but this passes all the tests if this is 7, so...?

            PPU_OddFrame = true;    // And this is technically considered an "odd" frame when it comes to even/odd frame timing.

            APU_DMC_SampleAddress = 0xC000;
            APU_DMC_AddressCounter = 0xC000;

            APU_DMC_SampleLength = 1;
            APU_DMC_ShifterBitsRemaining = 8;
            APU_ChannelTimer_DMC = 1024; //APU_DMCRateLUT[0];
            DoReset = true; // This is used to force the first instruction at power on to be the RESET instruction.
            PPU_RESET = false; // I'm not even 100% certain my console has this behavior. I'll set it to false for now.


        }

        public bool PPU_RESET;

        // when pressing the reset button, this function runs
        public void Reset()
        {
            // The A, X, and Y registers are unchanged through reset.
            // most flags go unchanged as well, but the I flag is set to 1
            flag_Interrupt = true;
            // Triangle phase gets reset, though I'm not yet emulating audio.
            APU_DMC_Output &= 1;
            // All the bits of $4015 are cleared
            APU_Status_DMCInterrupt = false;
            APU_Status_FrameInterrupt = false;
            APU_Status_DelayedDMC = false;
            APU_Status_DMC = false;
            APU_Status_Noise = false;
            APU_Status_Triangle = false;
            APU_Status_Pulse2 = false;
            APU_Status_Pulse1 = false;
            APU_DMC_BytesRemaining = 0;
            APU_LengthCounter_Noise = 0;
            APU_LengthCounter_Triangle = 0;
            APU_LengthCounter_Pulse2 = 0;
            APU_LengthCounter_Pulse1 = 0;
            APU_Framecounter = 0; // reset the frame counter

            // PPU registers
            PPU_Update2000Delay = 0;
            PPU_Ctrl = 0; // this value is only used for debugging.
            PPUControl_NMIEnabled = false;
            PPUControlIncrementMode32 = false;
            PPU_Spritex16 = false;
            PPU_PatternSelect_Sprites = false;
            PPU_PatternSelect_Background = false;
            PPU_TempVRAMAddress = 0;

            PPU_Update2001Delay = 0;
            PPU_Mask_Greyscale = false;
            PPU_Mask_EmphasizeRed = false;
            PPU_Mask_EmphasizeGreen = false;
            PPU_Mask_EmphasizeBlue = false;
            PPU_Mask_8PxShowBackground = false;
            PPU_Mask_8PxShowSprites = false;
            PPU_Mask_ShowBackground = false;
            PPU_Mask_ShowSprites = false;

            PPU_Update2005Delay = 0;
            PPU_FineXScroll = 0;

            //$2006 is unchanged

            PPU_Data_StateMachine = 9;
            PPU_VRAMAddressBuffer = 0;
            PPU_OddFrame = false;

            PPU_Dot = 0;
            PPU_Scanline = 0;

            DoDMCDMA = false;
            DoOAMDMA = false;
            operationCycle = 0;
            operationComplete = false;
            DoReset = true;
            PPU_RESET = false; // I'm not even 100% certain my console has this behavior. I'll set it to false for now.
            // in theory, the CPU/PPU clock would be given random values. Let's just assume no changes.
        }

        public bool CPU_Read; // DMC DMA Has some specific behavior depending on if the CPU is currently reading or writing. DMA Halting fails / DMA $2007 bug.


        // The BRK instruction is reused in the IRQ, NMI, and RESET logic. These bools are used both to start the instruction, and also to make sure the correct logic is used.
        public bool DoBRK; // Set if the opcode is 00
        public bool DoNMI; // Set if a Non Maskable Interrupt is occurring
        public bool DoIRQ; // Set if an Interrupt Request is occurring

        public bool DoReset;  // Set when resetting the console, or power on.
        public bool DoOAMDMA; // If set, the Object Acctribute Memory's Direct Memory Access will occur.
        public bool FirstCycleOfOAMDMA; // The first cycle caa behave differently.
        public bool DoDMCDMA; // If set, the Delta Modulation Channel's Direct Memory Access will occur.
        public byte DMCDMADelay; // There's actually a slight delay between the audio chip preparing the DMA, and the CPU actually running it.
        public byte CannotRunDMCDMARightNow = 0;

        public byte DMAPage;    // When running an OAM DMA, this is used to determine which "page" to read bytes from. Typically, this is page 2 (address $200 through $2FF)
        public byte DMAAddress; // While this DMA runs, this value is incremented until it overflows.

        public bool FrameAdvance_ReachedVBlank; // For debugging. If frame advancing, this is set when VBlank occurs.

        public bool APU_ControllerPortsStrobing; // Set to true/false depending on the value written to $4016. When true, the buttons pressed are recorded in the shift registers.
        public bool APU_ControllerPortsStrobed;  // This bool prevents strobing from rushing through the TAS input log.
                                                 // This gets set to false if the controllers are unstrobed, or if the controller ports are read.

        public byte ControllerPort1;            // The buttons currently pressed on controller 1. These are in the "A, B, Select, Start, Up, Down, Left, Right" order.
        public byte ControllerPort2;            // The buttons currently pressed on controller 2. These are in the "A, B, Select, Start, Up, Down, Left, Right" order.
        public byte ControllerShiftRegister1;   // Controllers are read 1 bit at a time. First the A Button is read, then B, and so on.
        public byte ControllerShiftRegister2;   // Whenever the shift register is read, all the bits are shifted to the left, and a '1' replaces bit 0.
        public byte Controller1ShiftCounter;    // Subsequent CPU cycles reading from $2006 do not update the shift register.
        public byte Controller2ShiftCounter;    // Subsequent CPU cycles reading from $2007 do not update the shift register.



        // The PPU state machine:
        // In summary, the steps that are taken when writing to 2007 do not happen in a single ppu cycle.
        public byte PPU_Data_StateMachine = 0x7;                   // The value of the state machine indicates what step should be taken on any given ppu cycle.
        public bool PPU_Data_StateMachine_Read;                     // If this is a read instruction, the state machine behaves differently
        public bool PPU_Data_StateMachine_Read_Delayed;             // If the read cycle happens immediately before a write cycle, there's also different behavior.
        public bool PPU_Data_StateMachine_PerformMysteryWrite;      // This is only set during a read-modify-write instruction to $2007, if the current CPU/PPU alignment would result in "the mystery write" occurring.
        public byte PPU_Data_StateMachine_InputValue;               // This is the value that was written to $2007 while interrupting the state machine.
        public bool PPU_Data_StateMachine_UpdateVRAMAddressEarly;   // During read-modify-write instructions to $2007, certain CPU/PPU alignments will update the VRAM address earlier than expected.
        public bool PPU_Data_StateMachine_UpdateVRAMBufferLate;     // During read-modify-write instructions to $2007, certain CPU/PPU alignments will update the VRAM buffer later than expected.
        public bool PPU_Data_StateMachine_NormalWriteBehavior;      // If this write instruction is not interrupting the state machine.
        public bool PPU_Data_StateMachine_InterruptedReadToWrite;   // If a write happens on cycle 3 of the state machine.

        public byte MMC3_M2Filter;  // The MMC3 chip only clocks the IRQ timer if A12 has been low for at *least* 3 falling edges of M2.
        public bool ResetM2Filter;  // Due to how I implemented the M2 filter, I need to reset it to zero at a specific moment, or else I can miss an IRQ clock.

        public bool LagFrame; // True if the controller port was not strobed in a frame.
        public bool TASTimelineClockFiltering; // Primarily used in the TASTimeline if you are using subframe Inputs.

        public void _CoreFrameAdvance()
        {
            // If we're running this emulator 1 frame at a time, this waits until VBlank and then returns.
            FrameAdvance_ReachedVBlank = false;
            LagFrame = true; // Many emulators detect "lag frames" by checking if the controller ports were strobed during this frame.
            while (!FrameAdvance_ReachedVBlank)
            {
                _EmulatorCore();
            }
        }

        public int CycleCountForCycleTAS = 0; // If we're running a intercycle cart swapping TAS, we need to keep track of which cycle we're on.
        public void _CoreCycleAdvance()
        {
            // this runs 12 master clock cycles, or 1 CPU cycle.
            int i = 0;
            while (i < 12)
            {
                _EmulatorCore();
                i++;
            }
            CycleCountForCycleTAS++;
        }

        void _EmulatorCore()
        {
            // master clock
            MasterClock++;
            if (MasterClock == 24)
            {
                MasterClock = 0;
            }
            // counters count down to 0, run the appropriate chip's logic, and the counter is reset.
            // If multiple counters read 0 at the same time, there's an order of events.
            // The order of events:
            // CPU
            // PPU
            // APU

            if (CPUClock == 0)
            {

                _6502(); // This is where I run the CPU
                totalCycles++;         // for debugging mostly
                if (operationComplete) // If this instruction is complete
                {
                    operationComplete = false;
                    operationCycle = 0;
                    addressBus = programCounter;
                    CPU_Read = true;
                    IgnoreH = false;
                }

                _EmulateMappers(); // currently just used to clock the sunsoft FME-7 IRQ counter.
                CPUClock = 12; // there is 1 CPU cycle for every 12 master clock cycles
            }
            if (CPUClock == 8)
            {
                NMILine |= PPUControl_NMIEnabled && PPUStatus_VBlank;
                if (operationCycle == 0 && !(PPUStatus_VBlank && PPUControl_NMIEnabled))
                {
                    NMILine = false;
                }
            }
            if (PPUClock == 0)
            {
                _EmulatePPU();
                if (PPUBus != 0)
                {
                    DecayPPUDataBus();
                }
                PPUClock = 4; // there is 1 PPU cycle for every 12 master clock cycles
            }
            if (PPUClock == 2)
            {
                _EmulateHalfPPU();
            }
            if (CPUClock == 5)
            {
                IRQLine = IRQ_LevelDetector;
                if (APU_Status_FrameInterrupt && !APU_FrameCounterInhibitIRQ)
                {
                    IRQ_LevelDetector = true; // if the APU frame counter flag is never cleared, you will get another IRQ when the I flag is cleared.
                }
                if ((PPU_AddressBus & 0b0001000000000000) == 0)
                {
                    if (MMC3_M2Filter < 3)
                    {
                        MMC3_M2Filter++;
                    }
                }
                else
                {
                    ResetM2Filter = true; // the filter gets reset in the function that clocks the MMC3 IRQ
                }
            }

            if (APUClock == 0)
            {
                APU_PutCycle = !APU_PutCycle;

                _EmulateAPU();

                APUClock = 12; //24
                // the APU is actually clocked every 24 master clock cycles.
                // yet there's a lot of timing that happens every cpu cycle anyway??
                // If the timing needs to be exactly n and a half APU cycles, then I'll just multiply the numbers by 2 and clock this twice as fast.
            }

            // Decrement the clocks.
            PPUClock--;
            CPUClock--;
            APUClock--;
        }

        void _EmulateMappers()
        {
            if (Cart.MemoryMapper == 69)
            {
                // The sunsoft FME-7 mapper chip has an IRQ counter that ticks down once per CPU cycle.
                if (Cart.Mapper_69_EnableIRQCounterDecrement)
                {
                    ushort temp = Cart.Mapper_69_IRQCounter;
                    Cart.Mapper_69_IRQCounter--;
                    if (Cart.Mapper_69_EnableIRQ && temp < Cart.Mapper_69_IRQCounter)
                    {
                        IRQ_LevelDetector = true;
                    }
                }
            }
        }

        // Audio Processing Unit Variables //

        // APU Status is at address $4015
        public bool APU_Status_DMCInterrupt;  // Bit 7 of $4015
        public bool APU_Status_FrameInterrupt;// Bit 6 of $4015
        public bool APU_Status_DMC;           // Bit 5 of $4015
        public bool APU_Status_DelayedDMC;    // Bit 5 of $4015, but with a slight delay.
        public bool APU_Status_Noise;         // Bit 3 of $4015
        public bool APU_Status_Triangle;      // Bit 2 of $4015
        public bool APU_Status_Pulse2;        // Bit 1 of $4015
        public bool APU_Status_Pulse1;        // Bit 0 of $4015

        public bool Clearing_APU_FrameInterrupt;


        public byte APU_DelayedDMC4015;         // When writing to $4015, there's a 3 or 4 cycle delay between the APU actually changing this value.
        public bool APU_ImplicitAbortDMC4015;   // An edge case of the DMC DMA, where regardless of the buffer being empty, there will be a 1-cycle DMA that gets aborted 2 cycles after the load DMA ends
        public bool APU_SetImplicitAbortDMC4015;// This is used to make that happen.

        public byte[] APU_Register = new byte[0x10]; // Instead of making a series of variables, I made an array here for some reason.

        public bool APU_FrameCounterMode;       // Bit 7 of $4017 : Determines if the APU frame counter is using the 4 step or 5 step modes.
        public bool APU_FrameCounterInhibitIRQ; // Bit 6 of $4017 : If set, prevents the APU from creating IRQ's

        public byte APU_FrameCounterReset = 0xFF; // When resetting the APU Frame counter by writing to address $4017, there's a 3 (or 4) CPU cycle delay. (3 if it's an even cpu cycle, 4 if odd.)
        public ushort APU_Framecounter = 0;       // Increments every APU cycle. Since there are events that happen at half-step intervals, I actually increment this every CPU cycle and multiplied all intervals by 2.
        public bool APU_QuarterFrameClock = false;// This is clocked approximately 4 times a frame, depending on the frame counter mode.
        public bool APU_HalfFrameClock = false;   // This is clocked approximately twice a frame, depending on the frame counter mode.

        public bool APU_Envelope_StartFlag = false;
        public bool APU_Envelope_DividerClock = false;
        public byte APU_Envelope_DecayLevel = 0;

        public byte APU_LengthCounter_Pulse1 = 0;   // The length counter for the APU's Pulse 1 channel.
        public byte APU_LengthCounter_Pulse2 = 0;   // The length counter for the APU's Pulse 2 channel.
        public byte APU_LengthCounter_Triangle = 0; // The length counter for the APU's Triangle channel.
        public byte APU_LengthCounter_Noise = 0;    // The length counter for the APU's Noise channel.

        // When a length counter's reloaded value is set by writing to $4003, $4007, $400B, or $400F, this LookUp Table is used to determine the length based on the value written.
        public static readonly byte[] APU_LengthCounterLUT = { 10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14, 12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30 };

        public bool APU_LengthCounter_HaltPulse1 = false;   // set if Bit 5 of $4000 is 1
        public bool APU_LengthCounter_HaltPulse2 = false;   // set if Bit 5 of $4004 is 1
        public bool APU_LengthCounter_HaltTriangle = false; // set if Bit 7 of $4008 is 1
        public bool APU_LengthCounter_HaltNoise = false;    // set if Bit 5 of $400C is 1

        public bool APU_LengthCounter_ReloadPulse1 = false;  // When writing to $4003 (if the pulse 1 channel is enabled) this is set to true. The value is reloaded in the next APU cycle.
        public bool APU_LengthCounter_ReloadPulse2 = false;  // When writing to $4007 (if the pulse 2 channel is enabled) this is set to true. The value is reloaded in the next APU cycle.
        public bool APU_LengthCounter_ReloadTriangle = false;// When writing to $400B (if the triangle channel is enabled) this is set to true. The value is reloaded in the next APU cycle.
        public bool APU_LengthCounter_ReloadNoise = false;   // When writing to $400F (if the noise channel is enabled) this is set to true. The value is reloaded in the next APU cycle.

        public byte APU_LengthCounter_ReloadValuePulse1 = 0;  // When the pulse 1 channel is reloaded, the length counter will be set to this value. Modified by writing to $4003.
        public byte APU_LengthCounter_ReloadValuePulse2 = 0;  // When the pulse 2 channel is reloaded, the length counter will be set to this value. Modified by writing to $4007.
        public byte APU_LengthCounter_ReloadValueTriangle = 0;// When the triangle channel is reloaded, the length counter will be set to this value. Modified by writing to $400B.
        public byte APU_LengthCounter_ReloadValueNoise = 0;   // When the noise channel is reloaded, the length counter will be set to this value. Modified by writing to $400F.

        public ushort APU_ChannelTimer_Pulse1 = 0;  // Decrements every "get" cycle.
        public ushort APU_ChannelTimer_Pulse2 = 0;  // Decrements every "get" cycle.
        public ushort APU_ChannelTimer_Triangle = 0;// Decrements every CPU cycle.
        public ushort APU_ChannelTimer_Noise = 0;   // Decrements every "get" cycle.
        public ushort APU_ChannelTimer_DMC = 0;     // Decrements every CPU cycle.


        // $4010
        public bool APU_DMC_EnableIRQ = false;  // Will the DMC create IRQ's? Set by writing to address $4010
        public bool APU_DMC_Loop = false;       // Will DPCM samples loop?
        public ushort APU_DMC_Rate = 428;       // The default sample rate is the slowest.
        // LookUp Table for how many CPU cycles are between each bit of the DPCM sample being played. (8 bits per byte, so to calculate how many cycles there are between each DMA, multiply these numbers by 8)
        public static readonly ushort[] APU_DMCRateLUT = { 428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106, 84, 72, 54 };

        // $4011 (and DPCM stuff)
        public byte APU_DMC_Output; // Directly writing here (Address $4011) will set the DMC output. This is how you play PCM audio.

        // $4012
        public ushort APU_DMC_SampleAddress = 0xC000;   // Where the DPCM sample is being read from.

        // $4013
        public ushort APU_DMC_SampleLength = 0;  // How many bytes are being played in this DPCM sample? (multiplied by 64, and add 1)

        public ushort APU_DMC_BytesRemaining = 0; // How many bytes are left in the sample. When a sample starts or loops, this is set to APU_DMC_SampleLength.
        public byte APU_DMC_Buffer = 0;  // The value that goes into the shift register.
        public ushort APU_DMC_AddressCounter = 0xC000; // What byte is fetched in the next DMA for DPCM audio? When a sample starts or loops, this is set to APU_DMC_SampleAddress.
        public byte APU_DMC_Shifter = 0; // The 8 bits of the sample that were fetched from the DMA.
        public byte APU_DMC_ShifterBitsRemaining = 8; // This tracks how many bits are left before needing to run another DMA
        public bool DPCM_Up;    // If the next bit of the DPCM sample is a 1, the output goes up. Otherwise it goes down.

        public bool APU_Silent = true;  // If the APU is not making any noise, this is set.

        void _EmulateAPU()
        {
            // This runs every 12 master clock cycles, though has different logic for even/odd CPU cycles.

            if (Controller1ShiftCounter > 0)
            {
                Controller1ShiftCounter--;
                if (Controller1ShiftCounter == 0)
                {
                    ControllerShiftRegister1 <<= 1;
                    ControllerShiftRegister1 |= 1;
                }
            }
            if (Controller2ShiftCounter > 0)
            {
                Controller2ShiftCounter--;
                if (Controller2ShiftCounter == 0)
                {
                    ControllerShiftRegister2 <<= 1;
                    ControllerShiftRegister2 |= 1;
                }
            }

            if (APU_PutCycle)
            {

                // controller reading is handled here in the APU chip.

                // If a 1 was written to $4016, we are strobing the controller.
                if (APU_ControllerPortsStrobing)
                {
                    if (!APU_ControllerPortsStrobed)
                    {
                        LagFrame = false;
                        APU_ControllerPortsStrobed = true;
                        if(TASTimelineClockFiltering)
                        {
                            FrameAdvance_ReachedVBlank = true; // Obviously this isn't actually VBlank, but we want to stop emulating here anyway.
                        }
                        // this will be reset to false if:
                        // 1.) the controllers are un-strobed. Ready for the next strobe.
                        // 2.) the controller ports are read, while still strobed. This allows data to be streamed in through the A button.

                        if (TAS_ReadingTAS) // This is specifically how I load inputs from a TAS, and has nothing to do with actual NES behavior.
                        {
                            if (TAS_InputSequenceIndex < TAS_InputLog.Length)
                            {
                                ControllerPort1 = (byte)(TAS_InputLog[TAS_InputSequenceIndex] & 0xFF);
                                ControllerPort2 = (byte)((TAS_InputLog[TAS_InputSequenceIndex] & 0xFF00) >> 8);
                            }
                            else // if the TAS has ended, only provide 0 as the inputs.
                            {
                                ControllerPort1 = 0;
                                ControllerPort2 = 0;
                            }
                            if (ClockFiltering)
                            {
                                TAS_InputSequenceIndex++; // Instead of using 1 input per frame, this just advances to the next input
                            }

                        }
                        // this sets up the shift registers with the value of the controller ports.
                        // If not set by the TAS, these are probably set outside this script in the script for the form.
                        ControllerShiftRegister1 = ControllerPort1;
                        ControllerShiftRegister2 = ControllerPort2;
                    }
                }
                else
                {
                    APU_ControllerPortsStrobed = false;
                }

                // clock timers
                APU_ChannelTimer_Pulse1--; // every APU GET cycle.
                APU_ChannelTimer_Pulse2--;
                APU_ChannelTimer_Noise--;


                //this happens whether a sample is playing or not
                APU_ChannelTimer_DMC--;
                APU_ChannelTimer_DMC--; // the table is in CPU cycles, but the count is in APU cycles
                if (APU_ChannelTimer_DMC == 0)
                {
                    APU_ChannelTimer_DMC = APU_DMC_Rate;
                    DPCM_Up = (APU_DMC_Shifter & 1) == 1;
                    if (DPCM_Up)
                    {
                        if (APU_DMC_Output <= 125) // this is 7 bit, and cannot go above 127
                        {
                            APU_DMC_Output += 2;
                        }
                    }
                    else
                    {
                        if (APU_DMC_Output >= 2) // this is 7 bit, and cannot go below 0
                        {
                            APU_DMC_Output -= 2;
                        }
                    }
                    APU_DMC_Shifter >>= 1; // shift the bits in the shift register
                    APU_DMC_ShifterBitsRemaining--; // and decrement the "bits remaining" counter.
                    if (APU_DMC_ShifterBitsRemaining == 0) // If there are no bits left,
                    {
                        APU_DMC_ShifterBitsRemaining = 8; // it's time for a DMC DMA!

                        if (APU_DMC_BytesRemaining > 0 || APU_SetImplicitAbortDMC4015)
                        {
                            if (!DoDMCDMA && CannotRunDMCDMARightNow != 2)
                            {
                                // if playing a sample:
                                DoDMCDMA = true;
                                DMCDMA_Halt = true;
                            }
                            if (APU_SetImplicitAbortDMC4015)
                            {
                                APU_ImplicitAbortDMC4015 = true; // check for weird DMA abort behavior
                                APU_SetImplicitAbortDMC4015 = false;
                            }
                            APU_DMC_Shifter = APU_DMC_Buffer; // and set up the shifter with the new values.
                            APU_Silent = false; // The APU is not silent.

                        }
                        else
                        {
                            APU_Silent = true;
                        }
                    }
                }
                if (CannotRunDMCDMARightNow > 0)
                {
                    CannotRunDMCDMARightNow -= 2;
                }
            }
            else
            {
                if (Clearing_APU_FrameInterrupt)
                {
                    Clearing_APU_FrameInterrupt = false;
                    APU_Status_FrameInterrupt = false;
                    IRQ_LevelDetector = false;
                }
                // DMC load from 4015
                if (DMCDMADelay > 0)
                {
                    DMCDMADelay--; // there's a small delay beetween the write occurring and the DMA beginning
                    if (DMCDMADelay == 0 && !DoDMCDMA) // if the DMA is already happening because of the timer
                    {
                        DoDMCDMA = true;
                        DMCDMA_Halt = true;
                        APU_DMC_Shifter = APU_DMC_Buffer;
                        APU_Silent = false;
                    }
                }
            }
            if (APU_DelayedDMC4015 > 0)
            {
                APU_DelayedDMC4015--;
                if (APU_DelayedDMC4015 == 0)
                {
                    APU_Status_DMC = APU_Status_DelayedDMC;
                    if (!APU_Status_DMC)
                    {
                        APU_DMC_BytesRemaining = 0;
                    }
                }
            }

            APU_ChannelTimer_Triangle--; // every CPU cycle.

            // clock sequencer
            if ((APU_FrameCounterReset & 0x80) == 0)
            {
                APU_FrameCounterReset--;
                if ((APU_FrameCounterReset & 0x80) != 0)
                {
                    APU_Framecounter = 0;
                }
            }

            APU_Framecounter++;

            // We're clocking the APU twice as fast in order to get the frame counter timing to allow the 'half APU cycle' timing.
            // these numbers are just multiplied by 2.

            if (APU_FrameCounterMode)
            {
                // 5 step
                switch (APU_Framecounter)
                {
                    default: break;
                    case 7457:
                        APU_QuarterFrameClock = true;
                        break;
                    case 14913:
                        APU_QuarterFrameClock = true;
                        APU_HalfFrameClock = true;
                        break;
                    case 22371:
                        APU_QuarterFrameClock = true;
                        break;
                    case 29829:
                        break;
                    case 37281:
                        APU_QuarterFrameClock = true;
                        APU_HalfFrameClock = true;
                        break;
                    case 37282:
                        APU_Framecounter = 0;
                        break;
                }
            }
            else
            {
                // 4 step
                switch (APU_Framecounter)
                {
                    default: break;
                    case 7457:
                        APU_QuarterFrameClock = true;
                        break;
                    case 14913:
                        APU_QuarterFrameClock = true;
                        APU_HalfFrameClock = true;
                        break;
                    case 22371:
                        APU_QuarterFrameClock = true;
                        break;
                    case 29828:
                        APU_Status_FrameInterrupt = true;
                        break;
                    case 29829:
                        APU_QuarterFrameClock = true;
                        APU_Status_FrameInterrupt = true;
                        IRQ_LevelDetector |= !APU_FrameCounterInhibitIRQ;
                        APU_HalfFrameClock = true;
                        break;
                    case 29830:
                        APU_Status_FrameInterrupt = !APU_FrameCounterInhibitIRQ;
                        IRQ_LevelDetector |= !APU_FrameCounterInhibitIRQ;

                        APU_Framecounter = 0;

                        break;
                }

            }





            // perform quarter frame / half frame stuff

            if (APU_QuarterFrameClock)
            {
                APU_QuarterFrameClock = false;
                if (APU_Envelope_StartFlag)
                {
                    APU_Envelope_StartFlag = false;
                    APU_Envelope_DecayLevel = 15;

                }
                else
                {
                    APU_Envelope_DividerClock = true;
                }
            }

            if (APU_HalfFrameClock)
            {
                if (APU_LengthCounter_ReloadPulse1 && APU_LengthCounter_Pulse1 == 0) { APU_LengthCounter_Pulse1 = APU_LengthCounter_ReloadValuePulse1; } else { APU_LengthCounter_ReloadPulse1 = false; }
                if (APU_LengthCounter_ReloadPulse2 && APU_LengthCounter_Pulse2 == 0) { APU_LengthCounter_Pulse2 = APU_LengthCounter_ReloadValuePulse2; } else { APU_LengthCounter_ReloadPulse2 = false; }
                if (APU_LengthCounter_ReloadTriangle && APU_LengthCounter_Triangle == 0) { APU_LengthCounter_Triangle = APU_LengthCounter_ReloadValueTriangle; } else { APU_LengthCounter_ReloadTriangle = false; }
                if (APU_LengthCounter_ReloadNoise && APU_LengthCounter_Noise == 0) { APU_LengthCounter_Noise = APU_LengthCounter_ReloadValueNoise; } else { APU_LengthCounter_ReloadNoise = false; }
                APU_HalfFrameClock = false;
                // length counters and sweep
                if (!APU_Status_Pulse1) { APU_LengthCounter_Pulse1 = 0; }
                if (!APU_Status_Pulse2) { APU_LengthCounter_Pulse2 = 0; }
                if (!APU_Status_Triangle) { APU_LengthCounter_Triangle = 0; }
                if (!APU_Status_Noise) { APU_LengthCounter_Noise = 0; }

                if (APU_LengthCounter_Pulse1 != 0 && !APU_LengthCounter_HaltPulse1 && !APU_LengthCounter_ReloadPulse1)
                {
                    APU_LengthCounter_Pulse1--;
                }
                if (APU_LengthCounter_Pulse2 != 0 && !APU_LengthCounter_HaltPulse2 && !APU_LengthCounter_ReloadPulse2)
                {
                    APU_LengthCounter_Pulse2--;
                }
                if (APU_LengthCounter_Triangle != 0 && !APU_LengthCounter_HaltTriangle && !APU_LengthCounter_ReloadTriangle)
                {
                    APU_LengthCounter_Triangle--;
                }
                if (APU_LengthCounter_Noise != 0 && !APU_LengthCounter_HaltNoise && !APU_LengthCounter_ReloadNoise)
                {
                    APU_LengthCounter_Noise--;
                }
            }
            else
            {
                if (APU_LengthCounter_ReloadPulse1) { APU_LengthCounter_Pulse1 = APU_LengthCounter_ReloadValuePulse1; }
                if (APU_LengthCounter_ReloadPulse2) { APU_LengthCounter_Pulse2 = APU_LengthCounter_ReloadValuePulse2; }
                if (APU_LengthCounter_ReloadTriangle) { APU_LengthCounter_Triangle = APU_LengthCounter_ReloadValueTriangle; }
                if (APU_LengthCounter_ReloadNoise) { APU_LengthCounter_Noise = APU_LengthCounter_ReloadValueNoise; }
                APU_LengthCounter_ReloadPulse1 = false;
                APU_LengthCounter_ReloadPulse2 = false;
                APU_LengthCounter_ReloadTriangle = false;
                APU_LengthCounter_ReloadNoise = false;
            }

            APU_LengthCounter_HaltPulse1 = ((APU_Register[0] & 0x20) != 0);
            APU_LengthCounter_HaltPulse2 = ((APU_Register[4] & 0x20) != 0);
            APU_LengthCounter_HaltTriangle = ((APU_Register[8] & 0x80) != 0);
            APU_LengthCounter_HaltNoise = ((APU_Register[0xC] & 0x20) != 0);



        } // and that's it for the APU cycle

        // PPU variables

        public byte PPUBus; // The databus of the Picture Processing Unit
        public int[] PPUBusDecay = new int[8];
        const int PPUBusDecayConstant = 1786830; // 20 frames. Approximately how long it takes for the PPU bus to decay on my console.
        public byte PPUOAMAddress; // The address used to index into Object Attribute Memory
        public bool PPUStatus_VBlank; // This is set during Vblank, and cleared at the end, or if $2002 is read. This value can be read in address $2002
        public bool PPUStatus_SpriteZeroHit; // If a sprite zero hit occurs, this is set. This value can be read in address $2002
        public bool PPUStatus_SpriteOverflow; // If a scanline had more than 8 objects in range, this is set. This value can be read in address $2002

        bool PPU_Spritex16; // Are sprites using 8x8 mode, or 8x16 mode? Set by writing to $2000

        public ushort PPU_Scanline; // Which scanline is the PPU currently on
        public ushort PPU_Dot; // Which dot of the scanline is the PPU currently on

        public bool PPU_VRegisterChangedOutOfVBlank;    // when changing the v register (Read write address) out of vblank, palettes can become corrupted
        public bool PPU_OAMCorruptionRenderingDisabledOutOfVBlank;  // When rendering is disabled on specific dots of visible scanlines, OAM data can become corrupted
        public bool PPU_PendingOAMCorruption;// The corruption doesn't take place until rendering is re-enabled.
        public byte PPU_OAMCorruptionIndex;  // The object that gets corrupted depends on when the data was corrupted
        // OAM corruption during OAM evaluation happens with the instant write to $2001 using the databus value. Other parts of sprite evaluation apparently do not.
        public bool PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant;  // When rendering is disabled on specific dots of visible scanlines, OAM data can become corrupted
        public bool PPU_OAMCorruptionRenderingEnabledOutOfVBlank; // If enabling rendering outside vblank, there are alignment specific effects.
        public bool PPU_OAMEvaluationCorruptionOddCycle; // If rendering is disabled during OAM evaluation, it matters if it was on an odd or even cycle.
        public bool PPU_OAMEvaluationObjectInRange; // If rendering is disabled during OAM evaluation, it matters if the most recent object evaluated was in vertical range of this scanline.
        public bool PPU_OAMEvaluationObjectInXRange; // If rendering is disabled during OAM evaluation, it matters if the most recent object evaluated was in vertical range of this scanline.

        public bool PPU_PaletteCorruptionRenderingDisabledOutOfVBlank;  // When rendering is disabled on specific dots of visible scanlines, OAM data can become corrupted


        byte PPU_AttributeLatchRegister;
        ushort PPU_BackgroundAttributeShiftRegisterL; // 8 bit latch for the background tile attributes low bit plane.
        ushort PPU_BackgroundAttributeShiftRegisterH; // 8 bit latch register for the background tile attributes high bit plane.
        ushort PPU_BackgroundPatternShiftRegisterL; // 16 bit shift register for the background tile pattern low bit plane.
        ushort PPU_BackgroundPatternShiftRegisterH; // 16 bit shift register for the background tile pattern high bit plane.
        //TempPPUAddr
        public byte PPU_FineXScroll; // Set when writing to address $2005. 3 bits. This is up to a 7 pixel offset when rendering the screen.

        byte[] PPU_SpriteShiftRegisterL = new byte[8]; // 8 bit shift register for a sprite's low bit plane. Secondary OAM can have up to 8 object in it.
        byte[] PPU_SpriteShiftRegisterH = new byte[8]; // 8 bit shift register for a sprite's high bit plane. Secondary OAM can have up to 8 object in it.

        byte[] PPU_SpriteAttribute = new byte[8]; // Secondary OAM attribute values. Secondary OAM can have up to 8 objects in it.
        byte[] PPU_SpritePattern = new byte[8]; // Secondary OAM pattern values. Secondary OAM can have up to 8 objects in it.
        byte[] PPU_SpriteXposition = new byte[8]; // Secondary OAM x positions. Secondary OAM can have up to 8 objects in it.
        byte[] PPU_SpriteYposition = new byte[8]; // Secondary OAM y positions. Secondary OAM can have up to 8 objects in it.

        byte[] PPU_SpriteShifterCounter = new byte[8]; // This counter tracks how long until the objects are drawn.


        bool PPU_NextScanlineContainsSpriteZero;    // If this upcoming scanline contains sprite zero
        bool PPU_CurrentScanlineContainsSpriteZero; // if the sprite evaluation for this current scanline contained sprite zero. Used for Sprite Zero Hit detection.

        public byte PPU_SpritePatternL; // Temporary value used in sprite evaluation.
        public byte PPU_SpritePatternH; // Temporary value used in sprite evaluation.

        byte PPU_Ctrl; // Used exclusively in debugging. If "observing" address $2000, this holds a copy of the value written there.

        byte PPU_Mask; // Used exclusively in debugging. If "observing" address $2001, this holds a copy of the value written there.
        bool PPU_Mask_Greyscale;         // Set by writing to $2001. If set, only use color 00, 10, 20, or 30 when drawing a pixel.
        bool PPU_Mask_8PxShowBackground; // Set by writing to $2001. If set, the background will be visible in the 8 left-most pixels of the screen.
        bool PPU_Mask_8PxShowSprites;    // Set by writing to $2001. If set, the sprites will be visible in the 8 left-most pixels of the screen.
        bool PPU_Mask_ShowBackground;    // Set by writing to $2001. If set, the background will be visible. Anything that requires rendering to be enabled will run, even if it doesn't involve the background.
        bool PPU_Mask_ShowSprites;       // Set by writing to $2001. If set, the sprites will be visible.  Anything that requires rendering to be enabled will run, even if it doesn't involve sprites.
        bool PPU_Mask_EmphasizeRed;      // Set by writing to $2001. Adjusts the colors on screen to be a bit more red.
        bool PPU_Mask_EmphasizeGreen;    // Set by writing to $2001. Adjusts the colors on screen to be a bit more green.
        bool PPU_Mask_EmphasizeBlue;     // Set by writing to $2001. Adjusts the colors on screen to be a bit more blue.

        bool PPU_Mask_ShowBackground_Delayed; // Sprite evaluation has a 1 ppu cycle delay on checking if rendering is enabled.
        bool PPU_Mask_ShowSprites_Delayed; // Sprite evaluation has a 1 ppu cycle delay on checking if rendering is enabled.
        bool PPU_Mask_ShowBackground_Instant; // OAM evaluation will stop immediately if writing to $2001
        bool PPU_Mask_ShowSprites_Instant; // OAM evaluation will stop immediately if writing to $2001

        byte PPU_LowBitPlane; // Temporary value used in background shift register preparation.
        byte PPU_HighBitPlane;// Temporary value used in background shift register preparation.
        byte PPU_Attribute; // Temporary value used in background shift register preparation.
        byte PPU_NextCharacter; // Temporary value used in background shift register preparation.

        bool PPU_CanDetectSpriteZeroHit; // Only 1 sprite zero hit is allowed per frame. This gets set if a sprite zero hit occurs, and cleared at the end of vblank.

        ushort PPU_ADDR_Prev; // The MMC3 chip's IRQ counter is changed whenever bit 12 of the PPU Address is changing from a 0 to a 1. This is recorded at the start of a PPU cycle, and checked at the end.

        public bool PPU_OddFrame; // Every other frame is 1 ppu cycle shorter.

        public byte DotColor; // The pixel output is delayed by 2 dots.
        public byte PrevDotColor; // This is the value from last cycle.
        public byte PrevPrevDotColor; // And this is from 2 cycles ago.
        public byte PrevPrevPrevDotColor; // And this is from 2 cycles ago.
        public int PrevPrevPrevPrevDotColor; // This is used with NTSC signal decoding.
        public byte PaletteRAMAddress;
        public bool ThisDotReadFromPaletteRAM;


        public bool NMI_PinsSignal; // I'm using this to detect the rising edge of $2000.7 and $2002.7
        public bool NMI_PreviousPinsSignal; // I'm using this to detect the rising edge of $2000.7 and $2002.7
        public bool IRQ_LevelDetector; // If set, it's time to run an IRQ whenever this is detected
        public bool NMILine; // Set to true if $2000.7 and $2002.7 are both set. This is checked during the second half od a CPU cycle.
        public bool IRQLine; // Set during phi2 to true if the IRQ level detector is low.

        bool CopyV = false; // set by writes to $2006. If it occurs on the same dot the scroll values are naturally incremented, some bugs occur.
        bool SkippedPreRenderDot341 = false;

        void _EmulatePPU()
        {

            // When writing to ppu registers, there's a slight delay before resulting action is taken.
            // This delay can vary depending on the CPU/PPU alignment.

            // For instance, after writing to $2006, this delay value will either be 4 or 5.
            CopyV = false;
            if (PPU_Update2006Delay > 0)
            {
                PPU_Update2006Delay--; // this counts down,
                if (PPU_Update2006Delay == 0) // and when it reaches zero
                {
                    ushort temp_Prev_V = PPU_ReadWriteAddress;
                    CopyV = true;
                    PPU_ReadWriteAddress = PPU_TempVRAMAddress; // the PPU_ReadWriteAddress is updated!
                    PPU_AddressBus = PPU_ReadWriteAddress; // This value is the same thing.
                    if ((temp_Prev_V & 0x3FFF) >= 0x3F00 && (PPU_AddressBus & 0x3FFF) < 0x3F00) // Palette corruption check. Are we leaving Palette ram?
                    {
                        if ((PPU_Scanline < 240) && PPU_Dot <= 256) // if this dot is visible
                        {
                            if ((temp_Prev_V & 0xF) != 0)  // also, Palette corruption only happens if the previous address did not end in a 0
                            {
                                PPU_VRegisterChangedOutOfVBlank = true;
                            }
                        }
                    }
                }
            }
            // after writing to $2005, there is either a 1 or 2 cycle delay.
            if (PPU_Update2005Delay > 0)
            {
                PPU_Update2005Delay--;
                if (PPU_Update2005Delay == 0)
                {
                    if (!PPUAddrLatch)
                    {
                        // if this is the first write to $2005
                        PPU_FineXScroll = (byte)(PPU_Update2005Value & 7); // This updates the fine X scroll
                        PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0111111111100000) | (PPU_Update2005Value >> 3)); // as well as changing the 't' register.
                    }
                    else
                    {
                        // if this is the second write to $2005
                        PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0000110000011111) | (((PPU_Update2005Value & 0xF8) << 2) | ((PPU_Update2005Value & 7) << 12))); // this also writes to 't'
                    }
                    PPUAddrLatch = !PPUAddrLatch; // flip the latch
                }
            }
            // after writing to $2000, there's either a 1 or 2 cycle delay
            if (PPU_Update2000Delay > 0)
            {
                PPU_Update2000Delay--;
                if (PPU_Update2000Delay == 0)
                {
                    PPU_Ctrl = PPU_Update2000Value; // this value is only used for debugging.
                    PPUControl_NMIEnabled = (PPU_Update2000Value & 0x80) != 0;
                    PPUControlIncrementMode32 = (PPU_Update2000Value & 0x4) != 0;
                    PPU_Spritex16 = (PPU_Update2000Value & 0x20) != 0;
                    PPU_PatternSelect_Sprites = (PPU_Update2000Value & 0x8) != 0;
                    PPU_PatternSelect_Background = (PPU_Update2000Value & 0x10) != 0;
                    PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0111001111111111) | ((PPU_Update2000Value & 0x3) << 10)); // change which nametable to render.


                }
            }

            if (PPU_Data_StateMachine < 9)
            {
                // This info was not determined by using visualNES or visual2c02, and is entirely "speculation" based on behavior I was able to detect on my console through read-modify-write instructions to address $2007.

                // reading/writing to address $2007 will set the state machine value to 0. Increment it every PPU Cycle
                // There's a handful of unexpected behavior if this state machine is currently happening when another read/write to $2007 occurs
                // in other words, if 2 consecutive CPU cycles access $2007 there's unexpected behavior.
                // that behavior is handled here.

                // NOTE: This behavior matches my console, though different revisions have shown different behaviors.

                // TODO: Something is going wrong with the timing of STA $2007, X (where X = 0). Gotta figure that out, and probably re-do this entire function. I have no idea how inaccurate this is. 

                if (PPU_Data_StateMachine == 1) // 1 ppu cycle after the read occurs
                {
                    if (PPU_Data_StateMachine_Read && !PPU_Data_StateMachine_UpdateVRAMBufferLate) // if this is a read, and PPU_Data_StateMachine_UpdateVRAMBufferLate is not set: (I think this is just for alignments 2 and 3?)
                    {
                        if (PPU_ReadWriteAddress >= 0x3F00) // If the read/write address is where the Palette info is...
                        {
                            PPU_AddressBus = PPU_ReadWriteAddress;
                            PPU_VRAMAddressBuffer = FetchPPU((ushort)(PPU_AddressBus & 0x2FFF)); // The buffer cannot read from the palettes.
                        }
                        else
                        {
                            PPU_AddressBus = PPU_ReadWriteAddress;
                            PPU_VRAMAddressBuffer = FetchPPU((ushort)(PPU_AddressBus & 0x3FFF));
                        }
                    }
                }
                if (PPU_Data_StateMachine == 3)
                {
                    // This is only relevant when the state machine is not interrupted.
                    if (PPU_Data_StateMachine_NormalWriteBehavior)
                    {
                        PPU_Data_StateMachine_NormalWriteBehavior = false;
                        if (!PPU_Data_StateMachine_Read || !PPU_Data_StateMachine_Read_Delayed)
                        {
                            PPU_AddressBus = PPU_ReadWriteAddress;
                            StorePPUData(PPU_AddressBus, PPU_Data_StateMachine_InputValue);
                        }
                    }
                    // if the state machine *is* interrupted, this runs
                    else
                    if (!PPU_Data_StateMachine_Read && PPU_Data_StateMachine_PerformMysteryWrite)
                    {
                        // the mystery write

                        // Here's how the mystery write behaves:
                        // Suppose we're writing a value of $ZZ to address $2007, and the PPU Read/Write address is at address $YYXX
                        // The mystery write will store $ZZ at address $YYZZ
                        // In addition to that, $XX (The low byte of the read/write address) is also written to $YYXX

                        // This only occurs if there's 2 consecutive CPU cycles that access $2007

                        // The mystery writes cannot write to palettes. Instead, write the modified value read from palette RAM to the following address.
                        if (PPU_VRAM_MysteryAddress >= 0x3F00)
                        {

                            StorePPUData((ushort)(PPU_ReadWriteAddress), (byte)PPU_VRAM_MysteryAddress);
                            PPU_AddressBus = PPU_ReadWriteAddress;

                        }
                        else
                        {
                            // As far as I know, the PPU can only make 1 write per cycle... The exact timing here might be wrong, but the end result of the behavior emulated here seems to match my console.
                            StorePPUData((ushort)(PPU_VRAM_MysteryAddress), (byte)PPU_VRAM_MysteryAddress);
                            StorePPUData((ushort)(PPU_ReadWriteAddress), (byte)PPU_ReadWriteAddress);
                            PPU_AddressBus = PPU_ReadWriteAddress;
                        }

                        // That second write can be overwritten in the next steps depending on the CPU/PPU alignment.
                        // My current understanding is: if the mystery write happens, that other extra write happens too.
                        // but again, I'm not certain on the timing. Do these actually both happen on the same cycle?
                    }
                    // the PPU Read/Write address is incremented 1 cycle after the write occurs.
                }
                if (PPU_Data_StateMachine == 4) // 4 ppu cycles after a read or  1 ppu cycle after a write occurs
                {
                    // This is alignment-specific behavior due to a Read-Modify-Write instruction on address $2007
                    if (PPU_Data_StateMachine_Read && PPU_Data_StateMachine_UpdateVRAMBufferLate)
                    {
                        if (PPU_ReadWriteAddress >= 0x3F00) // If the read/write address is where the Palette info is...
                        {
                            PPU_AddressBus = PPU_ReadWriteAddress;
                            PPU_VRAMAddressBuffer = FetchPPU((ushort)(PPU_AddressBus & 0x2FFF));// The buffer cannot read from the palettes.
                        }
                        else
                        {
                            PPU_AddressBus = PPU_ReadWriteAddress;
                            PPU_VRAMAddressBuffer = FetchPPU((ushort)(PPU_AddressBus & 0x3FFF));
                        }
                    }
                    // We're getting deep into alignment specific state machine shenanigans.
                    // If the state machine was interrupted with a read cycle, and the CPU/PPU is not in alignment 0:
                    if (PPU_Data_StateMachine_UpdateVRAMAddressEarly)
                    {
                        PPU_Data_StateMachine_UpdateVRAMAddressEarly = false;
                        // The VRAM address is updated earlier than expected.
                        PPU_ReadWriteAddress += PPUControlIncrementMode32 ? (ushort)32 : (ushort)1; // add either 1 or 32 depending on PPU_CRTL
                        PPU_ReadWriteAddress &= 0x3FFF; // and truncate to just 15 bits
                        PPU_AddressBus = PPU_ReadWriteAddress;
                        // Read from the new VRAM address
                        if (PPU_Data_StateMachine_Read)
                        {
                            if (PPU_ReadWriteAddress >= 0x3F00) // If the read/write address is where the Palette info is...
                            {
                                PPU_VRAMAddressBuffer = FetchPPU((ushort)(PPU_AddressBus & 0x2FFF)); // The buffer cannot read from the palettes.
                            }
                            else
                            {
                                PPU_VRAMAddressBuffer = FetchPPU((ushort)(PPU_AddressBus & 0x3FFF));
                            }
                        }
                        // And then the VRAM address is updated again!
                    }



                    if ((PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && (PPU_Scanline < 240 || PPU_Scanline == 261))
                    {
                        // If rendering is enabled when v increments, v increments both horizontally and vertically, with wraparound behavior too.
                        PPU_IncrementScrollX();
                        PPU_IncrementScrollY();
                    }
                    else
                    {
                        // This part here happens regardless of state machine shenanigans. This is just the state machine working as intended.
                        PPU_ReadWriteAddress += PPUControlIncrementMode32 ? (ushort)32 : (ushort)1; // add either 1 or 32 depending on PPU_CRTL
                        PPU_ReadWriteAddress &= 0x3FFF;                                             // and truncate to just 15 bits
                    }

                    PPU_AddressBus = PPU_ReadWriteAddress;

                    // The mystery write strikes back! (Keep in mind, this is only used during state machine shenanigans. Normal writes to $2007 happen on cycle 3 of the state machine.
                    // (at least that's how I'm emulating it? More research is needed for the actual cycle-by-cycle breakdown of this state machine.)
                    if (!PPU_Data_StateMachine_Read || !PPU_Data_StateMachine_Read_Delayed)
                    {
                        if (PPU_Data_StateMachine_PerformMysteryWrite)
                        {
                            if ((CPUClock & 3) != 0) // This write only occurs on phases 1, 2, and 3
                            {
                                // Store the expected value at the *recently modified* Read/Write address.
                                StorePPUData(PPU_AddressBus, PPU_Data_StateMachine_InputValue);
                            }
                        }
                    }
                    PPU_Data_StateMachine_Read = PPU_Data_StateMachine_Read_Delayed;
                    PPU_Data_StateMachine_PerformMysteryWrite = false;
                }
                // And that's it for the PPU $2007 State Machine.
                PPU_Data_StateMachine++;    // this stops counting up at 8.
            }
            if (PPU_Data_StateMachine == 8)
            {
                if (PPU_Data_StateMachine_InterruptedReadToWrite)
                {
                    if ((CPUClock & 3) != 0) // This write only occurs on phases 1, 2, and 3
                    {
                        StorePPUData(PPU_AddressBus, PPU_Data_StateMachine_InputValue);
                    }
                    PPU_Data_StateMachine_InterruptedReadToWrite = false;
                    PPU_ReadWriteAddress += PPUControlIncrementMode32 ? (ushort)32 : (ushort)1; // add either 1 or 32 depending on PPU_CRTL
                    PPU_ReadWriteAddress &= 0x3FFF; // and truncate to just 15 bits
                    PPU_AddressBus = PPU_ReadWriteAddress;


                }
            }

            // Updating the scroll registers during screen rendering
            if (PPU_Scanline < 240 || PPU_Scanline == 261)// if this is the pre-render line, or any line before vblank
            {
                if ((PPU_Mask_ShowBackground || PPU_Mask_ShowSprites))
                {
                    if (PPU_Dot == 256) //The Y scroll is incremented on dot 256.
                    {
                        PPU_IncrementScrollY();
                    }
                    else if (PPU_Dot == 257) //The X scroll is reset on dot 257.
                    {
                        PPU_ResetXScroll();
                    }
                    if (PPU_Dot >= 280 && PPU_Dot <= 304 && PPU_Scanline == 261) //numbers from the nesdev wiki
                    {
                        PPU_ResetYScroll(); //The Y scroll is reset on every dot from 280 through 304 on the pre-render scanline.
                    }
                }
            }

            // Increment the PPU dot
            PPU_Dot++;
            if (PPU_Dot > 340) // There are only 341 dots per scanline
            {
                PPU_Dot = 0;  // reset the dot back to 0
                PPU_Scanline++;     // and increment the scanline
                // Sprite zero hits rely on the previous scanline's sprite evaluation.

                if (PPU_Scanline > 261) // There are 262 scanlines in a frame.
                {
                    PPU_Scanline = 0;   // reset to scanline 0.
                }
            }

            if (PPU_Scanline == 241) // If this is the first scanline of VBLank
            {
                if (PPU_Dot == 0)
                {
                    // If Address $2002 is read during the next ppu cycle, the PPU Status flags aren't set.
                    // These variables are used to check if Address $2002 is read during the next ppu cycle.
                    // I usually refer to this as the $2002 race condition.
                    // The more proper term would be "Vblank/NMI flag suppression".

                    // oh- and also if we're running a fm2 TAS file, due to FCEUX's incorrect timing of the first frame, I need to prevent this from being set just a few cycles after power on.
                    if (!SyncFM2)
                    {
                        PPU_PendingVBlank = true;
                    }
                    else
                    {
                        SyncFM2 = false;
                    }
                }
                if (PPU_Dot == 1)
                {
                    if (PPU_PendingVBlank) // If a read to $2002 did not happen this cycle. (Reading $2002 sets PPU_PendingVBlank to false)
                    {
                        // Huzzah! The status flags are set.
                        PPUStatus_VBlank = true;
                        PPU_PendingVBlank = false; // clear this flag
                                                   // if PPUControl_NMIEnabled is set to true, then the NMI edge detector will detect this at the end of the CPU cycle!
                        PPU_RESET = false;
                    }
                    // else, address $2002 was read on this ppu cycle. no VBlank flag.
                    if (!PPU_ShowScreenBorders)
                    {
                        FrameAdvance_ReachedVBlank = true; // Emulator specific stuff. Used for frame advancing to detect the frame has ended, and nothing else.
                    }
                    if (!ClockFiltering) // specifically for TASing stuff. Increment the index for the input log.
                    {
                        // If this was using "SubFrame", TAS_InputSequenceIndex is incremented evnever the controller is strobed.
                        // Instead, I increment the index here at the start of vblank.
                        TAS_InputSequenceIndex++;
                    }


                }

            }
            else if (PPU_Scanline == 242 && PPU_Dot == 1)
            {
                if (PPU_ShowScreenBorders && !PPU_DecodeSignal) // if we're showing the boarders, we need to wait for 2 more scanlines to render.
                {
                    FrameAdvance_ReachedVBlank = true; // Emulator specific stuff. Used for frame advancing to detect the frame has ended, and nothing else.
                }
            }
            else if (PPU_Scanline == 260 && PPU_Dot == 340)
            {
                PPU_OddFrame = !PPU_OddFrame; // I guess this could happen on pretty much any cycle?
            }
            else if (PPU_Scanline == 261 && PPU_Dot == 0)
            {
                PPUStatus_SpriteZeroHit = false;
                // this contradicts the information on the nesdev wiki, but I think I'm going to go mad if this really is cleared on dot 1.
            }
            else if (PPU_Scanline == 261 && PPU_Dot == 1)
            {
                // On the dot 1 of the pre-render scanline, all of these flags are cleared.
                PPUStatus_VBlank = false;
                PPUStatus_SpriteOverflow = false;
                PPU_CanDetectSpriteZeroHit = true;
            }

            else if (PPU_Scanline == 0 && PPU_Dot == 1)
            {
                if (PPU_ShowScreenBorders && PPU_DecodeSignal) // if we're showing the boarders, we need to wait for scanline 0.
                {
                    FrameAdvance_ReachedVBlank = true; // Emulator specific stuff. Used for frame advancing to detect the frame has ended, and nothing else.
                }
            }
            // Right now, I'm only emulating MMC3's IRQ counter in this function.
            PPU_MapperSpecificFunctions();
            PPU_ADDR_Prev = PPU_AddressBus; // Record the value of the ppu address bus. This is used in the PPU_MapperSpecificFunctions(), so if this changes between here and next ppu cycle, we'll know.
            if (PPU_OddFrame && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites))
            {
                if (PPU_Scanline == 261 && PPU_Dot == 340)
                {
                    // On every other frame, dot 0 of scanline 0 is skipped.
                    // this cycle is technically (0,0), but this still makes the Nametable fetch during the last cycle of the pre-render line
                    PPU_Scanline = 0;
                    PPU_Dot = 0;
                    SkippedPreRenderDot341 = true;
                }
            }
            if (PPU_OddFrame && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && PPU_Scanline == 0 && PPU_Dot == 2)
            {
                SkippedPreRenderDot341 = false; // This variable is used for some esoteric business on dot 1 of scanline 0.
            }
            // Okay, now that we're updated all those flags, let's render stuff to the screen!

            // let's establish the order of operations.
            // Sprite evaluation
            // then calculate the color for the next dot.

            //but to complicate things, the delay after writing to $2001 happens between those 2 steps, and also on a specific alignment, this delay is 1 cycle longer for sprite evaluation.

            // If this is NOT phase 1
            if ((MasterClock & 3) != 2)
            {
                // sprite evaluation has a 1 ppu cycle delay before recognizing these flags were set or cleared.
                PPU_Mask_ShowBackground_Delayed = PPU_Mask_ShowBackground;
                PPU_Mask_ShowSprites_Delayed = PPU_Mask_ShowSprites;
            }
            if ((PPU_Scanline < 240 || PPU_Scanline == 261))// if this is the pre-render line, or any line before vblank
            {
                // Sprite evaluation
                if (PPU_Scanline < 241 || PPU_Scanline == 261)
                {
                    PPU_Render_SpriteEvaluation(); // fill in secondary OAM, and set up various arrays of sprite properties.
                }
            }
            if ((MasterClock & 3) == 2)
            {
                // on phase 1,
                // sprite evaluation has a 2 ppu cycle delay before recognizing these flags were set or cleared.
                PPU_Mask_ShowBackground_Delayed = PPU_Mask_ShowBackground;
                PPU_Mask_ShowSprites_Delayed = PPU_Mask_ShowSprites;
            }
            // after sprite evaluation, but before screen rendering...
            if (PPU_Update2001Delay > 0) // if we wrote to 2001 recently
            {
                PPU_Update2001Delay--;
                if (PPU_Update2001Delay == 1)
                {

                }
                if (PPU_Update2001Delay == 0) // if we've waited enough cycles, apply the changes
                {
                    PPU_Mask = PPU_Update2001Value; // this value is only used for debugging.
                    PPU_Mask_8PxShowBackground = (PPU_Update2001Value & 0x02) != 0;
                    PPU_Mask_8PxShowSprites = (PPU_Update2001Value & 0x04) != 0;
                    PPU_Mask_ShowBackground = (PPU_Update2001Value & 0x08) != 0;
                    PPU_Mask_ShowSprites = (PPU_Update2001Value & 0x10) != 0;

                    PPU_Mask_ShowBackground_Instant = PPU_Mask_ShowBackground; // now that the PPU has updated, OAM evaluation will also recognize the change
                    PPU_Mask_ShowSprites_Instant = PPU_Mask_ShowSprites;
                }
            }
            if (PPU_Update2001OAMCorruptionDelay > 0) // if we wrote to 2001 recently
            {
                PPU_Update2001OAMCorruptionDelay--;
                if (PPU_Update2001OAMCorruptionDelay == 0) // if we've waited enough cycles, apply the changes
                {
                    if (PPU_WasRenderingBefore2001Write && (PPU_Update2001Value & 0x08) == 0 && (PPU_Update2001Value & 0x10) == 0)
                    {
                        if ((PPU_Scanline < 240 || PPU_Scanline == 261)) // if this is the pre-render line, or any line before vblank
                        {
                            if (!PPU_PendingOAMCorruption) // due to OAM corruption occurring inside OAM evaluation before this even occurs, make sure OAM isn't already corrupt
                            {
                                PPU_OAMCorruptionRenderingDisabledOutOfVBlank = true;
                            }
                        }
                    }
                }
            }
            if (PPU_Update2001EmphasisBitsDelay > 0)
            {
                PPU_Update2001EmphasisBitsDelay--;
                if (PPU_Update2001EmphasisBitsDelay == 0)
                {
                    PPU_Mask_Greyscale = (PPU_Update2001Value & 0x01) != 0;
                    PPU_Mask_EmphasizeRed = (PPU_Update2001Value & 0x20) != 0;
                    PPU_Mask_EmphasizeGreen = (PPU_Update2001Value & 0x40) != 0;
                    PPU_Mask_EmphasizeBlue = (PPU_Update2001Value & 0x80) != 0;
                }
            }

            PrevPrevPrevDotColor = PrevPrevDotColor; // Drawing a color to the screen has a 3(?) ppu cycle delay between deciding the color, and drawing it.
            PrevPrevDotColor = PrevDotColor;
            PrevDotColor = DotColor; // These variables here just record the color, and swap them through these variables so it can be used 3 cycles after it was chosen.
            PPU_Render_CommitShiftRegistersAndBitPlanes();
            if ((PPU_Scanline < 240 || PPU_Scanline == 261))// if this is the pre-render line, or any line before vblank
            {
                if ((PPU_Dot >= 0 && PPU_Dot < 257) || (PPU_Dot > 320 && PPU_Dot <= 336)) // if this is a visible pixel, or preparing the start of next scanline
                {
                    if ((PPU_Mask_ShowBackground || PPU_Mask_ShowSprites)) // if rendering background or sprites
                    {
                        PPU_Render_ShiftRegistersAndBitPlanes(); // update shift registers for the background.
                    }
                }
                else if (PPU_Dot > 336)
                {
                    PPU_Render_ShiftRegistersAndBitPlanes_DummyNT();
                }

                if ((PPU_Dot > 0 && PPU_Dot <= 257)) // if this is a visible pixel, or preparing the start of next scanline
                {
                    if (PPU_Scanline < 241)
                    {
                        PPU_Render_CalculatePixel(false); // this determines the color of the pixel being drawn.
                    }
                    UpdateSpriteShiftRegisters(); // update shift registers for the sprites.
                }
                else
                {
                    if (PPU_ShowScreenBorders) // Draw the pixels in the boarder too.
                    {
                        PPU_Render_CalculatePixel(true); // this determines the color of the pixel being drawn.
                    }
                }


                if (!PPU_ShowScreenBorders)
                {
                    DrawToScreen();

                    if (PPU_DecodeSignal && (PPU_Dot == 0) && PPU_Scanline < 241)
                    {
                        ntsc_signal_of_dot_0 = ntsc_signal;
                        chosenColor = PaletteRAM[0x00] & 0x3F;
                        if (PPU_Mask_Greyscale) // if the ppu greyscale mode is active,
                        {
                            chosenColor &= 0x30; //To force greyscale, bitwise AND this color with 0x30
                        }
                        // emphasis bits
                        int emphasis = 0;
                        if (PPU_Mask_EmphasizeRed) { emphasis |= 0x40; } // if emhpasizing r, add 0x40 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeGreen) { emphasis |= 0x80; } // if emhpasizing g, add 0x80 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeBlue) { emphasis |= 0x100; } // if emhpasizing b, add 0x100 to the index into the palette LUT.
                        PrevPrevPrevPrevDotColor = chosenColor | emphasis; // set up samples for dot 1
                        PPU_SignalDecode(chosenColor | emphasis);
                    }
                    if (PPU_DecodeSignal && (PPU_Dot == 260) && PPU_Scanline < 241)
                    {
                        PPU_SignalDecode(PrevPrevPrevPrevDotColor);
                    }
                    else if (PPU_DecodeSignal && (PPU_Dot == 261) && PPU_Scanline < 241)
                    {
                        RenderNTSCScanline();
                    }
                }
            }
            else if (PPU_ShowScreenBorders)
            {
                PPU_Render_CalculatePixel(true); // this determines the color of the pixel being drawn.
            }
            if (PPU_ShowScreenBorders)
            {
                DrawToBorderedScreen();
            }
            ThisDotReadFromPaletteRAM = false;

            if (PPU_DecodeSignal)
            {
                ntsc_signal += 8;
                ntsc_signal %= 12;
            }
        } // and that's all for the PPU cycle!

        void _EmulateHalfPPU()
        {
            // Oh boy, it's time for half PPU cycles.
            if ((PPU_Scanline < 240 || PPU_Scanline == 261))// if this is the pre-render line, or any line before vblank
            {
                if ((PPU_Dot > 0 && PPU_Dot <= 257) || (PPU_Dot > 320 && PPU_Dot <= 336)) // if this is a visible pixel, or preparing the start of next scanline
                {
                    if ((PPU_Mask_ShowBackground || PPU_Mask_ShowSprites)) // if rendering background or sprites
                    {
                        PPU_UpdateShiftRegisters(); // shift all the shift registers 1 bit
                    }
                }
            }
            PPU_Render_CommitShiftRegistersAndBitPlanes_HalfDot();
            if ((PPU_Scanline < 240 || PPU_Scanline == 261))// if this is the pre-render line, or any line before vblank
            {
                if ((PPU_Dot >= 0 && PPU_Dot < 257) || (PPU_Dot >= 320 && PPU_Dot < 336)) // if this is a visible pixel, or preparing the start of next scanline
                {
                    if ((PPU_Mask_ShowBackground || PPU_Mask_ShowSprites)) // if rendering background or sprites
                    {
                        PPU_Render_ShiftRegistersAndBitPlanes_HalfDot(); // Check if we need to reload the shift registers.
                    }
                }
            }
        }

        void DrawToScreen()
        {
            if (PPU_Dot > 3 && PPU_Dot <= 259 && PPU_Scanline < 241) // the process of drawing a dot to the screen actually has a 2 ppu cycle delay, which the emphasis bits happen after
            {
                // in other words, the geryscale/emphasis bits can affect the color that was decided 2 ppu cycles ago.
                chosenColor = PrevPrevPrevDotColor;
                if (PPU_Mask_Greyscale) // if the ppu greyscale mode is active,
                {
                    chosenColor &= 0x30; //To force greyscale, bitwise AND this color with 0x30
                }
                // emphasis bits
                int emphasis = 0;
                if (PPU_Mask_EmphasizeRed) { emphasis |= 0x40; } // if emhpasizing r, add 0x40 to the index into the palette LUT.
                if (PPU_Mask_EmphasizeGreen) { emphasis |= 0x80; } // if emhpasizing g, add 0x80 to the index into the palette LUT.
                if (PPU_Mask_EmphasizeBlue) { emphasis |= 0x100; } // if emhpasizing b, add 0x100 to the index into the palette LUT.
                int scanline0OddFrameOffset = 0;
                if (PPU_Scanline == 0 && PPU_OddFrame && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites))
                {
                    scanline0OddFrameOffset = 1;
                }
                if (!PPU_DecodeSignal)
                {
                    if (!PPU_ShowScreenBorders)
                    {
                        if (scanline0OddFrameOffset == 1 && PPU_Dot == 4)
                        {
                            // do nothing. This would be off screen.
                        }
                        else
                        {
                            Screen.SetPixel(PPU_Dot - 4 - scanline0OddFrameOffset, PPU_Scanline, unchecked((int)NesPalInts[chosenColor | emphasis])); // this sets the pixel on screen to the chosen color.
                        }
                    }
                    else
                    {
                        Screen.SetPixel(PPU_Dot - 4 - scanline0OddFrameOffset, PPU_Scanline, unchecked((int)NesPalInts[chosenColor | emphasis])); // this sets the pixel on screen to the chosen color.
                    }
                }
                else
                {
                    if (PPU_Mask_Greyscale) // if the ppu greyscale mode is active,
                    {
                        chosenColor &= 0x30; //To force greyscale, bitwise AND this color with 0x30
                    }
                    PPU_SignalDecode(chosenColor | emphasis);
                    PrevPrevPrevPrevDotColor = chosenColor | emphasis;
                }
            }
            if (PPU_Scanline == 0 && PPU_OddFrame && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && PPU_Dot == 259)
            {
                // draw the backdrop.
                chosenColor = PaletteRAM[0];
                // emphasis bits
                int emphasis = 0;
                if (PPU_Mask_EmphasizeRed) { emphasis |= 0x40; } // if emhpasizing r, add 0x40 to the index into the palette LUT.
                if (PPU_Mask_EmphasizeGreen) { emphasis |= 0x80; } // if emhpasizing g, add 0x80 to the index into the palette LUT.
                if (PPU_Mask_EmphasizeBlue) { emphasis |= 0x100; } // if emhpasizing b, add 0x100 to the index into the palette LUT.
                if (!PPU_DecodeSignal)
                {
                    Screen.SetPixel(255, PPU_Scanline, unchecked((int)NesPalInts[chosenColor | emphasis])); // this sets the pixel on screen to the chosen color.
                }
                else
                {
                    if (PPU_Mask_Greyscale) // if the ppu greyscale mode is active,
                    {
                        chosenColor &= 0x30; //To force greyscale, bitwise AND this color with 0x30
                    }
                    PPU_SignalDecode(chosenColor | emphasis);
                    PrevPrevPrevPrevDotColor = chosenColor | emphasis;
                }
            }
        }

        void DrawToBorderedScreen()
        {

            int dot = PPU_Dot;
            int scanline = PPU_Scanline;
            int emphasis = 0;

            dot -= 3;
            if (dot < 0)
            {
                dot = 341 + dot;
                scanline--;
                if (scanline < 0)
                {
                    scanline = 261;
                }
            }

            int boarderedDot = 0;
            int boarderedScanline = scanline;

            if (PPU_ShowScreenBorders && dot == 325)
            {
                ntsc_signal_of_dot_0 = ntsc_signal;
            }
            if (PPU_DecodeSignal && dot == 277)
            {
                RenderNTSCScanline();
            }

            if (scanline < 241 || ((scanline == 241 && dot < 277)) || scanline == 261)
            {

                if (dot >= 1 && dot <= 256) // visible pixels.
                {
                    if (scanline == 261)
                    {
                        chosenColor = 0x0F;
                    }
                    else
                    {

                        chosenColor = PrevPrevPrevDotColor;

                        if (PPU_Mask_Greyscale) // if the ppu greyscale mode is active,
                        {
                            chosenColor &= 0x30; //To force greyscale, bitwise AND this color with 0x30
                        }
                        // emphasis bits
                        if (PPU_Mask_EmphasizeRed) { emphasis |= 0x40; } // if emhpasizing r, add 0x40 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeGreen) { emphasis |= 0x80; } // if emhpasizing g, add 0x80 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeBlue) { emphasis |= 0x100; } // if emhpasizing b, add 0x100 to the index into the palette LUT.
                    }
                    boarderedDot = dot + 64;
                    boarderedScanline = scanline;
                }
                else if (dot >= 257 && dot <= 267) // right boarder
                {
                    if (scanline == 261)
                    {
                        chosenColor = 0x0F;
                    }
                    else
                    {
                        // backdrop.
                        chosenColor = PrevPrevPrevDotColor;
                        if (PPU_Mask_Greyscale) // if the ppu greyscale mode is active,
                        {
                            chosenColor &= 0x30; //To force greyscale, bitwise AND this color with 0x30
                        }
                        // emphasis bits
                        if (PPU_Mask_EmphasizeRed) { emphasis |= 0x40; } // if emhpasizing r, add 0x40 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeGreen) { emphasis |= 0x80; } // if emhpasizing g, add 0x80 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeBlue) { emphasis |= 0x100; } // if emhpasizing b, add 0x100 to the index into the palette LUT.
                    }
                    boarderedDot = dot + 64;
                    boarderedScanline = scanline;
                }
                else if (dot >= 268 && dot <= 276) // front porch 
                {
                    // black.
                    chosenColor = 0x0F;
                    boarderedDot = dot + 64;
                    boarderedScanline = scanline;
                }
                else if (dot >= 277 && dot <= 301) // horizontal sync 
                {
                    // black.
                    chosenColor = 0x0F;
                    boarderedDot = dot - 277;
                    boarderedScanline = scanline + 1;
                }
                else if (dot >= 302 && dot <= 305) // back porch 
                {
                    // black.
                    chosenColor = 0x0F;
                    boarderedDot = dot - 277;
                    boarderedScanline = scanline + 1;
                }
                else if (dot >= 306 && dot <= 320) // colorburst 
                {
                    // extremely dark olive.
                    chosenColor = Signal_COLORBURST;
                    boarderedDot = dot - 277;
                    boarderedScanline = scanline + 1;
                }
                else if (dot >= 321 && dot <= 325) // back porch 
                {
                    // black.
                    chosenColor = 0x0F;
                    boarderedDot = dot - 277;
                    boarderedScanline = scanline + 1;
                }
                else if (dot == 326) // pulse  
                {

                    // backdrop in greyscale
                    if (ThisDotReadFromPaletteRAM)
                    {
                        chosenColor = PrevPrevPrevDotColor;
                    }
                    else
                    {
                        chosenColor = PrevPrevPrevDotColor & 0x30;
                    }

                    // emphasis bits
                    if (PPU_Mask_EmphasizeRed) { emphasis |= 0x40; } // if emhpasizing r, add 0x40 to the index into the palette LUT.
                    if (PPU_Mask_EmphasizeGreen) { emphasis |= 0x80; } // if emhpasizing g, add 0x80 to the index into the palette LUT.
                    if (PPU_Mask_EmphasizeBlue) { emphasis |= 0x100; } // if emhpasizing b, add 0x100 to the index into the palette LUT.

                    boarderedDot = dot - 277;
                    boarderedScanline = scanline + 1;
                }
                else // right boarder
                {
                    // backdrop.
                    if (scanline == 261 && dot == 0)
                    {
                        chosenColor = 0x0F;
                    }
                    else
                    {
                        chosenColor = PrevPrevPrevDotColor;

                        if (PPU_Mask_Greyscale) // if the ppu greyscale mode is active,
                        {
                            chosenColor &= 0x30; //To force greyscale, bitwise AND this color with 0x30
                        }
                        // emphasis bits
                        if (PPU_Mask_EmphasizeRed) { emphasis |= 0x40; } // if emhpasizing r, add 0x40 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeGreen) { emphasis |= 0x80; } // if emhpasizing g, add 0x80 to the index into the palette LUT.
                        if (PPU_Mask_EmphasizeBlue) { emphasis |= 0x100; } // if emhpasizing b, add 0x100 to the index into the palette LUT.
                    }
                    if (dot != 0)
                    {
                        boarderedDot = dot - 277;
                        boarderedScanline = scanline + 1;

                    }
                    else
                    {
                        boarderedDot = dot + 64;
                        boarderedScanline = scanline;
                    }
                }
            }
            else
            {
                if (scanline >= 245 && scanline <= 247)
                {
                    // black.
                    chosenColor = 0x0F;
                    if (dot >= 277)
                    {
                        boarderedDot = dot - 277;
                        boarderedScanline = scanline + 1;
                    }
                    else
                    {
                        boarderedDot = dot + 64;
                        boarderedScanline = scanline;
                    }
                }
                else
                {
                    // colorburst happens on this line too.
                    if (dot >= 306 && dot <= 320) // colorburst 
                    {
                        // extremely dark olive.
                        chosenColor = Signal_COLORBURST;
                        boarderedDot = dot - 277;
                        boarderedScanline = scanline + 1;
                    }
                    else
                    {
                        // black.
                        chosenColor = 0x0F;
                        if (dot >= 277)
                        {
                            boarderedDot = dot - 277;
                            boarderedScanline = scanline + 1;
                        }
                        else
                        {
                            boarderedDot = dot + 64;
                            boarderedScanline = scanline;
                        }
                    }
                }
            }
            if (PPU_Scanline == 0 && PPU_OddFrame && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && PPU_Dot < 277)
            {
                boarderedDot--;
            }
            if (boarderedScanline == 0x106)
            {
                boarderedScanline = 0;
            }
            if (PPU_DecodeSignal)
            {
                PPU_SignalDecode(chosenColor | emphasis);
            }
            else
            {
                BorderedScreen.SetPixel(boarderedDot, boarderedScanline, unchecked((int)NesPalInts[chosenColor | emphasis])); // this sets the pixel on screen to the chosen color.
            }
        }


        public bool PPU_DecodeSignal;
        public bool PPU_ShowScreenBorders;
        static float[] Voltages =
            { 0.228f, 0.312f, 0.552f, 0.880f, // Signal low
		        0.616f, 0.840f, 1.100f, 1.100f, // Signal high
		        0.192f, 0.256f, 0.448f, 0.712f, // Signal low, attenuated
		        0.500f, 0.676f, 0.896f, 0.896f  // Signal high, attenuated
		        };
        public byte ntsc_signal;
        public byte ntsc_signal_of_dot_0;
        public float[] NTSC_Samples = new float[257 * 8 + 24];
        public float[] Bordered_NTSC_Samples = new float[341 * 8 + 24];
        static float[] Levels =
            {
            (Voltages[0] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[1] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[2] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[3] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[4] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[5] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[6] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[7] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[8] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[9] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[10] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[11] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[12] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[13] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[14] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f,
            (Voltages[15] - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f
        };
        float Saturation = 0.75f;
        int SignalBufferWidth = 12;
        static double hue = 0;
        static float chroma_saturation_correction = 2.4f;
        static double[] SinTable =
            {
            Math.Sin(Math.PI* (0 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (1 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (2 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (3 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (4 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (5 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (6 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (7 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (8 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (9 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (10 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Sin(Math.PI* (11 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction
             };
        static double[] CosTable =
            {
            Math.Cos(Math.PI* (0 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (1 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (2 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (3 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (4 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (5 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (6 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (7 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (8 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (9 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (10 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction,
            Math.Cos(Math.PI* (11 + 3 - 0.5 + hue) / 6) * chroma_saturation_correction
             };
        bool InColorPhase(int col, int DecodePhase)
        {
            return (col + DecodePhase) % 12 < 6;
        }
        static float ntsc_black = 0.312f, ntsc_white = 1.100f;
        static int Signal_COLORBURST = 512;
        static int Signal_SYNC = 513;
        static float Colorburst_High = (0.524f - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f;
        static float Colorburst_Low = (0.148f - Voltages[1]) / (Voltages[6] - Voltages[1]) / 12f;

        void PPU_SignalDecode(int nesColor)
        {
            bool boardered = PPU_ShowScreenBorders;
            byte phase = ntsc_signal;
            int i = 0;
            while (i < 8)
            {
                float sample = 0;
                // Decode the NES color.
                if (nesColor == Signal_COLORBURST)
                {
                    sample = InColorPhase(0x8, phase) ? Colorburst_High : Colorburst_Low;
                }
                else
                {
                    int colInd = (nesColor & 0x0F);   // 0..15 "cccc"
                    int level = (nesColor >> 4) & 3;  // 0..3  "ll"
                    int emphasis = (nesColor >> 6);   // 0..7  "eee"
                    if (colInd > 13) { level = 1; }   // For colors 14..15, level 1 is forced.
                    int attenuation = (
                                ((((emphasis & 1) != 0) && InColorPhase(0xC, phase)) ||
                                (((emphasis & 2) != 0) && InColorPhase(0x4, phase)) ||
                                (((emphasis & 4) != 0) && InColorPhase(0x8, phase))) && (colInd < 0xE)) ? 8 : 0;
                    float low = Levels[0 + level + attenuation];
                    float high = Levels[4 + level + attenuation];
                    if (colInd == 0) { low = high; } // For color 0, only high level is emitted
                    if (colInd > 12) { high = low; } // For colors 13..15, only low level is emitted
                    sample = InColorPhase(colInd, phase) ? high : low;
                }
                if (boardered)
                {
                    int dot = PPU_Dot - 3;
                    if (dot < 0)
                    {
                        dot = 341 + dot;
                    }
                    if (dot >= 277)
                    {
                        dot -= 277;
                    }
                    else
                    {
                        dot += 64;
                    }
                    if (PPU_Scanline == 0 && PPU_OddFrame && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && PPU_Dot < 277)
                    {
                        dot--;
                    }
                    Bordered_NTSC_Samples[dot * 8 + i] = sample;
                }
                else if (PPU_Dot <= 256 + 3)
                {
                    if (PPU_Dot == 0)
                    {
                        NTSC_Samples[i] = sample;
                    }
                    else
                    {
                        NTSC_Samples[(PPU_Dot - 3) * 8 + i] = sample;
                    }
                }
                phase++;
                phase %= 12;
                i++;
            }
        }
        public bool PPU_ShowRawNTSCSignal;
        void RenderNTSCScanline()
        {
            byte phase = ntsc_signal_of_dot_0;
            bool bordered = PPU_ShowScreenBorders; // this value could change at any moment, so it would be nice to avoid errors due to array lengths.

            int scanline0OddFrameOffset = 0;
            if (PPU_Scanline == 0 && PPU_OddFrame && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && !bordered)
            {
                scanline0OddFrameOffset = 8;
            }

            int width = bordered ? BorderedNTSCScreen.Width : NTSCScreen.Width;

            int i = 0;
            while (i < width + scanline0OddFrameOffset)
            {
                double R = 0;
                double G = 0;
                double B = 0;
                if (!PPU_ShowRawNTSCSignal)
                {
                    int center = i + 8;
                    int begin = center - 6;
                    int end = center + 6;
                    double Y = 0;
                    double U = 0;
                    double V = 0;
                    for (int p = begin; p < end; ++p) // Collect and accumulate samples
                    {
                        float sample = bordered ? (Bordered_NTSC_Samples[p]) : (NTSC_Samples[p]);
                        Y += sample;
                        U += sample * SinTable[(phase + p) % 12];
                        V += sample * CosTable[(phase + p) % 12];
                    }

                    //U *= (0.35355339 * 2);
                    //V *= (0.35355339 * 2);

                    U = U * 0.5f + 0.5f;
                    V = V * 0.5f + 0.5f;

                    bool DebugYUV = false;
                    if (DebugYUV)
                    {
                        Y = 0.5;
                        U = (i + 0.0f) / width;
                        V = 1 - (PPU_Scanline / 240f);

                        U -= 0.5f;
                        V -= 0.5f;

                        U *= (0.35355339 * 2);
                        V *= (0.35355339 * 2);

                        U += 0.5f;
                        V += 0.5f;
                    }

                    // convert YUV to RGB
                    R = 1.164 * (Y - 16 / 256.0) + 1.596 * (V - 128 / 256.0);
                    G = 1.164 * (Y - 16 / 256.0) - 0.392 * (U - 128 / 256.0) - 0.813 * (V - 128 / 256.0);
                    B = 1.164 * (Y - 16 / 256.0) + 2.017 * (U - 128 / 256.0);

                    // other values ?
                    //double R = 1.164 * (Y - 16 / 256.0) + 1.14 * (V - 128 / 256.0);
                    //double G = 1.164 * (Y - 16 / 256.0) - (1 / 1.14) * (U - 128 / 256.0) - (1 / (1.14 * 1.78)) * (V - 128 / 256.0);
                    //double B = 1.164 * (Y - 16 / 256.0) + (1.14 * 1.78) * (U - 128 / 256.0);

                    // convert YUV to normalized RGB
                    //double R = 1.164 * (Y - 16 / 256.0) + 1 * (V - 128 / 256.0);
                    //double G = 1.164 * (Y - 16 / 256.0) - 0.31764705882 * (U - 128 / 256.0) - 0.68359375 * (V - 128 / 256.0);
                    //double B = 1.164 * (Y - 16 / 256.0) + 1 * (U - 128 / 256.0);

                    if (R < 0) { R = 0; }
                    if (R > 1) { R = 1; }
                    if (G < 0) { G = 0; }
                    if (G > 1) { G = 1; }
                    if (B < 0) { B = 0; }
                    if (B > 1) { B = 1; }
                }
                if (PPU_ShowScreenBorders)
                {
                    if (PPU_ShowRawNTSCSignal)
                    {
                        R = Bordered_NTSC_Samples[i] * 12;
                        G = Bordered_NTSC_Samples[i] * 12;
                        B = Bordered_NTSC_Samples[i] * 12;
                        if (R < 0) { R = 0; }
                        if (R > 1) { R = 1; }
                        if (G < 0) { G = 0; }
                        if (G > 1) { G = 1; }
                        if (B < 0) { B = 0; }
                        if (B > 1) { B = 1; }
                    }
                    BorderedNTSCScreen.SetPixel(i, PPU_Scanline, Color.FromArgb((byte)(R * 255), (byte)(G * 255), (byte)(B * 255))); // this sets the pixel on screen to the chosen color. 
                }
                else
                {
                    if (PPU_ShowRawNTSCSignal)
                    {
                        R = NTSC_Samples[i] * 12;
                        G = NTSC_Samples[i] * 12;
                        B = NTSC_Samples[i] * 12;
                        if (R < 0) { R = 0; }
                        if (R > 1) { R = 1; }
                        if (G < 0) { G = 0; }
                        if (G > 1) { G = 1; }
                        if (B < 0) { B = 0; }
                        if (B > 1) { B = 1; }
                    }
                    if (scanline0OddFrameOffset == 0)
                    {
                        NTSCScreen.SetPixel(i, PPU_Scanline, Color.FromArgb((byte)(R * 255), (byte)(G * 255), (byte)(B * 255))); // this sets the pixel on screen to the chosen color.
                    }
                    else
                    {
                        if (i >= 8)
                        {
                            NTSCScreen.SetPixel(i - 8, PPU_Scanline, Color.FromArgb((byte)(R * 255), (byte)(G * 255), (byte)(B * 255))); // this sets the pixel on screen to the chosen color.
                        }
                    }
                }
                i++;
            }
        }

        void PPU_MapperSpecificFunctions()
        {
            if (Cart.MemoryMapper == 4)// MMC3 stuff.
            {
                // if bit 12 of the ppu address bus (A12) changes:
                if (((PPU_ADDR_Prev & 0b0001000000000000) == 0) && ((PPU_AddressBus & 0b0001000000000000) != 0) && MMC3_M2Filter == 3)
                {
                    if (Cart.Mapper_4_ReloadIRQCounter)
                    {
                        // If we're reloading the IRQ counter
                        Cart.Mapper_4_IRQCounter = Cart.Mapper_4_IRQLatch; // The latch is the reset value.
                        Cart.Mapper_4_ReloadIRQCounter = false;
                        if (Cart.Mapper_4_IRQCounter == 0)  // if the latch is set to 0, you need to enable the IRQ.
                        {
                            if (Cart.Mapper_4_EnableIRQ) // if setting the value to zero, run an IRQ
                            {
                                IRQ_LevelDetector = true;
                            }
                        }
                    }
                    else
                    {
                        // decrement the counter
                        Cart.Mapper_4_IRQCounter--;
                        if (Cart.Mapper_4_IRQCounter == 0) // if decrementing the counter moved it to 0...
                        {
                            if (Cart.Mapper_4_EnableIRQ) // and the MMC3 IRQ is enabled...
                            {
                                IRQ_LevelDetector = true; // Run an IRQ!
                            }
                        }
                        else if (Cart.Mapper_4_IRQCounter == 255) // if the counter underflows...
                        {
                            Cart.Mapper_4_IRQCounter = Cart.Mapper_4_IRQLatch; // reset the irq counter
                            if (Cart.Mapper_4_IRQCounter == 0)  // if the latch is set to 0, you need to enable the IRQ... again
                            {
                                if (Cart.Mapper_4_EnableIRQ)
                                {
                                    IRQ_LevelDetector = true;
                                }
                            }
                        }

                    }
                }
                if (ResetM2Filter)
                {
                    ResetM2Filter = false;
                    MMC3_M2Filter = 0;
                }
            }
        }

        // If OAM corruption is pending, it occurs on the first rendered dot.
        public void CorruptOAM()
        {
            // basically 8 entries of OAM are getting replaced (this is considered a single "row" of OAM) 
            // PPU_OAMCorruptionIndex is the row that gets corrupted.
            if (PPU_OAMCorruptionIndex == 0x20)
            {
                PPU_OAMCorruptionIndex = 0;
            }
            int i = 0;
            while (i < 8) // 8 entries in a row
            {
                OAM[PPU_OAMCorruptionIndex * 8 + i] = OAM[i]; // The corrupted row is replaced with the values from row 0
                i++;
            }
            OAM2[PPU_OAMCorruptionIndex] = OAM2[0]; // Also corrupt this byte.
            // this all happens in a single cycle.
        }







        bool OamCorruptedOnOddCycle;
        public byte PPU_SpriteEvaluationTemp; // is this just the ppubus?
        void PPU_Render_SpriteEvaluation()
        {
            bool SpriteEval_ReadOnly_PreRenderLine = false;
            if (PPU_Scanline == 261)
            {
                SpriteEval_ReadOnly_PreRenderLine = true;
            }
            if ((PPU_Mask_ShowBackground_Instant || PPU_Mask_ShowSprites_Instant))
            {
                if (PPU_PendingOAMCorruption) // OAM corruption occurs on the visible dot after rendering was enabled. It also can happen on the pre-render line.
                {
                    PPU_PendingOAMCorruption = false;
                    if (!PPU_OAMCorruptionRenderingEnabledOutOfVBlank)
                    {
                        CorruptOAM();
                    }
                    PPU_OAMCorruptionRenderingEnabledOutOfVBlank = false;
                }
            }

            if ((PPU_Dot >= 0 && PPU_Dot <= 64)) // Dots 1 through 64, not on the pre-render line. (and also dot 0 for OAM corruption purposes)
            {

                // this step is clearing secondary OAM, and writing FF to each byte in the array.
                if ((PPU_Dot & 1) == 1)
                { //odd cycles
                    if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed))
                    {
                        if (SpriteEval_ReadOnly_PreRenderLine)
                        {
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address];
                        }
                        else
                        {
                            PPU_SpriteEvaluationTemp = 0xFF;
                        }
                        if (PPU_Dot == 1)
                        {
                            OAM2Address = 0; // if this is dot 1, reset the secondary OAM address
                            SecondaryOAMFull = false;// also reset the flag that checks of secondary OAM is full.
                                                     // in preparation for the next section, let's clear these flags too
                            SpriteEvaluationTick = 0;
                            OAMAddressOverflowedDuringSpriteEvaluation = false;
                        }
                        if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank)
                        {
                            PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                            PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                            PPU_PendingOAMCorruption = true;
                            PPU_OAMCorruptionIndex = OAM2Address; // this value will be used when rendering is re-enabled and the corruption occurs
                        }
                    }
                }
                else
                { //even cycles
                    if (PPU_Dot > 0)
                    {
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed))
                        {
                            if (!SpriteEval_ReadOnly_PreRenderLine)
                            {
                                OAM2[OAM2Address] = PPU_SpriteEvaluationTemp; // store FF in secondary OAM
                            }
                            if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank)
                            {
                                PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                                PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                                PPU_PendingOAMCorruption = true;
                                PPU_OAMCorruptionIndex = OAM2Address; // this value will be used when rendering is re-enabled and the corruption occurs
                            }

                            OAM2Address++;  // increment this value so on the next even cycle, we write to the next SecondaryOAM address.
                            OAM2Address &= 0x1F;  // keep the secondary OAM address in-bounds

                            if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant && PPU_Dot == 64)
                            {
                                PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                                PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                                PPU_PendingOAMCorruption = true;
                            }
                        }
                        else
                        {
                            if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank)
                            {
                                PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                                PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                                PPU_PendingOAMCorruption = true;
                                PPU_OAMCorruptionIndex = OAM2Address; // this value will be used when rendering is re-enabled and the corruption occurs
                            }
                        }
                    }
                    else
                    {
                        OAM2Address++;  // increment this value so on the next even cycle, we write to the next SecondaryOAM address.
                        OAM2Address &= 0x1F;  // keep the secondary OAM address in-bounds
                    }
                }
            }
            else if ((PPU_Dot >= 65 && PPU_Dot <= 256)) // Dots 65 through 256, not on the pre-render line
            {
                if (PPU_Dot == 65)
                {
                    OAM2Address = 0;
                }
                if (PPU_Mask_ShowBackground_Instant || PPU_Mask_ShowSprites_Instant || PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant) // if rendering is enabled, or was *just* disabled mid evaluation
                {
                    if ((PPU_Dot & 1) == 1)
                    { //odd cycles
                        byte PrevSpriteEvalTemp = PPU_SpriteEvaluationTemp;
                        PPU_SpriteEvaluationTemp = OAM[PPUOAMAddress]; // read from OAM
                        if ((PPUOAMAddress & 3) == 2)
                        {
                            PPU_SpriteEvaluationTemp &= 0xE7; // OAM address 02, 06, 0A, 0E, 12... are missing bits 3 and 4.
                        }

                        // If rendering was disabled *this* cycle (the odd cycle) then the even cycle will run normally, and the *next odd cycle* will have the OAM address increment. Presumably, that's when we record secondOAMAddr.
                        if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant)
                        {
                            PPU_OAMEvaluationCorruptionOddCycle = false;
                            PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                            if (!SpriteEval_ReadOnly_PreRenderLine)
                            {
                                PPUOAMAddress++;
                            }
                            OamCorruptedOnOddCycle = true;

                        }
                    }
                    else
                    { //even cycles                       

                        if (!OAMAddressOverflowedDuringSpriteEvaluation)
                        {
                            byte PreIncVal = PPUOAMAddress; // for checking if PPUOAMAddress overflows
                            if (!SecondaryOAMFull && !SpriteEval_ReadOnly_PreRenderLine) // If secondary OAM is not yet full,
                            {
                                OAM2[OAM2Address] = PPU_SpriteEvaluationTemp; // store this value at the secondary oam address.
                            }
                            byte OAM2READ = OAM2[OAM2Address];
                            if (SpriteEvaluationTick == 0) // tick 0: check if this object's y position is in range for this scanline
                            {
                                PPU_OAMEvaluationObjectInXRange = false;
                                if (!SpriteEval_ReadOnly_PreRenderLine && (PPU_Scanline & 0xFF) - PPU_SpriteEvaluationTemp >= 0 && (PPU_Scanline & 0xFF) - PPU_SpriteEvaluationTemp < (PPU_Spritex16 ? 16 : 8))
                                {
                                    PPU_OAMEvaluationObjectInRange = true;
                                    // if this sprite is within range.
                                    if (!SecondaryOAMFull)
                                    {
                                        if (!OamCorruptedOnOddCycle)
                                        {
                                            if (!SpriteEval_ReadOnly_PreRenderLine)
                                            {
                                                PPUOAMAddress++; // +1
                                            }
                                            OAM2Address++; // increment this for the next write to secondary OAM
                                        }
                                        if (!SecondaryOAMFull) // if secondary OAM is not full
                                        {
                                            OAM2Address &= 0x1F; // keep the secondary OAM address in-bounds
                                            if (OAM2Address == 0) // If we've overflowed the secondary OAM address
                                            {
                                                SecondaryOAMFull = true; // secondary OAM is now full.
                                            }
                                        }
                                        // Sprite zero hits actually have nothing to do with reading the object at OAM index 0. Rather, if an object is within range of the scanline on dot 66.
                                        // typically, the object processed on dot 66 is OAM[0], though it's possible using precisely timed writes to $2003 to have PPUOAMAddress start processing here from a different value.
                                        if (PPU_Dot == 66)
                                        {
                                            PPU_NextScanlineContainsSpriteZero = true; // this value will be transferred to PPU_PreviousScanlineContainsSpriteZero at the end of the scanline, and that variable is used in sp 0 hit detection.
                                        }
                                    }
                                    else // if secondary OAM is full, yet another object is on this scanline
                                    {
                                        PPUStatus_SpriteOverflow = true; // set the sprite overflow flag
                                    }
                                    if (!SpriteEval_ReadOnly_PreRenderLine)
                                    {
                                        SpriteEvaluationTick++; // increment the tick for next even ppu cycle.
                                    }
                                }
                                else
                                {
                                    if (PPU_Dot == 66)
                                    {
                                        PPU_NextScanlineContainsSpriteZero = false; // this value will be transferred to PPU_PreviousScanlineContainsSpriteZero at the end of the scanline, and that variable is used in sp 0 hit detection.
                                    }
                                    PPU_OAMEvaluationObjectInRange = false;
                                    if (!OamCorruptedOnOddCycle && !SpriteEval_ReadOnly_PreRenderLine)
                                    {
                                        if (SecondaryOAMFull)
                                        {
                                            if ((PPUOAMAddress & 0x3) == 3)
                                            {
                                                PPUOAMAddress++; // A real hardware bug.
                                            }
                                            else
                                            {
                                                PPUOAMAddress += 4; // +4
                                                PPUOAMAddress++; // A real hardware bug.
                                            }
                                        }
                                        else
                                        {
                                            PPUOAMAddress += 4; // +4
                                            PPUOAMAddress &= 0xFC; // also mask away the lower 2 bits
                                        }
                                    }
                                }
                            }
                            else // ticks 1, 2, or 3
                            {
                                if (SpriteEvaluationTick == 3) // tick 3: X position.
                                {
                                    PPU_OAMEvaluationObjectInRange = false;
                                    // OAM X coordinate.
                                    // This also runs the "vertical in range check", though typically the result doesn't matter.
                                    if (PPU_Scanline - PPU_SpriteEvaluationTemp >= 0 && PPU_Scanline - PPU_SpriteEvaluationTemp < (PPU_Spritex16 ? 16 : 8))
                                    {
                                        // if this sprite is within range.
                                        PPU_OAMEvaluationObjectInXRange = true;
                                        if (!SecondaryOAMFull)
                                        {
                                            if (!OamCorruptedOnOddCycle && !SpriteEval_ReadOnly_PreRenderLine)
                                            {
                                                PPUOAMAddress++; // +1
                                            }
                                        }
                                        else
                                        {
                                            if (!OamCorruptedOnOddCycle && !SpriteEval_ReadOnly_PreRenderLine)
                                            {
                                                PPUOAMAddress += 4; // +1 (In theory, this should be +4, though my experiments only reflect my consoles behavior if this is +1?)
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PPU_OAMEvaluationObjectInXRange = false;
                                        if (!SecondaryOAMFull)
                                        {
                                            if (!OamCorruptedOnOddCycle && !SpriteEval_ReadOnly_PreRenderLine)
                                            {
                                                PPUOAMAddress += 1; // +1 (In theory, this should be +4, though my experiments only reflect my consoles behavior if this is +1?)
                                                PPUOAMAddress &= 0xFC; // also mask away the lower 2 bits
                                            }
                                        }
                                    }
                                }
                                else // ticks 1 and 2 don't make any checks. Only increment the OAM address.
                                {
                                    if (!OamCorruptedOnOddCycle && !SpriteEval_ReadOnly_PreRenderLine)
                                    {
                                        PPUOAMAddress++; // +1
                                    }
                                }
                                SpriteEvaluationTick++; // increment the tick for next even ppu cycle.
                                SpriteEvaluationTick &= 3; // and reset the tick to 0 if it reaches 4.
                                if (!SecondaryOAMFull && !SpriteEval_ReadOnly_PreRenderLine) // if secondary OAM is not full
                                {
                                    OAM2Address++; // increment the secondary OAM address.
                                    OAM2Address &= 0x1F; // keep the secondary OAM address in-bounds
                                    if (OAM2Address == 0) // If we've overflowed the secondary OAM address
                                    {
                                        SecondaryOAMFull = true; // secondary OAM is now full.
                                    }
                                }
                            }
                            OamCorruptedOnOddCycle = false;

                            if (PPUOAMAddress < PreIncVal && PPUOAMAddress < 4) // If an overflow occured
                            {
                                OAMAddressOverflowedDuringSpriteEvaluation = true; // set this flag.
                            }
                            PPU_SpriteEvaluationTemp = OAM2READ; // When overflowed, the ppu reads instead of writing to OAM2. (Run this regardless of if OAM2 is full or not.)
                        }
                        else
                        {   // OAM Address Overflowerd During Sprite Evaluation
                            // fail to write to SecondaryOAM
                            // boo womp.

                            // also update the PPUOAMAddress.
                            if (!OamCorruptedOnOddCycle && !SpriteEval_ReadOnly_PreRenderLine)
                            {
                                PPUOAMAddress += 4; // +4
                                PPUOAMAddress &= 0xFC; // also mask away the lower 2 bits
                            }
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // When overflowed, the ppu reads instead of writing to OAM2.
                        }
                        if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant && !PPU_OAMEvaluationCorruptionOddCycle) // if we just disabled rendering mid OAM evaluation, the address is incremented yet again.
                        {
                            PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                            PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                            PPU_PendingOAMCorruption = true;

                            if ((OAM2Address & 3) != 0 && !OAMAddressOverflowedDuringSpriteEvaluation && !SpriteEval_ReadOnly_PreRenderLine)
                            {
                                OAM2Address &= 0xFC;
                                OAM2Address += 4;
                            }
                            if (PPUClock == 0 || PPUClock == 3)
                            {
                                PPU_OAMCorruptionIndex = (byte)(OAM2Address); // this value will be used when rendering is re-enabled and the corruption occurs
                            }
                            if (PPUClock == 1 || PPUClock == 2)
                            {
                                PPU_OAMCorruptionIndex = (byte)(OAM2Address); // this value will be used when rendering is re-enabled and the corruption occurs
                            }
                            if (PPU_Dot == 256)
                            {
                                PPU_OAMCorruptionIndex = OamCorruptedOnOddCycle ? (byte)0 : (byte)1; //I have no idea.
                            }

                        }
                        PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                    }
                }

            }
            else if (PPU_Dot >= 257 && PPU_Dot <= 320) // this also happens on the pre-render line.
            {
                PPU_CurrentScanlineContainsSpriteZero = PPU_NextScanlineContainsSpriteZero;

                if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed))
                {
                    PPUOAMAddress = 0; // this is reset during every one of these cycles, 257 through 320
                }
                if (PPU_Dot == 257)
                {
                    // reset these flags for this section.
                    OAM2Address = 0;
                    SpriteEvaluationTick = 0;
                }

                if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank && (PPUClock == 0 || PPUClock == 3))
                {
                    PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                    PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                    PPU_PendingOAMCorruption = true;
                    PPU_OAMCorruptionIndex = OAM2Address; // this value will be used when rendering is re-enabled and the corruption occurs
                }


                switch (SpriteEvaluationTick)
                {
                    // So each scanline can only have up to 8 sprites.
                    // Each sprite has a Y position, Pattern, Attributes, and X position.
                    // So there's an 8-index-long array for each of those.
                    // Each index in the array is for a different sprite.

                    // Sprites also have 2 "bit plane" shift registers.
                    // These are the 8 pixels to draw for the object on this scanline.
                    // Again, there are 8 objects, so there are 2 8-index-long arrays of bit planes.

                    // each case is a different ppu cycle.
                    // case 0.
                    // next cycle, case 1.
                    // next cycle, case 2, and so on.
                    // case 7 then leads back to case 0.


                    case 0: // Y position         dot 257, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed)) // if rendering has been enabled for at least 1 cycle.
                        {
                            // set this object's Y position in the array
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpriteYposition[OAM2Address / 4] = PPU_SpriteEvaluationTemp;
                            PPU_Render_ShiftRegistersAndBitPlanes(); // Dummy Nametable Fetch
                        }
                        OAM2Address++; // and increment the Secondary OAM address for next cycle
                        break;
                    case 1: // Pattern            dot 258, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed)) // if rendering has been enabled for at least 1 cycle.
                        {
                            // set this object's pattern in the array
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpritePattern[OAM2Address / 4] = PPU_SpriteEvaluationTemp;
                            PPU_Render_ShiftRegistersAndBitPlanes(); // Dummy Nametable Fetch
                        }
                        OAM2Address++; // and increment the Secondary OAM address for next cycle
                        break;
                    case 2: // Attribute          dot 259, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed)) // if rendering has been enabled for at least 1 cycle.
                        {
                            // set this object's attribute in the array
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpriteAttribute[OAM2Address / 4] = PPU_SpriteEvaluationTemp;
                            PPU_Render_ShiftRegistersAndBitPlanes(); // Dummy Nametable Fetch
                        }
                        OAM2Address++; // and increment the Secondary OAM address for next cycle
                        break;
                    case 3: // X position         dot 260, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed)) // if rendering has been enabled for at least 1 cycle.
                        {
                            // set this object's X position in the array
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpriteXposition[OAM2Address / 4] = PPU_SpriteEvaluationTemp;
                            PPU_Render_ShiftRegistersAndBitPlanes(); // Dummy Nametable Fetch
                        }
                        // notably, the secondary OAM address does not get incremented until case 7
                        break;
                    case 4: // X position (again) dot 261, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed)) // if rendering has been enabled for at least 1 cycle.
                        {
                            // set this object's X position in the array... again.
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpriteXposition[OAM2Address / 4] = PPU_SpriteEvaluationTemp;
                            // But also: Find the PPU address of this sprite's graphical data inside the Pattern Tables.
                            PPU_SpriteEvaluation_GetSpriteAddress((byte)(OAM2Address / 4));
                        }

                        break;
                    case 5: // X position (again)  dot 262, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed)) // if rendering has been enabled for at least 1 cycle.
                        {
                            // set this object's X position in the array... again.
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpriteXposition[OAM2Address / 4] = PPU_SpriteEvaluationTemp;
                            // but also: set up the bit plane shift register.
                            PPU_SpritePatternL = FetchPPU(PPU_AddressBus);
                            if (((PPU_SpriteAttribute[OAM2Address / 4] >> 6) & 1) == 1) // Attributes are set up to flip X
                            {
                                PPU_SpritePatternL = Flip(PPU_SpritePatternL);
                            }
                            PPU_SpriteShiftRegisterL[OAM2Address / 4] = PPU_SpritePatternL;
                        }


                        // in-range check. (The pre-render line ends up checking scanline 5 due to the `& 0xFF`.
                        if (!((PPU_Scanline & 0xFF) - PPU_SpriteYposition[OAM2Address / 4] >= 0 && (PPU_Scanline & 0xFF) - PPU_SpriteYposition[OAM2Address / 4] < (PPU_Spritex16 ? 16 : 8)))
                        {
                            PPU_SpriteShiftRegisterL[OAM2Address / 4] = 0; // clear the value in this shift register if this object isn't in range.
                        }

                        break;
                    case 6: // X position (again)  dot 263, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed))
                        {
                            // set this object's X position in the array... again.
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpriteXposition[OAM2Address / 4] = PPU_SpriteEvaluationTemp;
                            // but also: add 8 to the PPU address. The other bit plane is 8 addresses away.
                            PPU_SpriteEvaluation_GetSpriteAddress((byte)(OAM2Address / 4)); // we need to recalculate this. Slow, but accurate. (TODO: Can we test for this with a well timed write to $2000?)
                            PPU_AddressBus += 8; // at this point, the address couldn't possibly overflow, so there's no need to worry about that.
                        }

                        break;

                    case 7: // X position (again)  dot 264, (+8), (+16) ...
                        if ((PPU_Mask_ShowBackground_Delayed || PPU_Mask_ShowSprites_Delayed))
                        {
                            // set this object's X position in the array... again.
                            PPU_SpriteEvaluationTemp = OAM2[OAM2Address]; // Updating PPU_SpriteEvaluationTemp so reading from $2004 works properly.
                            PPU_SpriteXposition[OAM2Address / 4] = PPU_SpriteEvaluationTemp; // read X pos again
                            // but also: set up the second bit plane
                            PPU_SpritePatternH = FetchPPU(PPU_AddressBus);
                            if (((PPU_SpriteAttribute[OAM2Address / 4] >> 6) & 1) == 1) // Attributes are set up to flip X
                            {
                                PPU_SpritePatternH = Flip(PPU_SpritePatternH);
                            }
                            PPU_SpriteShiftRegisterH[OAM2Address / 4] = PPU_SpritePatternH;
                        }

                        // in-range check. (The pre-render line ends up checking scanline 5 due to the `& 0xFF`.
                        if (!((PPU_Scanline & 0xFF) - PPU_SpriteYposition[OAM2Address / 4] >= 0 && (PPU_Scanline & 0xFF) - PPU_SpriteYposition[OAM2Address / 4] < (PPU_Spritex16 ? 16 : 8)))
                        {
                            PPU_SpriteShiftRegisterH[OAM2Address / 4] = 0; // clear the value in this shift register if this object isn't in range.
                        }

                        OAM2Address++; // and increment the Secondary OAM address for next cycle

                        break;
                }
                OAM2Address &= 0x1F; // keep the secondary OAM address in-bounds

                SpriteEvaluationTick++; // increment the tick, so next cycle uses the following case in the switch statement
                SpriteEvaluationTick &= 7; // and reset at 8

                if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank && (PPUClock == 1 || PPUClock == 2))
                {
                    PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                    PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                    PPU_PendingOAMCorruption = true;
                    PPU_OAMCorruptionIndex = OAM2Address; // this value will be used when rendering is re-enabled and the corruption occurs
                }

            }
            else
            {
                // cycles 320 to 340
                if (PPU_OAMCorruptionRenderingDisabledOutOfVBlank || PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant)
                {
                    PPU_OAMCorruptionRenderingDisabledOutOfVBlank = false;
                    PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = false;
                    PPU_PendingOAMCorruption = true;
                    PPU_OAMCorruptionIndex = OAM2Address; // this value will be used when rendering is re-enabled and the corruption occurs
                }

                if (PPU_Dot == 339)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if ((PPU_Mask_ShowSprites || PPU_Mask_ShowBackground))
                        {
                            PPU_SpriteShifterCounter[i] = PPU_SpriteXposition[i];
                        }
                        else
                        {
                            PPU_SpriteShifterCounter[i] = 0;
                        }
                    }
                }
            }
            // and that's all for sprite evaluation!
        }

        void PPU_SpriteEvaluation_GetSpriteAddress(byte SecondOAMSlot)
        {
            // PPU_PatternSelect_Sprites is set by writing to bit 3 of address $2000

            if (!PPU_Spritex16) //8x8 sprites
            {
                // The address is $0000 or $1000 depending on the nametable.
                // plus the pattern value from OAM * 16
                // plus the number of scanlines from the top of the object.
                // if the attributes are set to flip Y, it's 7 - the number of scanlines from the top of the object.
                if (((PPU_SpriteAttribute[SecondOAMSlot] >> 7) & 1) == 0) // Attributes are not set up to flip Y
                {
                    PPU_AddressBus = (ushort)((PPU_PatternSelect_Sprites ? 0x1000 : 0) + (PPU_SpritePattern[SecondOAMSlot] << 4) + ((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot]));
                }
                else  // Attributes are set up to flip Y
                {
                    PPU_AddressBus = (ushort)((PPU_PatternSelect_Sprites ? 0x1000 : 0) + (PPU_SpritePattern[SecondOAMSlot] << 4) + ((7 - ((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot])) & 7));
                }
            }
            else //8x16 sprites
            {
                // In 8x16 mode, instead of using PPU_PatternSelect_Sprites to determine which pattern table to fetch data from...
                // these sprites instead use bit 0 of the object's pattern information from OAM.

                // The address is $0000 or $1000 depending on the nametable.
                // plus (the pattern value from OAM, clearing bit 0) * 16
                // plus the number of scanlines from the top of the object.
                // if the attributes are set to flip Y, it's 7 - the number of scanlines from the top of the object.

                // if we're drawing the bottom half of the sprite, add 16.
                if (((PPU_SpriteAttribute[SecondOAMSlot] >> 7) & 1) == 0) // Attributes are not set up to flip Y
                {
                    if ((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot] < 8)
                    {
                        PPU_AddressBus = (ushort)((((PPU_SpritePattern[SecondOAMSlot] & 1) == 1) ? 0x1000 : 0) | ((PPU_SpritePattern[SecondOAMSlot] & 0xFE) << 4) + ((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot]));
                    }
                    else
                    {
                        PPU_AddressBus = (ushort)((((PPU_SpritePattern[SecondOAMSlot] & 1) == 1) ? 0x1000 : 0) | (((PPU_SpritePattern[SecondOAMSlot] & 0xFE) << 4) + 16) + (((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot]) & 7));
                    }
                }
                else // Attributes are set up to flip Y
                {
                    if ((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot] < 8)
                    {
                        PPU_AddressBus = (ushort)((((PPU_SpritePattern[SecondOAMSlot] & 1) == 1) ? 0x1000 : 0) | (((PPU_SpritePattern[SecondOAMSlot] & 0xFE) << 4) + 16) - (((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot]) & 7) + 7);
                    }
                    else
                    {
                        PPU_AddressBus = (ushort)((((PPU_SpritePattern[SecondOAMSlot] & 1) == 1) ? 0x1000 : 0) | (((PPU_SpritePattern[SecondOAMSlot] & 0xFE) << 4) + 7) - (((PPU_Scanline & 0xFF) - PPU_SpriteYposition[SecondOAMSlot]) & 7));
                    }
                }
            }
        }





        void PPU_Render_CalculatePixel(bool borders)
        {
            // dots 1 through 256
            if (PPU_Dot > 256)
            {
                borders = true;
            }
            if (PPU_Dot <= 256 || borders)
            {
                // there are 8 palettes in the PPU
                // 4 are for the background, and the other 4 are for sprites.
                byte Palette = 0;
                // each of these palettes have 4 colors
                byte Color = 0;
                if (!borders)
                {
                    if (PPU_Mask_ShowBackground && (PPU_Dot > 8 || PPU_Mask_8PxShowBackground)) // if rendering is enables for this pixel
                    {
                        byte col0 = (byte)(((PPU_BackgroundPatternShiftRegisterL >> (15 - PPU_FineXScroll))) & 1); // take the bit from the shift register for the pattern low bit plane
                        byte col1 = (byte)(((PPU_BackgroundPatternShiftRegisterH >> (15 - PPU_FineXScroll))) & 1); // take the bit from the shift register for the pattern high bit plane
                        Color = (byte)((col1 << 1) | col0);

                        byte pal0 = (byte)(((PPU_BackgroundAttributeShiftRegisterL) >> (7 - PPU_FineXScroll)) & 1); // take the bit from the shift register for the attribute low bit plane
                        byte pal1 = (byte)(((PPU_BackgroundAttributeShiftRegisterH) >> (7 - PPU_FineXScroll)) & 1); // take the bit from the shift register for the attribute high bit plane
                        Palette = (byte)((pal1 << 1) | pal0);

                        if (Color == 0 && Palette != 0) // color 0 of all palettes are mirrors of color 0 of palette 0
                        {
                            Palette = 0;
                        }
                    }
                }
                // pretty much the same thing, but for sprites instead of background
                byte SpritePalette = 0;
                byte SpriteColor = 0;
                bool SpritePriority = false; // if set, this sprite will be in front of background tiles. Otherwise, it will only take priority if the background is using color 0.
                if (!borders)
                {
                    if (PPU_Mask_ShowSprites && (PPU_Dot > 8 || PPU_Mask_8PxShowSprites))
                    {
                        int i = 0;

                        // check all 8 objects in secondary OAM
                        while (i < 8)
                        {
                            if (PPU_SpriteShifterCounter[i] == 0 || SkippedPreRenderDot341) // if the shifter counter == 0 (the shifter counter is decremented each ppu cycle)
                            {
                                bool SpixelL = ((PPU_SpriteShiftRegisterL[i]) & 0x80) != 0; // take the bit from the shift register for the pattern low bit plane
                                bool SpixelH = ((PPU_SpriteShiftRegisterH[i]) & 0x80) != 0; // take the bit from the shift register for the pattern high bit plane
                                SpriteColor = 0;
                                if (SpixelL) { SpriteColor = 1; }
                                if (SpixelH) { SpriteColor |= 2; }

                                SpritePalette = (byte)((PPU_SpriteAttribute[i] & 0x03) | 0x04); // read the palette from secondary OAM attributes.
                                SpritePriority = ((PPU_SpriteAttribute[i] >> 5) & 1) == 0;      // read the priority from secondary OAM attributes.

                            }
                            else // if no objects are in range of this pixel...
                            {
                                i++; // try the next one
                                continue;
                            }

                            if (SpriteColor != 0) // if we found an object, exit the loop. This means, objects earlier in secondary OAM hive higher priority over sprites later in secondary OAM
                            {
                                break;
                            }

                            i++; // This pixel wasn't a part of the previous object. Try the next slot in secondary oam.
                        }

                        // if we hit sprite zero and both rendering background and sprites are enabled...
                        if (PPU_CanDetectSpriteZeroHit && i == 0 && PPU_CurrentScanlineContainsSpriteZero && PPU_Mask_ShowBackground && PPU_Mask_ShowSprites)
                        {
                            if (Color != 0 && SpriteColor != 0) // if both the background and sprites are visible on this pixel
                            {
                                if ((PPU_Mask_8PxShowSprites || PPU_Dot > 8) && PPU_Dot < 256) // and if this isn't on pixel 256, or in the first 8 pixels being masked away from the nametable, if that setting is enabled...
                                {
                                    PPUStatus_SpriteZeroHit = true; // we did it! sprite zero hit achieved.
                                    PPU_CanDetectSpriteZeroHit = false; // another sprite zero hit cannot occur until the end of next vblank.
                                    if (Logging) // and for some debug logging...
                                    {
                                        string S = DebugLog.ToString(); // let's add text to the current line letting me know a sprite zero hit occured, and on which dot
                                        if (S.Length > 0)
                                        {
                                            S = S.Substring(0, S.Length - 2); // trim off \n
                                            DebugLog = new StringBuilder(S);
                                            DebugLog.Append(" ! Sprite Zero Hit ! (Dot " + PPU_Dot + ")\r\n");

                                        }
                                    }
                                }
                            }
                        }

                        // which do we draw, the background or the sprite?
                        if (Color == 0 && SpriteColor != 0) // Well, if the background was using color 0, and the sprite wasn't,  always draw the sprite.
                        {
                            Color = SpriteColor; // I'm just reusing this background color variable.
                            Palette = SpritePalette;       // I'm also just reusing the background palette variable.
                        }
                        else if (SpriteColor != 0) // the background color isn't zero...
                        {
                            if (SpritePriority) // if the sprite has priority, always draw the sprite.
                            {
                                Color = SpriteColor; // I'm just reusing this cackground color variable.
                                Palette = SpritePalette; // I'm also just reusing the background palette variable.
                            }
                        }
                    }
                }
                if ((PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && PPU_Scanline < 240) // if rendering is enabled...
                {
                    PaletteRAMAddress = (byte)(Palette << 2 | Color); // the Palette RAM address is determined by the palette and color we found.
                }
                else
                {
                    // rendering is disabled...
                    if ((PPU_ReadWriteAddress & 0x3F1F) >= 0x3F00) // if v points to palette ram:
                    {
                        PaletteRAMAddress = (byte)(PPU_ReadWriteAddress & 0x1F); // The palette RAM address is simply wherever the v register is. (bitwise and with $1F due to palette RAM mirroring)
                        if ((PaletteRAMAddress & 3) == 0)
                        {
                            PaletteRAMAddress &= 0x0F; // the transparent colors for sprites and backgrounds are shared.
                        }
                    }
                    else
                    {
                        // EXT Pins
                        PaletteRAMAddress = 0; // I'm not really emulating the EXT pins, and as far as I'm aware they aren't used in any games, official or homebrew.
                        // This is typically why the background color is using Palette[0] when rendering is disabled.
                    }
                }

                if (PPU_PaletteCorruptionRenderingDisabledOutOfVBlank || PPU_VRegisterChangedOutOfVBlank)
                {
                    PPU_VRegisterChangedOutOfVBlank = false;
                    PPU_PaletteCorruptionRenderingDisabledOutOfVBlank = false;
                    // PPU palette corruption!

                    CorruptPalettes(Color, Palette);
                    // This corruption also results in a single discolored pixel, and this occurs on all alignments.
                    // I'm not entirely sure how this works, and I think it's the *next* pixel that gets corrupt? More research needed.

                }

                DotColor = (byte)((PaletteRAM[0x00 | PaletteRAMAddress]) & 0x3F); // Get the color by reading from Palette RAM

                // though this is actually drawn to the screen 2 ppu cycles from now.
            }
        }

        void CorruptPalettes(byte Color, byte Palette)
        {
            // Depending on the index into a color palette being used to select a color being drawn when rendering was disabled during a nametable fetch on a visible pixel with the PPU V Register (bitwise AND with $3FFF) being >= $3C00...
            // Palettes get "corrupted" with a specific pattern.
            // This pattern is determined by:
            // The lowest nybble of the PPU's V register,
            // The color index into the palette,
            // and if this is using a sprite palette. (TODO: emulate this part)

            // All of this was determined by observations with a custom test cart.
            // It is entirely possible that the logic defined in this functions is incorrect, or possibly there are more factors at play.
            // As far as I can tell though, this is "good enough" emulation of palette corruption.

            if ((CPUClock & 3) != 2)
            {
                // this behavior occurs on other alignments, but seems consistent on alignment 2, and very hit or miss on other alignments.
                // Currently, I'm only emulating this on alignment 2, but I'll probably change this in the future.
                return;
            }


            byte[] CorruptedPalette = new byte[PaletteRAM.Length];
            for (int i = 0; i < CorruptedPalette.Length; i++)
            {
                CorruptedPalette[i] = PaletteRAM[i];
            }

            switch (Color)
            {
                case 0:
                    // simply take the low nybble from the V register. that's the color to corrupt.
                    CorruptedPalette[PPU_ReadWriteAddress & 0xF] = (byte)((PaletteRAM[0] & PaletteRAM[PPU_ReadWriteAddress & 0xC]) | (PaletteRAM[0] & PaletteRAM[PPU_ReadWriteAddress & 0xF]) | (PaletteRAM[PPU_ReadWriteAddress & 0xC] & PaletteRAM[PPU_ReadWriteAddress & 0xF]));
                    // TODO: Nybble 7 can corrupt color F. It's inconsistent though, so I'll need to circle back to this.

                    break;
                case 1:

                    // To be honest, I'm not sure what's going on, so forgive the lack of comments.
                    // There's almost a pattern, but again- unsure on why this is how it behaves.
                    // and also it's likely this isn't entirely accurate, either due to mistyping something, or not enough research.

                    switch (PPU_ReadWriteAddress & 0xF)
                    {
                        case 0:
                            CorruptedPalette[0x0] = (byte)((PaletteRAM[0x1] & PaletteRAM[0xD]) | PaletteRAM[0x0]);
                            CorruptedPalette[0x4] = PaletteRAM[0x5];
                            CorruptedPalette[0x8] = PaletteRAM[0x9];
                            CorruptedPalette[0xC] = PaletteRAM[0xD];
                            break;
                        case 1:
                            break;
                        case 2:
                            CorruptedPalette[0x2] = (byte)((PaletteRAM[0x2] | PaletteRAM[0xD]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x3] = (byte)((PaletteRAM[0x1] | PaletteRAM[0x2]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x6] = (byte)((PaletteRAM[0x6] | PaletteRAM[0x5]) & PaletteRAM[0x7]);
                            CorruptedPalette[0xA] = (byte)((PaletteRAM[0xA] | PaletteRAM[0x9]) & PaletteRAM[0xB]);
                            CorruptedPalette[0xE] = PaletteRAM[0xD];
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                        case 3:
                            CorruptedPalette[0x3] &= (byte)(PaletteRAM[0x1] | PaletteRAM[0xD]);
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                        case 4:
                            CorruptedPalette[0x0] = PaletteRAM[0x1];
                            CorruptedPalette[0x4] = (byte)((PaletteRAM[0x5] & PaletteRAM[0xD]) | PaletteRAM[0x4]);
                            CorruptedPalette[0x8] = PaletteRAM[0x9];
                            CorruptedPalette[0xC] = PaletteRAM[0xD];
                            break;
                        case 5:
                            break;
                        case 6:
                            CorruptedPalette[0x2] = (byte)((PaletteRAM[0x2] | PaletteRAM[0x1]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x6] = (byte)((PaletteRAM[0x6] | PaletteRAM[0x7]) & PaletteRAM[0xD]);
                            CorruptedPalette[0x7] = (byte)((PaletteRAM[0x7] | PaletteRAM[0x6]) & PaletteRAM[0x5]);
                            CorruptedPalette[0xA] = (byte)((PaletteRAM[0xA] | PaletteRAM[0x9]) & PaletteRAM[0xB]);
                            CorruptedPalette[0xE] = PaletteRAM[0xD];
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                        case 7:
                            CorruptedPalette[0x7] &= (byte)(PaletteRAM[0x5] | PaletteRAM[0xD]);
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                        case 8:
                            CorruptedPalette[0x0] = PaletteRAM[0x1];
                            CorruptedPalette[0x4] = PaletteRAM[0x5];
                            CorruptedPalette[0x8] = (byte)((PaletteRAM[0x9] & PaletteRAM[0xD]) | PaletteRAM[0x8]);
                            CorruptedPalette[0xC] = PaletteRAM[0xD];
                            break;
                        case 9:
                            break;
                        case 0xA:
                            CorruptedPalette[0x2] = (byte)((PaletteRAM[0x2] | PaletteRAM[0x1]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x6] = (byte)((PaletteRAM[0x6] | PaletteRAM[0xD]) & PaletteRAM[0x7]);
                            CorruptedPalette[0xA] = (byte)((PaletteRAM[0xB] | PaletteRAM[0xD]) & PaletteRAM[0xA]);
                            CorruptedPalette[0xB] = (byte)((PaletteRAM[0x9] | PaletteRAM[0xA]) & PaletteRAM[0xB]);
                            CorruptedPalette[0xE] = PaletteRAM[0xD];
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                        case 0xB:
                            CorruptedPalette[0xB] &= (byte)(PaletteRAM[0x9] | PaletteRAM[0xD]);
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                        case 0xC:
                            CorruptedPalette[0x0] = PaletteRAM[0x1];
                            CorruptedPalette[0x4] = PaletteRAM[0x5];
                            CorruptedPalette[0x8] = PaletteRAM[0x9];
                            CorruptedPalette[0xC] = PaletteRAM[0xD];
                            break;
                        case 0xD:
                            break;
                        case 0xE:
                            CorruptedPalette[0x2] = (byte)((PaletteRAM[0x2] | PaletteRAM[0x1]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x6] = (byte)((PaletteRAM[0x6] | PaletteRAM[0xD]) & PaletteRAM[0x7]);
                            CorruptedPalette[0xA] = (byte)((PaletteRAM[0xA] | PaletteRAM[0x9]) & PaletteRAM[0xB]);
                            CorruptedPalette[0xE] = PaletteRAM[0xD];
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                        case 0xF:
                            CorruptedPalette[0xF] = PaletteRAM[0xD];
                            break;
                    }


                    // In some tests with case A, bit 3 ($08) of color 3 can remove bit 2 ($04) from the value of color 0 for the purposes of the bitwise AND. It's inconsistent though.


                    break;
                case 2:

                    // To be honest, I'm not sure what's going on, so forgive the lack of comments.
                    // There's almost a pattern, but again- unsure on why this is how it behaves.
                    // and also it's likely this isn't entirely accurate, either due to mistyping something, or not enough research.

                    switch (PPU_ReadWriteAddress & 0xF)
                    {
                        case 0:
                            CorruptedPalette[0x0] = (byte)(PaletteRAM[0x0] | (PaletteRAM[0x2] & PaletteRAM[0xE]));
                            CorruptedPalette[0x4] = PaletteRAM[0x6];
                            CorruptedPalette[0x8] = PaletteRAM[0xA];
                            CorruptedPalette[0xC] = PaletteRAM[0xE];
                            break;
                        case 1:
                            CorruptedPalette[0x1] = (byte)((PaletteRAM[0x2] | PaletteRAM[0x1] | PaletteRAM[0xE]) & (PaletteRAM[0x3] | PaletteRAM[0xE]));
                            CorruptedPalette[0x3] = (byte)((PaletteRAM[0x2] | PaletteRAM[0xE] | 0x3C) & PaletteRAM[0x3]);
                            CorruptedPalette[0x5] = (byte)((PaletteRAM[0x6] | PaletteRAM[0x7]) & PaletteRAM[0x5]);
                            CorruptedPalette[0x9] = (byte)((PaletteRAM[0xA] | PaletteRAM[0xB]) & PaletteRAM[0x9]);
                            CorruptedPalette[0xD] = PaletteRAM[0xE];
                            CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                        case 2:
                            break;
                        case 3:
                            CorruptedPalette[0x3] &= (byte)(PaletteRAM[0x2] | PaletteRAM[0xE]);
                            CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                        case 4:
                            CorruptedPalette[0x0] = PaletteRAM[0x2];
                            CorruptedPalette[0x4] = (byte)(PaletteRAM[0x4] | (PaletteRAM[0x6] & PaletteRAM[0xE]));
                            CorruptedPalette[0x8] = PaletteRAM[0xA];
                            CorruptedPalette[0xC] = PaletteRAM[0xE];
                            break;
                        case 5:
                            CorruptedPalette[0x1] = (byte)((PaletteRAM[0x2] | PaletteRAM[0x1]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x5] = (byte)((PaletteRAM[0xE] | PaletteRAM[0x6]) & PaletteRAM[0x5]);
                            CorruptedPalette[0x7] = (byte)((PaletteRAM[0xE] | PaletteRAM[0x6]) & PaletteRAM[0x7]);
                            CorruptedPalette[0xD] = PaletteRAM[0xE];
                            CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                        case 6:
                            break;
                        case 7:
                            CorruptedPalette[0x7] &= (byte)(PaletteRAM[0x6] | PaletteRAM[0xE]);
                            //CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                        case 8:
                            CorruptedPalette[0x0] = PaletteRAM[0x2];
                            CorruptedPalette[0x4] = PaletteRAM[0x6];
                            CorruptedPalette[0x8] = (byte)(PaletteRAM[0x8] | (PaletteRAM[0xA] & PaletteRAM[0xE]));
                            CorruptedPalette[0xC] = PaletteRAM[0xE];
                            break;
                        case 9:
                            CorruptedPalette[0x1] = (byte)((PaletteRAM[0x2] | PaletteRAM[0x1]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x5] = (byte)((PaletteRAM[0x6] | PaletteRAM[0x5]) & PaletteRAM[0x7]);
                            CorruptedPalette[0x9] = (byte)((PaletteRAM[0xE] | PaletteRAM[0xA] | 0x01) & PaletteRAM[0x9]);
                            CorruptedPalette[0xB] = (byte)((PaletteRAM[0xE] | PaletteRAM[0xA] | 0x31) & PaletteRAM[0xB]);
                            CorruptedPalette[0xD] = PaletteRAM[0xE];
                            CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                        case 0xA:
                            break;
                        case 0xB:
                            CorruptedPalette[0xB] &= (byte)(PaletteRAM[0xA] | PaletteRAM[0xE]);
                            CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                        case 0xC:
                            CorruptedPalette[0x0] = PaletteRAM[0x2];
                            CorruptedPalette[0x4] = PaletteRAM[0x6];
                            CorruptedPalette[0x8] = PaletteRAM[0xA];
                            CorruptedPalette[0xC] = PaletteRAM[0xE];
                            break;
                        case 0xD:
                            CorruptedPalette[0x1] = (byte)((PaletteRAM[0x2] | PaletteRAM[0x1]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x5] = (byte)((PaletteRAM[0x6] | PaletteRAM[0x5]) & PaletteRAM[0x7]);
                            CorruptedPalette[0x9] = (byte)((PaletteRAM[0xA] | PaletteRAM[0x9]) & PaletteRAM[0xB]);
                            CorruptedPalette[0xD] = PaletteRAM[0xE];
                            CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                        case 0xE:
                            break;
                        case 0xF:
                            CorruptedPalette[0xF] = PaletteRAM[0xE];
                            break;
                    }


                    break;
                case 3:

                    // To be honest, I'm not sure what's going on, so forgive the lack of comments.
                    // There's almost a pattern, but again- unsure on why this is how it behaves.
                    // and also it's likely this isn't entirely accurate, either due to mistyping something, or not enough research.

                    switch (PPU_ReadWriteAddress & 0xF)
                    {
                        case 0:
                            CorruptedPalette[0x0] = (byte)((PaletteRAM[0x3] | (PaletteRAM[0xF] & PaletteRAM[0x0])));
                            CorruptedPalette[0x4] &= PaletteRAM[0x7];
                            CorruptedPalette[0x8] &= (byte)(PaletteRAM[0x9] | PaletteRAM[0xA] | PaletteRAM[0xB] | PaletteRAM[0xF] | 0x22); // magic number... Probably a temperature thing? I've seen 02, 22, 2C, or 2E
                            CorruptedPalette[0xC] = PaletteRAM[0xF];
                            break;
                        case 1:
                            CorruptedPalette[0x1] = (byte)((PaletteRAM[0x1] | PaletteRAM[0xF]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x5] = PaletteRAM[0x7];
                            CorruptedPalette[0x9] = PaletteRAM[0xB];
                            CorruptedPalette[0xD] = PaletteRAM[0xF];
                            break;
                        case 2:
                            CorruptedPalette[0x2] = (byte)((PaletteRAM[0x3] | PaletteRAM[0xF]) & PaletteRAM[0x3]);
                            CorruptedPalette[0x6] = PaletteRAM[0x7];
                            CorruptedPalette[0xA] = PaletteRAM[0xB];
                            CorruptedPalette[0xE] = PaletteRAM[0xF];
                            break;
                        case 3:
                            break;
                        case 4:
                            CorruptedPalette[0x0] &= (byte)(((PaletteRAM[0xF] ^ 0xFF)) | PaletteRAM[0x1] | PaletteRAM[0x2] | PaletteRAM[0x3] | 0x7); // magic number... I've only seen it as 07 though.
                            CorruptedPalette[0x4] &= (byte)(PaletteRAM[0x7] | PaletteRAM[0xF]);
                            CorruptedPalette[0x8] &= (byte)(PaletteRAM[0xB] | PaletteRAM[0xF] | (PaletteRAM[0xC] ^ 0xFF));
                            CorruptedPalette[0xC] = (byte)((PaletteRAM[0x7] & PaletteRAM[0xF]) | PaletteRAM[0xC]);
                            break;
                        case 5:
                            CorruptedPalette[0x1] = PaletteRAM[0x3];
                            CorruptedPalette[0x5] = (byte)((PaletteRAM[0x5] | PaletteRAM[0xF]) & PaletteRAM[0x7]);
                            CorruptedPalette[0x9] = PaletteRAM[0xB];
                            CorruptedPalette[0xD] = PaletteRAM[0xF];
                            break;
                        case 6:
                            CorruptedPalette[0x2] = PaletteRAM[0x3];
                            CorruptedPalette[0x6] = (byte)((PaletteRAM[0x6] | PaletteRAM[0xF]) & PaletteRAM[0x7]);
                            CorruptedPalette[0xA] = PaletteRAM[0xB];
                            CorruptedPalette[0xE] = PaletteRAM[0xF];
                            break;
                        case 7:
                            break;
                        case 8:
                            CorruptedPalette[0x0] &= (byte)(((PaletteRAM[0xF] ^ 0xFF)) | PaletteRAM[0x1] | PaletteRAM[0x2] | PaletteRAM[0x3] | 0x23); // magic number... I've only seen it as 23 though.
                            CorruptedPalette[0x4] = (byte)(PaletteRAM[0x7]);
                            CorruptedPalette[0x8] &= (byte)(PaletteRAM[0xB] | PaletteRAM[0xF] | (PaletteRAM[0xC] ^ 0xFF));
                            CorruptedPalette[0xC] = (byte)((PaletteRAM[0xB] & PaletteRAM[0xF]) | PaletteRAM[0xC]);
                            break;
                        case 9:
                            CorruptedPalette[0x1] = PaletteRAM[0x3];
                            CorruptedPalette[0x5] = PaletteRAM[0x7];
                            CorruptedPalette[0x9] = (byte)((PaletteRAM[0x9] | PaletteRAM[0xF]) & PaletteRAM[0xB]);
                            CorruptedPalette[0xD] = PaletteRAM[0xF];
                            break;
                        case 0xA:
                            CorruptedPalette[0x2] = PaletteRAM[0x3];
                            CorruptedPalette[0x6] = PaletteRAM[0x7];
                            CorruptedPalette[0xA] = (byte)((PaletteRAM[0xA] | PaletteRAM[0xF]) & PaletteRAM[0xB]);
                            CorruptedPalette[0xE] = PaletteRAM[0xF];
                            break;
                        case 0xB:
                            break;
                        case 0xC:
                            CorruptedPalette[0x0] &= (byte)(((PaletteRAM[0xF] ^ 0xFF)) | PaletteRAM[0x1] | PaletteRAM[0x2] | PaletteRAM[0x3] | 0x37); // magic number... I've only seen it as 23 though.
                            CorruptedPalette[0x4] = PaletteRAM[0x7];
                            CorruptedPalette[0x8] &= (byte)(PaletteRAM[0xB] | 0x2F); // Magic number. I've seen 2F and 2E
                            CorruptedPalette[0xC] = PaletteRAM[0xF];
                            break;
                        case 0xD:
                            CorruptedPalette[0x1] = PaletteRAM[0x3];
                            CorruptedPalette[0x5] = PaletteRAM[0x7];
                            CorruptedPalette[0x9] = PaletteRAM[0xB];
                            CorruptedPalette[0xD] = PaletteRAM[0xF];
                            break;
                        case 0xE:
                            CorruptedPalette[0x2] = PaletteRAM[0x3];
                            CorruptedPalette[0x6] = PaletteRAM[0x7];
                            CorruptedPalette[0xA] = PaletteRAM[0xB];
                            CorruptedPalette[0xE] = PaletteRAM[0xF];
                            break;
                        case 0xF:
                            break;
                    }

                    break;


            }
            for (int i = 0; i < CorruptedPalette.Length; i++)
            {
                PaletteRAM[i] = CorruptedPalette[i];
            }


        }





        byte PPU_RenderTemp; // a variable used in the following function to store information between ppu cycles.
        bool PPU_Commit_NametableFetch;
        bool PPU_Commit_AttributeFetch;
        bool PPU_Commit_PatternLowFetch;
        bool PPU_Commit_PatternHighFetch;


        void PPU_Render_ShiftRegistersAndBitPlanes()
        {
            byte cycleTick; // for the switch statement below, this checks which case to run on a given ppu cycle.
            cycleTick = (byte)((PPU_Dot) & 7);

            switch (cycleTick)
            {
                case 0:
                    break;
                case 1:
                    // fetch byte from Nametable
                    PPU_AddressBus = (ushort)(0x2000 + (PPU_ReadWriteAddress & 0x0FFF));
                    PPU_RenderTemp = FetchPPU(PPU_AddressBus);
                    PPU_Commit_NametableFetch = true;
                    break;
                case 2:

                    break;
                case 3:
                    // fetch attribute byte from attribute table
                    PPU_AddressBus = (ushort)(0x23C0 | (PPU_ReadWriteAddress & 0x0C00) | ((PPU_ReadWriteAddress >> 4) & 0x38) | ((PPU_ReadWriteAddress >> 2) & 0x07));
                    PPU_RenderTemp = FetchPPU(PPU_AddressBus);
                    PPU_Commit_AttributeFetch = true;
                    // now we only have the 2 bits we're looking for
                    break;
                case 4:

                    break;
                case 5:
                    // fetch pattern bits from value read off the nametable
                    PPU_AddressBus = (ushort)(((PPU_ReadWriteAddress & 0b0111000000000000) >> 12) | PPU_NextCharacter * 16 | (PPU_PatternSelect_Background ? 0x1000 : 0));
                    PPU_RenderTemp = FetchPPU((ushort)(PPU_AddressBus & 0x1FFF));
                    PPU_Commit_PatternLowFetch = true;
                    break;
                case 6:

                    break;
                case 7:
                    // fetch pattern bits with the new address
                    PPU_AddressBus = (ushort)(((PPU_ReadWriteAddress & 0b0111000000000000) >> 12) | PPU_NextCharacter * 16 | (PPU_PatternSelect_Background ? 0x1000 : 0) + 8);

                    PPU_RenderTemp = FetchPPU((ushort)(PPU_AddressBus & 0x1FFF));
                    PPU_Commit_PatternHighFetch = true;
                    break;
            }

        }

        bool PPU_Commit_LoadShiftRegisters;
        void PPU_Render_ShiftRegistersAndBitPlanes_HalfDot()
        {
            byte cycleTick; // for the switch statement below, this checks which case to run on a given ppu cycle.
            cycleTick = (byte)((PPU_Dot) & 7);

            switch (cycleTick)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    break;
                case 7:
                    PPU_Commit_LoadShiftRegisters = true;
                    break;
            }
        }

        void PPU_Render_CommitShiftRegistersAndBitPlanes()
        {
            if (PPU_Commit_NametableFetch)
            {
                PPU_Commit_NametableFetch = false;
                PPU_NextCharacter = PPU_RenderTemp;
            }
            if (PPU_Commit_AttributeFetch)
            {
                PPU_Commit_AttributeFetch = false;
                PPU_Attribute = PPU_RenderTemp;
                // 1 byte of attribute data is 4 tiles worth. determine which tile this is for.
                if ((PPU_ReadWriteAddress & 3) >= 2) // If this is on the right tile
                {
                    PPU_Attribute = (byte)(PPU_Attribute >> 2);
                }
                if ((((PPU_ReadWriteAddress & 0b0000001111100000) >> 5) & 3) >= 2) // If this is on the bottom tile
                {
                    PPU_Attribute = (byte)(PPU_Attribute >> 4);
                }
                PPU_Attribute = (byte)(PPU_Attribute & 3);
            }
            if (PPU_Commit_PatternLowFetch)
            {
                PPU_Commit_PatternLowFetch = false;
                PPU_LowBitPlane = PPU_RenderTemp;
            }
            if (PPU_Commit_PatternHighFetch)
            {
                PPU_Commit_PatternHighFetch = false;
                PPU_HighBitPlane = PPU_RenderTemp;
                PPU_IncrementScrollX();
            }
        }
        void PPU_Render_CommitShiftRegistersAndBitPlanes_HalfDot()
        {
            if (PPU_Commit_LoadShiftRegisters)
            {
                PPU_Commit_LoadShiftRegisters = false;
                PPU_LoadShiftRegisters();
            }
        }

        void PPU_Render_ShiftRegistersAndBitPlanes_DummyNT()
        {
            byte cycleTick; // for the switch statement below, this checks which case to run on a given ppu cycle.
            cycleTick = (byte)(PPU_Dot - 336);

            switch (cycleTick)
            {
                case 0:
                    // fetch byte from Nametable
                    PPU_AddressBus = (ushort)(0x2000 + (PPU_ReadWriteAddress & 0x0FFF));
                    PPU_RenderTemp = FetchPPU(PPU_AddressBus);
                    break;
                case 1:
                    // store the character read from the nametable
                    PPU_NextCharacter = PPU_RenderTemp;
                    break;
                case 2:
                    // fetch byte from Nametable
                    PPU_AddressBus = (ushort)(0x2000 + (PPU_ReadWriteAddress & 0x0FFF));
                    PPU_RenderTemp = FetchPPU(PPU_AddressBus);
                    break;
                case 3:
                    // store the character read from the nametable
                    PPU_NextCharacter = PPU_RenderTemp;
                    break;
            }

        }


        // in sprite evaluation, if a sprite is horizontally mirrored, we need to flip all the order of the bits in the shift register.
        public byte Flip(byte b)
        {
            b = (byte)(((b & 0xF0) >> 4) | ((b & 0xF) << 4));
            b = (byte)(((b & 0xCC) >> 2) | ((b & 0x33) << 2));
            b = (byte)(((b & 0xAA) >> 1) | ((b & 0x55) << 1));
            return b;
        }

        void PPU_UpdateShiftRegisters()
        {
            PPU_BackgroundPatternShiftRegisterL = (ushort)((PPU_BackgroundPatternShiftRegisterL << 1) | 0); // shift 1 bit to the left. Bring in a 0.
            PPU_BackgroundPatternShiftRegisterH = (ushort)((PPU_BackgroundPatternShiftRegisterH << 1) | 1); // shift 1 bit to the left. Bring in a 1.
            PPU_BackgroundAttributeShiftRegisterL = (ushort)((PPU_BackgroundAttributeShiftRegisterL << 1) | (PPU_AttributeLatchRegister & 1)); // shift 1 bit to the left. Bring in Attribute low bit.
            PPU_BackgroundAttributeShiftRegisterH = (ushort)((PPU_BackgroundAttributeShiftRegisterH << 1) | ((PPU_AttributeLatchRegister & 10) >> 1)); // shift 1 bit to the left. Bring in Attribute high bit.
        }

        void UpdateSpriteShiftRegisters()
        {
            if (PPU_Dot <= 256) // the shift registers for sprites are shifted after the rendering process.
            {
                // shift all 8 sprite shift registers.
                int i = 0;
                while (i < 8)
                {
                    if (PPU_SpriteShifterCounter[i] > 0 && !SkippedPreRenderDot341)
                    {
                        PPU_SpriteShifterCounter[i]--; // decrement the X position of all objects in secondary OAM. When this is zero, the ppu can draw it.
                    }
                    else
                    {
                        if ((PPU_Mask_ShowSprites || PPU_Mask_ShowBackground)) // this happens if rendering either sprites or background.
                        {
                            PPU_SpriteShiftRegisterL[i] = (byte)(PPU_SpriteShiftRegisterL[i] << 1); // shift 1 bit to the left.
                            PPU_SpriteShiftRegisterH[i] = (byte)(PPU_SpriteShiftRegisterH[i] << 1); // shift 1 bit to the left.
                        }
                    }

                    i++;
                }

            }
        }

        void PPU_LoadShiftRegisters()
        {
            // this runs as the first step of PPU_Render_ShiftRegistersAndBitPlanes(), using the values determined by the previous 8 steps of PPU_Render_ShiftRegistersAndBitPlanes().
            PPU_BackgroundPatternShiftRegisterL = (ushort)((PPU_BackgroundPatternShiftRegisterL & 0xFF00) | PPU_LowBitPlane);
            PPU_BackgroundPatternShiftRegisterH = (ushort)((PPU_BackgroundPatternShiftRegisterH & 0xFF00) | PPU_HighBitPlane);
            PPU_AttributeLatchRegister = PPU_Attribute;
        }

        void PPU_IncrementScrollX()
        {
            // used when setting up shift registers for the background
            // update the v register. Either increment it, or reset the scroll
            if ((PPU_ReadWriteAddress & 0x001F) == 31)
            {
                PPU_ReadWriteAddress &= 0xFFE0; // resetting the scroll
                PPU_ReadWriteAddress ^= 0x0400;
            }
            else
            {
                PPU_ReadWriteAddress++; // increment
            }
        }

        void PPU_IncrementScrollY()
        {
            if (CopyV)
            {
                PPU_ReadWriteAddress = (ushort)(PPU_Update2006Value_Temp & PPU_Update2006Value); // This isn't actually accurate. More research needed.
            }
            else
            {
                if ((PPU_ReadWriteAddress & 0x7000) != 0x7000)
                {
                    PPU_ReadWriteAddress += 0x1000;
                }
                else
                {
                    PPU_ReadWriteAddress &= 0x0FFF;
                    int y = (PPU_ReadWriteAddress & 0x03E0) >> 5;
                    if (y == 29)
                    {
                        y = 0; // reset the Y value and also flip some other bit in the 'v' register
                        PPU_ReadWriteAddress ^= 0x0800;
                    }
                    else if (y == 31)
                    {
                        y = 0; // reset the Y value
                    }
                    else
                    {
                        y++; // increment the Y value
                    }
                    PPU_ReadWriteAddress = (ushort)((PPU_ReadWriteAddress & 0xFC1F) | (y << 5));
                }
            }
        }

        void PPU_ResetXScroll()
        {
            // If a write to $2000 occurs during this ppu cycle, PPU_TempVRAMAddress will be the incorrect value!
            // The value of PPU_TempVRAMAddress will be corrected on the next ppu cycle, but it's already too late.
            // This is the "scanline bug" : https://www.nesdev.org/wiki/PPU_glitches#PPUCTRL
            // The bug is only visible if the nametable mirroring is vertical.
            PPU_ReadWriteAddress = (ushort)((PPU_ReadWriteAddress & 0b0111101111100000) | (PPU_TempVRAMAddress & 0b0000010000011111));
        }
        void PPU_ResetYScroll()
        {
            // The exact same issue from PPU_ResetXScroll() can happen here too, except this corrupts an entire frame.
            // The bug is only visible if the nametable mirroring is horizontal.
            //PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0111110000011111) | (0b000001111000000)); //Uncomment this line to experiment with the "Attirbutes as tiles" bug.
            PPU_ReadWriteAddress = (ushort)((PPU_ReadWriteAddress & 0b0000010000011111) | (PPU_TempVRAMAddress & 0b0111101111100000));
        }

        void DecayPPUDataBus()
        {
            int i = 0;
            while (i < PPUBusDecay.Length)
            {
                if (PPUBusDecay[i] > 0)
                {
                    PPUBusDecay[i]--;
                    if (PPUBusDecay[i] == 0)
                    {
                        PPUBus &= DecayBitmask[i];
                    }
                }
                i++;
            }
        }
        byte[] DecayBitmask = { 0xFE, 0xFD, 0xFB, 0xF7, 0xEF, 0xDF, 0xBF, 0x7F };

        // The object attribute memory DMA!
        bool OAMDMA_Aligned = false;
        bool OAMDMA_Halt = false;
        bool DMCDMA_Halt = false;
        byte OAM_InternalBus;   // a data bus that's used for the OAM DMA
        ushort OAMAddressBus;   // the address bus of the OAM DMA

        // The DMAs (Direct Memory Accesses) Have "get" and "put" cycles.
        // they can also be "halted" in which case, it will always read instead of write.

        // the following functions,
        // OAMDMA_Get()    : Get cycles are reads
        // OAMDMA_Halted() : Halted gets and halted puts are both reads from the current address bus
        // OAMDMA_Put()    : Put cycles are writes to OAM.

        // DMCDMA_Get()    : Get cycles are reads
        // DMCDMA_Halted() : Halted gets and halted puts are both reads from the current address bus
        // DMCDMA_Put()    : Put cycles are writes to the DMC shifter.

        void OAMDMA_Get()
        {
            OAMAddressBus = (ushort)(DMAPage << 8 | DMAAddress);
            OAMDMA_Aligned = true;
            // the fetch happens regardless of halt
            OAM_InternalBus = Fetch(OAMAddressBus);
        }
        void OAMDMA_Halted()
        {
            Fetch(addressBus); // if halted, just read from the current address bus.
        }

        void OAMDMA_Put()
        {

            if (OAMDMA_Aligned) // if the DMA is aligned
            {
                Store(OAM_InternalBus, 0x2004); // write to OAM
                DMAAddress++;
                if (DMAAddress == 0) // if we overflow the DMA address
                {
                    DoOAMDMA = false; // we have completed the DMA.
                    OAMDMA_Aligned = false;
                    return;
                }
            }
            else // if this is an alignment cycle
            {
                Fetch(addressBus); // just read from the current address bus
            }

        }

        void DMCDMA_Get()
        {
            // now reload the DMC buffer.
            APU_DMC_Buffer = Fetch(APU_DMC_AddressCounter);

            APU_DMC_AddressCounter++;
            if (APU_DMC_AddressCounter == 0)
            {
                APU_DMC_AddressCounter = 0x8000;
            }
            if (APU_DMC_BytesRemaining > 0)
            {
                // due to writes to $4015 setting the BytesRemaining to 0 if disabled, this could potentially underflow without the if statement.
                APU_DMC_BytesRemaining--;
            }

            if (APU_DMC_BytesRemaining == 0)
            {
                //reset sample

                if (!APU_DMC_Loop)
                {
                    APU_Status_DMC = false;
                    if (APU_DMC_EnableIRQ) // if the DMC should fire an IRQ when it completes...
                    {
                        IRQ_LevelDetector = true;
                        APU_Status_DMCInterrupt = true;
                    }
                }
                else
                {
                    StartDMCSample();
                }
            }
            DoDMCDMA = false;
            OAMDMA_Aligned = false;
            CannotRunDMCDMARightNow = 2;

        }

        void DMCDMA_Halted()
        {
            Fetch(addressBus);
        }
        void DMCDMA_Put()
        {
            Fetch(addressBus);
        }

        // Typically in the last CPU cycle of an instruction, the console will check if the NMI edge detector or IRQ level detector is set. In which case, it's time to run an interrupt.
        // The timing on this is different for branch instructions, and the BRK instruction doesn't do this at all.
        void PollInterrupts()
        {
            NMI_PreviousPinsSignal = NMI_PinsSignal;
            NMI_PinsSignal = NMILine;
            if (NMI_PinsSignal && !NMI_PreviousPinsSignal)
            {
                DoNMI = true;
            }
            DoIRQ = IRQLine && !flag_Interrupt;
        }

        void PollInterrupts_CantDisableIRQ()
        {
            NMI_PreviousPinsSignal = NMI_PinsSignal;
            NMI_PinsSignal = NMILine;
            if (NMI_PinsSignal && !NMI_PreviousPinsSignal)
            {
                DoNMI = true;
            }
            if (!DoIRQ)
            {
                DoIRQ = IRQLine && !flag_Interrupt;
            }
        }

        public void _6502()
        {
            if ((DoDMCDMA && (APU_Status_DMC || APU_ImplicitAbortDMC4015) && CPU_Read) || (DoOAMDMA && CPU_Read)) // Are we running a DMA? Did it fail? Also some specific behavior can force a DMA to abort. Did that occur?
            {
                if (
                    (opCode == 0x93 && operationCycle == 4) ||
                    (opCode == 0x9B && operationCycle == 3) ||
                    (opCode == 0x9C && operationCycle == 3) ||
                    (opCode == 0x9E && operationCycle == 3) ||
                    (opCode == 0x9F && operationCycle == 3)
                    )
                {
                    IgnoreH = true;
                }

                if (DoOAMDMA && FirstCycleOfOAMDMA) // interrupt suppression. (There's probably a better way to implement this) if this is the first cycle of the OAM DMA...
                {
                    FirstCycleOfOAMDMA = false; // disable this flag.
                    if (!APU_PutCycle)
                    {
                        OAMDMA_Halt = true;
                    }
                }

                if (APU_PutCycle) // even cycles are puts, odd cycles are gets.
                {
                    // Put cycle (write)
                    if (DoDMCDMA && DoOAMDMA) // if we're running both a DMC and OAM DMA.
                    {
                        if (DMCDMA_Halt && OAMDMA_Halt) // both halt cycles
                        {
                            OAMDMA_Halted();
                        }
                        else if (!OAMDMA_Halt && DMCDMA_Halt) // only DMC halted
                        {
                            OAMDMA_Put();
                        }
                        else if (OAMDMA_Halt && !DMCDMA_Halt) // only OAM halted
                        {
                            DMCDMA_Put(); // Can this logically ever happen?
                        }
                        else // none halted : OAM DMA has priority
                        {
                            OAMDMA_Put();
                        }
                    }
                    else // only performing a single DMA
                    {
                        if (DoDMCDMA) // only running DMC DMA
                        {
                            if (DMCDMA_Halt)
                            {
                                DMCDMA_Halted();
                            }
                            else
                            {
                                DMCDMA_Put();
                            }
                        }
                        else // only running OAM DMA
                        {
                            if (OAMDMA_Halt)
                            {
                                OAMDMA_Halted();
                            }
                            else
                            {
                                OAMDMA_Put();
                            }
                        }
                    }
                }
                else
                {
                    // Get cycle (read)
                    if (DoDMCDMA && DoOAMDMA) // if we're running both a DMC and OAM DMA.
                    {
                        if (DMCDMA_Halt && OAMDMA_Halt) // both halt cycles
                        {
                            DMCDMA_Halted();
                        }
                        else if (!OAMDMA_Halt && DMCDMA_Halt) // only DMC halted
                        {
                            OAMDMA_Get();
                        }
                        else if (OAMDMA_Halt && !DMCDMA_Halt) // only OAM halted
                        {
                            DMCDMA_Get();
                        }
                        else // none halted : DMC DMA has priority
                        {
                            DMCDMA_Get();
                        }
                    }
                    else
                    {
                        // only performing a single DMA
                        if (DoDMCDMA) // only running DMC DMA
                        {
                            if (DMCDMA_Halt)
                            {
                                DMCDMA_Halted();
                            }
                            else
                            {
                                DMCDMA_Get();
                            }
                        }
                        else // only running OAM DMA
                        {
                            if (OAMDMA_Halt)
                            {
                                OAMDMA_Halted();
                            }
                            else
                            {
                                OAMDMA_Get();
                            }
                        }
                    }

                    DMCDMA_Halt = false; // both halt cycles get cleared after a get cycle.
                    OAMDMA_Halt = false;
                }

            }
            else if (operationCycle == 0) // We are not running any DMAs, and this is the first cycle of an instruction.
            {
                // cycle 0. fetch opcode:
                addressBus = programCounter;

                opCode = Fetch(addressBus); // Fetch the value at the program counter. This is the opcode.


                if (DoNMI) // If an NMI is occurring,
                {
                    opCode = 0; // replace the opcode with 0. (A BRK, which has modified behavior for NMIs)
                }
                else if (DoIRQ) // If an IRQ is occurring,
                {
                    opCode = 0; // replace the opcode with 0. (A BRK, which has modified behavior for IRQs)
                }
                else if (DoReset) // If a RESET is occurring,
                {
                    opCode = 0; // replace the opcode with 0. (A BRK, which has modified behavior for RESETs)
                }
                else if (opCode == 0) // Otherwise, if an interrupt is not occurring, and the opcode is already 0
                {
                    DoBRK = true; // There's also specific behavior for the BRK instruction if it is in-fact a BRK, and not an interrupt.
                }


                if (Logging) // For debugging only.
                {
                    Debug(); // This is where the tracelogger occurs.
                }
                if ((!DoNMI && !DoIRQ && !DoReset)) // If we aren't running any interrupts...
                {
                    programCounter++; // the PC is incremented to the next address
                    addressBus = programCounter;
                }

                operationCycle++; // increment this for use in the following CPU cycle.

            }
            else
            {
                // a really big switch statement.
                // depending on the value of the opcode, different behavior will take place.
                // this is how instructions work.

                // All instructions are labeled. If it's an undocumented opcode, I also write "***" next to it.

                switch (opCode)
                {
                    case 0x00: //BRK
                        switch (operationCycle)
                        {
                            case 1:
                                if (!DoBRK)
                                {
                                    addressBus = programCounter;
                                    Fetch(addressBus); //dummy fetch without incrementing PC.
                                }
                                else
                                {
                                    GetImmediate(); //dummy fetch and PC increment
                                }
                                break;
                            case 2:
                                if (!DoReset)
                                {
                                    Push((byte)(programCounter >> 8));
                                }
                                else
                                {
                                    ResetReadPush();
                                }
                                break;
                            case 3:
                                if (!DoReset)
                                {
                                    Push((byte)programCounter);
                                }
                                else
                                {
                                    ResetReadPush();
                                }
                                break;
                            case 4:
                                if (!DoReset)
                                {
                                    status = flag_Carry ? (byte)0x01 : (byte)0;
                                    status |= flag_Zero ? (byte)0x02 : (byte)0;
                                    status |= flag_Interrupt ? (byte)0x04 : (byte)0;
                                    status |= flag_Decimal ? (byte)0x08 : (byte)0;
                                    status |= DoBRK ? (byte)0x10 : (byte)0;
                                    status |= 0x20;
                                    status |= flag_Overflow ? (byte)0x40 : (byte)0;
                                    status |= flag_Negative ? (byte)0x80 : (byte)0;
                                    Push(status);
                                }
                                else
                                {
                                    ResetReadPush();
                                }
                                PollInterrupts(); // check for NMI?
                                break;
                            case 5:
                                if (DoNMI)
                                {
                                    programCounter = (ushort)((programCounter & 0xFF00) | (Fetch(0xFFFA)));
                                }
                                else if (DoReset)
                                {
                                    programCounter = (ushort)((programCounter & 0xFF00) | (Fetch(0xFFFC)));
                                }
                                else
                                {
                                    programCounter = (ushort)((programCounter & 0xFF00) | (Fetch(0xFFFE)));
                                }

                                break;
                            case 6:
                                if (DoNMI)
                                {
                                    programCounter = (ushort)((programCounter & 0xFF) | (Fetch(0xFFFB) << 8));
                                }
                                else if (DoReset)
                                {
                                    programCounter = (ushort)((programCounter & 0xFF) | (Fetch(0xFFFD) << 8));
                                }
                                else
                                {
                                    programCounter = (ushort)((programCounter & 0xFF) | (Fetch(0xFFFF) << 8));
                                }

                                operationComplete = true; // notably, BRK does not check the NMI edge detector at the end of the instruction
                                DoReset = false;

                                DoNMI = false;
                                DoIRQ = false;
                                IRQLine = false;

                                DoBRK = false;

                                flag_Interrupt = true;



                                break;
                        }
                        break;

                    case 0x01: //(ORA, X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_ORA(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x02: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x03: //(SLO, X)  *** 
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // write back to the address
                                Store(dl, addressBus);
                                break; // perform the operation
                            case 7:
                                PollInterrupts();
                                Op_SLO(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x04: //DOP ***
                        if (operationCycle == 1)
                        {
                            GetAddressZeroPage();
                        }
                        else
                        {
                            // read from address
                            PollInterrupts();
                            Fetch(addressBus);
                            operationComplete = true;
                        }
                        break;

                    case 0x05: //ORA zp
                        if (operationCycle == 1)
                        {
                            GetAddressZeroPage();
                        }
                        else
                        {
                            // read from address
                            PollInterrupts();
                            Op_ORA(Fetch(addressBus));
                            operationComplete = true;
                        }
                        break;

                    case 0x06: //ASL, zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_ASL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x07: //SLO zp  *** 
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_SLO(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x08: //PHP

                        if (operationCycle == 1)
                        {
                            //dummy fetch
                            Fetch(programCounter);
                        }
                        else
                        {
                            PollInterrupts();
                            // read from address
                            status = flag_Carry ? (byte)0x01 : (byte)0;
                            status += flag_Zero ? (byte)0x02 : (byte)0;
                            status += flag_Interrupt ? (byte)0x04 : (byte)0;
                            status += flag_Decimal ? (byte)0x08 : (byte)0;
                            status += 0x10; //always set in PHP
                            status += 0x20; //always set in PHP
                            status += flag_Overflow ? (byte)0x40 : (byte)0;
                            status += flag_Negative ? (byte)0x80 : (byte)0;
                            Push(status);
                            operationComplete = true;
                        }
                        break;

                    case 0x09: //ORA Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_ORA(dl);
                        operationComplete = true;
                        break;

                    case 0x0A: //ASL A
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        Op_ASL_A();
                        operationComplete = true;
                        break;

                    case 0x0B: //ANC Imm ***
                        PollInterrupts();
                        GetImmediate();
                        A = (byte)(A & dl);
                        flag_Carry = A >= 0x80;
                        flag_Zero = A == 0;
                        flag_Negative = A >= 0x80;
                        operationComplete = true;

                        break;

                    case 0x0C: //TOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x0D: //ORA Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_ORA(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x0E: //ASL, Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_ASL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x0F: //SLO Abs  *** 
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_SLO(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x10: //BPL
                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (flag_Negative)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x11: //(ORA) Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_ORA(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x12: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x13: //(SLO) Y  *** 
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                break;
                            case 5: // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // dummy write
                                Store(dl, addressBus);
                                break;
                            case 7: // read from address
                                PollInterrupts();
                                Op_SLO(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x14: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x15: //ORA zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_ORA(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x16: //ASL, zp X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_ASL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x17: //SLO zp X *** 
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_SLO(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x18: //CLC
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        flag_Carry = false;
                        operationComplete = true;
                        break;

                    case 0x19: //ORA Abs, Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_ORA(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x1A: //NOP ***
                        PollInterrupts();
                        addressBus = programCounter; Fetch(addressBus);
                        operationComplete = true;
                        break;

                    case 0x1B: //SLO Abs Y *** 
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_SLO(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x1C: //TOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x1D: //ORA Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_ORA(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x1E: //ASL, Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_ASL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;


                    case 0x1F: //SLO Abs, X *** 
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_SLO(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x20: //JSR

                        switch (operationCycle)
                        {
                            // this is pretty cursed, though according to visual6502, this is apparently what happens.
                            case 1: // fetch the byte that will be PC low
                                addressBus = programCounter;
                                dl = Fetch(addressBus);
                                programCounter++;
                                break;
                            case 2: // transfer stack pointer to address bus, and alu to stack pointer. I'm just reusing `dl` here, but this instruction actually uses the Arithmetic Logic Unit for this.
                                addressBus = (ushort)(0x100 | stackPointer);
                                stackPointer = dl;
                                CPU_Read = false;
                                Fetch(addressBus); // dummy read
                                break;
                            case 3: // push PC high to stack via address bus
                                Store((byte)((programCounter & 0xFF00) >> 8), addressBus);
                                addressBus = (ushort)((byte)(addressBus - 1) | 0x100);
                                break;
                            case 4: // push PC low to stack via address bus
                                Store((byte)(programCounter & 0xFF), addressBus);
                                addressBus = (ushort)((byte)(addressBus - 1) | 0x100);
                                specialBus = (byte)addressBus;
                                CPU_Read = true;
                                break;
                            case 5: // fetch PC High, transfer stack pointer to PC low, address bus to stack pointer.
                                PollInterrupts();
                                addressBus = programCounter;
                                programCounter = (ushort)((Fetch(addressBus) << 8) | stackPointer);
                                stackPointer = specialBus;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x21: //(AND, X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_AND(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x22: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x23: //(RLA, X)  ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // write back to the address
                                Store(dl, addressBus);
                                break; // perform the operation
                            case 7:
                                PollInterrupts();
                                Op_RLA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x24: //BIT Zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                dl = Fetch(addressBus);
                                flag_Zero = (A & dl) == 0;
                                flag_Negative = (dl & 0x80) != 0;
                                flag_Overflow = (dl & 0x40) != 0;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x25: //AND zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Op_AND(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x26: //ROL zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_ROL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x27: //RLA zp  ***
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_RLA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x28: //PLP
                        switch (operationCycle)
                        {
                            case 1: //dummy fetch
                                addressBus = programCounter;
                                Fetch(addressBus);
                                break;
                            case 2: //increment S
                                addressBus = (ushort)(0x100 + stackPointer);
                                Fetch(addressBus); // dummy read
                                stackPointer++;
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                addressBus = (ushort)(0x100 + stackPointer);
                                status = Fetch(addressBus);
                                flag_Carry = (status & 1) == 1;
                                flag_Zero = ((status & 0x02) >> 1) == 1;
                                flag_Interrupt = ((status & 0x04) >> 2) == 1;
                                flag_Decimal = ((status & 0x08) >> 3) == 1;
                                flag_Overflow = ((status & 0x40) >> 6) == 1;
                                flag_Negative = ((status & 0x80) >> 7) == 1;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x29: //AND Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_AND(dl);
                        operationComplete = true;
                        break;

                    case 0x2A: //ROL A
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        Op_ROL_A();
                        operationComplete = true;
                        break;

                    case 0x2B: //ANC Imm *** (same as 0x0B)
                        PollInterrupts();
                        GetImmediate();
                        A = (byte)(A & dl);
                        flag_Carry = A >= 0x80;
                        flag_Zero = A == 0;
                        flag_Negative = A >= 0x80;
                        operationComplete = true;

                        break;

                    case 0x2C: //BIT Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                dl = Fetch(addressBus);
                                flag_Zero = (A & dl) == 0;
                                flag_Negative = (dl & 0x80) != 0;
                                flag_Overflow = (dl & 0x40) != 0;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x2D: //AND Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_AND(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x2E: //ROL Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_ROL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x2F: //RLA Abs ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_RLA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x30: //BMI
                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (!flag_Negative)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x31: //(AND), Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_AND(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x32: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;
                    case 0x33: //(RLA), Y  ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                break;
                            case 5: // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // dummy write
                                Store(dl, addressBus);
                                break;
                            case 7: // read from address
                                PollInterrupts();
                                Op_RLA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x34: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x35: //AND zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_AND(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x36: //ROL zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_ROL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x37: //RLA zp, X  ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_RLA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x38: //SEC
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        flag_Carry = true;
                        operationComplete = true;
                        break;

                    case 0x39: //AND Abs, Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_AND(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x3A: //NOP ***
                        PollInterrupts();
                        addressBus = programCounter; Fetch(addressBus);
                        operationComplete = true;
                        break;

                    case 0x3B: //RLA Abs, Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_RLA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x3C: //TOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x3D: //AND Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_AND(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x3E: //ROL Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_ROL(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x3F: //RLA Abs, X ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_RLA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x40: //RTI
                        switch (operationCycle)
                        {
                            case 1:
                                GetImmediate();
                                break;
                            case 2:
                                addressBus = (ushort)(0x100 | stackPointer);
                                Fetch(addressBus);
                                addressBus = (ushort)((byte)(addressBus + 1) | 0x100);
                                break;
                            case 3:
                                status = Fetch(addressBus);
                                flag_Carry = (status & 1) != 0;
                                flag_Zero = (status & 0x02) != 0;
                                flag_Interrupt = (status & 0x04) != 0;
                                flag_Decimal = (status & 0x08) != 0;
                                flag_Overflow = (status & 0x40) != 0;
                                flag_Negative = (status & 0x80) != 0;

                                addressBus = (ushort)((byte)(addressBus + 1) | 0x100);
                                break;
                            case 4:
                                dl = Fetch(addressBus);
                                programCounter = (ushort)((programCounter & 0xFF00) | dl); //technically not accurate, as this happens in cycle 5
                                addressBus = (ushort)((byte)(addressBus + 1) | 0x100);
                                break;
                            case 5:
                                PollInterrupts();
                                dl = Fetch(addressBus);
                                programCounter = (ushort)((programCounter & 0xFF) | (dl << 8));
                                stackPointer = (byte)addressBus;
                                operationComplete = true;
                                break;

                        }
                        break;

                    case 0x41: //(EOR X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_EOR(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x42: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x43: //(SRE, X) ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // write back to the address
                                Store(dl, addressBus);
                                break; // perform the operation
                            case 7:
                                PollInterrupts();
                                Op_SRE(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x44: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x45: //EOR zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Op_EOR(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x46: //LSR zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_LSR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x47: //SRE zp ***

                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_SRE(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x48: //PHA

                        switch (operationCycle)
                        {
                            case 1: //dummy fetch
                                dl = Fetch(addressBus);
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Push(A);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x49: //EOR Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_EOR(dl);
                        operationComplete = true;
                        break;

                    case 0x4A: //LSR A
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        Op_LSR_A();
                        operationComplete = true;
                        break;

                    case 0x4B: //ASR Imm ***
                        PollInterrupts();
                        GetImmediate();
                        A = (byte)(A & dl);
                        Op_LSR_A();
                        operationComplete = true;
                        break;

                    case 0x4C: //JMP
                        if (operationCycle == 1)
                        {
                            GetAddressAbsolute();

                        }
                        else
                        {
                            PollInterrupts();
                            GetAddressAbsolute();
                            programCounter = addressBus;
                            operationComplete = true;
                        }
                        break;

                    case 0x4D: //EOR Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_EOR(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x4E: //LSR abs

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_LSR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x4F: //SRE abs ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_SRE(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x50: //BVC

                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (flag_Overflow)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x51: //(EOR), Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_EOR(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x52: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x53: //(SRE) Y ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                break;
                            case 5: // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // dummy write
                                Store(dl, addressBus);
                                break;
                            case 7: // read from address
                                PollInterrupts();
                                Op_SRE(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x54: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x55: //EOR zp , X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_EOR(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x56: //LSR zp, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_LSR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x57: //SRE zp X ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_SRE(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x58: //CLI
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        flag_Interrupt = false;
                        operationComplete = true;
                        break;

                    case 0x59: //EOR Abs Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_EOR(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x5A: //NOP ***
                        PollInterrupts();
                        addressBus = programCounter; Fetch(addressBus);
                        operationComplete = true;
                        break;

                    case 0x5B: //SRE abs, Y ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_SRE(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x5C: //TOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x5D: //EOR Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_EOR(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x5E: //LSR abs, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_LSR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x5F: //SRE abs, X ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_SRE(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x60: //RTS


                        switch (operationCycle)
                        {
                            case 1:
                                GetImmediate();
                                break;
                            case 2:
                                addressBus = (ushort)(0x100 | stackPointer);
                                Fetch(addressBus);
                                addressBus = (ushort)((byte)(addressBus + 1) | 0x100);
                                break;
                            case 3:
                                dl = Fetch(addressBus);
                                programCounter = (ushort)((programCounter & 0xFF00) | dl); //technically not accurate, as this happens in cycle 5
                                addressBus = (ushort)((byte)(addressBus + 1) | 0x100);
                                break;
                            case 4:
                                dl = Fetch(addressBus);
                                programCounter = (ushort)((programCounter & 0xFF) | (dl << 8));
                                break;
                            case 5:
                                PollInterrupts();
                                stackPointer = (byte)addressBus;
                                GetImmediate();
                                operationComplete = true;
                                break;

                        }
                        break;

                    case 0x61: //(ADC X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_ADC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x62: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x63: //(RRA X) ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // write back to the address
                                Store(dl, addressBus);
                                break; // perform the operation
                            case 7:
                                PollInterrupts();
                                Op_RRA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x64: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x65: //ADC Zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Op_ADC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x66: //ROR zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_ROR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x67: //RRA zp ***
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_RRA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;
                    case 0x68: //PLA

                        switch (operationCycle)
                        {
                            case 1: //dummy fetch
                                addressBus = programCounter;
                                Fetch(addressBus);
                                break;
                            case 2: // read from address
                                addressBus = (ushort)(0x100 | (stackPointer));
                                Fetch(addressBus); // dummy read
                                stackPointer++;
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                addressBus = (ushort)(0x100 | (stackPointer));
                                A = Fetch(addressBus);
                                flag_Zero = A == 0;
                                flag_Negative = A >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x69: //ADC Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_ADC(dl);
                        operationComplete = true;
                        break;

                    case 0x6A: //ROR A
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        Op_ROR_A();
                        operationComplete = true;
                        break;

                    case 0x6B: // ARR ***
                        PollInterrupts();
                        GetImmediate();
                        A = (byte)(A & dl);
                        Op_ROR_A();
                        flag_Zero = A == 0;
                        flag_Carry = ((A & 0x40) >> 6) == 1;
                        flag_Overflow = (((A & 0x20) >> 5) ^ ((A & 0x40) >> 6)) == 1;
                        flag_Negative = A >= 0x80;
                        operationComplete = true;
                        break;

                    case 0x6C: //JMP (indirect)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3:
                                specialBus = Fetch(addressBus); // Okay, this doesn't actually use the SB register. I'm just reusing that variable.
                                break;
                            case 4:
                                PollInterrupts();
                                dl = Fetch((ushort)((addressBus & 0xFF00) | (byte)(addressBus + 1)));
                                programCounter = (ushort)((dl << 8) | specialBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x6D: //ADC Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_ADC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x6E: //ROR Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_ROR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x6F: //RRA Abs ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_RRA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x70: //BVS
                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (!flag_Overflow)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x71: //(ADC), Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_ADC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x72: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x73: //(RRA) Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                break;
                            case 5: // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // dummy write
                                Store(dl, addressBus);
                                break;
                            case 7: // read from address
                                PollInterrupts();
                                Op_RRA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x74: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x75: //ADC Zp, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_ADC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x76: //ROR zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_ROR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x77: //RRA zp X ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_RRA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x78: //SEI
                        PollInterrupts();
                        Fetch(addressBus); // dummy read
                        flag_Interrupt = true;
                        operationComplete = true;
                        break;
                    case 0x79: //ADC Abs, Y

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_ADC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x7A: //NOP ***
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus);
                        operationComplete = true;
                        break;

                    case 0x7B: //RRA Abs, Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_RRA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x7C: //TOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x7D: //ADC Abs, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_ADC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x7E: //ROR Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_ROR(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x7F: //RRA Abs, X ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_RRA(dl, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x80: //DOP ***
                        PollInterrupts();
                        GetImmediate();
                        operationComplete = true;
                        break;


                    case 0x81: //(STA X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Store(A, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x82: //DOP ***
                        PollInterrupts();
                        GetImmediate();
                        operationComplete = true;
                        break;

                    case 0x83: //(SAX X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Store((byte)(A & X), addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x84: //STY zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                CPU_Read = false;
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Store(Y, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x85: //STA zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                CPU_Read = false;
                                break;
                            case 2:
                                PollInterrupts();
                                Store(A, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x86: //STX zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                CPU_Read = false;
                                break;
                            case 2:
                                PollInterrupts();
                                Store(X, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;
                    case 0x87: //AAX zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                CPU_Read = false;
                                break;
                            case 2:
                                PollInterrupts();
                                Store((byte)(A & X), addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x88: //DEY

                        PollInterrupts();
                        Y--;
                        flag_Zero = Y == 0;
                        flag_Negative = Y >= 0x80;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        operationComplete = true;

                        break;

                    case 0x89: //DOP ***
                        PollInterrupts();
                        GetImmediate();
                        operationComplete = true;

                        break;

                    case 0x8A: //TXA
                        PollInterrupts();
                        A = X;
                        flag_Zero = A == 0;
                        flag_Negative = A >= 0x80;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        operationComplete = true;
                        break;

                    case 0x8B: //ANE
                        PollInterrupts();
                        GetImmediate();
                        //A = (((A | 0xFF) & X) & temp); 
                        // Magic = FF
                        A = (byte)((A | 0xFF) & X & dl); // 0xEE is also known as "MAGIC", and can supposedly be different depending on the CPU's temperature.
                        flag_Zero = A == 0;
                        flag_Negative = A >= 0x80;
                        operationComplete = true;
                        break;

                    case 0x8C: //STY Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Store(Y, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x8D: //STA Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Store(A, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x8E: //STX Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3:
                                PollInterrupts();
                                Store(X, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x8F: //AAX Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Store((byte)(A & X), addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x90: //BCC
                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (flag_Carry)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x91: //(STA), Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:
                                PollInterrupts();
                                Store(A, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x92: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0x93: // (SHA) Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                if (operationCycle == 4)
                                {
                                    CPU_Read = false;
                                }
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                if ((temporaryAddress & 0xFF00) != (addressBus & 0xFF00))
                                {
                                    // if adding Y to the target address crossed a page boundary, this opcode has "gone unstable"
                                    addressBus = (ushort)((byte)addressBus | ((addressBus >> 8) /*& A*/ & X) << 8); // Alternate SHA behavior. The A register isn't used here!
                                }
                                // pd = the high byte of the target address + 1
                                if (IgnoreH)
                                {
                                    H = 0xFF;
                                }
                                Store((byte)(A & (X | 0xF5) & H), addressBus); // Alternate SHA behavior. X is ORed with a magic number. On my console, it's $F5 for a few hours, then it flickers from $F5 and $FD.
                                operationComplete = true;
                                break;
                        }


                        break;

                    case 0x94: //STY zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Store(Y, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x95: //STA zp, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Store(A, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x96: //STX zp, Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffY();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Store(X, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x97: //AAX zp, Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffY();
                                if (operationCycle == 2) { CPU_Read = false; }
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Store((byte)(A & X), addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x98: //TYA
                        PollInterrupts();
                        A = Y;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Zero = A == 0;
                        flag_Negative = A >= 0x80;
                        operationComplete = true;

                        break;

                    case 0x99: //STA Abs, Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 3) { CPU_Read = false; }
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Store(A, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x9A: //TXS
                        PollInterrupts();
                        stackPointer = X;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        operationComplete = true;
                        break;


                    case 0x9B: //SHS, Abs Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 3) { CPU_Read = false; }
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                if ((temporaryAddress & 0xFF00) != (addressBus & 0xFF00))
                                {
                                    // if adding Y to the target address crossed a page boundary, this opcode has "gone unstable"
                                    addressBus = (ushort)((byte)addressBus | ((addressBus >> 8) /*& A*/ & X) << 8); // Alternate SHS behavior. The A register isn't used here!
                                }
                                // pd = the high byte of the target address + 1
                                stackPointer = (byte)(A & X);
                                if (IgnoreH)
                                {
                                    H = 0xFF;
                                }
                                Store((byte)(A & (X | 0xF5) & H), addressBus); // Alternate SHS behavior. X is ORed with a magic number. On my console, it's $F5 for a few hours, then it flickers from $F5 and $FD.
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x9C: //SHY Abs, X ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 3) { CPU_Read = false; }
                                break;
                            case 4:
                                PollInterrupts();
                                if ((temporaryAddress & 0xFF00) != (addressBus & 0xFF00))
                                {
                                    // if adding X to the target address crossed a page boundary, this opcode has "gone unstable"
                                    addressBus = (ushort)((byte)addressBus | ((addressBus >> 8) & Y) << 8);
                                }
                                if (IgnoreH)
                                {
                                    H = 0xFF;
                                }
                                Store((byte)(Y & H), addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x9D: //STA Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 3) { CPU_Read = false; }
                                break;
                            case 4:
                                PollInterrupts();
                                Store(A, addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x9E: // SHX Abs, Y***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 3) { CPU_Read = false; }
                                break;
                            case 4:
                                PollInterrupts();
                                // Not even close to what the documentation says this instruction does.
                                if ((temporaryAddress & 0xFF00) != (addressBus & 0xFF00))
                                {
                                    // if adding Y to the target address crossed a page boundary, this opcode has "gone unstable"
                                    addressBus = (ushort)((byte)addressBus | ((addressBus >> 8) & X) << 8);
                                }
                                if (IgnoreH)
                                {
                                    H = 0xFF;
                                }
                                Store((byte)(X & H), addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0x9F: // SHA Abs, Y***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 3) { CPU_Read = false; }
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                if ((temporaryAddress & 0xFF00) != (addressBus & 0xFF00))
                                {
                                    // if adding Y to the target address crossed a page boundary, this opcode has "gone unstable"
                                    addressBus = (ushort)((byte)addressBus | ((addressBus >> 8) /*& A*/ & X) << 8); // Alternate SHA behavior. The A register isn't used here!
                                }
                                if (IgnoreH)
                                {
                                    H = 0xFF;
                                }
                                Store((byte)(A & (X | 0xF5) & H), addressBus); // Alternate SHA behavior. X is ORed with a magic number. On my console, it's $F5 for a few hours, then it flickers from $F5 and $FD.
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xA0: //LDY imm
                        PollInterrupts();
                        GetImmediate();
                        Y = dl;
                        flag_Zero = Y == 0;
                        flag_Negative = Y >= 0x80;
                        operationComplete = true;

                        break;

                    case 0xA1: //(LDA, X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                flag_Zero = A == 0;
                                flag_Negative = A >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xA2: //LDX imm
                        PollInterrupts();
                        GetImmediate();
                        X = dl;
                        flag_Zero = X == 0;
                        flag_Negative = X >= 0x80;
                        operationComplete = true;

                        break;

                    case 0xA3: //(LAX, X) ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5:
                                PollInterrupts();
                                A = Fetch(addressBus);
                                X = A;
                                flag_Zero = X == 0;
                                flag_Negative = X >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xA4: //LDY zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Y = Fetch(addressBus);
                                flag_Zero = Y == 0;
                                flag_Negative = Y >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xA5: //LDA zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                flag_Zero = A == 0;
                                flag_Negative = A >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xA6: //LDX zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                X = Fetch(addressBus);
                                flag_Zero = X == 0;
                                flag_Negative = X >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xA7: //LAX zp ***
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                X = A;
                                flag_Zero = X == 0;
                                flag_Negative = X >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xA8: //TAY
                        PollInterrupts();
                        Y = A;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Zero = A == 0;
                        flag_Negative = Y >= 0x80;
                        operationComplete = true;
                        break;

                    case 0xA9: //LDA Imm
                        PollInterrupts();
                        GetImmediate();
                        A = dl;
                        flag_Zero = A == 0;
                        flag_Negative = A >= 0x80;
                        operationComplete = true;
                        break;

                    case 0xAA: //TAX
                        PollInterrupts();
                        X = A;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Zero = X == 0;
                        flag_Negative = X >= 0x80;
                        operationComplete = true;
                        break;

                    case 0xAB: //LXA ***
                        PollInterrupts();
                        GetImmediate();
                        A = (byte)((A | 0xFF) & dl); // 0xEE is also known as "MAGIC", and can supposedly be different depending on the CPU's temperature.
                        X = A;  // this instruction is basically XAA but using LAX behavior, so X is also affected..
                        flag_Negative = X >= 0x80;
                        flag_Zero = X == 0x00;
                        operationComplete = true;
                        break;

                    case 0xAC: //LDY Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Y = Fetch(addressBus);
                                flag_Negative = Y >= 0x80;
                                flag_Zero = Y == 0x00;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xAD: //LDA Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                flag_Negative = A >= 0x80;
                                flag_Zero = A == 0x00;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xAE: //LDX Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                X = Fetch(addressBus);
                                flag_Negative = X >= 0x80;
                                flag_Zero = X == 0x00;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xAF: //LAX Abs ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                X = A;
                                flag_Negative = X >= 0x80;
                                flag_Zero = X == 0x00;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xB0: //BCS
                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (!flag_Carry)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xB1: //(LDA), Y

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5:
                                PollInterrupts();
                                A = Fetch(addressBus);
                                flag_Zero = A == 0;
                                flag_Negative = A >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xB2: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0xB3: //(LAX), Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                X = A;
                                flag_Zero = X == 0;
                                flag_Negative = X >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;
                    case 0xB4: //LDY zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Y = Fetch(addressBus);
                                flag_Zero = Y == 0;
                                flag_Negative = Y >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xB5: //LDA zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                flag_Zero = A == 0;
                                flag_Negative = A >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xB6: //LDX zp,  Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffY();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                X = Fetch(addressBus);
                                flag_Zero = X == 0;
                                flag_Negative = X >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xB7: //LAX zp, Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffY();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                X = A;
                                flag_Zero = X == 0;
                                flag_Negative = X >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xB8: //CLV
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Overflow = false;
                        operationComplete = true;
                        break;

                    case 0xB9: //LDA abs , Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                flag_Zero = A == 0;
                                flag_Negative = A >= 0x80;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xBA: //TSX

                        PollInterrupts();
                        X = stackPointer;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Negative = X >= 0x80;
                        flag_Zero = X == 0;
                        operationComplete = true;
                        break;

                    case 0xBB: //LAE Abs, Y***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                dl = Fetch(addressBus);
                                A = (byte)(dl & stackPointer);
                                X = (byte)(dl & stackPointer);
                                stackPointer = (byte)(dl & stackPointer);
                                flag_Negative = X >= 0x80;
                                flag_Zero = X == 0;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xBC: //LDY abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Y = Fetch(addressBus);
                                flag_Negative = Y >= 0x80;
                                flag_Zero = Y == 0;
                                operationComplete = true;
                                break;
                        }
                        break;


                    case 0xBD: //LDA abs, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                flag_Negative = A >= 0x80;
                                flag_Zero = A == 0;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xBE: //LDX abs , Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                X = Fetch(addressBus);
                                flag_Negative = X >= 0x80;
                                flag_Zero = X == 0;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xBF: //LAX Abs, Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                A = Fetch(addressBus);
                                X = A;
                                flag_Negative = X >= 0x80;
                                flag_Zero = X == 0;
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xC0: //CPY Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_CPY(dl);
                        operationComplete = true;

                        break;

                    case 0xC1: //(CMP X),
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_CMP(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xC2: //DOP ***
                        PollInterrupts();
                        GetImmediate();
                        operationComplete = true;

                        break;

                    case 0xC3: //(DCP, X) ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // write back to the address
                                Store(dl, addressBus);
                                break; // perform the operation
                            case 7:
                                PollInterrupts();
                                dl--;
                                Store(dl, addressBus);
                                Op_CMP(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xC4: //CPY zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Op_CPY(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xC5: //CMP zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Op_CMP(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xC6: //DEC zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2:
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3:
                                Store(dl, addressBus); //dummy write
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xC7: //DCP zp ***
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2:
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3:
                                Store(dl, addressBus); //dummy write
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                Op_CMP(dl);
                                operationComplete = true;
                                break;
                        }
                        break;


                    case 0xC8: //INY
                        PollInterrupts();
                        Y++;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Zero = Y == 0;
                        flag_Negative = Y >= 0x80;
                        operationComplete = true;
                        break;

                    case 0xC9: //CMP Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_CMP(dl);
                        operationComplete = true;
                        break;

                    case 0xCA: //DEX
                        PollInterrupts();
                        X--;
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Zero = X == 0;
                        flag_Negative = X >= 0x80;
                        operationComplete = true;

                        break;

                    case 0xCB: // AXS ***
                        PollInterrupts();
                        GetImmediate();
                        X = (byte)(X & A);
                        flag_Carry = X >= dl;
                        X -= dl;
                        flag_Zero = X == 0;
                        flag_Negative = (X >= 0x80);

                        operationComplete = true;
                        break;


                    case 0xCC: //CPY Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_CPY(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xCD: //CMP Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_CMP(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xCE: //DEC Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3:
                                // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4:
                                // dummy write
                                Store(dl, addressBus);
                                break;
                            case 5: // write
                                PollInterrupts();
                                Op_DEC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xCF: //DCP Abs ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3:
                                // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4:
                                // dummy write
                                Store(dl, addressBus);
                                break;
                            case 5: // write
                                PollInterrupts();
                                Op_DEC(addressBus);
                                Op_CMP(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD0: //BNE
                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (flag_Zero)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD1: //(CMP), Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_CMP(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD2: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0xD3: //(DCP) Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                break;
                            case 5: // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // dummy write
                                Store(dl, addressBus);
                                break;
                            case 7: // read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                Op_CMP(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD4: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD5: //CMP zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_CMP(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD6: //DEC zp, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3:
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4:
                                Store(dl, addressBus); //dummy write
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD7: //DCP Zp X ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3:
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4:
                                Store(dl, addressBus); //dummy write
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                Op_CMP(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xD8: //CLD
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Decimal = false;
                        operationComplete = true;

                        break;
                    case 0xD9: //CMP abs, Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_CMP(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xDA: //NOP ***
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        operationComplete = true;
                        break;

                    case 0xDB: //DCP Abs Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                Op_CMP(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xDC: //TOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xDD: //CMP abs, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_CMP(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xDE: //DEC Abs X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xDF: //DCP Abs X ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_DEC(addressBus);
                                Op_CMP(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xE0: //CPX Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_CPX(dl);
                        operationComplete = true;
                        break;

                    case 0xE1: //(SBC X)
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_SBC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xE2: //DOP ***
                        PollInterrupts();
                        GetImmediate();
                        operationComplete = true;
                        break;

                    case 0xE3: //(ISC, X) ***

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffX();
                                break;
                            case 5: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // write back to the address
                                Store(dl, addressBus);
                                break; // perform the operation
                            case 7:
                                PollInterrupts();
                                Op_INC(addressBus);
                                Op_SBC(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xE4: //CPX zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Op_CPX(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xE5: //SBC Zp

                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                PollInterrupts();
                                Op_SBC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xE6: //INC zp
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_INC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xE7: //ISC zp ***
                        switch (operationCycle)
                        {
                            case 1:
                                GetAddressZeroPage();
                                break;
                            case 2: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 3: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 4: // perform operation
                                PollInterrupts();
                                Op_INC(addressBus);
                                Op_SBC(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xE8: //INX
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        X++;
                        flag_Zero = X == 0;
                        flag_Negative = X >= 0x80;
                        operationComplete = true;
                        break;

                    case 0xE9: //SBC Imm
                        PollInterrupts();
                        GetImmediate();
                        Op_SBC(dl);
                        operationComplete = true;
                        break;

                    case 0xEA: //NOP
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        operationComplete = true;
                        break;

                    case 0xEB: //SBC Imm ***
                        PollInterrupts();
                        GetImmediate();
                        Op_SBC(dl);
                        operationComplete = true;
                        break;

                    case 0xEC: //CPX Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_CPX(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xED: //SBC Abs

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_SBC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xEE: //INC Abs
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                if (addressBus == 0x4014)
                                {

                                }
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_INC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xEF: //ISC Abs ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressAbsolute();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_INC(addressBus);
                                Op_SBC(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF0: //BEQ
                        switch (operationCycle)
                        {
                            case 1:
                                PollInterrupts();
                                GetImmediate();
                                if (!flag_Zero)
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 2:
                                Fetch(addressBus); // dummy read
                                temporaryAddress = (ushort)(programCounter + ((dl >= 0x80) ? -(256 - dl) : dl));
                                programCounter = (ushort)((programCounter & 0xFF00) | (byte)((programCounter & 0xFF) + dl));
                                addressBus = programCounter;
                                if ((temporaryAddress & 0xFF00) == (programCounter & 0xFF00))
                                {
                                    operationComplete = true;
                                }
                                break;
                            case 3: // read from address
                                PollInterrupts_CantDisableIRQ(); // If the first poll detected an IRQ, this second poll should not be allowed to un-set the IRQ.
                                Fetch(addressBus); // dummy read
                                programCounter = (ushort)((programCounter & 0xFF) | (temporaryAddress & 0xFF00));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF1: //(SBC) Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(true);
                                break;
                            case 5: // read from address
                                PollInterrupts();
                                Op_SBC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF2: ///HLT ***
                        switch (operationCycle)
                        {
                            case 1:
                                dl = Fetch(programCounter);
                                break;
                            case 2:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 3:
                            case 4:
                                addressBus = 0xFFFE;
                                Fetch(addressBus);
                                break;
                            case 5:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                break;
                            case 6:
                                addressBus = 0xFFFF;
                                Fetch(addressBus);
                                operationCycle = 5; //makes this loop infinitely.
                                break;
                        }
                        break;

                    case 0xF3: //(ISC) Y
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressIndOffY(false);
                                break;
                            case 5: // dummy read
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 6: // dummy write
                                Store(dl, addressBus);
                                break;
                            case 7: // read from address
                                PollInterrupts();
                                Op_INC(addressBus);
                                Op_SBC(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF4: //DOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF5: //SBC Zp, X

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                PollInterrupts();
                                Op_SBC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF6: //INC Zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_INC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF7: //ISC zp, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                                GetAddressZPOffX();
                                break;
                            case 3: // read from address
                                dl = Fetch(addressBus);
                                CPU_Read = false;
                                break;
                            case 4: //dummy write
                                Store(dl, addressBus);
                                break;
                            case 5:
                                PollInterrupts();
                                Op_INC(addressBus);
                                Op_SBC(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xF8: //SED
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        flag_Decimal = true;
                        operationComplete = true;
                        break;

                    case 0xF9: //SBC Abs Y

                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffY(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_SBC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xFA: //NOP ***
                        PollInterrupts();
                        addressBus = programCounter;
                        Fetch(addressBus); // dummy read
                        operationComplete = true;
                        break;

                    case 0xFB: //ISC Abs Y ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffY(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_INC(addressBus);
                                Op_SBC(dl);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xFC: //TOP ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Fetch(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xFD: //SBC Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                                GetAddressAbsOffX(true);
                                break;
                            case 4: // read from address
                                PollInterrupts();
                                Op_SBC(Fetch(addressBus));
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xFE: //INC Abs, X
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_INC(addressBus);
                                operationComplete = true;
                                break;
                        }
                        break;

                    case 0xFF: //ISC Abs, X ***
                        switch (operationCycle)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                GetAddressAbsOffX(false);
                                if (operationCycle == 4) { CPU_Read = false; }
                                break;
                            case 5:// dummy write
                                Store(dl, addressBus);
                                break;
                            case 6:// read from address
                                PollInterrupts();
                                Op_INC(addressBus);
                                Op_SBC(dl);
                                operationComplete = true;
                                break;
                        }
                        break;
                    // And that's all 256 instructions!

                    default: return; // logically, this can never happen.
                }
                operationCycle++; // increment this for next CPU cycle.
                // If operationComplete is true, operationCycle will be set to 0 for next instruction.
            }
            if (DoDMCDMA && APU_ImplicitAbortDMC4015)
            {
                APU_ImplicitAbortDMC4015 = false; // If this was delayed by a write cycle, it won't run at all.
            }
        }


        public void ResetReadPush()
        {
            // the RESET instruction has unique behavior where it reads from the stack, and decrements the stack pointer.
            Fetch((ushort)(0x100 + stackPointer));
            stackPointer--;
        }

        public void Push(byte A)
        {
            // Store to the stack, and decrement the stack pointer.
            Store(A, (ushort)(0x100 + stackPointer));
            stackPointer--;
        }

        // I don't have a void for pop... All instructions that pull form the stack just perform the logic.


        ushort PPU_VRAM_MysteryAddress; // used during consecutive write cycles to VRAM. The PPU makes 2 extra writes to VRAM, and one of them I call "the mystery write".

        ushort PPU_AddressBus;  // the Address Bus of the PPU

        public ushort PPU_ReadWriteAddress = 0;// PPU Internal Register 'v'
        public ushort PPU_TempVRAMAddress = 0; // PPU Internal Register 't'. "can also be thought of as the address of the top left onscreen tile: https://www.nesdev.org/wiki/PPU_scrolling"
        /*
        The v and t registers are 15 bits:
        yyy NN YYYYY XXXXX
        ||| || ||||| +++++-- coarse X scroll
        ||| || +++++-------- coarse Y scroll
        ||| ++-------------- nametable select
        +++----------------- fine Y scroll
        */

        byte PPU_Update2006Delay;   // The number of PPU cycles to wait between writing to $2006 and the ppu from updating
        byte PPU_Update2005Delay;   // The number of PPU cycles to wait between writing to $2004 and the ppu from updating
        byte PPU_Update2005Value;   // The value written to $2005, for use when the delay has ended.
        byte PPU_Update2001Delay;   // The number of PPU cycles to wait between writing to $2001 and the ppu from updating
        byte PPU_Update2001EmphasisBitsDelay;   // The number of PPU cycles to wait between writing to $2001 and the ppu from updating the emphasis bits and greyscale
        byte PPU_Update2001OAMCorruptionDelay;  // The number of PPU cycles to wait before OAM gets corrupted if OAM corruption is occurring.
        byte PPU_Update2001Value;   // The value written to $2001, for use when the delay has ended.
        byte PPU_Update2000Delay;   // The number of PPU cycles to wait between writing to $2000 and the ppu from updating
        byte PPU_Update2000Value;   // The value written to $2000, for use when the delay has ended.
        ushort PPU_Update2006Value;   // The value written to $2006, for use when the delay has ended.
        ushort PPU_Update2006Value_Temp;

        bool PPU_WasRenderingBefore2001Write; // Were we rendering before writing to $2001? (used for OAM corruption)

        byte PPU_VRAMAddressBuffer = 0; // when reading from $2007, this buffer holds the value from VRAM that gets read. Updated after reading from $2007.

        bool PPUAddrLatch = false;  // Certain ppu registers take two writes to fully set things up. It's flipped when writing to $2005 and $2006. Reset when reading from $2002

        bool PPUControlIncrementMode32; // Set by writing to $2000. If set, the VRAM address is incremented by 32 instead of 1 after reads/writes to $2007.
        bool PPUControl_NMIEnabled;     // Set by writing to $2000. If set, the NMI can occur.

        public bool PPU_PatternSelect_Sprites; //which pattern table is used for sprites / background
        public bool PPU_PatternSelect_Background; //which pattern table is used for sprites / background

        //for logging purposes. doesn't update databus.
        bool DebugObserve = false;
        public byte Observe(ushort Address)
        {
            // Reading from anywhere goes through this function.
            if ((Address >= 0x8000))
            {
                // Reading from ROM.
                // Different mappers could rearrange the data from the ROM into different locations on the system bus.
                return MapperObserve(Address, Cart.MemoryMapper);
            }
            else if (Address < 0x2000)
            {
                // Reading from RAM.
                // Ram mirroring! Only addresses $0000 through $07FF exist in RAM, so ignore bits 11 and 12
                return RAM[Address & 0x7FF];
            }
            else if (Address >= 0x2000 && Address < 0x4000)
            {
                // PPU registers. most of these aren't meant to be read.
                Address = (ushort)(Address & 0x2007);
                switch (Address)
                {
                    case 0x2000:
                        // Write only. Return the PPU databus.
                        return PPUBus;
                    case 0x2001:
                        // Write only. Return the PPU databus.
                        return PPUBus;
                    case 0x2002:
                        // PPU Flags.
                        return (byte)((((PPUStatus_VBlank ? 0x80 : 0) | (PPUStatus_SpriteZeroHit ? 0x40 : 0) | (PPUStatus_SpriteOverflow ? 0x20 : 0)) & 0xE0) + (PPUBus & 0x1F));
                    case 0x2003:
                        // write only. Return the PPU databus.
                        return PPUBus;
                    case 0x2004:
                        // Read from OAM
                        return (byte)(ReadOAM() & 0xE3);
                    case 0x2005:
                        // write only. Return the PPU databus.
                        return PPUBus;
                    case 0x2006:
                        // write only. Return the PPU databus.
                        return PPUBus;
                    case 0x2007:
                        // Reading from VRAM.
                        return ObservePPU(PPU_ReadWriteAddress);
                }

            }
            else if (Address >= 0x4000 && Address <= 0x401F) // observe the APU registers
            {
                //addressBus 
                byte Reg = (byte)(Address & 0x1F);
                if (Reg == 0x15)
                {

                    byte InternalBus = dataBus;

                    InternalBus &= 0x20;
                    InternalBus |= (byte)(APU_Status_DMCInterrupt ? 0x80 : 0);
                    InternalBus |= (byte)(APU_Status_FrameInterrupt ? 0x40 : 0);
                    InternalBus |= (byte)((APU_DMC_BytesRemaining != 0 && APU_Status_DelayedDMC) ? 0x10 : 0); // see footnote.
                    InternalBus |= (byte)((APU_LengthCounter_Noise != 0) ? 0x08 : 0);
                    InternalBus |= (byte)((APU_LengthCounter_Triangle != 0) ? 0x04 : 0);
                    InternalBus |= (byte)((APU_LengthCounter_Pulse2 != 0) ? 0x02 : 0);
                    InternalBus |= (byte)((APU_LengthCounter_Pulse1 != 0) ? 0x01 : 0);
                    return InternalBus; // reading from $4015 can not affect the databus
                }
                else if (Reg == 0x16 || Reg == 0x17)
                {
                    return (byte)((((Reg == 0x16) ? (ControllerShiftRegister1 & 0x80) : (ControllerShiftRegister2 & 0x80)) == 0 ? 0 : 1) | (dataBus & 0xE0));
                }
            }
            else
            {
                //mapper chip stuff, but also open bus!
                return MapperObserve(Address, Cart.MemoryMapper);
            }

            return dataBus;
        }
        public byte Fetch(ushort Address)
        {
            DataPinsAreNotFloating = false;
            // Reading from anywhere goes through this function.
            if ((Address >= 0x8000))
            {
                // Reading from ROM.
                // Different mappers could rearrange the data from the ROM into different locations on the system bus.
                MapperFetch(Address, Cart.MemoryMapper);
                DataPinsAreNotFloating = true;
            }
            else if (Address < 0x2000)
            {
                // Reading from RAM.
                // Ram mirroring! Only addresses $0000 through $07FF exist in RAM, so ignore bits 11 and 12
                dataBus = RAM[Address & 0x7FF];
                DataPinsAreNotFloating = true;
            }
            else if (Address >= 0x2000 && Address < 0x4000)
            {
                // PPU registers. most of these aren't meant to be read.
                Address = (ushort)(Address & 0x2007);
                switch (Address)
                {
                    case 0x2000:
                        // Write only. Return the PPU databus.
                        dataBus = PPUBus;

                        break;
                    case 0x2001:
                        // Write only. Return the PPU databus.
                        dataBus = PPUBus;

                        break;
                    case 0x2002:
                        // PPU Flags.
                        dataBus = (byte)((((PPUStatus_VBlank ? 0x80 : 0) | (PPUStatus_SpriteZeroHit ? 0x40 : 0) | (PPUStatus_SpriteOverflow ? 0x20 : 0)) & 0xE0) + (PPUBus & 0x1F));
                        
                        PPUAddrLatch = false;
                        PPUStatus_VBlank = false;
                        PPU_PendingVBlank = false;
                        PPUBus = dataBus;
                        for (int i = 5; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                        
                        break;
                    case 0x2003:
                        // write only. Return the PPU databus.
                        dataBus = PPUBus; break;
                    case 0x2004:
                        // Read from OAM
                        dataBus = ReadOAM();
                        if ((PPUOAMAddress & 3) == 2)
                        {
                            dataBus &= 0xE3; // the attributes always return 0 for bits 2, 3, and 4
                        }
                        
                        PPUBus = dataBus;
                        for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                        
                        break;
                    case 0x2005:
                        // write only. Return the PPU databus.
                        dataBus = PPUBus; break;
                    case 0x2006:
                        // write only. Return the PPU databus.
                        dataBus = PPUBus; break;
                    case 0x2007:
                        // Reading from VRAM.

                        // if this is 1 CPU cycle after another read, there's interesting behavior.
                        if (PPU_Data_StateMachine == 3 && PPU_Data_StateMachine_Read)
                        {
                            //Behavior that is CPU/PPU alignment specific
                            if (PPUClock == 0)
                            {
                                dataBus = PPU_VRAMAddressBuffer; // just read the buffer
                            }
                            else if (PPUClock == 1)
                            {
                                PPU_Data_StateMachine_UpdateVRAMAddressEarly = true;
                                dataBus = PPU_VRAMAddressBuffer; // just read the buffer, but *also* the VRAM address will be updated early.

                            }
                            else if (PPUClock == 2)
                            {
                                PPU_Data_StateMachine_UpdateVRAMAddressEarly = true; // update the vram address early...

                                dataBus = (byte)(PPU_ReadWriteAddress & 0xFF); // the value read is not the buffer, but instead it's the low byte of the read/write address. 
                            }
                            else if (PPUClock == 3)
                            {
                                if (PPU_ReadWriteAddress >= 0x2000) // this is apparently different depending on where the read is? TODO: More testing required.
                                {
                                    if (PPU_VRAMAddressBuffer != 0)
                                    {
                                        // TODO: Inconsistent on real hardware, even with the same alignment.
                                    }
                                    dataBus = PPU_VRAMAddressBuffer; // with some bits missing
                                    PPU_Data_StateMachine_UpdateVRAMAddressEarly = true; // update the vram address early...

                                }
                                else
                                {
                                    PPU_Data_StateMachine_UpdateVRAMAddressEarly = true; // update the vram address early...

                                    dataBus = (byte)(PPU_ReadWriteAddress & 0xFF); // the value read is not the buffer, but instead it's the low byte of the read/write address. 
                                }
                            }
                        }
                        else // a normal read, not interrupting another read.
                        {
                            // this isn't a RMW instruction
                            if (PPU_ReadWriteAddress >= 0x3F00)
                            {
                                // reading from the palettes
                                PPU_AddressBus = PPU_ReadWriteAddress;
                                dataBus = FetchPPU((ushort)(PPU_AddressBus & 0x3FFF));
                            }
                            else
                            {
                                // not reading from the palettes, reading from the buffer.
                                dataBus = PPU_VRAMAddressBuffer;
                            }                               
                        }

                        // if the PPU state machine is not currently in progress...
                        if (PPU_Data_StateMachine == 9)
                        {
                            PPU_Data_StateMachine = 0; // start it at 0
                            if (PPUClock == 1 || PPUClock == 0)
                            {
                                // and if this is phase 0 or 1, the buffer is updated later.
                                PPU_Data_StateMachine_UpdateVRAMBufferLate = true;
                            }
                            if ((DoDMCDMA && (APU_Status_DMC || APU_ImplicitAbortDMC4015)))
                            {
                                PPU_ReadWriteAddress++; // I'm unsure on the timing of this, but I know the DMC DMA landing here ends up incrementing this one more time than my "state machine" currently runs.
                            }
                        }

                        PPU_Data_StateMachine_Read = true; // This is a read instruction, so the state machien needs to read.
                        PPU_Data_StateMachine_Read_Delayed = true; // This is also set, in case the state machine is interrupted.
                        PPUBus = dataBus;
                        for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                        
                        break;
                }
                DataPinsAreNotFloating = true;

            }
            else
            {
                //mapper chip stuff, but also open bus!
                MapperFetch(Address, Cart.MemoryMapper);
            }

            if (addressBus >= 0x4000 && addressBus <= 0x401F) // If APU registers are active, bus conflicts can occur. Or perhaps you are intentionally reading from the APU registers...
            {
                //addressBus 
                byte Reg = (byte)(Address & 0x1F);
                if (Reg == 0x15)
                {
                    
                    byte InternalBus = dataBus;

                    InternalBus &= 0x20;
                    InternalBus |= (byte)(APU_Status_DMCInterrupt ? 0x80 : 0);
                    InternalBus |= (byte)(APU_Status_FrameInterrupt ? 0x40 : 0);
                    InternalBus |= (byte)((APU_DMC_BytesRemaining != 0 && APU_Status_DelayedDMC) ? 0x10 : 0); // see footnote.
                    InternalBus |= (byte)((APU_LengthCounter_Noise != 0) ? 0x08 : 0);
                    InternalBus |= (byte)((APU_LengthCounter_Triangle != 0) ? 0x04 : 0);
                    InternalBus |= (byte)((APU_LengthCounter_Pulse2 != 0) ? 0x02 : 0);
                    InternalBus |= (byte)((APU_LengthCounter_Pulse1 != 0) ? 0x01 : 0);
                    
                    Clearing_APU_FrameInterrupt = true;
                    

                    // footnote:
                    // Consider the following. LDA #0, STA $4015, LDA $4015.
                    // The APU_DMC_BytesRemaining byte isn't cleared until 3 or 4 cycles after writing 0 to $4015.
                    // However, reading from $4015 after the needs to immediately have bit 4 cleared.

                    return InternalBus; // reading from $4015 can not affect the databus
                }
                else if (Reg == 0x16 || Reg == 0x17)
                {
                    byte ControllerRead = (byte)((((Reg == 0x16) ? (ControllerShiftRegister1 & 0x80) : (ControllerShiftRegister2 & 0x80)) == 0 ? 0 : 1) | (dataBus & 0xE0));

                    // controller ports
                    // grab 1 bit from the controller's shift register.
                    // also add the upper 3 bits of the databus.
                    
                    if (Reg == 0x16)
                    {
                        // if there are 2 CPU cycles in a row that read from this address, the registers don't get shifted
                        Controller1ShiftCounter = 2; // The shift register isn't shifted until this is 0, decremented in every APU PUT cycle
                    }
                    else
                    {
                        // if there are 2 CPU cycles in a row that read from this address, the registers don't get shifted
                        Controller2ShiftCounter = 2; // The shift register isn't shifted until this is 0, decremented in every APU PUT cycle
                    }
                    
                    APU_ControllerPortsStrobed = false; // This allows data to rapidly be streamed in through the A button if the controllers are read while strobed.
                    if (DoOAMDMA && DataPinsAreNotFloating) // If all the databus pins are floating, then the controller bits are visible. Otherwise... not so much.
                    {
                        return dataBus;
                    }
                    dataBus = ControllerRead;

                }
            }

            return dataBus;
        }

        /// <summary>
        /// Returns the value from the PPU RAM, or the cartridge's CHR RAM/ROM at the target PPU address. 
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte FetchPPU(ushort Address)
        {
            if (Cart == null)
            {
                return 0;
            }
            // when reading from the PPU's Video RAM, there's a lot of mapper-specific behavior to consider.
            Address &= 0x3FFF;
            if (Address < 0x2000)
            {
                if (Cart.UsingCHRRAM)
                {
                    return Cart.CHRRAM[Address];
                }
                else
                {
                    //Pattern Table
                    switch (Cart.MemoryMapper)
                    {
                        case 0: return Cart.CHRROM[Address & (Cart.CHRROM.Length - 1)];
                        case 1: // MMC1
                            // bit 4 of Mapper_1_Control controls how the pattern tables are swapped. if set, 2 banks of 4Kib. Otherwise, 1 8Kib bank
                            if ((Cart.Mapper_1_Control & 0x10) != 0)
                            {
                                // with the MMC1 chip, you can swap out the pattern tables.
                                // address < 0x1000 is the first pattern table, else, the second pattern table.
                                // if the final write for the MMC1 shift register was in the $A000 - $BFFF, this updates Cart.Mapper_1_CHR0
                                // if the final write for the MMC1 shift register was in the $B000 - $CFFF, this updates Cart.Mapper_1_CHR1
                                if (Address < 0x1000) { return Cart.CHRROM[((Cart.Mapper_1_CHR0 & 0x1F) * 0x1000 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else { Address &= 0xFFF; return Cart.CHRROM[((Cart.Mapper_1_CHR1 & 0x1F) * 0x1000 + Address) & (Cart.CHRROM.Length - 1)]; }
                            }
                            else // one swappable bank that changes both pattern tables.
                            {
                                // this uses the value written to Mapper_1_CHR0
                                return Cart.CHRROM[((Cart.Mapper_1_CHR0 & 0b11111110) * 0x2000 + Address) & (Cart.CHRROM.Length - 1)];
                            }
                        case 3: // CNROM
                            // by writing to any address $8000 or greater with CNROM, bits 0 and 1 determine the CHR bank.
                            return Cart.CHRROM[(Cart.Mapper_3_CHRBank * 0x2000 + Address) & (Cart.CHRROM.Length - 1)];
                        case 4:
                        case 118:
                        case 119: // MMC3
                            //Writes to $8000 determine the mode, writes to $8001 determine the banks
                            if ((Cart.Mapper_4_8000 & 0x80) == 0) // bit 7 of the previous write to $8000 determines which pattern table is 2 2kb banks, and which is 4 1kb banks.
                            {
                                if (Address < 0x800) { return Cart.CHRROM[(Cart.Mapper_4_CHR_2K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1000) { Address &= 0x7FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_2K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1400) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1C00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1KC * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            }
                            else
                            {
                                if (Address < 0x400) { return Cart.CHRROM[(Cart.Mapper_4_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0xC00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1000) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1KC * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1800) { Address &= 0x7FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_2K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else { Address &= 0x7FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_2K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            }
                        case 9: //MMC2                            
                            byte temp = 0;
                            ushort Addr = Address;
                            if (Address < 0x1000) { temp = Cart.CHRROM[(Cart.Mapper_9_Latch0_FE ? Cart.Mapper_9_CHR0_FE : Cart.Mapper_9_CHR0_FD) * 0x1000 + Addr]; }
                            else { Addr &= 0xFFF; temp = Cart.CHRROM[(Cart.Mapper_9_Latch1_FE ? Cart.Mapper_9_CHR1_FE : Cart.Mapper_9_CHR1_FD) * 0x1000 + Addr]; }
                            if (Address == 0x0FD8)
                            {
                                Cart.Mapper_9_Latch0_FE = false;
                            }
                            else if (Address == 0x0FE8)
                            {
                                Cart.Mapper_9_Latch0_FE = true;
                            }
                            else if (Address >= 0x1FD8 && Address <= 0x1FDF)
                            {
                                Cart.Mapper_9_Latch1_FE = false;
                            }
                            else if (Address >= 0x1FE8 && Address <= 0x1FEF)
                            {
                                Cart.Mapper_9_Latch1_FE = true;
                            }
                            return temp;
                        case 69: // Sunsoft FME-7
                            if (Address < 0x400) { return Cart.CHRROM[(Cart.Mapper_69_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K1 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0xC00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K2 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1000) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K3 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1400) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K5 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1C00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K6 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K7 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }

                    }
                    // if it wasn't any of those mappers, I still need to implement stuff.

                    return Cart.CHRROM[Address & (Cart.CHRROM.Length - 1)];
                }

            }
            else // if the VRAM address is >= $2000, we need to consider nametable mirroring.
            {
                Address = PPUAddressWithMirroring(Address);
                if (Address >= 0x3F00)
                {
                    ThisDotReadFromPaletteRAM = true;
                    // read from palette RAM.
                    // Palette RAM only returns bits 0-5, so bits 6 and 7 are PPU open bus.
                    return (byte)((PaletteRAM[Address & 0x1F] & 0x3F) | (PPUBus & 0xC0));
                }
                Address &= 0x7FF;
                return VRAM[Address];
            }
        }

        public byte ObservePPU(ushort Address)
        {
            // pretty much a copy of FetchPPU, except it doesn't trigger MMC2 stuff.
            if (Cart == null)
            {
                return 0;
            }
            // when reading from the PPU's Video RAM, there's a lot of mapper-specific behavior to consider.
            Address &= 0x3FFF;
            if (Address < 0x2000)
            {
                if (Cart.UsingCHRRAM)
                {
                    return Cart.CHRRAM[Address];
                }
                else
                {
                    //Pattern Table
                    switch (Cart.MemoryMapper)
                    {
                        case 0: return Cart.CHRROM[Address & (Cart.CHRROM.Length - 1)];
                        case 1: // MMC1
                            // bit 4 of Mapper_1_Control controls how the pattern tables are swapped. if set, 2 banks of 4Kib. Otherwise, 1 8Kib bank
                            if ((Cart.Mapper_1_Control & 0x10) != 0)
                            {
                                // with the MMC1 chip, you can swap out the pattern tables.
                                // address < 0x1000 is the first pattern table, else, the second pattern table.
                                // if the final write for the MMC1 shift register was in the $A000 - $BFFF, this updates Cart.Mapper_1_CHR0
                                // if the final write for the MMC1 shift register was in the $B000 - $CFFF, this updates Cart.Mapper_1_CHR1
                                if (Address < 0x1000) { return Cart.CHRROM[((Cart.Mapper_1_CHR0 & 0x1F) * 0x1000 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else { Address &= 0xFFF; return Cart.CHRROM[((Cart.Mapper_1_CHR1 & 0x1F) * 0x1000 + Address) & (Cart.CHRROM.Length - 1)]; }
                            }
                            else // one swappable bank that changes both pattern tables.
                            {
                                // this uses the value written to Mapper_1_CHR0
                                return Cart.CHRROM[((Cart.Mapper_1_CHR0 & 0b11111110) * 0x2000 + Address) & (Cart.CHRROM.Length - 1)];
                            }
                        case 3: // CNROM
                            // by writing to any address $8000 or greater with CNROM, bits 0 and 1 determine the CHR bank.
                            return Cart.CHRROM[(Cart.Mapper_3_CHRBank * 0x2000 + Address) & (Cart.CHRROM.Length - 1)];
                        case 4:
                        case 118:
                        case 119: // MMC3
                            //Writes to $8000 determine the mode, writes to $8001 determine the banks
                            if ((Cart.Mapper_4_8000 & 0x80) == 0) // bit 7 of the previous write to $8000 determines which pattern table is 2 2kb banks, and which is 4 1kb banks.
                            {
                                if (Address < 0x800) { return Cart.CHRROM[(Cart.Mapper_4_CHR_2K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1000) { Address &= 0x7FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_2K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1400) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1C00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1KC * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            }
                            else
                            {
                                if (Address < 0x400) { return Cart.CHRROM[(Cart.Mapper_4_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0xC00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1000) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_1KC * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else if (Address < 0x1800) { Address &= 0x7FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_2K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                                else { Address &= 0x7FF; return Cart.CHRROM[(Cart.Mapper_4_CHR_2K8 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            }
                        case 9: //MMC2                            
                            byte temp = 0;
                            ushort Addr = Address;
                            if (Address < 0x1000) { temp = Cart.CHRROM[(Cart.Mapper_9_Latch0_FE ? Cart.Mapper_9_CHR0_FE : Cart.Mapper_9_CHR0_FD) * 0x1000 + Addr]; }
                            else { Addr &= 0xFFF; temp = Cart.CHRROM[(Cart.Mapper_9_Latch1_FE ? Cart.Mapper_9_CHR1_FE : Cart.Mapper_9_CHR1_FD) * 0x1000 + Addr]; }
                            return temp;
                        case 69: // Sunsoft FME-7
                            if (Address < 0x400) { return Cart.CHRROM[(Cart.Mapper_69_CHR_1K0 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K1 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0xC00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K2 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1000) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K3 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1400) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K4 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1800) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K5 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else if (Address < 0x1C00) { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K6 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }
                            else { Address &= 0x3FF; return Cart.CHRROM[(Cart.Mapper_69_CHR_1K7 * 0x400 + Address) & (Cart.CHRROM.Length - 1)]; }

                    }
                    // if it wasn't any of those mappers, I still need to implement stuff.

                    return Cart.CHRROM[Address & (Cart.CHRROM.Length - 1)];
                }

            }
            else // if the VRAM address is >= $2000, we need to consider nametable mirroring.
            {
                Address = PPUAddressWithMirroring(Address);
                if (Address >= 0x3F00)
                {
                    // read from palette RAM.
                    // Palette RAM only returns bits 0-5, so bits 6 and 7 are PPU open bus.
                    return (byte)((PaletteRAM[Address & 0x1F] & 0x3F) | (PPUBus & 0xC0));
                }
                Address &= 0x7FF;
                return VRAM[Address];
            }
        }


        ushort PPUAddressWithMirroring(ushort Address)
        {
            // if the address is less than $2000, there is no mirroring.
            if (Address < 0x2000)
            {
                return Address;
            }

            // if the vram address is pointing to the color palettes:
            if (Address >= 0x3F00)
            {
                Address &= 0x3F1F;
                if ((Address & 3) == 0)
                {
                    Address &= 0x3F0F;
                }
                return Address;
            }
            Address &= 0x2FFF; // $3000 through $3F00 is always mirrored down.
            switch (Cart.MemoryMapper)
            {
                default:
                case 0: // NROM, just use the mirror setting from the iNES header.
                    if (!Cart.NametableHorizontalMirroring)
                    {
                        Address &= 0x37FF; // mask away $0800
                    }
                    else // horizontal
                    {
                        Address = (ushort)((Address & 0x33FF) | ((Address & 0x0800) >> 1)); // mask away $0C00, bit 10 becomes the former bit 11
                    }
                    break;
                case 1: // MMC1
                    switch (Cart.Mapper_1_Control & 3)
                    {
                        case 0: //one screen, low
                            Address &= 0x33FF;
                            break;
                        case 1: //one screen, high
                            Address &= 0x33FF;
                            Address |= 0x400;
                            break;
                        case 2: //vertical
                            Address &= 0x37FF; // mask away $0800
                            break;
                        case 3: //horizontal
                            Address = (ushort)((Address & 0x33FF) | ((Address & 0x0800) >> 1)); // mask away $0C00, bit 10 becomes the former bit 11

                            break;
                    }
                    break;
                case 4:
                case 118:
                case 119: // MMC3
                    if (Cart.Mapper_4_NametableMirroring) //horizontal
                    {
                        Address = (ushort)((Address & 0x33FF) | ((Address & 0x0800) >> 1)); // mask away $0C00, bit 10 becomes the former bit 11
                    }
                    else //vertical
                    {
                        Address &= 0x37FF; // mask away $0800
                    }
                    break;
                case 7: // AOROM
                    if ((Cart.Mapper_7_BankSelect & 0x10) == 0) // show nametable 0
                    {
                        Address &= 0x33FF;
                    }
                    else // show nametable 1
                    {
                        Address &= 0x33FF;
                        Address |= 0x400;
                    }
                    break;
                case 9: // MMC2
                    if (Cart.Mapper_9_NametableMirroring) //horizontal
                    {
                        Address = (ushort)((Address & 0x33FF) | ((Address & 0x0800) >> 1)); // mask away $0C00, bit 10 becomes the former bit 11
                    }
                    else //vertical
                    {
                        Address &= 0x37FF; // mask away $0800
                    }
                    break;
                case 69: // Sunsoft FME-7
                    switch (Cart.Mapper_69_NametableMirroring)
                    {
                        case 0: //vertical
                            Address &= 0x37FF; // mask away $0800
                            break;
                        case 1: //horizontal
                            Address = (ushort)((Address & 0x33FF) | ((Address & 0x0800) >> 1)); // mask away $0C00, bit 10 becomes the former bit 11
                            break;
                        case 2: //one-screen A
                            Address &= 0x33FF;
                            break;
                        case 3: //one-screen B
                            Address &= 0x33FF;
                            Address |= 0x400;
                            break;
                    }
                    break;
            }
            return Address;
        }

        byte MapperObserve(ushort Address, byte Mapper)
        {
            switch (Mapper)
            {
                default:
                case 0: //NROM
                    if (Address >= 0x8000)
                    {
                        return Cart.PRGROM[Address & (Cart.PRGROM.Length - 1)]; // Get the address form the ROM file. If the ROM only has $4000 bytes, this will make addresses > $BFFF mirrors of $8000 through $BFFF.
                    }
                    //open bus
                    return dataBus;

                case 1: //MMC1
                    if (Address >= 0x8000)
                    {
                        // The bank mode for MMC1:
                        byte MMC1PRGROMBankMode = (byte)((Cart.Mapper_1_Control & 0b01100) >> 2);
                        switch (MMC1PRGROMBankMode)
                        {
                            case 0:
                            case 1:
                                {
                                    // switch 32 KB at $8000, ignoring low bit of bank number
                                    ushort tempo = (ushort)(Address & 0x7FFF);
                                    return Cart.PRGROM[(0x8000 * (Cart.Mapper_1_PRG & 0x0E) + tempo) % Cart.PRGROM.Length];
                                }
                            case 2:
                                // fix first bank at $8000 and switch 16 KB bank at $C000
                                if (Address >= 0xC000)
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    return Cart.PRGROM[0x4000 * (Cart.Mapper_1_PRG) + tempo];
                                }
                                else
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    return Cart.PRGROM[tempo];
                                }
                            case 3:
                                // fix last bank at $C000 and switch 16 KB bank at $8000
                                if (Address >= 0xC000)
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    return Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + tempo];
                                }
                                else
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    return Cart.PRGROM[(0x4000 * (Cart.Mapper_1_PRG & 0x0F) + tempo) & (Cart.PRGROM.Length - 1)];
                                }
                        }
                    }
                    else // if the address is < $8000
                    {
                        if (((Cart.Mapper_1_PRG & 0x10) == 0)) // if Work RAM is enabled
                        {
                            return Cart.PRGRAM[Address & 0x1FFF];
                        }
                        // else, open bus.
                    }
                    //open bus
                    return dataBus;

                case 71:
                case 2: //UxROM
                    if (Address >= 0x8000)
                    {
                        if (Address >= 0xC000)
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            return Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + tempo];
                        }
                        else
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            return Cart.PRGROM[0x4000 * (Cart.Mapper_2_BankSelect & 0x0F) + tempo];
                        }
                    }
                    return dataBus;
                // case 3, CNROM doesn't have any PRG bank switching, so it shares the logic with NROM
                case 4:
                case 118:
                case 119:
                    //MMC3
                    if (Address >= 0xE000) // This bank is fixed the the final PRG bank of the ROM
                    {
                        return Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x3FFF)];
                    }
                    else if (Address >= 0xC000)
                    {
                        if ((Cart.Mapper_4_8000 & 0x40) == 0x40)
                        {
                            //$C000 swappable
                            return Cart.PRGROM[(Cart.Mapper_4_Bank8C << 13) | (Address & 0x1FFF)];
                        }
                        else
                        {
                            //$8000 swappable
                            return Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x1FFF)];
                        }
                    }
                    else if (Address >= 0xA000)
                    {
                        //$8000 swappable
                        return Cart.PRGROM[(Cart.Mapper_4_BankA << 13) | (Address & 0x1FFF)];
                    }
                    else if (Address >= 0x8000)
                    {
                        if ((Cart.Mapper_4_8000 & 0x40) == 0x40)
                        {
                            //$8000 swappable
                            return Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x1FFF)];
                        }
                        else
                        {
                            //$C000 swappable
                            return Cart.PRGROM[(Cart.Mapper_4_Bank8C << 13) | (Address & 0x1FFF)];
                        }
                    }
                    else if (Address >= 0x6000)
                    {
                        if ((Cart.Mapper_4_PRGRAMProtect & 0x80) != 0)
                        {
                            return Cart.PRGRAM[Address & 0x1FFF];
                        }
                        return dataBus;
                    }
                    //else, open bus
                    return dataBus;
                case 7: // AOROM
                    if (Address >= 0x8000)
                    {
                        ushort tempo = (ushort)(Address & 0x7FFF);
                        return Cart.PRGROM[(0x8000 * (Cart.Mapper_7_BankSelect & 0x07) + tempo) & (Cart.PRGROM.Length - 1)];
                    }
                    // AOROM doesn't have any PRG RAM
                    return dataBus;
                case 9: //MMC2
                    if (Address >= 0xA000)
                    {
                        return Cart.PRGROM[((Cart.PRG_Size - 2) << 14) | (Address & 0x7FFF)];
                    }
                    else
                    {
                        return Cart.PRGROM[(Cart.Mapper_9_BankSelect << 13) | (Address & 0x1FFF)];
                    }
                    return dataBus;
                case 69:
                    //Sunsoft FME-7 (used in Gimmick)
                    if (Address >= 0x6000)
                    {
                        ushort tempo = (ushort)(Address % 0x2000);
                        if (Address >= 0x6000)
                        {
                            //actions
                            if (Address < 0x8000)
                            {
                                if (Cart.Mapper_69_Bank_6_isRAM)
                                {
                                    if (Cart.Mapper_69_Bank_6_isRAMEnabled)
                                    {
                                        return Cart.PRGRAM[Address & 0x1FFF];
                                    }
                                    else
                                    {   //open bus
                                        return dataBus;
                                    }
                                }
                                else
                                {   //read from ROM
                                    return Cart.PRGROM[(Cart.Mapper_69_Bank_6 * 0x2000 + tempo) % Cart.PRGROM.Length];
                                }
                            }
                            else if (Address < 0xA000)
                            {
                                return Cart.PRGROM[(Cart.Mapper_69_Bank_8 * 0x2000 + tempo) % Cart.PRGROM.Length];
                            }
                            else if (Address < 0xC000)
                            {
                                return Cart.PRGROM[(Cart.Mapper_69_Bank_A * 0x2000 + tempo) % Cart.PRGROM.Length];
                            }
                            else if (Address < 0xE000)
                            {
                                return Cart.PRGROM[(Cart.Mapper_69_Bank_C * 0x2000 + tempo) % Cart.PRGROM.Length];
                            }
                            else
                            {
                                return Cart.PRGROM[Cart.PRGROM.Length - 0x2000 + tempo];
                            }
                        }
                    }
                    //open bus
                    return dataBus;

            }

        }

        void MapperFetch(ushort Address, byte Mapper)
        {
            switch (Mapper)
            {
                default:
                case 0: //NROM
                    if (Address >= 0x8000)
                    {
                        dataBus = Cart.PRGROM[Address & (Cart.PRGROM.Length - 1)]; // Get the address form the ROM file. If the ROM only has $4000 bytes, this will make addresses > $BFFF mirrors of $8000 through $BFFF.
                        DataPinsAreNotFloating = true;
                        return;
                    }
                    //open bus
                    return;

                case 1: //MMC1
                    if (Address >= 0x8000)
                    {
                        DataPinsAreNotFloating = true;
                        // The bank mode for MMC1:
                        byte MMC1PRGROMBankMode = (byte)((Cart.Mapper_1_Control & 0b01100) >> 2);
                        switch (MMC1PRGROMBankMode)
                        {
                            case 0:
                            case 1:
                                {
                                    // switch 32 KB at $8000, ignoring low bit of bank number
                                    ushort tempo = (ushort)(Address & 0x7FFF);
                                    dataBus = Cart.PRGROM[(0x8000 * (Cart.Mapper_1_PRG & 0x0E) + tempo) % Cart.PRGROM.Length];
                                    return;
                                }
                            case 2:
                                // fix first bank at $8000 and switch 16 KB bank at $C000
                                if (Address >= 0xC000)
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    dataBus = Cart.PRGROM[0x4000 * (Cart.Mapper_1_PRG) + tempo];
                                    return;
                                }
                                else
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    dataBus = Cart.PRGROM[tempo];
                                    return;
                                }
                            case 3:
                                // fix last bank at $C000 and switch 16 KB bank at $8000
                                if (Address >= 0xC000)
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    dataBus = Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + tempo];
                                    return;
                                }
                                else
                                {
                                    ushort tempo = (ushort)(Address & 0x3FFF);
                                    dataBus = Cart.PRGROM[(0x4000 * (Cart.Mapper_1_PRG & 0x0F) + tempo) & (Cart.PRGROM.Length - 1)];
                                    return;
                                }
                        }
                    }
                    else // if the address is < $8000
                    {
                        if (((Cart.Mapper_1_PRG & 0x10) == 0)) // if Work RAM is enabled
                        {
                            dataBus = Cart.PRGRAM[Address & 0x1FFF];
                            DataPinsAreNotFloating = true;
                            return;
                        }
                        // else, open bus.
                    }
                    //open bus
                    return;

                case 71:
                case 2: //UxROM
                    if (Address >= 0x8000)
                    {
                        DataPinsAreNotFloating = true;
                        if (Address >= 0xC000)
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            dataBus = Cart.PRGROM[Cart.PRGROM.Length - 0x4000 + tempo];
                            return;
                        }
                        else
                        {
                            ushort tempo = (ushort)(Address & 0x3FFF);
                            dataBus = Cart.PRGROM[0x4000 * (Cart.Mapper_2_BankSelect & 0x0F) + tempo];
                            return;
                        }
                    }
                    return;
                // case 3, CNROM doesn't have any PRG bank switching, so it shares the logic with NROM
                case 4:
                case 118:
                case 119:
                    //MMC3
                    if (Address >= 0xE000) // This bank is fixed the the final PRG bank of the ROM
                    {
                        DataPinsAreNotFloating = true;
                        dataBus = Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x3FFF)];
                        return;
                    }
                    else if (Address >= 0xC000)
                    {
                        DataPinsAreNotFloating = true;
                        if ((Cart.Mapper_4_8000 & 0x40) == 0x40)
                        {
                            //$C000 swappable
                            dataBus = Cart.PRGROM[(Cart.Mapper_4_Bank8C << 13) | (Address & 0x1FFF)];
                        }
                        else
                        {
                            //$8000 swappable
                            dataBus = Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x1FFF)];
                        }
                        return;
                    }
                    else if (Address >= 0xA000)
                    {
                        DataPinsAreNotFloating = true;
                        //$8000 swappable
                        dataBus = Cart.PRGROM[(Cart.Mapper_4_BankA << 13) | (Address & 0x1FFF)];

                        return;
                    }
                    else if (Address >= 0x8000)
                    {
                        DataPinsAreNotFloating = true;
                        if ((Cart.Mapper_4_8000 & 0x40) == 0x40)
                        {
                            //$8000 swappable
                            dataBus = Cart.PRGROM[(Cart.PRG_SizeMinus1 << 14) | (Address & 0x1FFF)];
                        }
                        else
                        {
                            //$C000 swappable
                            dataBus = Cart.PRGROM[(Cart.Mapper_4_Bank8C << 13) | (Address & 0x1FFF)];
                        }
                        return;
                    }
                    else if (Address >= 0x6000)
                    {
                        if ((Cart.Mapper_4_PRGRAMProtect & 0x80) != 0)
                        {
                            DataPinsAreNotFloating = true;
                            dataBus = Cart.PRGRAM[Address & 0x1FFF];
                        }
                        return;
                    }
                    //else, open bus
                    return;
                case 7: // AOROM
                    if (Address >= 0x8000)
                    {
                        DataPinsAreNotFloating = true;
                        ushort tempo = (ushort)(Address & 0x7FFF);
                        dataBus = Cart.PRGROM[(0x8000 * (Cart.Mapper_7_BankSelect & 0x07) + tempo) & (Cart.PRGROM.Length - 1)];
                    }
                    // AOROM doesn't have any PRG RAM
                    return;
                case 9: //MMC2
                    if (Address >= 0xA000)
                    {
                        dataBus = Cart.PRGROM[((Cart.PRG_Size - 2) << 14) | (Address & 0x7FFF)];
                    }
                    else
                    {
                        dataBus = Cart.PRGROM[(Cart.Mapper_9_BankSelect << 13) | (Address & 0x1FFF)];
                    }
                    return;
                case 69:
                    //Sunsoft FME-7 (used in Gimmick)
                    if (Address >= 0x6000)
                    {
                        ushort tempo = (ushort)(Address % 0x2000);
                        if (Address >= 0x6000)
                        {
                            //actions
                            if (Address < 0x8000)
                            {
                                if (Cart.Mapper_69_Bank_6_isRAM)
                                {
                                    if (Cart.Mapper_69_Bank_6_isRAMEnabled)
                                    {
                                        dataBus = Cart.PRGRAM[Address & 0x1FFF];
                                        DataPinsAreNotFloating = true;
                                        return;
                                    }
                                    else
                                    {   //open bus
                                        return;
                                    }
                                }
                                else
                                {   //read from ROM
                                    DataPinsAreNotFloating = true;
                                    dataBus = Cart.PRGROM[(Cart.Mapper_69_Bank_6 * 0x2000 + tempo) % Cart.PRGROM.Length];
                                    return;
                                }
                            }
                            else if (Address < 0xA000)
                            {
                                DataPinsAreNotFloating = true;
                                dataBus = Cart.PRGROM[(Cart.Mapper_69_Bank_8 * 0x2000 + tempo) % Cart.PRGROM.Length];
                                return;
                            }
                            else if (Address < 0xC000)
                            {
                                DataPinsAreNotFloating = true;
                                dataBus = Cart.PRGROM[(Cart.Mapper_69_Bank_A * 0x2000 + tempo) % Cart.PRGROM.Length];
                                return;
                            }
                            else if (Address < 0xE000)
                            {
                                DataPinsAreNotFloating = true;
                                dataBus = Cart.PRGROM[(Cart.Mapper_69_Bank_C * 0x2000 + tempo) % Cart.PRGROM.Length];
                                return;
                            }
                            else
                            {
                                DataPinsAreNotFloating = true;
                                dataBus = Cart.PRGROM[Cart.PRGROM.Length - 0x2000 + tempo];
                                return;
                            }
                        }
                    }
                    //open bus
                    return;

            }

        }

        byte ReadOAM()
        {
            if ((PPU_Mask_ShowBackground || PPU_Mask_ShowSprites) && PPU_Scanline < 240)
            {
                if (PPU_Dot > 0 && PPU_Dot <= 64)
                {
                    return 0xFF;
                }
                else if (PPU_Dot <= 256)
                {
                    return PPU_SpriteEvaluationTemp;
                }
                else if (PPU_Dot <= 320)
                {
                    return PPU_SpriteEvaluationTemp;
                }
                return OAM[PPUOAMAddress];
            }
            return OAM[PPUOAMAddress];
        }

        bool PPU_PendingVBlank;

        bool DataPinsAreNotFloating = false;   // used in controller reading + OAM DMA.
        public bool TAS_ReadingTAS;         // if we're reading inputs from a TAS, this will be set.
        public int TAS_InputSequenceIndex;  // which index from the TAS input log will be used for this current controller strobe?
        public ushort[] TAS_InputLog; // controller [22222222 11111111]
        public bool ClockFiltering = false; // If set, TAS_InputSequenceIndex increments every time the controllers are strobed (or clocked, if the controller is held strobing). Otherwise, "latch filtering" is used, incrementing TAS_InputSequenceIndex once a frame.
        public bool SyncFM2; // This is set if we're running an FM2 TAS, which (due to FCEUX's very incorrect timing of the first frame after power on) I need to start execution on scanline 240, and prevent the vblank flag from being set.
        public void Store(byte Input, ushort Address)
        {
            // This is used whenever writing anywhere with the CPU
            if (Address < 0x2000)
            {
                //guaranteed to be RAM

                RAM[Address & 0x7FF] = Input;

            }
            else if (Address < 0x4000)
            {
                // $2000 through $3FFF writes to the PPU registers
                StorePPURegisters(Address, Input);
            }
            else if (Address >= 0x4000 && Address <= 0x4015)
            {
                // Writing to $4000 through $4015 are APU registers
                switch (Address)
                {
                    default:
                        APU_Register[Address & 0xFF] = Input; break;
                    case 0x4003:
                        if (APU_Status_Pulse1)
                        {
                            APU_LengthCounter_ReloadValuePulse1 = APU_LengthCounterLUT[Input >> 3];
                            APU_LengthCounter_ReloadPulse1 = true;
                        }
                        APU_ChannelTimer_Pulse1 |= (ushort)((Input &= 0x7) << 8);
                        break;
                    case 0x4007:
                        if (APU_Status_Pulse2)
                        {
                            APU_LengthCounter_ReloadValuePulse2 = APU_LengthCounterLUT[Input >> 3];
                            APU_LengthCounter_ReloadPulse2 = true;
                        }
                        APU_ChannelTimer_Pulse2 |= (ushort)((Input &= 0x7) << 8);
                        break;
                    case 0x400B:
                        if (APU_Status_Triangle)
                        {
                            APU_LengthCounter_ReloadValueTriangle = APU_LengthCounterLUT[Input >> 3];
                            APU_LengthCounter_ReloadTriangle = true;

                        }
                        APU_ChannelTimer_Triangle |= (ushort)((Input &= 0x7) << 8);
                        break;
                    case 0x400F:
                        if (APU_Status_Noise)
                        {
                            APU_LengthCounter_ReloadValueNoise = APU_LengthCounterLUT[Input >> 3];
                            APU_LengthCounter_ReloadNoise = true;
                        }
                        break;

                    case 0x4010:
                        APU_DMC_EnableIRQ = (Input & 0x80) != 0;
                        APU_DMC_Loop = (Input & 0x40) != 0;
                        APU_DMC_Rate = APU_DMCRateLUT[Input & 0xF];
                        if (!APU_DMC_EnableIRQ)
                        {
                            APU_Status_DMCInterrupt = false;
                            IRQ_LevelDetector = false;
                        }
                        break;

                    case 0x4011:
                        APU_DMC_Output = (byte)(Input & 0x7F);

                        break;

                    case 0x4012:
                        APU_DMC_SampleAddress = (ushort)(0xC000 | (Input << 6));
                        break;

                    case 0x4013:
                        APU_DMC_SampleLength = (ushort)((Input << 4) | 1);
                        break;

                    case 0x4014:    //OAM DMA
                        DoOAMDMA = true;
                        FirstCycleOfOAMDMA = true;
                        DMAAddress = 0; // the starting address for the OAM DMC is always page aligned.
                        DMAPage = Input;
                        break;
                    case 0x4015:    //DMC DMA (and other audio channels)

                        APU_Status_DelayedDMC = (Input & 0x10) != 0;
                        APU_Status_Noise = (Input & 0x08) != 0;
                        APU_Status_Triangle = (Input & 0x04) != 0;
                        APU_Status_Pulse2 = (Input & 0x02) != 0;
                        APU_Status_Pulse1 = (Input & 0x01) != 0;

                        APU_DelayedDMC4015 = (byte)(APU_PutCycle ? 3 : 4); // Enable in 1 APU cycles, or 1.5 APU cycles. (it will be decremented later this cycle, so it's really like 2 : 3.

                        if (APU_Status_DelayedDMC && APU_DMC_BytesRemaining == 0)
                        {
                            // sets up the sample bytes_remaining and sample address.
                            StartDMCSample();
                            // However, the sample will only begin playing if the DMC is currently silent
                            if (APU_Silent)
                            {
                                DMCDMADelay = 2; // 2 APU cycles
                            }
                        }

                        if (!APU_Status_Noise) { APU_LengthCounter_Noise = 0; }
                        if (!APU_Status_Triangle) { APU_LengthCounter_Triangle = 0; }
                        if (!APU_Status_Pulse2) { APU_LengthCounter_Pulse2 = 0; }
                        if (!APU_Status_Pulse1) { APU_LengthCounter_Pulse1 = 0; }
                        APU_Status_DMCInterrupt = false;
                        IRQ_LevelDetector = false;

                        // Explicit abort stuff.
                        if (!APU_Status_DelayedDMC && ((APU_ChannelTimer_DMC == 2 && !APU_PutCycle) || (APU_ChannelTimer_DMC == APU_DMC_Rate && APU_PutCycle))) // this will be the APU cycle that fires a DMC DMA
                        {
                            APU_DelayedDMC4015 = (byte)(APU_PutCycle ? 5 : 6); // Disable in 2.5 APU cycles, or 3 APU cycles.
                            // basically, if the DMA has already begun, don't abort it for *this* edge case.
                        }

                        // Implicit abort stuff.
                        if (APU_Status_DelayedDMC && ((APU_ChannelTimer_DMC == 10 && !APU_PutCycle) || (APU_ChannelTimer_DMC == 8 && APU_PutCycle)))
                        {
                            // okay, so the series of events is as follows:
                            // the Load DMA will occur
                            // regardless of the buffer being empty, there will be a 1-cycle DMA that gets aborted 2 cycles after the load DMA ends.
                            APU_SetImplicitAbortDMC4015 = true; // This will occur in 8 (or 9) cpu cycles
                        }

                        break;
                }

            }
            else if (Address == 0x4016)
            {
                if (TAS_ReadingTAS)
                {
                    APU_ControllerPortsStrobing = ((Input & 1) != 0);
                }
                APU_ControllerPortsStrobing = ((Input & 1) != 0);
                if (!APU_ControllerPortsStrobing)
                {
                    APU_ControllerPortsStrobed = false;
                }
            }
            else if (Address == 0x4017)
            {
                APU_FrameCounterMode = (Input & 0x80) != 0;
                APU_FrameCounterInhibitIRQ = (Input & 0x40) != 0;
                if (APU_FrameCounterMode)
                {
                    APU_HalfFrameClock = true;
                    APU_QuarterFrameClock = true;
                }
                if (APU_FrameCounterInhibitIRQ)
                {
                    APU_Status_FrameInterrupt = false;
                    IRQ_LevelDetector = false;
                }
                APU_FrameCounterReset = (byte)((APU_PutCycle ? 3 : 4));
            }
            else if (Address >= 0x6000)
            {
                // mapper chip specific stuff- but also open bus!
                MapperStore(Input, Address, Cart.MemoryMapper);

            }
            else
            {
                // open bus!
                // this doesn't write anywhere, but it still updates the databus!
            }

            dataBus = Input;

        }

        public void StorePPURegisters(ushort Addr, byte In)
        {
            ushort AddrT = (ushort)((Addr & 0x2007));
            switch (AddrT)
            {
                case 0x2000:
                    // writing here updates a large amount of PPU flags
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    if (PPU_RESET)
                    {
                        return;
                    }

                    // NOTE: This uses the contents of the databus (instead of "In") for a single ppu cycle. (alignment dependent)
                    // this will be fixed on the next PPU cycle. no worries :)
                    // In other words, this can cause a visual bug if this write occurs on the wrong ppu cycle. (dot 257 of a visible scanline)
                    PPUControl_NMIEnabled = (In & 0x80) != 0;
                    PPUControlIncrementMode32 = (dataBus & 0x4) != 0;
                    PPU_Spritex16 = (dataBus & 0x20) != 0;           // these bits don't seem to be affected by open bus
                    PPU_PatternSelect_Sprites = (In & 0x8) != 0;     // these bits don't seem to be affected by open bus
                    PPU_PatternSelect_Background = (In & 0x10) != 0; // these bits don't seem to be affected by open bus
                    PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0111001111111111) | ((dataBus & 0x3) << 10)); // using 'databus' here for 1 ppu cycle is the cause of the scanline bug.

                    switch (PPUClock & 3) //depending on CPU/PPU alignment, the delay could be different.
                    {
                        case 0:
                            PPU_Update2000Delay = 2; break;
                        case 1:
                            PPU_Update2000Delay = 2; break;
                        case 2:
                            PPU_Update2000Delay = 1; break; // the bug does not happen, as this PPU cycle fixes it.
                        case 3:
                            PPU_Update2000Delay = 1; break; // the bug does not happen, as this PPU cycle fixes it.
                    }
                    PPU_Update2000Value = In;


                    break;

                case 0x2001:
                    // writing here updates a large amount of PPU flags
                    // Is the background being drawn? Are sprites being drawn? Greyscale / color emphasis?
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    if (PPU_RESET)
                    {
                        return;
                    }
                    switch (PPUClock & 3) //depending on CPU/PPU alignment, the delay could be different.
                    {
                        case 0:
                            PPU_Update2001Delay = 2; PPU_Update2001EmphasisBitsDelay = 2; PPU_Update2001OAMCorruptionDelay = 2; break;
                        case 1:
                            PPU_Update2001Delay = 2; PPU_Update2001EmphasisBitsDelay = 1; PPU_Update2001OAMCorruptionDelay = 3; break; // PPU_Update2001EmphasisBitsDelay is actually 2, but different behavior than case 0 and 3.
                        case 2:
                            PPU_Update2001Delay = 3; PPU_Update2001EmphasisBitsDelay = 1; PPU_Update2001OAMCorruptionDelay = 3; break; // PPU_Update2001EmphasisBitsDelay is actually 2, but different behavior than case 0 and 3.
                        case 3:
                            PPU_Update2001Delay = 2; PPU_Update2001EmphasisBitsDelay = 2; PPU_Update2001OAMCorruptionDelay = 2; break;
                    }
                    PPU_WasRenderingBefore2001Write = PPU_Mask_ShowBackground || PPU_Mask_ShowSprites;
                    bool temp_rendering = PPU_WasRenderingBefore2001Write;
                    bool temp_renderingFromInput = ((In & 0x08) != 0) || ((In & 0x10) != 0);
                    //PPU_Mask_8PxShowBackground = (dataBus & 0x02) != 0;
                    //PPU_Mask_8PxShowSprites = (dataBus & 0x04) != 0;
                    PPU_Mask_ShowBackground_Instant = (dataBus & 0x08) != 0;
                    PPU_Mask_ShowSprites_Instant = (dataBus & 0x10) != 0;



                    // disabling rendering can cause OAM corruption.
                    if (temp_rendering && !temp_renderingFromInput)
                    {
                        // we are disabling rendering inside vblank
                        if (PPU_Scanline < 241 || PPU_Scanline == 261)
                        {
                            PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = true; // used in the next cycle of sprite evaluation
                            if ((PPU_Dot & 7) < 2 && PPU_Dot <= 250)
                            {
                                // Palette corruption only occurs if rendering was disabled during the first 2 dots of a nametable fetch
                                if ((PPU_ReadWriteAddress & 0x3FFF) >= 0x3C00) // palette corruption only appears to occur when disabling rendering if the VRAM address is currently greater than 3C00
                                {
                                    PPU_PaletteCorruptionRenderingDisabledOutOfVBlank = true; // used in the color calculation for the next dot being drawn
                                }
                            }
                        }
                    }
                    else if (!temp_rendering && temp_renderingFromInput)
                    {
                        if (PPU_Scanline < 241 || PPU_Scanline == 261)
                        {
                            // if re-enabling rendering outside vblank
                            if (PPU_PendingOAMCorruption)
                            {
                                // If OAM corruption is going to occur
                                if (PPUClock == 1 || PPUClock == 2)
                                {
                                    // if on clock alignment 1 or 2, it doesn't happen!
                                    PPU_OAMCorruptionRenderingEnabledOutOfVBlank = true;
                                }
                            }
                        }
                    }

                    // this part happens immediately though?
                    if (PPU_Update2001EmphasisBitsDelay == 2)
                    {
                        PPU_Mask_Greyscale = (dataBus & 0x01) != 0;
                        PPU_Mask_EmphasizeBlue = (dataBus & 0x80) != 0;
                    }
                    else
                    {
                        PPU_Update2001EmphasisBitsDelay++; // it's always 2.
                    }
                    PPU_Mask_EmphasizeRed = (In & 0x20) != 0;
                    PPU_Mask_EmphasizeGreen = (In & 0x40) != 0;

                    PPU_Update2001Value = In;

                    break;

                case 0x2002: // this value is Read only.
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    break;

                case 0x2003:
                    // writing here updates the OAM address
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    PPUOAMAddress = PPUBus;
                    break;

                case 0x2004:
                    // writing here updates the OAM byte at the current OAM address
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    if (((PPU_Scanline >= 240 && PPU_Scanline < 261) && (PPU_Mask_ShowBackground || PPU_Mask_ShowSprites)) || (!PPU_Mask_ShowBackground && !PPU_Mask_ShowSprites))
                    {
                        if ((PPUOAMAddress & 3) == 2)
                        {
                            In &= 0xE3;
                        }
                        OAM[PPUOAMAddress] = In;
                        PPUOAMAddress++;
                    }
                    else
                    {
                        PPUOAMAddress += 4;
                        PPUOAMAddress &= 0xFC;

                    }
                    break;

                case 0x2005:
                    // writing here updates the X and Y scroll
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    if (PPU_RESET)
                    {
                        return;
                    }
                    switch (PPUClock & 3) //depending on CPU/PPU alignment, the delay could be different.
                    {
                        case 0: PPU_Update2005Delay = 1; break;
                        case 1: PPU_Update2005Delay = 1; break;
                        case 2: PPU_Update2005Delay = 2; break;
                        case 3: PPU_Update2005Delay = 1; break;
                    }
                    PPU_Update2005Value = In;
                    // There's a slight delay before the PPU updates the scroll with the correct values.
                    // In the meantime, it uses the value from the databus.
                    if (!PPUAddrLatch)
                    {
                        PPU_FineXScroll = (byte)(dataBus & 7);
                        PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0111111111100000) | (dataBus >> 3));
                    }
                    else
                    {
                        PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0000110000011111) | (((dataBus & 0xF8) << 2) | ((dataBus & 7) << 12)));
                    }
                    break;

                case 0x2006:
                    // writing here updates the PPU's read/write address.
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    if (PPU_RESET)
                    {
                        return;
                    }

                    if (!PPUAddrLatch)
                    {
                        PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b000000011111111) | ((In & 0x3F) << 8));

                    }
                    else
                    {
                        PPU_TempVRAMAddress = (ushort)((PPU_TempVRAMAddress & 0b0111111100000000) | (In));
                        PPU_Update2006Value = PPU_TempVRAMAddress;
                        PPU_Update2006Value_Temp = PPU_ReadWriteAddress;
                        switch (PPUClock & 3) //depending on CPU/PPU alignment, the delay could be different.
                        {
                            case 0: PPU_Update2006Delay = 4; break;
                            case 1: PPU_Update2006Delay = 4; break;
                            case 2: PPU_Update2006Delay = 5; break;
                            case 3: PPU_Update2006Delay = 4; break;
                        }
                    }
                    PPUAddrLatch = !PPUAddrLatch;

                    break;

                case 0x2007:
                    // writing here updates the byte at the current read/write address
                    PPUBus = In;
                    for (int i = 0; i < 8; i++) { PPUBusDecay[i] = PPUBusDecayConstant; }
                    PPU_Data_StateMachine_InputValue = In;

                    ushort Address = PPU_ReadWriteAddress;
                    // This if statement is only relevent in an edge case. Read-Modify-Write instructions to $2007 are *complicated*.
                    if (PPU_Data_StateMachine == 3 || PPU_Data_StateMachine == 6) // This write follows another read/write cycle
                    {
                        // during Read-Modify-Write instructions to $2007, there's alignment specific side effects.
                        PPU_VRAM_MysteryAddress = (ushort)(Address & 0xFF00 | In);
                        if (!PPU_Data_StateMachine_Read)
                        {
                            PPU_Data_StateMachine_PerformMysteryWrite = true;
                        }
                        else
                        {
                            PPU_Data_StateMachine_InterruptedReadToWrite = true;
                        }
                    }
                    else
                    {
                        // if this isn't interrupting the PPU's state machine due to a read-modify-write, don't worry about all that.
                        PPU_Data_StateMachine_NormalWriteBehavior = true;
                    }

                    if (PPU_Data_StateMachine != 3) // as long as this isn't 1 CPU cycle after the previous access to $2007...
                    {
                        if (PPU_Data_StateMachine == 9) // If this is not interrupting the state machine. (This is just a standard write to the $2007. No back-to-back cycles reading/writing)
                        {
                            PPU_Data_StateMachine = 3; // then the ppu VRAM read/write address needs to be updated *next* cycle.
                        }
                        else
                        {
                            PPU_Data_StateMachine = 0; // otherwise, the state machine will need to go back to zero.
                        }
                        PPU_Data_StateMachine_Read = false; // this is a write, not a read.
                    }
                    else
                    {
                        PPU_Data_StateMachine_Read_Delayed = false; // this is a write, not a read, but we likely just cut off a read.
                    }

                    break;
                // and that's it for the ppu registers!

                default: break; //should never happen
            }


        }

        void StorePPUData(ushort Address, byte In)
        {
            // writing to the PPU's VRAM.
            // first, check if the address has any mirroring going on:
            Address = PPUAddressWithMirroring(Address);
            if (Address < 0x2000) // if this is pointing to CHR RAM
            {
                Cart.CHRRAM[Address] = In;
            }
            else if (Address >= 0x3F00)
            {
                PaletteRAM[Address & 0x1F] = In;
            }
            else // if this is not pointing to CHR RAM or palettes
            {
                VRAM[Address & 0x7FF] = In;

            }
        }

        void MapperStore(byte Input, ushort Address, byte Mapper)
        {
            // Storing to mapper specific registers
            // Address should always be 0x6000 or greater
            switch (Mapper)
            {
                default:
                    return;
                case 1:// MMC1
                    if (Address < 0x8000) //WRAM not available on MMC1A
                    {
                        if (((Cart.Mapper_1_PRG & 0x10) == 0) /*&& Mapper != 1*/)
                        {
                            //Battery backed RAM
                            Cart.PRGRAM[Address & 0x1FFF] = Input;
                            return;
                        }
                        else
                        {
                            return; //do nothing
                        }
                    }
                    else
                    {   // shift the shirftRegister and add the new bit
                        Cart.Mapper_1_PB = (Cart.Mapper_1_ShiftRegister & 1) == 1;
                        Cart.Mapper_1_ShiftRegister >>= 1;
                        Cart.Mapper_1_ShiftRegister |= (byte)((Input & 1) << 4);
                    }
                    if (Cart.Mapper_1_PB) // if the '1' that was initialized in bit 4 is shifted into the bus
                    {
                        // copy shift register to the desired internal register.
                        switch (Address & 0xE000)
                        {
                            case 0x8000: //control
                                Cart.Mapper_1_Control = Cart.Mapper_1_ShiftRegister;
                                break;
                            case 0xA000: //CHR0
                                Cart.Mapper_1_CHR0 = Cart.Mapper_1_ShiftRegister;
                                break;
                            case 0xC000: //CHR1
                                Cart.Mapper_1_CHR1 = Cart.Mapper_1_ShiftRegister;
                                break;
                            case 0xE000: //PRG
                                Cart.Mapper_1_PRG = Cart.Mapper_1_ShiftRegister;
                                break;
                        }
                        Cart.Mapper_1_ShiftRegister = 0b10000;
                    }
                    if ((Input & 0b10000000) != 0)
                    {
                        Cart.Mapper_1_ShiftRegister = 0b10000;
                        Cart.Mapper_1_Control |= 0b01100;
                    }
                    break;

                case 71:
                case 2: //UxROM
                    if (Address >= 0x8000)
                    {
                        Cart.Mapper_2_BankSelect = (byte)(Input & 0xF);
                    }
                    return;
                case 3: //CNROM
                    if (Address >= 0x8000)
                    {
                        Cart.Mapper_3_CHRBank = (byte)(Input & 0x3);
                    }
                    return;
                case 4:
                case 118:
                case 119:   //MMC3
                    if (Address < 0x8000)
                    {   //Battery backed RAM
                        if ((Cart.Mapper_4_PRGRAMProtect & 0xC0) != 0) // bit 7 enables PRG RAM, bit 6 enables writing there.
                        {
                            Cart.PRGRAM[Address & 0x1FFF] = Input;
                        }
                        return;
                    }
                    else
                    {   //MMC3 actions
                        ushort tempo = (ushort)(Address & 0xE001);
                        switch (tempo)
                        {
                            case 0x8000:
                                Cart.Mapper_4_8000 = Input;
                                return;
                            case 0x8001:
                                byte mode = (byte)(Cart.Mapper_4_8000 & 7);
                                switch (mode)
                                {
                                    case 0: //PPU ($0000 - $07FF) ?+ $1000
                                        Cart.Mapper_4_CHR_2K0 = (byte)(Input & 0xFE);
                                        return;
                                    case 1: //PPU ($0800 - $0FFF) ?+ $1000
                                        Cart.Mapper_4_CHR_2K8 = (byte)(Input & 0xFE);
                                        return;
                                    case 2: //PPU ($1000 - $13FF) ?- $1000
                                        Cart.Mapper_4_CHR_1K0 = Input;
                                        return;
                                    case 3: //PPU ($1400 - $17FF) ?- $1000
                                        Cart.Mapper_4_CHR_1K4 = Input;
                                        return;
                                    case 4: //PPU ($1800 - $1BFF) ?- $1000
                                        Cart.Mapper_4_CHR_1K8 = Input;
                                        return;
                                    case 5: //PPU ($1C00 - $1FFF) ?- $1000
                                        Cart.Mapper_4_CHR_1KC = Input;
                                        return;
                                    case 6: //PRG ($8000 - $9FFF) ?+ 0x4000
                                        Cart.Mapper_4_Bank8C = (byte)(Input & (Cart.PRG_Size * 2 - 1));
                                        return;
                                    case 7: //PRG ($A000 - $BFFF)
                                        Cart.Mapper_4_BankA = (byte)(Input & (Cart.PRG_Size * 2 - 1));
                                        return;
                                }
                                return;
                            case 0xA000:
                                Cart.Mapper_4_NametableMirroring = (Input & 1) == 1;
                                return;
                            case 0xA001:
                                Cart.Mapper_4_PRGRAMProtect = Input;
                                return;
                            case 0xC000:
                                Cart.Mapper_4_IRQLatch = Input;
                                return;
                            case 0xC001:
                                Cart.Mapper_4_IRQCounter = 0xFF;
                                Cart.Mapper_4_ReloadIRQCounter = true;
                                return;
                            case 0xE000:
                                Cart.Mapper_4_EnableIRQ = false;
                                IRQ_LevelDetector = false;
                                return;
                            case 0xE001:
                                Cart.Mapper_4_EnableIRQ = true;
                                return;
                        }
                    }
                    break;
                case 7: //AOROM
                    if (Address >= 0x8000)
                    {
                        Cart.Mapper_7_BankSelect = Input;
                    }
                    break;
                case 9: //MMC2
                    if (Address < 0xA000)
                    {
                        // nothing
                    }
                    else if (Address < 0xB000) // PRG Bank select
                    {
                        Cart.Mapper_9_BankSelect = (byte)(Input & 0x0F);
                    }
                    else if (Address < 0xC000) // CHR0 Bank select
                    {
                        Cart.Mapper_9_CHR0_FD = (byte)(Input & 0x1F);
                    }
                    else if (Address < 0xD000) // CHR0 Bank select
                    {
                        Cart.Mapper_9_CHR0_FE = (byte)(Input & 0x1F);
                    }
                    else if (Address < 0xE000) // CHR1 Bank select
                    {
                        Cart.Mapper_9_CHR1_FD = (byte)(Input & 0x1F);
                    }
                    else if (Address < 0xF000) // CHR1 Bank select
                    {
                        Cart.Mapper_9_CHR1_FE = (byte)(Input & 0x1F);
                    }
                    else // Nametable mirroring
                    {
                        Cart.Mapper_9_NametableMirroring = (Input & 0x1) == 1;
                    }
                    break;
                case 69://Sunsoft FME-7 (used in Gimmick)
                    if (Address >= 0x6000)
                    {
                        //actions
                        if (Address < 0x8000)
                        {
                            if (Cart.Mapper_69_Bank_6_isRAM)
                            {
                                if (Cart.Mapper_69_Bank_6_isRAMEnabled)
                                {
                                    //writing to RAM
                                    Cart.PRGRAM[Address & 0x1FFF] = Input;
                                } //else, writing to open bus
                            } //else it's ROM. writing here does nothing.
                        }
                        else if (Address < 0xA000)
                        {
                            Cart.Mapper_69_CMD = (byte)(Input & 0x0F);
                        }
                        else if (Address < 0xC000)
                        {
                            switch (Cart.Mapper_69_CMD)
                            {
                                case 0: Cart.Mapper_69_CHR_1K0 = Input; break;
                                case 1: Cart.Mapper_69_CHR_1K1 = Input; break;
                                case 2: Cart.Mapper_69_CHR_1K2 = Input; break;
                                case 3: Cart.Mapper_69_CHR_1K3 = Input; break;
                                case 4: Cart.Mapper_69_CHR_1K4 = Input; break;
                                case 5: Cart.Mapper_69_CHR_1K5 = Input; break;
                                case 6: Cart.Mapper_69_CHR_1K6 = Input; break;
                                case 7: Cart.Mapper_69_CHR_1K7 = Input; break;
                                case 8: Cart.Mapper_69_Bank_6 = (byte)(Input & 0x3F); Cart.Mapper_69_Bank_6_isRAM = (Input & 0x40) != 0; Cart.Mapper_69_Bank_6_isRAMEnabled = (Input & 0x80) != 0; break;
                                case 9: Cart.Mapper_69_Bank_8 = (byte)(Input & 0x3F); break;
                                case 10: Cart.Mapper_69_Bank_A = (byte)(Input & 0x3F); break;
                                case 11: Cart.Mapper_69_Bank_C = (byte)(Input & 0x3F); break;
                                case 12: Cart.Mapper_69_NametableMirroring = (byte)(Input & 0x3); break;
                                case 13: Cart.Mapper_69_EnableIRQ = (Input & 0x1) != 0; Cart.Mapper_69_EnableIRQCounterDecrement = (Input & 0x80) != 0; IRQ_LevelDetector = false; break;
                                case 14: Cart.Mapper_69_IRQCounter = (ushort)((Cart.Mapper_69_IRQCounter & 0xFF00) | Input); break;
                                case 15: Cart.Mapper_69_IRQCounter = (ushort)((Cart.Mapper_69_IRQCounter & 0xFF) | (Input << 8)); break;
                            }
                        } // else do nothing
                    }
                    break;
            }


        }

        void StartDMCSample()
        {
            // This runs when writing to $4015, or if a DPCM sample is looping and needs to restart.
            APU_DMC_AddressCounter = APU_DMC_SampleAddress;
            APU_DMC_BytesRemaining = APU_DMC_SampleLength;
        }


        #region GetAddressFunctions

        // these functions are used inside the giant opcode switch statement.

        void GetImmediate()
        {
            // Fetch the value at the program counter, store it in the DataLatch, and increment the Program Counter.
            dl = Fetch(programCounter);
            programCounter++;
            addressBus = programCounter;
        }

        void GetAddressAbsolute()
        {
            // Fetch the value at the PC, and write to either the High byte or Low byte of the 16 bit address bus. Also increment the Program Counter.
            if (operationCycle == 1)
            {
                // fetch address low
                dl = Fetch(programCounter);
            }
            else
            {
                // fetch address high
                addressBus = (ushort)(dl | (Fetch(programCounter) << 8));
            }
            programCounter++;
        }

        void GetAddressZeroPage()
        {
            // Fetch the value at the PC, and this 8 bit value replaces the contents of the 16 bit address bus.
            addressBus = Fetch(programCounter);
            programCounter++;
        }

        void GetAddressIndOffX()
        {
            // Fetch the value from the PC, then using that value as an 8-bit address on the zero page, add the X register, then set the High byte and Low byte of the Address Bus from there.
            switch (operationCycle)
            {
                case 1: // fetch pointer address
                    addressBus = Fetch(programCounter);
                    programCounter++;
                    break;
                case 2: // Add X
                    // dummy read
                    Fetch(addressBus);
                    addressBus = (byte)(addressBus + X);
                    break;
                case 3: // fetch address low
                    dl = Fetch((byte)(addressBus));
                    break;
                case 4: // fetch address high
                    addressBus = (ushort)(dl | (Fetch((byte)(addressBus + 1)) << 8));
                    break;
            }
        }

        void GetAddressIndOffY(bool TakeExtraCycleOnlyIfPageBoundaryCrossed)
        {
            // Some instructions will always take 4 cycles to determine the address, and others will normally take 3, but take the extra cycle if a page boundary was crossed.

            // either way, the general gist of this function is:
            // Fetch the value from the PC. use that 8 bit location on the zero page to fetch the High and Low byte of the new Address Bus location, then add Y to that.
            if (TakeExtraCycleOnlyIfPageBoundaryCrossed)
            {
                switch (operationCycle)
                {
                    case 1: // fetch pointer address
                        addressBus = Fetch(programCounter);
                        programCounter++;
                        break;
                    case 2: // fetch address low
                        dl = Fetch((byte)(addressBus));
                        break;
                    case 3: // fetch address high, add Y to low byte
                        addressBus = (ushort)(dl | (Fetch((byte)(addressBus + 1)) << 8));
                        temporaryAddress = addressBus;
                        H = (byte)(addressBus >> 8);
                        if (((temporaryAddress + Y) & 0xFF00) == (temporaryAddress & 0xFF00))
                        {
                            operationCycle++; //skip next cycle
                        }
                        addressBus = (ushort)((addressBus & 0xFF00) | ((addressBus + Y) & 0xFF));
                        break;
                    case 4: // increment high byte
                        dl = Fetch(addressBus); // dummy read
                        H = (byte)(addressBus >> 8);
                        H++; // This is incremented.
                        addressBus += 0x100;
                        break;
                }
            }
            else
            {
                switch (operationCycle)
                {
                    case 1: // fetch pointer address
                        addressBus = Fetch(programCounter);
                        programCounter++;
                        break;
                    case 2: // fetch address low
                        dl = Fetch((byte)(addressBus));
                        break;
                    case 3: // fetch address high, add Y to low byte
                        addressBus = (ushort)(dl | (Fetch((byte)(addressBus + 1)) << 8));
                        temporaryAddress = addressBus;
                        addressBus = (ushort)((addressBus & 0xFF00) | ((addressBus + Y) & 0xFF));
                        break;
                    case 4: // increment high byte
                        dl = Fetch(addressBus); // dummy read
                        H = (byte)(addressBus >> 8);
                        H++; // This is incremented.
                        if (((temporaryAddress + Y) & 0xFF00) != (temporaryAddress & 0xFF00))
                        {
                            addressBus += 0x100; // really, this would just replace the high byte with H, but this is less computationally expensive
                        }
                        break;
                }
            }

        }

        void GetAddressZPOffX()
        {
            // Fetch the value from the PC, then add X to that.
            if (operationCycle == 1)
            {
                // fetch address
                addressBus = Fetch(programCounter);
                programCounter++;
            }
            else
            {
                // dummy read, and add X
                dl = Fetch(addressBus);
                addressBus = (byte)(addressBus + X);
            }
        }

        void GetAddressZPOffY()
        {
            // Fetch the value from the PC, then add Y to that.
            if (operationCycle == 1)
            {
                // fetch address
                addressBus = Fetch(programCounter);
                programCounter++;
            }
            else
            {
                // dummy read, and add Y
                dl = Fetch(addressBus);
                addressBus = (byte)(addressBus + Y);
            }
        }

        void GetAddressAbsOffX(bool TakeExtraCycleIfPageBoundaryCrossed)
        {
            // Some instructions will always take 4 cycles to determine the address, and others will normally take 3, but take the extra cycle if a page boundary was crossed.

            // Fetch the High and Low byte values from the byte at the PC, then add X.
            if (TakeExtraCycleIfPageBoundaryCrossed)
            {
                switch (operationCycle)
                {
                    case 1: // fetch address low
                        dl = Fetch(programCounter);
                        programCounter++;

                        break;
                    case 2: // fetch address high, add Y to low byte
                        addressBus = (ushort)(dl | Fetch(programCounter) << 8);
                        temporaryAddress = addressBus;
                        H = (byte)(addressBus >> 8);

                        if (((temporaryAddress + X) & 0xFF00) == (temporaryAddress & 0xFF00))
                        {
                            operationCycle++; //skip next cycle
                            FixHighByte = false;
                        }
                        else
                        {
                            FixHighByte = true;
                        }

                        addressBus = (ushort)((addressBus & 0xFF00) | ((addressBus + X) & 0xFF));
                        programCounter++;

                        break;
                    case 3: // increment high byte
                        dl = Fetch(addressBus);
                        H = (byte)(addressBus >> 8);
                        H++;
                        if (FixHighByte)
                        {
                            addressBus += 0x100;
                        }
                        break;
                    case 4: // dummy read
                        dl = Fetch(addressBus); // read into pd
                        break;
                }
            }
            else
            {
                switch (operationCycle)
                {
                    case 1: // fetch address low
                        dl = Fetch(programCounter);
                        programCounter++;

                        break;
                    case 2: // fetch address high, add Y to low byte
                        addressBus = (ushort)(dl | Fetch(programCounter) << 8);
                        temporaryAddress = addressBus;
                        addressBus = (ushort)((addressBus & 0xFF00) | ((addressBus + X) & 0xFF));
                        programCounter++;

                        break;
                    case 3: // fix high byte if applicable
                        dl = Fetch(addressBus); // read into pd
                        H = (byte)(addressBus >> 8);
                        H++;
                        if (((temporaryAddress + X) & 0xFF00) != (temporaryAddress & 0xFF00))
                        {
                            addressBus += 0x100;
                        }
                        break;
                    case 4: // dummy read
                        dl = Fetch(addressBus); // read into pd
                        break;
                }
            }
        }
        bool FixHighByte = false;
        void GetAddressAbsOffY(bool TakeExtraCycleIfPageBoundaryCrossed)
        {
            // Some instructions will always take 4 cycles to determine the address, and others will normally take 3, but take the extra cycle if a page boundary was crossed.

            // Fetch the High and Low byte values from the byte at the PC, then add Y.
            if (TakeExtraCycleIfPageBoundaryCrossed)
            {
                switch (operationCycle)
                {
                    case 1: // fetch address low
                        dl = Fetch(programCounter);
                        programCounter++;

                        break;
                    case 2: // fetch address high, add Y to low byte
                        addressBus = (ushort)(dl | Fetch(programCounter) << 8);
                        temporaryAddress = addressBus;
                        H = (byte)(addressBus >> 8);

                        if (((temporaryAddress + Y) & 0xFF00) == (temporaryAddress & 0xFF00))
                        {
                            operationCycle++; //skip next cycle
                            FixHighByte = false;
                        }
                        else
                        {
                            FixHighByte = true;
                        }

                        addressBus = (ushort)((addressBus & 0xFF00) | ((addressBus + Y) & 0xFF));
                        programCounter++;

                        break;
                    case 3: // increment high byte
                        dl = Fetch(addressBus);
                        H = (byte)(addressBus >> 8);
                        H++;
                        if (FixHighByte)
                        {
                            addressBus += 0x100;
                        }
                        break;
                    case 4: // dummy read
                        dl = Fetch(addressBus); // read into databus
                        break;
                }
            }
            else
            {
                switch (operationCycle)
                {
                    case 1: // fetch address low
                        dl = Fetch(programCounter);
                        programCounter++;

                        break;
                    case 2: // fetch address high, add Y to low byte
                        addressBus = (ushort)(dl | Fetch(programCounter) << 8);
                        temporaryAddress = addressBus;
                        addressBus = (ushort)((addressBus & 0xFF00) | ((addressBus + Y) & 0xFF));
                        programCounter++;

                        break;
                    case 3: // fix high byte if applicable
                        dl = Fetch(addressBus); // read into pd
                        H = (byte)(addressBus >> 8);
                        H++;
                        if (((temporaryAddress + Y) & 0xFF00) != (temporaryAddress & 0xFF00))
                        {
                            addressBus += 0x100;
                        }
                        break;
                    case 4: // dummy read
                        dl = Fetch(addressBus); // read into pd
                        break;
                }
            }
        }
        #endregion

        #region OpFunctions

        // This is not every instruction!!!
        // These are just the ones that have frequently repeated logic.
        // Instructions like STA just simply `Store(A, Address);`, which doesn't need a jump somewhere to do that.
        // Many undocumented opcodes have unique behavior that is also just handled in the switch statement, instead of jumping to a unique function.

        void Op_ORA(byte Input)
        {
            // Bitwise OR A with some value
            A |= Input;
            flag_Negative = A >= 0x80; // if bit 7 of the result is set
            flag_Zero = A == 0x00;     // if all bits are cleared
        }

        void Op_ASL(byte Input, ushort Address)
        {
            // Arithmetic shift left.
            flag_Carry = Input >= 0x80;    // If bit 7 was set before the shift
            Input <<= 1;
            Store(Input, Address);         // store the result at the target address
            flag_Negative = Input >= 0x80; // if bit 7 of the result is set
            flag_Zero = Input == 0x00;     // if all bits are cleared
        }

        void Op_ASL_A()
        {
            // Arithmetic shift left the Accumulator
            flag_Carry = A >= 0x80;    // If bit 7 was set before the shift
            A <<= 1;
            flag_Negative = A >= 0x80; // if bit 7 of the result is set
            flag_Zero = A == 0x00;     // if all bits are cleared
        }

        void Op_SLO(byte Input, ushort Address)
        {
            // Undocumented Opcode: equivalent to ASL + ORA
            Op_ASL(Input, Address);
            Op_ORA(dataBus);
        }

        void Op_AND(byte Input)
        {
            // Bitwise AND with A
            A &= Input;
            flag_Negative = A >= 0x80; // if bit 7 of the result is set
            flag_Zero = A == 0x00;     // if all bits are cleared
        }

        void Op_ROL(byte Input, ushort Address)
        {
            // Rotate Left
            bool Futureflag_Carry = Input >= 0x80;
            Input <<= 1;
            if (flag_Carry)
            {
                Input |= 1; // Put the old carry flag value into bit 0
            }
            Store(Input, Address);         // store the result at the target address
            flag_Carry = Futureflag_Carry; // if bit 7 of the initial value was set
            flag_Negative = Input >= 0x80; // if bit 7 of the result is set
            flag_Zero = Input == 0x00;     // if all bits are cleared
        }

        void Op_ROL_A()
        {
            // Rotate Left the Accumulator
            bool Futureflag_Carry = A >= 0x80;
            A <<= 1;
            if (flag_Carry)
            {
                A |= 1; // Put the old carry flag value into bit 0
            }
            flag_Carry = Futureflag_Carry; // if bit 7 of the initial value was set
            flag_Negative = A >= 0x80;     // if bit 7 of the result is set
            flag_Zero = A == 0x00;         // if all bits are cleared
        }

        void Op_RLA(byte Input, ushort Address)
        {
            // Undocumented Opcode: equivalent to ROL + AND
            Op_ROL(Input, Address);
            Op_AND(dataBus);
        }

        void Op_EOR(byte Input)
        {
            // Bitwise Exclusive OR A
            A ^= Input;
            flag_Negative = A >= 0x80; // if bit 7 of the result is set
            flag_Zero = A == 0x00;     // if all bits are cleared
        }

        void Op_LSR(byte Input, ushort Address)
        {
            // Logical Shift Right
            flag_Carry = (Input & 1) == 1; // If bit 0 of the initial value is set
            Input >>= 1;
            Store(Input, Address);         // store the result at the target address
            flag_Negative = Input >= 0x80; // if bit 7 of the result is set
            flag_Zero = Input == 0x00;     // if all bits are cleared
        }

        void Op_LSR_A()
        {
            // Logical Shift Right the Accumulator
            flag_Carry = (A & 1) == 1; // If bit 0 of the initial value is set
            A >>= 1;
            flag_Negative = A >= 0x80; // if bit 7 of the result is set
            flag_Zero = A == 0x00;     // if all bits are cleared
        }

        void Op_SRE(byte Input, ushort Address)
        {
            // Undocumented Opcode: equivalent to LSR + EOR
            Op_LSR(Input, Address);
            Op_EOR(dataBus);
        }

        void Op_ADC(byte Input)
        {
            // Add with Carry
            int Intput = Input + A + (flag_Carry ? 1 : 0);
            flag_Overflow = (~(A ^ Input) & (A ^ Intput) & 0x80) != 0;
            flag_Carry = Intput > 0xFF;
            A = (byte)Intput;
            flag_Negative = A >= 0x80; // if bit 7 of the result is set
            flag_Zero = A == 0x00;     // if all bits are cleared
        }

        void Op_ROR(byte Input, ushort Address)
        {
            // Rotate Right
            bool FutureFlag_Carry = (Input & 1) == 1; // if bit 0 was set before the shift
            Input >>= 1;
            if (flag_Carry)
            {
                Input |= 0x80;  // put the old carry flag into bit 7
            }
            Store(Input, Address);
            flag_Carry = FutureFlag_Carry; // if bit 0 was set before the shift
            flag_Negative = Input >= 0x80; // if bit 7 of the result is set
            flag_Zero = Input == 0x00;     // if all bits are cleared
        }

        void Op_ROR_A()
        {
            bool FutureFlag_Carry = (A & 1) == 1;
            A >>= 1;
            if (flag_Carry)
            {
                A |= 0x80;  // put the old carry flag into bit 7
            }
            flag_Carry = FutureFlag_Carry; // if bit 0 was set before the shift
            flag_Negative = A >= 0x80;     // if bit 7 of the result is set
            flag_Zero = A == 0x00;         // if all bits are cleared
        }

        void Op_RRA(byte Input, ushort Address)
        {
            // Undocumented Opcode: equivalent to ROR + ADC
            Op_ROR(Input, Address);
            Op_ADC(dataBus);
        }

        void Op_CMP(byte Input)
        {
            // Compare A
            flag_Zero = A == Input; // if A is equal to the value being compared
            flag_Carry = A >= Input;// if A is greater than the value being compared
            flag_Negative = ((byte)(A - Input) >= 0x80); // if A - the value being compared would leave bit 7 set
        }

        void Op_CPY(byte Input)
        {
            // Compare Y
            flag_Zero = Y == Input; // if Y is equal to the value being compared
            flag_Carry = Y >= Input;// if Y is greater than the value being compared
            flag_Negative = ((byte)(Y - Input) >= 0x80); // if Y - the value being compared would leave bit 7 set
        }

        void Op_CPX(byte Input)
        {
            // Compare X
            flag_Zero = X == Input; // if X is equal to the value being compared
            flag_Carry = X >= Input;// if X is greater than the value being compared
            flag_Negative = ((byte)(X - Input) >= 0x80); // if X - the value being compared would leave bit 7 set
        }

        void Op_SBC(byte Input)
        {
            // Subtract with Carry
            int Intput = A - Input;
            if (!flag_Carry)
            {
                Intput -= 1;
            }
            flag_Overflow = ((A ^ Input) & (A ^ Intput) & 0x80) != 0;
            flag_Carry = Intput >= 0;
            A = (byte)Intput;
            flag_Negative = A >= 0x80; // if bit 7 of the result is set
            flag_Zero = A == 0x00;     // if all bits are cleared
        }

        void Op_INC(ushort Address)
        {
            // Increment
            dl++;   // The value read is currently stored in the PreDecode register
            flag_Zero = dl == 0;        // if all bits are cleared
            flag_Negative = dl >= 0x80; // if bit 7 of the result is set
            Store(dl, Address);

        }

        void Op_DEC(ushort Address)
        {
            // Decrement
            dl--;  // The value read is currently stored in the PreDecode register
            flag_Zero = dl == 0;        // if all bits are cleared
            flag_Negative = dl >= 0x80; // if bit 7 of the result is set
            Store(dl, Address);

        }


        #endregion

        // this is the tracelogger.
        // I call this function during the first cycle of every instruction.

        public ushort DebugRange_Low = 0x0000;
        public ushort DebugRange_High = 0xFFFF;
        public bool OnlyDebugInRange = false;
        void Debug()
        {
            if (OnlyDebugInRange)
            {
                if (programCounter < DebugRange_Low || programCounter > DebugRange_High)
                {
                    return;
                }
            }

            string addr = programCounter.ToString("X4");
            string bytes = "";
            int b = 0;
            while (b < Documentation.OpDocs[opCode].length)
            {
                string t = Observe((ushort)(programCounter + b)).ToString("X");
                if (t.Length == 1) { t = "0" + t; }
                t += " ";
                bytes = bytes + t;
                b++;
            }

            if (bytes.Length < 7)
            {
                bytes += "\t";
            }

            string sA = A.ToString("X2");
            string sX = X.ToString("X2");
            string sY = Y.ToString("X2");
            string sS = stackPointer.ToString("X2");

            string Flags = "";
            Flags += flag_Negative ? "N" : "n";
            Flags += flag_Overflow ? "V" : "v";
            Flags += "--";
            Flags += flag_Decimal ? "D" : "d";
            Flags += flag_Interrupt ? "I" : "i";
            Flags += flag_Zero ? "Z" : "z";
            Flags += flag_Carry ? "C" : "c";


            if (DebugLog == null)
            {
                DebugLog = new StringBuilder();
            }

            string instruction = Documentation.OpDocs[opCode].mnemonic + " ";

            if (opCode == 0)
            {
                if (DoReset)
                {
                    instruction = "RESET";
                    bytes = "--\t";
                }
                else if (DoNMI)
                {
                    instruction = "NMI";
                    bytes = "--\t";

                }
                else if (DoIRQ)
                {
                    instruction = "IRQ";
                    bytes = "--\t";

                }
            }

            ushort Target = 0;

            switch (Documentation.OpDocs[opCode].mode)
            {
                case "i": //implied
                    break;
                case "d": //zp
                    instruction += "<$" + Observe((ushort)(programCounter + 1)).ToString("X2"); Target = Observe((ushort)(programCounter + 1)); break;
                case "a": //abs
                    instruction += "$" + Observe((ushort)(programCounter + 2)).ToString("X2") + Observe((ushort)(programCounter + 1)).ToString("X2"); Target = (ushort)((Observe((ushort)(programCounter + 2)) << 8) | Observe((ushort)(programCounter + 1))); break;
                case "r": //relative
                    instruction += "$" + ((ushort)(programCounter + (sbyte)Observe((ushort)(programCounter + 1))) + 2).ToString("X4"); Target = (ushort)((ushort)(programCounter + (sbyte)Observe((ushort)(programCounter + 1))) + 2); break;
                case "#v": //imm
                    instruction += "#" + Observe((ushort)(programCounter + 1)).ToString("X2"); Target = Observe((ushort)(programCounter + 1)); break;
                case "A": //A
                    instruction += "A"; break;
                case "(a)": //(ind)
                    instruction += "($" + Observe((ushort)(programCounter + 2)).ToString("X2") + Observe((ushort)(programCounter + 1)).ToString("X2") + ") -> $" + (Observe((ushort)(Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100)) + Observe((ushort)((Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100) + 1)) * 0x100).ToString("X4"); Target = (ushort)(Observe((ushort)(Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100)) + Observe((ushort)((Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100) + 1)) * 0x100); break;
                case "d,x": //zp, x
                    instruction += "<$" + Observe((ushort)(programCounter + 1)).ToString("X2") + ", X -> $" + (Observe((ushort)(programCounter + 1)) + X).ToString("X2"); Target = (ushort)(Observe((ushort)(programCounter + 1)) + X); break;
                case "d,y": //zp, y
                    instruction += "<$" + Observe((ushort)(programCounter + 1)).ToString("X2") + ", Y -> $" + (Observe((ushort)(programCounter + 1)) + Y).ToString("X2"); Target = (ushort)(Observe((ushort)(programCounter + 1)) + Y); break;
                case "a,x": //abs, x
                    instruction += "$" + Observe((ushort)(programCounter + 2)).ToString("X2") + Observe((ushort)(programCounter + 1)).ToString("X2") + ", X -> $" + ((ushort)(Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100 + X)).ToString("X4"); Target = (ushort)(Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100 + X); break;
                case "a,y": //abs, Y
                    instruction += "$" + Observe((ushort)(programCounter + 2)).ToString("X2") + Observe((ushort)(programCounter + 1)).ToString("X2") + ", Y -> $" + ((ushort)(Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100 + Y)).ToString("X4"); Target = (ushort)(Observe((ushort)(programCounter + 1)) + Observe((ushort)(programCounter + 2)) * 0x100 + Y); break;
                case "(d),y": //(zp), Y
                    instruction += "($00" + Observe((ushort)(programCounter + 1)).ToString("X2") + "), Y -> $" + ((ushort)(Observe(Observe((ushort)(programCounter + 1))) + Observe((ushort)((byte)(Observe((ushort)(programCounter + 1)) + 1) + (ushort)((Observe((ushort)(programCounter + 1)) & 0xFF00)))) * 0x100) + Y).ToString("X4"); Target = (ushort)((ushort)(Observe(Observe((ushort)(programCounter + 1))) + Observe((ushort)((byte)(Observe((ushort)(programCounter + 1)) + 1) + (ushort)((Observe((ushort)(programCounter + 1)) & 0xFF00)))) * 0x100) + Y); break;
                case "(d,x)": //(zp, X)
                    instruction += "($00" + Observe((ushort)(programCounter + 1)).ToString("X2") + ", X) -> $" + (Observe((byte)(Observe((ushort)(programCounter + 1)) + X)) + Observe((ushort)((byte)((byte)(Observe((ushort)(programCounter + 1)) + X) + 1) + (ushort)(((byte)(Observe((ushort)(programCounter + 1)) + X) & 0xFF00)))) * 0x100).ToString("X4"); Target = (ushort)(Observe((byte)(Observe((ushort)(programCounter + 1)) + X)) + Observe((ushort)((byte)((byte)(Observe((ushort)(programCounter + 1)) + X) + 1) + (ushort)(((byte)(Observe((ushort)(programCounter + 1)) + X) & 0xFF00)))) * 0x100); break;

            }

            if (Target == 0x2007)
            {
                instruction += " | PPU[$" + PPU_ReadWriteAddress.ToString("X4") + "]";
            }



            if (instruction.Length < 8)
            {
                instruction += "\t";
            }
            if (instruction.Length < 17)
            {
                instruction += "\t";
            }

            int PPUCycle = 0;
            String PPUPos = "(" + PPU_Scanline + ", " + PPU_Dot + ")";



            if (totalCycles < 27395)
            {
                PPUCycle = PPU_Scanline * 341 + PPU_Dot;
            }
            else
            {
                if (PPU_Scanline >= 241)
                {
                    PPUCycle = (PPU_Scanline - 241) * 341 + PPU_Dot;
                }
                else
                {
                    PPUCycle = (PPU_Scanline + 21) * 341 + PPU_Dot;
                }
            }

            if ((PPUPos.Length + PPUCycle.ToString().Length + 1) < 13)
            {
                PPUPos += "\t";
            }

            string LogLine = "$" + addr + "\t" + bytes + "\t" + instruction + "\tA:" + sA + "\tX:" + sX + "\tY:" + sY + "\tSP:" + sS + "\t" + Flags + "\tCycle: " + totalCycles;

            bool LogExtra = true;
            if (LogExtra)
            {
                string TempLine_APU_Full = LogLine + "\t" + "DMC :: S_Addr: $" + APU_DMC_SampleAddress.ToString("X4") + "\t S_Length:" + APU_DMC_SampleLength.ToString() + "\t AddrCounter: $" + APU_DMC_AddressCounter.ToString("X4") + "\t BytesLeft:" + APU_DMC_BytesRemaining.ToString() + "\t Shifter:" + APU_DMC_Shifter.ToString() + ":" + APU_DMC_ShifterBitsRemaining.ToString() + "\tDMC_Timer:" + (APU_PutCycle ? APU_ChannelTimer_DMC : (APU_ChannelTimer_DMC - 1)).ToString();


                string TempLine_APUFrameCounter_IRQs = LogLine + " \t$4015: " + Observe(0x4015).ToString("X2") + "\t APU_FrameCounter: " + APU_Framecounter.ToString() + " \tEvenCycle = : " + APU_PutCycle + " \tDoIRQ = " + DoIRQ;


                string TempLine_PPU = LogLine + "\t$2000:" + Observe(0x2000).ToString("X2") + "\t$2001:" + Observe(0x2001).ToString("X2") + "\t$2002:" + Observe(0x2002).ToString("X2") + "\tR/W Addr:" + PPU_ReadWriteAddress.ToString("X4") + "\tPPUAddrLatch:" + PPUAddrLatch + "\tPPU AddressBus: " + PPU_AddressBus.ToString("X4");
                string TempLine_PPU2 = LogLine + "\tVRAMAddress:" + PPU_ReadWriteAddress.ToString("X4") + "\tPPUReadBuffer:" + PPU_VRAMAddressBuffer.ToString("X2");
                string TempLine_PPU3 = LogLine + "\tPPU_Coords (" + PPU_Scanline + ", " + PPU_Dot + ")\tv: " + PPU_ReadWriteAddress.ToString("X4");

                string TempLine_MMC3IRQ = LogLine + "\tPPU_Coords (" + PPU_Scanline + ", " + PPU_Dot + ")\tIRQTimer:" + Cart.Mapper_4_IRQCounter + "\tIRQLatch: " + Cart.Mapper_4_IRQLatch + "\tIRQEnabled: " + Cart.Mapper_4_EnableIRQ + "\tDoIRQ: " + DoIRQ + "\tPPU_ADDR_Prev: " + PPU_ADDR_Prev.ToString("X4");


                DebugLog.AppendLine(TempLine_MMC3IRQ);
            }
            else
            {
                DebugLog.AppendLine(LogLine);
            }


        }

        public List<Byte> SaveState()
        {
            List<Byte> State = new List<byte>();

            State.Add((byte)programCounter);
            State.Add((byte)(programCounter >> 8));
            State.Add((byte)addressBus);
            State.Add((byte)(addressBus >> 8));
            State.Add((byte)temporaryAddress);
            State.Add((byte)(temporaryAddress >> 8));
            State.Add((byte)OAMAddressBus);
            State.Add((byte)(OAMAddressBus >> 8));
            State.Add((byte)PPU_ReadWriteAddress);
            State.Add((byte)(PPU_ReadWriteAddress >> 8));
            State.Add((byte)PPU_TempVRAMAddress);
            State.Add((byte)(PPU_TempVRAMAddress >> 8));

            State.Add((byte)totalCycles);
            State.Add((byte)(totalCycles >> 8));
            State.Add((byte)(totalCycles >> 16));
            State.Add((byte)(totalCycles >> 24));

            State.Add(PPUClock);
            State.Add(CPUClock);
            State.Add(APUClock);
            State.Add(MasterClock);

            State.Add(operationCycle);
            State.Add(opCode);
            State.Add((byte)(operationComplete ? 1 : 0));

            State.Add(dl);
            State.Add(dataBus);
            State.Add(A);
            State.Add(X);
            State.Add(Y);
            State.Add(stackPointer);
            status = flag_Carry ? (byte)0x01 : (byte)0;
            status += flag_Zero ? (byte)0x02 : (byte)0;
            status += flag_Interrupt ? (byte)0x04 : (byte)0;
            status += flag_Decimal ? (byte)0x08 : (byte)0;
            status += flag_Overflow ? (byte)0x40 : (byte)0;
            status += flag_Negative ? (byte)0x80 : (byte)0;
            State.Add(status);

            State.Add(specialBus);
            State.Add(H);
            State.Add((byte)(IgnoreH ? 1 : 0));

            State.Add((byte)(CPU_Read ? 1 : 0));
            State.Add((byte)(DoBRK ? 1 : 0));
            State.Add((byte)(DoNMI ? 1 : 0));
            State.Add((byte)(DoIRQ ? 1 : 0));
            State.Add((byte)(DoReset ? 1 : 0));
            State.Add((byte)(DoOAMDMA ? 1 : 0));
            State.Add((byte)(FirstCycleOfOAMDMA ? 1 : 0));
            State.Add((byte)(DoDMCDMA ? 1 : 0));
            State.Add(DMCDMADelay);
            State.Add(CannotRunDMCDMARightNow);
            State.Add(DMAPage);
            State.Add(DMAAddress);
            State.Add((byte)(APU_ControllerPortsStrobing ? 1 : 0));
            State.Add((byte)(APU_ControllerPortsStrobed ? 1 : 0));
            State.Add(ControllerPort1);
            State.Add(ControllerPort2);
            State.Add(ControllerShiftRegister1);
            State.Add(ControllerShiftRegister2);
            State.Add(Controller1ShiftCounter);
            State.Add(Controller2ShiftCounter);
            State.Add((byte)(DataPinsAreNotFloating ? 1 : 0));

            State.Add((byte)(APU_PutCycle ? 1 : 0));
            State.Add((byte)(APU_Status_DMCInterrupt ? 1 : 0));
            State.Add((byte)(APU_Status_FrameInterrupt ? 1 : 0));
            State.Add((byte)(APU_Status_DMC ? 1 : 0));
            State.Add((byte)(APU_Status_DelayedDMC ? 1 : 0));
            State.Add((byte)(APU_Status_Noise ? 1 : 0));
            State.Add((byte)(APU_Status_Triangle ? 1 : 0));
            State.Add((byte)(APU_Status_Pulse2 ? 1 : 0));
            State.Add((byte)(APU_Status_Pulse1 ? 1 : 0));
            State.Add((byte)(Clearing_APU_FrameInterrupt ? 1 : 0));
            State.Add(APU_DelayedDMC4015);
            State.Add((byte)(APU_ImplicitAbortDMC4015 ? 1 : 0));
            State.Add((byte)(APU_SetImplicitAbortDMC4015 ? 1 : 0));
            foreach (Byte b in APU_Register) { State.Add(b); }
            State.Add((byte)(APU_FrameCounterMode ? 1 : 0));
            State.Add((byte)(APU_FrameCounterInhibitIRQ ? 1 : 0));
            State.Add(APU_FrameCounterReset);
            State.Add((byte)APU_Framecounter);
            State.Add((byte)(APU_Framecounter >> 8));
            State.Add((byte)(APU_QuarterFrameClock ? 1 : 0));
            State.Add((byte)(APU_HalfFrameClock ? 1 : 0));
            State.Add((byte)(APU_Envelope_StartFlag ? 1 : 0));
            State.Add((byte)(APU_Envelope_DividerClock ? 1 : 0));
            State.Add(APU_Envelope_DecayLevel);
            State.Add(APU_LengthCounter_Pulse1);
            State.Add(APU_LengthCounter_Pulse2);
            State.Add(APU_LengthCounter_Triangle);
            State.Add(APU_LengthCounter_Noise);
            State.Add((byte)(APU_LengthCounter_HaltPulse1 ? 1 : 0));
            State.Add((byte)(APU_LengthCounter_HaltPulse2 ? 1 : 0));
            State.Add((byte)(APU_LengthCounter_HaltTriangle ? 1 : 0));
            State.Add((byte)(APU_LengthCounter_HaltNoise ? 1 : 0));
            State.Add((byte)(APU_LengthCounter_ReloadPulse1 ? 1 : 0));
            State.Add((byte)(APU_LengthCounter_ReloadPulse2 ? 1 : 0));
            State.Add((byte)(APU_LengthCounter_ReloadTriangle ? 1 : 0));
            State.Add((byte)(APU_LengthCounter_ReloadNoise ? 1 : 0));
            State.Add(APU_LengthCounter_ReloadValuePulse1);
            State.Add(APU_LengthCounter_ReloadValuePulse2);
            State.Add(APU_LengthCounter_ReloadValueTriangle);
            State.Add(APU_LengthCounter_ReloadValueNoise);
            State.Add((byte)APU_ChannelTimer_Pulse1);
            State.Add((byte)(APU_ChannelTimer_Pulse1 >> 8));
            State.Add((byte)APU_ChannelTimer_Pulse2);
            State.Add((byte)(APU_ChannelTimer_Pulse2 >> 8));
            State.Add((byte)APU_ChannelTimer_Triangle);
            State.Add((byte)(APU_ChannelTimer_Triangle >> 8));
            State.Add((byte)APU_ChannelTimer_Noise);
            State.Add((byte)(APU_ChannelTimer_Noise >> 8));
            State.Add((byte)APU_ChannelTimer_DMC);
            State.Add((byte)(APU_ChannelTimer_DMC >> 8));
            State.Add((byte)(APU_DMC_EnableIRQ ? 1 : 0));
            State.Add((byte)(APU_DMC_Loop ? 1 : 0));
            State.Add((byte)APU_DMC_Rate);
            State.Add((byte)(APU_DMC_Rate >> 8));
            State.Add(APU_DMC_Output);
            State.Add((byte)APU_DMC_SampleAddress);
            State.Add((byte)(APU_DMC_SampleAddress >> 8));
            State.Add((byte)APU_DMC_SampleLength);
            State.Add((byte)(APU_DMC_SampleLength >> 8));
            State.Add((byte)APU_DMC_BytesRemaining);
            State.Add((byte)(APU_DMC_BytesRemaining >> 8));
            State.Add(APU_DMC_Buffer);
            State.Add((byte)APU_DMC_AddressCounter);
            State.Add((byte)(APU_DMC_AddressCounter >> 8));
            State.Add(APU_DMC_Shifter);
            State.Add(APU_DMC_ShifterBitsRemaining);
            State.Add((byte)(APU_Silent ? 1 : 0));

            State.Add(SecondaryOAMSize);
            State.Add(OAM2Address);
            State.Add((byte)(SecondaryOAMFull ? 1 : 0));
            State.Add(SpriteEvaluationTick);
            State.Add((byte)(OAMAddressOverflowedDuringSpriteEvaluation ? 1 : 0));
            State.Add(OAM2Address);
            State.Add(PPUBus);
            for (int i = 0; i < 8; i++)
            {
                State.Add((byte)PPUBusDecay[i]);
                State.Add((byte)(PPUBusDecay[i] >> 8));
                State.Add((byte)(PPUBusDecay[i] >> 16));
                State.Add((byte)(PPUBusDecay[i] >> 24));
            }
            State.Add(PPUOAMAddress);
            State.Add((byte)(PPUStatus_VBlank ? 1 : 0));
            State.Add((byte)(PPUStatus_SpriteZeroHit ? 1 : 0));
            State.Add((byte)(PPUStatus_SpriteOverflow ? 1 : 0));
            State.Add((byte)(PPU_Spritex16 ? 1 : 0));
            State.Add((byte)PPU_Scanline);
            State.Add((byte)(PPU_Scanline >> 8));
            State.Add((byte)PPU_Dot);
            State.Add((byte)(PPU_Dot >> 8));
            State.Add((byte)(PPU_VRegisterChangedOutOfVBlank ? 1 : 0));
            State.Add((byte)(PPU_OAMCorruptionRenderingDisabledOutOfVBlank ? 1 : 0));
            State.Add((byte)(PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant ? 1 : 0));
            State.Add((byte)(PPU_PendingOAMCorruption ? 1 : 0));
            State.Add(PPU_OAMCorruptionIndex);
            State.Add((byte)(PPU_OAMCorruptionRenderingEnabledOutOfVBlank ? 1 : 0));
            State.Add((byte)(PPU_OAMEvaluationCorruptionOddCycle ? 1 : 0));
            State.Add((byte)(PPU_OAMEvaluationObjectInRange ? 1 : 0));
            State.Add((byte)(PPU_OAMEvaluationObjectInXRange ? 1 : 0));
            State.Add((byte)(PPU_PaletteCorruptionRenderingDisabledOutOfVBlank ? 1 : 0));
            State.Add(PPU_AttributeLatchRegister);
            State.Add((byte)PPU_BackgroundAttributeShiftRegisterL);
            State.Add((byte)(PPU_BackgroundAttributeShiftRegisterL >> 8));
            State.Add((byte)PPU_BackgroundAttributeShiftRegisterH);
            State.Add((byte)(PPU_BackgroundAttributeShiftRegisterH >> 8));
            State.Add((byte)PPU_BackgroundPatternShiftRegisterL);
            State.Add((byte)(PPU_BackgroundPatternShiftRegisterL >> 8));
            State.Add((byte)PPU_BackgroundPatternShiftRegisterH);
            State.Add((byte)(PPU_BackgroundPatternShiftRegisterH >> 8));
            State.Add(PPU_FineXScroll);
            for (int i = 0; i < 8; i++) { State.Add(PPU_SpriteShiftRegisterL[i]); }
            for (int i = 0; i < 8; i++) { State.Add(PPU_SpriteShiftRegisterH[i]); }
            for (int i = 0; i < 8; i++) { State.Add(PPU_SpriteAttribute[i]); }
            for (int i = 0; i < 8; i++) { State.Add(PPU_SpritePattern[i]); }
            for (int i = 0; i < 8; i++) { State.Add(PPU_SpriteXposition[i]); }
            for (int i = 0; i < 8; i++) { State.Add(PPU_SpriteYposition[i]); }
            for (int i = 0; i < 8; i++) { State.Add(PPU_SpriteShifterCounter[i]); }
            State.Add((byte)(PPU_NextScanlineContainsSpriteZero ? 1 : 0));
            State.Add((byte)(PPU_CurrentScanlineContainsSpriteZero ? 1 : 0));
            State.Add(PPU_SpritePatternL);
            State.Add(PPU_SpritePatternH);
            State.Add((byte)(PPU_Mask_Greyscale ? 1 : 0));
            State.Add((byte)(PPU_Mask_8PxShowBackground ? 1 : 0));
            State.Add((byte)(PPU_Mask_8PxShowSprites ? 1 : 0));
            State.Add((byte)(PPU_Mask_ShowBackground ? 1 : 0));
            State.Add((byte)(PPU_Mask_ShowSprites ? 1 : 0));
            State.Add((byte)(PPU_Mask_EmphasizeRed ? 1 : 0));
            State.Add((byte)(PPU_Mask_EmphasizeGreen ? 1 : 0));
            State.Add((byte)(PPU_Mask_EmphasizeBlue ? 1 : 0));
            State.Add((byte)(PPU_Mask_ShowBackground_Delayed ? 1 : 0));
            State.Add((byte)(PPU_Mask_ShowSprites_Delayed ? 1 : 0));
            State.Add((byte)(PPU_Mask_ShowBackground_Instant ? 1 : 0));
            State.Add((byte)(PPU_Mask_ShowSprites_Instant ? 1 : 0));
            State.Add(PPU_LowBitPlane);
            State.Add(PPU_HighBitPlane);
            State.Add(PPU_Attribute);
            State.Add(PPU_NextCharacter);
            State.Add((byte)(PPU_CanDetectSpriteZeroHit ? 1 : 0));
            State.Add((byte)PPU_ADDR_Prev);
            State.Add((byte)(PPU_ADDR_Prev >> 8));
            State.Add((byte)(PPU_OddFrame ? 1 : 0));
            State.Add(PaletteRAMAddress);
            State.Add((byte)(ThisDotReadFromPaletteRAM ? 1 : 0));
            State.Add((byte)(NMI_PinsSignal ? 1 : 0));
            State.Add((byte)(NMI_PreviousPinsSignal ? 1 : 0));
            State.Add((byte)(IRQ_LevelDetector ? 1 : 0));
            State.Add((byte)(NMILine ? 1 : 0));
            State.Add((byte)(IRQLine ? 1 : 0));
            State.Add((byte)(CopyV ? 1 : 0));
            State.Add((byte)(SkippedPreRenderDot341 ? 1 : 0));
            State.Add((byte)(OamCorruptedOnOddCycle ? 1 : 0));
            State.Add(PPU_SpriteEvaluationTemp);
            State.Add(PPU_RenderTemp);
            State.Add((byte)(PPU_Commit_NametableFetch ? 1 : 0));
            State.Add((byte)(PPU_Commit_AttributeFetch ? 1 : 0));
            State.Add((byte)(PPU_Commit_PatternLowFetch ? 1 : 0));
            State.Add((byte)(PPU_Commit_PatternHighFetch ? 1 : 0));
            State.Add((byte)(PPU_Commit_LoadShiftRegisters ? 1 : 0));

            State.Add((byte)PPU_VRAM_MysteryAddress);
            State.Add((byte)(PPU_VRAM_MysteryAddress >> 8));
            State.Add((byte)PPU_AddressBus);
            State.Add((byte)(PPU_AddressBus >> 8));
            State.Add(PPU_Update2006Delay);
            State.Add(PPU_Update2005Delay);
            State.Add(PPU_Update2005Value);
            State.Add(PPU_Update2001Delay);
            State.Add(PPU_Update2001EmphasisBitsDelay);
            State.Add(PPU_Update2001OAMCorruptionDelay);
            State.Add(PPU_Update2001Value);
            State.Add(PPU_Update2000Delay);
            State.Add(PPU_Update2000Value);
            State.Add((byte)PPU_Update2006Value);
            State.Add((byte)(PPU_Update2006Value >> 8));
            State.Add((byte)PPU_Update2006Value_Temp);
            State.Add((byte)(PPU_Update2006Value_Temp >> 8));
            State.Add((byte)(PPU_WasRenderingBefore2001Write ? 1 : 0));
            State.Add(PPU_VRAMAddressBuffer);
            State.Add((byte)(PPUAddrLatch ? 1 : 0));
            State.Add((byte)(PPUControlIncrementMode32 ? 1 : 0));
            State.Add((byte)(PPUControl_NMIEnabled ? 1 : 0));
            State.Add((byte)(PPU_PatternSelect_Sprites ? 1 : 0));
            State.Add((byte)(PPU_PatternSelect_Background ? 1 : 0));
            State.Add((byte)(PPU_PendingVBlank ? 1 : 0));

            State.Add((byte)(OAMDMA_Aligned ? 1 : 0));
            State.Add((byte)(OAMDMA_Halt ? 1 : 0));
            State.Add((byte)(DMCDMA_Halt ? 1 : 0));
            State.Add(OAM_InternalBus);

            foreach (Byte b in RAM) { State.Add(b); }
            foreach (Byte b in VRAM) { State.Add(b); }
            foreach (Byte b in OAM) { State.Add(b); }
            foreach (Byte b in OAM2) { State.Add(b); }
            foreach (Byte b in PaletteRAM) { State.Add(b); }
            foreach (Byte b in Cart.PRGRAM) { State.Add(b); }
            foreach (Byte b in Cart.CHRRAM) { State.Add(b); }

            State.Add(Cart.Mapper_1_ShiftRegister);
            State.Add(Cart.Mapper_1_Control);
            State.Add(Cart.Mapper_1_CHR0);
            State.Add(Cart.Mapper_1_CHR1);
            State.Add(Cart.Mapper_1_ShiftRegister);
            State.Add((byte)(Cart.Mapper_1_PB ? 1 : 0));

            State.Add(Cart.Mapper_2_BankSelect);

            State.Add(Cart.Mapper_4_8000);
            State.Add(Cart.Mapper_4_BankA);
            State.Add(Cart.Mapper_4_Bank8C);
            State.Add(Cart.Mapper_4_CHR_2K0);
            State.Add(Cart.Mapper_4_CHR_2K8);
            State.Add(Cart.Mapper_4_CHR_1K0);
            State.Add(Cart.Mapper_4_CHR_1K4);
            State.Add(Cart.Mapper_4_CHR_1K8);
            State.Add(Cart.Mapper_4_CHR_1KC);
            State.Add(Cart.Mapper_4_IRQLatch);
            State.Add(Cart.Mapper_4_IRQCounter);
            State.Add((byte)(Cart.Mapper_4_EnableIRQ ? 1 : 0));
            State.Add((byte)(Cart.Mapper_4_ReloadIRQCounter ? 1 : 0));
            State.Add((byte)(Cart.Mapper_4_NametableMirroring ? 1 : 0));
            State.Add(Cart.Mapper_4_PRGRAMProtect);

            State.Add(Cart.Mapper_7_BankSelect);

            State.Add(Cart.Mapper_9_BankSelect);
            State.Add(Cart.Mapper_9_CHR0_FD);
            State.Add(Cart.Mapper_9_CHR0_FE);
            State.Add(Cart.Mapper_9_CHR1_FD);
            State.Add(Cart.Mapper_9_CHR1_FE);
            State.Add((byte)(Cart.Mapper_9_NametableMirroring ? 1 : 0));
            State.Add((byte)(Cart.Mapper_9_Latch0_FE ? 1 : 0));
            State.Add((byte)(Cart.Mapper_9_Latch1_FE ? 1 : 0));

            State.Add(Cart.Mapper_69_CMD);
            State.Add(Cart.Mapper_69_CHR_1K0);
            State.Add(Cart.Mapper_69_CHR_1K1);
            State.Add(Cart.Mapper_69_CHR_1K2);
            State.Add(Cart.Mapper_69_CHR_1K3);
            State.Add(Cart.Mapper_69_CHR_1K4);
            State.Add(Cart.Mapper_69_CHR_1K5);
            State.Add(Cart.Mapper_69_CHR_1K6);
            State.Add(Cart.Mapper_69_CHR_1K7);
            State.Add(Cart.Mapper_69_Bank_6);
            State.Add((byte)(Cart.Mapper_69_Bank_6_isRAM ? 1 : 0));
            State.Add((byte)(Cart.Mapper_69_Bank_6_isRAMEnabled ? 1 : 0));
            State.Add(Cart.Mapper_69_Bank_8);
            State.Add(Cart.Mapper_69_Bank_A);
            State.Add(Cart.Mapper_69_Bank_C);
            State.Add(Cart.Mapper_69_NametableMirroring);
            State.Add((byte)(Cart.Mapper_69_EnableIRQ ? 1 : 0));
            State.Add((byte)(Cart.Mapper_69_EnableIRQCounterDecrement ? 1 : 0));
            State.Add((byte)Cart.Mapper_69_IRQCounter);
            State.Add((byte)(Cart.Mapper_69_IRQCounter >> 8));

            // putting stuff down here that I plan to refactor in future updates to the emulator.

            State.Add(PPU_Data_StateMachine);
            State.Add((byte)(PPU_Data_StateMachine_Read ? 1 : 0));
            State.Add((byte)(PPU_Data_StateMachine_Read_Delayed ? 1 : 0));
            State.Add((byte)(PPU_Data_StateMachine_PerformMysteryWrite ? 1 : 0));
            State.Add(PPU_Data_StateMachine_InputValue);
            State.Add((byte)(PPU_Data_StateMachine_UpdateVRAMAddressEarly ? 1 : 0));
            State.Add((byte)(PPU_Data_StateMachine_UpdateVRAMBufferLate ? 1 : 0));
            State.Add((byte)(PPU_Data_StateMachine_NormalWriteBehavior ? 1 : 0));
            State.Add((byte)(PPU_Data_StateMachine_InterruptedReadToWrite ? 1 : 0));

            State.Add(MMC3_M2Filter);
            State.Add((byte)(ResetM2Filter ? 1 : 0));

            return State;
        }

        public void LoadState(List<byte> State)
        {
            int p = 0;
            programCounter = State[p++];
            programCounter |= (ushort)(State[p++] << 8);
            addressBus = State[p++];
            addressBus |= (ushort)(State[p++] << 8);
            temporaryAddress = State[p++];
            temporaryAddress |= (ushort)(State[p++] << 8);
            OAMAddressBus = State[p++];
            OAMAddressBus |= (ushort)(State[p++] << 8);
            PPU_ReadWriteAddress = State[p++];
            PPU_ReadWriteAddress |= (ushort)(State[p++] << 8);
            PPU_TempVRAMAddress = State[p++];
            PPU_TempVRAMAddress |= (ushort)(State[p++] << 8);

            totalCycles = State[p++];
            totalCycles |= (State[p++] << 8);
            totalCycles |= (State[p++] << 16);
            totalCycles |= (State[p++] << 24);

            PPUClock = State[p++];
            CPUClock = State[p++];
            APUClock = State[p++];
            MasterClock = State[p++];

            operationCycle = State[p++];
            opCode = State[p++];
            operationComplete = (State[p++] & 1) == 1;

            dl = State[p++];
            dataBus = State[p++];
            A = State[p++];
            X = State[p++];
            Y = State[p++];
            stackPointer = State[p++];

            status = State[p++];
            flag_Carry = (status & 1) == 1;
            flag_Zero = ((status & 0x02) >> 1) == 1;
            flag_Interrupt = ((status & 0x04) >> 2) == 1;
            flag_Decimal = ((status & 0x08) >> 3) == 1;
            flag_Overflow = ((status & 0x40) >> 6) == 1;
            flag_Negative = ((status & 0x80) >> 7) == 1;

            specialBus = State[p++];
            H = State[p++];
            IgnoreH = (State[p++] & 1) == 1;

            CPU_Read = (State[p++] & 1) == 1;
            DoBRK = (State[p++] & 1) == 1;
            DoNMI = (State[p++] & 1) == 1;
            DoIRQ = (State[p++] & 1) == 1;
            DoReset = (State[p++] & 1) == 1;
            DoOAMDMA = (State[p++] & 1) == 1;
            FirstCycleOfOAMDMA = (State[p++] & 1) == 1;
            DoDMCDMA = (State[p++] & 1) == 1;
            DMCDMADelay = State[p++];
            CannotRunDMCDMARightNow = State[p++];
            DMAPage = State[p++];
            DMAAddress = State[p++];
            APU_ControllerPortsStrobing = (State[p++] & 1) == 1;
            APU_ControllerPortsStrobed = (State[p++] & 1) == 1;
            ControllerPort1 = State[p++];
            ControllerPort2 = State[p++];
            ControllerShiftRegister1 = State[p++];
            ControllerShiftRegister2 = State[p++];
            Controller1ShiftCounter = State[p++];
            Controller2ShiftCounter = State[p++];
            DataPinsAreNotFloating = (State[p++] & 1) == 1;

            APU_PutCycle = (State[p++] & 1) == 1;
            APU_Status_DMCInterrupt = (State[p++] & 1) == 1;
            APU_Status_FrameInterrupt = (State[p++] & 1) == 1;
            APU_Status_DMC = (State[p++] & 1) == 1;
            APU_Status_DelayedDMC = (State[p++] & 1) == 1;
            APU_Status_Noise = (State[p++] & 1) == 1;
            APU_Status_Triangle = (State[p++] & 1) == 1;
            APU_Status_Pulse2 = (State[p++] & 1) == 1;
            APU_Status_Pulse1 = (State[p++] & 1) == 1;
            Clearing_APU_FrameInterrupt = (State[p++] & 1) == 1;
            APU_DelayedDMC4015 = State[p++];
            APU_ImplicitAbortDMC4015 = (State[p++] & 1) == 1;
            APU_SetImplicitAbortDMC4015 = (State[p++] & 1) == 1;
            for (int i = 0; i < APU_Register.Length; i++) { APU_Register[i] = State[p++]; }
            APU_FrameCounterMode = (State[p++] & 1) == 1;
            APU_FrameCounterInhibitIRQ = (State[p++] & 1) == 1;
            APU_FrameCounterReset = State[p++];
            APU_Framecounter = State[p++];
            APU_Framecounter |= (ushort)(State[p++] << 8);
            APU_QuarterFrameClock = (State[p++] & 1) == 1;
            APU_HalfFrameClock = (State[p++] & 1) == 1;
            APU_Envelope_StartFlag = (State[p++] & 1) == 1;
            APU_Envelope_DividerClock = (State[p++] & 1) == 1;
            APU_Envelope_DecayLevel = State[p++];
            APU_LengthCounter_Pulse1 = State[p++];
            APU_LengthCounter_Pulse2 = State[p++];
            APU_LengthCounter_Triangle = State[p++];
            APU_LengthCounter_Noise = State[p++];
            APU_LengthCounter_HaltPulse1 = (State[p++] & 1) == 1;
            APU_LengthCounter_HaltPulse2 = (State[p++] & 1) == 1;
            APU_LengthCounter_HaltTriangle = (State[p++] & 1) == 1;
            APU_LengthCounter_HaltNoise = (State[p++] & 1) == 1;
            APU_LengthCounter_ReloadPulse1 = (State[p++] & 1) == 1;
            APU_LengthCounter_ReloadPulse2 = (State[p++] & 1) == 1;
            APU_LengthCounter_ReloadTriangle = (State[p++] & 1) == 1;
            APU_LengthCounter_ReloadNoise = (State[p++] & 1) == 1;
            APU_LengthCounter_ReloadValuePulse1 = State[p++];
            APU_LengthCounter_ReloadValuePulse2 = State[p++];
            APU_LengthCounter_ReloadValueTriangle = State[p++];
            APU_LengthCounter_ReloadValueNoise = State[p++];
            APU_ChannelTimer_Pulse1 = State[p++];
            APU_ChannelTimer_Pulse1 |= (ushort)(State[p++] << 8);
            APU_ChannelTimer_Pulse2 = State[p++];
            APU_ChannelTimer_Pulse2 |= (ushort)(State[p++] << 8);
            APU_ChannelTimer_Triangle = State[p++];
            APU_ChannelTimer_Triangle |= (ushort)(State[p++] << 8);
            APU_ChannelTimer_Noise = State[p++];
            APU_ChannelTimer_Noise |= (ushort)(State[p++] << 8);
            APU_ChannelTimer_DMC = State[p++];
            APU_ChannelTimer_DMC |= (ushort)(State[p++] << 8);
            APU_DMC_EnableIRQ = (State[p++] & 1) == 1;
            APU_DMC_Loop = (State[p++] & 1) == 1;
            APU_DMC_Rate = State[p++];
            APU_DMC_Rate |= (ushort)(State[p++] << 8);
            APU_DMC_Output = State[p++];
            APU_DMC_SampleAddress = State[p++];
            APU_DMC_SampleAddress |= (ushort)(State[p++] << 8);
            APU_DMC_SampleLength = State[p++];
            APU_DMC_SampleLength |= (ushort)(State[p++] << 8);
            APU_DMC_BytesRemaining = State[p++];
            APU_DMC_BytesRemaining |= (ushort)(State[p++] << 8);
            APU_DMC_Buffer = State[p++];
            APU_DMC_AddressCounter = State[p++];
            APU_DMC_AddressCounter |= (ushort)(State[p++] << 8);
            APU_DMC_Shifter = State[p++];
            APU_DMC_ShifterBitsRemaining = State[p++];
            APU_Silent = (State[p++] & 1) == 1;

            SecondaryOAMSize = State[p++];
            OAM2Address = State[p++];
            SecondaryOAMFull = (State[p++] & 1) == 1;
            SpriteEvaluationTick = State[p++];
            OAMAddressOverflowedDuringSpriteEvaluation = (State[p++] & 1) == 1;
            OAM2Address = State[p++];
            PPUBus = State[p++];
            for (int i = 0; i < 8; i++)
            {
                PPUBusDecay[i] = State[p++];
                PPUBusDecay[i] |= (State[p++] << 8);
                PPUBusDecay[i] |= (State[p++] << 16);
                PPUBusDecay[i] |= (State[p++] << 24);
            }
            PPUOAMAddress = State[p++];
            PPUStatus_VBlank = (State[p++] & 1) == 1;
            PPUStatus_SpriteZeroHit = (State[p++] & 1) == 1;
            PPUStatus_SpriteOverflow = (State[p++] & 1) == 1;
            PPU_Spritex16 = (State[p++] & 1) == 1;
            PPU_Scanline = State[p++];
            PPU_Scanline |= (ushort)(State[p++] << 8);
            PPU_Dot = State[p++];
            PPU_Dot |= (ushort)(State[p++] << 8);
            PPU_VRegisterChangedOutOfVBlank = (State[p++] & 1) == 1;
            PPU_OAMCorruptionRenderingDisabledOutOfVBlank = (State[p++] & 1) == 1;
            PPU_OAMCorruptionRenderingDisabledOutOfVBlank_Instant = (State[p++] & 1) == 1;
            PPU_PendingOAMCorruption = (State[p++] & 1) == 1;
            PPU_OAMCorruptionIndex = State[p++];
            PPU_OAMCorruptionRenderingEnabledOutOfVBlank = (State[p++] & 1) == 1;
            PPU_OAMEvaluationCorruptionOddCycle = (State[p++] & 1) == 1;
            PPU_OAMEvaluationObjectInRange = (State[p++] & 1) == 1;
            PPU_OAMEvaluationObjectInXRange = (State[p++] & 1) == 1;
            PPU_PaletteCorruptionRenderingDisabledOutOfVBlank = (State[p++] & 1) == 1;
            PPU_AttributeLatchRegister = State[p++];
            PPU_BackgroundAttributeShiftRegisterL = State[p++];
            PPU_BackgroundAttributeShiftRegisterL |= (ushort)(State[p++] << 8);
            PPU_BackgroundAttributeShiftRegisterH = State[p++];
            PPU_BackgroundAttributeShiftRegisterH |= (ushort)(State[p++] << 8);
            PPU_BackgroundPatternShiftRegisterL = State[p++];
            PPU_BackgroundPatternShiftRegisterL |= (ushort)(State[p++] << 8);
            PPU_BackgroundPatternShiftRegisterH = State[p++];
            PPU_BackgroundPatternShiftRegisterH |= (ushort)(State[p++] << 8);
            PPU_FineXScroll = State[p++];
            for (int i = 0; i < 8; i++) { PPU_SpriteShiftRegisterL[i] = State[p++]; }
            for (int i = 0; i < 8; i++) { PPU_SpriteShiftRegisterH[i] = State[p++]; }
            for (int i = 0; i < 8; i++) { PPU_SpriteAttribute[i] = State[p++]; }
            for (int i = 0; i < 8; i++) { PPU_SpritePattern[i] = State[p++]; }
            for (int i = 0; i < 8; i++) { PPU_SpriteXposition[i] = State[p++]; }
            for (int i = 0; i < 8; i++) { PPU_SpriteYposition[i] = State[p++]; }
            for (int i = 0; i < 8; i++) { PPU_SpriteShifterCounter[i] = State[p++]; }
            PPU_NextScanlineContainsSpriteZero = (State[p++] & 1) == 1;
            PPU_CurrentScanlineContainsSpriteZero = (State[p++] & 1) == 1;
            PPU_SpritePatternL = State[p++];
            PPU_SpritePatternH = State[p++];
            PPU_Mask_Greyscale = (State[p++] & 1) == 1;
            PPU_Mask_8PxShowBackground = (State[p++] & 1) == 1;
            PPU_Mask_8PxShowSprites = (State[p++] & 1) == 1;
            PPU_Mask_ShowBackground = (State[p++] & 1) == 1;
            PPU_Mask_ShowSprites = (State[p++] & 1) == 1;
            PPU_Mask_EmphasizeRed = (State[p++] & 1) == 1;
            PPU_Mask_EmphasizeGreen = (State[p++] & 1) == 1;
            PPU_Mask_EmphasizeBlue = (State[p++] & 1) == 1;
            PPU_Mask_ShowBackground_Delayed = (State[p++] & 1) == 1;
            PPU_Mask_ShowSprites_Delayed = (State[p++] & 1) == 1;
            PPU_Mask_ShowBackground_Instant = (State[p++] & 1) == 1;
            PPU_Mask_ShowSprites_Instant = (State[p++] & 1) == 1;
            PPU_LowBitPlane = State[p++];
            PPU_HighBitPlane = State[p++];
            PPU_Attribute = State[p++];
            PPU_NextCharacter = State[p++];
            PPU_CanDetectSpriteZeroHit = (State[p++] & 1) == 1;
            PPU_ADDR_Prev = State[p++];
            PPU_ADDR_Prev |= (ushort)(State[p++] << 8);
            PPU_OddFrame = (State[p++] & 1) == 1;
            PaletteRAMAddress = State[p++];
            ThisDotReadFromPaletteRAM = (State[p++] & 1) == 1;
            NMI_PinsSignal = (State[p++] & 1) == 1;
            NMI_PreviousPinsSignal = (State[p++] & 1) == 1;
            IRQ_LevelDetector = (State[p++] & 1) == 1;
            NMILine = (State[p++] & 1) == 1;
            IRQLine = (State[p++] & 1) == 1;
            CopyV = (State[p++] & 1) == 1;
            SkippedPreRenderDot341 = (State[p++] & 1) == 1;
            OamCorruptedOnOddCycle = (State[p++] & 1) == 1;
            PPU_SpriteEvaluationTemp = State[p++];
            PPU_RenderTemp = State[p++];
            PPU_Commit_NametableFetch = (State[p++] & 1) == 1;
            PPU_Commit_AttributeFetch = (State[p++] & 1) == 1;
            PPU_Commit_PatternLowFetch = (State[p++] & 1) == 1;
            PPU_Commit_PatternHighFetch = (State[p++] & 1) == 1;
            PPU_Commit_LoadShiftRegisters = (State[p++] & 1) == 1;

            PPU_VRAM_MysteryAddress = State[p++];
            PPU_VRAM_MysteryAddress |= (ushort)(State[p++] << 8);
            PPU_AddressBus = State[p++];
            PPU_AddressBus |= (ushort)(State[p++] << 8);
            PPU_Update2006Delay = State[p++];
            PPU_Update2005Delay = State[p++];
            PPU_Update2005Value = State[p++];
            PPU_Update2001Delay = State[p++];
            PPU_Update2001EmphasisBitsDelay = State[p++];
            PPU_Update2001OAMCorruptionDelay = State[p++];
            PPU_Update2001Value = State[p++];
            PPU_Update2000Delay = State[p++];
            PPU_Update2000Value = State[p++];
            PPU_Update2006Value = State[p++];
            PPU_Update2006Value |= (ushort)(State[p++] << 8);
            PPU_Update2006Value_Temp = State[p++];
            PPU_Update2006Value_Temp |= (ushort)(State[p++] << 8);
            PPU_WasRenderingBefore2001Write = (State[p++] & 1) == 1;
            PPU_VRAMAddressBuffer = State[p++];
            PPUAddrLatch = (State[p++] & 1) == 1;
            PPUControlIncrementMode32 = (State[p++] & 1) == 1;
            PPUControl_NMIEnabled = (State[p++] & 1) == 1;
            PPU_PatternSelect_Sprites = (State[p++] & 1) == 1;
            PPU_PatternSelect_Background = (State[p++] & 1) == 1;
            PPU_PendingVBlank = (State[p++] & 1) == 1;

            OAMDMA_Aligned = (State[p++] & 1) == 1;
            OAMDMA_Halt = (State[p++] & 1) == 1;
            DMCDMA_Halt = (State[p++] & 1) == 1;
            OAM_InternalBus = State[p++];

            for (int i = 0; i < RAM.Length; i++) { RAM[i] = State[p++]; }
            for (int i = 0; i < VRAM.Length; i++) { VRAM[i] = State[p++]; }
            for (int i = 0; i < OAM.Length; i++) { OAM[i] = State[p++]; }
            for (int i = 0; i < OAM2.Length; i++) { OAM2[i] = State[p++]; }
            for (int i = 0; i < PaletteRAM.Length; i++) { PaletteRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.PRGRAM.Length; i++) { Cart.PRGRAM[i] = State[p++]; }
            for (int i = 0; i < Cart.CHRRAM.Length; i++) { Cart.CHRRAM[i] = State[p++]; }

            Cart.Mapper_1_ShiftRegister = State[p++];
            Cart.Mapper_1_Control = State[p++];
            Cart.Mapper_1_CHR0 = State[p++];
            Cart.Mapper_1_CHR1 = State[p++];
            Cart.Mapper_1_ShiftRegister = State[p++];
            Cart.Mapper_1_PB = (State[p++] & 1) == 1;

            Cart.Mapper_2_BankSelect = State[p++];

            Cart.Mapper_4_8000 = State[p++];
            Cart.Mapper_4_BankA = State[p++];
            Cart.Mapper_4_Bank8C = State[p++];
            Cart.Mapper_4_CHR_2K0 = State[p++];
            Cart.Mapper_4_CHR_2K8 = State[p++];
            Cart.Mapper_4_CHR_1K0 = State[p++];
            Cart.Mapper_4_CHR_1K4 = State[p++];
            Cart.Mapper_4_CHR_1K8 = State[p++];
            Cart.Mapper_4_CHR_1KC = State[p++];
            Cart.Mapper_4_IRQLatch = State[p++];
            Cart.Mapper_4_IRQCounter = State[p++];
            Cart.Mapper_4_EnableIRQ = (State[p++] & 1) == 1;
            Cart.Mapper_4_ReloadIRQCounter = (State[p++] & 1) == 1;
            Cart.Mapper_4_NametableMirroring = (State[p++] & 1) == 1;
            Cart.Mapper_4_PRGRAMProtect = State[p++];

            Cart.Mapper_7_BankSelect = State[p++];

            Cart.Mapper_9_BankSelect = State[p++];
            Cart.Mapper_9_CHR0_FD = State[p++];
            Cart.Mapper_9_CHR0_FE = State[p++];
            Cart.Mapper_9_CHR1_FD = State[p++];
            Cart.Mapper_9_CHR1_FE = State[p++];
            Cart.Mapper_9_NametableMirroring = (State[p++] & 1) == 1;
            Cart.Mapper_9_Latch0_FE = (State[p++] & 1) == 1;
            Cart.Mapper_9_Latch1_FE = (State[p++] & 1) == 1;

            Cart.Mapper_69_CMD = State[p++];
            Cart.Mapper_69_CHR_1K0 = State[p++];
            Cart.Mapper_69_CHR_1K1 = State[p++];
            Cart.Mapper_69_CHR_1K2 = State[p++];
            Cart.Mapper_69_CHR_1K3 = State[p++];
            Cart.Mapper_69_CHR_1K4 = State[p++];
            Cart.Mapper_69_CHR_1K5 = State[p++];
            Cart.Mapper_69_CHR_1K6 = State[p++];
            Cart.Mapper_69_CHR_1K7 = State[p++];
            Cart.Mapper_69_Bank_6 = State[p++];
            Cart.Mapper_69_Bank_6_isRAM = (State[p++] & 1) == 1;
            Cart.Mapper_69_Bank_6_isRAMEnabled = (State[p++] & 1) == 1;
            Cart.Mapper_69_Bank_8 = State[p++];
            Cart.Mapper_69_Bank_A = State[p++];
            Cart.Mapper_69_Bank_C = State[p++];
            Cart.Mapper_69_NametableMirroring = State[p++];
            Cart.Mapper_69_EnableIRQ = (State[p++] & 1) == 1;
            Cart.Mapper_69_EnableIRQCounterDecrement = (State[p++] & 1) == 1;
            Cart.Mapper_69_IRQCounter = State[p++];
            Cart.Mapper_69_IRQCounter |= (ushort)(State[p++] << 8);


            // putting stuff down here that I plan to refactor in future updates to the emulator.

            PPU_Data_StateMachine = State[p++];
            PPU_Data_StateMachine_Read = (State[p++] & 1) == 1;
            PPU_Data_StateMachine_Read_Delayed = (State[p++] & 1) == 1;
            PPU_Data_StateMachine_PerformMysteryWrite = (State[p++] & 1) == 1;
            PPU_Data_StateMachine_InputValue = State[p++];
            PPU_Data_StateMachine_UpdateVRAMAddressEarly = (State[p++] & 1) == 1;
            PPU_Data_StateMachine_UpdateVRAMBufferLate = (State[p++] & 1) == 1;
            PPU_Data_StateMachine_NormalWriteBehavior = (State[p++] & 1) == 1;
            PPU_Data_StateMachine_InterruptedReadToWrite = (State[p++] & 1) == 1;

            MMC3_M2Filter = State[p++];
            ResetM2Filter = (State[p++] & 1) == 1;
        }

        public void Dispose()
        {
            Cart = null;
            Screen.Dispose();
            BorderedScreen.Dispose();
            NTSCScreen.Dispose();
            BorderedNTSCScreen.Dispose();
        }

    }

    public class DirectBitmap : IDisposable
    {
        // This class was copied from Stack Overflow
        // Writing to the standard Bitmap class is slow, so this class exists as a faster alternative.
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color color)
        {
            int index = x + (y * Width);
            int col = color.ToArgb();

            Bits[index] = col;
        }

        public void SetPixel(int x, int y, int colorRGBA)
        {
            int index = x + (y * Width);
            Bits[index] = colorRGBA;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }

}
