using System.Collections.Generic;
using EasyPredicateKiller;
using SharpDisasm;

namespace ConfuserDeobfuscator.Engine.Routines.Ex.x86.Instructions
{
    internal class X86NOT : X86Instruction
    {
        public X86NOT(Instruction rawInstruction)
        {
            Operands = new IX86Operand[1];
            Operands[0] = rawInstruction.Operands[0].GetOperand();
        }

        public override X86OpCode OpCode => X86OpCode.NOT;

        public override void Execute(Dictionary<string, int> registers, Stack<int> localStack)
        {
            registers[((X86RegisterOperand) Operands[0]).Register.ToString()] =
                ~registers[((X86RegisterOperand) Operands[0]).Register.ToString()];
        }
    }
}