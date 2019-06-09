namespace CawkEmulatorV4.Instructions.Arrays
{
    internal class StelemI1
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value = valueStack.CallStack.Pop();
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();

            array[location] = (byte) value;
        }
    }
}