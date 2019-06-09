namespace CawkEmulatorV4.Instructions.Arithmatic
{
    internal class Rem
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            var addedValue = value2 % value1;

            valueStack.CallStack.Push(addedValue);
        }

        public static void Emulate_Un(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();

            var addedValue = value2 % value1;


            if (value1 is long && value2 is long)
            {
                var final = (long) ((ulong) value2 % (ulong) value1);
                valueStack.CallStack.Push(final);
            }
            else
            {
                valueStack.CallStack.Push(addedValue);
            }
        }
    }
}