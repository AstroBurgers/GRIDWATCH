namespace GRIDWATCH.Native.Extensions;

internal static class VectorExtensions
{
    internal static float DistanceToSquared(this Vector3 a, Vector3 b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z -  b.Z;
        return dx * dx  + dy * dy + dz * dz;
    }
}