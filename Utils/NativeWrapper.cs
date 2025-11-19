using Rage.Native;

namespace GRIDWATCH.Utils;

internal static class NativeWrapper
{
    internal static bool HasEntityClearLosToEntity(Entity source, Entity target, int options = 17)
    {
        return NativeFunction.Natives.HAS_ENTITY_CLEAR_LOS_TO_ENTITY<bool>(source, target, options);
    }
}