namespace AoE
{
    internal sealed class SMXHeader
    {
        internal string Descriptor  { get; set; }
        internal short  Version     { get; set; }
        internal short  FrameCount  { get; set; }
        internal int    FileSizeSMX { get; set; }
        internal int    FileSizeSMP { get; set; }
        internal string Comment     { get; set; }
    }
}