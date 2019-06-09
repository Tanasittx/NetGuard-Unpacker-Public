using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Branches
{
    internal class Br
    {
        public static int Emulate(ValueStack valueStack, Instruction instruction, IList<Instruction> allinstructions)
        {
            var branchTo = (Instruction)instruction.Operand;
            return allinstructions.IndexOf(branchTo) - 1;
        }
    }
}