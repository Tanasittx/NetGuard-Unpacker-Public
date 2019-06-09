using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CawkEmulatorV4.Instructions.Pointer
{
    class LdindI1
    {
        public static unsafe void Emulate(ValueStack valueStack)
        {
            var address = valueStack.CallStack.Pop();
            var ptr = ((IntPtr)address).ToPointer();
            var value = *(sbyte*)ptr;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
            valueStack.CallStack.Push((int)value);
        }

        public static unsafe void UEmulate(ValueStack valueStack)
        {
            var address = valueStack.CallStack.Pop();
            var ptr = ((IntPtr)address).ToPointer();
            var value = *(byte*)ptr;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
            valueStack.CallStack.Push((int)value);
        }
    }
}
