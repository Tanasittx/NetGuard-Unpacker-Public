using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Parameters
{
    internal class Ldarg
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction, MethodDef methods)
        {
            var loc = instruction.GetParameter(methods.Parameters);
            valueStack.CallStack.Push(valueStack.Parameters[loc]);
        }
    }
}