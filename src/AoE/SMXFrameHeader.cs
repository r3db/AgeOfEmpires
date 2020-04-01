using System;

namespace AoE
{
    internal sealed class SMXFrameHeader
    {
        internal SMXFrameType     Kind             { get; set; }
        internal SMXFrameCompression Compression   { get; set; }
        internal byte             PaletteNumber    { get; set; }
        internal int              UncompressedSize { get; set; }

        internal bool HasGraphics()
        {
            return (SMXFrameType.Graphics & Kind) == SMXFrameType.Graphics;
        }

        internal bool HasShadow()
        {
            return (SMXFrameType.Shadow & Kind) == SMXFrameType.Shadow;
        }

        internal bool HasOutline()
        {
            return (SMXFrameType.Outline & Kind) == SMXFrameType.Outline;
        }
    }
}