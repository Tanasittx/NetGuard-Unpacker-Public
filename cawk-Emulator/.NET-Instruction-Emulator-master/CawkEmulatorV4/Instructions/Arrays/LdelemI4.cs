namespace CawkEmulatorV4.Instructions.Arrays
{
    internal class LdelemI4
    {
        public static void Emulate(ValueStack valueStack)
        {
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();
            valueStack.CallStack.Push(array[location]);
        }

        public static void UEmulate(ValueStack valueStack)
        {
            var location = valueStack.CallStack.Pop();
            var array = valueStack.CallStack.Pop();
            var abc = array[location];
            var tee = (uint) abc;
            var tttt = (int) tee;
            valueStack.CallStack.Push((int)(uint) array[location]);
        }
    }
}