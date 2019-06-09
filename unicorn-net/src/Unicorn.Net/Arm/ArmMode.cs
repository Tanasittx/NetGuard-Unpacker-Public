namespace Unicorn.Arm
{
    /// <summary>
    /// Defines the modes of an <see cref="ArmEmulator"/>.
    /// </summary>
    public enum ArmMode
    {
        /// <summary>
        /// ARM mode.
        /// </summary>
        Arm = Bindings.Mode.ARM,

        /// <summary>
        /// Thumb mode.
        /// </summary>
        Thumb = Bindings.Mode.ARMThumb,

        /// <summary>
        /// Cortext-M series mode.
        /// </summary>
        MClass = Bindings.Mode.ARMMClass,

        /// <summary>
        /// ARMv8 A32 encoding mode.
        /// </summary>
        v8 = Bindings.Mode.ARMv8
    }
}
