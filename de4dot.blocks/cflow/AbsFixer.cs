using System;
using System.Collections.Generic;
using System.Text;
using dnlib.DotNet.Emit;

namespace de4dot.blocks.cflow {
	class AbsFixer : BlockDeobfuscator {
		protected override bool Deobfuscate(Block switchBlock) {
			bool modified = false;
			for (int i = 0; i < switchBlock.Instructions.Count; i++) {
				var instr = switchBlock.Instructions[i];
				if (instr.OpCode == OpCodes.Call) {
					if (switchBlock.Instructions[i - 1].IsLdcI4()) {
						if (instr.Operand.ToString().Contains("Abs")) {
							var value = switchBlock.Instructions[i - 1].GetLdcI4Value();
							instr = new Instr(new Instruction(OpCodes.Nop));
							switchBlock.Instructions[i - 1] = new Instr(new Instruction(OpCodes.Ldc_I4, Math.Abs(value)));
							modified = true;
						}
						
					}
				}
			}
			return modified;
		}
	}
}
