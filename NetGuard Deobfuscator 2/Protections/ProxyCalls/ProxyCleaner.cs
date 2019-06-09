using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.ProxyCalls
{
    class ProxyCleaner : Base
    {
        public static ProxyBase[] proxyModules { get; set; } =
          {
            new ProxyCalls.Delegates.Remover(),
          new ProxyCalls.Calli.Remover(),
              new ProxyCalls.Calli.Remover(),
        };
        public override void Deobfuscate()
        {
            ProxyBase.methods = (from type in ModuleDefMD.GetTypes()
                                 where type.HasMethods
                                 from method in type.Methods
                                 where method.HasBody && method.Body.Instructions.Count > 5
                                 select method).ToList();
            ProxyBase.ModuleDefMD = ModuleDefMD;
            var @base2 = new Mutations.De4Dot.De4DotCleaner();
            Protections.CodeFlow.CflowCleaning.ControlFlowRemover.Melt(ModuleDefMD);
            for (int i = 0; i < 3; i++)
            {
                 
                foreach (ProxyBase @base in proxyModules)
                {
                    base2.Deobfuscate();
                    @base.Deobfuscate();
                }
            }
        }
    }
}
