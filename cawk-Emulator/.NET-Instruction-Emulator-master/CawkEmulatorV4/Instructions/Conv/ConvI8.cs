namespace CawkEmulatorV4.Instructions.Conv
{
    internal class ConvI8
    {
        public static void Emulation(ValueStack valueStack)
        {
            var value = valueStack.CallStack.Pop();
            valueStack.CallStack.Push((long) value);
        }

        public static void UEmulation(ValueStack valueStack)
        {
            var value = valueStack.CallStack.Pop();
            var x = unchecked((long) (ulong) value);

            valueStack.CallStack.Push(x);
        }
    }
}