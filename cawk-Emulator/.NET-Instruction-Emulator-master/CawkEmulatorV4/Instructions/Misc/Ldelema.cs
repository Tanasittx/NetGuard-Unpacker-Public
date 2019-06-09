using System;
using System.Runtime.InteropServices;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class AutoPinner : IDisposable
    {
        public GCHandle _pinnedArray;

        public AutoPinner(object obj)
        {
            _pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        public void Dispose()
        {
            _pinnedArray.Free();
        }

        public static implicit operator IntPtr(AutoPinner ap)
        {
            return ap._pinnedArray.AddrOfPinnedObject();
        }
    }

    internal class Ldelema
    {
        public static void Emulate(ValueStack valueStack)
        {
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();


            var pointer = Marshal.UnsafeAddrOfPinnedArrayElement(array, location);
            //Marshal.WriteInt32(pointer,5);
            valueStack.CallStack.Push(pointer);
        }
    }
}