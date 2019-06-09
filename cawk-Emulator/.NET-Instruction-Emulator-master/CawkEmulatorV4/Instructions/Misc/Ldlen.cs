namespace CawkEmulatorV4.Instructions.Misc
{
    internal class Ldlen
    {
        public static void Emulate(ValueStack valueStack)
        {
            var val = valueStack.CallStack.Pop();
            if (val == null)
                valueStack.CallStack.Push(0);
            else
                valueStack.CallStack.Push(val.Length);
        }
    }
}