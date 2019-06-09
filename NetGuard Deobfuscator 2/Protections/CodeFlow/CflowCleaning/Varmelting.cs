using de4dot.blocks.cflow;
using System.Collections.Generic;
using de4dot.blocks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.CflowCleaning
{
    public class VariableMelting : BlockDeobfuscator
    {
        class VMInfo
        {
            public int Size { get; set; }
            public Local Local { get; set; }
            public Instr Instruction { get; set; }
            public ElementType[] Info { get; set; }
        }
        bool CheckAlocBlock(Block block, out Dictionary<Local, VMInfo> vmDictonary)
        {
            vmDictonary = new Dictionary<Local, VMInfo>();
            var outLocals = new List<VMInfo>();
            var instr = block.Instructions;
            for (int i = 3; i < instr.Count; i++)
            {
                if (!Utilis.IsLoc(instr[i]))
                    continue;
                var local = instr[i].Instruction.GetLocal(blocks.Locals);
                if (!Utilis.IsPtrElementType(local.Type, ElementType.U1))
                    continue;
                if (!instr[i].IsStloc())
                    continue;
                if (vmDictonary.ContainsKey(local))
                {
                    outLocals.Remove(vmDictonary[local]);
                    continue;
                }
                if (instr[i - 1].OpCode != OpCodes.Localloc)
                    continue;
                if (instr[i - 2].OpCode != OpCodes.Conv_U)
                    continue;
                if (!instr[i - 3].IsLdcI4())
                    continue;
                var size = instr[i - 3].GetLdcI4Value();
                var vmInfo = new VMInfo() { Size = size, Local = local, Instruction = instr[i], Info = NewEmptyElementTypeArray(size) };
                vmDictonary[local] = vmInfo;
                outLocals.Add(vmInfo);
            }
            return outLocals.Count != 0;
        }
        ElementType[] NewEmptyElementTypeArray(int size)
        {
            var array = new ElementType[size];
            for (int i = 0; i < size; i++)
                array[i] = ElementType.Void;
            return array;
        }
        bool isValidPtrInstrLoad(Instr instr, out ElementType type)
        {
            type = ElementType.Void;
            switch (instr.OpCode.Code)
            {
                case Code.Ldind_I1:
                    //  case Code.Stind_I1:
                    type = ElementType.I1;
                    break;
                case Code.Ldind_I2:
                    //   case Code.Stind_I2:
                    type = ElementType.I2;
                    break;
                case Code.Ldind_I4:
                    //   case Code.Stind_I4:
                    type = ElementType.I4;
                    break;
                case Code.Ldind_I8:
                    //   case Code.Stind_I8:
                    type = ElementType.I8;
                    break;
                case Code.Ldind_R4:
                    //    case Code.Stind_R4:
                    type = ElementType.R4;
                    break;
                case Code.Ldind_R8:
                    //  case Code.Stind_R8:
                    type = ElementType.R8;
                    break;
                case Code.Ldind_U1:
                    type = ElementType.U1;
                    break;
                case Code.Ldind_U2:
                    type = ElementType.U2;
                    break;
                case Code.Ldind_U4:
                    type = ElementType.U4;
                    break;
                default:
                    return false;
            }
            return true;
        }
        bool isLoad0(IList<Instr> instr, int index, out ElementType type, out int ptrIndex, out int size)
        {
            ptrIndex = 0;
            size = 1;
            type = ElementType.Void;
            if (index + size >= instr.Count)
                return false;
            if (!isValidPtrInstrLoad(instr[index + 1], out type))
                return false;
            return true;
        }
        bool isLoadIndex(IList<Instr> instr, int index, out ElementType type, out int ptrIndex, out int size)
        {
            ptrIndex = 0;
            size = 4;
            type = ElementType.Void;
            if (index + size >= instr.Count)
                return false;
            if (!instr[index + 1].IsLdcI4())
                return false;
            ptrIndex = instr[index + 1].GetLdcI4Value();
            if (instr[index + 2].OpCode != OpCodes.Conv_I)
                return false;
            if (instr[index + 3].OpCode != OpCodes.Add_Ovf_Un)
                return false;
            if (!isValidPtrInstrLoad(instr[index + 4], out type))
                return false;
            return true;
        }
        bool GetInlineBlock(IList<Instr> instr, int start, out int size)
        {
            size = 0;
            int stackSize = -1;
            for (int i = start; i < instr.Count; i++)
            {
                size++;
                int pop, push;
                instr[i].Instruction.CalculateStackUsage(out push, out pop);
                stackSize -= push;
                stackSize += pop;
                if (stackSize == 0)
                    return true;
            }
            return false;
        }
        bool isValidPtrInstrSet(Instr instr, out ElementType type)
        {
            type = ElementType.Void;
            switch (instr.OpCode.Code)
            {
                case Code.Stind_I1:
                    type = ElementType.I1;
                    break;
                case Code.Stind_I2:
                    type = ElementType.I2;
                    break;
                case Code.Stind_I4:
                    type = ElementType.I4;
                    break;
                case Code.Stind_I8:
                    type = ElementType.I8;
                    break;
                case Code.Stind_R4:
                    type = ElementType.R4;
                    break;
                case Code.Stind_R8:
                    type = ElementType.R8;
                    break;
                default:
                    return false;
            }
            return true;
        }
        bool isSet0(IList<Instr> instr, int index, out ElementType type, out int ptrIndex, out int size)
        {
            ptrIndex = 0;
            size = 0;
            type = ElementType.Void;
            if (index + 1 >= instr.Count)
                return false;
            int newSize;
            if (!GetInlineBlock(instr, index + 1, out newSize))
                return false;
            size += newSize;
            if (!isValidPtrInstrSet(instr[index + size], out type))
                return false;
            return true;
        }
        bool isSetIndex(IList<Instr> instr, int index, out ElementType type, out int ptrIndex, out int size)
        {
            ptrIndex = 0;
            size = 3;
            type = ElementType.Void;
            if (index + size >= instr.Count)
                return false;
            if (!instr[index + 1].IsLdcI4())
                return false;
            ptrIndex = instr[index + 1].GetLdcI4Value();
            if (instr[index + 2].OpCode != OpCodes.Conv_I)
                return false;
            if (instr[index + 3].OpCode != OpCodes.Add_Ovf_Un)
                return false;
            int newSize;
            if (!GetInlineBlock(instr, index + 4, out newSize))
                return false;
            size += newSize;
            if (!isValidPtrInstrSet(instr[index + size], out type))
                return false;
            return true;
        }
        bool isValidLoad(Block block, int index, out ElementType type, out int ptrIndex, out int size)
        {
            var instr = block.Instructions;
            if (!isLoad0(instr, index, out type, out ptrIndex, out size) && !isLoadIndex(instr, index, out type, out ptrIndex, out size) &&
                !isSetIndex(instr, index, out type, out ptrIndex, out size) && !isSet0(instr, index, out type, out ptrIndex, out size))
                return false;
            return true;
        }
        bool CheckBlock(Block block, Dictionary<Local, VMInfo> vmDictonary, out List<Tuple<Tuple<int, int>, Tuple<int, Local>>> loalBlocks)
        {
            loalBlocks = new List<Tuple<Tuple<int, int>, Tuple<int, Local>>>();
            var instr = block.Instructions;
            for (int i = 0; i < instr.Count; i++)
            {
                if (!Utilis.IsLoc(instr[i]))
                    continue;
                var local = instr[i].Instruction.GetLocal(blocks.Locals);
                if (local == null || !vmDictonary.ContainsKey(local))
                    continue;
                var vmInfo = vmDictonary[local];
                if (Utilis.IsStloc(instr[i]))
                {
                    if (vmInfo.Instruction != instr[i])
                        return false;
                }
                else
                {
                    ElementType type;
                    int ptrIndex, size;
                    if (!isValidLoad(block, i, out type, out ptrIndex, out size))
                        return false;
                    if (vmInfo.Info[ptrIndex] != ElementType.Void && !Utilis.IsValidElementType(vmInfo.Info[ptrIndex], type))
                        return false;
                    vmInfo.Info[ptrIndex] = Utilis.GetBestElementType(type);
                    var tup1 = new Tuple<int, int>() { Item1 = i, Item2 = size };
                    var tup2 = new Tuple<int, Local>() { Item1 = ptrIndex, Item2 = local };
                    loalBlocks.Add(new Tuple<Tuple<int, int>, Tuple<int, Local>>() { Item1 = tup1, Item2 = tup2 });
                }
            }
            return true;
        }
        List<Tuple<Tuple<int, int>, Tuple<int, Local>>>[] CreateLocasInfos(Dictionary<Local, VMInfo> vmInfos)
        {
            var blocksInfos = new List<Tuple<Tuple<int, int>, Tuple<int, Local>>>[allBlocks.Count];
            for (int i = 0; i < allBlocks.Count; i++)
            {
                List<Tuple<Tuple<int, int>, Tuple<int, Local>>> blocksInfo;
                if (!CheckBlock(allBlocks[i], vmInfos, out blocksInfo))
                    return null;
                blocksInfos[i] = blocksInfo;
            }
            return blocksInfos;
        }
        bool CheckIfIndexesAreRight(Dictionary<Local, VMInfo> vmInfos)
        {
            foreach (var vmInfo in vmInfos.Values)
            {
                var info = vmInfo.Info;
                for (int i = 0; i < info.Length; i++)
                {
                    if (info[i] == ElementType.Void)
                        continue;
                    var size = Utilis.GetElementTypeSize(info[i]);
                    if (size == -1 || i + size - 1 >= info.Length)
                        return false;
                    for (int j = i + 1; j < size - 1; j++)
                        if (info[j] != ElementType.Void)
                            return false;
                }
            }
            return true;
        }
        Dictionary<int, Local>[] CreateLocals(Dictionary<Local, VMInfo> vmInfos)
        {
            var dict = new Dictionary<int, Local>[vmInfos.Count];
            var infos = new List<VMInfo>(vmInfos.Values);
            for (int i = 0; i < vmInfos.Count; i++)
            {
                dict[i] = new Dictionary<int, Local>();
                var info = infos[i].Info;
                for (int j = 0; j < info.Length; j++)
                {
                    if (info[j] == ElementType.Void)
                        continue;
                    var sig = Utilis.ElementTypeToTypeSig(blocks.Method.Module, info[j]);
                    if (sig == null)
                        return null;
                    var local = new Local(sig);
                    blocks.Method.Body.Variables.Add(local);
                    dict[i][j] = local;
                }

            }
            return dict;
        }
        protected override bool Deobfuscate(Block block)
        {
          
            if (block != allBlocks[0])
                return false;
            Dictionary<Local, VMInfo> vmInfos;
            if (!CheckAlocBlock(block, out vmInfos))
                return false;
            var infos = CreateLocasInfos(vmInfos);
            if (infos == null)
                return false;
            if (!CheckIfIndexesAreRight(vmInfos))
                return false;
            var locals = CreateLocals(vmInfos);
            if (locals == null)
                return false;
            return Fix(vmInfos, infos, locals);
        }
        bool IsAdd(IList<Instr> instr, int i)
        {
            if (i + 3 >= instr.Count)
                return false;
            if (!instr[i + 1].IsLdcI4())
                return false;
            if (instr[i + 2].OpCode != OpCodes.Conv_I)
                return false;
            if (instr[i + 3].OpCode != OpCodes.Add_Ovf_Un)
                return false;
            return true;
        }
        int getVmInfoIndex(Dictionary<Local, VMInfo> vmInfos, Local local)
        {
            var vmInfo = vmInfos[local];
            var infos = new List<VMInfo>(vmInfos.Values);
            for (int i = 0; i < infos.Count; i++)
                if (infos[i] == vmInfo)
                    return i;
            return -1;
        }
        bool Fix(Dictionary<Local, VMInfo> vmInfos, List<Tuple<Tuple<int, int>, Tuple<int, Local>>>[] infos, Dictionary<int, Local>[] locals)
        {
            bool modified = false;
            for (int j = 0; j < allBlocks.Count; j++)
            {
                var info = infos[j];
                if (info.Count == 0)
                    continue;
                var instr = allBlocks[j].Instructions;
                for (int i = instr.Count - 1; i >= 0; i--)
                {
                    var tup = info.Find(l => l.Item1.Item1 == i);
                    if (tup == null)
                        continue;

                    int ptrIndex = i + tup.Item1.Item2;
                    int infoIndex = tup.Item2.Item1;
                    var local = tup.Item2.Item2;
                    var vmInfoIndex = getVmInfoIndex(vmInfos, local);
                    if (vmInfoIndex == -1)
                        continue;
                    var newLocal = locals[vmInfoIndex][infoIndex];

                    ElementType type;
                    if (isValidPtrInstrLoad(instr[ptrIndex], out type))
                    {
                        instr[ptrIndex] = new Instr(new Instruction(OpCodes.Ldloc, newLocal));
                        modified |= true;
                    }
                    else if (isValidPtrInstrSet(instr[ptrIndex], out type))
                    {
                        instr[ptrIndex] = new Instr(new Instruction(OpCodes.Stloc, newLocal));
                        modified |= true;
                    }
                    else
                        continue;
                    instr[i] = new Instr(OpCodes.Nop.ToInstruction());
                    if (IsAdd(instr, i))
                        for (int k = 1; k < 4; k++)
                            instr[i + k] = new Instr(OpCodes.Nop.ToInstruction());
                }

            }
            return modified;
        }
    }
}
