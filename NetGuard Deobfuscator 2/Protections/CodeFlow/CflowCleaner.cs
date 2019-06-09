namespace NetGuard_Deobfuscator_2.Protections.CodeFlow
{
    class CflowCleaner : Base
    {
        public static CodeFlow.CodeFlowBase[] CflowModules { get; set; } =
        {
          new FieldFixers.NativeMethodCleaner(),
          new FieldFixers.FieldsInCtor(),
          new FieldFixers.FieldsInFirstCall(),
          new CflowCleanersHelpers.OneTwoCleaner(),
          new VM.CleanCflowVM(),
          new CflowCleaning.ControlFlowRemover(),
        };
        public override void Deobfuscate()
        {
            CodeFlowBase.ModuleDefMD = Base.ModuleDefMD;
            foreach (CodeFlowBase cflow in CflowModules)
            {
                cflow.Deobfuscate();
            }
        }
    }
}
