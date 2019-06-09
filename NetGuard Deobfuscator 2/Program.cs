using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.IO;

namespace NetGuard_Deobfuscator_2
{
    public class Program
    {
        public static string Path { get; private set; }
        public static Protections.Base[] modules { get; set; } =
        {
            new Protections.AntiTamper.Remover(),
            new Protections.Strings.Initalise.GetOriginalBytes(),
            new Protections.CodeFlow.CflowCleaner(),
            new Protections.ProxyCalls.Basic.StandardHiding(),
            new Protections.Mutations.MutationsCleaner(),
            new Protections.Strings.StringCleaner(),
            new Protections.ProxyCalls.ProxyCleaner(),
            new Protections.CleanUp.RemoveMethods(),
            new Protections.CleanUp.AntiDnspyClean(),
            new Protections.CleanUp.Cctor(),
            new Protections.CleanUp.IntegrityCleaner(),
            
        };
        static void Main(string[] args)
        {
            Path = Console.ReadLine();
            LoadModule(Path);
            foreach (Protections.Base @Base in modules)
            {
                Base.Deobfuscate();
            }
            SaveModule();
        }
        public static MemoryStream Unpack(string path)
        {
            Path = path;
            MemoryStream memoryStream = new MemoryStream();
            LoadModule(Path);
            if (Protections.Base.ModuleDefMD == null) return null;
            var attr = Protections.Base.ModuleDefMD.CustomAttributes;
            var detected = false;
            foreach (var att in attr)
            {
                if (!att.HasConstructorArguments) continue;
                var te = att.ConstructorArguments[0].Value;
                if (te.ToString().Contains("KoiVM v0.2.0"))


                {
                    detected = true;
                    break;
                }
            }
            if (!detected)
                return null;

            foreach (Protections.Base @Base in modules)
            {
                Base.Deobfuscate();
            }
            SaveModule(memoryStream);
            return memoryStream;
        }



        public static void SaveModule()
        {

            var moduleWriterOptions = new NativeModuleWriterOptions(Protections.Base.ModuleDefMD, false)
            {
                Logger = DummyLogger.NoThrowInstance,
                MetadataOptions = { Flags = MetadataFlags.PreserveAll },

            };
            Protections.Base.ModuleDefMD.NativeWrite(Path + "Test2.exe", moduleWriterOptions);

        }
        public static void SaveModule(MemoryStream mem)
        {

            var moduleWriterOptions = new NativeModuleWriterOptions(Protections.Base.ModuleDefMD, false)
            {
                Logger = DummyLogger.NoThrowInstance,
                MetadataOptions = { Flags = MetadataFlags.PreserveAll },

            };
            Protections.Base.ModuleDefMD.NativeWrite(mem, moduleWriterOptions);

        }

        public static void LoadAsmRef()
        {
            var asmResolver = new AssemblyResolver();
            var modCtx = new ModuleContext(asmResolver);
            asmResolver.DefaultModuleContext = modCtx;
            asmResolver.EnableTypeDefCache = true;

            Protections.Base.ModuleDefMD.Location = Path;
            var asmRefs = Protections.Base.ModuleDefMD.GetAssemblyRefs();
            Protections.Base.ModuleDefMD.Context = modCtx;
            foreach (var asmRef in asmRefs)
            {
                if (asmRef == null)
                    continue;
                var asma = asmResolver.Resolve(asmRef.FullName, Protections.Base.ModuleDefMD);


                //	Protections.Protections.ModuleDef.Context.AssemblyResolver.AddToCache(asma);
                ((AssemblyResolver)Protections.Base.ModuleDefMD.Context.AssemblyResolver).AddToCache(asma);
            }
        }

        public static void LoadModule(string path)
        {
            try
            {

                Protections.Base.ModuleDefMD = ModuleDefMD.Load(path);

                LoadAsmRef();

          //     Console.WriteLine("[+] Loaded Module!");
            }
            catch (BadImageFormatException)
            {
            //    Console.WriteLine("[!] Native Packer Detected.\r\n[!] Decrypting");
                Protections.Base.NativePacker = true;
                var arr = Protections.Native_Unpacker.Unpack.unpacker(path);
                if (arr == null)
                    return;
                Protections.Base.ModuleDefMD = ModuleDefMD.Load(arr);
                LoadAsmRef();
          //      Console.WriteLine("[!] Native Packer Removed");
            }
            catch (Exception ex) { return; }
        }
    }
}
