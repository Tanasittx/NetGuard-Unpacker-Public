using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    class MathCleaner:MutationsBase
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
                for(int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    var instruction = method.Body.Instructions[i];
                    if (instruction.OpCode != OpCodes.Call) continue;
                    if (!(instruction.Operand is IMethodDefOrRef)) continue;
                    if (!instruction.Operand.ToString().Contains("System.Math::")) continue;
                    if(FixMaths(method,i,instruction.Operand as IMethodDefOrRef))
                        modified = true;
                }
            }

            return modified;
        }
        private static bool FixMaths(MethodDef method,int i,IMethodDefOrRef MathMethod)
        {
            var modified = false;
            var fullName = MathMethod.FullName;
            if (fullName == "System.Double System.Math::Pow(System.Double,System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8 && method.Body.Instructions[i - 2].OpCode == OpCodes.Ldc_R8)
                {

                    var value = (double)method.Body.Instructions[i - 2].Operand;
                    var value2 = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Pow(value, value2);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 2].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8 && method.Body.Instructions[i - 2].OpCode == OpCodes.Ldc_R4)
                {

                    var value = (Single)method.Body.Instructions[i - 2].Operand;
                    var value2 = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Pow(value, value2);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 2].Operand =(double) result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4 && method.Body.Instructions[i - 2].OpCode == OpCodes.Ldc_R8)
                {

                    var value = (double)method.Body.Instructions[i - 2].Operand;
                    var value2 = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Pow(value, value2);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 2].Operand = (double)result;
                    modified = true;
                }
            }
            else if (fullName == "System.UInt32 System.Math::Max(System.UInt32,System.UInt32)")
            {
                if (!method.Body.Instructions[i - 1].IsLdcI4()) return modified;
                if (!method.Body.Instructions[i - 2].IsLdcI4()) return modified;
                var value = method.Body.Instructions[i - 2].GetLdcI4Value();
                var value2 = method.Body.Instructions[i - 1].GetLdcI4Value();
                var result = Math.Max(value, value2);
                method.Body.Instructions[i].OpCode = OpCodes.Nop;
                method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                method.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_R8;
                method.Body.Instructions[i - 2].Operand = result;
                modified = true;
            }
            else if (fullName == "System.Double System.Math::Abs(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Abs(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Abs(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            
            else if (fullName == "System.Double System.Math::Cos(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Cos(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Cos(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            else if (fullName == "System.Double System.Math::Sin(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Sin(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Sin(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            else if (fullName == "System.Double System.Math::Floor(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Floor(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Floor(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            else if (fullName == "System.Double System.Math::Log(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Log(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Log(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            else if (fullName == "System.Double System.Math::Sqrt(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Sqrt(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Sqrt(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            else if (fullName == "System.Double System.Math::Ceiling(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Ceiling(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Ceiling(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            else if (fullName == "System.Double System.Math::Exp(System.Double)")
            {
                if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
                {
                    var value = (double)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Exp(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = result;
                    modified = true;
                }
                else if (method.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    var value = (Single)method.Body.Instructions[i - 1].Operand;
                    var result = Math.Exp(value);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_R8;
                    method.Body.Instructions[i - 1].Operand = (double)result;
                    modified = true;
                }
            }
            else
            {

            }




            return modified;
        }
    }
}
