using CawkEmulatorV4;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.FieldFixers
{
    internal class IChannelReceiverHook
    {
        // Token: 0x0600001E RID: 30 RVA: 0x00002252 File Offset: 0x00000452

        // Token: 0x0600001D RID: 29 RVA: 0x00005E64 File Offset: 0x00004064
        internal static uint[] ParseNumbers(byte[] array, int num)
        {
            var array2 = new uint[array.Length / 4];
            var ex = new DuplicateWaitObjectException(array, num);
            for (var i = 0; i < array2.Length; i++)
                array2[i] = ex.get_Item(i);
            return array2;
        }

        // Token: 0x0200000A RID: 10
        public class DuplicateWaitObjectException
        {
            // Token: 0x0400000F RID: 15
            public uint ResourceAttributes;

            // Token: 0x0400000E RID: 14
            private readonly byte[] SyncTextReader;

            // Token: 0x0600001F RID: 31 RVA: 0x0000225A File Offset: 0x0000045A
            internal DuplicateWaitObjectException(byte[] syncTextReader, int resourceAttributes)
            {
                SyncTextReader = syncTextReader;
                ResourceAttributes = (uint)resourceAttributes;
            }

            // Token: 0x06000020 RID: 32 RVA: 0x00002270 File Offset: 0x00000470
            internal uint get_Item(int num)
            {
                return BitConverter.ToUInt32(SyncTextReader, num * 4) ^ ResourceAttributes;
            }
        }
    }

    class FieldsInCtor : CodeFlowBase
    {
        public override void Deobfuscate()
        {
       //     WriteModule(nameof(FieldsInCtor));
            Cleaner();

        }

        internal static void Cleaner()
        {
            var cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;
            foreach (Instruction t in cctor.Body.Instructions)
                if (t.OpCode == OpCodes.Call && t.Operand is MethodDef)
                {
                    var methodDef = (MethodDef)t.Operand;
                    if (!methodDef.HasBody) continue;
                    if (methodDef.Body.Instructions.Count < 9) continue;
                    if (methodDef.Body.Instructions[0].IsLdcI4() && methodDef.Body.Instructions.Count == 9 &&
                        methodDef.Body.Instructions[3].OpCode == OpCodes.Ldtoken &&
                        methodDef.Body.Instructions[7].OpCode == OpCodes.Stsfld)
                    {
                        var field = methodDef.Body.Instructions[3].Operand as FieldDef;
                        var cflowFieldArray = methodDef.Body.Instructions[7].Operand as FieldDef;
                        var arrrr = field.InitialValue;
                        var tester = IChannelReceiverHook.ParseNumbers(arrrr, methodDef.Body.Instructions[5].GetLdcI4Value());
                        FieldReplaace(ModuleDefMD, cflowFieldArray, tester);
                        Base.methodsToRemove.Add(methodDef);
                    }
                    else if (methodDef.Body.Instructions[1].IsLdcI4() && methodDef.Body.Instructions.Count == 10 &&
                             methodDef.Body.Instructions[4].OpCode == OpCodes.Ldtoken &&
                             methodDef.Body.Instructions[8].OpCode == OpCodes.Stsfld)
                    {
                        var field = methodDef.Body.Instructions[4].Operand as FieldDef;
                        var cflowFieldArray = methodDef.Body.Instructions[8].Operand as FieldDef;
                        var arrrr = field.InitialValue;
                        var tester = IChannelReceiverHook.ParseNumbers(arrrr, methodDef.Body.Instructions[6].GetLdcI4Value());
                        FieldReplaace(ModuleDefMD, cflowFieldArray, tester);
                        if (methodDef.Body.Instructions[0].OpCode == OpCodes.Call)
                        {
                            MethodDef methods2 = (MethodDef)methodDef.Body.Instructions[0].Operand;
                            Emulation emu = new Emulation(methods2);
                            emu.Emulate();
                            FieldsInFirstCall.allFields = emu.ValueStack.Fields;
                            FieldsInFirstCall.FieldReplacer(ModuleDefMD);
                            FieldsInFirstCall.allFields = null;
                            Base.methodsToRemove.Add(methodDef);
                        }
                    }
                }
        }

        public static void FieldReplaace(ModuleDefMD module, FieldDef cflowFieldArray, uint[] decoded)
        {
 //           Console.WriteLine("[!] Replacing Fields");
            //findField(module);
            foreach (var types in module.GetTypes())
            {

                foreach (var methods in types.Methods)
                {
                    //   if ((!methods.FullName.Contains("Outfit"))) continue;
                    if (!methods.HasBody) continue;


                    for (var i = 0; i < methods.Body.Instructions.Count; i++)
                        try
                        {
                            if (cflowFieldArray == null) continue;
                            if (methods.Body.Instructions[i].OpCode != OpCodes.Ldsfld) continue;
                            if (!methods.Body.Instructions[i + 1].IsLdcI4() &&
                                methods.Body.Instructions[i + 1].OpCode != OpCodes.Ldc_I4_S) continue;
                            if (methods.Body.Instructions[i + 2].OpCode != OpCodes.Ldelem_U4) continue;
                            if (!methods.Body.Instructions[i].Operand.ToString().Contains(cflowFieldArray.Name)) continue;
                            var dgrfsd = methods.Body.Instructions[i + 1].GetLdcI4Value();
                            var dfgds = uint.Parse(decoded[dgrfsd].ToString());

                            methods.Body.Instructions[i].OpCode = OpCodes.Nop;
                            methods.Body.Instructions[i + 1].OpCode = OpCodes.Ldc_I4;
                            methods.Body.Instructions[i + 1].Operand = (int)dfgds;
                            methods.Body.Instructions[i + 2].OpCode = OpCodes.Nop;
                          
                            //         decoded[dgrfsd] = 0;
                        }
                        catch
                        {
                        }
                }
            }
        }
    }
}
