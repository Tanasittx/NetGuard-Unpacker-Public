using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CleanUp
{
    class Cctor:Base
    {
        public static List<Instruction> C = new List<Instruction>();
        public static MethodDef GetMethod;

        public static byte[] byte_0 { get; set; }
        public override void Deobfuscate()
        {
         //   Console.WriteLine("[!] Cleaning Cctor");
            var getRescMethod = firstStep(ModuleDefMD);
            var cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;
            if (getRescMethod != null)
            {

                cctor.Body.Instructions[0].OpCode = OpCodes.Call;
                cctor.Body.Instructions[0].Operand = getRescMethod;
                cctor.Body.Instructions[1].OpCode = OpCodes.Ret;

            }
            else
            {
                cctor.Body.Instructions[0].OpCode = OpCodes.Ret;
            }


           
        }
        public static bool SortList()
        {
            var dgrfs = "System.Reflection.Assembly System.Reflection.Assembly::Load(";
            return C.Any(t => t.Operand.ToString().Contains(dgrfs));
        }
        public static bool callGetter(ModuleDefMD module, MethodDef method)
        {
            foreach (Instruction t in method.Body.Instructions)
            {
                if (t.OpCode == OpCodes.Call)
                {
                    C.Add(t);
                }
            }
            return SortList();
        }
        public static MethodDef firstStep(ModuleDefMD module)
        {
            foreach (TypeDef type in module.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    if (!method.IsConstructor) continue;
                    if (!method.FullName.ToLower().Contains("module")) continue;
                    foreach (Instruction t in method.Body.Instructions)
                    {
                        if (t.OpCode != OpCodes.Call) continue;
                        try
                        {
                            MethodDef initMethod = (MethodDef)t.Operand;
                            if (!initMethod.HasBody) continue;
                            if (initMethod.Body.Instructions.Count < 300) continue;
                            for (int y = 0; y < initMethod.Body.Instructions.Count; y++)
                            {



                                C.Clear();
                                var grfds = callGetter(module, initMethod);
                                if (grfds == false) break;
                                else
                                    return initMethod;




                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return null;

        }
    }

}
