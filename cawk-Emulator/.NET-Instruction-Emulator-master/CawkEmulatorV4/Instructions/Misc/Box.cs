using System;
using System.Reflection.Emit;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class Box
    {
        public static void Emulate_Box(ValueStack valueStack, Instruction instruction)
        {
            var size = valueStack.CallStack.Pop();
            var type = (ITypeDefOrRef) instruction.Operand;
            var adff = typeof(string).Module.GetType(type.ReflectionFullName);
            var dynamicMethod = new DynamicMethod("abc", adff, new[] {typeof(int)}, typeof(string).Module, true);
            var ilg = dynamicMethod.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Box, adff);
            ilg.Emit(OpCodes.Ret);
            valueStack.CallStack.Push(dynamicMethod.Invoke(null, new object[] {size}));
        }

        public static void Emulate_UnBox(ValueStack valueStack)
        {
        }

        public static void Emulate_UnBox_Any(ValueStack valueStack, Instruction instruction)
        {
            var size = valueStack.CallStack.Pop();
            var type = (ITypeDefOrRef) instruction.Operand;
            var adff = typeof(string).Module.GetType(type.ReflectionFullName);
            if (adff == null)
            {
                valueStack.CallStack.Push(size);
            }
            else
            {
              valueStack.CallStack.Push(Convert.ChangeType(size,adff));
            }
        }
    }
}