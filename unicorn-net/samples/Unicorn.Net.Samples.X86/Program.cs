using System;
using System.Diagnostics;
using Unicorn.X86;

namespace Unicorn.Net.Samples.X86
{
    // Similar to
    // samples/sample_x86.c

    public class Program
    {
        // test_i386_invalid_mem_read
        public static void TestInvalidMemoryRead()
        {
            var addr = 0x1000000UL;
            var code = new byte[]
            {
                0x8B, 0x0D, 0xAA, 0xAA, 0xAA, 0xAA, // MOV ecx [0xaaaaaaaa]
                0x41, // INC ecx
                0x4A // DEC edx
            };

            Console.WriteLine("===================================");
            Console.WriteLine("Emulate i386 code that read from invalid memory.");

            using (var emulator = new X86Emulator(X86Mode.b32))
            {
                // Map 2MB of memory.
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                // Write machine code to be emulated.
                emulator.Memory.Write(addr, code, code.Length);

                // Initialize registers.
                emulator.Registers.ECX = 0x1234;
                emulator.Registers.EDX = 0x7890;

                // Trace all basic blocks.
                emulator.Hooks.Block.Add(BlockHook, null);
                // Trace all instructions.
                emulator.Hooks.Code.Add(CodeHook, null);

                try
                {
                    // Start emulating the machine written machine code.
                    emulator.Start(addr, addr + (ulong)code.Length);
                }
                catch (UnicornException ex)
                {
                    Debug.Assert(ex.ErrorCode == Bindings.Error.ReadUnmapped, "Unexpected error code in caught UnicornException.");
                    Console.WriteLine($"Failed to start emulator instance. -> {ex.Message}.");
                }

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> ECX = 0x{emulator.Registers.ECX.ToString("x2")}");
                Console.WriteLine($">>> EDX = 0x{emulator.Registers.EDX.ToString("x2")}");
            }
        }

        // test_i386_invalid_mem_write
        public static void TestInvalidMemoryWrite()
        {
            var addr = 0x1000000UL;
            var code = new byte[]
            {
                0x89, 0x0D, 0xAA, 0xAA, 0xAA, 0xAA, // MOV [0xaaaaaaaa] ecx
                0x41, // INC ecx
                0x4A // DEC edx
            };

            Console.WriteLine("===================================");
            Console.WriteLine("Emulate i386 code that write to invalid memory.");

            using (var emulator = new X86Emulator(X86Mode.b32))
            {
                // Map 2MB of memory.
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                // Write machine code to be emulated.
                emulator.Memory.Write(addr, code, code.Length);

                // Initialize registers.
                emulator.Registers.ECX = 0x1234;
                emulator.Registers.EDX = 0x7890;

                emulator.Hooks.Code.Add(CodeHook, null);
                emulator.Hooks.Memory.Add(MemoryEventHookType.UnmappedRead | MemoryEventHookType.UnmappedWrite, InvalidMemoryHook, null);

                // Start emulating the machine written machine code.
                emulator.Start(addr, addr + (ulong)code.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> ECX = 0x{emulator.Registers.ECX.ToString("x2")}");
                Console.WriteLine($">>> EDX = 0x{emulator.Registers.EDX.ToString("x2")}");

                // Read from memory.
                var buffer = new byte[4];
                var tmp = 0;

                emulator.Memory.Read(0xAAAAAAAA, buffer, buffer.Length);
                tmp = BitConverter.ToInt32(buffer, 0);

                Console.WriteLine($">>> Read 4 bytes from [0x{0xAAAAAAAA.ToString("x2")}] = 0x{tmp.ToString("x2")}");

                try
                {
                    emulator.Memory.Read(0xFFFFFFAA, buffer, buffer.Length);
                    tmp = BitConverter.ToInt32(buffer, 0);
                    Console.WriteLine($">>> Read 4 bytes from [0x{0xFFFFFFAA.ToString("x2")}] = 0x{tmp.ToString("x2")}");
                }
                catch
                {
                    Console.WriteLine($">>> Failed to read 4 bytes from [0x{0xFFFFFFAA.ToString("x2")}]");
                }
            }
        }

        // test_i386_context_save
        public static void TestContextSave()
        {
            var addr = 0x1000000UL;
            var code = new byte[]
            {
                0x40, // INC eax
            };

            Console.WriteLine("===================================");
            Console.WriteLine("Save/restore CPU context in opaque blob.");

            using (var emulator = new X86Emulator(X86Mode.b32))
            {
                // Map 8KB of memory for emulation.
                emulator.Memory.Map(addr, 8 * 1024, MemoryPermissions.All);
                // Write machine code to emulate.
                emulator.Memory.Write(addr, code, code.Length);

                // Initialize registers.
                emulator.Registers.EAX = 1;

                // Emulate written machine code.
                emulator.Start(addr, addr + (ulong)code.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> EAX = 0x{emulator.Registers.EAX.ToString("x2")}");

                Console.WriteLine(">>> Saving CPU context");

                using (var context = emulator.Context)
                {
                    Console.WriteLine(">>> Running emulation for the second time");

                    // Emulate machine code again.
                    emulator.Start(addr, addr + (ulong)code.Length);

                    Console.WriteLine(">>> Emulation done. Below is the CPU context");
                    Console.WriteLine($">>> EAX = 0x{emulator.Registers.EAX.ToString("x2")}");

                    emulator.Context = context;
                }

                Console.WriteLine(">>> CPU context restored. Below is the CPU context");
                Console.WriteLine($">>> EAX = 0x{emulator.Registers.EAX.ToString("x2")}");
            }
        }

        // test_i386_inout
        public static void TestInOut()
        {
            var addr = 0x1000000UL;
            var code = new byte[]
            {
                0x41, // INC ecx
                0xE4, 0x3F, // IN AL 0x3F
                0x4A, // DEC edx
                0xE6, 0x46, // OUT 0x46 AL
                0x43 // INC ebx
            };

            Console.WriteLine("===================================");
            Console.WriteLine("Emulate i386 code with IN/OUT instructions.");

            using (var emulator = new X86Emulator(X86Mode.b32))
            {
                // Map 2MB of memory.
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                // Write machine code to be emulated.
                emulator.Memory.Write(addr, code, code.Length);

                // Initialize registers.
                emulator.Registers.ECX = 0x1234;
                emulator.Registers.EDX = 0x7890;

                // Trace all basic blocks.
                emulator.Hooks.Block.Add(BlockHook, null);
                // Trace all instructions.
                emulator.Hooks.Code.Add(CodeHook, null);
                // Hook X86 IN instructions.
                emulator.Hooks.Instruction.Add(HookIn, X86Instructions.IN, null);
                // Hook X86 OUT instructions.
                emulator.Hooks.Instruction.Add(HookOut, X86Instructions.OUT, null);

                // Start emulating the machine written machine code.
                emulator.Start(addr, addr + (ulong)code.Length);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> ECX = 0x{emulator.Registers.ECX.ToString("x2")}");
                Console.WriteLine($">>> EDX = 0x{emulator.Registers.EDX.ToString("x2")}");
            }
        }

        // test_i386_loop
        public static void TestLoop()
        {
            var addr = 0x1000000UL;
            var code = new byte[]
            {
                0x41, // INC ecx
                0x4A, // DEC edx
                0xEB, // JMP self-loop
                0xFE
            };

            Console.WriteLine("===================================");
            Console.WriteLine("Emulate i386 code that emulates forever.");

            using (var emulator = new X86Emulator(X86Mode.b32))
            {
                // Map 2MB of memory.
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                // Write machine code to be emulated.
                emulator.Memory.Write(addr, code, code.Length);

                // Initialize registers.
                emulator.Registers.ECX = 0x1234;
                emulator.Registers.EDX = 0x7890;

                // Emulate code for 2 seconds, so we can exit the code since it loops forever.
                emulator.Start(addr, addr + (ulong)code.Length, TimeSpan.FromSeconds(2), 0);

                Console.WriteLine(">>> Emulation done. Below is the CPU context");
                Console.WriteLine($">>> ECX = 0x{emulator.Registers.ECX.ToString("x2")}");
                Console.WriteLine($">>> EDX = 0x{emulator.Registers.EDX.ToString("x2")}");
            }
        }

        // hook_code
        public static void CodeHook(Emulator emulator, ulong address, int size, object userData)
        {
            var eflags = ((X86Emulator)emulator).Registers.EFLAGS;

            Console.WriteLine($">>> Tracing instruction at 0x{address.ToString("x2")}, instruction size = 0x{size.ToString("x2")}.");
            Console.WriteLine($">>> --- EFLAGS is {eflags.ToString("x2")}");
        }

        // hook_block
        private static void BlockHook(Emulator emulator, ulong address, int size, object userData)
        {
            Console.WriteLine($">>> Tracing basic block at 0x{address.ToString("x2")}, block size = 0x{size.ToString("x2")}");
        }

        // hook_out
        private static void HookOut(Emulator emulator, int port, int size, int value, object userData)
        {
            var registers = ((X86Emulator)emulator).Registers;
            var eip = registers.EIP;
            var tmp = 0L;

            Console.WriteLine($">>> --- Writing to port 0x{port.ToString("x2")}, size: {size}, value: 0x{value.ToString("x2")}, address: 0x{eip.ToString("x2")}");

            switch (size)
            {
                case 1:
                    tmp = registers.AL;
                    break;
                case 2:
                    tmp = registers.AX;
                    break;
                case 4:
                    tmp = registers.EAX;
                    break;
                default:
                    return;
            }

            Console.WriteLine($">>> --- Register value = 0x{tmp.ToString("x2")}");
        }

        // hook_in
        private static int HookIn(Emulator emulator, int port, int size, object userData)
        {
            var eip = ((X86Emulator)emulator).Registers.EIP;
            Console.WriteLine($">>> --- Reading from port 0x{port.ToString("x2")}, size: {size}, address: 0x{eip.ToString("x2")}");

            switch (size)
            {
                case 1:
                    return 0xF1;
                case 2:
                    return 0xF2;
                case 4:
                    return 0xF4;
                default:
                    return 0;
            }
        }

        // hook_mem_invalid
        private static bool InvalidMemoryHook(Emulator emulator, MemoryType type, ulong address, int size, ulong value, object userData)
        {
            switch (type)
            {
                case MemoryType.WriteUnmapped:
                    Console.WriteLine($">>> Missing memory is being WRITE at 0x{address.ToString("x2")}, data size = {size}, data value = 0x{value.ToString("x2")}");

                    // Map missing memory & return true to tell unicorn we want to continue execution.
                    emulator.Memory.Map(0xAAAA0000, 2 * 1024 * 1024, MemoryPermissions.All);
                    return true;

                default:
                    return false;
            }
        }

        public static void Main(string[] args)
        {
            TestInvalidMemoryRead();
            TestInvalidMemoryWrite();
            TestInOut();
            TestContextSave();
            TestLoop();

            Console.ReadLine();
        }
    }
}
