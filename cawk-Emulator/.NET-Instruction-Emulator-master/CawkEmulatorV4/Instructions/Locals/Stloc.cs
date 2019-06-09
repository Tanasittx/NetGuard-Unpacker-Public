using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Locals
{
    internal class Stloc
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction, MethodDef method)
        {
            var value = valueStack.CallStack.Pop();
            var loc = instruction.GetLocal(method.Body.Variables);
            valueStack.Locals[loc.Index] = value;
        }
    }
}