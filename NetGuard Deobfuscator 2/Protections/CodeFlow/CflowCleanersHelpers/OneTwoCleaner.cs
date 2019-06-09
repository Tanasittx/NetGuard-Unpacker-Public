using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.CflowCleanersHelpers
{
    class OneTwoCleaner:CodeFlowBase
    {
        public override void Deobfuscate()
        {
       //     WriteModule(nameof(OneTwoCleaner));
     //       Console.WriteLine("[!] Cleaning 1234's For easier cflow");
            OnetwoFixer();

        }

        public static void OnetwoFixer()
        {
            foreach (var types in ModuleDefMD.GetTypes())
                foreach (var methods in types.Methods)
                {
                    if (!methods.HasBody) continue;
                    for (var i = 0; i < methods.Body.Instructions.Count; i++)
                    {
                        if (!methods.Body.Instructions[i].IsLdcI4()) continue;
                        if (!methods.Body.Instructions[i + 1].IsStloc() ||
                            methods.Body.Instructions[i].GetLdcI4Value() != 1234) continue;
                        methods.Body.Instructions[i].OpCode = OpCodes.Nop;
                        methods.Body.Instructions[i + 1].OpCode = OpCodes.Nop;
                    }
                }
        }
    }
}
