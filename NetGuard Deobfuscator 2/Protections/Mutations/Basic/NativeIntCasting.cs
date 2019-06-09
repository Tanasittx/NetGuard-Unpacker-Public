using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    class NativeIntCasting : MutationsBase
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
                    if (!method.Body.Instructions[i].Operand.ToString().Contains("System.IntPtr::op_Explicit(System.Int32)")) continue;
                    if (method.Body.Instructions[i + 1].OpCode != OpCodes.Call) continue;
                    if (!method.Body.Instructions[i+1].Operand.ToString().Contains("System.IntPtr::op_Explicit(System.IntPtr)")) continue;
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i + 1].OpCode = OpCodes.Nop;
                    modified = true;
                }
            }

            return modified;
        }
    }
}
