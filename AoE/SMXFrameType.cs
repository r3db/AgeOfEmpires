using System;

namespace AoE
{
    [Flags]
    internal enum SMXFrameType
    {
        Graphics    = 0b_0000_0001,
        Shadow      = 0b_0000_0010,
        Outline     = 0b_0000_0100,
        Bridge      = 0b_0001_0000,
    }
}