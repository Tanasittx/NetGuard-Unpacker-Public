using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Misc
{
    internal class CallVirt
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction, MethodDef method)
        {
            var instructionOperand = instruction.Operand as IMethodDefOrRef;
            var resolved = instructionOperand.ResolveMethodDef();
            if (resolved.Module != method.Module)
                if (!resolved.IsStatic)
                {
                    var mod = typeof(string).Module;
                    var asmCall = mod.ResolveMethod(resolved.MDToken.ToInt32());
                    int pushes;
                    int pops;
                    instruction.CalculateStackUsage(out pushes, out pops);
                    pops--;
                    var paramsObjects = new dynamic[pops];

                    for (var i = 0; i < pops; i++) paramsObjects[i] = valueStack.CallStack.Pop();

                    var objectVal = valueStack.CallStack.Pop();
                    try
                    {
                        if (pushes != 0)
                        {
                            paramsObjects = paramsObjects.Reverse().ToArray();
                            var av = asmCall.Invoke(objectVal, paramsObjects);
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