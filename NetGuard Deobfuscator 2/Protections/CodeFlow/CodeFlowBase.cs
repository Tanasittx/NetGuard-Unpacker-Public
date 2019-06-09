using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow
{
    abstract class CodeFlowBase
    {
        public abstract void Deobfuscate();
        public static ModuleDefMD ModuleDefMD = Protections.Base.ModuleDefMD;
        public static void WriteModule(string module)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] Running Module {0}", module);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
