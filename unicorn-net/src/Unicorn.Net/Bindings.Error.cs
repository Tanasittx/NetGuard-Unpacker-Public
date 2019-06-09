using Unicorn.Internal;

namespace Unicorn
{
    public partial class Bindings
    {
        /// <summary>
        /// Errors defined in unicorn.h
        /// </summary>
        public enum Error
        {
            /// <summary>
            /// No error.
            /// </summary>
            Ok = uc_err.UC_ERR_OK,
            /// <summary>
            /// Out of memory.
            /// </summary>
            NoMem = uc_err.UC_ERR_NOMEM,
            /// <summary>
            /// Unsupported architecture.
            /// </summary>
            Arch = uc_err.UC_ERR_ARCH,
            /// <summary>
            /// Invalid handle.
            /// </summary>
            Handle = uc_err.UC_ERR_HANDLE,
            /// <summary>
            /// Invalid or unsupported mode.
            /// </summary>
            Mode = uc_err.UC_ERR_MODE,
            /// <summary>
            /// Unsupported version.
            /// </summary>
            Version = uc_err.UC_ERR_VERSION,
            /// <summary>
            /// Quit emulation due to read on unmapped memory.
            /// </summary>
            ReadUnmapped = uc_err.UC_ERR_READ_UNMAPPED,
            /// <summary>
            /// Quit emulation due to write on unmapped memory.
            /// </summary>
            WriteUnmapped = uc_err.UC_ERR_WRITE_UNMAPPED,
            /// <summary>
            /// Quit emulation due to fetch on unmapped memory.
            /// </summary>
            FetchUnmapped = uc_err.UC_ERR_FETCH_UNMAPPED,
            /// <summary>
            /// Invalid hook type.
            /// </summary>
            Hook = uc_err.UC_ERR_HOOK,
            /// <summary>
            /// Quit emulation due to invalid instruction.
            /// </summary>
            InstructionInvalid = uc_err.UC_ERR_INSN_INVALID,
            /// <summary>
            /// Invalid memory mapping.
            /// </summary>
            Map = uc_err.UC_ERR_MAP,
            /// <summary>
            /// Quit emulation due to write on write protected memory.
            /// </summary>
            WriteProtected = uc_err.UC_ERR_WRITE_PROT,
            /// <summary>
            /// Quit emulation due to write on read protected memory.
            /// </summary>
            ReadProtected = uc_err.UC_ERR_READ_PROT,
            /// <summary>
            /// Quit emulation due to write on non-executable memory.
            /// </summary>
            FetchProtected = uc_err.UC_ERR_FETCH_PROT,
            /// <summary>
            /// Invalid argument provided.
            /// </summary>
            Argument = uc_err.UC_ERR_ARG,
            /// <summary>
            /// Unaligned read.
            /// </summary>
            ReadUnaligned = uc_err.UC_ERR_READ_UNALIGNED,
            /// <summary>
            /// Unaligned write.
            /// </summary>
            WriteUnaligned = uc_err.UC_ERR_WRITE_UNALIGNED,
            /// <summary>
            /// Unaligned fetch.
            /// </summary>
            FetchUnaligned = uc_err.UC_ERR_FETCH_UNALIGNED,
            /// <summary>
            /// Hook for this event already existed.
            /// </summary>
            HookExist = uc_err.UC_ERR_HOOK_EXIST,
            /// <summary>
            /// Insufficient resource.
            /// </summary>
            Resource = uc_err.UC_ERR_RESOURCE,
            /// <summary>
            /// Unhandled CPU exception.
            /// </summary>
            Exception = uc_err.UC_ERR_EXCEPTION,
        }
    }
}
