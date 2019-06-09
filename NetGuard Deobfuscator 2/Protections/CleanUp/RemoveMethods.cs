using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CleanUp
{
    class RemoveMethods:Base
    {
        public override void Deobfuscate()
        {
            methodsToRemove = methodsToRemove.Distinct().ToList();
            foreach(IMethod method in methodsToRemove)
            {
                if (method == null) continue;
                var resolved = method.ResolveMethodDef();
                if (resolved.DeclaringType == null) continue;
                resolved.DeclaringType.Methods.Remove(resolved);
            }
        }
    }
}
