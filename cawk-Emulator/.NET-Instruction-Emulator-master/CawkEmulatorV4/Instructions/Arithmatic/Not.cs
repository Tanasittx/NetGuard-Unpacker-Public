namespace CawkEmulatorV4.Instructions.Arithmatic
{
    internal class Not
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();

            var addedValue = ~ value1;

            valueStack.CallStack.Push(addedValue);
        }
    }
}