namespace Unicorn
{
    /// <summary>
    /// Represents a memory region for emulation.
    /// </summary>
    public struct MemoryRegion
    {
        private readonly ulong _begin;
        private readonly ulong _end;
        private readonly MemoryPermissions _perms;

        // Wrap the native uc_mem_region structure.
        internal MemoryRegion(ulong begin, ulong end, MemoryPermissions perms)
        {
            _begin = begin;
            _end = end;
            _perms = perms;
        }
        
        /// <summary>
        /// Gets the address at which the <see cref="MemoryRegion"/> starts.
        /// </summary>
        public ulong Begin => _begin;

        /// <summary>
        /// Gets the address at which the <see cref="MemoryRegion"/> ends.
        /// </summary>
        public ulong End => _end;

        /// <summary>
        /// Gets the permissions of the <see cref="MemoryRegion"/>.
        /// </summary>
        public MemoryPermissions Permissions => _perms;
    }
}
