namespace GRIDWATCH.Native.Extensions;

internal static class VectorExtensions
{
    internal static float DistanceToSquared(this Vector3 a, Vector3 b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        float dz = a.Z - b.Z;
        return dx * dx + dy * dy + dz * dz;
    }
}