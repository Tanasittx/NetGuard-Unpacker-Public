using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections
{
    public abstract class Base
    {
        
        public abstract void Deobfuscate();
        public static ModuleDefMD ModuleDefMD;
        public static bool NativePacker = false;
        public static List<IMethod> methodsToRemove = new List<IMethod>();
        public static void WriteModule(string module)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] Running Module {0}", module);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
