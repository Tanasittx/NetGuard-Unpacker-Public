using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unicorn.Internal;

namespace Unicorn
{
    /// <summary>
    /// Callback for tracing interrupts. 
    /// </summary>
    /// <param name="emulator"><see cref="Emulator"/> which raised the callback.</param>
    /// <param name="into">Interrupt number.</param>
    /// <param name="userToken">Object associated with the callback.</param>
    public delegate void InterruptHookCallback(Emulator emulator, int into, object userToken);

    /// <summary>
    /// Represents hooks for interrupts of an <see cref="Emulator"/>.
    /// </summary>
    public class InterruptHookContainer : HookContainer
    {
        internal InterruptHookContainer(Emulator emulator) : base(emulator)
        {
            // Space
        }

        /// <summary>
        /// Adds a <see cref="InterruptHookCallback"/> to the <see cref="Emulator"/> with the specified user token which
        /// is called anytime the hook is triggered.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="InterruptHookCallback"/> to add.</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(InterruptHookCallback callback, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddInternal(callback, 1, 0, userToken);
        }

        /// <summary>
        /// Adds a <see cref="InterruptHookCallback"/> to the <see cref="Emulator"/> with the specified user token which
        /// is called when the hook is triggered within the specified start address and end address.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="InterruptHookCallback"/> to add.</param>
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
        public HookHandle Add(InterruptHookCallback callback, ulong begin, ulong end, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddInternal(callback, begin, end, userToken);
        }

        private HookHandle AddInternal(InterruptHookCallback callback, ulong begin, ulong end, object userToken)
        {
            var wrapper = new uc_cb_hookintr((uc, into, user_data) =>
            {
                Debug.Assert(uc == Emulator.Bindings.UCHandle);
                callback(Emulator, into, userToken);
            });

            var ptr = Marshal.GetFunctionPointerForDelegate(wrapper);
            return Add(Bindings.HookType.Interrupts, ptr, begin, end);
        }
    }
}
