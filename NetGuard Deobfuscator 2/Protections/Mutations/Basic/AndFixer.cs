using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    class AndFixer : MutationsBase
    {
        public override bool Deobfuscate()
        {
            return Clean();
        }
        private static bool Clean()
        {
            bool modified = false;
            foreach (MethodDef method in methods)
            {
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    var instruction = method.Body.Instructions[i];
                    if (!instruction.IsLdloc()) continue;
                    if (!method.Body.Instructions[i + 1].IsLdloc()) continue;
                    if (method.Body.Instructions[i + 2].OpCode != OpCodes.Neg) continue;
                    if (method.Body.Instructions[i + 3].OpCode != OpCodes.And) continue;
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i + 1].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = 0;
                    method.Body.Instructions[i + 1].Operand = 0;
                    modified = true;
                }
            }
            return modified;
        }
    }
}
