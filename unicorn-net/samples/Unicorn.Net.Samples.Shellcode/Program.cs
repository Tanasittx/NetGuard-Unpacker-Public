using System;
using System.Text;
using Unicorn.X86;

namespace Unicorn.Net.Samples.Shellcode
{
    // Similar to
    // samples/sample_shellcode.c

    public class Program
    {
        // test_i386
        public static void Test()
        {
            ulong addr = 0x1000000;
            ulong esp = addr + 0x200000;
            byte[] code =
            {
                0xeb, 0x1c, 0x5a, 0x89, 0xd6, 0x8b, 0x02, 0x66, 0x3d,
                0xca, 0x7d, 0x75, 0x06, 0x66, 0x05, 0x03, 0x03, 0x89,
                0x02, 0xfe, 0xc2, 0x3d, 0x41, 0x41, 0x41, 0x41, 0x75,
                0xe9, 0xff, 0xe6, 0xe8, 0xdf, 0xff, 0xff, 0xff, 0x31,
                0xd2, 0x6a, 0x0b, 0x58, 0x99, 0x52, 0x68, 0x2f, 0x2f,
                0x73, 0x68, 0x68, 0x2f, 0x62, 0x69, 0x6e, 0x89, 0xe3,
                0x52, 0x53, 0x89, 0xe1, 0xca, 0x7d, 0x41, 0x41, 0x41,
                0x41, 0x41, 0x41, 0x41, 0x41
            };

            Console.WriteLine("Emulate i386 code");

            using (var emulator = new X86Emulator(X86Mode.b32))
            {
                emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);
                emulator.Memory.Write(addr, code, code.Length);

                emulator.Registers.ESP = (long)esp;

                emulator.Hooks.Code.Add(HookCode, null);
                emulator.Hooks.Interrupt.Add(HookInterrupt, null);

                Console.WriteLine();
                Console.WriteLine(">>> Start tracing this Linux code");

                emulator.Start(addr, addr + (ulong)code.Length);

                Console.WriteLine();
                Console.WriteLine(">>> Emulation done.");
            }
        }

        // hook_intr
        private static void HookInterrupt(Emulator emulator, int into, object userData)
        {
            var registers = ((X86Emulator)emulator).Registers;
            var buffer = new byte[256];

            if (into != 0x80)
                return;

            var eax = registers.EAX;
            var eip = registers.EIP;
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
                    var ecx = registers.ECX;
                    var edx = registers.EDX;

                    var count = buffer.Length < edx ? buffer.Length : (int)edx;
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
            Console.WriteLine($"Tracing instruction at 0x{address.ToString("x2")}, instruction size = 0x{size.ToString("x2")}");

            var tmp = new byte[16];
            var eip = ((X86Emulator)emulator).Registers.EIP;
            var count = tmp.Length < size ? tmp.Length : size; // MIN

            Console.Write($"*** EIP = {eip.ToString("x2")} ***: ");

            emulator.Memory.Read(address, tmp, count);
            for (int i = 0; i < count; i++)
                Console.Write(tmp[i].ToString("x2") + " ");
            Console.WriteLine();
        }

        public static void Main(string[] args)
        {
            Test();

            Console.ReadLine();
        }
    }
}
