using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    class DecimalCompare : MutationsBase
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
                    if (method.Body.Instructions[i].OpCode != OpCodes.Call) continue;
                    if (!method.Body.Instructions[i].Operand.ToString().Contains("::Compare")) continue;
                    if (!method.Body.Instructions[i - 2].IsLdcI4()) continue;
                    if (!method.Body.Instructions[i -4].IsLdcI4()) continue;
                    if (method.Body.Instructions[i - 3].OpCode != OpCodes.Newobj) continue;
                    if (method.Body.Instructions[i - 1].OpCode != OpCodes.Newobj) continue;
                    var val1 = method.Body.Instructions[i - 4].GetLdcI4Value();
                    var val2 = method.Body.Instructions[i - 2].GetLdcI4Value();
                    var newValue = decimal.Compare(val1, val2);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 3].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 4].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i - 4].Operand = newValue;
                    modified = true;
                }
            }

            return modified;
        }
    }
}
