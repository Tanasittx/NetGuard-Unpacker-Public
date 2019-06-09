using System;

namespace CawkEmulatorV4.Instructions.Arrays
{
    internal class LdelemI1
    {
        public static void Emulate(ValueStack valueStack)
        {
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();
            valueStack.CallStack.Push((int) (sbyte) array[location]);
        }

        public static void UEmulate(ValueStack valueStack)
        {

            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();
            var valueinByte = Convert.ToByte(array[location]);
            var valueinInt = Convert.ToInt32(valueinByte);
            valueStack.CallStack.Push(valueinInt);
        }
    }
}