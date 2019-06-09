using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetGuard_Deobfuscator_2.Protections.Native_Unpacker
{
    internal class RC4
    {
        private readonly byte[] S = new byte[256];

        private int x;
        private int y;

        public RC4(byte[] key)
        {
            init(key);
        }

        // Key-Scheduling Algorithm
        // Алгоритм ключевого расписания
        private void init(byte[] key)
        {
            var keyLength = key.Length;

            for (var i = 0; i < 256; i++)
                S[i] = (byte)i;

            var j = 0;
            for (var i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % keyLength]) % 256;
                S.Swap(i, j);
            }
        }

        public byte[] Encode(byte[] dataB, int size)
        {
            var data = dataB.Take(size).ToArray();

            var cipher = new byte[data.Length];

            for (var m = 0; m < data.Length; m++)
                cipher[m] = (byte)(data[m] ^ keyItem());

            return cipher;
        }

        public byte[] Decode(byte[] dataB, int size)
        {
            return Encode(dataB, size);
        }

        // Pseudo-Random Generation Algorithm
        // Генератор псевдослучайной последовательности
        private byte keyItem()
        {
            x = (x + 1) % 256;
            y = (y + S[x]) % 256;

            S.Swap(x, y);

            return S[(S[x] + S[y]) % 256];
        }
    }

    internal static class SwapExt
    {
        public static void Swap<T>(this T[] array, int index1, int index2)
        {
            var temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }

    internal class Unpack
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);


        [DllImport("kernel32.dll")]
        private static extern IntPtr FindResource(IntPtr hModule, string lpID, string lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        public static byte[] valid = new byte[] { 0x4D, 0x5A };
        public static byte[] valid_gzip = new byte[] { 0x1F, 0x8B };
        public static byte[] gzip_decompress(byte[] decoded)
        {
            var to = new MemoryStream();
            var from = new MemoryStream(decoded);
            new GZipStream(from, CompressionMode.Decompress).CopyTo(to);
            from.Close();
            return to.ToArray();
        }

        public static byte[] FindMaxLength(List<byte[]> list)
        {
            byte[] result = null;
            int Max_Length = int.MinValue;
            foreach (byte[] type in list)
            {
                if (type.Length > Max_Length)
                {
                    Max_Length = type.Length;
                    result = type;
                }
            }
            return result;
        }

        public static byte[] DecryptAES(byte[] src, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.ECB;
            using (ICryptoTransform decrypt = aes.CreateDecryptor(key, null))
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                decrypt.Dispose();
                return dest;
            }
        }

        public static byte[] AES_Decrypt_Method(byte[] raw_resource, out byte[] aes_key, out byte[] xor_key)
        {
            try
            {
                byte[] decoded = null;
                Stream stream = new MemoryStream(raw_resource);
                stream.Position = stream.Length - 1;

                int l = stream.ReadByte();
                stream.Position = stream.Length - 1 - l;

                byte[] key = new byte[l];
                stream.Read(key, 0, l);
                //File.WriteAllBytes(@"C:\Users\owner\source\repos\NetGuard Native Stub Protection\NetGuard Native Stub Protection\bin\Debug\key.bin", key);
                SHA1Managed sha1 = new SHA1Managed();
                aes_key = sha1.ComputeHash(key).Take(0x10).ToArray();
                xor_key = key;

                byte[] bPtr_real = new byte[stream.Length - 1 - l];
                stream.Position = 0;
                stream.Read(bPtr_real, 0, (int)stream.Length - 1 - l);

                decoded = DecryptAES(bPtr_real, aes_key);

                if (decoded.Take(2).SequenceEqual(valid))
                {
                    return decoded;
                }
                else if (decoded.Take(2).SequenceEqual(valid_gzip))
                {
                    decoded = gzip_decompress(decoded);
                    if (decoded.Take(2).SequenceEqual(valid))
                    {
                        return decoded;
                    }
                    else return null;
                }
                return null;
            }
            catch
            {
                aes_key = null;
                xor_key = null;
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
                if (decoded.Take(2).SequenceEqual(valid))
                {
                    return decoded;
                }
                else if (decoded.Take(2).SequenceEqual(valid_gzip))
                {
                    decoded = gzip_decompress(decoded);
                    if (decoded.Take(2).SequenceEqual(valid))
                    {
                        return decoded;
                    }
                    else continue;
                }
            }
            return null;
        }

        public static byte[] unpacker(string path)
        {
            var abc2 = AsmResolver.WindowsAssembly.FromFile(path).RootResourceDirectory;

            byte[] bPtr = null;
            List<byte[]> res_data = new List<byte[]>();
            var resources2 = abc2.Entries.Where(i => i.Name == "RC_DATA").ToArray().FirstOrDefault();
            if (resources2 == null) return null;
            var resources = resources2.SubDirectory.Entries;
            for (int i = 0; i < resources.Count; i++)
            {
                res_data.Add(resources[i].SubDirectory.Entries[0].DataEntry.Data);
            }

            bPtr = FindMaxLength(res_data);

            byte[] decoded = null;

            decoded = RC4_Decrypt_Method(bPtr);
            if (decoded != null)
            {
                return decoded;
            }
            byte[] aes_key = null;
            byte[] xor_key = null;
            decoded = AES_Decrypt_Method(bPtr, out aes_key, out xor_key);
            if (decoded != null)
            {
                Helper.aes_key = aes_key;
                Helper.xor_key = xor_key;
                return decoded;
            }
            //Console.WriteLine("ERROR: Cannot decrypt native resource.");
            throw new NotSupportedException();
        }
    }
}
