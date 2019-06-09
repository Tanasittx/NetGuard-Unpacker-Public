using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Fields
{
    class NativeIntDecryption : MutationsBase
    {
        public override bool Deobfuscate()
        {
            return Clean();
        }
        private static bool Clean()
        {
            var modified = false;
            foreach (MethodDef method in methods)
            {
                for(int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    var instruction = method.Body.Instructions[i];
                    if (instruction.OpCode != OpCodes.Call) continue;
                    if (!instruction.Operand.ToString().Contains("::Invoke")) continue;
                    if (!(instruction.Operand is MethodDef)) continue;
                    var callingMethod = instruction.Operand as MethodDef;
                    if (callingMethod.Parameters.Count != 3) continue;
                    if (callingMethod.ReturnType != ModuleDefMD.CorLibTypes.Double) continue;
                    if (method.Body.Instructions[i - 1].OpCode != OpCodes.Ldc_R8) continue;
                    if (!method.Body.Instructions[i - 2].IsLdcI4()) continue;
                    if (method.Body.Instructions[i - 3].OpCode != OpCodes.Ldsfld) continue;
                    var value1 = (double)method.Body.Instructions[i - 1].Operand;
                    var value2 = method.Body.Instructions[i - 2].GetLdcI4Value();
                    var decryptedValue = Decrypt(value1, value2);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 3].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].Operand = decryptedValue;
                    modified = true;
                }
            }
            return modified;
        }
        private static double Decrypt(double val, int first)
        {
            var v4 = ~first ^ Helper.xor_key[(int)(Math.Truncate(Math.Abs(val)) % Helper.xor_key.Length)];
            double result = 0;
            switch (v4)
            {
                case 0:
                    result = Math.Sin(val);
                    break;
                case 1:
                    result = Math.Cos(val);
                    break;
                case 2:
                    result = Math.Sqrt(val);
                    break;
                case 3:
                    result = Math.Truncate(val);
                    if (Frac(val) < 0.0)
                        result--;
                    break;
                case 4:
                    result = Math.Log(val);
                    break;
                case 5:
                    var result0 = val * 10;
                    var result2 = Math.Truncate(result0);
                    var result3 = val * 10;
                    var result4 = Math.Truncate(result3);
                    result = (double)((int)result2 & ~(int)result4);
                    break;
                default:
                    break;
            }
            return result;
        }
        public static double Frac(double value)
        {
            return value - Math.Truncate(value);
        }
    }
}
