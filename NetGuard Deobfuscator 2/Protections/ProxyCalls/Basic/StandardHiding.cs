using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace NetGuard_Deobfuscator_2.Protections.ProxyCalls.Basic
{
    class StandardHiding : Base
    {
        public override void Deobfuscate()
        {
            Cleaner();
        }
        private static void Cleaner()
        {
            foreach (TypeDef types in ModuleDefMD.GetTypes())
            {
                foreach (MethodDef methods in types.Methods)
                {
                    if (!methods.HasBody) continue;
                    for (int i = 0; i < methods.Body.Instructions.Count; i++)
                    {
                        var instruction = methods.Body.Instructions[i];
                        if (instruction.OpCode != OpCodes.Call) continue;
                        if (!(instruction.Operand is MethodDef)) continue;
                        var proxyMethod = instruction.Operand as MethodDef;
                        if (proxyMethod.DeclaringType != types) continue;
                        var paramCount = proxyMethod.Parameters.Count;
                        if (!proxyMethod.HasBody) continue;
                        if (proxyMethod.Body.Instructions.Count != 4 + paramCount) continue;
                        var callResult = proxyMethod.Body.Instructions[proxyMethod.Body.Instructions.Count - 4];
                        methods.Body.Instructions[i] = callResult;
                        methodsToRemove.Add(proxyMethod);
                    }
                }
            }
        }
    }
}
