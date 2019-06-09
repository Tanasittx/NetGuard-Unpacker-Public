using System;
using ConfuserDeobfuscator.Engine.Routines.Ex.x86;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace EasyPredicateKiller.x86
{
    public class X86MethodToILConverter
    {
        public static MethodDef CreateILFromX86Method(X86Method methodToConvert)
        {
            // Find the Int32 type 
            var int32Type = methodToConvert.OriginalMethod.ReturnType;

            // Create a method with the same name as the native and append "_IL"
            var returnMethod = new MethodDefUser(new UTF8String(methodToConvert.OriginalMethod.Name + "_IL"),
                MethodSig.CreateStatic(int32Type, int32Type), MethodImplAttributes.IL | MethodImplAttributes.Managed,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig);

            returnMethod.Body = new CilBody();
            returnMethod.Body.MaxStack = 12;

            // Add 8 registers
            for (var i = 0; i < 8; i++)
                returnMethod.Body.Variables.Add(new Local(int32Type));

            var returnMethodBody = returnMethod.Body;

            // load param to stack, it later gets popped into ecx
            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldarg_0));

            // Translate each x86 Instruction to IL
            foreach (var x86Instruction in methodToConvert.Instructions)
                switch (x86Instruction.OpCode)
                {
                    case X86OpCode.MOV:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't mov value to immediate value");

                        if (x86Instruction.Operands[1] is X86RegisterOperand)
                            returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                                (x86Instruction.Operands[1] as X86RegisterOperand).Register,
                                returnMethodBody));

                        else if (x86Instruction.Operands[1] is X86ImmediateOperand)
                            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldc_I4,
                                (x86Instruction.Operands[1] as X86ImmediateOperand).Immediate));

                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register,
                            returnMethodBody));
                        break;

                    case X86OpCode.ADD:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't add value to immediate value");

                        returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));

                        if (x86Instruction.Operands[1] is X86RegisterOperand)
                            returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                                (x86Instruction.Operands[1] as X86RegisterOperand).Register,
                                returnMethodBody)); // 25 is EAX

                        else if (x86Instruction.Operands[1] is X86ImmediateOperand)
                            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldc_I4,
                                (x86Instruction.Operands[1] as X86ImmediateOperand).Immediate));

                        returnMethodBody.Instructions.Add(new Instruction(OpCodes.Add));
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;

                    case X86OpCode.DIV:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't div value to immediate value");

                        returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));

                        if (x86Instruction.Operands[1] is X86RegisterOperand)
                            returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                                (x86Instruction.Operands[1] as X86RegisterOperand).Register,
                                returnMethodBody)); // 25 is EAX

                        else if (x86Instruction.Operands[1] is X86ImmediateOperand)
                            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldc_I4,
                                (x86Instruction.Operands[1] as X86ImmediateOperand).Immediate));

                        returnMethodBody.Instructions.Add(new Instruction(OpCodes.Div_Un));
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;

                    case X86OpCode.IMUL:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't imul value to immediate value");

                        if (x86Instruction.Operands[1] is X86RegisterOperand)
                            returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                                (x86Instruction.Operands[1] as X86RegisterOperand).Register,
                                returnMethodBody)); // 25 is EAX

                        else if (x86Instruction.Operands[1] is X86ImmediateOperand)
                            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldc_I4,
                                (x86Instruction.Operands[1] as X86ImmediateOperand).Immediate));

                        if (x86Instruction.Operands[2] is X86RegisterOperand)
                            returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                                (x86Instruction.Operands[2] as X86RegisterOperand).Register,
                                returnMethodBody)); // 25 is EAX

                        else if (x86Instruction.Operands[2] is X86ImmediateOperand)
                            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldc_I4,
                                (x86Instruction.Operands[2] as X86ImmediateOperand).Immediate));

                        returnMethodBody.Instructions.Add(new Instruction(OpCodes.Mul));
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;

                    case X86OpCode.NEG:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't neg immediate value");

                        returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register,
                            returnMethodBody)); // 25 is EAX
                        returnMethodBody.Instructions.Add(new Instruction(OpCodes.Neg));
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;

                    case X86OpCode.NOT:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't not immediate value");

                        returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register,
                            returnMethodBody)); // 25 is EAX
                        returnMethodBody.Instructions.Add(new Instruction(OpCodes.Not));
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;

                    case X86OpCode.POP:
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;

                    case X86OpCode.SUB:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't sub value to immediate value");

                        returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));

                        if (x86Instruction.Operands[1] is X86RegisterOperand)
                            returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                                (x86Instruction.Operands[1] as X86RegisterOperand).Register,
                                returnMethodBody)); // 25 is EAX

                        else if (x86Instruction.Operands[1] is X86ImmediateOperand)
                            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldc_I4,
                                (x86Instruction.Operands[1] as X86ImmediateOperand).Immediate));

                        returnMethodBody.Instructions.Add(new Instruction(OpCodes.Sub));
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;

                    case X86OpCode.XOR:
                        if (x86Instruction.Operands[0] is X86ImmediateOperand)
                            throw new Exception("Can't xor value to immediate value");

                        returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));

                        if (x86Instruction.Operands[1] is X86RegisterOperand)
                            returnMethodBody.Instructions.Add(GetLdlocInstructionFromRegister(
                                (x86Instruction.Operands[1] as X86RegisterOperand).Register,
                                returnMethodBody)); // 25 is EAX

                        else if (x86Instruction.Operands[1] is X86ImmediateOperand)
                            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldc_I4,
                                ((X86ImmediateOperand) x86Instruction.Operands[1]).Immediate));

                        returnMethodBody.Instructions.Add(new Instruction(OpCodes.Xor));
                        returnMethodBody.Instructions.Add(GetStlocInstructionFromRegister(
                            (x86Instruction.Operands[0] as X86RegisterOperand).Register, returnMethodBody));
                        break;
                }

            // Load EAX
            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ldloc_0));
            // Return eax
            returnMethodBody.Instructions.Add(new Instruction(OpCodes.Ret));


            return returnMethod;
        }

        private static Instruction GetLdlocInstructionFromRegister(X86Register register, CilBody body)
        {
            switch (register)
            {
                case X86Register.EAX:
                    return new Instruction(OpCodes.Ldloc_0);

                case X86Register.ECX:
                    return new Instruction(OpCodes.Ldloc_1);

                case X86Register.EDX:
                    return new Instruction(OpCodes.Ldloc_2);

                case X86Register.EBX:
                    return new Instruction(OpCodes.Ldloc_3);

                case X86Register.ESI:
                    return new Instruction(OpCodes.Ldloc_S, body.Variables[6]);
                case X86Register.EDI:
                    return new Instruction(OpCodes.Ldloc, body.Variables[7]);
                default:
                    throw new Exception("Not implemented");
            }
        }

        private static Instruction GetStlocInstructionFromRegister(X86Register register, CilBody body)
        {
            switch (register)
            {
                case X86Register.EAX:
                    return new Instruction(OpCodes.Stloc_0);

                case X86Register.ECX:
                    return new Instruction(OpCodes.Stloc_1);

                case X86Register.EDX:
                    return new Instruction(OpCodes.Stloc_2);

                case X86Register.EBX:
                    return new Instruction(OpCodes.Stloc_3);

                case X86Register.ESI:
                    return new Instruction(OpCodes.Stloc_S, body.Variables[6]);
                case X86Register.EDI:
                    return new Instruction(OpCodes.Stloc_S, body.Variables[7]);
                default:
                    throw new Exception("Not implemented");
            }
        }
    }
}