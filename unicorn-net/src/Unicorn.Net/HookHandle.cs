using System;

namespace Unicorn
{
    /// <summary>
    /// Represents a hook handle.
    /// </summary>
    public struct HookHandle
    {
        internal readonly IntPtr _hh;

        internal HookHandle(IntPtr hh)
        {
            _hh = hh;
        }
    }
}
