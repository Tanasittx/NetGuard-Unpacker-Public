using System.Collections.Generic;
using System.Linq;
using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet.Emit;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.CflowCleaning
{
    internal class De4DotClass : BlockDeobfuscator
    {
        private InstructionEmulator ins;
        private Local loc;
        private Block swBlock;
        private Local swlocal;

        protected override bool Deobfuscate(Block block)
        {
            ins = ControlFlowRemover.Inemu;
            var modified = false;

            //		var blocks2 = blocks.MethodBlocks.GetAllBlocks();


            if (block.LastInstr.OpCode != OpCodes.Switch) return false;
            if (blocks.Method.MDToken.ToInt32() == 0x060000CC) { }
            if (block.Instructions.Count <= 4) return false;
            if (block.Instructions[block.Instructions.Count - 4].IsStloc())
            {
                if (blocks.Method.MDToken.ToInt32() == 0x060000CC) { }
                //		return false;
                var baseBlocksParent = block.Parent;
                var abc = baseBlocksParent.GetAllBlocks()[0].Instructions;

                foreach (var blockInstruction in abc)
                    if (blockInstruction.IsStloc())
                        if (blockInstruction.Instruction.GetLocal(blocks.Method.Body.Variables).Type ==
                            blocks.Method.Module.CorLibTypes.UInt32)
                        {
                            loc = blockInstruction.Instruction.GetLocal(blocks.Locals);
                            ins.Emulate(abc);
                            break;
                        }

                //    return false;

                swlocal = Instr.GetLocalVar(blocks.Locals, block.Instructions[block.Instructions.Count - 4]);

                swBlock = block;

                modified = doit(block);
            }
            else if (!block.Instructions[block.Instructions.Count - 4].IsStloc() &&
                     block.Instructions[block.Instructions.Count - 2].OpCode != OpCodes.Nop)
            {
                //		return false;
                if (blocks.Method.MDToken.ToInt32() == 0x060000CC) { }
                var baseBlocksParent = block.Parent;
                var abc = baseBlocksParent.GetAllBlocks()[0].Instructions;

                foreach (var blockInstruction in abc)
                    if (blockInstruction.IsStloc())
                    {
                        loc = blockInstruction.Instruction.GetLocal(blocks.Locals);
                        ins.Emulate(abc);
                        break;
                    }

                //    return false;

                swlocal = null;

                swBlock = block;

                modified = doit(block);
            }

            return modified;
        }

        private bool doit(Block swBlock)
        {
            var modified = false;

            if (swBlock.Targets == null)
                return modified;


            var allSwBlocks = allBlocks.Where(block => block.FallThrough == swBlock).ToList();
            foreach (var block in allSwBlocks)
            {
                var instr = block.Instructions;
                var count = block.Instructions.Count;
                int localVal;
                if (block.LastInstr.IsLdcI4() && count == 3)
                {
                    ins.Emulate(instr, count - 3, count);
                    var nextCase = caseEmulate(out localVal);
                    block.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                    block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                    replace(swBlock.Targets[nextCase], localVal);
                    modified = true;
                }
                else if (use1(block))
                {
                    ins.Emulate(instr, count - 9, count);
                    var nextCase = caseEmulate(out localVal);
                    block.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                    block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                    replace(swBlock.Targets[nextCase], localVal);
                    modified = true;
                }
                else if (use2(block))
                {
                    ins.Emulate(instr, count - 5, count);
                    var nextCase = caseEmulate(out localVal);
                    block.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                    block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                    replace(swBlock.Targets[nextCase], localVal);
                    modified = true;
                }
                else if (iseasyBlock(block))
                {
                    ins.Emulate(instr, count - 3, count);
                    var nextCase = caseEmulate(out localVal);
                    block.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                    block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                    replace(swBlock.Targets[nextCase], localVal);
                    modified = true;
                }
                else if (isMathCase(block) || isMathCase3(block))
                {
                    if (!instr[count - 7].IsLdcI4()) continue;
                    ins.Emulate(instr, count - 7, count);
                    var nextCase = caseEmulate(out localVal);
                    block.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                    block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                    replace(swBlock.Targets[nextCase], localVal);
                    modified = true;
                }
                else if (isMathCase2(block))
                {
                    if (!instr[count - 9].IsLdcI4()) continue;
                    ins.Emulate(instr, count - 9, count);
                    var nextCase = caseEmulate(out localVal);
                    block.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                    block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                    replace(swBlock.Targets[nextCase], localVal);
                    modified = true;
                }
                else if (isMathCase4(block))
                {
                    if (!instr[count - 9].IsLdcI4()) continue;
                    ins.Emulate(instr, count - 9, count);
                    var nextCase = caseEmulate(out localVal);
                    block.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                    block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));

                    replace(swBlock.Targets[nextCase], localVal);
                    modified = true;
                }
                else if (anotherunSure(block))
                {
                    var sources = new List<Block>(block.Sources);
                    foreach (var sour in sources)
                    {
                        if (!sour.FirstInstr.IsLdcI4()) continue;
                        //  ins.Push(new Int32Value(sour.FirstInstr.GetLdcI4Value()));
                        block.Instructions[count - 3] =
                            new Instr(Instruction.CreateLdcI4(sour.FirstInstr.GetLdcI4Value()));
                        ins.Emulate(instr, count - 3, count);
                        var nextCase = caseEmulate(out localVal);
                        sour.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                        sour.Remove(0, sour.Instructions.Count);
                        replace(swBlock.Targets[nextCase], localVal);
                        modified = true;
                    }
                }
                else if (unsureCase(block))
                {
                    var sources = new List<Block>(block.Sources);
                    foreach (var sour in sources)
                    {
                        if (!sour.FirstInstr.IsLdcI4()) continue;
                        if (sour.Instructions.Count != 2) continue;
                        //  ins.Push(new Int32Value(sour.FirstInstr.GetLdcI4Value()));
                        block.Instructions[count - 7] =
                            new Instr(Instruction.CreateLdcI4(sour.FirstInstr.GetLdcI4Value()));
                        ins.Emulate(instr, count - 7, count);
                        var nextCase = caseEmulate(out localVal);
                        sour.ReplaceLastNonBranchWithBranch(0, swBlock.Targets[nextCase]);
                        sour.Remove(0, sour.Instructions.Count);
                        replace(swBlock.Targets[nextCase], localVal);
                        modified = true;
                    }
                }
                else if (count == 1)
                {
                }
            }

            return modified;
        }

        private bool isMathCase2(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 7)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsArithmetic())
                return false;
            if (!instr[count - 4].IsLdcI4())
                return false;
            if (!instr[count - 6].IsLdcI4())
                return false;

            if (!instr[count - 5].IsArithmetic())
                return false;
            if (!instr[count - 3].IsLdcI4())
                return false;

            return true;
        }

        private bool isMathCase4(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 7)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsArithmetic())
                return false;
            if (!instr[count - 4].IsLdloc())
                return false;
            if (!instr[count - 6].IsLdcI4())
                return false;

            if (!instr[count - 5].IsArithmetic())
                return false;
            if (!instr[count - 3].IsLdcI4())
                return false;

            return true;
        }

        private bool isMathCase3(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 7)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsLdcI4())
                return false;
            if (!instr[count - 3].IsArithmetic())
                return false;
            if (!instr[count - 4].IsArithmetic())
                return false;

            if (!instr[count - 6].IsLdcI4())
                return false;
            if (!instr[count - 5].IsLdcI4())
                return false;
            if (!instr[count - 7].IsLdcI4())
                return false;

            return true;
        }

        private bool unsureCase(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (count < 7)
                return false;
            if (blo.Sources.Count != 2)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsLdcI4())
                return false;
            if (!instr[count - 3].IsArithmetic())
                return false;
            if (!instr[count - 4].IsArithmetic())
                return false;
            if (!instr[count - 5].IsLdcI4())
                return false;
            if (!instr[count - 6].IsLdcI4())
                return false;
            if (instr[count - 7].OpCode != OpCodes.Pop)
                return false;
            return true;
        }

        private bool iseasyBlock(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (count < 3)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsLdcI4())
                return false;
            if (!instr[count - 3].IsLdcI4())
                return false;

            return true;
        }

        private bool anotherunSure(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (count < 3)
                return false;
            if (blo.Sources.Count != 2)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsLdcI4())
                return false;
            if (instr[count - 3].OpCode != OpCodes.Pop)
                return false;

            return true;
        }

        private bool isMathCase(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 7)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsLdcI4())
                return false;
            if (!instr[count - 4].IsLdcI4())
                return false;
            if (!instr[count - 6].IsLdcI4())
                return false;

            if (!instr[count - 5].IsArithmetic())
                return false;
            if (!instr[count - 3].IsArithmetic())
                return false;

            return true;
        }

        private bool use1(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 7)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsArithmetic())
                return false;
            if (!instr[count - 3].IsLdcI4())
                return false;
            if (!instr[count - 4].IsLdloc())
                return false;

            if (!instr[count - 5].IsArithmetic())
                return false;
            if (!instr[count - 6].IsLdcI4())
                return false;
            if (!instr[count - 7].IsArithmetic())
                return false;
            if (!instr[count - 8].IsLdcI4())
                return false;
            if (!instr[count - 9].IsLdcI4())
                return false;
            if (instr[count - 4].Instruction.GetLocal(blocks.Locals) != loc)
                return false;
            return true;
        }

        private bool use2(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 5)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsArithmetic())
                return false;
            if (!instr[count - 3].IsLdcI4())
                return false;
            if (!instr[count - 4].IsLdloc())
                return false;

            if (!instr[count - 5].IsLdcI4())
                return false;

            if (instr[count - 4].Instruction.GetLocal(blocks.Locals) != loc)
                return false;
            return true;
        }

        private bool use4(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 5)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsArithmetic())
                return false;
            if (!instr[count - 3].IsLdcI4())
                return false;
            if (!instr[count - 4].IsLdloc())
                return false;

            if (!instr[count - 5].IsLdcI4())
                return false;

            if (instr[count - 4].Instruction.GetLocal(blocks.Locals).Type == blocks.Method.Module.CorLibTypes.UInt32)
                return false;
            return true;
        }

        private bool use3(Block blo)
        {
            var instr = blo.Instructions;
            var count = blo.Instructions.Count;
            if (instr.Count < 6)
                return false;
            if (!blo.LastInstr.IsStloc())
                return false;
            if (!instr[count - 2].IsArithmetic())
                return false;
            if (!instr[count - 3].IsLdcI4())
                return false;
            if (!instr[count - 4].IsLdloc())
                return false;

            if (!instr[count - 5].IsLdcI4())
                return false;

            if (instr[count - 4].Instruction.GetLocal(blocks.Locals) != loc)
                return false;
            return true;
        }

        private int caseEmulate(out int localVal)
        {
            if (swlocal == null)
            {
                ins.Emulate(swBlock.Instructions, 0, swBlock.Instructions.Count - 1);
                var caseVal = ins.Pop() as Int32Value;

                localVal = 0;

                return caseVal.Value;
            }
            else
            {
                ins.Emulate(swBlock.Instructions, 0, swBlock.Instructions.Count - 1);
                var caseVal = ins.Pop() as Int32Value;
                var localVal32 = ins.GetLocal(swlocal) as Int32Value;
                localVal = localVal32.Value;

                //		Console.WriteLine(caseVal.Value);
                return caseVal.Value;
            }
        }

        private bool replace(Block test, int locVal)
        {
            if (swlocal == null)
                return false;
            //we replace the ldloc values with the correct ldc value 
            if (test.IsConditionalBranch())
                test = test.FallThrough.FallThrough == swBlock ? test.FallThrough : test.FallThrough.FallThrough;
            if (test.LastInstr.OpCode == OpCodes.Switch)
                test = test.FallThrough;
            if (test == swBlock) return false;

            for (var i = 0; i < test.Instructions.Count; i++)
                if (test.Instructions[i].Instruction.GetLocal(blocks.Method.Body.Variables) == swlocal)
                {
                    //check to see if the local is the same as the one from the switch block and replace it
                    test.Instructions[i] = new Instr(Instruction.CreateLdcI4(locVal));
                    return true;
                }

            return false;
        }
    }
}
