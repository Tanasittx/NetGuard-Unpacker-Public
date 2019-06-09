using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Locals
{
    class NullLocals:MutationsBase
    {
        public override bool Deobfuscate()
        {
            return Clean();
        }
        private static bool Clean()
        {
            var modified = false;
            foreach(MethodDef method in methods)
            {
                method.Body.SimplifyMacros(method.Parameters);
                foreach(Local local in method.Body.Variables)
                {
                    if (local.Type != ModuleDefMD.CorLibTypes.Int32) continue;
                    if (method.Body.Instructions.Any(i => i.OpCode == OpCodes.Stloc && (i.Operand as Local).Index == local.Index)) continue;
                    for(int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].OpCode != OpCodes.Ldloc) continue;
                        if (method.Body.Instructions[i].Operand is null) continue;
                        if ((method.Body.Instructions[i].Operand as Local).Index != local.Index) continue;
                        method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4_0;
                        modified = true;
                    }
                }
            }

            return modified;
        }
    }
}
