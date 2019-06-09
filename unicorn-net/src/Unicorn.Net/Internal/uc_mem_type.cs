namespace Unicorn.Internal
{
    internal enum uc_mem_type
    {
        // From unicorn.h

        UC_MEM_READ = 16,           // Memory is read from
        UC_MEM_WRITE,               // Memory is written to
        UC_MEM_FETCH,               // Memory is fetched
        UC_MEM_READ_UNMAPPED,       // Unmapped memory is read from
        UC_MEM_WRITE_UNMAPPED,      // Unmapped memory is written to
        UC_MEM_FETCH_UNMAPPED,      // Unmapped memory is fetched
        UC_MEM_WRITE_PROT,          // Write to write protected, but mapped, memory
        UC_MEM_READ_PROT,           // Read from read protected, but mapped, memory
        UC_MEM_FETCH_PROT,          // Fetch from non-executable, but mapped, memory
        UC_MEM_READ_AFTER,          // Memory is read from (successful access)
    }
}
