using System.Runtime.InteropServices;

namespace Unicorn.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct uc_mem_region
    {
        public ulong begin;
        public ulong end;
        public ulong perms;
    }
}
