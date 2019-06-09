using System.Collections.Generic;
using EasyPredicateKiller;
using SharpDisasm;

namespace ConfuserDeobfuscator.Engine.Routines.Ex.x86.Instructions
{
    internal class X86MOV : X86Instruction
    {
        public X86MOV(Instruction rawInstruction)
        {
            Operands = new IX86Operand[2];
            Operands[0] = rawInstruction.Operands[0].GetOperand();
            Operands[1] = rawInstruction.Operands[1].GetOperand();
        }

        public override X86OpCode OpCode => X86OpCode.MOV;

        public override void Execute(Dictionary<string, int> registers, Stack<int> localStack)
        {
            if (Operands[1] is X86ImmediateOperand)
                registers[((X86RegisterOperand) Operands[0]).Register.ToString()] =
                    (Operands[1] as X86ImmediateOperand).Immediate;
            else
                registers[((X86RegisterOperand) Operands[0]).Register.ToString()] =
                    registers[(Operands[1] as X86RegisterOperand).Register.ToString()];
        }
    }
}