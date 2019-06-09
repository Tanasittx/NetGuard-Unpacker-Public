namespace CawkEmulatorV4.Instructions.Arithmatic
{
    internal class Div
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            var addedValue = value2 / value1;

            valueStack.CallStack.Push(addedValue);
        }

        public static void Emulate_Un(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            dynamic addedValue = (int) ((uint) value2 / value1);

            valueStack.CallStack.Push(addedValue);
        }
    }
}