using System;

namespace CawkEmulatorV4.Instructions.Arithmatic
{
    internal class Add
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            var addedValue = value2 + value1;

            valueStack.CallStack.Push(addedValue);
        }

        public static void Emulate_Ovf(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            try
            {
                var addedValue = checked(value2 + value1);

                valueStack.CallStack.Push(addedValue);
            }
            catch (OverflowException)
            {
                valueStack.CallStack.Push(-1);
            }
        }
    }
}