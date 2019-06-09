using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.De4Dot
{
    class De4DotCleaner:MutationsBase
    {
        private static readonly BlocksCflowDeobfuscator CfDeob = new BlocksCflowDeobfuscator();
        private static FieldDef field;

        public override bool Deobfuscate()
        {
       //     Console.WriteLine("[!] Running Module Through De4Dot");
            run(ModuleDefMD);
            return false;
        }

        public static void run(ModuleDefMD module)
        {
            foreach (var typeDef in module.GetTypes())
                foreach (var method in typeDef.Methods)
                {
                    if (!method.HasBody) continue;
                    DeobfuscateCflow(method);
                }
        }

        public static void DeobfuscateCflow(MethodDef meth)
        {
           
                var blocks = new Blocks(meth);
                var test = blocks.MethodBlocks.GetAllBlocks();
                CfDeob.Initialize(blocks);
                //	    CfDeob.Add(new VariableMelting());
                CfDeob.Deobfuscate();
                blocks.RepartitionBlocks();
                //  blocks.RepartitionBlocks();


                IList<Instruction> instructions;
                IList<ExceptionHandler> exceptionHandlers;
                blocks.GetCode(out instructions, out exceptionHandlers);
                DotNetUtils.RestoreBody(meth, instructions, exceptionHandlers);
            }
        
    }
}
