using System;

namespace CawkEmulatorV4.Instructions.Pointer
{
    internal class StindI4
    {
        public static unsafe void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var address = valueStack.CallStack.Pop();

            var ptr = ((IntPtr) address).ToPointer();
            *(int*) ptr = value1;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
        }
    }
}