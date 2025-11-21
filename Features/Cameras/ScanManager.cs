using static GRIDWATCH.Features.Alerts.SharedMethods;

namespace GRIDWATCH.Features.Cameras;

internal class ScanManager : ISensor
{
    private static readonly Dictionary<Vehicle, uint> ScannedVehicles = new();

    public void Tick(IEnumerable<Entity> cameras)
    {
        var vehicles = World.GetAllVehicles();

        foreach (var cam in cameras)
        {
            foreach (var veh in vehicles)
            {
                if (!veh.Exists() || ScannedVehicles.ContainsKey(veh))
                    continue;

                var distance = veh.DistanceTo(cam.Position);
                if (distance >= 50f || !HasEntityClearLosToEntity(cam, veh))
                    continue;

                if (!ShouldReadPlate(veh))
                    continue;

                ProcessPlate(cam, veh);
                ScannedVehicles[veh] = Game.GameTime;
            }
        }
    }

    internal static void Cleanup()
    {
        var toRemove = ScannedVehicles.Keys
            .Where(v => !v.Exists() || !v.IsDriveable)
            .ToList();

        foreach (var v in toRemove)
            ScannedVehicles.Remove(v);
    }
}