using System.Reflection.Emit;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class NewArr
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction)
        {
            var size = valueStack.CallStack.Pop();
            var type = (ITypeDefOrRef) instruction.Operand;
            var adff = typeof(string).Module.GetType(type.ReflectionFullName);
            var dynamicMethod = new DynamicMethod("abc", adff.MakeArrayType(), new[] {typeof(int)},
                typeof(string).Module, true);
            var ilg = dynamicMethod.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Newarr, adff);
            ilg.Emit(OpCodes.Ret);
            valueStack.CallStack.Push(dynamicMethod.Invoke(null, new object[] {size}));
        }
    }
}