using System;

namespace CawkEmulatorV4.Instructions.Conv
{
    internal class ConvI
    {
        public static void Emulation(ValueStack valueStack)
        {
            return;
            dynamic value = (IntPtr) valueStack.CallStack.Pop();
            valueStack.CallStack.Push((IntPtr) value);
        }

        public static void UEmulation(ValueStack valueStack)
        {
            return;
            dynamic value = (UIntPtr) valueStack.CallStack.Pop();
            valueStack.CallStack.Push(value);
        }
    }
}