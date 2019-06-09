using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace NetGuard_Deobfuscator_2.Protections.ProxyCalls.Calli
{
    public class Validator
    {
        // Token: 0x06000036 RID: 54 RVA: 0x000024F2 File Offset: 0x000006F2
        public Validator() : this(3988292384u, uint.MaxValue)
        {
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00003A08 File Offset: 0x00001C08
        public Validator(uint num, uint num2)
        {
            this.table = InitializeTable(num);
            this.hash = num2;
            this.seed = num2;
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00002500 File Offset: 0x00000700
        public void Initialize()
        {
            this.hash = this.seed;
        }

        // Token: 0x06000039 RID: 57 RVA: 0x0000250E File Offset: 0x0000070E
        protected void HashCore(byte[] array, int num, int num2)
        {
            this.hash = Validator.CalculateHash(this.table, this.hash, array, num, num2);
        }

        // Token: 0x0600003A RID: 58 RVA: 0x0000252A File Offset: 0x0000072A
        protected byte[] HashFinal()
        {
            return Validator.UInt32ToBigEndianBytes(~this.hash);
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00002538 File Offset: 0x00000738
        public int get_HashSize()
        {
            return 32;
        }

        // Token: 0x0600003C RID: 60 RVA: 0x0000253C File Offset: 0x0000073C
        public static uint Compute(string s)
        {
            return Validator.Compute(Encoding.UTF7.GetBytes(s));
        }

        // Token: 0x0600003D RID: 61 RVA: 0x0000254E File Offset: 0x0000074E
        public static uint Compute(byte[] array)
        {
            return Validator.Compute(uint.MaxValue, array);
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00002557 File Offset: 0x00000757
        public static uint Compute(uint num, byte[] array)
        {
            return Validator.Compute(3988292384u, num, array);
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00002565 File Offset: 0x00000765
        public static uint Compute(uint num, uint num2, byte[] array)
        {
            return ~Validator.CalculateHash(Validator.InitializeTable(num), num2, array, 0, array.Length);
        }

        // Token: 0x06000040 RID: 64 RVA: 0x00003A38 File Offset: 0x00001C38
        private static uint[] InitializeTable(uint num)
        {
            if (num == 3988292384u && Validator.defaultTable != null)
            {
                return Validator.defaultTable;
            }
            uint[] array = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                uint num2 = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((num2 & 1u) == 1u)
                    {
                        num2 = (num2 >> 1 ^ num);
                    }
                    else
                    {
                        num2 >>= 1;
                    }
                }
                array[i] = num2;
            }
            if (num == 3988292384u)
            {

                Validator.defaultTable = array;
            }
            return array;
        }

        // Token: 0x06000041 RID: 65 RVA: 0x00003AA8 File Offset: 0x00001CA8
        private static uint CalculateHash(uint[] array, uint num, IList<byte> list, int num2, int num3)
        {
            uint num4 = num;
            for (int i = num2; i < num2 + num3; i++)
            {
                num4 = (num4 >> 8 ^ array[(int)((uint)list[i] ^ (num4 & 255u))]);
            }
            return num4;
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00003AE0 File Offset: 0x00001CE0
        private static byte[] UInt32ToBigEndianBytes(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        // Token: 0x0400003D RID: 61
        public uint DefaultPolynomial;

        // Token: 0x0400003E RID: 62
        public uint DefaultSeed;

        // Token: 0x0400003F RID: 63
        private static uint[] defaultTable;

        // Token: 0x04000040 RID: 64
        private readonly uint seed;

        // Token: 0x04000041 RID: 65
        private readonly uint[] table;

        // Token: 0x04000042 RID: 66
        private uint hash;
    }
    #region Custom Info Classes
    public class DelegateInfo
    {
        public int ByteKey;
        public FieldDef Field;
        public MethodDef Decrypter;
        public IMethodDefOrRef Resolved;
        public OpCode CallOpcode;
        public override string ToString()
        {
            return $"{Field.Name} -> {Resolved.FullName}";
        }
    }
    public class CallInfo
    {
        public int Index;
        public MethodDef TargetMethod;
        public List<CallRef> References = new List<CallRef>();
        public override string ToString()
        {
            return $"{Index} - {TargetMethod.FullName} - {References.Count} refs";
        }
    }
    public class CallRef
    {
        public MethodDef ParentMethod;
        public Instruction CalliInst;
    }
    #endregion
    class Remover : ProxyBase
    {
        public override void Deobfuscate()
        {
  //          Console.WriteLine("[!] Cleaning Callis");
            List<CallInfo> calls = FindPointers();
            if (calls == null) return;
            if (calls.Count == 0)
            {
                //Console.WriteLine("[!] No calls found!");
                //		Console.ReadLine();
                return;
            }

         //   Console.WriteLine("[!] Calli Total Calls: {0}", calls.Count);

            foreach (CallInfo call in calls)
            {
                call.References.AddRange(FindReferences(call));
           
            }

            int replaced = 0;
            foreach (CallInfo call in calls)
            {
               
                foreach (CallRef reference in call.References)
                {
                    if (reference.ParentMethod.MDToken.ToInt32() == 0x06000260)
                    {

                    }
                    int instIndex = reference.ParentMethod.Body.Instructions.IndexOf(reference.CalliInst);
                    if (instIndex == -1)
                        continue;

                    // Nop the above 3 insts
                    var fieldCalli = reference.ParentMethod.Body.Instructions[instIndex - 3].Operand as FieldDef;
                    if (fieldCalli != fieldDef) continue;
                    for (int i = 1; i <= 3; i++)
                    {
                        reference.ParentMethod.Body.Instructions[instIndex - i].OpCode = OpCodes.Nop;
                        reference.ParentMethod.Body.Instructions[instIndex - i].Operand = null;
                    }
                    reference.ParentMethod.Body.Instructions[instIndex].OpCode = OpCodes.Call;
                    reference.ParentMethod.Body.Instructions[instIndex].Operand = ModuleDefMD.Import(call.TargetMethod);
                    replaced++;
                }
            }

         //   Console.WriteLine("[!] Calli Total Replaced: {0}", replaced);
            return;
        }

        #region Extra Functions
        static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
        #endregion
        #region Calli Calls
        static Dictionary<MethodDef, FieldDef> completedMethod = new Dictionary<MethodDef, FieldDef>();
        private static FieldDef fieldDef;

        static MethodDef findCalliCall()
        {
            /*474	0416	ldloc.s	V_14 (14)
475	0418	callvirt	instance valuetype [mscorlib]System.RuntimeMethodHandle [mscorlib]System.Reflection.MethodBase::get_MethodHandle()
476	041D	stloc.s	V_22 (22)
477	041F	ldloca.s	V_22 (22)
478	0421	call	instance native int [mscorlib]System.RuntimeMethodHandle::GetFunctionPointer()
479	0426	stloc.1
*/
            var cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;

            foreach (MethodDef methods in cctor.DeclaringType.Methods)
            {
                if (!methods.HasBody) continue;
                for (int i = 0; i < methods.Body.Instructions.Count; i++)
                {
                    if (methods.Body.Instructions[i].OpCode == OpCodes.Call && methods.Body.Instructions[i].Operand.ToString()
                            .Contains("RuntimeMethodHandle::GetFunctionPointer"))
                    {


                        if (completedMethod.ContainsKey(methods)) continue;
                        else
                        {
                            fieldDef = getField(methods);
                            completedMethod.Add(methods, fieldDef);
                            return methods;
                        }


                    }
                }
            }
            return null;
        }
        static FieldDef getField(MethodDef methods)
        {
            foreach (Instruction instr in methods.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Stsfld)
                {
                    return instr.Operand as FieldDef;
                }
            }
            return null;
        }
        static List<CallInfo> FindPointers()
        {
            List<CallInfo> calls = new List<CallInfo>();

            MethodDef resolveMethod = null;


            resolveMethod = findCalliCall();
            if (resolveMethod == null) return null;
            byte[] data = FindInitialData(resolveMethod);
            int dataStored = 0;
            if (data == null || data.Length == 0)
            {
                return calls;
            }


            if (resolveMethod == null)
            {
             //   Console.WriteLine("[!!] Calli Call Not Found -- Fix It");
                return calls;
            }

            // Remove call from Module.cctor
            var cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;

            for (int i = 0; i < cctor.Body.Instructions.Count; i++)
            {
                Instruction inst = cctor.Body.Instructions[i];
                if (inst.Operand != null && inst.OpCode == OpCodes.Call && inst.Operand == resolveMethod)
                {
                    cctor.Body.Instructions[i].OpCode = OpCodes.Nop;
                    cctor.Body.Instructions[i].Operand = null;
                    break;
                }
            }


            int index = 0;
            for (int i = 0; i < resolveMethod.Body.Instructions.Count; i++)
            {
                Instruction inst = resolveMethod.Body.Instructions[i];
                if (inst.OpCode == OpCodes.Ldftn && resolveMethod.Body.Instructions[i + 1].OpCode == OpCodes.Callvirt)
                {
                    calls.Add(new CallInfo()
                    {
                        Index = index,
                        References = new List<CallRef>(),
                        TargetMethod = inst.Operand as MethodDef
                    });
                    index++;
                }
                // Could cause some serious problems being only sbyte, need to improve the casting to int when required
                if (inst.OpCode == OpCodes.Ldsfld && (resolveMethod.Body.Instructions[i - 1].IsStloc() || resolveMethod.Body.Instructions[i - 1].OpCode == OpCodes.Stind_I4) && resolveMethod.Body.Instructions[i - 2].IsLdcI4())
                {
                    dataStored = resolveMethod.Body.Instructions[i - 2].GetLdcI4Value();

                }

                //dataStored = 253;

            }


            // XOR decrypt
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= (byte)i;
            }

            // Decompress the data
            data = Decompress(data);

            Dictionary<uint, TypeSig> ComputedFields = new Dictionary<uint, TypeSig>();

            foreach (FieldDef fd in cctor.DeclaringType.Fields)
            {
                ComputedFields.Add(Validator.Compute(fd.Name), fd.FieldType);
            }

            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                for (int i = 0; i < dataStored; i++)
                {
                    if (i == 77)
                    {

                    }
                    int offset = i * 20;
                    br.BaseStream.Position = offset;

                    int paramLength = br.ReadInt32();

                    int methodNamePtr = br.ReadInt32();
                    int paramTypesPtr = br.ReadInt32();

                    uint fieldNameChecksum = br.ReadUInt32();

                    int methodReturnPtr = br.ReadInt32();

                    uint paramTypesChecksum = 0u;
                    if (paramLength > 0)
                        paramTypesChecksum = BitConverter.ToUInt32(data, paramTypesPtr);

                    br.BaseStream.Position = methodReturnPtr;
                    uint methodReturnChecksum = br.ReadUInt32();

                    br.BaseStream.Position = methodNamePtr;
                    uint methodNameChecksum = br.ReadUInt32();

                    TypeRef methodType = null;

                    if (ComputedFields.ContainsKey(fieldNameChecksum))
                    {
                        methodType = (TypeRef)ComputedFields[fieldNameChecksum].ToTypeDefOrRef();
                    }
                    if (methodType.FullName.Contains("dnlib"))
                    {


                    }

                    foreach (MethodDef md in methodType.ResolveThrow().Methods)
                    {
                        if (md.FullName.Contains("set_DialogResult"))
                        {
                            //"System.Windows.Window.set_DialogResult(System.Nullable`1[System.Boolean])"
                        }
                        if (Validator.Compute(md.Name) != methodNameChecksum || Validator.Compute(md.ReturnType.ReflectionFullName) != methodReturnChecksum)
                            continue;


                        int mParamLength = md.Parameters.Count;
                        if (md.HasThis)
                            mParamLength -= 1;

                        if (mParamLength != paramLength)
                        {
                            continue;
                        }

                        string[] mParams = md.HasThis ? md.Parameters.Skip(1).Select(x => x.Type.ReflectionFullName).ToArray() : md.Parameters.Select(x => x.Type.ReflectionFullName).ToArray();
                        for (int y = 0; y < mParams.Length; y++)
                        {
                            if (mParams[y].Contains("mscorlib"))
                            {
                                mParams[y] = SoapMonthRandom(mParams[y]);
                            }
                        }
                        if (Validator.Compute(string.Join(string.Empty, mParams)) != paramTypesChecksum)
                        {
                            continue;
                        }

                        calls.Add(new CallInfo()
                        {
                            Index = index,
                            References = new List<CallRef>(),
                            TargetMethod = md
                        });
                        index++;

                    }
                }
            }

            return calls;
        }
        public static string SoapMonthRandom(string A_0)
        {
            int num = A_0.IndexOf("mscorlib");
            if (num == -1)
            {
                return A_0;
            }
            int num2 = A_0.IndexOf("]", num);
            if (num2 == -1)
            {
                return A_0;
            }
            if (num > num2)
            {
                return A_0;
            }
            return A_0.Remove(num, num2 - num);
        }
        static byte[] FindInitialData(MethodDef method)
        {
            var cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;

            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Call)
                {
                    MethodDef methods2 = (MethodDef)method.Body.Instructions[i].Operand;
                    if (methods2.Parameters.Count != 0) continue;
                    var getname = getFieldName(methods2);
                    if (getname == null)
                    {
                       for(int y = 0; y < methods2.Body.Instructions.Count; y++)
                        {
                            if(methods2.Body.Instructions[y].OpCode == OpCodes.Ldtoken&&methods2.Body.Instructions[y+1].OpCode == OpCodes.Call)
                            {
                                var ldtokenOperand = methods2.Body.Instructions[y].Operand as IField;
                                getname = ldtokenOperand.Name;
                                break;
                            }
                        }
                    }
                    return cctor.DeclaringType.GetFields(getname).First().InitialValue;

                }
            }
            return null;
        }
        static string getFieldName(MethodDef method)
        {
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Ldsfld)
                {
                    if (method.Body.Instructions[i].Operand.ToString().Contains("Ldtoken"))
                    {
                        if (method.Body.Instructions[i + 3].OpCode == OpCodes.Ldstr)
                        {
                            return method.Body.Instructions[i + 3].Operand.ToString();
                        }
                    }
                }
            }
            return null;
        }
        static List<CallRef> FindReferences(CallInfo info)
        {
            List<CallRef> refs = new List<CallRef>();

            foreach (TypeDef td in ModuleDefMD.GetTypes())
            {
                foreach (MethodDef md in td.Methods)
                {
                    if (!md.HasBody)
                        continue;
                    for (int i = 0; i < md.Body.Instructions.Count; i++)
                    {
                        Instruction inst = md.Body.Instructions[i];

                        if (inst.OpCode == OpCodes.Calli)
                        {
                            Instruction indexInst = md.Body.Instructions[i - 2];
                            if (indexInst.IsLdcI4())
                            {
                                if (info.Index == indexInst.GetLdcI4Value())
                                {
                                    refs.Add(new CallRef()
                                    {
                                        CalliInst = inst,
                                        ParentMethod = md,
                                    });
                                }
                            }

                        }
                    }
                }
            }

            return refs;
        }
        #endregion
    }
}
