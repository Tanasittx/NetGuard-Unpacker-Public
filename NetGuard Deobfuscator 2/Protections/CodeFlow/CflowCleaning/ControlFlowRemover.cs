using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.CflowCleaning
{
    internal class ControlFlowRemover : CodeFlowBase
    {
        internal static List<FieldDef> DoneFields = new List<FieldDef>();
        internal static FieldDef CflowFieldArray;
        internal static uint[] Decoded;
        private static readonly BlocksCflowDeobfuscator CfDeob = new BlocksCflowDeobfuscator();

        public static InstructionEmulator Inemu;

        public override void Deobfuscate()
        {
      //      WriteModule(nameof(ControlFlowRemover));
    //        Console.WriteLine("[!] Cleaning Control Flow");
            Cflow(ModuleDefMD);
            
        }

        public static void Cflow(ModuleDefMD asm)
        {
            foreach (var types in asm.GetTypes())
                foreach (var methods in types.Methods)
                {

                    if (!methods.HasBody) continue;

           //         Console.WriteLine("[!] Cleaning Method " + methods.Name + " " + methods.MDToken.ToInt32().ToString("X2"));
                    DeobfuscateCflow2(methods);
                }
        }
        public static void Melt(ModuleDefMD asm)
        {
            foreach (var types in asm.GetTypes())
                foreach (var methods in types.Methods)
                {

                    if (!methods.HasBody) continue;

                    //         Console.WriteLine("[!] Cleaning Method " + methods.Name + " " + methods.MDToken.ToInt32().ToString("X2"));
                    CleanVarMelt(methods);
                }
        }

        public static void DeobfuscateCflow2(MethodDef meth)
        {

            var blocks = new Blocks(meth);

            CfDeob.Initialize(blocks);
            Inemu = new InstructionEmulator(meth);

            //			CfDeob.Add(new LocalsSolver());
            CfDeob.Add(new De4DotClass());
           CfDeob.Add(new VariableMelting());
            CfDeob.Deobfuscate();
            blocks.RepartitionBlocks();

            //      de4dot.blocks.NetguardCflow tfhdgrs = new de4dot.blocks.NetguardCflow();
            //    de4dot.blocks.NetguardCflow.test2 = blocks;
            //  tfhdgrs.Deobfuscate(test);
            IList<Instruction> instructions;
            IList<ExceptionHandler> exceptionHandlers;
            blocks.GetCode(out instructions, out exceptionHandlers);
            DotNetUtils.RestoreBody(meth, instructions, exceptionHandlers);
        }
        public static void CleanVarMelt(MethodDef meth)
        {

            var blocks = new Blocks(meth);

            CfDeob.Initialize(blocks);
            Inemu = new InstructionEmulator(meth);

            //			CfDeob.Add(new LocalsSolver());
            CfDeob.Add(new VariableMelting());
            //   CfDeob.Add(new VariableMelting());
            CfDeob.Deobfuscate();
            blocks.RepartitionBlocks();

            //      de4dot.blocks.NetguardCflow tfhdgrs = new de4dot.blocks.NetguardCflow();
            //    de4dot.blocks.NetguardCflow.test2 = blocks;
            //  tfhdgrs.Deobfuscate(test);
            IList<Instruction> instructions;
            IList<ExceptionHandler> exceptionHandlers;
            blocks.GetCode(out instructions, out exceptionHandlers);
            DotNetUtils.RestoreBody(meth, instructions, exceptionHandlers);
        }
    }
}
