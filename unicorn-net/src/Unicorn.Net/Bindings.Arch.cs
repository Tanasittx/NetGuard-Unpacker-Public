using Unicorn.Internal;

namespace Unicorn
{
    public partial class Bindings
    {
        /// <summary>
        /// Types of arches.
        /// </summary>
        public enum Arch
        {
            /// <summary>
            /// ARM.
            /// </summary>
            ARM = uc_arch.UC_ARCH_ARM,

            /// <summary>
            /// ARM64.
            /// </summary>
            ARM64 = uc_arch.UC_ARCH_ARM64,

            /// <summary>
            /// MIPS.
            /// </summary>
            MIPS = uc_arch.UC_ARCH_MIPS,

            /// <summary>
            /// x86.
            /// </summary>
            x86 = uc_arch.UC_ARCH_X86,

            /// <summary>
            /// PPC.
            /// </summary>
            PPC = uc_arch.UC_ARCH_PPC,

            /// <summary>
            /// SPARC.
            /// </summary>
            SPARC = uc_arch.UC_ARCH_SPARC,

            /// <summary>
            /// M68k.
            /// </summary>
            M68k = uc_arch.UC_ARCH_M68K
        }
    }
}
