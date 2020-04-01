using System;

namespace AoE
{
    internal sealed class SMXLayerHeader
    {
        internal ushort Width       { get; set; }
        internal ushort Height      { get; set; }
        internal short  AnchorX     { get; set; }
        internal short  AnchorY     { get; set; }
        internal uint   LayerLength { get; set; }
    }
}