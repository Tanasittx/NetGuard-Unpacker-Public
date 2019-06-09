using System;

namespace CawkEmulatorV4.Instructions.Pointer
{
    internal class LdindI4
    {
        public static unsafe void Emulate(ValueStack valueStack)
        {
            var address = valueStack.CallStack.Pop();
            var ptr = ((IntPtr) address).ToPointer();
            var value = *(int*) ptr;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
            valueStack.CallStack.Push(value);
        }

        public static unsafe void UEmulate(ValueStack valueStack)
        {
            var address = valueStack.CallStack.Pop();
            var ptr = ((IntPtr) address).ToPointer();
            var value = *(uint*) ptr;
            //		   Console.WriteLine("GET "+address.ToString("X8") + "	Value = " + value.ToString("X8"));
            valueStack.CallStack.Push((int)(uint) value);
        }
    }
}