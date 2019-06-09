using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CleanUp
{
    class AntiDnspyClean : Base
    {
        public override void Deobfuscate()
        {
    //        WriteModule("Anti De4Dot");
      //      Remove();
            FixAntiDnSpy(ModuleDefMD);
            
        }
        private static bool FixAntiDnSpy(ModuleDefMD md)
        {
            CustomAttribute asmTitle = md.Assembly.CustomAttributes.Find("System.Reflection.AssemblyTitleAttribute");
            if (asmTitle != null)
            {
                if (asmTitle.ConstructorArguments[0].Value.ToString() != md.Assembly.Name)
                {
                    asmTitle.ConstructorArguments[0] = new CAArgument(md.CorLibTypes.String, md.Assembly.Name);
                    return true;
                }
            }
            return false;
        }
        public static void Remove()
        {
            for (var i = 0; i < ModuleDefMD.Types.Count; i++)
            {
                var type = ModuleDefMD.Types[i];
                if (type.BaseType == null || !type.BaseType.FullName.Contains("System.Attribute")) continue;
              
                ModuleDefMD.Types.Remove(type);
                i--;
            }
        }
    }
}
