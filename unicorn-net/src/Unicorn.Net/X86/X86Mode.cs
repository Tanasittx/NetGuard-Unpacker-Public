namespace Unicorn.X86
{
    /// <summary>
    /// Defines the modes of an <see cref="X86Emulator"/>.
    /// </summary>
    public enum X86Mode
    {
        // Prefixed the identifiers with a 'b' because identifiers cant start with digits.

        /// <summary>
        /// 16-bit mode.
        /// </summary>
        b16 = Bindings.Mode.x86b16,

        /// <summary>
        /// 32-bit mode.
        /// </summary>
        b32 = Bindings.Mode.x86b32,

        /// <summary>
        /// 64-bit mode.
        /// </summary>
        b64 = Bindings.Mode.x86b64
    }
}
