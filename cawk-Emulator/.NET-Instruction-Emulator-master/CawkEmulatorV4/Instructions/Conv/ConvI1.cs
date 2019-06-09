namespace CawkEmulatorV4.Instructions.Conv
{
    internal class ConvI1
    {
        public static void Emulation(ValueStack valueStack)
        {
            dynamic value = (sbyte) valueStack.CallStack.Pop();
            valueStack.CallStack.Push((int) value);
        }

        public static void UEmulation(ValueStack valueStack)
        {
            dynamic value = (byte) valueStack.CallStack.Pop();
            valueStack.CallStack.Push((int) value);
        }
    }
}