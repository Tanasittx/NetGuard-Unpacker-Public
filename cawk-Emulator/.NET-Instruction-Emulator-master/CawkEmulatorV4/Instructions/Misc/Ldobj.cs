using System.Runtime.InteropServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class Ldobj
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction)
        {
            var type = (ITypeDefOrRef) instruction.Operand;
            var typeResolve = typeof(string).Module.GetType(type.ReflectionFullName);
            var address = valueStack.CallStack.Pop();
            valueStack.CallStack.Push(Marshal.PtrToStructure(address, typeResolve));
        }
    }
}