using System;
using Unicorn.Internal;

namespace Unicorn
{
    /// <summary>
    /// Exception thrown when the native unicorn library does not return UC_ERR_OK.
    /// </summary>
    public class UnicornException : Exception
    {
        private readonly Bindings.Error _err;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnicornException"/> class.
        /// </summary>
        public UnicornException() : base()
        {
            // Space
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnicornException"/> class with the specified message describing the cause of the exception.
        /// </summary>
        /// <param name="message">Message describing the cause of the exception.</param>
        public UnicornException(string message) : base(message)
        {
            // Space
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicornException"/> class with the specified <see cref="Bindings.Error"/>.
        /// </summary>
        /// <param name="error"><see cref="Bindings.Error"/> error code.</param>
        public UnicornException(Bindings.Error error) : base(Bindings.ErrorToString(error))
        {
            _err = error;
        }

        internal UnicornException(uc_err err) : this((Bindings.Error)err)
        {
            // Space
        }

        /// <summary>
        /// Gets the <see cref="Bindings.Error"/> of the <see cref="UnicornException"/>.
        /// </summary>
        public Bindings.Error ErrorCode => _err;
    }
}
