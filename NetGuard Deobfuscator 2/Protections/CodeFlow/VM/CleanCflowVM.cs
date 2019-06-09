using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.CodeFlow.VM
{
    class CleanCflowVM:CodeFlowBase
    {
        private static Stream resources;
        private static bool koivm;

        public override void Deobfuscate()
        {

            resources = ((EmbeddedResource)ModuleDefMD.Resources.Find("661644340"))?.CreateReader().AsStream();
          
            Cleaner();
            
        }
        private static string getString(MethodDef method)
        {
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldstr)
                {
                    return instr.Operand.ToString();
                }
            }
            return null;
        }
        public static void Cleaner()
        {
            foreach (TypeDef typeDef in ModuleDefMD.GetTypes())
            {
                foreach (MethodDef methods in typeDef.Methods)
                {
                    if (!methods.HasBody) continue;
                    for (int i = 0; i < methods.Body.Instructions.Count; i++)
                    {
                        if (methods.Body.Instructions[i].OpCode == OpCodes.Call &&
                            methods.Body.Instructions[i].Operand is MethodDef)
                        {
                            MethodDef methods2 = (MethodDef)methods.Body.Instructions[i].Operand;
                            if (methods2.Parameters.Count == 2 && methods2.ReturnType == ModuleDefMD.CorLibTypes.Int32)
                            {
                                if (methods.Body.Instructions[i - 1].IsLdcI4() &&
                                    methods.Body.Instructions[i - 2].OpCode == OpCodes.Ldstr)
                                {
                                    if (resources == null)
                                    {
                                        foreach (MethodDef methods3 in methods2.DeclaringType.Methods)
                                        {
                                            if (!methods3.HasBody) continue;
                                            for (int z = 0; z < methods3.Body.Instructions.Count; z++)
                                            {
                                                if (methods3.Body.Instructions[z].OpCode == OpCodes.Callvirt && methods3.Body.Instructions[z].Operand.ToString().Contains("GetManifestResourceStream"))
                                                {

                                                    resources = ((EmbeddedResource)ModuleDefMD.Resources.Find(getString(methods3)))?.CreateReader().AsStream();
                                                    if (getString(methods3) == "DLL")

                                                        koivm = true;

                                                    break;
                                                }
                                            }
                                            if (resources != null)
                                                break;

                                        }
                                    }
                                    if (koivm)
                                    {
                                        foreach (MethodDef methods3 in methods2.DeclaringType.Methods)
                                        {
                                            if (!methods3.HasBody) continue;
                                            if (!methods3.Body.Instructions.Any(y => y.OpCode == OpCodes.Callvirt && y.Operand.ToString().Contains("GetManifestResourceStream"))) continue;
                                            for (int z = 0; z < methods3.Body.Instructions.Count; z++)
                                            {
                                                if (methods3.Body.Instructions[z].OpCode == OpCodes.Ldstr && methods3.Body.Instructions[z - 1].OpCode == OpCodes.Ldstr && methods3.Body.Instructions[z - 2].OpCode == OpCodes.Ldstr)
                                                {
                                                    var str = methods3.Body.Instructions[z - 2].Operand.ToString();
                                                    var key = methods3.Body.Instructions[z - 1].Operand.ToString();
                                                    var iv = methods3.Body.Instructions[z].Operand.ToString();
                                                    using (RijndaelManaged rijAlg = new RijndaelManaged())
                                                    {
                                                        rijAlg.Key = (Convert.FromBase64String(key));
                                                        rijAlg.IV = Convert.FromBase64String(iv);

                                                        // Create a decryptor to perform the stream transform.
                                                        ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                                                        // Create the streams used for decryption.
                                                        var str2 = Convert.FromBase64String(str);
                                                        using (MemoryStream msDecrypt = new MemoryStream((str2)))
                                                        {
                                                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                                            {
                                                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                                                {
                                                                    // Read the decrypted bytes from the decrypting stream
                                                                    // and place them in a string.
                                                                    var plaintext = srDecrypt.ReadToEnd();
                                                                    resources = ((EmbeddedResource)ModuleDefMD.Resources.Find(plaintext))?.CreateReader().AsStream();
                                                                }
                                                            }
                                                        }

                                                    }


                                                    break;

                                                }
                                            }
                                        }
                                    }
                                    string valueStr = methods.Body.Instructions[i - 2].Operand.ToString();
                                    int valueInt = methods.Body.Instructions[i - 1].GetLdcI4Value();
                                    var decryptedVal = Class_0.Method_0(valueStr, valueInt, resources);
                                    methods.Body.Instructions[i].OpCode = OpCodes.Nop;
                                    methods.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                                    methods.Body.Instructions[i - 2].OpCode = OpCodes.Ldc_I4;
                                    methods.Body.Instructions[i - 2].Operand = decryptedVal;

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
