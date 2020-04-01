using System;

namespace AoE
{
    internal sealed class SMXLayerRowEdge
    {
        internal ushort LeftSpacing  { get; set; }
        internal ushort RightSpacing { get; set; }

        internal bool IsEmpty()
        {
            return LeftSpacing == 0xffff || RightSpacing == 0xffff;
        }
    }
}