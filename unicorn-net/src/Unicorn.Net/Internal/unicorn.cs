using System;
using System.Runtime.InteropServices;

namespace Unicorn.Internal
{
    /// <summary>
    /// Provides DLL imports of the unicorn library.
    /// </summary>
    internal static class unicorn
    {
        #region Misc
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern int uc_version(ref int major, ref int minor);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uc_strerror(uc_err err);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_free(IntPtr mem);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_query(IntPtr uc, uc_query_type query, ref int result);

#if !RELEASE
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool uc_arch_supported(int arch); // Not used.
#endif
        #endregion

        #region Open/Close
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_open(uc_arch arch, uc_mode mode, ref IntPtr uc);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_close(IntPtr uc);
        #endregion

        #region Registers Read/Write
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_reg_read(IntPtr uc, int regid, ref long value);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_reg_write(IntPtr uc, int regid, ref long value);
        #endregion

        #region Emulator Start/Stop
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_emu_start(IntPtr uc, ulong begin, ulong end, ulong timeout, int count);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_emu_stop(IntPtr uc);
        #endregion

        #region Memory Read/Write/Map/Unmap/Protect/Regions
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_mem_map(IntPtr uc, ulong address, int size, int perms);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_mem_unmap(IntPtr uc, ulong address, int size);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_mem_write(IntPtr uc, ulong address, byte[] bytes, int size);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_mem_read(IntPtr uc, ulong address, byte[] bytes, int size);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_mem_protect(IntPtr uc, ulong address, int size, int perms);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_mem_regions(IntPtr uc, ref IntPtr regions, ref int count);
        #endregion

        #region Context Alloc/Save/Restore.
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_context_alloc(IntPtr uc, ref IntPtr context);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_context_save(IntPtr uc, IntPtr context);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_context_restore(IntPtr uc, IntPtr context);
        #endregion

        #region Hooks Add/Del
        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_hook_add(IntPtr uc, ref IntPtr hh, uc_hook_type type, IntPtr callback, IntPtr user_data, ulong address, ulong end);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_hook_add(IntPtr uc, ref IntPtr hh, uc_hook_type type, IntPtr callback, IntPtr user_data, ulong address, ulong end, int instruction);

        [DllImport("unicorn", CallingConvention = CallingConvention.Cdecl)]
        public static extern uc_err uc_hook_del(IntPtr uc, IntPtr hh);
        #endregion
    }

    #region Callbacks
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void uc_cb_hookcode(IntPtr uc, ulong address, int size, IntPtr user_data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void uc_cb_hookintr(IntPtr uc, int into, IntPtr user_data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int uc_cb_insn_in(IntPtr uc, int port, int size, IntPtr user_data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void uc_cb_insn_out(IntPtr uc, int port, int size, int value, IntPtr user_data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void uc_cb_hookmem(IntPtr uc, uc_mem_type type, ulong address, int size, ulong value, IntPtr user_data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate bool uc_cb_eventmem(IntPtr uc, uc_mem_type type, ulong address, int size, ulong value, IntPtr user_data);
    #endregion
}
