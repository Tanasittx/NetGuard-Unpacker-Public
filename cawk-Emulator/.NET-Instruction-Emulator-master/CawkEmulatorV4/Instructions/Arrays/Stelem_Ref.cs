using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CawkEmulatorV4.Instructions.Arrays
{
    class Stelem_Ref
    {
        public static void Emulate(ValueStack valueStack,Instruction instr)
        {
            var value = valueStack.CallStack.Pop();
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();
            if (array is uint[])
                array[location] = (uint)value;
            else
                array[location] = value;
        }
    }
}
