using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CleanUp
{
    class IntegrityCleaner:Base
    {
        public override void Deobfuscate()
        {
            //Console.WriteLine("[!] Cleaning method checker");
            Cleaner();
        //    return 1;
        }

        public static void Cleaner()
        {
            var cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;

            foreach (var methods in cctor.DeclaringType.Methods)
            {
                if (!methods.HasBody) continue;
                if (!methods.Body.Instructions.Any(t => t.OpCode == OpCodes.Callvirt && t.Operand != null && t.Operand.ToString()
                                                            .Contains("Cryptography.HashAlgorithm::ComputeHash"))) continue;
                if (!methods.Body.Instructions.Any(t => t.OpCode == OpCodes.Callvirt && t.Operand != null && t.Operand.ToString()
                                                            .Contains("ResolveString"))) continue;
                methods.Body.Instructions[0].OpCode = OpCodes.Ret;
                return;
            }
        }
    }
}
