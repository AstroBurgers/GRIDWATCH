using CommonDataFramework.Modules.VehicleDatabase;
using GRIDWATCH.Features.SharedSystems;
using static GRIDWATCH.Features.Alerts.SharedMethods;

namespace GRIDWATCH.Features.Cameras;

internal class ScanManager : ISensor
{
    private readonly Dictionary<Vehicle, uint> _scannedVehicles = new();

    public void Tick(IEnumerable<Entity> cameras)
    {
        var vehicles = World.GetAllVehicles();

        foreach (var cam in cameras)
        {
            foreach (var veh in vehicles)
            {
                if (!veh.Exists() || _scannedVehicles.ContainsKey(veh))
                    continue;

                var distance = veh.DistanceTo(cam.Position);
                if (distance >= 50f || !HasEntityClearLosToEntity(cam, veh))
                    continue;

                if (!ShouldReadPlate(veh))
                    continue;

                ProcessPlate(cam, veh);
                _scannedVehicles[veh] = Game.GameTime;
            }
        }

        Cleanup();
    }

    private void Cleanup()
    {
        var toRemove = _scannedVehicles.Keys
            .Where(v => !v.Exists() || !v.IsDriveable)
            .ToList();

        foreach (var v in toRemove)
            _scannedVehicles.Remove(v);
    }
}