using System.Collections.Generic;
using EasyPredicateKiller;
using SharpDisasm;

namespace ConfuserDeobfuscator.Engine.Routines.Ex.x86.Instructions
{
    internal class X86XOR : X86Instruction
    {
        public X86XOR(Instruction rawInstruction)
        {
            Operands = new IX86Operand[2];
            Operands[0] = rawInstruction.Operands[0].GetOperand();
            Operands[1] = rawInstruction.Operands[1].GetOperand();
        }

        public override X86OpCode OpCode => X86OpCode.XOR;

        public override void Execute(Dictionary<string, int> registers, Stack<int> localStack)
        {
            if (Operands[1] is X86ImmediateOperand)
                registers[((X86RegisterOperand) Operands[0]).Register.ToString()] ^=
                    ((X86ImmediateOperand) Operands[1]).Immediate;
            else
                registers[((X86RegisterOperand) Operands[0]).Register.ToString()] ^=
                    registers[((X86RegisterOperand) Operands[1]).Register.ToString()];
        }
    }
}