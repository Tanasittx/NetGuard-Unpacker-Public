using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    class EmptyTypeCleaner:MutationsBase
    {
        public override bool Deobfuscate()
        {
            return Clean();
        }
        private static bool Clean()
        {
            bool modified = false;
            foreach(MethodDef method in methods)
            {
                for(int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (method.Body.Instructions[i].OpCode != OpCodes.Ldsfld) continue;
                    if (!method.Body.Instructions[i].Operand.ToString().Contains("::EmptyTypes")) continue;
                    if (method.Body.Instructions[i+1].OpCode != OpCodes.Ldlen) continue;
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4_0;
                    method.Body.Instructions[i + 1].OpCode = OpCodes.Nop;
                    modified = true;
                }
            }

            return modified;
        }
    }
}
