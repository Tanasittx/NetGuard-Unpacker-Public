using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Branches
{
    internal class Blt
    {
        public static int Emulate(ValueStack valueStack, Instruction ins, IList<Instruction> instructions)
        {
            var value2 = valueStack.CallStack.Pop();
            var value1 = valueStack.CallStack.Pop();
            var branchTo = (Instruction) ins.Operand;
            if (value1 < value2)
                return instructions.IndexOf(branchTo) - 1;
            return -1;
        }
    }
}