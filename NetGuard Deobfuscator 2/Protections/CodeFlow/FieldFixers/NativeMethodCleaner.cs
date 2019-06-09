using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.FieldFixers
{
    class NativeMethodCleaner : CodeFlowBase
    {
        private static readonly uint XORConstant = 0x9747B28C;
        private static uint IMULConstant = 0x4BD1E995;


        public override void Deobfuscate()
        {
    //        WriteModule(nameof(NativeMethodCleaner));
            InitializeConstants();
            Cleaner();
        }

        private static List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
        {
            List<int> positions = new List<int>();
            int patternLength = pattern.Length;
            int totalLength = bytes.Length;
            byte firstMatchByte = pattern[0];
            for (int i = 0; i < totalLength; i++)
            {
                if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
                {
                    byte[] match = new byte[patternLength];
                    Array.Copy(bytes, i, match, 0, patternLength);
                    if (match.SequenceEqual<byte>(pattern))
                    {
                        positions.Add(i);
                        i += patternLength - 1;
                    }
                }
            }
            return positions;
        }

        private static void InitializeConstants()
        {
            // if not found - use default constant
            byte[] file = File.ReadAllBytes(Program.Path);
            List<int> positions = SearchBytePattern(new byte[] { 0x8B, 0xC3, 0xC1, 0xE8, 0x18, 0x33, 0xD8, 0x69, 0xDB }, file);
            if (positions.Count > 0)
            {
                foreach (int i in positions)
                {
                    if (file.Skip(i + 0xD).Take(2).SequenceEqual(new byte[] { 0x69, 0xF6 }))
                    {
                        IMULConstant = BitConverter.ToUInt32(file, i + 0xF);
                    }
                }
            }
        }

        public static void Cleaner()
        {
            foreach (var typeDef in ModuleDefMD.GetTypes())
                foreach (var methods in typeDef.Methods)
                {
                    if (!methods.HasBody) continue;
                    for (var i = 0; i < methods.Body.Instructions.Count; i++)
                    {
                        if (methods.Body.Instructions[i].OpCode != OpCodes.Ldsfld || !methods.Body.Instructions[i].Operand.ToString()
                                .ToLower().Contains("intmethod")) continue;
                        if (!methods.Body.Instructions[i + 1].IsLdcI4() ||
                            methods.Body.Instructions[i + 2].OpCode != OpCodes.Callvirt) continue;
                        var val = methods.Body.Instructions[i + 1].GetLdcI4Value();
                        var value = TransformInt(val);
                 //       Console.WriteLine($"[!] Original Value {val} Transformed Int is {value}");
                        methods.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                        methods.Body.Instructions[i].Operand = (int)value;
                        methods.Body.Instructions[i + 1].OpCode = OpCodes.Nop;
                        methods.Body.Instructions[i + 2].OpCode = OpCodes.Nop;
                    }
                }
        }

        private static uint TransformInt(int input)
        {
            var str_value = input.ToString();
            var tmp0 = str_value.Length;
            var tmp1 = (uint)(tmp0 ^ XORConstant);
            var i = 1;
            var oo = Encoding.ASCII.GetBytes(str_value);
            while (tmp0 >= 4)
            {
                var a1 = (uint)(IMULConstant * BitConverter.ToInt32(oo, i - 1));
                var a2 = a1 >> 0x18;
                var a3 = a2 ^ a1;
                tmp1 = (tmp1 * IMULConstant) ^ (a3 * IMULConstant);
                tmp0 -= 4;
                i += 4;
            }
            if (tmp0 == 3)
                tmp1 = (uint)(tmp1 ^ (oo[i + 1] << 16));
            if (tmp0 >= 2)
                tmp1 = (uint)(tmp1 ^ (oo[i] << 8));
            if (tmp0 >= 1)
                tmp1 = IMULConstant * (oo[i - 1] ^ tmp1);
            return ((IMULConstant * ((tmp1 >> 13) ^ tmp1)) >> 15) ^ (IMULConstant * ((tmp1 >> 13) ^ tmp1));
        }
    }
}
