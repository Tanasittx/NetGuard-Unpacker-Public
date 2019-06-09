namespace CawkEmulatorV4.Instructions.Arrays
{
    internal class Stelem_I4
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value = valueStack.CallStack.Pop();
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();
            if (array is uint[])
                array[location] = (uint) value;
            else
                array[location] = (int) value;
        }
    }
}