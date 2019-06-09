using System;

namespace CawkEmulatorV4.Instructions.Pointer
{
    internal class LdindI2
    {
        public static unsafe void Emulate(ValueStack valueStack)
        {
            var address = valueStack.CallStack.Pop();
            var ptr = ((IntPtr) address).ToPointer();
            var value = *(short*) ptr;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
            valueStack.CallStack.Push((int) value);
        }

        public static unsafe void UEmulate(ValueStack valueStack)
        {
            var address = valueStack.CallStack.Pop();
            var ptr = ((IntPtr) address).ToPointer();
            var value = *(ushort*) ptr;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
            valueStack.CallStack.Push((int) value);
        }
    }
}