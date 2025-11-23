using static GRIDWATCH.Native.Extensions.VectorExtensions;

namespace GRIDWATCH.Native.Extensions;

internal static class EntityExtensions
{
    internal static bool IsNear(this Entity e1, Entity e2, float maxDistance)
    {
        Vector3 e1Pos = e1.Position;
        Vector3 e2Pos = e2.Position;
        var distSqr = e1Pos.DistanceToSquared(e2Pos);
        return  distSqr <= maxDistance * maxDistance;
    }
}