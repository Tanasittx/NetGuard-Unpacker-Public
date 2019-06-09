using System.Collections.Generic;
using EasyPredicateKiller;
using SharpDisasm;

namespace ConfuserDeobfuscator.Engine.Routines.Ex.x86.Instructions
{
    internal class X86POP : X86Instruction
    {
        public X86POP(Instruction rawInstruction)
        {
            Operands = new IX86Operand[1];
            Operands[0] = rawInstruction.Operands[0].GetOperand();
        }

        public override X86OpCode OpCode => X86OpCode.POP;

        public override void Execute(Dictionary<string, int> registers, Stack<int> localStack)
        {
            // Pretend to pop stack
            if (localStack.Count < 1)
                return;

            registers[((X86RegisterOperand) Operands[0]).Register.ToString()] = localStack.Pop();
        }
    }
}