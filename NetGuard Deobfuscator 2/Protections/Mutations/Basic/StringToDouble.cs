using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    class StringToDouble:MutationsBase
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
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    var instruction = method.Body.Instructions[i];
                    if (instruction.OpCode != OpCodes.Call) continue;
                    if (!(instruction.Operand is MethodDef)) continue;
                    MethodDef strMethod = instruction.Operand as MethodDef;
                    if (strMethod.Parameters.Count != 1) continue;
                    if (strMethod.ReturnType != ModuleDefMD.CorLibTypes.Int32) continue;
                    if (method.Body.Instructions[i - 1].OpCode != OpCodes.Ldstr) continue;
                    var stringVal = method.Body.Instructions[i - 1].Operand.ToString();
                    var decryptedInt = MuiResourceTypeIdStringEntryFieldId(stringVal);
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i - 1].Operand = decryptedInt;
                    modified = true;

                }
               
            }
            return modified;
        }
        internal static int MuiResourceTypeIdStringEntryFieldId(string A_0)
        {
            if (!NodeKeyValueEnumerator.ContainsKey(A_0))
            {

                abc(A_0);
            }
            return (int)((double)NodeKeyValueEnumerator[A_0]);
        }
        public static Dictionary<string, int> NodeKeyValueEnumerator = new Dictionary<string, int>();
        internal static void abc(string A_0)
        {

            double num = 0.0;
            byte[] bytes = Encoding.ASCII.GetBytes(A_0);
            var one = 0;
            while (one < bytes.Length)
            {
                num += (double)bytes[one] * Math.Pow((double)bytes.Length, (double)(-1 * one));
                one = one + 1;
            }
            double num2 = Math.Floor(num);
            num = Math.Ceiling(num + Math.Pow(num, 3.1415926535897931 * (num - num2)));

            NodeKeyValueEnumerator.Add(A_0, (int)num);
        }
    }
}
