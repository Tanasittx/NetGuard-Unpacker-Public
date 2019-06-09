using System.Reflection.Emit;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class CastClass
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction)
        {
            var size = valueStack.CallStack.Pop();
            var type = (ITypeDefOrRef) instruction.Operand;

            var adff = typeof(string).Module.GetType(type.ReflectionFullName);
            var dynamicMethod =
                new DynamicMethod("CastClass", adff, new[] {typeof(object)}, typeof(string).Module, true);
            var ilg = dynamicMethod.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Castclass, adff);
            ilg.Emit(OpCodes.Ret);
            valueStack.CallStack.Push(dynamicMethod.Invoke(null, new[] {(object) size}));
        }
    }
}