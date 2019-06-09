using System;

namespace Unicorn.Mips
{
    /// <summary>
    /// Represents a MIPS architecture <see cref="Emulator"/>.
    /// </summary>
    public class MipsEmulator : Emulator
    {
        // Registsers for the MIPS emulator.
        private readonly MipsRegisters _registers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MipsEmulator"/> class with the specified
        /// <see cref="MipsMode"/> to use.
        /// </summary>
        /// <param name="mode">Mode to use.</param>
        public MipsEmulator(MipsMode mode) : base(Bindings.Arch.MIPS, (Bindings.Mode)mode)
        {
            _registers = new MipsRegisters(this);
        }

        /// <summary>
        /// Gets the <see cref="MipsRegisters"/> of the <see cref="MipsRegisters"/> instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public MipsRegisters Registers
        {
            get
            {
                CheckDisposed();

                return _registers;
            }
        }

        /// <summary>
        /// Gets the <see cref="MipsMode"/> of the <see cref="MipsEmulator"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public MipsMode Mode
        {
            get
            {
                CheckDisposed();

                return (MipsMode)_mode;
            }
        }
    }
}
