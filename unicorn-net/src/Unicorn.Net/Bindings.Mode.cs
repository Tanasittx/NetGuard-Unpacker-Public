using Unicorn.Internal;

namespace Unicorn
{
    public partial class Bindings
    {
        /// <summary>
        /// Types of modes.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Little-endian mode default mode.
            /// </summary>
            LittleEndian = uc_mode.UC_MODE_LITTLE_ENDIAN,
            /// <summary>
            /// Big-endian mode.
            /// </summary>
            BigEndian = uc_mode.UC_MODE_BIG_ENDIAN,

            /// <summary>
            /// ARM mode.
            /// </summary>
            ARM = uc_mode.UC_MODE_ARM,
            /// <summary>
            /// Thumb mode.
            /// </summary>
            ARMThumb = uc_mode.UC_MODE_THUMB,
            /// <summary>
            /// Cortex-M series.
            /// </summary>
            ARMMClass = uc_mode.UC_MODE_MCLASS,
            /// <summary>
            /// ARMv8 A32 encodings.
            /// </summary>
            ARMv8 = uc_mode.UC_MODE_V8,

            /// <summary>
            /// MicroMips mode.
            /// </summary>
            MIPSMicro = uc_mode.UC_MODE_MICRO,
            /// <summary>
            /// MIPS III ISA mode.
            /// </summary>
            MIPS3 = uc_mode.UC_MODE_MIPS3,
            /// <summary>
            /// MIPS32R6 ISA mode.
            /// </summary>
            MIPS32R6 = uc_mode.UC_MODE_MIPS32R6,
            /// <summary>
            /// MIPS32 ISA mode.
            /// </summary>
            MIPS32 = uc_mode.UC_MODE_MIPS32,
            /// <summary>
            /// MIPS64 ISA mode.
            /// </summary>
            MIPS64 = uc_mode.UC_MODE_MIPS64,

            /// <summary>
            /// 16-bit mode.
            /// </summary>
            x86b16 = uc_mode.UC_MODE_16,
            /// <summary>
            /// 32-bit mode.
            /// </summary>
            x86b32 = uc_mode.UC_MODE_32,
            /// <summary>
            /// 64-bit mode.
            /// </summary>
            x86b64 = uc_mode.UC_MODE_64,

            /// <summary>
            /// 32-bit mode.
            /// </summary>
            PPC32 = uc_mode.UC_MODE_PPC32,
            /// <summary>
            /// 64-bit mode.
            /// </summary>
            PPC64 = uc_mode.UC_MODE_PPC64,
            /// <summary>
            /// Quad processing eXtensions mode.
            /// </summary>
            PPCQPX = uc_mode.UC_MODE_QPX,

            /// <summary>
            /// 32-bit mode.
            /// </summary>
            SPARC32 = uc_mode.UC_MODE_SPARC32,
            /// <summary>
            /// 64-bit mode.
            /// </summary>
            SPARC64 = uc_mode.UC_MODE_SPARC64,
            /// <summary>
            /// SPARCV9 mode.
            /// </summary>
            SPARCV9 = uc_mode.UC_MODE_V9,
        }
    }
}
