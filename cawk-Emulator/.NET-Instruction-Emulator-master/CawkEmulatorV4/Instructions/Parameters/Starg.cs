using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Parameter = dnlib.DotNet.Parameter;

namespace CawkEmulatorV4.Instructions.Parameters
{
    internal class Starg
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction, MethodDef methods)
        {
            var val = valueStack.CallStack.Pop();
            var type = ((Parameter)instruction.Operand).Type;
    
            var loc = instruction.GetParameter(methods.Parameters);
            valueStack.Parameters[loc] = val;
        }
    }
}