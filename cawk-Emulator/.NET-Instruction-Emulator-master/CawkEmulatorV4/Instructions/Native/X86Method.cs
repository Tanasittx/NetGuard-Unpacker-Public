using System.Collections.Generic;
using System.Linq;
using ConfuserDeobfuscator.Engine.Routines.Ex.x86.Instructions;
using dnlib.DotNet;
using EasyPredicateKiller;
using SharpDisasm;

namespace ConfuserDeobfuscator.Engine.Routines.Ex.x86
{
    public sealed class X86Method
    {
        public List<X86Instruction> Instructions;

        public Stack<int> LocalStack = new Stack<int>();

        public MethodDef OriginalMethod;

        public Dictionary<string, int> Registers = new Dictionary<string, int>
        {
            {"EAX", 0},
            {"EBX", 0},
            {"ECX", 0},
            {"EDX", 0},
            {"ESP", 0},
            {"EBP", 0},
            {"ESI", 0},
            {"EDI", 0}
        };


        public X86Method(MethodDef method)
        {
            Instructions = new List<X86Instruction>();
            ParseInstructions(method);

            OriginalMethod = method;
        }

        private void ParseInstructions(MethodDef method)
        {
            var rawInstructions = new List<Instruction>();


            var body = method.ReadBodyFromRva();
            
            var disasm = new Disassembler(body, ArchitectureMode.x86_32);
            rawInstructions = disasm.Disassemble().ToList();


            foreach (var instr in rawInstructions)
            {
                var retReached = false;

                var currentInstruction = instr + " "; // fix for ret
                currentInstruction = currentInstruction.Remove(currentInstruction.IndexOf(" "));

                switch (currentInstruction)
                {
                    case "mov":
                        Instructions.Add(new X86MOV(instr));
                        break;
                    case "add":
                        Instructions.Add(new X86ADD(instr));
                        break;
                    case "sub":
                        Instructions.Add(new X86SUB(instr));
                        break;
                    case "imul":
                        Instructions.Add(new X86IMUL(instr));
                        break;
                    case "div":
                        Instructions.Add(new X86DIV(instr));
                        break;
                    case "neg":
                        Instructions.Add(new X86NEG(instr));
                        break;
                    case "not":
                        Instructions.Add(new X86NOT(instr));
                        break;
                    case "xor":
                        Instructions.Add(new X86XOR(instr));
                        break;
                    case "pop":
                        Instructions.Add(new X86POP(instr));
                        break;

                    case "ret":
                        // Remove last pop instructions
                        while (Instructions[Instructions.Count - 1].OpCode == X86OpCode.POP)
                            Instructions.RemoveAt(Instructions.Count - 1);
                        retReached = true;
                        break;
                }

                if (retReached)
                    break;
            }
        }

        public int Execute(params int[] @params)
        {
            foreach (var param in @params)
                LocalStack.Push(param);

            foreach (var instr in Instructions)
                instr.Execute(Registers, LocalStack);

            return Registers["EAX"];
        }
    }
}