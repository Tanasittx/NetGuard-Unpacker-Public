using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CawkEmulatorV4;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace NetGuard_Deobfuscator_2.Protections.ProxyCalls.Delegates
{
    public static class MathResolver
    {
        public static int GetResult(int num, MethodDef targetMd)
        {
            InstructionEmulator em = new InstructionEmulator();

            MethodDefUser dummy_method = new MethodDefUser();
            dummy_method.Body = new dnlib.DotNet.Emit.CilBody();
            em.Initialize(dummy_method);

            List<Instruction> emuTargets = new List<Instruction>();

            foreach (Instruction inst in targetMd.Body.Instructions)
            {
                if (inst.OpCode == OpCodes.Stfld || inst.OpCode == OpCodes.Ldarg_0 || inst.OpCode == OpCodes.Call || inst.OpCode == OpCodes.Ret)
                {
                    continue;
                }

                if (inst.OpCode == OpCodes.Ldarg_1)
                {
                    emuTargets.Add(Instruction.Create(OpCodes.Ldc_I4, num));
                }
                else if (inst.OpCode == OpCodes.Ldarg)
                {
                    if (((Parameter)inst.Operand).Type.FullName == typeof(System.Int32).FullName && ((Parameter)inst.Operand).Index == 0x1)
                    {
                        emuTargets.Add(Instruction.Create(OpCodes.Ldc_I4, num));
                    }
                }
                else
                {
                    emuTargets.Add(inst);
                }

            }

            foreach (Instruction inst in emuTargets)
            {
                em.Emulate(inst);
            }
            var x = (Int32Value)em.Pop();

            return x.Value.GetHashCode();
        }
    }
    internal class DelegateInfo2
    {
        public FieldDef fieldDef;
        public int byteVal;
        public MethodDef callingMethodDef;
        public uint mdtoken;
        public MethodDef resolvedMethod;
        public OpCode opcode;

    }
    class Remover : ProxyBase
    {
        public override void Deobfuscate()
        {
            ListedDelegateInfo2s = new List<DelegateInfo2>();

            ScrapeCctor(ModuleDefMD);
            ResolveCalls(ModuleDefMD);
            delegateReplacer(ModuleDefMD);
            
        }

        public static List<DelegateInfo2> ListedDelegateInfo2s = new List<DelegateInfo2>();

        public static void delegateReplacer(ModuleDefMD module)
        {
            int amoiunt = 0;
            foreach (TypeDef types in module.GetTypes())
            {
                foreach (MethodDef methods in types.Methods)
                {
                    if (!methods.HasBody) continue;
                    for (int i = 0; i < methods.Body.Instructions.Count; i++)
                    {
                        Instruction inst = methods.Body.Instructions[i];

                        if (inst.OpCode == OpCodes.Call && inst.Operand is MethodDef)
                        {

                            var mtd = (MethodDef)inst.Operand;

                            if (mtd.Name != "Invoke")
                                continue;

                            // Get ldsfld above
                            int x = i;
                            while (x >= 0)
                            {
                                Instruction xInst = methods.Body.Instructions[x];
                                if (xInst.OpCode == OpCodes.Ldsfld && xInst.Operand is FieldDef)
                                {
                                    FieldDef fd = (FieldDef)xInst.Operand;
                                    if (fd.FieldSig.Type.IsOptionalModifier)
                                        break;
                                }
                                x--;
                            }

                            if (x < 0)
                            {
                                continue;
                            }


                            FieldDef fie = (FieldDef)methods.Body.Instructions[x].Operand;

                            foreach (DelegateInfo2 delegateInfo in ListedDelegateInfo2s)
                            {
                                if (delegateInfo.fieldDef == fie)
                                {
                                    var abc = module.ResolveToken(delegateInfo.mdtoken);

                                    if (abc != null)
                                    {
                                        methods.Body.Instructions[i].OpCode = delegateInfo.opcode;
                                        methods.Body.Instructions[i].Operand = abc;


                                        methods.Body.Instructions[x].OpCode = OpCodes.Nop;
                                        amoiunt++;

                                    }
                                    break;

                                }
                            }
                        }
                    }
                }
            }
        }

        public static void fixer(TypeDef callingType, TypeDef newObjType)
        {
            if (callingType.IsSealed && newObjType.IsNestedPrivate)
            {
                newObjType.Attributes -= TypeAttributes.NestedPrivate;
                newObjType.Attributes |= TypeAttributes.Public;
            }
        }
        public static void ScrapeCctor(ModuleDefMD module)
        {
            var cctor = module.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;
            for (int i = 0; i < cctor.Body.Instructions.Count; i++)
            {
                if (cctor.Body.Instructions[i].OpCode == OpCodes.Ldtoken && cctor.Body.Instructions[i + 1].IsLdcI4() &&
                    cctor.Body.Instructions[i + 2].OpCode == OpCodes.Call)
                {
                    FieldDef fieldDef = cctor.Body.Instructions[i].Operand as FieldDef;
                    var byteVal = cctor.Body.Instructions[i + 1].GetLdcI4Value();
                    var setMethod = cctor.Body.Instructions[i + 2].Operand as MethodDef;
                    DelegateInfo2 del = new DelegateInfo2
                    {
                        callingMethodDef = setMethod,
                        byteVal = byteVal,
                        fieldDef = fieldDef
                    };
                    ListedDelegateInfo2s.Add(del);
                }
                else if (cctor.Body.Instructions[i].OpCode == OpCodes.Ldtoken && cctor.Body.Instructions[i + 2].IsLdcI4() &&
                         cctor.Body.Instructions[i + 3].OpCode == OpCodes.Call)
                {
                    FieldDef fieldDef = cctor.Body.Instructions[i].Operand as FieldDef;
                    var byteVal = cctor.Body.Instructions[i + 2].GetLdcI4Value();
                    var setMethod = cctor.Body.Instructions[i + 3].Operand as MethodDef;
                    DelegateInfo2 del = new DelegateInfo2
                    {
                        callingMethodDef = setMethod,
                        byteVal = byteVal,
                        fieldDef = fieldDef
                    };
                    ListedDelegateInfo2s.Add(del);
                }
                else if (cctor.Body.Instructions[i].OpCode == OpCodes.Ldtoken && cctor.Body.Instructions[i + 3].IsLdcI4() &&
                         cctor.Body.Instructions[i + 4].OpCode == OpCodes.Call)
                {
                    FieldDef fieldDef = cctor.Body.Instructions[i].Operand as FieldDef;
                    var byteVal = cctor.Body.Instructions[i + 3].GetLdcI4Value();
                    var setMethod = cctor.Body.Instructions[i + 4].Operand as MethodDef;
                    DelegateInfo2 del = new DelegateInfo2
                    {
                        callingMethodDef = setMethod,
                        byteVal = byteVal,
                        fieldDef = fieldDef
                    };
                    ListedDelegateInfo2s.Add(del);
                }

            }
        }

        public static bool EncryptedDelegateMethod(MethodDef methods)
        {
            bool containsDelegate = false;
            /*
             * 2	000A	ldsfld	class AutoResetEvent modopt(SafeLsaLogonProcessHandle)  '<Module>'::'N£ÿ\u0012\u0017'
3	000F	ldarg	A_0 (0)
4	0013	call	instance object AutoResetEvent::Invoke(valuetype [mscorlib]System.RuntimeFieldHandle)
5	0018	stloc	V_0 (0)

             */
            for (int i = 0; i < methods.Body.Instructions.Count; i++)
            {
                if (methods.Body.Instructions[i].OpCode == OpCodes.Call && methods.Body.Instructions[i].Operand
                        .ToString().Contains("Invoke(System.RuntimeFieldHandle)"))
                {
                    return true;
                }
            }

            return false;
        }
        public static void ResolveCalls(ModuleDefMD module)
        {
            foreach (DelegateInfo2 info2 in ListedDelegateInfo2s)
            {
                MethodDef methods = info2.callingMethodDef;
                FieldDef fields = info2.fieldDef;
                int byteVal = info2.byteVal;
                if (methods.MDToken.ToInt32() == 0x0600000A && byteVal == 9)
                {

                }

                if (EncryptedDelegateMethod(methods))
                {
                    continue;
                }
                var boolean = module.Metadata.TablesStream.TryReadFieldRow(fields.Rid, out RawFieldRow rw);
                byte[] data = module.Metadata.BlobStream.Read(rw.Signature);
                Emulation emu = new Emulation(methods);
                var mdtoken = 0;
                if (methods.MDToken.ToInt32() == 100663336 && byteVal == 4)
                {

                }
                emu.ValueStack.Parameters[methods.Parameters[0]] = null;
                emu.ValueStack.Parameters[methods.Parameters[1]] = (byte)byteVal;
                emu.OnInstructionPrepared = (sender, e) =>
                {
                    if (e.Instruction.OpCode != OpCodes.Callvirt || !e.Instruction.Operand.ToString()
                            .Contains("System.Reflection.MethodBase System.Reflection.Module::ResolveMethod(System.Int32)")) return;
                    mdtoken = sender.ValueStack.CallStack.Pop();
                    if (0x806EC032 == (uint)mdtoken)
                    {

                    }
                    e.Break = true;
                };
                emu.OnCallPrepared = (sender, e) =>
                {
                    var ebc = e.Instruction;
                    if (ebc.Operand.ToString().Contains("System.Byte[] System.Reflection.Module::ResolveSignature(System.Int32)"))
                    {
                        sender.ValueStack.CallStack.Pop();
                        sender.ValueStack.CallStack.Pop();
                        sender.ValueStack.CallStack.Push(data);
                        e.bypassCall = true;
                    }
                    else if (ebc.Operand.ToString().Contains("System.Type[] System.Reflection.FieldInfo::GetOptionalCustomModifiers()"))
                    {
                        sender.ValueStack.CallStack.Pop();

                        sender.ValueStack.CallStack.Push(new Type[500]);
                        e.bypassCall = true;
                    }
                    else if (ebc.Operand.ToString().Contains("System.Int32 System.Reflection.MemberInfo::get_MetadataToken()"))
                    {
                        sender.ValueStack.CallStack.Pop();
                        CModOptSig sig = (CModOptSig)fields.FieldSig.Type;
                        int modToken = sig.Modifier.MDToken.ToInt32();
                        sender.ValueStack.CallStack.Push(modToken);
                        e.bypassCall = true;
                    }
                    else if (ebc.Operand.ToString().Contains("System.String System.Reflection.MemberInfo::get_Name()"))
                    {
                        sender.ValueStack.CallStack.Pop();

                        string sig = fields.Name;

                        sender.ValueStack.CallStack.Push(sig);
                        e.bypassCall = true;
                    }
                    else if (ebc.Operand.ToString().Contains("System.Char System.String::get_Chars(System.Int32)"))
                    {
                        var one = sender.ValueStack.CallStack.Pop();
                        var two = sender.ValueStack.CallStack.Pop();
                        sender.ValueStack.CallStack.Push(two[one]);
                        e.bypassCall = true;
                    }
                    else if (ebc.Operand.ToString().Contains("System.Object[] System.Reflection.MemberInfo::GetCustomAttributes(System.Boolean)"))
                    {
                        var one = sender.ValueStack.CallStack.Pop();
                        var two = sender.ValueStack.CallStack.Pop();
                        sender.ValueStack.CallStack.Push(new object[500]);
                        e.bypassCall = true;
                    }
                    else if (ebc.Operand.ToString().Contains("System.Int32 System.Object::GetHashCode()"))
                    {
                        var one = sender.ValueStack.CallStack.Pop();
                        //			var two = sender.ValueStack.CallStack.Pop();
                        TypeDef caType = fields.CustomAttributes[0].AttributeType.ResolveTypeDef();
                        int caCtorNum = (int)fields.CustomAttributes[0].ConstructorArguments[0].Value;
                        MethodDef meth = First(caType.FindConstructors());
                        int caKey = MathResolver.GetResult(caCtorNum, meth);
                        sender.ValueStack.CallStack.Push(caKey);
                        e.bypassCall = true;
                    }
                    else
                    {

                        e.AllowCall = false;
                        e.bypassCall = false;
                    }

                };
                emu.Emulate();

                info2.mdtoken = (uint)mdtoken;
                var ins = findInstruction(methods);
                var ab = fields.Name.String[ins] ^ byteVal;
                var aa = OpCodes.Call.Value;
                var v = OpCodes.Callvirt.Value;
                var t = OpCodes.Newobj.Value;

                OpCode te;
                if (ab == aa)
                    te = OpCodes.Call;
                else if (ab == v)
                    te = OpCodes.Callvirt;
                else
                    te = OpCodes.Newobj;
                info2.opcode = te;
            }

        }

        private static int findInstruction(MethodDef methods)
        {
            int[] returns = new int[2];
            methods.Body.OptimizeMacros();
            for (int i = 0; i < methods.Body.Instructions.Count; i++)
            {
                if (methods.Body.Instructions[i].OpCode == OpCodes.Ldarg_1 && methods.Body.Instructions[i + 1].OpCode == OpCodes.Xor)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        if (methods.Body.Instructions[i - z].OpCode == OpCodes.Callvirt && methods.Body.Instructions[i - z].Operand.ToString().Contains("System.String System.Reflection.MemberInfo::get_Name()") && methods.Body.Instructions[i - z + 1].IsLdcI4())
                        {
                            var abc = methods.Body.Instructions[i - z + 1].GetLdcI4Value();
                            return abc;
                        }
                    }
                }
            }
            return -1;
        }
        static T First<T>(IEnumerable<T> items)
        {
            using (IEnumerator<T> iter = items.GetEnumerator())
            {
                iter.MoveNext();
                return iter.Current;
            }
        }
    }
}
