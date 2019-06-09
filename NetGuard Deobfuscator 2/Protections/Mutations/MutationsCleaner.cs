using System.Linq;

namespace NetGuard_Deobfuscator_2.Protections.Mutations
{
    internal class MutationsCleaner : Base
    {
        public static Mutations.MutationsBase[] mutationModules { get; set; } =
        {
          new Mutations.De4Dot.De4DotCleaner(),
          new Mutations.Basic.EmptyTypeCleaner(),
          new Mutations.Basic.NativeIntCasting(),
          new Mutations.Basic.DecimalCompare(),
          new Mutations.Fields.NativeIntDecryption(),
          new Mutations.Locals.NullLocals(),
          new Mutations.Basic.SizeOfCleaner(),
          new Mutations.Basic.MathCleaner(),
          new Mutations.Basic.StringToDouble(),
          new Mutations.Basic.DateTimes(),
          new Mutations.Fields.FieldsAssignedAtStart(),
          new Mutations.Basic.AndFixer(),
          new Mutations.Branches.IFInliner(),
          new Mutations.Opaques.Public.PatternOpaqueCleaner(),
          new Mutations.De4Dot.De4DotCleaner(),
        };
        public override void Deobfuscate()
        {
            MutationsBase.ModuleDefMD = Base.ModuleDefMD;
            MutationsBase.methods = (from type in ModuleDefMD.GetTypes()
                                     where type.HasMethods
                                     from method in type.Methods
                                     where method.HasBody && method.Body.Instructions.Count > 5
                                     select method).ToList();
            bool modified = true;
            while (modified)
            {
                modified = false;
                var ab = new Protections.CodeFlow.FieldFixers.FieldsInCtor();
                ab.Deobfuscate();
                foreach (MutationsBase cflow in mutationModules)
                {
                    bool result = cflow.Deobfuscate();
                    if (result)
                        modified = true;
                }
            }
        }
    }
}
