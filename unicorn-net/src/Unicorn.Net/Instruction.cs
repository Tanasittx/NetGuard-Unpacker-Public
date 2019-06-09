namespace Unicorn
{
    /// <summary>
    /// Represents an instruction.
    /// </summary>
    public struct Instruction
    {
        internal readonly int _id;
        internal Instruction(int id)
        {
            _id = id;
        }
    }
}
