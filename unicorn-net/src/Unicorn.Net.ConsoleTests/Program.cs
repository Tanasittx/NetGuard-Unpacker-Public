using System;
using Unicorn.Arm;
using Unicorn.Mips;
using Unicorn.X86;

namespace Unicorn.ConsoleTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Unicorn version - " + Version.Current);

            using (var emulator = new MipsEmulator(MipsMode.b32 | MipsMode.BigEndian))
            {
                ulong addr = 0x10000;
                byte[] mipscode =
                {
                      0x34, 0x21, 0x34, 0x56
                };
                
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                emulator.Memory.Write(addr, mipscode, mipscode.Length);

                emulator.Registers._1 = 0x6789;

                emulator.Hooks.Code.Add(CodeHook, null);
                emulator.Start(addr, addr + (ulong)mipscode.Length);

                Console.WriteLine("{0}", emulator.Registers._1);
            }

            using (var emulator = new ArmEmulator(ArmMode.Arm))
            {
                ulong addr = 0x10000;

                // mov r0, #0x37; sub r1, r2, r3
                byte[] armcode =
                {
                    0x37, 0x00, 0xa0, 0xe3, 0x03, 0x10, 0x42, 0xe0
                };

                // Map 2mb of memory.
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                emulator.Memory.Write(addr, armcode, armcode.Length);

                emulator.Registers.R0 = 0x1234;
                emulator.Registers.R2 = 0x6789;
                emulator.Registers.R3 = 0x3333;

                emulator.Hooks.Block.Add((emu, address, size, userToken) =>
                {
                    Console.WriteLine($">>> Tracing basic block at 0x{address.ToString("x2")}, block size = 0x{size.ToString("x2")}");
                }, null);

                emulator.Hooks.Code.Add((emu, address, size, userToken) =>
                {
                    Console.WriteLine($">>> Tracing instruction at 0x{address.ToString("x2")}, instruction size = 0x{size.ToString("x2")}");
                }, null);

                emulator.Start(addr, addr + (ulong)armcode.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> R0 = 0x{emulator.Registers.R0.ToString("x2")}");
                Console.WriteLine($">>> R1 = 0x{emulator.Registers.R1.ToString("x2")}");
            }

            /*
            using (var emulator = new X86Emulator(X86Mode.b32))
            {
                ulong addr = 0x1000000;
                byte[] x86code =
                {
                    0x41, // INC ECX
                    0x4a  // DEC EDX
                };

                var ecx = 0x1234;
                var edx = 0x7890;

                // Map 2mb of memory.
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);

                var handle = emulator.Hooks.Code.Add(CodeHook, null);

                // Capture context.
                Console.WriteLine("-> Capturing context...");
                using (var context = emulator.Context)
                {
                    emulator.Registers.ECX = ecx;
                    emulator.Registers.EDX = edx;

                    emulator.Memory.Write(addr, x86code, x86code.Length);

                    emulator.Start(addr, addr + (ulong)x86code.Length);

                    Console.WriteLine($"ECX = {emulator.Registers.ECX}");
                    Console.WriteLine($"EDX = {emulator.Registers.EDX}");


                    Console.WriteLine("-> Restoring context...");

                    // Restore captured context.
                    emulator.Context = context;
                }

                Console.WriteLine($"ECX = {emulator.Registers.ECX}");
                Console.WriteLine($"EDX = {emulator.Registers.EDX}");
            }
            */

            Console.ReadLine();
        }

        private static void CodeHook(Emulator emulator, ulong address, int size, object userData)
        {
            var casted = (X86Emulator)emulator;
            var eflags = casted.Registers.EFLAGS;

            Console.WriteLine($"-> Tracing instruction at 0x{address.ToString("x2")} of size 0x{size.ToString("x2")}.");
            Console.WriteLine($"-> EFLAGS = {eflags.ToString("x2")}");
        }
    }
}
