using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Branches
{
    internal class BrFalse
    {
        public static int Emulate(ValueStack valueStack, Instruction ins, IList<Instruction> instructions)
        {
            var value1 = valueStack.CallStack.Pop();
            if (value1 is bool)
                value1 = Convert.ToInt32(value1);
            var branchTo = (Instruction) ins.Operand;
            if (value1 == 0||value1 == null)
                return instructions.IndexOf(branchTo) - 1;
            return -1;
        }
    }
}