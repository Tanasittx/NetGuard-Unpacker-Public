# Unicorn.Net
Slightly fancier *WIP* .NET binding/wrapper for the [Unicorn engine](https://github.com/unicorn-engine/unicorn).

#### NOTE
The API is very prone to changes at the moment.

### Examples

Here is an example of how to use it. This is also the same example as the official documentation available [here](http://www.unicorn-engine.org/docs/tutorial.html) but in C# and using Unicorn.Net.
```csharp
using (var emulator = new X86Emulator(X86Mode.b32))
{
    ulong addr = 0x1000000;
    byte[] x86code =
    {
        0x41, // INC ECX
        0x4a  // DEC EDX
    };

    var ecx = 0x1234;
    var edx = 0x7890;

    // Map 2mb of memory.
    emulator.Memory.Map(addr, 2 * 1024 * 1024, MemoryPermissions.All);

    emulator.Registers.ECX = ecx;
    emulator.Registers.EDX = edx;

    emulator.Memory.Write(addr, x86code, x86code.Length);

    emulator.Start(addr, addr + (ulong)x86code.Length);

    Console.WriteLine(emulator.Registers.ECX);
    Console.WriteLine(emulator.Registers.EDX);
}
```

#### Registers
Reading and writing to registers.
##### NOTE
Currently there is no way to write to registers using register IDs, but this may change.
```csharp
// Assume emulator is an instance of the X86Emulator type.

// Reading from registers.
var val = emulator.Registers.ECX;
// Writing to registers.
emulator.Registers.ECX = 0x10;
```

#### Memory
Mapping, unmapping, reading, writing and changing memory permissions.
```csharp
var addr = 0x100000;
// Getting memory regions.
var regions = emulator.Memory.Regions;
// Getting memory page size.
var pageSize = emulator.Memory.PageSize;

// Mapping memory.
emulator.Memory.Map(addr, 2 * 1024, 2 * 1024, MemoryPermissions.All);
// Unmapping memory.
emulator.Memory.Unmap(addr + (4 * 1024), 4 * 1024);
// Changing memory permissions.
emulator.Memory.Protect(addr + (4 * 1024), 4 * 1024, MemoryPermissions.Read);

// Code to write to memory.
var code = new byte[]
{
    0x41, // INC ECX
    0x4a  // DEC EDX
}

// Buffer thats going to be the storage for data read from memory.
var buffer = new byte[2];

// Writing to memory.
emulator.Memory.Write(code, 0, code.Length);
// Reading to memory.
emulator.Memory.Read(buffer, 0, buffer.Length);
```

#### Hooking
Currently hooking is still under the works and may change.
```csharp
// Adding a memory read hook.
emulator.Hooks.Memory.Add(MemoryHookType.Read, (emu, type, address, size, val, userData) => {
    Console.WriteLine(" stuff was read from memory.");
}, addr, addr + (ulong)code.Length, null);
```

#### Contexts
Capturing and restoring contexts.
```csharp
// emulator.Context will create a new Context object
// which you will to dispose afterwards. Hence the `using` statement.

// Capturing the context.
using (var ctx = emulator.Context)
{
    ...
    
    // To restore the context simply do this.
    emulator.Context = ctx;
}
```

#### Raw Bindings
Unicorn.Net also provide some raw bindings through the `Bindings` class.
```csharp
var bindings = new Bindings();
bindings.Open(...);
bindings.MemMap(...);
...
```


### TODO
List of stuff thats needs to be implemented or that has been implemented.
- [x] Emulator
    - [x] uc_emu_start
    - [x] uc_emu_stop
    - [x] uc_query
- [x] Context
    - [x] uc_context_alloc
    - [x] uc_context_save
    - [x] uc_context_restore
- [x] Registers
    - [x] uc_reg_read
    - [x] uc_reg_write
- [x] Memory
    - [x] uc_mem_write
    - [x] uc_mem_read
    - [x] uc_mem_protect
    - [x] uc_mem_regions
    - [x] uc_mem_map
    - [x] uc_mem_unmap
- [ ] Hooking
    - [ ] uc_hook_add
    - [ ] uc_hook_del
- [ ] Arches
    - [x] x86
    - [ ] arm
    - [ ] arm64
    - [ ] m68k
    - [ ] mips
    - [ ] sparc
    
- [ ] Actual bindings

### Licensing
Unicorn.Net is licensed under the permissive [MIT License](/LICENSE).
