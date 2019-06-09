using System.Collections.Generic;
using EasyPredicateKiller;
using SharpDisasm;

namespace ConfuserDeobfuscator.Engine.Routines.Ex.x86.Instructions
{
    internal class X86DIV : X86Instruction
    {
        public X86DIV(Instruction rawInstruction)
        {
            Operands = new IX86Operand[2];
            Operands[0] = rawInstruction.Operands[0].GetOperand();
            Operands[1] = rawInstruction.Operands[1].GetOperand();
        }

        public override X86OpCode OpCode => X86OpCode.DIV;

        public override void Execute(Dictionary<string, int> registers, Stack<int> localStack)
        {
        }
    }
}