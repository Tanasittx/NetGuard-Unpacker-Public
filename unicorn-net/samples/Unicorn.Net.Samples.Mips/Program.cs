using System;
using Unicorn.Mips;

namespace Unicorn.Net.Samples.Mips
{
    // Similar to
    // samples/sample_mips.c

    public static class Program
    {
        // test_mips_eb
        public static void TestMipsEb()
        {
            Console.WriteLine("Emulate MIPS code (big-endian)");

            using (var emulator = new MipsEmulator(MipsMode.b32 | MipsMode.BigEndian))
            {
                ulong addr = 0x10000;
                byte[] mipscode =
                {
                      0x34, 0x21, 0x34, 0x56
                };

                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                emulator.Memory.Write(addr, mipscode, mipscode.Length);

                emulator.Registers.AT = 0x6789;
                // or
                // emulator.Registers._1 = 0x6789

                emulator.Hooks.Block.Add(BlockHook, null);
                emulator.Hooks.Code.Add(CodeHook, addr, addr, null);
                emulator.Start(addr, addr + (ulong)mipscode.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> R1 = 0x{emulator.Registers.AT.ToString("x2")}");
            }
        }

        // test_mips_el
        public static void TestMipsEl()
        {
            Console.WriteLine("===========================");
            Console.WriteLine("Emulate MIPS code (little-endian)");

            using (var emulator = new MipsEmulator(MipsMode.b32 | MipsMode.LittleEndian))
            {
                ulong addr = 0x10000;
                byte[] mipscode =
                {
                      0x56, 0x34, 0x21, 0x34
                };

                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                emulator.Memory.Write(addr, mipscode, mipscode.Length);

                emulator.Registers.AT = 0x6789;
                // or
                // emulator.Registers._1 = 0x6789

                emulator.Hooks.Block.Add(BlockHook, null);
                emulator.Hooks.Code.Add(CodeHook, addr, addr, null);
                emulator.Start(addr, addr + (ulong)mipscode.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> R1 = 0x{emulator.Registers.AT.ToString("x2")}");
            }
        }

        public static void Main(string[] args)
        {
            TestMipsEb();
            TestMipsEl();
        }

        private static void BlockHook(Emulator emulator, ulong address, int size, object userToken)
        {
            Console.WriteLine($">>> Tracing basic block at 0x{address.ToString("x2")}, block size = 0x{size.ToString("x2")}");
        }

        private static void CodeHook(Emulator emulator, ulong address, int size, object userToken)
        {
            Console.WriteLine($">>> Tracing instruction at 0x{address.ToString("x2")}, instruction size = 0x{size.ToString("x2")}");
        }
    }
}
