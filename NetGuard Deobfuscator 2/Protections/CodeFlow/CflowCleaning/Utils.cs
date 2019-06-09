using de4dot.blocks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.CflowCleaning
{
    public class Utilis
    {

        public static bool IsValidBlock(Block block, bool NativeMethods = true, bool Arrays = false)
        {
            return IsValidBlock(block, 0, block.Instructions.Count, NativeMethods, Arrays);
        }
        public static bool IsValidBlock(Block block, int start, int end, bool NativeMethods = true, bool Arrays = false)
        {
            for (int i = start; i < end; i++)
            {
                var instr = block.Instructions[i].Instruction;
                if (IsValidInstruction(instr))
                    continue;
                if (NativeMethods && IsNativeCallInstruction(instr))
                    continue;
                if (Arrays && IsArrayInstrucion(instr))
                    continue;
                return false;
            }
            return true;
        }

        public static bool IsElementTypeValid(ElementType type)
        {
            switch (type)
            {
                case ElementType.Boolean:
                case ElementType.Char:
                case ElementType.I1:
                case ElementType.U1:
                case ElementType.I2:
                case ElementType.U2:
                case ElementType.I4:
                case ElementType.U4:
                case ElementType.I8:
                case ElementType.U8:
                case ElementType.R4:
                case ElementType.R8:
                case ElementType.String:
                    return true;
            }
            return false;
        }
        public static bool AreMethodParametersValid(MethodDef method)
        {
            foreach (var parameter in method.Parameters)
                if (!IsElementTypeValid(parameter.Type.ElementType))
                    return false;
            return true;
        }
        public static bool IsMethodReturnTypeValid(MethodDef method)
        {
            return IsElementTypeValid(method.ReturnType.ElementType);
        }

        public static bool IsValidInstructionConstant(Instruction instr)
        {
            return IsConvInstruction(instr) || IsConstantInstruction(instr)
               || IsMathInstruction(instr) || IsEngineInstruction(instr);
        }
        public static bool IsValidInstruction(Instruction instr)
        {
            return IsConvInstruction(instr) || IsLocalInstruction(instr) || IsArgInstruction(instr) || IsConstantInstruction(instr)
                || IsMathInstruction(instr) || IsEngineInstruction(instr);
        }
        public static bool IsArrayInstrucion(Instruction instr)
        {
            switch (instr.OpCode.Code)
            {
                case Code.Ldelem:
                case Code.Ldelem_I:
                case Code.Ldelem_I1:
                case Code.Ldelem_I2:
                case Code.Ldelem_I4:
                case Code.Ldelem_I8:
                case Code.Ldelem_R4:
                case Code.Ldelem_R8:
                case Code.Ldelem_Ref:
                case Code.Ldelem_U1:
                case Code.Ldelem_U2:
                case Code.Ldelem_U4:
                case Code.Ldelema:

                case Code.Ldind_I:
                case Code.Ldind_I1:
                case Code.Ldind_I2:
                case Code.Ldind_I4:
                case Code.Ldind_I8:
                case Code.Ldind_R4:
                case Code.Ldind_R8:
                case Code.Ldind_Ref:
                case Code.Ldind_U1:
                case Code.Ldind_U2:
                case Code.Ldind_U4:
                case Code.Ldlen:

                case Code.Starg:
                case Code.Starg_S:
                case Code.Stelem:
                case Code.Stelem_I:
                case Code.Stelem_I1:
                case Code.Stelem_I2:
                case Code.Stelem_I4:
                case Code.Stelem_I8:
                case Code.Stelem_R4:
                case Code.Stelem_R8:
                case Code.Stelem_Ref:

                case Code.Stfld:
                case Code.Stind_I:
                case Code.Stind_I1:
                case Code.Stind_I2:
                case Code.Stind_I4:
                case Code.Stind_I8:
                case Code.Stind_R4:
                case Code.Stind_R8:
                case Code.Stind_Ref:

                case Code.Newobj:
                case Code.Newarr:
                    return true;
            }
            return false;
        }
        public static bool IsConvInstruction(Instruction instr)
        {
            switch (instr.OpCode.Code)
            {
                case Code.Conv_I:
                case Code.Conv_I1:
                case Code.Conv_I2:
                case Code.Conv_I4:
                case Code.Conv_I8:
                case Code.Conv_Ovf_I:
                case Code.Conv_Ovf_I_Un:
                case Code.Conv_Ovf_I1:
                case Code.Conv_Ovf_I1_Un:
                case Code.Conv_Ovf_I2:
                case Code.Conv_Ovf_I2_Un:
                case Code.Conv_Ovf_I4:
                case Code.Conv_Ovf_I4_Un:
                case Code.Conv_Ovf_I8:
                case Code.Conv_Ovf_I8_Un:
                case Code.Conv_Ovf_U:
                case Code.Conv_Ovf_U_Un:
                case Code.Conv_Ovf_U1:
                case Code.Conv_Ovf_U1_Un:
                case Code.Conv_Ovf_U2:
                case Code.Conv_Ovf_U2_Un:
                case Code.Conv_Ovf_U4:
                case Code.Conv_Ovf_U4_Un:
                case Code.Conv_Ovf_U8:
                case Code.Conv_Ovf_U8_Un:
                case Code.Conv_R_Un:
                case Code.Conv_R4:
                case Code.Conv_R8:
                case Code.Conv_U:
                case Code.Conv_U1:
                case Code.Conv_U2:
                case Code.Conv_U4:
                case Code.Conv_U8:
                    return true;
            }
            return false;
        }
        public static bool IsLocalInstruction(Instruction instr)
        {
            return instr.IsStloc() || instr.IsLdloc() || instr.OpCode == OpCodes.Ldloca || instr.OpCode == OpCodes.Ldloca_S;
        }
        public static bool IsArgInstruction(Instruction instr)
        {
            return instr.IsStarg() || instr.IsLdarg();
        }
        public static bool IsConstantInstruction(Instruction instr)
        {
            return instr.IsLdcI4() || instr.OpCode == OpCodes.Ldstr || instr.OpCode == OpCodes.Ldc_I8 ||
                instr.OpCode == OpCodes.Ldc_R4 || instr.OpCode == OpCodes.Ldc_R8 || instr.OpCode == OpCodes.Ldnull
                || instr.OpCode == OpCodes.Ldtoken;
        }
        public static bool IsMathInstruction(Instruction instr)
        {
            switch (instr.OpCode.Code)
            {
                case Code.Add:
                case Code.Add_Ovf:
                case Code.Add_Ovf_Un:
                case Code.Div:
                case Code.Div_Un:
                case Code.Mul:
                case Code.Mul_Ovf:
                case Code.Mul_Ovf_Un:
                case Code.Not:
                case Code.Shl:
                case Code.Shr:
                case Code.Shr_Un:
                case Code.Sub:
                case Code.Sub_Ovf:
                case Code.Sub_Ovf_Un:
                case Code.Xor:
                case Code.And:
                case Code.Rem:
                case Code.Rem_Un:
                case Code.Ceq:
                case Code.Cgt:
                case Code.Cgt_Un:
                case Code.Clt:
                case Code.Clt_Un:
                case Code.Neg:
                case Code.Or:
                    return true;
            }
            return false;
        }
        public static bool IsEngineInstruction(Instruction instr)
        {
            return instr.OpCode == OpCodes.Nop || instr.OpCode == OpCodes.Dup || instr.OpCode == OpCodes.Pop;
        }
        public static bool IsNativeCallInstruction(Instruction instr)
        {
            if (instr.OpCode != OpCodes.Call)
                return false;
            var method = instr.Operand as MethodDef;
            if (method == null || !method.IsNative)
                return false;
            if (method.ReturnType.ElementType != ElementType.I4)
                return false;
            foreach (var parameter in method.Parameters)
                if (parameter.Type.ElementType != ElementType.I4)
                    return false;
            return true;
        }


        public static bool IsLdloc(Instr instr) => IsLdloc(instr.Instruction);
        public static bool IsLdloc(Instruction instr)
        {
            switch (instr.OpCode.Code)
            {
                case Code.Ldloc:
                case Code.Ldloc_0:
                case Code.Ldloc_1:
                case Code.Ldloc_2:
                case Code.Ldloc_3:
                case Code.Ldloc_S:
                case Code.Ldloca:
                case Code.Ldloca_S:
                    return true;
            }
            return false;
        }
        public static bool IsStloc(Instr instr) => IsStloc(instr.Instruction);
        public static bool IsStloc(Instruction instr)
        {
            switch (instr.OpCode.Code)
            {
                case Code.Stloc:
                case Code.Stloc_0:
                case Code.Stloc_1:
                case Code.Stloc_2:
                case Code.Stloc_3:
                case Code.Stloc_S:
                    return true;
            }
            return false;
        }
        public static bool IsLoc(Instr instr) => IsLoc(instr.Instruction);
        public static bool IsLoc(Instruction instr) => IsLdloc(instr) || IsStloc(instr);

        public static bool IsPtrElementType(TypeSig sig, ElementType type)
        {
            if (sig == null)
                return false;
            if (sig.ElementType != ElementType.Ptr)
                return false;
            if (sig.Next == null || sig.Next.ElementType != type)
                return false;
            return true;
        }

        public static int GetElementTypeSize(ElementType type)
        {
            switch (type)
            {
                case ElementType.Boolean:
                case ElementType.I1:
                case ElementType.U1:
                    return 1;
                case ElementType.Char:
                case ElementType.I2:
                case ElementType.U2:
                    return 2;
                case ElementType.I4:
                case ElementType.U4:
                case ElementType.R4:
                    return 4;
                case ElementType.I8:
                case ElementType.U8:
                case ElementType.R8:
                    return 8;
            }
            return -1;
        }

        public static TypeSig ElementTypeToTypeSig(ModuleDef module, ElementType type)
        {
            var libTypes = module.CorLibTypes;
            switch (type)
            {
                case ElementType.Boolean:
                    return libTypes.Boolean;
                case ElementType.Char:
                    return libTypes.Char;
                case ElementType.I1:
                    return libTypes.SByte;
                case ElementType.U1:
                    return libTypes.Byte;
                case ElementType.I2:
                    return libTypes.Int16;
                case ElementType.U2:
                    return libTypes.UInt16;
                case ElementType.I4:
                    return libTypes.Int32;
                case ElementType.U4:
                    return libTypes.UInt32;
                case ElementType.I8:
                    return libTypes.Int64;
                case ElementType.U8:
                    return libTypes.UInt64;
                case ElementType.R4:
                    return libTypes.Single;
                case ElementType.R8:
                    return libTypes.Double;
            }
            return null;
        }
        public static ElementType? TypeSigToElementType(ModuleDef module, ITypeDefOrRef type)
        {
            var libTypes = module.CorLibTypes;
            if (type.FullName == libTypes.Boolean.FullName)
                return ElementType.Boolean;
            if (type.FullName == libTypes.Char.FullName)
                return ElementType.Char;
            if (type.FullName == libTypes.SByte.FullName)
                return ElementType.I1;
            if (type.FullName == libTypes.Byte.FullName)
                return ElementType.U1;
            if (type.FullName == libTypes.Int16.FullName)
                return ElementType.I2;
            if (type.FullName == libTypes.UInt16.FullName)
                return ElementType.U2;
            if (type.FullName == libTypes.Int32.FullName)
                return ElementType.I4;
            if (type.FullName == libTypes.UInt32.FullName)
                return ElementType.U4;
            if (type.FullName == libTypes.Int64.FullName)
                return ElementType.I8;
            if (type.FullName == libTypes.UInt64.FullName)
                return ElementType.U8;
            if (type.FullName == libTypes.Single.FullName)
                return ElementType.R4;
            if (type.FullName == libTypes.Double.FullName)
                return ElementType.R8;
            return null;
        }

        public static bool IsValidElementType(ElementType originaLType, ElementType newType)
        {
            return GetBestElementType(originaLType) == GetBestElementType(newType);
        }
        public static ElementType GetBestElementType(ElementType type)
        {
            switch (type)
            {
                case ElementType.U1:
                    return ElementType.I1;
                case ElementType.U2:
                    return ElementType.I2;
                case ElementType.U4:
                    return ElementType.I4;
                case ElementType.U8:
                    return ElementType.I8;
            }
            return type;
        }
    }
}
