using System;
using System.Diagnostics;

namespace Unicorn
{
    /// <summary>
    /// Represents a unicorn-engine emulator.
    /// </summary>
    public abstract class Emulator : IDisposable
    {
        // To determine if we've been disposed or not.
        private bool _disposed;
        // Memory object instance which represents the memory of the emulator.
        private readonly Memory _memory;
        // Hooks object instance which represents the hooks of the emulator.
        private readonly Hooks _hooks;

        // Bindings to the unicorn engine.
        private readonly Bindings _bindings;

        // Arch with which the Emulator instance was initialized.
        internal readonly Bindings.Arch _arch;
        // Mode with which the Emulator instance was initialized.
        internal readonly Bindings.Mode _mode;

        internal Emulator(Bindings.Arch arch, Bindings.Mode mode)
        {
            _arch = arch;
            _mode = mode;
            _bindings = new Bindings();
            _memory = new Memory(this);
            _hooks = new Hooks(this);

            _bindings.Open(arch, mode);
        }

        internal Bindings Bindings => _bindings;

        /// <summary>
        /// Gets the <see cref="Unicorn.Memory"/> of the <see cref="Emulator"/>.
        /// </summary>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public Memory Memory
        {
            get
            {
                CheckDisposed();

                return _memory;
            }
        }

        /// <summary>
        /// Gets the <see cref="Unicorn.Hooks"/> of the <see cref="Emulator"/>.
        /// </summary>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public Hooks Hooks
        {
            get
            {
                CheckDisposed();

                return _hooks;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Unicorn.Context"/> of the <see cref="Emulator"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> has a differnt mode or architecture than the <see cref="Emulator"/>.</exception>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public Context Context
        {
            get
            {
                CheckDisposed();

                //TODO: Make contexts reusable so we don't create new instances and do unneeded allocations?

                var context = new Context(this);
                context.Capture(this);
                return context;
            }
            set
            {
                CheckDisposed();

                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value._disposed)
                    throw new ObjectDisposedException(null, "Can not access disposed Context object.");
                if (value._arch != _arch || value._mode != _mode)
                    throw new ArgumentException("value must have same arch and mode as the Emulator instance.", nameof(value));

                value.Restore(this);
            }
        }

        /// <summary>
        /// Starts emulation at the specified begin address and end address.
        /// </summary>
        /// <param name="begin">Address at which to begin emulation.</param>
        /// <param name="end">Address at which to end emulation.</param>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public void Start(ulong begin, ulong end)
        {
            CheckDisposed();

            Bindings.EmuStart(begin, end, 0, 0);
        }

        /// <summary>
        /// Starts emulation at the specified begin address, end address, timeout and number of instructions
        /// to execute.
        /// </summary>
        /// <param name="begin">Address at which to begin emulation.</param>
        /// <param name="end">Address at which to end emulation.</param>
        /// <param name="timeout">Duration to run emulation.</param>
        /// <param name="count">Number of instructions to execute.</param>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public void Start(ulong begin, ulong end, TimeSpan timeout, int count)
        {
            CheckDisposed();

            // Convert TimeSpan value into micro seconds.
            var microSeconds = (ulong)(Math.Round(timeout.TotalMilliseconds * 1000));
            Bindings.EmuStart(begin, end, microSeconds, count);
        }

        /// <summary>
        /// Stops the emulation.
        /// </summary>
        /// <exception cref="UnicornException">Unicorn did not return <see cref="Bindings.Error.Ok"/>.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Emulator"/> instance is disposed.</exception>
        public void Stop()
        {
            CheckDisposed();

            Bindings.EmuStop();
        }

        /// <summary>
        /// Finalizes the <see cref="Emulator"/> instance.
        /// </summary>
        ~Emulator()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Emulator"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all unmanaged and optionally managed resources used by the current instance of the <see cref="Emulator"/> class.
        /// </summary>
        /// <param name="disposing"><c>true</c> to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            //NOTE: Might consider throwing an exception here?
            try
            {
                Bindings.Close();
            }
            catch (Exception)
            {
                Debug.WriteLine("Closing of unicorn engine handle threw an exception.");
            }

            _disposed = true;
        }

        internal void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "Can not access disposed Emulator object.");
        }
    }
}
