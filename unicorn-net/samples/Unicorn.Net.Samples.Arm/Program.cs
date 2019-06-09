using System;
using Unicorn.Arm;

namespace Unicorn.Net.Samples.Arm
{
    // Similar to sample_arm.c
    public class Program
    {
        public static void HookBlock(Emulator emu, ulong address, int size, object userToken)
        {
            Console.WriteLine($">>> Tracing basic block at 0x{address.ToString("x2")}, block size = 0x{size.ToString("x2")}");
        }

        public static void HookCode(Emulator emu, ulong address, int size, object userToken)
        {
            Console.WriteLine($">>> Tracing instruction at 0x{address.ToString("x2")}, instruction size = 0x{size.ToString("x2")}");
        }

        // test_arm
        public static void TestArm()
        {
            Console.WriteLine("Emulate ARM code");

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

                emulator.Hooks.Block.Add(HookBlock, null);
                emulator.Hooks.Code.Add(HookCode, addr, addr, null);

                emulator.Start(addr, addr + (ulong)armcode.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> R0 = 0x{emulator.Registers.R0.ToString("x2")}");
                Console.WriteLine($">>> R1 = 0x{emulator.Registers.R1.ToString("x2")}");
            }
        }

        // test_thumb
        public static void TestThumb()
        {
            Console.WriteLine("Emulate THUMB code");

            using (var emulator = new ArmEmulator(ArmMode.Thumb))
            {
                ulong addr = 0x10000;

                // sub    sp, #0xc
                byte[] armcode =
                {
                    0x83, 0xb0
                };

                // Map 2mb of memory.
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                emulator.Memory.Write(addr, armcode, armcode.Length);

                emulator.Registers.SP = 0x1234;

                emulator.Hooks.Block.Add(HookBlock, null);
                emulator.Hooks.Code.Add(HookCode, addr, addr, null);

                emulator.Start(addr | 1, addr + (ulong)armcode.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> SP = 0x{emulator.Registers.SP.ToString("x2")}");
            }
        }

        public static void Main(string[] args)
        {
            TestArm();

            Console.WriteLine("==========================");

            TestThumb();

            Console.ReadLine();
        }
    }
}
