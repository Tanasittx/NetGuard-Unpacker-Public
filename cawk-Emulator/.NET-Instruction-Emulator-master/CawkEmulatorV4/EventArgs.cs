using System;
using dnlib.DotNet.Emit;

namespace CawkEmulatorV4
{
    public class InstructionEventArgs : EventArgs
    {
        public InstructionEventArgs(Instruction inst, int pushes, int pops)
        {
            Instruction = inst;
            Pushes = pushes;
            Pops = pops;
        }

        public Instruction Instruction { get; set; }
        public int Pushes { get; }
        public int Pops { get; }

        /// <summary>
        ///     <para>Cancel the instruction from being emulated.</para>
        ///     <para>Will not prevent emulation of other instructions.</para>
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        ///     <para>Will prevent further emulation of the method.</para>
        /// </summary>
        public bool Break { get; set; }
    }

    public class CallEventArgs : EventArgs
    {
        public CallEventArgs(Instruction inst, int pushes, int pops)
        {
            Instruction = inst;
            Pushes = pushes;
            Pops = pops;
        }

        public Instruction Instruction { get; set; }
        public int Pushes { get; }
        public int Pops { get; }

        /// <summary>
        ///     <para>Allow a call to be emulated (invokes the original call). This can be very risky when the target is malicious.</para>
        ///     <para>
        ///         If this property is not set to true (defaults to false) the <see cref="Emulation" /> will pop arguments and
        ///         (dependent on return type) return null.
        ///     </para>
        /// </summary>
        public bool AllowCall { get; set; }

        /// <summary>
        ///     <para>All the call to be completely bypassed</para>
        /// </summary>
        public bool bypassCall { get; set; }

        /// <summary>
        ///     <para>Enable you to end the emulation early</para>
        /// </summary>
        public bool endMethod { get; set; }
    }
}