using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Strings.Initalise
{
    class GetOriginalBytes:Base
    {
        public override void Deobfuscate()
        {
            Scraper(ModuleDefMD);
            return;
        }
        private static byte[][] methodbodies;
        public static byte[] tester(MethodDef methodDef, ModuleDefMD updated)
        {

            var streamFull = updated.Metadata.PEImage.CreateReader();
            var upated = (updated.ResolveToken(methodDef.MDToken.ToInt32()) as MethodDef);
            var offset = updated.Metadata.PEImage.ToFileOffset(upated.RVA);
            streamFull.Position = (uint)offset;
            byte b = streamFull.ReadByte();

            ushort flags;
            byte headerSize;
            ushort maxStack;
            uint codeSize = 0;

            switch (b & 7)
            {
                case 2:
                case 6:
                    flags = 2;
                    maxStack = 8;
                    codeSize = (uint)(b >> 2);
                    headerSize = 1;
                    break;

                case 3:
                    flags = (ushort)((streamFull.ReadByte() << 8) | b);
                    headerSize = (byte)(flags >> 12);
                    maxStack = streamFull.ReadUInt16();
                    codeSize = streamFull.ReadUInt32();
                    break;
            }
            if (codeSize != 0)
            {
                byte[] il_byte = new byte[codeSize];
                streamFull.Position = (uint)offset + upated.Body.HeaderSize;
                streamFull.ReadBytes(il_byte, 0, il_byte.Length);
                return il_byte;
            }
            return null;
        }
        public static Dictionary<uint, byte[]> bytesDict = new Dictionary<uint, byte[]>();
        public static void Scraper(ModuleDefMD module)
        {
            foreach (TypeDef types in module.GetTypes())
            {
                foreach (MethodDef methods in types.Methods)
                {
                    if (!methods.HasBody) continue;
                    byte[] bytes = tester(methods, module);
                    bytesDict.Add(methods.MDToken.ToUInt32(), bytes);
                }
            }
        }
    }
}
