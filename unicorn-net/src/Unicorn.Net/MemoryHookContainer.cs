using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unicorn.Internal;

namespace Unicorn
{
    /// <summary>
    /// Callback for hooking memory.
    /// </summary>
    /// <param name="emulator"><see cref="Emulator"/> which raised the callback.</param>
    /// <param name="type">Type of memory access.</param>
    /// <param name="address">Address where code is being executed.</param>
    /// <param name="size">Size of data being read or written.</param>
    /// <param name="value">Data being written to memory; irrelevant if <paramref name="type"/> is <see cref="MemoryType.Read"/>.</param>
    /// <param name="userToken">Object associated with the callback.</param>
    public delegate void MemoryHookCallback(Emulator emulator, MemoryType type, ulong address, int size, ulong value, object userToken);

    /// <summary>
    /// Callback for handling invalid memory accesses.
    /// </summary>
    /// <param name="emulator"><see cref="Emulator"/> which raised the callback.</param>
    /// <param name="type">Type of memory access.</param>
    /// <param name="address">Address where code is being executed.</param>
    /// <param name="size">Size of data being read or written.</param>
    /// <param name="value">Data being written to memory; irrelevant if <paramref name="type"/> is <see cref="MemoryType.Read"/>.</param>
    /// <param name="userToken">Object associated with the callback.</param>
    /// <returns>Return <c>true</c> to continue execution; otherwise <c>false</c> to stop execution.</returns>
    public delegate bool MemoryEventHookCallback(Emulator emulator, MemoryType type, ulong address, int size, ulong value, object userToken);

    /// <summary>
    /// Represents hooks for memory of an <see cref="Emulator"/>.
    /// </summary>
    public class MemoryHookContainer : HookContainer
    {
        internal MemoryHookContainer(Emulator emulator) : base(emulator)
        {
            // Space
        }

        /// <summary>
        /// Adds a <see cref="MemoryHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="MemoryHookType"/> and user token which
        /// is called anytime the hook is triggered.
        /// </summary>
        /// 
        /// <param name="type">Type of <see cref="MemoryHookType"/>.</param>
        /// <param name="callback"><see cref="MemoryHookCallback"/> to add.</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(MemoryHookType type, MemoryHookCallback callback, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddInternal(type, callback, 1, 0, userToken);
        }

        /// <summary>
        /// Adds a <see cref="MemoryHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="MemoryHookType"/> and user token which
        /// is called when the hook is triggered within the specified start address and end address.
        /// </summary>
        /// 
        /// <param name="type">Type of <see cref="MemoryHookType"/>.</param>
        /// <param name="callback"><see cref="MemoryHookCallback"/> to add.</param>
        /// <param name="begin">Start address of where the hook is effective (inclusive).</param>
        /// <param name="end">End address of where the hook is effective (inclusive).</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <remarks>
        /// If <paramref name="begin"/> &gt; <paramref name="end"/>, the callback is called anytime the hook triggers.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(MemoryHookType type, MemoryHookCallback callback, ulong begin, ulong end, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddInternal(type, callback, begin, end, userToken);
        }

        /// <summary>
        /// Adds a <see cref="MemoryEventHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="MemoryEventHookType"/> and user token which
        /// is called anytime the hook is triggered.
        /// </summary>
        /// 
        /// <param name="type">Type of <see cref="MemoryEventHookType"/>.</param>
        /// <param name="callback"><see cref="MemoryEventHookCallback"/> to add.</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(MemoryEventHookType type, MemoryEventHookCallback callback, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddEventInternal(type, callback, 1, 0, userToken);
        }

        /// <summary>
        /// Adds a <see cref="MemoryEventHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="MemoryEventHookType"/> and user token which
        /// is called when the hook is triggered within the specified start address and end address.
        /// </summary>
        /// 
        /// <param name="type">Type of <see cref="MemoryEventHookType"/>.</param>
        /// <param name="callback"><see cref="MemoryEventHookCallback"/> to add.</param>
        /// <param name="begin">Start address of where the hook is effective (inclusive).</param>
        /// <param name="end">End address of where the hook is effective (inclusive).</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <remarks>
        /// If <paramref name="begin"/> &gt; <paramref name="end"/>, the callback is called anytime the hook triggers.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(MemoryEventHookType type, MemoryEventHookCallback callback, ulong begin, ulong end, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddEventInternal(type, callback, begin, end, userToken);
        }

        private HookHandle AddEventInternal(MemoryEventHookType type, MemoryEventHookCallback callback, ulong begin, ulong end, object userToken)
        {
            var wrapper = new uc_cb_eventmem((uc, _type, addr, size, value, user_data) =>
            {
                Debug.Assert(uc == Emulator.Bindings.UCHandle);
                return callback(Emulator, (MemoryType)_type, addr, size, value, userToken);
            });

            var ptr = Marshal.GetFunctionPointerForDelegate(wrapper);
            return Add((Bindings.HookType)type, ptr, begin, end);
        }

        private HookHandle AddInternal(MemoryHookType type, MemoryHookCallback callback, ulong begin, ulong end, object userToken)
        {
            var wrapper = new uc_cb_hookmem((uc, _type, addr, size, value, user_data) =>
            {
                Debug.Assert(uc == Emulator.Bindings.UCHandle);
                callback(Emulator, (MemoryType)_type, addr, size, value, userToken);
            });

            var ptr = Marshal.GetFunctionPointerForDelegate(wrapper);
            return Add((Bindings.HookType)type, ptr, begin, end);
        }
    }

    /// <summary>
    /// Types of memory accesses for <see cref="MemoryHookCallback"/>.
    /// </summary>
    [Flags]
    public enum MemoryHookType
    {
        /// <summary>
        /// Read memory.
        /// </summary>
        Read = Bindings.HookType.MemRead,

        /// <summary>
        /// Write memory.
        /// </summary>
        Write = Bindings.HookType.MemWrite,

        /// <summary>
        /// Fetch memory.
        /// </summary>
        Fetch = Bindings.HookType.MemFetch,

        /// <summary>
        /// Read memory successful access.
        /// </summary>
        ReadAfter = Bindings.HookType.MemReadAfter
    }

    /// <summary>
    /// Types of invalid memory accesses for <see cref="MemoryEventHookCallback"/>.
    /// </summary>
    [Flags]
    public enum MemoryEventHookType
    {
        /// <summary>
        /// Unmapped memory read.
        /// </summary>
        UnmappedRead = Bindings.HookType.MemReadUnmapped,

        /// <summary>
        /// Unmapped memory write.
        /// </summary>
        UnmappedWrite = Bindings.HookType.MemWriteUnmapped,

        /// <summary>
        /// Unmapped memory fetch.
        /// </summary>
        UnmappedFetch = Bindings.HookType.MemFetchUnmapped,

        /// <summary>
        /// Protected memory read.
        /// </summary>
        ProtectedRead = Bindings.HookType.MemReadProtected,

        /// <summary>
        /// Protected memory write.
        /// </summary>
        ProtectedWrite = Bindings.HookType.MemWriteProtected,

        /// <summary>
        /// Protected memory fetch.
        /// </summary>
        ProtectedFetch = Bindings.HookType.MemFetchProtected,
    }

    /// <summary>
    /// Types of memory accesses for memory hooks.
    /// </summary>
    public enum MemoryType
    {
        /// <summary>
        /// Memory read from.
        /// </summary>
        Read = uc_mem_type.UC_MEM_READ,
        /// <summary>
        /// Memory written to.
        /// </summary>
        Write = uc_mem_type.UC_MEM_WRITE,
        /// <summary>
        /// Memory fetched at.
        /// </summary>
        Fetch = uc_mem_type.UC_MEM_FETCH,

        /// <summary>
        /// Unmapped memory read from.
        /// </summary>
        ReadUnmapped = uc_mem_type.UC_MEM_READ_UNMAPPED,
        /// <summary>
        /// Unmapped memory written to.
        /// </summary>
        WriteUnmapped = uc_mem_type.UC_MEM_WRITE_UNMAPPED,
        /// <summary>
        /// Unmapped memory fetched at.
        /// </summary>
        FetchUnmapped = uc_mem_type.UC_MEM_FETCH_UNMAPPED,

        /// <summary>
        /// Write to write protected memory.
        /// </summary>
        WriteProtected = uc_mem_type.UC_MEM_WRITE_PROT,
        /// <summary>
        /// Read to read protected memory.
        /// </summary>
        ReadProtected = uc_mem_type.UC_MEM_READ_PROT,
        /// <summary>
        /// Fetch on non-executable memory.
        /// </summary>
        FetchProtected = uc_mem_type.UC_MEM_FETCH_PROT,

        /// <summary>
        /// Successful memory read.
        /// </summary>
        ReadAfter = uc_mem_type.UC_MEM_READ_AFTER
    }
}
