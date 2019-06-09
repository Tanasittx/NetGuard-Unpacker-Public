namespace Unicorn.Mips
{
    /// <summary>
    /// Defines the modes of an <see cref="MipsEmulator"/>.
    /// </summary>
    public enum MipsMode
    {
        /// <summary>
        /// Big endian mode.
        /// </summary>
        BigEndian = Bindings.Mode.BigEndian,

        /// <summary>
        /// Little endian mode.
        /// </summary>
        LittleEndian = Bindings.Mode.LittleEndian,

        /// <summary>
        /// MicroMips mode.
        /// </summary>
        Micro = Bindings.Mode.MIPSMicro,

        /// <summary>
        /// MIPS III ISA mode.
        /// </summary>
        III = Bindings.Mode.MIPS3,

        /// <summary>
        /// MIPS32R6 ISA mode.
        /// </summary>
        b32R6 = Bindings.Mode.MIPS32R6,

        /// <summary>
        /// MIPS32 ISA mode.
        /// </summary>
        b32 = Bindings.Mode.MIPS32,
        
        /// <summary>
        /// MIPS64 ISA mode.
        /// </summary>
        b64 = Bindings.Mode.MIPS64,
    }
}
