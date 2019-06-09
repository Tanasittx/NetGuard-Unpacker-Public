using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unicorn.Internal;
using Unicorn.X86;

namespace Unicorn
{
    /// <summary>
    /// Callback for tracing <see cref="X86Instructions.IN"/>.
    /// </summary>
    /// <param name="emulator"><see cref="Emulator"/> which raised the callback.</param>
    /// <param name="port">Port number.</param>
    /// <param name="size">Data size read from <paramref name="port"/>.</param>
    /// <param name="userToken">Object associated with the callback.</param>
    /// <returns>Data read.</returns>
    public delegate int InstructionInHookCallback(Emulator emulator, int port, int size, object userToken);

    /// <summary>
    /// Callback for tracing <see cref="X86Instructions.OUT"/>.
    /// </summary>
    /// <param name="emulator"><see cref="Emulator"/> which raised the callback.</param>
    /// <param name="port">Port number.</param>
    /// <param name="size">Data size written to <paramref name="port"/>.</param>
    /// <param name="value">Data to be written to <paramref name="port"/>.</param>
    /// <param name="userToken">Object associated with the callback.</param>
    public delegate void InstructionOutHookCallback(Emulator emulator, int port, int size, int value, object userToken);

    /// <summary>
    /// Represents hooks for instructions of an <see cref="Emulator"/>.
    /// </summary>
    public class InstructionHookContainer : HookContainer
    {
        internal InstructionHookContainer(Emulator emulator) : base(emulator)
        {
            // Space
        }

        /// <summary>
        /// Adds a <see cref="InstructionInHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="Instruction"/> and user token which
        /// is called anytime the hook is triggered.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="InstructionInHookCallback"/> to add.</param>
        /// <param name="instruction"><see cref="Instruction"/> to hook.</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(InstructionInHookCallback callback, Instruction instruction, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddInInternal(callback, instruction, 1, 0, userToken);
        }

        /// <summary>
        /// Adds a <see cref="InstructionInHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="Instruction"/> and user token which
        /// is called when the hook is triggered within the specified start address and end address.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="InstructionInHookCallback"/> to add.</param>
        /// <param name="instruction"><see cref="Instruction"/> to hook.</param>
        /// <param name="begin">Start address of where the hook is effective (inclusive).</param>
        /// <param name="end">End address of where the hook is effective (inclusive).</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(InstructionInHookCallback callback, Instruction instruction, ulong begin, ulong end, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddInInternal(callback, instruction, begin, end, userToken);
        }

        /// <summary>
        /// Adds a <see cref="InstructionOutHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="Instruction"/> and user token which
        /// is called anytime the hook is triggered.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="InstructionOutHookCallback"/> to add.</param>
        /// <param name="instruction"><see cref="Instruction"/> to hook.</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(InstructionOutHookCallback callback, Instruction instruction, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddOutInternal(callback, instruction, 1, 0, userToken);
        }

        /// <summary>
        /// Adds a <see cref="InstructionOutHookCallback"/> to the <see cref="Emulator"/> with the specified <see cref="Instruction"/> and user token which
        /// is called when the hook is triggered within the specified start address and end address.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="InstructionOutHookCallback"/> to add.</param>
        /// <param name="instruction"><see cref="Instruction"/> to hook.</param>
        /// <param name="begin">Start address of where the hook is effective (inclusive).</param>
        /// <param name="end">End address of where the hook is effective (inclusive).</param>
        /// <param name="userToken">Object associated with the callback.</param>
        /// <returns>A <see cref="HookHandle"/> which represents the hook.</returns>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public HookHandle Add(InstructionOutHookCallback callback, Instruction instruction, ulong begin, ulong end, object userToken)
        {
            Emulator.CheckDisposed();

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return AddOutInternal(callback, instruction, begin, end, userToken);
        }

        private HookHandle AddInInternal(InstructionInHookCallback callback, Instruction instruction, ulong begin, ulong end, object userToken)
        {
            var wrapper = new uc_cb_insn_in((uc, port, size, user_data) =>
            {
                Debug.Assert(uc == Emulator.Bindings.UCHandle);
                return callback(Emulator, port, size, userToken);
            });

            var ptr = Marshal.GetFunctionPointerForDelegate(wrapper);
            return AddInternal(ptr, begin, end, instruction);
        }

        private HookHandle AddOutInternal(InstructionOutHookCallback callback, Instruction instruction, ulong begin, ulong end, object userToken)
        {
            var wrapper = new uc_cb_insn_out((uc, port, size, value, user_data) =>
            {
                Debug.Assert(uc == Emulator.Bindings.UCHandle);
                callback(Emulator, port, size, value, userToken);
            });

            var ptr = Marshal.GetFunctionPointerForDelegate(wrapper);
            return AddInternal(ptr, begin, end, instruction);
        }

        private HookHandle AddInternal(IntPtr callback, ulong begin, ulong end, Instruction inst)
        {
            var ptr = IntPtr.Zero;
            Emulator.Bindings.HookAdd(ref ptr, Bindings.HookType.Instructions, callback, IntPtr.Zero, begin, end, inst._id);

            var handle = new HookHandle(ptr);
            Handles.Add(handle);

            return handle;
        }
    }
}
