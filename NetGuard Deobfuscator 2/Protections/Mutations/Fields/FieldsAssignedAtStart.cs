using CawkEmulatorV4;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Mutations.Fields
{
    class FieldsAssignedAtStart:MutationsBase
    {
        public override bool Deobfuscate()
        {
            return Clean();
        }
        private static bool Clean()
        {
            var modified = false;
            foreach(MethodDef method in methods)
            {
                string fieldName = null;
                int? fieldValue = null;
                if (!method.HasBody) continue;
                for (var i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (fieldName == null)
                    {

                        if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand is MethodDef)
                            if (method.Body.Instructions[i].Operand.ToString().ToLower().Contains("int32") &&
                                method.Body.Instructions[i].Operand.ToString().ToLower().Contains("void") && method.Body.Instructions[i]
                                    .Operand.ToString().Contains(method.DeclaringType.Name))
                                if (method.Body.Instructions[i - 1].IsLdcI4())
                                    if (method.Body.Instructions[i].Operand.ToString().Contains("ErrorWrapperApplicationException"))
                                    {
                                    }
                                    else
                                    {
                                        var setMethod = (MethodDef)method.Body.Instructions[i].Operand;
                                        if (!setMethod.HasBody) continue;
                                        if (setMethod.Body.Instructions[setMethod.Body.Instructions.Count - 2]
                                                .OpCode == OpCodes.Stsfld)
                                        {
                                            
                                            fieldValue = GetFieldValue(setMethod, ModuleDefMD, method.Body.Instructions[i - 1].GetLdcI4Value(),
                                                out fieldName);
                                            method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                            method.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                                            if (fieldValue == null)
                                            {
                                                throw new Exception("Field Value Is Empty!!");
                                                
                                            }
                                        }
                                    }
                    }
                    else
                    {
                        if (method.Body.Instructions[i].Operand is FieldDef)
                            if (method.Body.Instructions[i].OpCode == OpCodes.Ldsfld &&
                                method.Body.Instructions[i].Operand.ToString().Contains("System.Int32") && method.Body.Instructions[i]
                                    .Operand.ToString().Contains(fieldName))
                                if (fieldValue != null)
                                {
                                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                                    method.Body.Instructions[i].Operand = fieldValue;
                                    modified = true;
                                }
                    }
                }
            }
            return modified;
        }
        public static int? GetFieldValue(MethodDef method, ModuleDefMD module, int argValue, out string fieldName)
        {
            fieldName = null;
            Base.methodsToRemove.Add(method);
            var insemu = new Emulation(method);
            insemu.ValueStack.Parameters[method.Parameters[0]] = argValue;
            insemu.Emulate();
            fieldName = insemu.ValueStack.Fields.First().Key.Name;
            insemu.Emulate();
            var value = (int)insemu.ValueStack.Fields.First().Value;
            return value;

        }
    }
}
