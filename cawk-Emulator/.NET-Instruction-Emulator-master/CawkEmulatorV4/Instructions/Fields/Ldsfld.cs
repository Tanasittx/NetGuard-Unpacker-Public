using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4.Instructions.Fields
{
    internal class Ldsfld
    {
        public static void Emulate(ValueStack valueStack, Instruction instruction)
        {
            if (instruction.Operand.ToString().Contains("EmptyTypes"))
            {
                valueStack.CallStack.Push(new Type[0]);
            }
            else
            {
                if (instruction.Operand is FieldDef)
                {
                    var abc = instruction.Operand as FieldDef;
                    if (valueStack.Fields.ContainsKey(abc))
                    {
                        if (valueStack.Fields[abc] is byte[])
                        {
                            
                        }

                        valueStack.CallStack.Push(valueStack.Fields[abc]);
                    }
                    else
                    {
                        var fied = (FieldDef) instruction.Operand;
                        if (fied.FieldType.IsValueType)
                            valueStack.CallStack.Push(0);
                        else
                            valueStack.CallStack.Push(null);
                    }
                }

                else
                {
                    valueStack.CallStack.Push(null);
                }
            }
        }
    }
}