using System.Collections.Generic;

namespace ConfuserDeobfuscator.Engine.Routines.Ex.x86
{
    public enum X86OpCode
    {
        MOV,
        ADD,
        SUB,
        IMUL,
        DIV,
        NEG,
        NOT,
        XOR,
        POP
    }

    public enum X86Register
    {
        EAX = 0x25,
        ECX = 0x26,
        EDX = 0x27,
        EBX = 0x28,
        ESP = 0x29,
        EBP = 0x2a,
        ESI = 0x2b,
        EDI = 0x2c
    }


    public interface IX86Operand
    {
    }

    public class X86RegisterOperand : IX86Operand
    {
        public X86RegisterOperand(X86Register reg)
        {
            Register = reg;
        }

        public X86Register Register { get; set; }
    }

    public class X86ImmediateOperand : IX86Operand
    {
        public X86ImmediateOperand(int imm)
        {
            Immediate = imm;
        }

        public int Immediate { get; set; }
    }

    public abstract class X86Instruction
    {
        public abstract X86OpCode OpCode { get; }
        public IX86Operand[] Operands { get; set; }
        public abstract void Execute(Dictionary<string, int> registers, Stack<int> localStack);
    }
}