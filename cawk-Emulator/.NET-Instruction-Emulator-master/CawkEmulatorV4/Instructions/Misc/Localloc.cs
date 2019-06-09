using System;
using System.Runtime.InteropServices;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class Localloc
    {
        public static void Emulate(ValueStack valueStack)
        {
            var amount = valueStack.CallStack.Pop();
            IntPtr results = Marshal.AllocHGlobal(amount * IntPtr.Size);

            valueStack.CallStack.Push(results);
        }
    }
}