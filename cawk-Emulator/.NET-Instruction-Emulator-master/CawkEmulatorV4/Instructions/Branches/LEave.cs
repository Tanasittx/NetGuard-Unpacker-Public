using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Branches
{
    class LEave
    {
        public static int Emulate(ValueStack valueStack, Instruction instruction, IList<Instruction> allinstructions)
        {
            var branchTo = (Instruction)instruction.Operand;
            valueStack.CallStack.Clear();
            return allinstructions.IndexOf(branchTo) - 1;
        }
    }
}
