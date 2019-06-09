using CawkEmulatorV4;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.FieldFixers
{
    class FieldsInFirstCall : CodeFlowBase
    {
        public static Dictionary<FieldDef, dynamic> allFields;

        public override void Deobfuscate()
        {
     //       WriteModule("Global Integrity");
            Remove();

        }

        public static void Remove()
        {
            TamperFields(ModuleDefMD);
            if (allFields == null) TamperFields2(ModuleDefMD);

            //Console.WriteLine("[!] Fields Deteted!");
            if (allFields == null) return;
            FieldReplacer(ModuleDefMD);
            //       TamperFields2(ModuleDefMD);
            //    FieldReplacer(ModuleDefMD);
        }

        public static void TamperFields2(ModuleDefMD module)
        {
            //Console.WriteLine("[!] Finding Field And Its Value");
            var cctor = module.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;
            foreach (var t1 in cctor.Body.Instructions)
                if (t1.OpCode == OpCodes.Call ||
                    t1.Operand is MethodDef)
                {
                    var methodDef = (MethodDef)t1.Operand;
                    if (!methodDef.HasBody) continue;
                    if (!methodDef.Body.Instructions[0].IsLdcI4() && methodDef.Body.Instructions[1].OpCode != OpCodes.Dup) continue;
                    var intEnd = FindEnd(methodDef);
                    var insEmu = new Emulation(methodDef);
                    insEmu.Emulate2(methodDef.Body.Instructions, 0, intEnd);
                    allFields = insEmu.ValueStack.Fields;
                    return;
                }
        }

        private static int FindEnd(MethodDef methods)
        {
            for (int i = 0; i < methods.Body.Instructions.Count; i++)
            {
                if (methods.Body.Instructions[i].OpCode == OpCodes.Call)
                    return i;
            }

            return -1;
        }
        public static void TamperFields(ModuleDefMD module)
        {
   //         Console.WriteLine("[!] Finding Field And Its Value");
            var cctor = module.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;
            foreach (var t1 in cctor.Body.Instructions)
                if (t1.OpCode == OpCodes.Call ||
                    t1.Operand is MethodDef)
                {
                    var methodDef = (MethodDef)t1.Operand;
                    if (!methodDef.HasBody) continue;
                    if (methodDef.Body.Instructions[0].OpCode != OpCodes.Call) continue;
                    var methodDef2 = methodDef.Body.Instructions[0].Operand as MethodDef;
                    if (methodDef2 == null) continue;
                    if (!methodDef2.HasBody) continue;
                    if (!methodDef2.Body.Instructions.Where((t, z) => t.IsStloc() && methodDef2.Body.Instructions[z + 1].IsLdcI4())
                        .Any()) continue;
                    if (ContainsP0(methodDef2)) continue;
                    var insEmu = new Emulation(methodDef2);
                    insEmu.Emulate();
                    allFields = insEmu.ValueStack.Fields;
                    return;
                }
        }

        public static bool ContainsP0(MethodDef methods)
        {
            foreach (var bodyInstruction in methods.Body.Instructions)
            {
                if (bodyInstruction.OpCode == OpCodes.Ldstr && bodyInstruction.Operand.ToString() == "P0")
                    return true;
            }

            return false;
        }
        public static void FieldReplacer(ModuleDefMD module)
        {
            foreach (var typeDef in module.GetTypes())
                foreach (var methods in typeDef.Methods)
                {
                    if (!methods.HasBody) continue;
                    foreach (Instruction t in methods.Body.Instructions)
                    {
                        if (t.OpCode != OpCodes.Ldsfld ||
                            !(t.Operand is FieldDef)) continue;
                        var fie = (FieldDef)t.Operand;
                        if (!allFields.ContainsKey(fie)) continue;
                        t.OpCode = OpCodes.Ldc_I4;
                        t.Operand = (int)allFields[fie];
                    }
                }
        }
    }
}
