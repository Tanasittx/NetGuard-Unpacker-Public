using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    class DateTimes:MutationsBase
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
                for (var i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i]
                            .Operand.ToString().Contains("get_TotalDays"))
                        if ((method.Body.Instructions[i - 1].OpCode == OpCodes.Ldloca_S || method.Body.Instructions[i - 1].IsLdloc() ||
                             method.Body.Instructions[i - 1].OpCode == OpCodes.Ldloca) &&
                            method.Body.Instructions[i - 2].IsStloc() &&
                            method.Body.Instructions[i - 3].OpCode == OpCodes.Call && method.Body
                                .Instructions[i - 3].Operand.ToString().Contains("op_Subtraction"))

                            if (method.Body.Instructions[i - 5].IsLdcI4() &&
                                method.Body.Instructions[i - 6].IsLdcI4() &&
                                method.Body.Instructions[i - 7].IsLdcI4() &&
                                method.Body.Instructions[i - 9].IsLdcI4() &&
                                method.Body.Instructions[i - 10].IsLdcI4() &&
                                method.Body.Instructions[i - 11].IsLdcI4())
                                try
                                {
                                    var int1 = method.Body.Instructions[i - 11].GetLdcI4Value();
                                    var int2 = method.Body.Instructions[i - 10].GetLdcI4Value();
                                    var int3 = method.Body.Instructions[i - 9].GetLdcI4Value();
                                    var date1 = new System.DateTime(int1, int2, int3);

                                    var int4 = method.Body.Instructions[i - 7].GetLdcI4Value();
                                    var int5 = method.Body.Instructions[i - 6].GetLdcI4Value();
                                    var int6 = method.Body.Instructions[i - 5].GetLdcI4Value();
                                    var date2 = new System.DateTime(int4, int5, int6);
                                    var result = (date1 - date2).TotalDays;
                                    for (var y = 0; y < 12; y++)
                                        method.Body.Instructions[i - y].OpCode = OpCodes.Nop;
                                    method.Body.Instructions[i - 4].OpCode = OpCodes.Ldc_I4;
                                    method.Body.Instructions[i - 4].Operand = (int)result;
                                    modified = true;
                                }
                                catch
                                {
                                }
                }
            }
            return modified;
        }
    }
}
