using CawkEmulatorV4;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using NetGuard_Deobfuscator_2.Protections.Strings.Initalise;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unicorn;
using Unicorn.X86;

namespace NetGuard_Deobfuscator_2.Protections.Strings.DecryptStrings
{
    internal class DecryptStrings : StringBase
    {
        private static UTF8String methodName;
        private static byte[] methodBytes;

        public override void Deobfuscate()
        {
            Clean();
        }

        public static List<dnlib.DotNet.Emit.Instruction> GetInstructions(IList<dnlib.DotNet.Emit.Instruction> instrs, int index)
        {
            int stackSize = 0, end = 1;
            List<dnlib.DotNet.Emit.Instruction> condInstrs = new List<dnlib.DotNet.Emit.Instruction>();
            if (instrs[index - 1].OpCode == OpCodes.Dup)
                end = 1;
            for (int i = index; i > -1; i--)
            {
                dnlib.DotNet.Emit.Instruction instr = instrs[i];
                instr.CalculateStackUsage(out int push, out int pop);
                stackSize += push - pop;
                condInstrs.Add(instr);
                if (stackSize == end)
                {
                    if(instr.OpCode!= OpCodes.Dup)
                        break;
                    else
                    {

                    }
                }
                    
            }
            condInstrs.Reverse();
            return condInstrs;
        }
        private static void Clean()
        {
            foreach (MethodDef method in methods)
            {
                methodName = method.Name;
                methodBytes = Strings.Initalise.GetOriginalBytes.bytesDict[method.MDToken.ToUInt32()];
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    dnlib.DotNet.Emit.Instruction instruction = method.Body.Instructions[i];
                    if (instruction.OpCode != OpCodes.Call) continue;
                    if (!(instruction.Operand is MethodSpec)) continue;
                    if (!instruction.Operand.ToString().Contains("System.String>")) continue;
                    if (instruction.Operand.ToString().Contains("(System.UInt32,System.Object)"))
                    {
                        List<dnlib.DotNet.Emit.Instruction> instructions = GetInstructions(method.Body.Instructions, i);
                        CawkEmulatorV4.Emulation emu = new CawkEmulatorV4.Emulation(method);
                        uint[] setUp = new uint[2];
                        emu.OnInstructionPrepared += (emulation, args) =>
                        {
                            if (args.Instruction.IsLdloc())
                            {
                                emulation.ValueStack.CallStack.Push(setUp);
                                args.Cancel = true;
                            }
                            if (args.Instruction.OpCode == OpCodes.Box)
                            {
                                args.Cancel = true;
                            }
                        };
                        if (instructions[0].OpCode == OpCodes.Dup)
                        {
                            emu.ValueStack.CallStack.Push(setUp);
                            emu.Emulate2(instructions, 0, instructions.Count - 1);
                        }
                        else
                        {
                            emu.Emulate2(instructions, 0, instructions.Count - 1);
                        }
                        dynamic array = emu.ValueStack.CallStack.Pop();
                        dynamic uintVal = emu.ValueStack.CallStack.Pop();
                        string decryptedString = DecryptSting(instruction.Operand as MethodSpec, (uint)uintVal, (object)array);
                        for (int y = 0; y < instructions.Count; y++)
                        {
                            method.Body.Instructions[i - y].OpCode = OpCodes.Nop;
                        }
                        method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                        method.Body.Instructions[i].Operand = decryptedString;

                    }
                    else
                    {
                 //       throw new NotSupportedException();
                    }

                }
            }
        }
        private static string DecryptSting(MethodSpec decryptionMethod, uint uintVal, object arrayVal)
        {
            Base.methodsToRemove.Add(decryptionMethod);
            MethodDef resolvedDef = decryptionMethod.ResolveMethodDef();
            CawkEmulatorV4.Emulation emulationD = new CawkEmulatorV4.Emulation(resolvedDef);
            emulationD.ValueStack.Fields[ModuleDefMD.ResolveToken(Strings.Initalise.DecryptInitialByteArray.fields.MDToken.ToInt32()) as FieldDef] = Strings.Initalise.DecryptInitialByteArray.byte_0;
            File.WriteAllBytes("Method.bin", methodBytes);
            if (FieldValueGrabber.value != null)
                emulationD.ValueStack.Fields[
                        ModuleDefMD.ResolveToken(FieldValueGrabber.value.Item1.MDToken.ToInt32()) as
                            FieldDef] = (int)
                    FieldValueGrabber.value.Item2;

            emulationD.ValueStack.Parameters[resolvedDef.Parameters[0]] = uintVal;
            emulationD.ValueStack.Parameters[resolvedDef.Parameters[1]] = arrayVal;
            emulationD.OnCallPrepared = (sender, e) => { HandleCall(sender, e); };
            emulationD.OnInstructionPrepared += (emulation, args) =>
            {
                if (args.Instruction.OpCode == OpCodes.Isinst)
                {
                    dynamic value = emulation.ValueStack.CallStack.Pop();
                    TypeRef instrType = args.Instruction.Operand as TypeRef;
                    dynamic name = value.GetType().Name;
                    if (instrType.FullName.Contains(name))
                    {
                        emulation.ValueStack.CallStack.Push(1);
                    }
                    else
                    {
                        emulation.ValueStack.CallStack.Push(null);
                    }
                    args.Cancel = true;

                }
                else if (args.Instruction.OpCode == OpCodes.Castclass)
                {
                    dynamic abc = emulation.ValueStack.CallStack.Pop();
                    if (abc is int[])
                    {
                        emulation.ValueStack.CallStack.Push((int[])abc);
                        args.Cancel = true;
                    }

                    if (args.Instruction.Operand.ToString().Contains("Assembly"))
                    {
                        emulation.ValueStack.CallStack.Push(null);
                        args.Cancel = true;
                    }
                }
            };
            emulationD.Emulate();
            return (string)emulationD.ValueStack.CallStack.Pop();
        }
        public static void HandleCall(Emulation sender, CallEventArgs e)
        {
            object instruction = e.Instruction.Operand;
            if (instruction.ToString().Contains("System.Void System.Diagnostics.StackTrace::.ctor()"))
            {
                sender.ValueStack.CallStack.Push(1);
                e.bypassCall = true;
            }

            else if (instruction.ToString().Contains("System.Diagnostics.StackFrame System.Diagnostics.StackTrace::GetFrame(System.Int32)"))
            {
                dynamic call = sender.ValueStack.CallStack.Pop();
                dynamic ldc = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(1);
                e.bypassCall = true;
            }
            else if (e.Instruction.Operand.ToString().Contains("System.String System.String::Intern(System.String)"))
            {
                dynamic abc = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(string.Intern(abc));
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Reflection.MethodBase System.Diagnostics.StackFrame::GetMethod()"))
            {
                dynamic call = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(1);
                e.bypassCall = true;

            }
            else if (instruction.ToString().Contains("System.Reflection.MethodBody System.Reflection.MethodBase::GetMethodBody()"))
            {
                dynamic call = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(1);
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Byte[] System.Reflection.MethodBody::GetILAsByteArray()"))
            {
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(methodBytes);
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Text.Encoding System.Text.Encoding::get_UTF8()"))
            {
                sender.ValueStack.CallStack.Push(Encoding.UTF8);
                e.bypassCall = true;
            }
            else if (e.Instruction.Operand.ToString().Contains("System.String System.Text.Encoding::GetString(System.Byte[],System.Int32,System.Int32)"))
            {
                dynamic abc = sender.ValueStack.CallStack.Pop();
                dynamic def = sender.ValueStack.CallStack.Pop();
                dynamic ghi = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(Encoding.UTF8.GetString(ghi, (int)def, (int)abc));
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.String System.Reflection.MemberInfo::get_Name()"))
            {
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(methodName);
                e.bypassCall = true;
            }
            else if (e.Instruction.Operand.ToString().Contains("System.Byte[] System.Text.Encoding::GetBytes(System.String)"))
            {
                dynamic abc = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(Encoding.UTF8.GetBytes(abc));
                e.bypassCall = true;
            }
            else if (e.Instruction.Operand.ToString().Contains("System.Type System.Object::GetType()"))
            {
                dynamic abc = sender.ValueStack.CallStack.Pop();
                //        sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(abc.GetType());
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Boolean System.String::op_Equality(System.String,System.String)"))
            {
                dynamic one = sender.ValueStack.CallStack.Pop();
                dynamic two = sender.ValueStack.CallStack.Pop();
                dynamic boole = one == two;
                sender.ValueStack.CallStack.Push(boole);
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Boolean System.String::op_Inequality(System.String,System.String)"))
            {
                dynamic one = sender.ValueStack.CallStack.Pop();
                dynamic two = sender.ValueStack.CallStack.Pop();
                dynamic boole = one != two;
                sender.ValueStack.CallStack.Push(boole);
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Reflection.Assembly System.Reflection.Assembly::GetCallingAssembly()"))
            {

                sender.ValueStack.CallStack.Push(1);
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.String System.Reflection.Assembly::get_FullName()"))
            {
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(ModuleDefMD.FullName);
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Reflection.Assembly System.Reflection.Assembly::GetExecutingAssembly()"))
            {

                sender.ValueStack.CallStack.Push(1);
                e.bypassCall = true;
            }
            else if (instruction.ToString()
                .Contains("System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle"))
            {
                dynamic stack = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(typeof(string).GetType());
                e.bypassCall = true;
            }
            else if (instruction.ToString().Contains("System.Object System.Reflection.MethodBase::Invoke"))
            {
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Push(1);
                e.bypassCall = true;
            }
            else if (instruction is MethodDef && ((MethodDef)instruction).IsNative)
            {
                dynamic value = sender.ValueStack.CallStack.Pop();
                MethodDef methodsss = (MethodDef)instruction;
                dnlib.IO.FileOffset offset = ModuleDefMD.Metadata.PEImage.ToFileOffset(methodsss.RVA);
                uint abc = (uint)Test(File.ReadAllBytes(Program.Path), (ulong)offset, (uint)value);
                sender.ValueStack.CallStack.Push((int)abc);
                e.bypassCall = true;

            }
            else if (e.Instruction.Operand.ToString()
                    .Contains(
                        "System.Void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle")
                )
            {
                dynamic stack2 = sender.ValueStack.CallStack.Pop();
                dynamic stack1 = sender.ValueStack.CallStack.Pop();
                sender.ValueStack.CallStack.Pop();
                FieldDef fielddef = ModuleDefMD.ResolveToken(stack2) as FieldDef;
                byte[] test = fielddef.InitialValue;
                uint[] decoded = new uint[test.Length / 4];
                Buffer.BlockCopy(test, 0, decoded, 0, test.Length);
                stack1 = decoded;
                sender.ValueStack.CallStack.Push(stack1);
                e.bypassCall = true;
            }
        }


        public static ulong addr = 0x1000000UL;
        public static ulong peb = addr + 0x500000;
        public static long stack_offset = 0x200000;
        public static long Test(byte[] file, ulong offset, uint value)
        {
            uint in_value = value;
            ulong stack_pointer = addr + (ulong)stack_offset;

            byte[] code = file.Skip((int)offset).ToArray();

            byte[] real_code = new byte[0x2000];

            Array.Copy(code, real_code, real_code.Length);

            //   Console.WriteLine("Emulate i386 code");

            using (X86Emulator emulator = new X86Emulator(X86Mode.b32))
            {
                emulator.Memory.Map(addr, 1024 * 1024 * 1024, MemoryPermissions.All);
                emulator.Memory.Write(addr, real_code, real_code.Length);


                byte[] esp_b = BitConverter.GetBytes(in_value);

                //Array.Reverse(esp_b);

                emulator.Memory.Write(stack_pointer + 4, BitConverter.GetBytes(in_value), sizeof(uint));


                emulator.Registers.ESP = (long)stack_pointer;
                //emulator.Registers.ECX = (long)esp;
                emulator.Registers.EAX = 0x0;
                emulator.Registers.EBX = 0x0;

                emulator.Hooks.Code.Add(HookCode, null);
                emulator.Hooks.Interrupt.Add(HookInterrupt, null);


                //     Console.WriteLine();
                // Console.WriteLine(">>> Start tracing this Linux code");

                try
                {
                    emulator.Start(addr, addr + (ulong)real_code.Length);
                }

                catch
                {

                }

                GC.Collect();
                return emulator.Registers.EAX;
            }

        }

        // hook_intr
        private static void HookInterrupt(Emulator emulator, int into, object userData)
        {
            X86Registers registers = ((X86Emulator)emulator).Registers;
            byte[] buffer = new byte[256];

            if (into != 0x80)
                return;

            long eax = registers.EAX;
            long eip = registers.EIP;
            switch (eax)
            {
                default:
                    Console.WriteLine($">>> 0x{eip.ToString("x2")}: interrupt 0x{into.ToString("x2")}, EAX = 0x{eax.ToString("x2")}");
                    break;

                case 1: // sys_exit
                    Console.WriteLine($">>> 0x{eip.ToString("x2")}: interrupt 0x{into.ToString("x2")}, SYS_EXIT. quit!");
                    Console.WriteLine();
                    emulator.Stop();
                    break;

                case 4:
                    long ecx = registers.ECX;
                    long edx = registers.EDX;

                    int count = buffer.Length < edx ? buffer.Length : (int)edx;
                    emulator.Memory.Read((ulong)ecx, buffer, count);

                    // >>> 0x%x: interrupt 0x%x, SYS_WRITE. buffer = 0x%x, size = %u, content = '%s'\n
                    //   r_eip, intno, r_ecx, r_edx, buffer
                    Console.WriteLine($">>> 0x{eip.ToString("x2")}: interrupts 0x{into.ToString("x2")}, SYS_WRITE. buffer = 0x{ecx.ToString("x2")}, size = {edx.ToString("x2")}, content = {Encoding.UTF8.GetString(buffer)}");
                    break;
            }
        }

        // hook_code
        private static void HookCode(Emulator emulator, ulong address, int size, object userData)
        {
            byte[] peb_pointer_instr = new byte[] { 0x64, 0x8B, 0x15, 0x30, 0x00, 0x00, 0x00 };
            byte[] beingdebugged = new byte[] { 0x0F, 0xB6, 0x52, 0x02 };


            if (address == addr + 0x99)
            {

            }



            //    Console.WriteLine($"Tracing instruction at 0x{address.ToString("x2")}, instruction size = 0x{size.ToString("x2")}");

            byte[] tmp = new byte[size];
            long eip = ((X86Emulator)emulator).Registers.EIP;
            int count = tmp.Length < size ? tmp.Length : size; // MIN

            //       Console.Write($"*** EIP = {eip.ToString("x2")} ***: ");

            emulator.Memory.Read(address, tmp, count);

            //   for (int i = 0; i < count; i++)
            //         Console.Write(tmp[i].ToString("x2") + " ");
            //       Console.WriteLine();

            if (peb_pointer_instr.SequenceEqual(tmp))
            {
                ((X86Emulator)emulator).Registers.EIP = ((X86Emulator)emulator).Registers.EIP + peb_pointer_instr.Length;
                ((X86Emulator)emulator).Registers.EDX = (long)peb;
                //tmp = new byte[beingdebugged.Length];
                //emulator.Memory.Read(address + (ulong)peb_pointer_instr.Length, tmp, tmp.Length);
                //if (beingdebugged.SequenceEqual(tmp))
                //{
                //    Console.Write($"Skipping bad instruction at {0}", address);
                //    ((X86Emulator)emulator).Registers.EIP = ((X86Emulator)emulator).Registers.EIP + peb_pointer_instr.Length + beingdebugged.Length;
                //    ((X86Emulator)emulator).Registers.EDX = (long)0x0;
                //}
            }
        }
    }
}
