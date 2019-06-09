using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Basic
{
    internal class SizeOfCleaner : MutationsBase
    {
        public override bool Deobfuscate()
        {
            return Clean();
        }
        private static bool Clean()
        {
            bool modified = false;
            foreach (MethodDef method in methods)
            {
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    Instruction instruction = method.Body.Instructions[i];
                    if (instruction.OpCode != OpCodes.Sizeof) continue;
                    ITypeDefOrRef type = (ITypeDefOrRef)instruction.Operand;
                    if (type.FullName == "-") continue;
                    int size = GetSize(type);
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = size;
                    modified = true;
                }
            }
            return modified;
        }
        private static int GetSize(ITypeDefOrRef refOrDef, bool topmost = true)
        {

            int ret = 0;
            TypeDef target = refOrDef.ResolveTypeDef();

            if ((typeof(object).Assembly.ManifestModule.Name).Contains(target.Module.Assembly.Name))
            {
                return System(refOrDef.FullName);
            }

            if (!topmost && target.BaseType != null && target.BaseType.Name == "ValueType")
            {
                //ret += 1;
            }

            foreach (FieldDef fd in target.Fields)
            {
                if (fd.FieldType.TryGetTypeDef() != null)
                {
                    int size = GetSize(fd.FieldType.ToTypeDefOrRef(), false);
                    ret += size;
                }
                else
                {
                    int size = System(fd.FieldSig.Type.FullName);
                    ret += size;
                }
            }

            if (ret % 4 != 0)
            {
                int rem = ret % 4;
                ret -= rem;
                ret += 4;
            }

            return ret;
        }
        private static int System(string instr)
        {

            string s = instr;
            switch (s)
            {
                case "System.Byte":
                    return 1;

                    break;
                case "System.Int16":
                case "System.UInt16":
                case "System.Char":
                    return 2;

                    break;
                case "System.Int32":
                case "System.UInt32":
                case "System.Single":
                    return 4;

                    break;
                case "System.IntPtr":
                case "System.Type":
                case "System.String":
                case "System.UIntPtr":
                    return 4;

                    break;

                case "System.Double":
                case "System.Int64":
                case "System.UInt64":
                case "System.DateTime":
                case "System.TimeSpan":
                    return 8;

                    break;
                case "System.Guid":
                    return 16;
                    break;
                case "System.Decimal":
                    return 16;

                    break;
                case "System.Boolean":
                    return 1;


                    break;
                case "System.SByte":
                    return 1;

                    break;

                default:
                    throw new Exception();
            }

        }
    }
}
