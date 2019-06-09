using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections
{
    internal class Helper
    {
        public static List<string> RC4keys = new List<string>();
        internal static byte[] aes_key;
        public static byte[] xor_key;

        public static void InitializeRC4List()
        {
            RC4keys.Add("qp2Uroy8xySG8t7VenP0Xze0yNPwCN92");
            RC4keys.Add("test only test");
        }
        public static void RetMethod(MethodDef method)
        {
            if (!method.HasBody) return;
            if (method.Body.Instructions.Count < 1) return;
            method.Body.Instructions[0].OpCode = OpCodes.Ret;
        }

        public static bool IsMethodUsingVirtualProtect(MethodDef method)
        {
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode != OpCodes.Call)
                    continue;
                if (!(instruction.Operand is MethodDef))
                    continue;
                var Method = instruction.Operand as MethodDef;
                if (Method == null) throw new ArgumentNullException(nameof(Method));
                if (Method?.ImplMap == null)
                    continue;
                if (Method.ImplMap.Name != "VirtualProtect")
                    continue;
                return true;
            }
            return false;
        }
    }
}
