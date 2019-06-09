using System;

namespace Unicorn.X86
{
    /// <summary>
    /// Represents an X86 architecture <see cref="Emulator"/>.
    /// </summary>
    public class X86Emulator : Emulator
    {
        // Registers of the X86 emulator.
        private readonly X86Registers _registers;

        /// <summary>
        /// Initializes a new instance of the <see cref="X86Emulator"/> class with the specified
        /// <see cref="X86Mode"/> to use.
        /// </summary>
        /// <param name="mode">Mode to use.</param>
        public X86Emulator(X86Mode mode) : base(Bindings.Arch.x86, (Bindings.Mode)mode)
        {
            _registers = new X86Registers(this);
        }

        /// <summary>
        /// Gets the <see cref="X86Registers"/> of the <see cref="X86Emulator"/> instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public X86Registers Registers
        {
            get
            {
                CheckDisposed();

                return _registers;
            }
        }

        /// <summary>
        /// Gets the <see cref="X86Mode"/> of the <see cref="X86Emulator"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public X86Mode Mode
        {
            get
            {
                CheckDisposed();

                return (X86Mode)_mode;
            }
        }
    }
}
