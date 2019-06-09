using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class Ldtoken
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction)
        {
            var fie = instruction.Operand;


            if (fie is FieldDef)
            {
                var fie2 = (FieldDef) fie;
                valueStack.CallStack.Push(fie2.MDToken.ToInt32());
            }
            else
            {
                var fie2 = (IMDTokenProvider) fie;
                valueStack.CallStack.Push(fie2.MDToken.ToInt32());
            }
        }
    }
}