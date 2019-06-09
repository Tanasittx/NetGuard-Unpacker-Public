using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.PE;
using NetGuard_Deobfuscator_2.Protections.Native_Unpacker;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NetGuard_Deobfuscator_2.Protections.AntiTamper
{
    internal class Remover : Base
    {
        private static readonly uint DecryptionKey;
        private static readonly MethodDef DecryptMethod;
        private static readonly uint[] dst;
        private static readonly Instruction[] mainInstructions;
        private static readonly uint[] src;
        private static MethodDef antitamp;
        private static uint[] arrayKeys;
        private static byte[] byteResult;
        private static MethodDef cctor;
        private static List<Instruction> dynInstr;
        public static uint[] initialKeys;
        private static readonly string assembly;
        private static ModuleDefMD module;
        private static readonly ModuleDefMD initialmodule;
        private static readonly byte[] buffer2;
        private static BinaryReader reader;
        private static MemoryStream input;
        public static uint DecryptionValue = 0x3dbb2819;
        public string DirectoryName = "";
        public static byte[] valid = new byte[] { 0x4D, 0x5A };
        public static byte[] valid_gzip = new byte[] { 0x1F, 0x8B };
        private static bool nativeCode;

        public static uint[] DataField { get; }
        public static BinaryReader origReader { get; private set; }
        public static ImageSectionHeader origConf { get; private set; }
        public static MemoryStream origInput { get; private set; }

        internal static MethodDef FindAntiTamperMethod()
        {
            MethodDef Cctor = ModuleDefMD.GlobalType.FindOrCreateStaticConstructor();
            if (Cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                Cctor.Body.Instructions[0].Operand.ToString().Contains("Koi"))
            {
                Cctor = (MethodDef)Cctor.Body.Instructions[0].Operand;
            }

            return (from t in Cctor.Body.Instructions
                    where t.OpCode == OpCodes.Call
                    select t.Operand as MethodDef
                into Method
                    where Method.HasBody
                    from instructions in Method.Body.Instructions
                    where instructions.OpCode == OpCodes.Call
                    select Method
                into tamperMethod
                    where tamperMethod != null
                    where tamperMethod.ReturnType.ElementType == ElementType.Void
                    select tamperMethod).FirstOrDefault(tamperMethod => Helper.IsMethodUsingVirtualProtect(tamperMethod));
        }

        public override void Deobfuscate()
        {
            module = ModuleDefMD;
      //      WriteModule("Anti Tamper");
            if (IsTampered(module) != true)
            {
                return;
            }

  //          Console.WriteLine("[!] Anti Tamper Detected");
            ValueGrabber(Program.Path);
  //          Console.WriteLine("[!] Cleaning Anti Tamper");
            ModuleDefMD b = UnAntiTamper(module,
                module.Metadata.PEImage.CreateReader().ReadBytes((int)module.Metadata.PEImage.CreateReader().Length));

            if (b != null)
            {
   //             Console.WriteLine("[!] Anti Tamper Cleaned");
                ModuleDefMD = b;
                Program.LoadAsmRef();
            }
            else
            {
                Console.WriteLine("[!!] Anti Tamper Failed To Remove");
            }
            return;
        }

        private static bool isNew(MethodDef methods)
        {
            /*
             * 223	02DD	ldloc	V_11 (11)
               224	02E1	ldc.i4	5
               225	02E6	shr.un
               226	02E7	ldloc	V_11 (11)
               227	02EB	ldc.i4	27
               228	02F0	shl
               229	02F1	or
               230	02F2	stloc	V_10 (10)
               
             */
            for (int i = 0; i < methods.Body.Instructions.Count; i++)
            {
                if (methods.Body.Instructions[i].IsStloc() && methods.Body.Instructions[i - 1].OpCode == OpCodes.Or &&
                    methods.Body.Instructions[i - 2].OpCode == OpCodes.Shl &&
                    methods.Body.Instructions[i - 3].IsLdcI4() && methods.Body.Instructions[i - 4].IsLdloc()
                    && methods.Body.Instructions[i - 5].OpCode == OpCodes.Shr_Un &&
                    methods.Body.Instructions[i - 6].IsLdcI4() && methods.Body.Instructions[i - 7].IsLdloc())
                {
                    int value1 = methods.Body.Instructions[i - 6].GetLdcI4Value();
                    int value2 = methods.Body.Instructions[i - 3].GetLdcI4Value();
                    if (value1 == 5 && value2 == 27)
                    {
                        return false;
                    }
                    else if (value1 == 6 && value2 == 26)
                    {
                        return true;
                    }
                    else
                    {
                        nativeCode = true
                            ;
                        return false;
                    }

                }


                else
                {



                }
            }

            throw new Exception("Tamper version not detected");
        }
        public static ModuleDefMD UnAntiTamper(ModuleDefMD module, byte[] rawbytes, bool reren = false, uint val = 808349983)
        {
            dynInstr = new List<Instruction>();
            initialKeys = new uint[4];
            cctor = module.GlobalType.FindStaticConstructor();
            if (cctor.Body.Instructions[0].OpCode == OpCodes.Call &&
                cctor.Body.Instructions[0].Operand.ToString().Contains("VM"))
            {
                cctor = (MethodDef)cctor.Body.Instructions[0].Operand;
            }

            antitamp = FindAntiTamperMethod();
            methodsToRemove.Add(antitamp);
            if (antitamp == null)
            {
                return null;
            }
            //Console.WriteLine("[!] AntiTamper Method Found: " + antitamp.Name);
            IList<ImageSectionHeader> imageSectionHeaders = module.Metadata.PEImage.ImageSectionHeaders;
            ImageSectionHeader confSec = imageSectionHeaders[1];
            FindInitialKeys(antitamp);
            if (initialKeys.Any(initialKey => initialKey == 0))
            {
                //	Console.WriteLine("[!!] First Initial Key Scraper Failed");
                FindInitialKeys2(antitamp);
            }
            if (initialKeys.Any(initialKey => initialKey == 0))
            {
                //			Console.WriteLine("[!!] Second Initial Key Scraper Failed");
                FindInitialKeys3(antitamp);
            }
            if (initialKeys.Any(initialKey => initialKey == 0))
            {
                //			Console.WriteLine("[!!] Third Initial Key Scraper Failed");
                FindInitialKeys4(antitamp);
            }
            if (initialKeys[1] == 0)
            {
                return null;
            }
            //		Console.WriteLine("[!] All Keys Scraped");
            input = new MemoryStream(rawbytes);
            reader = new BinaryReader(input);
            //		Console.WriteLine("[!] Hashing Keys");
            Hash1(input, reader, imageSectionHeaders, confSec);
            bool isNew = Remover.isNew(antitamp);
            //		Console.WriteLine("[!] Grabbing Next Keys");
            arrayKeys = GetArrayKeys(isNew);
            //		Console.WriteLine("[!] Decrypting Methods");
            origReader = reader;
            if (reren)
            {
                DecryptMethods(reader, confSec, input, val);
            }
            else
            {
                DecryptMethods(reader, confSec, input);
            }
            ModuleDefMD fmd2 = ModuleDefMD.Load(input);

            //			while(checker(fmd2))
            //			{
            //				reader.BaseStream.Position= origReader.BaseStream.Position;
            //				confSec = origConf;
            //				input = origInput;
            //				DecryptMethods(reader, confSec, input, 808349983);
            //				fmd2 = ModuleDefMD.Load(input);
            //			}
            //			Console.WriteLine("[!] Loaded Module: " + fmd2.FullName);
            //     fmd2.GlobalType.FindStaticConstructor().Body.Instructions.RemoveAt(0);
            return fmd2;
        }

        public static bool checker(ModuleDefMD fmd2)
        {
            IList<Instruction> ep = fmd2.EntryPoint.Body.Instructions;
            if (ep.Count == 0 || ep.Count < 7)
            {
                return false;
            }

            if (ep.Any(t => t.OpCode == OpCodes.UNKNOWN1 || t.OpCode == OpCodes.UNKNOWN2) || ep.Any(t => t.IsConditionalBranch() && t.Operand == null) || ep.Any(t => (t.OpCode.OperandType != OperandType.InlineNone) && t.Operand == null))
            {
                return false;
            }
            return true;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);


        [DllImport("kernel32.dll")]
        private static extern IntPtr FindResource(IntPtr hModule, string lpID, string lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        public static byte[] gzip_decompress(byte[] decoded)
        {
            MemoryStream to = new MemoryStream();
            MemoryStream from = new MemoryStream(decoded);
            new GZipStream(from, CompressionMode.Decompress).CopyTo(to);
            from.Close();
            return to.ToArray();
        }
        public static byte[] FindMinLength(List<byte[]> list)
        {
            byte[] result = null;
            int Min_Length = int.MaxValue;
            foreach (byte[] type in list)
            {
                if (type.Length < Min_Length)
                {
                    Min_Length = type.Length;
                    result = type;
                }
            }
            return result;
        }

        public static byte[] DecryptAES(byte[] src, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged
            {
                KeySize = 128,
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.ECB
            };
            using (ICryptoTransform decrypt = aes.CreateDecryptor(key, null))
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                decrypt.Dispose();
                return dest;
            }
        }

        public static byte[] AES_Decrypt_Method(byte[] raw_resource)
        {
            try
            {
                byte[] decoded = null;
                decoded = DecryptAES(raw_resource, Helper.aes_key);
                if (decoded.Take(2).SequenceEqual(valid_gzip))
                {
                    decoded = gzip_decompress(decoded);
                    return decoded;
                }
                else
                {
                    return decoded;
                }
            }
            catch
            {
                return null;
            }
        }

        public static byte[] RC4_Decrypt_Method(byte[] raw_resource)
        {
            byte[] decoded = null;
            for (int i = 0; i < Helper.RC4keys.Count; i++)
            {
                RC4 rc4 = new RC4(Encoding.ASCII.GetBytes(Helper.RC4keys[i]));
                decoded = rc4.Decode(raw_resource, raw_resource.Length);
                if (decoded.Take(2).SequenceEqual(valid_gzip))
                {
                    decoded = gzip_decompress(decoded);
                    return decoded;
                }
                else
                {
                    return decoded;
                }
            }
            return null;
        }

        public static bool ValueGrabber(string path)
        {
            try
            {
                AsmResolver.ImageResourceDirectory abc2 = AsmResolver.WindowsAssembly.FromFile(path).RootResourceDirectory;
                byte[] bPtr = null;
                List<byte[]> res_data = new List<byte[]>();
                IList<AsmResolver.ImageResourceDirectoryEntry> resources = abc2.Entries.Where(i => i.Name == "RC_DATA").ToArray().FirstOrDefault().SubDirectory.Entries;

                for (int i = 0; i < resources.Count; i++)
                {
                    res_data.Add(resources[i].SubDirectory.Entries[0].DataEntry.Data);
                }

                bPtr = FindMinLength(res_data);
                byte[] decoded = null;
                decoded = RC4_Decrypt_Method(bPtr);
                try
                {
                    string abc = Encoding.ASCII.GetString(decoded);
                    if (decoded != null)
                    {
                        DecryptionValue = uint.Parse(Encoding.ASCII.GetString(decoded).Split('|')[1]);
                        return true;
                    }
                }
                catch
                {
                }
                decoded = AES_Decrypt_Method(bPtr);
                try
                {
                    string abcd = Encoding.ASCII.GetString(decoded);
                    if (decoded != null)
                    {
                        //          DecryptionValue = uint.Parse(Encoding.ASCII.GetString(decoded).Split('|')[1]);
                        Match regex = new Regex(@"^v(\d+.\d+.\d+)\|(\d+)").Match(Encoding.ASCII.GetString(decoded));
                        if (regex.Success)
                        {
                            DecryptionValue = uint.Parse(regex.Groups[2].Value);
                        }

                        return true;
                    }
                }
                catch
                {
                }

                if (DecryptionValue == 0x3dbb2819)
                {
                    return false;
                }
                Console.WriteLine("[!] Anti Tamper Decryptor Key Found: " + DecryptionValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void DecryptMethods(BinaryReader reader, ImageSectionHeader confSec, Stream stream)
        {
            int num = (int)(confSec.SizeOfRawData >> 2);
            int pointerToRawData = (int)confSec.PointerToRawData;
            stream.Position = pointerToRawData;
            uint[] array = new uint[num];
            uint num2 = 0u;
            while (num2 < (ulong)num)
            {
                uint num3 = reader.ReadUInt32();
                array[(int)num2] = num3 ^ arrayKeys[(int)(IntPtr)(num2 & 15u)];
                arrayKeys[(int)(IntPtr)(num2 & 15u)] = num3 + DecryptionValue;
                num2 += 1u;
            }
            byteResult = new byte[num << 2];
            byteResult = array.SelectMany(BitConverter.GetBytes).ToArray();
            stream.Position = pointerToRawData;
            stream.Write(byteResult, 0, byteResult.Length);
        }
        private static void DecryptMethods(BinaryReader reader, ImageSectionHeader confSec, Stream stream, long val)
        {
            int num = (int)(confSec.SizeOfRawData >> 2);
            int pointerToRawData = (int)confSec.PointerToRawData;
            stream.Position = pointerToRawData;
            uint[] array = new uint[num];
            uint num2 = 0u;
            while (num2 < (ulong)num)
            {
                uint num3 = reader.ReadUInt32();
                array[(int)num2] = num3 ^ arrayKeys[(int)(IntPtr)(num2 & 15u)];
                arrayKeys[(int)(IntPtr)(num2 & 15u)] = num3 + (uint)val;
                num2 += 1u;
            }
            byteResult = new byte[num << 2];
            byteResult = array.SelectMany(BitConverter.GetBytes).ToArray();
            stream.Position = pointerToRawData;
            stream.Write(byteResult, 0, byteResult.Length);
        }
        //0019FE48   021040D4  \Value = "4053884700"

        public static bool? IsTampered(ModuleDefMD module)
        {
            IList<ImageSectionHeader> sections = module.Metadata.PEImage.ImageSectionHeaders;

            if (sections.Count == 3)
            {
                return false;
            }

            foreach (ImageSectionHeader section in sections)
            {
                switch (section.DisplayName)
                {
                    case ".text":
                    case ".rsrc":
                    case ".reloc":
                        continue;
                    default:

                        return true;
                }
            }

            return null;
        }

        private static byte[] ConvertUInt32ArrayToByteArray(uint[] value)
        {
            const int bytesPerUInt32 = 4;
            byte[] result = new byte[value.Length * bytesPerUInt32];
            for (int index = 0; index < value.Length; index++)
            {
                byte[] partialResult = BitConverter.GetBytes(value[index]);
                for (int indexTwo = 0; indexTwo < partialResult.Length; indexTwo++)
                {
                    result[index * bytesPerUInt32 + indexTwo] = partialResult[indexTwo];
                }
            }
            return result;
        }

        private static void FindInitialKeys(MethodDef antitamp)
        {
            int count = antitamp.Body.Instructions.Count;
            int num2 = count - 0x125;
            for (int i = 0; i < count; i++)
            {
                Instruction item = antitamp.Body.Instructions[i];
                if (!item.OpCode.Equals(OpCodes.Ldc_I4))
                {
                    continue;
                }

                if (!antitamp.Body.Instructions[i + 1].OpCode.Equals(OpCodes.Stloc_S))
                {
                    continue;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_19"))
                {
                    initialKeys[0] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_20"))
                {
                    initialKeys[1] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_21"))
                {
                    initialKeys[2] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_22"))
                {
                    initialKeys[3] = (uint)(int)item.Operand;
                }
            }
        }

        private static void FindInitialKeys2(MethodDef antitamp)
        {
            int count = antitamp.Body.Instructions.Count;
            int num2 = count - 0x125;
            for (int i = 0; i < count; i++)
            {
                Instruction item = antitamp.Body.Instructions[i];
                if (!item.OpCode.Equals(OpCodes.Ldc_I4))
                {
                    continue;
                }

                if (!antitamp.Body.Instructions[i + 1].OpCode.Equals(OpCodes.Stloc_S))
                {
                    continue;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_15"))
                {
                    initialKeys[0] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_16"))
                {
                    initialKeys[1] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_17"))
                {
                    initialKeys[2] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_18"))
                {
                    initialKeys[3] = (uint)(int)item.Operand;
                }
            }
        }

        private static void FindInitialKeys3(MethodDef antitamp)
        {
            int count = antitamp.Body.Instructions.Count;
            int num2 = count - 0x125;
            for (int i = 0; i < count; i++)
            {
                Instruction item = antitamp.Body.Instructions[i];
                if (!item.OpCode.Equals(OpCodes.Ldc_I4))
                {
                    continue;
                }

                if (!antitamp.Body.Instructions[i + 1].OpCode.Equals(OpCodes.Stloc_S))
                {
                    continue;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_10"))
                {
                    initialKeys[0] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_11"))
                {
                    initialKeys[1] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_12"))
                {
                    initialKeys[2] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_13"))
                {
                    initialKeys[3] = (uint)(int)item.Operand;
                }
            }
        }

        private static void FindInitialKeys4(MethodDef antitamp)
        {
            int count = antitamp.Body.Instructions.Count;
            int num2 = count - 0x125;
            for (int i = 0; i < count; i++)
            {
                Instruction item = antitamp.Body.Instructions[i];
                if (!item.OpCode.Equals(OpCodes.Ldc_I4))
                {
                    continue;
                }

                if (!antitamp.Body.Instructions[i + 1].OpCode.Equals(OpCodes.Stloc_S))
                {
                    continue;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_15"))
                {
                    initialKeys[0] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_16"))
                {
                    initialKeys[1] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_17"))
                {
                    initialKeys[2] = (uint)(int)item.Operand;
                }

                if (antitamp.Body.Instructions[i + 1].Operand.ToString().Contains("V_18"))
                {
                    initialKeys[3] = (uint)(int)item.Operand;
                }
            }
        }
        public static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        public static uint RotateRight(uint value, int count)
        {
            return (value >> count) | (value << (32 - count));
        }

        private static uint[] GetArrayKeys(bool isNew)
        {
            uint[] dst = new uint[0x10];
            uint[] src = new uint[0x10];
            for (int i = 0; i < 0x10; i++)
            {
                dst[i] = initialKeys[3];
                src[i] = initialKeys[1];
                if (isNew)
                {
                    initialKeys[0] = (initialKeys[1] >> 6) | (initialKeys[1] << 26);
                    initialKeys[1] = (initialKeys[2] >> 4) | (initialKeys[2] << 28);
                    initialKeys[2] = (initialKeys[3] >> 8) | (initialKeys[3] << 24);
                    initialKeys[3] = (initialKeys[0] >> 12) | (initialKeys[0] << 22);

                }
                else if (!isNew)
                {
                    initialKeys[0] = (initialKeys[1] >> 5) | (initialKeys[1] << 27);
                    initialKeys[1] = (initialKeys[2] >> 3) | (initialKeys[2] << 29);
                    initialKeys[2] = (initialKeys[3] >> 7) | (initialKeys[3] << 25);
                    initialKeys[3] = (initialKeys[0] >> 11) | (initialKeys[0] << 21);
                }
                else
                {
                    initialKeys[0] = (initialKeys[1] >> 6) | (initialKeys[1] << 26);
                    initialKeys[1] = (initialKeys[2] >> 4) | (initialKeys[2] << 28);
                    initialKeys[2] = (initialKeys[3] >> 8) | (initialKeys[3] << 24);
                    initialKeys[3] = (initialKeys[0] >> 12) | (initialKeys[0] << 22);
                }

            }
            return DeriveKeyAntiTamp(dst, src);
        }

        public static uint[] DeriveKeyAntiTamp(uint[] dst, uint[] src)
        {
            uint[] numArray = new uint[0x10];
            for (int i = 0; i < 0x10; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        numArray[i] = dst[i] ^ src[i];
                        break;
                    case 1:
                        numArray[i] = dst[i] * src[i];
                        break;
                    case 2:
                        numArray[i] = dst[i] + src[i];
                        break;
                }
            }

            return numArray;
        }

        private static void Hash1(Stream stream, BinaryReader reader, IList<ImageSectionHeader> sections,
            ImageSectionHeader confSec)
        {
            foreach (ImageSectionHeader current in sections)
            {
                bool flag = current != confSec && current.DisplayName != ".RVA";
                if (!flag)
                {
                    continue;
                }

                int num = (int)(current.SizeOfRawData >> 2);
                int pointerToRawData = (int)current.PointerToRawData;
                stream.Position = pointerToRawData;
                for (int i = 0; i < num; i++)
                {
                    uint num2 = reader.ReadUInt32();
                    uint num3 = (initialKeys[0] ^ num2) + initialKeys[1] + initialKeys[2] * initialKeys[3];
                    initialKeys[0] = initialKeys[1];
                    initialKeys[1] = initialKeys[3];
                    initialKeys[3] = num3;
                }
            }
        }
    }
}
