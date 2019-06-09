using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unicorn
{
    /// <summary>
    /// Base class of hook containers.
    /// </summary>
    public abstract class HookContainer : IEnumerable<HookHandle>
    {
        private readonly Emulator _emulator;
        private readonly List<HookHandle> _handles;

        internal HookContainer(Emulator emulator)
        {
            Debug.Assert(emulator != null);
            _emulator = emulator;
            _handles = new List<HookHandle>();
        }

        /// <summary>
        /// Gets the number of <see cref="HookHandle"/> in this <see cref="HookContainer"/>.
        /// </summary>
        public int Count => _handles.Count;

        /// <summary>
        /// Gets the <see cref="Unicorn.Emulator"/> instance which owns this <see cref="HookContainer"/>.
        /// </summary>
        protected Emulator Emulator => _emulator;

        /// <summary>
        /// Gets the list of <see cref="HookHandle"/> in this <see cref="HookContainer"/>.
        /// </summary>
        protected List<HookHandle> Handles => _handles;

        /// <summary>
        /// Base method to add a hook to <see cref="Emulator"/>.
        /// </summary>
        /// <param name="type">Type of hook.</param>
        /// <param name="callback">Pointer to callback method.</param>
        /// <param name="begin">Start address of where the hook is effective (inclusive).</param>
        /// <param name="end">End address of where the hook is effective (inclusive).</param>
        /// <returns>A <see cref="HookHandle"/> which represents hook.</returns>
        protected HookHandle Add(Bindings.HookType type, IntPtr callback, ulong begin, ulong end)
        {
            //NOTE: Not calling Emulator.CheckDispose() here because the caller should take responsibility of doing so.

            var hh = IntPtr.Zero;
            Emulator.Bindings.HookAdd(ref hh, type, callback, IntPtr.Zero, begin, end);

            var handle = new HookHandle(hh);
            _handles.Add(handle);

            return handle;
        }

        /// <summary>
        /// Removes a hook with the specified <see cref="HookHandle"/> from the <see cref="Emulator"/>.
        /// </summary>
        /// <param name="handle"><see cref="HookHandle"/> to the hook to remove.</param>
        /// <returns><c>true</c> if <paramref name="handle"/> was found and removed; otherwise <c>false</c>.</returns>
        /// 
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public bool Remove(HookHandle handle)
        {
            Emulator.CheckDisposed();

            Emulator.Bindings.HookRemove(handle._hh);
            return _handles.Remove(handle);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> which iterates through the <see cref="HookHandle"/> of the <see cref="HookContainer"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> which iterates through the <see cref="HookHandle"/> of the <see cref="HookContainer"/>.</returns>
        public IEnumerator<HookHandle> GetEnumerator() => ((IEnumerable<HookHandle>)_handles).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<HookHandle>)_handles).GetEnumerator();
    }
}
