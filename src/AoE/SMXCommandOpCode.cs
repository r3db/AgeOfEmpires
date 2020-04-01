namespace AoE
{
    internal enum SMXCommandOpCode
    {
        Skip   = 0b_000000_00,
        Draw   = 0b_000000_01,
        Player = 0b_000000_10,
        End    = 0b_000000_11,
    }
}