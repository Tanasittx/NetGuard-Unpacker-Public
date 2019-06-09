namespace CawkEmulatorV4.Instructions.Arithmatic
{
    internal class Shl
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            var addedValue = value2 << value1;

            valueStack.CallStack.Push(addedValue);
        }
    }
}