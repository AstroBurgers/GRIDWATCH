using GRIDWATCH.Native.Extensions;
using static GRIDWATCH.Features.Alerts.SharedMethods;

namespace GRIDWATCH.Features.Cameras;

internal class ScanManager : ISensor
{
    private static readonly Dictionary<Vehicle, uint> ScannedVehicles = new();

    public void Tick(IEnumerable<Entity> cameras)
    {
        Vehicle[] vehicles = World.GetAllVehicles();

        foreach (Entity cam in cameras)
        foreach (Vehicle veh in vehicles)
        {
            if (!veh.Exists() || ScannedVehicles.ContainsKey(veh))
                continue;

            if (!veh.IsNear(cam, 50f) || !HasEntityClearLosToEntity(cam, veh) || !veh.Driver.Exists())
                continue;

            if (!ShouldReadPlate(veh))
                continue;

            ProcessPlate(cam, veh);
            ScannedVehicles[veh] = Game.GameTime;
        }
    }

    internal static void Cleanup()
    {
        List<Vehicle> toRemove = ScannedVehicles.Keys
            .Where(v => !v.Exists() || !v.IsDriveable)
            .ToList();

        foreach (Vehicle v in toRemove)
            ScannedVehicles.Remove(v);
    }
}