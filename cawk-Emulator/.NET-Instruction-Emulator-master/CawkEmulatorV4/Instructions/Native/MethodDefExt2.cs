using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.PE;

namespace EasyPredicateKiller
{
    public static class MethodDefExt2
    {
        private static IntPtr moduleHandle = IntPtr.Zero;


        public static ModuleDefMD OriginalMD { get; set; }

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        public static byte[] ReadBodyFromRva(this MethodDef method)
        {
            var stream = OriginalMD.Metadata.PEImage.CreateReader();
            var offset = OriginalMD.Metadata.PEImage.ToFileOffset(method.RVA);
            var nextMethod2 = OriginalMD.TablesStream.TryReadMethodRow(method.Rid + 1, out var nextMethod);
            var size = OriginalMD.Metadata.PEImage.ToFileOffset((RVA) nextMethod.RVA) - offset;
            var buff = new byte[500];

          stream.Position = (uint) offset + 20;
            stream.ReadBytes(buff, 0, buff.Length);
            return buff;
        }

        // Not working for all tokens !
        /*public static IEnumerable<Tuple<Instruction, MethodDef>> FindAllReferences(this MethodDef mDef)
        {
            foreach (
                var method in
                    mDef.Module.Assembly.FindMethods(x => x.HasBody))
            {
                for (var i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (method.Body.Instructions[i].OpCode == OpCodes.Call)
                        if ((method.Body.Instructions[i].Operand as MethodSpec) != null)
                        {
                            var a = method.Body.Instructions[i].Operand as MethodSpec;
                            if (a.Method == mDef)
                                yield return Tuple.Create(method.Body.Instructions[i], method);
                        }
                        else if ((method.Body.Instructions[i].Operand as MethodDef) != null)
                        {
                            var a = method.Body.Instructions[i].Operand as MethodDef;
                            if (a == mDef)
                                yield return Tuple.Create(method.Body.Instructions[i], method);
                        }
                        else if ((method.Body.Instructions[i].Operand as MemberRef != null))
                        {
                            var a = method.Body.Instructions[i].Operand as MemberRef;
                            if (a.ResolveMethod() != null)
                                yield return Tuple.Create(method.Body.Instructions[i], method);
                        }
                }
            }
        }*/

        // Somehow the other method skipped some references
        // Now we resolve every method by its token and check the instructions, it works this way (but isn't as clean as the other)
        public static IEnumerable<Instruction> FindAllReferences(this MethodDef mDef, ModuleDefMD module)
        {
            var returnList = new List<Instruction>();

            var totalMethods2 = new List<MethodDef>();
            // TODO: Read count dynamically
            for (var i = 1; i < 0x10000; i++)
            {
                var resolved = module.ResolveMethod((uint) i);
                if (resolved == null)
                    continue;

                if (resolved.HasBody)
                    totalMethods2.Add(resolved);
            }

            foreach (
                var method in
                totalMethods2)
            {
                if (!method.HasBody)
                    continue;

                for (var i = 0; i < method.Body.Instructions.Count; i++)
                    if (method.Body.Instructions[i].OpCode == OpCodes.Call)
                    {
                        var currentMethod = method.Body.Instructions[i].Operand as MethodDef;

                        if (currentMethod != null && currentMethod.MDToken.ToInt32() == mDef.MDToken.ToInt32())
                            returnList.Add(method.Body.Instructions[i]);

                        var currentMethodSpec = method.Body.Instructions[i].Operand as MethodSpec;
                        if (currentMethodSpec != null && currentMethodSpec.MDToken.ToInt32() == mDef.MDToken.ToInt32())
                            returnList.Add(method.Body.Instructions[i]);
                    }
            }

            return returnList;
        }
    }
}