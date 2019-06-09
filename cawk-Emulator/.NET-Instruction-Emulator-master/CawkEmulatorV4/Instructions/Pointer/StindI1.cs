using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CawkEmulatorV4.Instructions.Pointer
{
    class StindI1
    {
        public static unsafe void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var address = valueStack.CallStack.Pop();

            var ptr = ((IntPtr)address).ToPointer();
            *(sbyte*)ptr = (sbyte)value1;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
        }
    }
}
