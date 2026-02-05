using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriCNES;

public class Op
{
    public byte code;
    public string mnemonic;
    public string mode;
    public int length;
    public int affectedFlags;
    public string CycleByCycle;
    public string InstructionDocumentation;

    public Op(byte c, string m, string mo, int l, int a, string d, string i)
    {
        code = c;
        mnemonic = m;
        mode = mo;
        length = l;
        affectedFlags = a;
        CycleByCycle = d;
        InstructionDocumentation = i;
    }
}

public static class Documentation
{

    // This class is exlusively referenced for debugging and trace logging information.
    // It's also possible I mistyped some numbers here and there, and probably shouldn't be 100% trusted.

    //cycle information from https://www.atarihq.com/danb/files/64doc.txt

    static int cFlag = 1;
    static int zFlag = 2;
    static int iFlag = 4;
    static int dFlag = 8;
    static int bFlag = 16;
    static int tFlag = 32;
    static int vFlag = 64;
    static int nFlag = 128;
    static int aChanges = 256;
    static int xChanges = 512;
    static int yChanges = 1024;
    static int stackPChanges = 2048;
    static int pcChanges = 4096;
    static int memChanges = 8192;

    static string[] CycleDocs =
    {
        //0 BRK
        "        #  address R/W description\r\n       --- ------- --- -----------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  read next instruction byte (and throw it away), increment PC\r\n        3  $0100,S  W  push PCH on stack, decrement S\r\n        4  $0100,S  W  push PCL on stack, decrement S\r\n        5  $0100,S  W  push P on stack (with B flag set), decrement S\r\n        6   $FFFE   R  fetch PCL\r\n        7   $FFFF   R  fetch PCH"
        ,
        //1 RTI
        "        #  address R/W description\r\n       --- ------- --- -----------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  read next instruction byte (and throw it away)\r\n        3  $0100,S  R  increment S\r\n        4  $0100,S  R  pull P from stack, increment S\r\n        5  $0100,S  R  pull PCL from stack, increment S\r\n        6  $0100,S  R  pull PCH from stack"
        ,
        //2 RTS
        "        #  address R/W description\r\n       --- ------- --- -----------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  read next instruction byte (and throw it away)\r\n        3  $0100,S  R  increment S\r\n        4  $0100,S  R  pull PCL from stack, increment S\r\n        5  $0100,S  R  pull PCH from stack\r\n        6    PC     R  increment PC"
        ,
        //3 PHA, PHP
        "        #  address R/W description\r\n       --- ------- --- -----------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  read next instruction byte (and throw it away)\r\n        3  $0100,S  W  push register on stack, decrement S"
        ,
        //4 PLA, PLP
        "        #  address R/W description\r\n       --- ------- --- -----------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  read next instruction byte (and throw it away)\r\n        3  $0100,S  R  increment S\r\n        4  $0100,S  R  pull register from stack"
        ,
        //5 JSR
        "        #  address R/W description\r\n       --- ------- --- -------------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch low address byte, increment PC\r\n        3  $0100,S  R  internal operation (predecrement S?)\r\n        4  $0100,S  W  push PCH on stack, decrement S\r\n        5  $0100,S  W  push PCL on stack, decrement S\r\n        6    PC     R  copy low address byte to PCL, fetch high address byte to PCH"
        ,
        //6 Accumulator or implied addressing
        "  Accumulator or implied addressing\r\n\r\n        #  address R/W description\r\n       --- ------- --- -----------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  read next instruction byte (and throw it away)"
        ,
        //7 Immediate addressing
        "        #  address R/W description\r\n       --- ------- --- ------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch value, increment PC"
        ,
        // --Absolute Instructions--
        //8 JMP
        "        #  address R/W description\r\n       --- ------- --- -------------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch low address byte, increment PC\r\n        3    PC     R  copy low address byte to PCL, fetch high address byte to PCH"
        ,
        //9 Read instructions (LDA, LDX, LDY, EOR, AND, ORA, ADC, SBC, CMP, BIT, LAX, NOP)
        "        #  address R/W description\r\n       --- ------- --- ------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch low byte of address, increment PC\r\n        3    PC     R  fetch high byte of address, increment PC\r\n        4  address  R  read from effective address"
        ,
        //10 Read-Modify-Write instructions (ASL, LSR, ROL, ROR, INC, DEC, SLO, SRE, RLA, RRA, ISC, DCP)
        "        #  address R/W description\r\n       --- ------- --- ------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch low byte of address, increment PC\r\n        3    PC     R  fetch high byte of address, increment PC\r\n        4  address  R  read from effective address\r\n        5  address  W  write the value back to effective address, and do the operation on it\r\n        6  address  W  write the new value to effective address"
        ,
        //11 Write instructions (STA, STX, STY, SAX)
        "        #  address R/W description\r\n       --- ------- --- ------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch low byte of address, increment PC\r\n        3    PC     R  fetch high byte of address, increment PC\r\n        4  address  W  write register to effective address"
        ,
        // --Zero page addressing--
        //12 Read instructions (LDA, LDX, LDY, EOR, AND, ORA, ADC, SBC, CMP, BIT, LAX, NOP)
        "        #  address R/W description\r\n       --- ------- --- ------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch address, increment PC\r\n        3  address  R  read from effective address"
        ,
        //13 Read-Modify-Write instructions (ASL, LSR, ROL, ROR, INC, DEC, SLO, SRE, RLA, RRA, ISC, DCP)
        "        #  address R/W description\r\n       --- ------- --- ------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch address, increment PC\r\n        3  address  R  read from effective address\r\n        4  address  W  write the value back to effective address, and do the operation on it\r\n        5  address  W  write the new value to effective address"
        ,
        //14 Write instructions (STA, STX, STY, SAX)
        "        #  address R/W description\r\n       --- ------- --- ------------------------------------------\r\n        1    PC     R  fetch opcode, increment PC\r\n        2    PC     R  fetch address, increment PC\r\n        3  address  W  write register to effective address"
        ,
        // --Zero page indexed addressing--
        //15 Read instructions (LDA, LDX, LDY, EOR, AND, ORA, ADC, SBC, CMP, BIT, LAX, NOP)
        "        #   address  R/W description\r\n       --- --------- --- ------------------------------------------\r\n        1     PC      R  fetch opcode, increment PC\r\n        2     PC      R  fetch address, increment PC\r\n        3   address   R  read from address, add index register to it\r\n        4  address+I* R  read from effective address\r\n\r\n       Notes: I denotes either index register (X or Y).\r\n\r\n              * The high byte of the effective address is always zero,\r\n                i.e. page boundary crossings are not handled."
        ,
        //16 Read-Modify-Write instructions (ASL, LSR, ROL, ROR, INC, DEC, SLO, SRE, RLA, RRA, ISC, DCP)
        "        #   address  R/W description\r\n       --- --------- --- ---------------------------------------------\r\n        1     PC      R  fetch opcode, increment PC\r\n        2     PC      R  fetch address, increment PC\r\n        3   address   R  read from address, add index register X to it\r\n        4  address+X* R  read from effective address\r\n        5  address+X* W  write the value back to effective address, and do the operation on it\r\n        6  address+X* W  write the new value to effective address\r\n\r\n       Note: * The high byte of the effective address is always zero,\r\n               i.e. page boundary crossings are not handled."
        ,
        //17 Write instructions (STA, STX, STY, SAX)
        "        #   address  R/W description\r\n       --- --------- --- -------------------------------------------\r\n        1     PC      R  fetch opcode, increment PC\r\n        2     PC      R  fetch address, increment PC\r\n        3   address   R  read from address, add index register to it\r\n        4  address+I* W  write to effective address\r\n\r\n       Notes: I denotes either index register (X or Y).\r\n\r\n              * The high byte of the effective address is always zero,\r\n                i.e. page boundary crossings are not handled."
        ,
        // --Absolute indexed addressing--
        //18 Read instructions (LDA, LDX, LDY, EOR, AND, ORA, ADC, SBC, CMP, BIT, LAX, LAE, SHS, NOP)
        "        #   address  R/W description\r\n       --- --------- --- ------------------------------------------\r\n        1     PC      R  fetch opcode, increment PC\r\n        2     PC      R  fetch low byte of address, increment PC\r\n        3     PC      R  fetch high byte of address, add index register to low address byte, increment PC\r\n        4  address+I* R  read from effective address, fix the high byte of effective address\r\n        5+ address+I  R  re-read from effective address\r\n\r\n       Notes: I denotes either index register (X or Y).\r\n\r\n              * The high byte of the effective address may be invalid\r\n                at this time, i.e. it may be smaller by $100.\r\n\r\n              + This cycle will be executed only if the effective address\r\n                was invalid during cycle #4, i.e. page boundary was crossed."
        ,
        //19 Read-Modify-Write instructions (ASL, LSR, ROL, ROR, INC, DEC, SLO, SRE, RLA, RRA, ISC, DCP)
        "        #   address  R/W description\r\n       --- --------- --- ------------------------------------------\r\n        1    PC       R  fetch opcode, increment PC\r\n        2    PC       R  fetch low byte of address, increment PC\r\n        3    PC       R  fetch high byte of address, add index register X to low address byte, increment PC\r\n        4  address+X* R  read from effective address, fix the high byte of effective address\r\n        5  address+X  R  re-read from effective address\r\n        6  address+X  W  write the value back to effective address, and do the operation on it\r\n        7  address+X  W  write the new value to effective address\r\n\r\n       Notes: * The high byte of the effective address may be invalid\r\n                at this time, i.e. it may be smaller by $100."
        ,
        //20 Write instructions (STA, STX, STY, SHA, SHX, SHY)
        "        #   address  R/W description\r\n       --- --------- --- ------------------------------------------\r\n        1     PC      R  fetch opcode, increment PC\r\n        2     PC      R  fetch low byte of address, increment PC\r\n        3     PC      R  fetch high byte of address, add index register to low address byte, increment PC\r\n        4  address+I* R  read from effective address, fix the high byte of effective address\r\n        5  address+I  W  write to effective address\r\n\r\n       Notes: I denotes either index register (X or Y).\r\n\r\n              * The high byte of the effective address may be invalid\r\n                at this time, i.e. it may be smaller by $100. Because\r\n                the processor cannot undo a write to an invalid\r\n                address, it always reads from the address first."
        ,
        //21 Relative addressing (BCC, BCS, BNE, BEQ, BPL, BMI, BVC, BVS)
        "        #   address  R/W description\r\n       --- --------- --- ---------------------------------------------\r\n        1     PC      R  fetch opcode, increment PC\r\n        2     PC      R  fetch operand, increment PC. If branch is not taken, the instruction has ended.\r\n        3+    PC      R  If branch is taken, add operand to PCL.\r\n        4!    PC*     R  Fix PCH.\r\n        Notes: * The high byte of Program Counter (PCH) may be invalid\r\n                at this time, i.e. it may be smaller or bigger by $100.\r\n\r\n              + If branch is taken, this cycle will be executed.\r\n\r\n              ! If branch occurs to different page, this cycle will be\r\n                executed."
        ,
        // --Indexed indirect addressing--
        //22 Read instructions (LDA, ORA, EOR, AND, ADC, CMP, SBC, LAX)
        "        #    address   R/W description\r\n       --- ----------- --- ------------------------------------------\r\n        1      PC       R  fetch opcode, increment PC\r\n        2      PC       R  fetch pointer address, increment PC\r\n        3    pointer    R  read from the address, add X to it\r\n        4   pointer+X   R  fetch effective address low\r\n        5  pointer+X+1  R  fetch effective address high\r\n        6    address    R  read from effective address\r\n\r\n       Note: The effective address is always fetched from zero page,\r\n             i.e. the zero page boundary crossing is not handled."
        ,
        //23 Read-Modify-Write instructions (SLO, SRE, RLA, RRA, ISC, DCP)
        "        #    address   R/W description\r\n       --- ----------- --- ------------------------------------------\r\n        1      PC       R  fetch opcode, increment PC\r\n        2      PC       R  fetch pointer address, increment PC\r\n        3    pointer    R  read from the address, add X to it\r\n        4   pointer+X   R  fetch effective address low\r\n        5  pointer+X+1  R  fetch effective address high\r\n        6    address    R  read from effective address\r\n        7    address    W  write the value back to effective address, and do the operation on it\r\n        8    address    W  write the new value to effective address\r\n\r\n       Note: The effective address is always fetched from zero page,\r\n             i.e. the zero page boundary crossing is not handled."
        ,
        //24 Write instructions (STA, SAX)
        "        #    address   R/W description\r\n       --- ----------- --- ------------------------------------------\r\n        1      PC       R  fetch opcode, increment PC\r\n        2      PC       R  fetch pointer address, increment PC\r\n        3    pointer    R  read from the address, add X to it\r\n        4   pointer+X   R  fetch effective address low\r\n        5  pointer+X+1  R  fetch effective address high\r\n        6    address    W  write to effective address\r\n\r\n       Note: The effective address is always fetched from zero page,\r\n             i.e. the zero page boundary crossing is not handled."
        ,
        // --Indirect indexed addressing--
        //25 Read instructions (LDA, EOR, AND, ORA, ADC, SBC, CMP)
        "        #    address   R/W description\r\n       --- ----------- --- ------------------------------------------\r\n        1      PC       R  fetch opcode, increment PC\r\n        2      PC       R  fetch pointer address, increment PC\r\n        3    pointer    R  fetch effective address low\r\n        4   pointer+1   R  fetch effective address high, add Y to low byte of effective address\r\n        5   address+Y*  R  read from effective address, fix high byte of effective address\r\n        6+  address+Y   R  read from effective address\r\n\r\n       Notes: The effective address is always fetched from zero page,\r\n              i.e. the zero page boundary crossing is not handled.\r\n\r\n              * The high byte of the effective address may be invalid\r\n                at this time, i.e. it may be smaller by $100.\r\n\r\n              + This cycle will be executed only if the effective address\r\n                was invalid during cycle #5, i.e. page boundary was crossed."
        ,
        //26 Read-Modify-Write instructions (SLO, SRE, RLA, RRA, ISC, DCP)
        "        #    address   R/W description\r\n       --- ----------- --- ------------------------------------------\r\n        1      PC       R  fetch opcode, increment PC\r\n        2      PC       R  fetch pointer address, increment PC\r\n        3    pointer    R  fetch effective address low\r\n        4   pointer+1   R  fetch effective address high, add Y to low byte of effective address\r\n        5   address+Y*  R  read from effective address, fix high byte of effective address\r\n        6   address+Y   R  read from effective address\r\n        7   address+Y   W  write the value back to effective address, and do the operation on it\r\n        8   address+Y   W  write the new value to effective address\r\n\r\n       Notes: The effective address is always fetched from zero page,\r\n              i.e. the zero page boundary crossing is not handled.\r\n\r\n              * The high byte of the effective address may be invalid\r\n                at this time, i.e. it may be smaller by $100."
        ,
        //27 Write instructions (STA, SHA)
        "        #    address   R/W description\r\n       --- ----------- --- ------------------------------------------\r\n        1      PC       R  fetch opcode, increment PC\r\n        2      PC       R  fetch pointer address, increment PC\r\n        3    pointer    R  fetch effective address low\r\n        4   pointer+1   R  fetch effective address high, add Y to low byte of effective address\r\n        5   address+Y*  R  read from effective address, fix high byte of effective address\r\n        6   address+Y   W  write to effective address\r\n\r\n       Notes: The effective address is always fetched from zero page,\r\n              i.e. the zero page boundary crossing is not handled.\r\n\r\n              * The high byte of the effective address may be invalid\r\n                at this time, i.e. it may be smaller by $100."
        ,
        // --Absolute indirect addressing--
        //28 JMP
        "        #   address  R/W description\r\n       --- --------- --- ------------------------------------------\r\n        1     PC      R  fetch opcode, increment PC\r\n        2     PC      R  fetch pointer address low, increment PC\r\n        3     PC      R  fetch pointer address high, increment PC\r\n        4   pointer   R  fetch low address to latch\r\n        5  pointer+1* R  fetch PCH, copy latch to PCL\r\n\r\n       Note: * The PCH will always be fetched from the same page\r\n               than PCL, i.e. page boundary crossing is not handled.\r\n\r\n                How Real Programmers Acknowledge Interrupts"
        ,
        //29
        //HLT
        "        #   address  R/W description\r\n       --- --------- --- ------------------------------------------\r\n        1     PC      R  fetch opcode, does not increment PC\r\n        2     PC      R  fetch opcode, does not increment PC\r\n        3     PC      R  fetch opcode, does not increment PC\r\n        4     PC      R  fetch opcode, does not increment PC\r\n        5     PC      R  fetch opcode, does not increment PC\r\n        6     PC      R  fetch opcode, does not increment PC\r\n        7     PC      R  fetch opcode, does not increment PC\r\n        ...   PC      R  fetch opcode, does not increment PC\r\n\r\n       Notes: This process goes on forever."
    };

    // this explains what each isntruction does by their pnuemonic

    static string[] InstructionDocs =
    {
        //0 BRK
        "Break\n\nPushes PC to the stack.\n\nPushes processor status to the stack.\nPC' = ($FFFE)\nSP' = SP-3"
        ,
        //1 ORA
        "Bitwise OR with Accumulator\n\nA' = A|M\n\nZFlag' = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //2 HLT
        "Halt\n\nHalts the processor."
        ,
        //3 NOP
        "No operation."
        ,
        //4 ASL
        "Arithmetic Shift Left\n\nM' = M<<1\n\nCflag' = (M>=0x80)\nZflag' = (M'==0)\nNflag' = (M'>=0x80)"
        ,
        //5 SLO
        "Arithemtic Shift Left then Bitwise OR with Accumulator\n\nM' = M<<1\nA' = A|M'\n\nCflag' = (M>=0x80)\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //6 PHP
        "Push Processor\n\nPushes processor status to the stack.\n\nSP' = SP-1"
        ,
        //7 ANC
        "Bitwise AND with Accumulator then Set Carry if Negative\n\nA' = A & M\n\nCflag' = (A'>=0x80)\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //8 BPL
        "Branch on Plus\n\nIf the negative flag is not set, branch.\n\nPC' = PC + (!NFlag ? SignedOperand : 0)"
        ,
        //9 CLC
        "Clear Carry Flag\n\nCFlag' = false"
        ,
        //10 JSR
        "Jump to Subroutine\n\nPushes PC to the stack\nPC' = Operand\nSP' = SP-2"
        ,
        //11 AND
        "Bitwise AND with Accumulator\n\nA' = A&M\n\nZFlag = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //12 RLA
        "Rotate Left then Bitwise AND with Accumulator\n\nM' = M<<1 + CFlag\nA' = A&M'\n\nCflag' = (M>=0x80)\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //13 BIT
        "Bit Test\n\nZFlag = (A=M)\nNFlag = ((M>>7)&1==1)\nVFlag = ((M>>6)&1==1)"
        ,
        //14 ROL
        "Rotate Left\n\nM' = M<<1 + CFlag\n\nCflag' = (M>=0x80)\nZflag' = (M'==0)\nNflag' = (M'>=0x80)"
        ,
        //15 PLP
        "Pull Processor\n\nPulls processor status from the stack.\n\nSP' = SP+1"
        ,
        //16 BMI
        "Branch on Minus\n\nIf the negative flag is set, branch.\n\nPC' = PC + (NFlag ? SignedOperand : 0)"
        ,
        //17 SEC
        "Set Carry Flag\n\nCFlag' = true"
        ,
        //18 RTI
        "Return from Interrupt\n\nPulls the processor from the stack.\nPulls PC from the stack.\n\nSP' = SP+3"
        ,
        //19 EOR
        "Bitwise Exclusive OR with Accumulator\n\nA' = A^M\n\nZFlag' = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //20 SRE
        "Logical Shift Right then Bitwise Exclusive OR with Accumulator\n\nM' = M>>2\nA' = A^M'\n\nCFlag' = (M&1==1)\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //21 LSR
        "Logical Shift Right\n\nM' = M>>2\n\nCFlag' = (M&1==1)\nZflag' = (M'==0)\nNflag' = (M'>=0x80)"
        ,
        //22 PHA
        "Push A\n\nPushes A to the stack.\n\nSP' = SP-1"
        ,
        //23 ASR
        "Bitwise AND with Accumulator then Logical Shift Right Accumulator\n\nA' = ((A&M)>>1)\nCFlag' = ((A&M)&1==1)\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //24 JMP
        "Jump\n\nPC' = M"
        ,
        //25 BVC
        "Branch on Overflow Clear\n\nIf the overflow flag is not set, branch.\n\nPC' = PC + (!VFlag ? SignedOperand : 0)"
        ,
        //26 CLI
        "Clear Interrupt Disable Flag\n\nIFlag' = false"
        ,
        //27 RTS
        "Return from Subroutine\n\nPulls the PC from the stack.\\SP' = SP + 2"
        ,
        //28 ADC
        "Add with Carry\n\nA' = A + M + CFlag\n\nVFlag' = ((A ^ (M + A + Carry)) & ((M + A + Carry) & M) & 0x80) == 0x80\nCFlag' = A + M > 0xFF\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //29 RRA
        "Rotate Right then Add With Carry\n\nM' = (M>>1)+Cflag*0x80\n\nVFlag' = ((A ^ (M + A + Carry)) & ((M + A + Carry) & M) & 0x80) == 0x80\nCFlag' = A + M > 0xFF\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //30 ROR
        "Rotate Right\n\nM' = M>>1 + CFlag*0x80\n\nCflag' = (M&1)\nZflag' = (M'==0)\nNflag' = (M'>=0x80)"
        ,
        //31 PLA
        "Pull A\n\nPull A from the stack\n\nSP' = SP+1"
        ,
        //32 ARR
        "Bitwise AND with A then Rotate A and check bits\n\nA' = ((A&M)>>1)+Carry*0x80\n\nCFlag = ((A'>>6)&1==1)\nVFlag = ((A'>>5)&1==1)\nZFlag' = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //33 BVS
        "Branch on Overflow Set\n\nIf the overflow flag is set, branch.\n\nPC' = PC + (VFlag ? SignedOperand : 0)"
        ,
        //34 SEI
        "Set Interrupt Disable Flag\n\nIFlag' = true"
        ,
        //35 STA
        "Store A\n\nM' = A"
        ,
        //36 SAX
        "Store A and X\n\nM' = A&X"
        ,
        //37 STY
        "Store Y\n\nM' = Y"
        ,
        //38 STX
        "Store X\n\nM' = X"
        ,
        //39 DEY
        "Decrement Y\n\nY' = Y-1\n\nZFlag' = (Y'==0)\nNFlag' = (Y'>=0x80)"
        ,
        //40 TXA
        "Transfer X to A\n\nA' = X\n\nZFlag' = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //41 ANE
        "Bitwise OR A with Magic then Bitwise AND with X AND with Memory\n\nA' = (A | magic) & X & M\n\n     *Note: 'Magic' depends on the chip manufacturer\n            'Magic' is usually 00, EE, EF, FE, or FF"
        ,
        //42 BCC
        "Branch on Carry Clear\n\nIf the carry flag is not set, branch.\n\nPC' = PC + (!CFlag ? SignedOperand : 0)"
        ,
        //43 TXS
        "Transfer X to Stack Pointer\n\nSP' = X"
        ,
        //44 SHA
        "Store Bitwise AND X with A AND the High Byte of the Operand Plus 1\n\nM' = A & X & (HIGH(Arg)+1)"
        ,
        //45 TYA
        "Transfer Y to A\n\nA' = Y\n\nZFlag' = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //46 SHY
        "Store Bitwise AND Y with The High Byte of the Operand Plus 1\n\nM' = Y & (HIGH(Arg)+1)"
        ,
        //47 SHS
        "Transfer Bitwise AND A with X to Stack Pointer then Store Bitwise And Stack Pointer with the High Byte of the Operand Plus 1\n\nSP' = A&X\nM' = SP'&(HIGH(Arg)+1)"
        ,
        //48 SHX
        "Store Bitwise AND X with The High Byte of the Operand Plus 1\n\nM' = X & (HIGH(Arg)+1)"
        ,
        //49 LDY
        "Load Y\n\nY' = M\n\nZFlag' = (Y'==0)\nNFlag' = (Y'>=0x80)"
        ,
        //50 LDA
        "Load A\n\nA' = M\n\nZFlag' = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //51 LDX
        "Load X\n\nX' = M\n\nZFlag' = (X'==0)\nNFlag' = (X'>=0x80)"
        ,
        //52 LAX
        "Load A X\n\nA' = M\nX' = M\n\nZFlag' = (X'==0)\nNFlag' = (X'>=0x80)"
        ,
        //53 TAY
        "Transfer A to Y\n\nY' = A\n\nZFlag' = (Y'==0)\nNFlag' = (Y'>=0x80)"
        ,
        //54 TAX
        "Transfer A to X\n\nX' = A\n\nZFlag' = (X'==0)\nNFlag' = (X'>=0x80)"
        ,
        //55 LXA
        "Bitwise AND with A then Transfer A to X\n\nA' = A&M\nX'=A'\n\nZFlag' = (X'==0)\nNFlag' = (X'>=0x80)"
        ,
        //56 BCS
        "Branch on Carry Set\n\nIf the carry flag is set, branch.\n\nPC' = PC + (CFlag ? SignedOperand : 0)"
        ,
        //57 CLV
        "Clear Overflow\n\nVFlag = false"
        ,
        //58 TSX
        "Transfer Stack Pointer to X\n\nX' = SP"
        ,
        //59 LAS
        "Transfer Bitwise AND with Stack Pointer to A, X, and Stack Pointer\n\nA' = M&SP\nX' = M&SP\nSP' = M&SP"
        ,
        //60 CPY
        "Compare Y\n\nZFlag' = (Y==M)\nCFlag' = (Y>=M)\nNFlag' = (Y-M)>0x80"
        ,
        //61 CMP
        "Compare A\n\nZFlag' = (A==M)\nCFlag' = (A>=M)\nNFlag' = (A-M)>0x80"
        ,
        //62 DCP
        "Decrement then Compare A\n\nM' = M-1\nZFlag' = (A==M')\nCFlag' = (A>=M')\nNFlag' = (A-M')>0x80"
        ,
        //63 DEC
        "Decrement\n\nM' = M-1\n\nZFlag' = (M'==0)\nNFlag' = (M'>=0x80)"
        ,
        //64 INY
        "Increment Y\n\nY' = Y+1\n\nZFlag' = (Y'==0)\nNFlag' = (Y'>=0x80)"
        ,
        //65 DEX
        "Decrement X\n\nX' = X-1\n\nZFlag' = (X'==0)\nNFlag' = (X'>=0x80)"
        ,
        //66 AXS
        "Load X with Subtraction with Bitwise AND X with A\n\nX' = (A&X)-M\n\nZFlag' = (X==M)\nCFlag' = (X>=M)\nNFlag' = (X-M)>0x80"
        ,
        //67 BNE
        "Branch on Not Equal\n\nIf the zero flag is not set, branch.\n\nPC' = PC + (!ZFlag ? SignedOperand : 0)"
        ,
        //68 CLD
        "Clear Decimal Flag\n\nDFlag = false"
        ,
        //69 CPX
        "Compare X\n\nZFlag' = (X==M)\nCFlag' = (X>=M)\nNFlag' = (X-M)>0x80"
        ,
        //70 SBC
        "Subtract with Carry\n\nA' = A+(0xFF-M)+CFlag\n\nVFlag' = ((A ^ (M + A + Carry)) & ((M + A + Carry) & M) & 0x80) == 0x80\nCFlag' = A + M > 0xFF\nZflag' = (A'==0)\nNflag' = (A'>=0x80)"
        ,
        //71 ISC
        "Increment then subtract from accumulator\n\nM' = M+1\nA' = A+(0xFF-M')+CFlag\n\nVFlag' = ((A ^ (M' + A + Carry)) & ((M' + A + Carry) & M') & 0x80) == 0x80\nCFlag' = A + M' > 0xFF\nZflag' = (M'==0)\nNflag' = (M'>=0x80)"
        ,
        //72 INC
        "Increment\n\nM' = M+1\n\nZFlag' = (A'==0)\nNFlag' = (A'>=0x80)"
        ,
        //73 INX
        "Increment\n\nX' = X+1\n\nZFlag' = (X'==0)\nNFlag' = (X'>=0x80)"
        ,
        //74 BEQ
        "Branch on Equal\n\nIf the zero flag is set, branch.\n\nPC' = PC + (ZFlag ? SignedOperand : 0)"
        ,
        //75 SED
        "Set Decimal\n\nDFlag = true"

    };

    // this table is referenced in the debugging stuff.
    // basically, for each index into this array, you can fetch an opcode's name, addressing mode, what flags/registers it can modify, documentation, and the number of cycles before a read/write.

    public static Op[] OpDocs = {
        new Op(0x00,"BRK","i"       ,2,bFlag | stackPChanges | pcChanges            ,CycleDocs[0] ,InstructionDocs[0]),
        new Op(0x01,"ORA","(d,x)"   ,2,nFlag | zFlag | aChanges                     ,CycleDocs[22],InstructionDocs[1]),
        new Op(0x02,"HLT","i"       ,1,0                                            ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x03,"SLO","(d,x)"   ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[23],InstructionDocs[5]),
        new Op(0x04,"NOP","d"       ,2,0                                            ,CycleDocs[12],InstructionDocs[3]),
        new Op(0x05,"ORA","d"       ,2,nFlag | zFlag | aChanges                     ,CycleDocs[12],InstructionDocs[1]),
        new Op(0x06,"ASL","d"       ,2,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[13],InstructionDocs[4]),
        new Op(0x07,"SLO","d"       ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[13],InstructionDocs[5]),
        new Op(0x08,"PHP","i"       ,1,stackPChanges                                ,CycleDocs[3] ,InstructionDocs[6]),
        new Op(0x09,"ORA","#v"      ,2,nFlag | zFlag | aChanges                     ,CycleDocs[7] ,InstructionDocs[1]),
        new Op(0x0A,"ASL","A"       ,1,nFlag | zFlag | cFlag | aChanges             ,CycleDocs[6] ,InstructionDocs[4]),
        new Op(0x0B,"ANC","#v"      ,2,nFlag | zFlag | cFlag | aChanges             ,CycleDocs[7] ,InstructionDocs[7]),
        new Op(0x0C,"NOP","a"       ,3,0                                            ,CycleDocs[9] ,InstructionDocs[3]),
        new Op(0x0D,"ORA","a"       ,3,nFlag | zFlag | aChanges                     ,CycleDocs[9] ,InstructionDocs[1]),
        new Op(0x0E,"ASL","a"       ,3,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[10],InstructionDocs[4]),
        new Op(0x0F,"SLO","a"       ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[10],InstructionDocs[5]),
        new Op(0x10,"BPL","r"       ,2,pcChanges                                    ,CycleDocs[21],InstructionDocs[8]),
        new Op(0x11,"ORA","(d),y"   ,2,nFlag | zFlag | aChanges                     ,CycleDocs[25],InstructionDocs[1]),
        new Op(0x12,"HLT","i"       ,1,0                                            ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x13,"SLO","(d),y"   ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[26],InstructionDocs[5]),
        new Op(0x14,"NOP","d,x"     ,2,0                                            ,CycleDocs[15],InstructionDocs[3]),
        new Op(0x15,"ORA","d,x"     ,2,nFlag | zFlag | aChanges                     ,CycleDocs[15],InstructionDocs[1]),
        new Op(0x16,"ASL","d,x"     ,2,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[16],InstructionDocs[4]),
        new Op(0x17,"SLO","d,x"     ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[16],InstructionDocs[5]),
        new Op(0x18,"CLC","i"       ,1,cFlag                                        ,CycleDocs[6] ,InstructionDocs[9]),
        new Op(0x19,"ORA","a,y"     ,3,nFlag | zFlag | aChanges                     ,CycleDocs[18],InstructionDocs[1]),
        new Op(0x1A,"NOP","i"       ,1,0                                            ,CycleDocs[6] ,InstructionDocs[3]),
        new Op(0x1B,"SLO","a,y"     ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[5]),
        new Op(0x1C,"NOP","a,x"     ,3,0                                            ,CycleDocs[18],InstructionDocs[3]),
        new Op(0x1D,"ORA","a,x"     ,3,nFlag | zFlag | aChanges                     ,CycleDocs[18],InstructionDocs[1]),
        new Op(0x1E,"ASL","a,x"     ,3,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[19],InstructionDocs[4]),
        new Op(0x1F,"SLO","a,x"     ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[5]),

        new Op(0x20,"JSR","a"       ,3,stackPChanges | pcChanges                    ,CycleDocs[5] ,InstructionDocs[10]),
        new Op(0x21,"AND","(d,x)"   ,2,nFlag | zFlag | aChanges                     ,CycleDocs[22],InstructionDocs[11]),
        new Op(0x22,"HLT","i"       ,1,0                                            ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x23,"RLA","(d,x)"   ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[23],InstructionDocs[12]),
        new Op(0x24,"BIT","d"       ,2,nFlag | zFlag | cFlag | vFlag                ,CycleDocs[12],InstructionDocs[13]),
        new Op(0x25,"AND","d"       ,2,nFlag | zFlag | aChanges                     ,CycleDocs[12],InstructionDocs[11]),
        new Op(0x26,"ROL","d"       ,2,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[13],InstructionDocs[14]),
        new Op(0x27,"RLA","d"       ,2,nFlag | zFlag | cFlag | aChanges             ,CycleDocs[13],InstructionDocs[12]),
        new Op(0x28,"PLP","i"       ,1,cFlag|zFlag|iFlag|dFlag|bFlag|tFlag|vFlag|nFlag,CycleDocs[4],InstructionDocs[15]),
        new Op(0x29,"AND","#v"      ,2,nFlag | zFlag | aChanges                     ,CycleDocs[7] ,InstructionDocs[11]),
        new Op(0x2A,"ROL","A"       ,1,nFlag | zFlag | cFlag | aChanges             ,CycleDocs[6] ,InstructionDocs[14]),
        new Op(0x2B,"ANC","#v"      ,2,nFlag | zFlag | cFlag | aChanges             ,CycleDocs[7] ,InstructionDocs[7]),
        new Op(0x2C,"BIT","a"       ,3,nFlag | zFlag | cFlag | vFlag                ,CycleDocs[9] ,InstructionDocs[13]),
        new Op(0x2D,"AND","a"       ,3,nFlag | zFlag | aChanges                     ,CycleDocs[9] ,InstructionDocs[11]),
        new Op(0x2E,"ROL","a"       ,3,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[10],InstructionDocs[14]),
        new Op(0x2F,"RLA","a"       ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[10],InstructionDocs[12]),
        new Op(0x30,"BMI","r"       ,2,pcChanges                                    ,CycleDocs[21],InstructionDocs[16]),
        new Op(0x31,"AND","(d),y"   ,2,nFlag | zFlag | aChanges                     ,CycleDocs[25],InstructionDocs[11]),
        new Op(0x32,"HLT","i"       ,1,0                                            ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x33,"RLA","(d),y"   ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[26],InstructionDocs[12]),
        new Op(0x34,"NOP","d,x"     ,2,0                                            ,CycleDocs[15],InstructionDocs[3]),
        new Op(0x35,"AND","d,x"     ,2,nFlag | zFlag | aChanges                     ,CycleDocs[15],InstructionDocs[11]),
        new Op(0x36,"ROL","d,x"     ,2,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[16],InstructionDocs[14]),
        new Op(0x37,"RLA","d,x"     ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[16],InstructionDocs[12]),
        new Op(0x38,"SEC","i"       ,1,cFlag                                        ,CycleDocs[6] ,InstructionDocs[17]),
        new Op(0x39,"AND","a,y"     ,3,nFlag | zFlag | aChanges                     ,CycleDocs[18],InstructionDocs[11]),
        new Op(0x3A,"NOP","i"       ,1,0                                            ,CycleDocs[6] ,InstructionDocs[3]),
        new Op(0x3B,"RLA","a,y"     ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[12]),
        new Op(0x3C,"NOP","a,x"     ,3,0                                            ,CycleDocs[18],InstructionDocs[3]),
        new Op(0x3D,"AND","a,x"     ,3,nFlag | zFlag | aChanges                     ,CycleDocs[18],InstructionDocs[11]),
        new Op(0x3E,"ROL","a,x"     ,3,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[19],InstructionDocs[14]),
        new Op(0x3F,"RLA","a,x"     ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[12]),

        new Op(0x40,"RTI","i"       ,1,0xFF | stackPChanges | pcChanges             ,CycleDocs[1] ,InstructionDocs[18]),
        new Op(0x41,"EOR","(d,x)"   ,2,nFlag | zFlag | aChanges                     ,CycleDocs[22],InstructionDocs[19]),
        new Op(0x42,"HLT","i"       ,1,0                                            ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x43,"SRE","(d,x)"   ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[23],InstructionDocs[20]),
        new Op(0x44,"NOP","d"       ,2,0                                            ,CycleDocs[12],InstructionDocs[3]),
        new Op(0x45,"EOR","d"       ,2,nFlag | zFlag | aChanges                     ,CycleDocs[12],InstructionDocs[19]),
        new Op(0x46,"LSR","d"       ,2,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[13],InstructionDocs[21]),
        new Op(0x47,"SRE","d"       ,2,nFlag | zFlag | cFlag                        ,CycleDocs[13],InstructionDocs[20]),
        new Op(0x48,"PHA","i"       ,1,stackPChanges                                ,CycleDocs[3] ,InstructionDocs[22]),
        new Op(0x49,"EOR","#v"      ,2,nFlag | zFlag                                ,CycleDocs[7] ,InstructionDocs[19]),
        new Op(0x4A,"LSR","A"       ,1,nFlag | zFlag | cFlag | aChanges             ,CycleDocs[6] ,InstructionDocs[21]),
        new Op(0x4B,"ASR","#v"      ,2,nFlag | zFlag | cFlag | aChanges             ,CycleDocs[7] ,InstructionDocs[23]),
        new Op(0x4C,"JMP","a"       ,3,0                                            ,CycleDocs[8] ,InstructionDocs[24]),
        new Op(0x4D,"EOR","a"       ,3,nFlag | zFlag | aChanges                     ,CycleDocs[9] ,InstructionDocs[19]),
        new Op(0x4E,"LSR","a"       ,3,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[10],InstructionDocs[21]),
        new Op(0x4F,"SRE","a"       ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[10],InstructionDocs[20]),
        new Op(0x50,"BVC","r"       ,2,pcChanges                                    ,CycleDocs[21],InstructionDocs[25]),
        new Op(0x51,"EOR","(d),y"   ,2,nFlag | zFlag | aChanges                     ,CycleDocs[25],InstructionDocs[19]),
        new Op(0x52,"HLT","i"       ,1,0                                            ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x53,"SRE","(d),y"   ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[26],InstructionDocs[20]),
        new Op(0x54,"NOP","d,x"     ,2,0                                            ,CycleDocs[15],InstructionDocs[3]),
        new Op(0x55,"EOR","d,x"     ,2,nFlag | zFlag | aChanges                     ,CycleDocs[15],InstructionDocs[19]),
        new Op(0x56,"LSR","d,x"     ,2,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[16],InstructionDocs[21]),
        new Op(0x57,"SRE","d,x"     ,2,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[16],InstructionDocs[20]),
        new Op(0x58,"CLI","i"       ,1,iFlag                                        ,CycleDocs[6] ,InstructionDocs[26]),
        new Op(0x59,"EOR","a,y"     ,3,nFlag | zFlag | aChanges                     ,CycleDocs[18],InstructionDocs[19]),
        new Op(0x5A,"NOP","i"       ,1,0                                            ,CycleDocs[6] ,InstructionDocs[3]),
        new Op(0x5B,"SRE","a,y"     ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[20]),
        new Op(0x5C,"NOP","a,x"     ,3,0                                            ,CycleDocs[18],InstructionDocs[3]),
        new Op(0x5D,"EOR","a,x"     ,3,nFlag | zFlag | aChanges                     ,CycleDocs[18],InstructionDocs[19]),
        new Op(0x5E,"LSR","a,x"     ,3,nFlag | zFlag | cFlag | memChanges           ,CycleDocs[19],InstructionDocs[21]),
        new Op(0x5F,"SRE","a,x"     ,3,nFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[20]),

        new Op(0x60,"RTS","i"       ,1,stackPChanges | pcChanges                            ,CycleDocs[2] ,InstructionDocs[27]),
        new Op(0x61,"ADC","(d,x)"   ,2,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[22],InstructionDocs[28]),
        new Op(0x62,"HLT","i"       ,1,0                                                    ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x63,"RRA","(d,x)"   ,2,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[23],InstructionDocs[29]),
        new Op(0x64,"NOP","d"       ,2,0                                                    ,CycleDocs[12],InstructionDocs[3]),
        new Op(0x65,"ADC","d"       ,2,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[12],InstructionDocs[28]),
        new Op(0x66,"ROR","d"       ,2,nFlag | zFlag | cFlag | memChanges                   ,CycleDocs[13],InstructionDocs[30]),
        new Op(0x67,"RRA","d"       ,2,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[13],InstructionDocs[29]),
        new Op(0x68,"PLA","i"       ,1,stackPChanges | aChanges                             ,CycleDocs[4] ,InstructionDocs[31]),
        new Op(0x69,"ADC","#v"      ,2,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[7] ,InstructionDocs[28]),
        new Op(0x6A,"ROR","A"       ,1,nFlag | zFlag | cFlag | aChanges                     ,CycleDocs[6] ,InstructionDocs[30]),
        new Op(0x6B,"ARR","#v"      ,2,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[7] ,InstructionDocs[32]),
        new Op(0x6C,"JMP","(a)"     ,3,0                                                    ,CycleDocs[28],InstructionDocs[24]),
        new Op(0x6D,"ADC","a"       ,3,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[9] ,InstructionDocs[28]),
        new Op(0x6E,"ROR","a"       ,3,nFlag | zFlag | cFlag | memChanges                   ,CycleDocs[10],InstructionDocs[30]),
        new Op(0x6F,"RRA","a"       ,3,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[10],InstructionDocs[29]),
        new Op(0x70,"BVS","r"       ,2,pcChanges                                            ,CycleDocs[21],InstructionDocs[33]),
        new Op(0x71,"ADC","(d),y"   ,2,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[25],InstructionDocs[28]),
        new Op(0x72,"HLT","i"       ,1,0                                                    ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x73,"RRA","(d),y"   ,2,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[26],InstructionDocs[29]),
        new Op(0x74,"NOP","d,x"     ,2,0                                                    ,CycleDocs[15],InstructionDocs[3]),
        new Op(0x75,"ADC","d,x"     ,2,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[15],InstructionDocs[28]),
        new Op(0x76,"ROR","d,x"     ,2,nFlag | zFlag | cFlag | memChanges                   ,CycleDocs[16],InstructionDocs[30]),
        new Op(0x77,"RRA","d,x"     ,2,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[16],InstructionDocs[29]),
        new Op(0x78,"SEI","i"       ,1,iFlag                                                ,CycleDocs[6] ,InstructionDocs[34]),
        new Op(0x79,"ADC","a,y"     ,3,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[18],InstructionDocs[28]),
        new Op(0x7A,"NOP","i"       ,1,0                                                    ,CycleDocs[6] ,InstructionDocs[3]),
        new Op(0x7B,"RRA","a,y"     ,3,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[29]),
        new Op(0x7C,"NOP","a,x"     ,3,0                                                    ,CycleDocs[18],InstructionDocs[3]),
        new Op(0x7D,"ADC","a,x"     ,3,nFlag | vFlag | zFlag | cFlag | aChanges             ,CycleDocs[18],InstructionDocs[28]),
        new Op(0x7E,"ROR","a,x"     ,3,nFlag | zFlag | cFlag | memChanges                   ,CycleDocs[19],InstructionDocs[30]),
        new Op(0x7F,"RRA","a,x"     ,3,nFlag | vFlag | zFlag | cFlag | aChanges | memChanges,CycleDocs[19],InstructionDocs[29]),

        new Op(0x80,"NOP","#v"      ,2,0                        ,CycleDocs[7] ,InstructionDocs[3]),
        new Op(0x81,"STA","(d,x)"   ,2,memChanges               ,CycleDocs[24],InstructionDocs[35]),
        new Op(0x82,"NOP","#v"      ,2,0                        ,CycleDocs[7] ,InstructionDocs[3]),
        new Op(0x83,"SAX","(d,x)"   ,2,memChanges               ,CycleDocs[24],InstructionDocs[36]),
        new Op(0x84,"STY","d"       ,2,memChanges               ,CycleDocs[14],InstructionDocs[37]),
        new Op(0x85,"STA","d"       ,2,memChanges               ,CycleDocs[14],InstructionDocs[35]),
        new Op(0x86,"STX","d"       ,2,memChanges               ,CycleDocs[14],InstructionDocs[38]),
        new Op(0x87,"SAX","d"       ,2,memChanges               ,CycleDocs[14],InstructionDocs[36]),
        new Op(0x88,"DEY","i"       ,1,yChanges | nFlag | zFlag ,CycleDocs[6] ,InstructionDocs[39]),
        new Op(0x89,"NOP","#v"      ,2,0                        ,CycleDocs[7] ,InstructionDocs[3]),
        new Op(0x8A,"TXA","i"       ,1,aChanges | nFlag | zFlag ,CycleDocs[7] ,InstructionDocs[40]),
        new Op(0x8B,"ANE","#v"      ,2,aChanges | nFlag | zFlag ,CycleDocs[7] ,InstructionDocs[41]),
        new Op(0x8C,"STY","a"       ,3,memChanges               ,CycleDocs[11],InstructionDocs[37]),
        new Op(0x8D,"STA","a"       ,3,memChanges               ,CycleDocs[11],InstructionDocs[35]),
        new Op(0x8E,"STX","a"       ,3,memChanges               ,CycleDocs[11],InstructionDocs[38]),
        new Op(0x8F,"SAX","a"       ,3,memChanges               ,CycleDocs[11],InstructionDocs[36]),
        new Op(0x90,"BCC","r"       ,2,pcChanges                ,CycleDocs[21],InstructionDocs[42]),
        new Op(0x91,"STA","(d),y"   ,2,memChanges               ,CycleDocs[27],InstructionDocs[35]),
        new Op(0x92,"HLT","i"       ,1,memChanges               ,CycleDocs[29],InstructionDocs[2]),
        new Op(0x93,"SHA","(d),y"   ,2,memChanges               ,CycleDocs[27],InstructionDocs[44]),
        new Op(0x94,"STY","d,x"     ,2,memChanges               ,CycleDocs[17],InstructionDocs[37]),
        new Op(0x95,"STA","d,x"     ,2,memChanges               ,CycleDocs[17],InstructionDocs[35]),
        new Op(0x96,"STX","d,y"     ,2,memChanges               ,CycleDocs[17],InstructionDocs[38]),
        new Op(0x97,"SAX","d,y"     ,2,memChanges               ,CycleDocs[17],InstructionDocs[36]),
        new Op(0x98,"TYA","i"       ,1,aChanges | nFlag | zFlag ,CycleDocs[6] ,InstructionDocs[45]),
        new Op(0x99,"STA","a,y"     ,3,memChanges               ,CycleDocs[20],InstructionDocs[35]),
        new Op(0x9A,"TXS","i"       ,1,stackPChanges            ,CycleDocs[6] ,InstructionDocs[43]),
        new Op(0x9B,"SHS","a,y"     ,3,stackPChanges | memChanges,CycleDocs[20],InstructionDocs[47]),
        new Op(0x9C,"SHY","a,x"     ,3,memChanges               ,CycleDocs[20],InstructionDocs[46]),
        new Op(0x9D,"STA","a,x"     ,3,memChanges               ,CycleDocs[20],InstructionDocs[35]),
        new Op(0x9E,"SHX","a,y"     ,3,memChanges               ,CycleDocs[20],InstructionDocs[48]),
        new Op(0x9F,"SHA","a,y"     ,3,memChanges               ,CycleDocs[20],InstructionDocs[44]),

        new Op(0xA0,"LDY","#v"      ,2,yChanges | nFlag | zFlag                             ,CycleDocs[7] ,InstructionDocs[49]),
        new Op(0xA1,"LDA","(d,x)"   ,2,aChanges | nFlag | zFlag                             ,CycleDocs[22],InstructionDocs[50]),
        new Op(0xA2,"LDX","#v"      ,2,xChanges | nFlag | zFlag                             ,CycleDocs[7] ,InstructionDocs[51]),
        new Op(0xA3,"LAX","(d,x)"   ,2,xChanges | aChanges | nFlag | zFlag                  ,CycleDocs[22],InstructionDocs[52]),
        new Op(0xA4,"LDY","d"       ,2,yChanges | nFlag | zFlag                             ,CycleDocs[12],InstructionDocs[49]),
        new Op(0xA5,"LDA","d"       ,2,aChanges | nFlag | zFlag                             ,CycleDocs[12],InstructionDocs[50]),
        new Op(0xA6,"LDX","d"       ,2,xChanges | nFlag | zFlag                             ,CycleDocs[12],InstructionDocs[51]),
        new Op(0xA7,"LAX","d"       ,2,xChanges | aChanges | nFlag | zFlag                  ,CycleDocs[12],InstructionDocs[52]),
        new Op(0xA8,"TAY","i"       ,1,yChanges | nFlag | zFlag                             ,CycleDocs[6] ,InstructionDocs[53]),
        new Op(0xA9,"LDA","#v"      ,2,aChanges | nFlag | zFlag                             ,CycleDocs[7] ,InstructionDocs[50]),
        new Op(0xAA,"TAX","i"       ,1,xChanges | nFlag | zFlag                             ,CycleDocs[6] ,InstructionDocs[54]),
        new Op(0xAB,"LXA","#v"      ,2,xChanges | aChanges | nFlag | zFlag                  ,CycleDocs[7] ,InstructionDocs[55]),
        new Op(0xAC,"LDY","a"       ,3,yChanges | nFlag | zFlag                             ,CycleDocs[9] ,InstructionDocs[49]),
        new Op(0xAD,"LDA","a"       ,3,aChanges | nFlag | zFlag                             ,CycleDocs[9] ,InstructionDocs[50]),
        new Op(0xAE,"LDX","a"       ,3,xChanges | nFlag | zFlag                             ,CycleDocs[10],InstructionDocs[51]),
        new Op(0xAF,"LAX","a"       ,3,xChanges | aChanges | nFlag | zFlag                  ,CycleDocs[10],InstructionDocs[52]),
        new Op(0xB0,"BCS","r"       ,2,pcChanges                                            ,CycleDocs[21],InstructionDocs[56]),
        new Op(0xB1,"LDA","(d),y"   ,2,aChanges | nFlag | zFlag                             ,CycleDocs[25],InstructionDocs[50]),
        new Op(0xB2,"HLT","i"       ,1,0                                                    ,CycleDocs[29],InstructionDocs[2]),
        new Op(0xB3,"LAX","(d),y"   ,2,xChanges | aChanges | nFlag | zFlag                  ,CycleDocs[25],InstructionDocs[52]),
        new Op(0xB4,"LDY","d,x"     ,2,yChanges | nFlag | zFlag                             ,CycleDocs[15],InstructionDocs[49]),
        new Op(0xB5,"LDA","d,x"     ,2,aChanges | nFlag | zFlag                             ,CycleDocs[15],InstructionDocs[50]),
        new Op(0xB6,"LDX","d,y"     ,2,xChanges | nFlag | zFlag                             ,CycleDocs[15],InstructionDocs[51]),
        new Op(0xB7,"LAX","d,y"     ,2,xChanges | aChanges | nFlag | zFlag                  ,CycleDocs[15],InstructionDocs[52]),
        new Op(0xB8,"CLV","i"       ,1,vFlag                                                ,CycleDocs[6] ,InstructionDocs[57]),
        new Op(0xB9,"LDA","a,y"     ,3,aChanges | nFlag | zFlag                             ,CycleDocs[18],InstructionDocs[50]),
        new Op(0xBA,"TSX","i"       ,1,xChanges | nFlag | zFlag                             ,CycleDocs[6] ,InstructionDocs[58]),
        new Op(0xBB,"LAS","a,y"     ,3,nFlag | zFlag | aChanges | xChanges | stackPChanges  ,CycleDocs[18],InstructionDocs[59]),
        new Op(0xBC,"LDY","a,x"     ,3,yChanges | nFlag | zFlag                             ,CycleDocs[18],InstructionDocs[49]),
        new Op(0xBD,"LDA","a,x"     ,3,aChanges | nFlag | zFlag                             ,CycleDocs[18],InstructionDocs[50]),
        new Op(0xBE,"LDX","a,y"     ,3,xChanges | nFlag | zFlag                             ,CycleDocs[18],InstructionDocs[51]),
        new Op(0xBF,"LAX","a,y"     ,3,xChanges | aChanges | nFlag | zFlag                  ,CycleDocs[18],InstructionDocs[52]),

        new Op(0xC0,"CPY","#v"      ,2,nFlag | zFlag | cFlag                ,CycleDocs[7] ,InstructionDocs[60]),
        new Op(0xC1,"CMP","(d,x)"   ,2,nFlag | zFlag | cFlag                ,CycleDocs[22],InstructionDocs[61]),
        new Op(0xC2,"NOP","#v"      ,2,0                                    ,CycleDocs[7] ,InstructionDocs[3]),
        new Op(0xC3,"DCP","(d,x)"   ,2,memChanges | nFlag | zFlag | cFlag   ,CycleDocs[23],InstructionDocs[62]),
        new Op(0xC4,"CPY","d"       ,2,nFlag | zFlag | cFlag                ,CycleDocs[12],InstructionDocs[60]),
        new Op(0xC5,"CMP","d"       ,2,nFlag | zFlag | cFlag                ,CycleDocs[12],InstructionDocs[61]),
        new Op(0xC6,"DEC","d"       ,2,memChanges | nFlag | zFlag           ,CycleDocs[13],InstructionDocs[63]),
        new Op(0xC7,"DCP","d"       ,2,memChanges | nFlag | zFlag | cFlag   ,CycleDocs[13],InstructionDocs[62]),
        new Op(0xC8,"INY","i"       ,1,yChanges | nFlag | zFlag             ,CycleDocs[6] ,InstructionDocs[64]),
        new Op(0xC9,"CMP","#v"      ,2,nFlag | zFlag | cFlag                ,CycleDocs[7] ,InstructionDocs[61]),
        new Op(0xCA,"DEX","i"       ,1,xChanges | nFlag | zFlag             ,CycleDocs[6] ,InstructionDocs[65]),
        new Op(0xCB,"AXS","#v"      ,2,memChanges  | nFlag | cFlag | zFlag  ,CycleDocs[7] ,InstructionDocs[66]),
        new Op(0xCC,"CPY","a"       ,3,nFlag | zFlag | cFlag                ,CycleDocs[9] ,InstructionDocs[60]),
        new Op(0xCD,"CMP","a"       ,3,nFlag | zFlag | cFlag                ,CycleDocs[9] ,InstructionDocs[61]),
        new Op(0xCE,"DEC","a"       ,3,memChanges | nFlag | zFlag           ,CycleDocs[10],InstructionDocs[63]),
        new Op(0xCF,"DCP","a"       ,3,memChanges | nFlag | zFlag | cFlag   ,CycleDocs[10],InstructionDocs[62]),
        new Op(0xD0,"BNE","r"       ,2,pcChanges                            ,CycleDocs[21],InstructionDocs[67]),
        new Op(0xD1,"CMP","(d),y"   ,2,nFlag | zFlag | cFlag                ,CycleDocs[25],InstructionDocs[61]),
        new Op(0xD2,"HLT","i"       ,1,0                                    ,CycleDocs[29],InstructionDocs[2]),
        new Op(0xD3,"DCP","(d),y"   ,2,memChanges | nFlag | zFlag | cFlag   ,CycleDocs[26],InstructionDocs[62]),
        new Op(0xD4,"NOP","d,x"     ,2,0                                    ,CycleDocs[15],InstructionDocs[3]),
        new Op(0xD5,"CMP","d,x"     ,2,nFlag | zFlag | cFlag                ,CycleDocs[15],InstructionDocs[61]),
        new Op(0xD6,"DEC","d,x"     ,2,memChanges | nFlag | zFlag           ,CycleDocs[16],InstructionDocs[63]),
        new Op(0xD7,"DCP","d,x"     ,2,memChanges | nFlag | zFlag | cFlag   ,CycleDocs[16],InstructionDocs[62]),
        new Op(0xD8,"CLD","i"       ,1,dFlag                                ,CycleDocs[6] ,InstructionDocs[68]),
        new Op(0xD9,"CMP","a,y"     ,3,nFlag | zFlag | cFlag                ,CycleDocs[18],InstructionDocs[61]),
        new Op(0xDA,"NOP","i"       ,1,0                                    ,CycleDocs[6] ,InstructionDocs[3]),
        new Op(0xDB,"DCP","a,x"     ,3,memChanges | nFlag | zFlag | cFlag   ,CycleDocs[19],InstructionDocs[62]),
        new Op(0xDC,"NOP","a,x"     ,3,0                                    ,CycleDocs[18],InstructionDocs[3]),
        new Op(0xDD,"CMP","a,x"     ,3,nFlag | zFlag | cFlag                ,CycleDocs[18],InstructionDocs[61]),
        new Op(0xDE,"DEC","a,x"     ,3,memChanges | nFlag | zFlag           ,CycleDocs[19],InstructionDocs[63]),
        new Op(0xDF,"DCP","a,x"     ,3,memChanges | nFlag | zFlag | cFlag   ,CycleDocs[19],InstructionDocs[62]),

        new Op(0xE0,"CPX","#v"      ,2,nFlag | zFlag | cFlag                                ,CycleDocs[7] ,InstructionDocs[69]),
        new Op(0xE1,"SBC","(d,x)"   ,2,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[22],InstructionDocs[70]),
        new Op(0xE2,"NOP","#v"      ,2,0                                                    ,CycleDocs[7] ,InstructionDocs[3]),
        new Op(0xE3,"ISC","(d,x)"   ,2,aChanges | memChanges | nFlag | zFlag | cFlag | vFlag,CycleDocs[23],InstructionDocs[71]),
        new Op(0xE4,"CPX","d"       ,2,nFlag | zFlag | cFlag                                ,CycleDocs[12],InstructionDocs[69]),
        new Op(0xE5,"SBC","d"       ,2,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[12],InstructionDocs[70]),
        new Op(0xE6,"INC","d"       ,2,memChanges | nFlag | zFlag                           ,CycleDocs[13],InstructionDocs[72]),
        new Op(0xE7,"ISC","d"       ,2,aChanges | memChanges | nFlag | zFlag | cFlag | vFlag,CycleDocs[13],InstructionDocs[71]),
        new Op(0xE8,"INX","i"       ,1,xChanges | nFlag | zFlag                             ,CycleDocs[6] ,InstructionDocs[73]),
        new Op(0xE9,"SBC","#v"      ,2,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[7] ,InstructionDocs[70]),
        new Op(0xEA,"NOP","i"       ,1,0                                                    ,CycleDocs[6] ,InstructionDocs[3]),
        new Op(0xEB,"SBC","#v"      ,2,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[7] ,InstructionDocs[70]),
        new Op(0xEC,"CPX","a"       ,3,nFlag | zFlag | cFlag                                ,CycleDocs[9] ,InstructionDocs[69]),
        new Op(0xED,"SBC","a"       ,3,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[9] ,InstructionDocs[70]),
        new Op(0xEE,"INC","a"       ,3,memChanges | nFlag | zFlag                           ,CycleDocs[10],InstructionDocs[72]),
        new Op(0xEF,"ISC","a"       ,3,aChanges | memChanges | nFlag | zFlag | cFlag | vFlag,CycleDocs[10],InstructionDocs[71]),
        new Op(0xF0,"BEQ","r"       ,2,pcChanges                                            ,CycleDocs[21],InstructionDocs[74]),
        new Op(0xF1,"SBC","(d),y"   ,2,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[25],InstructionDocs[70]),
        new Op(0xF2,"HLT","i"       ,1,0                                                    ,CycleDocs[29],InstructionDocs[2]),
        new Op(0xF3,"ISC","(d),y"   ,2,aChanges | memChanges | nFlag | zFlag | cFlag | vFlag,CycleDocs[26],InstructionDocs[71]),
        new Op(0xF4,"NOP","d,x"     ,2,0                                                    ,CycleDocs[15],InstructionDocs[3]),
        new Op(0xF5,"SBC","d,x"     ,2,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[15],InstructionDocs[70]),
        new Op(0xF6,"INC","d,x"     ,2,memChanges | nFlag | zFlag                           ,CycleDocs[16],InstructionDocs[72]),
        new Op(0xF7,"ISC","d,x"     ,2,aChanges | memChanges | nFlag | zFlag | cFlag | vFlag,CycleDocs[16],InstructionDocs[71]),
        new Op(0xF8,"SED","i"       ,1,dFlag                                                ,CycleDocs[6] ,InstructionDocs[75]),
        new Op(0xF9,"SBC","a,y"     ,3,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[18],InstructionDocs[70]),
        new Op(0xFA,"NOP","i"       ,1,0                                                    ,CycleDocs[6] ,InstructionDocs[3]),
        new Op(0xFB,"ISC","a,x"     ,3,aChanges | memChanges | nFlag | zFlag | cFlag | vFlag,CycleDocs[19],InstructionDocs[71]),
        new Op(0xFC,"NOP","a,x"     ,3,0                                                    ,CycleDocs[18],InstructionDocs[3]),
        new Op(0xFD,"SBC","a,x"     ,3,aChanges | nFlag | zFlag | cFlag | vFlag             ,CycleDocs[18],InstructionDocs[70]),
        new Op(0xFE,"INC","a,x"     ,3,memChanges | nFlag | zFlag                           ,CycleDocs[19],InstructionDocs[72]),
        new Op(0xFF,"ISC","a,x"     ,3,aChanges | memChanges | nFlag | zFlag | cFlag | vFlag,CycleDocs[19],InstructionDocs[71])

        };

}
