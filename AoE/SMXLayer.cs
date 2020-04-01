using System;
using System.Collections.Generic;

namespace AoE
{
    internal sealed class SMXLayer
    {
        internal SMXLayerHeader         Header   { get; set; }
        internal IList<SMXLayerRowEdge> Edges    { get; set; }
        internal SMXGraphics            Graphics { get; set; }
    }
}