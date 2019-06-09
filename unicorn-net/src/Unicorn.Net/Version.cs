namespace Unicorn
{
    /// <summary>
    /// Represents a version number.
    /// </summary>
    public struct Version
    {
        private static readonly Version _current;

        static Version()
        {
            var mmajor = 0;
            var mminor = 0;

            var nativeVersion = Bindings.Version(ref mmajor, ref mminor);
            var major = nativeVersion >> 0x8;
            var minor = nativeVersion & 0xF;

            _current = new Version(major, minor);
        }

        /// <summary>
        /// Gets the current <see cref="Version"/> of the wrapped unicorn-engine library.
        /// </summary>
        public static Version Current => _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> structure with the specified
        /// major and minor version number.
        /// </summary>
        /// <param name="major">Major version number.</param>
        /// <param name="minor">Minor version number.</param>
        public Version(int major, int minor)
        {
            _major = major;
            _minor = minor;
        }

        private readonly int _major;
        private readonly int _minor;

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        public int Major => _major;

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        public int Minor => _minor;

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString() => _major + "." + _minor;
    }
}
