using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Branches
{
    internal class Switch
    {
        public static int Emulate(ValueStack valueStack, Instruction ins, IList<Instruction> instructions)
        {
            var value1 = valueStack.CallStack.Pop();
            var branchTo = (Instruction[]) ins.Operand;
            //	Console.WriteLine(value1);
            return instructions.IndexOf(branchTo[value1]) - 1;
        }
    }
}