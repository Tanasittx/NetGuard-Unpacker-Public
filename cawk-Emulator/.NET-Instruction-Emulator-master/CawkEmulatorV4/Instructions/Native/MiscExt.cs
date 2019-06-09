using ConfuserDeobfuscator.Engine.Routines.Ex.x86;
using SharpDisasm;
using SharpDisasm.Udis86;

namespace EasyPredicateKiller
{
    public static class MiscExt
    {
        public static IX86Operand GetOperand(this Operand argument)
        {
            if (argument.Type == ud_type.UD_OP_IMM)
                return
                    new X86ImmediateOperand((int) argument.Value);

            return new X86RegisterOperand((X86Register) argument.Base);
        }
    }
}