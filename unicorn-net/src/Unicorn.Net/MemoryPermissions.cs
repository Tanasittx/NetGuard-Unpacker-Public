using System;

namespace Unicorn
{
    /// <summary>
    /// Defines memory region permissions.
    /// </summary>
    [Flags]
    public enum MemoryPermissions
    {
        /// <summary>
        /// No permission.
        /// </summary>
        None = 0,

        /// <summary>
        /// Read permission.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Write permission.
        /// </summary>
        Write = 2,

        /// <summary>
        /// Execute permission.
        /// </summary>
        Execute = 4,

        /// <summary>
        /// All permission.
        /// </summary>
        All = Read | Write | Execute
    }
}
