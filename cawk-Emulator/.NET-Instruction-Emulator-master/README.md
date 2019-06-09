# .NET-Instruction-Emulator
This project is an extremely helpful toolkit for any reverser dealing with complicated MSIL. using this you can emulate certain instructions complete methods or even just 1 instruction. this can be extremely helpful for many obfuscators i mainly use this in confuserex appfuscator and netguard

this can replace invoking of most methods as you can just run the instructions with this emulator and you have complete control over which instructions are ran and contains events so you can intercept certain instructions

this requires fw 4.0+ since this uses dynamic variables a few people have said that its a bad idea to use dynamic variables in this project however this is incorrect since an emulator is not made for performance rather its accuracy of emulating and getting the correct result they keep the code alot cleaner and easier to understand.

# Usage
to use this you just supply the method along with the start of the instructions to the end of the instructions

call/callvirt is included but the implementation is not good this invokes the method or atleast tries to i didnt bother coding it well as if you are emulating a method you should use the event handler to handle the call/virt instruction 

there are two handlers 

OnCallPrepared

OnInstructionPrepared

calls will use the fake call unless changed in eventhandler this is to prevent any malicious code to be executed 

there are many improvements to be made to this but as of now i have no real interest in changing anything as it works for everything i require for there are some missing opcodes if anyone feels free to add them just check how they are executed online and implement them is very simple

# Credits

Pan - for the events

NetGuard and ConfuserEx - for making obfuscation where static decryption is harder than just copying and pasting
