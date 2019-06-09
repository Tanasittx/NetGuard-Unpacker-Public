using System.IO;

namespace CawkEmulatorV4.Instructions.Arithmatic
{
    internal class Xor
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            var addedValue = value2 ^ value1;
 //           File.AppendAllText("linesd.txt",addedValue.ToString("X2")+"      "+addedValue.GetType()+"\n");
            valueStack.CallStack.Push(addedValue);
        }
    }
}