using System;
using System.Diagnostics;

namespace Unicorn
{
    /// <summary>
    /// Represents an <see cref="Emulator"/> context.
    /// </summary>
    public class Context : IDisposable
    {
        //TODO: Consider making strongly typed Contexts, like X86Context etc...

        internal bool _disposed;
        internal readonly Bindings.Arch _arch;
        internal readonly Bindings.Mode _mode;
        internal readonly IntPtr _context;

        internal Context(Emulator emulator)
        {
            Debug.Assert(emulator != null);

            emulator.Bindings.ContextAlloc(ref _context);

            _arch = emulator._arch;
            _mode = emulator._mode;
        }

        internal void Capture(Emulator emulator)
        {
            Debug.Assert(emulator != null);
            Debug.Assert(emulator._arch == _arch);
            Debug.Assert(emulator._mode == _mode);

            emulator.Bindings.ContextSave(_context);
        }

        internal void Restore(Emulator emulator)
        {
            Debug.Assert(emulator != null);
            Debug.Assert(emulator._arch == _arch);
            Debug.Assert(emulator._mode == _mode);

            emulator.Bindings.ContextRestore(_context);
        }

        // No real need for the dispose pattern here, but it
        // does not hurt.

        /// <summary>
        /// Finalizes the <see cref="Context"/> instance.
        /// </summary>
        ~Context()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Context"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all unmanaged and optionally managed resources used by the current instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="disposing"><c>true</c> to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            try
            {
                Bindings.Free(_context);
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception thrown during disposal of Context object.");
            }

            _disposed = true;
        }
    }
}
