namespace CawkEmulatorV4.Instructions.Arithmatic
{
    internal class Shr
    {
        public static void Emulate(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            var addedValue = value2 >> value1;

            valueStack.CallStack.Push(addedValue);
        }


        public static void Emulate_Un(ValueStack valueStack)
        {
            var value1 = valueStack.CallStack.Pop();
            var value2 = valueStack.CallStack.Pop();
            var shifted = (int) ((uint) value2 >> value1);
            //			DynamicMethod dynamicMethod = new DynamicMethod("abc", value2.GetType(), new Type[] { value2.GetType(),value1.GetType() }, typeof(String).Module, true);
            //			ILGenerator ilg = dynamicMethod.GetILGenerator();
            //			ilg.Emit(OpCodes.Ldarg_0);
            //			ilg.Emit(OpCodes.Ldarg_1);
            //			ilg.Emit(OpCodes.Shr_Un);
            //			ilg.Emit(OpCodes.Ret);
            //			var tester = dynamicMethod.Invoke(null, new object[] {value2, value1});
            valueStack.CallStack.Push(shifted);
        }
    }
}