namespace CawkEmulatorV4.Instructions.Conv
{
    internal class ConvI2
    {
        public static void Emulation(ValueStack valueStack)
        {
            dynamic value = (short) valueStack.CallStack.Pop();
            valueStack.CallStack.Push((int) value);
        }

        public static void UEmulation(ValueStack valueStack)
        {
            dynamic value = (ushort) valueStack.CallStack.Pop();
            valueStack.CallStack.Push((int) value);
        }
    }
}