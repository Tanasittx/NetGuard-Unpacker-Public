namespace CawkEmulatorV4.Instructions.Conv
{
    internal class ConvI4
    {
        public static void Emulation(ValueStack valueStack)
        {
            var value = valueStack.CallStack.Pop();
            valueStack.CallStack.Push((int) value);
        }

        public static void UEmulation(ValueStack valueStack)
        {
            dynamic value = (uint) valueStack.CallStack.Pop();
            valueStack.CallStack.Push((int) value);
        }
    }
}