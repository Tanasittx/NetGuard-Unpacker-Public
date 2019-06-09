using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Branches
{
    internal class BrTrue
    {
        public static int Emulate(ValueStack valueStack, Instruction ins, IList<Instruction> instructions)
        {
            var value1 = valueStack.CallStack.Pop();
            if (value1 == null)
                value1 = 0;
            var branchTo = (Instruction) ins.Operand;
            if (value1 is int &&value1 != 0)
                return instructions.IndexOf(branchTo) - 1;
           
            return -1;
        }
    }
}