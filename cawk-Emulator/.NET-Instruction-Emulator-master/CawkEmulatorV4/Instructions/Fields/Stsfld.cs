using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Fields
{
    internal class Stsfld
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction)
        {
            valueStack.Fields[instruction.Operand as FieldDef] = valueStack.CallStack.Pop();
        }
    }
}