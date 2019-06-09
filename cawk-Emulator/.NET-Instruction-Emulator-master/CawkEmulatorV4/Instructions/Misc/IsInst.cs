using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using OpCodes = dnlib.DotNet.Emit.OpCodes;

namespace CawkEmulatorV4.Instructions.Misc
{
    class IsInst
    {
        public static void Emulate_Box(ValueStack valueStack, Instruction instruction)
        {
            var size = valueStack.CallStack.Pop();
            var type = (ITypeDefOrRef)instruction.Operand;
            var adff = typeof(int);
            var dynamicMethod = new DynamicMethod("abc", typeof(object), new[] { typeof(object) }, typeof(string).Module, true);
            var ilg = dynamicMethod.GetILGenerator();
            ilg.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
            ilg.Emit(System.Reflection.Emit.OpCodes.Isinst, adff);
            ilg.Emit(System.Reflection.Emit.OpCodes.Ret);
            valueStack.CallStack.Push(dynamicMethod.Invoke(null, new object[] { size }));
        }
    }
}
