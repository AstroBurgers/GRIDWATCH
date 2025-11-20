namespace GRIDWATCH.Native;

internal static class NativeWrapper
{
    internal static bool HasEntityClearLosToEntity(Entity source, Entity target, int options = 17)
    {
        return NativeFunction.Natives.HAS_ENTITY_CLEAR_LOS_TO_ENTITY<bool>(source, target, options);
    }

    internal static string GetMakeNameFromVehicleModel(uint modelHash)
    {
        return NativeFunction.Natives.GET_MAKE_NAME_FROM_VEHICLE_MODEL<string>(modelHash);
    }
}