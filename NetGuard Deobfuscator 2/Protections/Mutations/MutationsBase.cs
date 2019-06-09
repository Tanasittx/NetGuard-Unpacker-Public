using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations
{
    abstract class MutationsBase
    {
        public static List<MethodDef> methods = null;
        public abstract bool Deobfuscate();
        public static ModuleDefMD ModuleDefMD;
        public static void WriteModule(string module)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] Running Module {0}", module);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
