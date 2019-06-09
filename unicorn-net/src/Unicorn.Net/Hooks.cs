using System;
using System.Diagnostics;

namespace Unicorn
{
    /// <summary>
    /// Represents the hooks of an <see cref="Emulator"/>.
    /// </summary>
    public class Hooks
    {
        // Emulator instance which owns this Hooks object instance.
        private readonly Emulator _emulator;
        private readonly MemoryHookContainer _memoryHooks;
        private readonly CodeHooksContainer _codeHooks;
        private readonly BlockHooksContainer _blockHooks;
        private readonly InstructionHookContainer _instructionHooks;
        private readonly InterruptHookContainer _interruptHooks;

        internal Hooks(Emulator emulator)
        {
            Debug.Assert(emulator != null);

            _emulator = emulator;
            _memoryHooks = new MemoryHookContainer(emulator);
            _codeHooks = new CodeHooksContainer(emulator);
            _blockHooks = new BlockHooksContainer(emulator);
            _instructionHooks = new InstructionHookContainer(emulator);
            _interruptHooks = new InterruptHookContainer(emulator);
        }

        /// <summary>
        /// Gets the <see cref="MemoryHookContainer"/> of the <see cref="Emulator"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public MemoryHookContainer Memory
        {
            get
            {
                _emulator.CheckDisposed();

                return _memoryHooks;
            }
        }

        /// <summary>
        /// Gets the <see cref="CodeHooksContainer"/> of the <see cref="Emulator"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public CodeHooksContainer Code
        {
            get
            {
                _emulator.CheckDisposed();

                return _codeHooks;
            }
        }

        /// <summary>
        /// Gets the <see cref="BlockHooksContainer"/> of the <see cref="Emulator"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public BlockHooksContainer Block
        {
            get
            {
                _emulator.CheckDisposed();

                return _blockHooks;
            }
        }

        /// <summary>
        /// Gets the <see cref="InstructionHookContainer"/> of the <see cref="Emulator"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public InstructionHookContainer Instruction
        {
            get
            {
                _emulator.CheckDisposed();

                return _instructionHooks;
            }
        }

        /// <summary>
        /// Gets the <see cref="InterruptHookContainer"/> of the <see cref="Emulator"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public InterruptHookContainer Interrupt
        {
            get
            {
                _emulator.CheckDisposed();

                return _interruptHooks;
            }
        }
    }
}
