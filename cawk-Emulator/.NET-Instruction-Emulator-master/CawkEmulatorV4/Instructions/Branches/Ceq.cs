namespace CawkEmulatorV4.Instructions.Branches
{
    internal class Ceq
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value2 = valueStack.CallStack.Pop();
            var value1 = valueStack.CallStack.Pop();
            valueStack.CallStack.Push(value2 == value1 ? 1 : 0);
        }
    }
}