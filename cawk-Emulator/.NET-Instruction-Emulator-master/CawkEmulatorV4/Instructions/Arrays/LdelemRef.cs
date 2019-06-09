namespace CawkEmulatorV4.Instructions.Arrays
{
    internal class LdelemRef
    {
        public static void Emulate(ValueStack valueStack)
        {
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();
            valueStack.CallStack.Push(array[location]);
        }
    }
}