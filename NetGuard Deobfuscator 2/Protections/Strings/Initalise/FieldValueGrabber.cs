using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Strings.Initalise
{
    class FieldValueGrabber : StringBase
    {
        public static Tuple<FieldDef, int> value;

        public override void Deobfuscate()
        {
      //      Console.WriteLine("[!] Getting Decryption Value");
            if (DecryptInitialByteArray.GetMethod != null)
                GetValue();
        //    return 1;
        }

        public static void GetValue()
        {
            bool first = false;
            for (int i = 0; i < DecryptInitialByteArray.GetMethod.Body.Instructions.Count; i++)
            {
                if (DecryptInitialByteArray.GetMethod.Body.Instructions[i].OpCode != OpCodes.Stsfld ||
                    !DecryptInitialByteArray.GetMethod.Body.Instructions[i - 1].IsLdcI4()) continue;
                if (first)
                {
                    value = new Tuple<FieldDef, int>((FieldDef)DecryptInitialByteArray.GetMethod.Body.Instructions[i].Operand, DecryptInitialByteArray.GetMethod.Body.Instructions[i - 1].GetLdcI4Value());
                    //value.Item2 = 
                    break;
                }
                else
                {
                    first = true;
                }
            }
        }
    }
}
