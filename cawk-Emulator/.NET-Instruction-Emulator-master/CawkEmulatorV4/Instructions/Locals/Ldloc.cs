using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Locals
{
    internal class Ldloc
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction, MethodDef method)
        {
            var loc = instruction.GetLocal(method.Body.Variables);
            valueStack.CallStack.Push(valueStack.Locals[loc.Index]);
        }

        public static void EmulateLdloca(ValueStack valueStack, Instruction instruction, MethodDef method)
        {
            var loc = (Local) instruction.Operand;
            valueStack.CallStack.Push(valueStack.Locals[loc.Index]);
        }
    }
}