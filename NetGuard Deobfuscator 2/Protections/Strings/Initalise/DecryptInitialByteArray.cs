using CawkEmulatorV4;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Strings.Initalise
{
    class DecryptInitialByteArray:StringBase
    {
        private static byte[] KeyBytes = new byte[256];
        public static List<Instruction> C = new List<Instruction>();
        public static MethodDef GetMethod;

        public static byte[] byte_0 { get; set; }
        public static FieldDef fields;
        public override void Deobfuscate()
        {
        //    Console.WriteLine("[!] Setting Up String Decryption Finding Init Method");
            GetMethod = firstStep(ModuleDefMD);
            Base.methodsToRemove.Add(GetMethod);
            if (GetMethod == null)
            {
          //      Console.WriteLine("[!!] Method Not Found Fix This");
                return;
            }
       //     Console.WriteLine("[!] Found String Init Method {0}. Emulating", GetMethod.Name);
            var insemu = new Emulation(GetMethod);
            insemu.OnInstructionPrepared = (sender, e) =>
            {
                if (e.Instruction.OpCode == OpCodes.Castclass)
                {
                    e.Cancel = true;
                }
            };
            insemu.OnCallPrepared = (sender, e) =>
            {
                if (e.Instruction.Operand.ToString()
                    .Contains(
                        "System.Void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle")
                )
                {
                    var stack2 = sender.ValueStack.CallStack.Pop();
                    var stack1 = sender.ValueStack.CallStack.Pop();
                    sender.ValueStack.CallStack.Pop();
                    var fielddef = ModuleDefMD.ResolveToken(stack2) as FieldDef;
                    var test = fielddef.InitialValue;
                    var decoded = new uint[test.Length / 4];
                    Buffer.BlockCopy(test, 0, decoded, 0, test.Length);
                    stack1 = decoded;
                    sender.ValueStack.CallStack.Push(stack1);
                    e.bypassCall = true;
                }
                else if (e.Instruction.Operand is MethodDef &&
                         e.Instruction.Operand.ToString().Contains("(System.Byte[])"))
                {
                    e.endMethod = true;
                }
                else
                {
                    e.AllowCall = false;
                }
            };

            GC.Collect();
            Thread.Sleep(1000);
            GC.Collect();
            insemu.Emulate();
            if (GetMethod.Body.Instructions[GetMethod.Body.Instructions.Count - 2].OpCode == OpCodes.Stsfld)
            {
                fields = (FieldDef)GetMethod.Body.Instructions[GetMethod.Body.Instructions.Count - 2].Operand;
            }
            var aaa = GetMethod.Body.Variables.Where(i => i.Type.FullName.Contains("System.Byte[]")).ToArray();


            var byteStackLocal = insemu.ValueStack.Locals[aaa[0].Index];
            //Console.WriteLine("[!] Emulation Success Got Array");

            if (Protections.Base.NativePacker)
                byteStackLocal = P1(byteStackLocal, byteStackLocal.Length);
            byte_0 = (byte[])byteStackLocal;

            return;
        }

        private static byte EncodeDecode(byte data, long index)
        {
            return (byte)(data ^ KeyBytes[index % KeyBytes.Length]);
        }


        private static void GenerateXorKey()
        {
            if (Helper.aes_key != null)
            {
                var index = 0;
                do
                {
                    var value = 500002 * (index + Helper.xor_key[index % Helper.xor_key.Length]) % 255;
                    KeyBytes[index] = (byte)value;
                    index++;
                } while (index != 256);
            }
            else
            {
                //
                var index = 0;

                do
                {
                    var value = 500002 * (index + 564545) % 255;
                    KeyBytes[index] = (byte)value;
                    index++;
                } while (index != 256);
            }


        }

        public static byte[] P1(byte[] param1, int paramLength)
        {
            GenerateXorKey();


            var decodedbytes = new byte[paramLength];
            for (var i = 0; i < paramLength; i++)
                decodedbytes[i] = EncodeDecode(param1[i], i);

            return decodedbytes;
        }
        //
        public static bool SortList()
        {
            var dgrfs = "System.Reflection.Assembly System.Reflection.Assembly::Load(System.Byte[])";

            return C.All(t => !t.Operand.ToString().Contains(dgrfs));
        }

        public static bool callGetter(ModuleDefMD module, MethodDef method)
        {

            foreach (Instruction t in method.Body.Instructions)
            {
                if (t.OpCode == OpCodes.Call)
                    C.Add(t);
            }
            return SortList();
        }

        public static MethodDef firstStep(ModuleDefMD module)
        {
            var cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;

            var method = cctor;
            for (var i = 0; i < method.Body.Instructions.Count; i++)
                if (method.Body.Instructions[i].OpCode == OpCodes.Call)
                    try
                    {
                        var initMethod = (MethodDef)method.Body.Instructions[i].Operand;
                        if (!initMethod.HasBody) continue;
                        if (initMethod.Body.Instructions.Count < 200) continue;
                        for (var y = 0; y < initMethod.Body.Instructions.Count; y++)
                            if (initMethod.Body.Instructions[y].OpCode == OpCodes.Stsfld)
                                if (initMethod.Body.Instructions[y - 1].OpCode == OpCodes.Call)
                                {
                                    C.Clear();
                                    var grfds = callGetter(module, initMethod);
                                    if (grfds == false) continue;
                                    return initMethod;
                                }
                    }
                    catch
                    {
                    }

            return null;
        }
    }
}
