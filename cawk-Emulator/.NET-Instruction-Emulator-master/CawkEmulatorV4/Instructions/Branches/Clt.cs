namespace CawkEmulatorV4.Instructions.Branches
{
    internal class Clt
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value2 = valueStack.CallStack.Pop();
            var value1 = valueStack.CallStack.Pop();
            valueStack.CallStack.Push(value1 < value2 ? 1 : 0);
        }
    }
}