using System.Linq;
using ConfuserDeobfuscator.Engine.Routines.Ex.x86;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class Call
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction, MethodDef method)
        {
            if (instruction.Operand is MethodDef && ((MethodDef) instruction.Operand).IsNative)
            {
                //this is really only for confuserex x86 methods 

                var abc2 = method.Module.ResolveToken(((MethodDef) instruction.Operand).MDToken.ToInt32());
                var x86 = new X86Method(abc2 as MethodDef);
                var value = valueStack.CallStack.Pop();
                var abc = x86.Execute((int) (uint) value);
                valueStack.CallStack.Push(abc);
            }
            else
            {
                var instructionOperand = instruction.Operand as IMethodDefOrRef;
                var resolved = instructionOperand.ResolveMethodDef();
                if (resolved.Module != method.Module)
                    if (resolved.IsStatic)
                    {
                        var mod = typeof(string).Module;
                        var asmCall = mod.ResolveMethod(resolved.MDToken.ToInt32());
                        int pushes;
                        int pops;
                        instruction.CalculateStackUsage(out pushes, out pops);
                        var paramsObjects = new dynamic[pops];
                        for (var i = 0; i < pops; i++) paramsObjects[i] = valueStack.CallStack.Pop();
                        paramsObjects = paramsObjects.Reverse().ToArray();
                        try
                        {
                            if (pushes != 0)
                            {
                                var av = asmCall.Invoke(null, paramsObjects);
                                valueStack.CallStack.Push(av);
                            }
                            else
                            {
                                asmCall.Invoke(null, paramsObjects);
                            }
                        }
                        catch
                        {
                        }
                    }
            }
        }
    }
}