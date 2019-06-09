using System;
using System.Runtime.InteropServices;
using Unicorn.Internal;

namespace Unicorn
{
    /// <summary>
    /// A thin layer bind to the Unicorn engine.
    /// </summary>
    public partial class Bindings
    {
        //TODO: Implement the other stuff as well. E.g: uc_reg_read_batch.

        /// <summary>
        /// Binds to uc_version.
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <returns></returns>
        public static int Version(ref int major, ref int minor)
        {
            return unicorn.uc_version(ref major, ref minor);
        }

        /// <summary>
        /// Binds to uc_strerror.
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static string ErrorToString(Error err)
        {
            var ptr = unicorn.uc_strerror((uc_err)err);
            var errString = Marshal.PtrToStringAnsi(ptr);
            return errString;
        }

        /// <summary>
        /// Binds to uc_free.
        /// </summary>
        /// <param name="addr"></param>
        public static void Free(IntPtr addr)
        {
            var err = unicorn.uc_free(addr);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        private IntPtr _uc;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bindings"/> class.
        /// </summary>
        public Bindings()
        {
            // Space
        }

        /// <summary>
        /// Gets the handle of the <see cref="Bindings"/> returned by <see cref="Open(Arch, Mode)"/>.
        /// </summary>
        public IntPtr UCHandle => _uc;

        /// <summary>
        /// Binds to uc_open.
        /// </summary>
        /// <param name="arch"></param>
        /// <param name="mode"></param>
        public void Open(Arch arch, Mode mode)
        {
            var err = unicorn.uc_open((uc_arch)arch, (uc_mode)mode, ref _uc);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_close.
        /// </summary>
        public void Close()
        {
            var err = unicorn.uc_close(_uc);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_emu_start.
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="timeout"></param>
        /// <param name="count"></param>
        public void EmuStart(ulong begin, ulong end, ulong timeout, int count)
        {
            var err = unicorn.uc_emu_start(_uc, begin, end, timeout, count);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_emu_stop.
        /// </summary>
        public void EmuStop()
        {
            var err = unicorn.uc_emu_stop(_uc);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_reg_read.
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="value"></param>
        public void RegRead(int regId, ref long value)
        {
            var err = unicorn.uc_reg_read(_uc, regId, ref value);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_reg_write.
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="value"></param>
        public void RegWrite(int regId, ref long value)
        {
            var err = unicorn.uc_reg_write(_uc, regId, ref value);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_mem_regions.
        /// </summary>
        /// <param name="regions"></param>
        /// <param name="count"></param>
        public void MemRegions(ref IntPtr regions, ref int count)
        {
            var err = unicorn.uc_mem_regions(_uc, ref regions, ref count);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_mem_regions but manages the reading of uc_mem_region structures.
        /// </summary>
        /// <param name="regions"></param>
        public void MemRegions(ref MemoryRegion[] regions)
        {
            var count = 0;
            var ptr = IntPtr.Zero;

            MemRegions(ref ptr, ref count);

            regions = new MemoryRegion[count];
            var size = Marshal.SizeOf(typeof(uc_mem_region));
            for (int i = 0; i < count; i++)
            {
                var nativeStruct = (uc_mem_region)Marshal.PtrToStructure(ptr, typeof(uc_mem_region));
                var region = new MemoryRegion(nativeStruct.begin, nativeStruct.end, (MemoryPermissions)nativeStruct.perms);

                regions[i] = region;
                ptr += size;
            }

            Free(ptr);
        }

        /// <summary>
        /// Binds to uc_mem_map.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        /// <param name="permissions"></param>
        public void MemMap(ulong address, int size, MemoryPermissions permissions)
        {
            var err = unicorn.uc_mem_map(_uc, address, size, (int)permissions);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_mem_unmap.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        public void MemUnmap(ulong address, int size)
        {
            var err = unicorn.uc_mem_unmap(_uc, address, size);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_mem_protect
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        /// <param name="permissions"></param>
        public void MemProtect(ulong address, int size, MemoryPermissions permissions)
        {
            var err = unicorn.uc_mem_protect(_uc, address, size, (int)permissions);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_mem_write.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        public void MemWrite(ulong address, byte[] buffer, int count)
        {
            var err = unicorn.uc_mem_write(_uc, address, buffer, count);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_mem_read.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        public void MemRead(ulong address, byte[] buffer, int count)
        {
            var err = unicorn.uc_mem_read(_uc, address, buffer, count);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_context_alloc.
        /// </summary>
        /// <param name="ctx"></param>
        public void ContextAlloc(ref IntPtr ctx)
        {
            var err = unicorn.uc_context_alloc(_uc, ref ctx);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_context_save.
        /// </summary>
        /// <param name="ctx"></param>
        public void ContextSave(IntPtr ctx)
        {
            var err = unicorn.uc_context_save(_uc, ctx);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_context_restore.
        /// </summary>
        /// <param name="ctx"></param>
        public void ContextRestore(IntPtr ctx)
        {
            var err = unicorn.uc_context_restore(_uc, ctx);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_hook_add.
        /// </summary>
        /// <param name="hh"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <param name="address"></param>
        /// <param name="end"></param>
        public void HookAdd(ref IntPtr hh, HookType type, IntPtr callback, IntPtr userData, ulong address, ulong end)
        {
            var err = unicorn.uc_hook_add(_uc, ref hh, (uc_hook_type)type, callback, userData, address, end);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_hook_add.
        /// </summary>
        /// <param name="hh"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        /// <param name="address"></param>
        /// <param name="end"></param>
        /// <param name="instruction"></param>
        public void HookAdd(ref IntPtr hh, HookType type, IntPtr callback, IntPtr userData, ulong address, ulong end, int instruction)
        {
            var err = unicorn.uc_hook_add(_uc, ref hh, (uc_hook_type)type, callback, userData, address, end, instruction);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_hook_del.
        /// </summary>
        /// <param name="hh"></param>
        public void HookRemove(IntPtr hh)
        {
            var err = unicorn.uc_hook_del(_uc, hh);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }

        /// <summary>
        /// Binds to uc_query.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void Query(QueryType type, ref int value)
        {
            var err = unicorn.uc_query(_uc, (uc_query_type)type, ref value);
            if (err != uc_err.UC_ERR_OK)
                throw new UnicornException(err);
        }
    }
}
