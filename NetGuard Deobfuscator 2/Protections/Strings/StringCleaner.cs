using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Strings
{
    class StringCleaner:Base
    {
        public static StringBase[] stringModules { get; set; } =
        {
            new Strings.Initalise.DecryptInitialByteArray(),
            new Strings.Initalise.LZMA(),
            new Strings.Initalise.FieldValueGrabber(),
            new Strings.DecryptStrings.DecryptStrings(),
        };
        public override void Deobfuscate()
        {
            StringBase.methods = (from type in ModuleDefMD.GetTypes()
                                     where type.HasMethods
                                     from method in type.Methods
                                     where method.HasBody && method.Body.Instructions.Count > 5
                                     select method).ToList();
            if (!Protections.Base.NativePacker)
            {
                Console.WriteLine("Current string protection on this file is not supported im currently working on a fix but not many files have this protection");
            //    return;
            }
            StringBase.ModuleDefMD = ModuleDefMD;
            foreach(StringBase @base in stringModules)
            {
                @base.Deobfuscate();
            }
        }
    }
}
