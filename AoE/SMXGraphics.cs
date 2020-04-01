using System;
using System.Drawing;

namespace AoE
{
    internal sealed class SMXGraphics
    {
        internal byte[] CommandData { get; set; }
        internal byte[] PixelData   { get; set; }
        internal Bitmap Image       { get; set; }
    }
}