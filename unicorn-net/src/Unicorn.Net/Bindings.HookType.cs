using Unicorn.Internal;

namespace Unicorn
{
    public partial class Bindings
    {
        /// <summary>
        /// Types of hooks.
        /// </summary>
        public enum HookType
        {
            /// <summary>
            /// Interrupts/Syscalls.
            /// </summary>
            Interrupts = uc_hook_type.UC_HOOK_INTR,
            /// <summary>
            /// Particular instruction.
            /// </summary>
            Instructions = uc_hook_type.UC_HOOK_INSN,

            /// <summary>
            /// Range of code.
            /// </summary>
            Code = uc_hook_type.UC_HOOK_CODE,
            /// <summary>
            /// Basic block.
            /// </summary>
            Block = uc_hook_type.UC_HOOK_BLOCK,

            /// <summary>
            /// Memory read on unmapped memory.
            /// </summary>
            MemReadUnmapped = uc_hook_type.UC_HOOK_MEM_READ_UNMAPPED,
            /// <summary>
            /// Memory write on unmapped memory.
            /// </summary>
            MemWriteUnmapped = uc_hook_type.UC_HOOK_MEM_WRITE_UNMAPPED,
            /// <summary>
            /// Memory fetch on unmapped memory.
            /// </summary>
            MemFetchUnmapped = uc_hook_type.UC_HOOK_MEM_FETCH_UNMAPPED,

            /// <summary>
            /// Memory read on read-protected memory.
            /// </summary>
            MemReadProtected = uc_hook_type.UC_HOOK_MEM_READ_PROT,
            /// <summary>
            /// Memory write on write-protected memory.
            /// </summary>
            MemWriteProtected = uc_hook_type.UC_HOOK_MEM_WRITE_PROT,
            /// <summary>
            /// Memory fetch on non-executable memory.
            /// </summary>
            MemFetchProtected = uc_hook_type.UC_HOOK_MEM_FETCH_PROT,

            /// <summary>
            /// Memory reads.
            /// </summary>
            MemRead = uc_hook_type.UC_HOOK_MEM_READ,
            /// <summary>
            /// Memory writes.
            /// </summary>
            MemWrite = uc_hook_type.UC_HOOK_MEM_WRITE,
            /// <summary>
            /// Memory fetches.
            /// </summary>
            MemFetch = uc_hook_type.UC_HOOK_MEM_FETCH,

            /// <summary>
            /// Successful memory reads.
            /// </summary>
            MemReadAfter = uc_hook_type.UC_HOOK_MEM_READ_AFTER,
        }
    }
}
