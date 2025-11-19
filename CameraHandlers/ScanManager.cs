using CommonDataFramework.Modules.VehicleDatabase;
using static GRIDWATCH.Utils.NativeWrapper;

namespace GRIDWATCH.CameraHandlers;

internal static class ScanManager
{
    internal static Dictionary<Vehicle, uint> ScannedVehicles = new();

    internal static void ScanProcess()
    {
        var scanInterval = Configs.Settings.UserConfig.ScanInterval;

        while (true)
        {
            GameFiber.Yield();

            // Pre-fetch vehicles once per scan cycle
            var vehicles = World.GetAllVehicles()
                .Where(v => v.Exists() && v.IsDriveable)
                .ToList();

            // Grab nearby lights
            var cameras = CameraFetcher.FetchNearbyCameras();

            foreach (var cam in cameras)
            {
                foreach (var veh in vehicles)
                {
                    float distance = veh.DistanceTo(cam.Position);
                    if (distance > 30f) continue;

                    // Line of sight test
                    if (!HasEntityClearLosToEntity(cam, veh))
                        continue;

                    // avoid reprocessing already scanned vehicles
                    if (ScannedVehicles.ContainsKey(veh)) continue;

                    // Run plate scan
                    // e.g. ProcessPlate(cam, veh, distance);
                    Game.DisplayNotification($"Camera at {cam.Position} scanned vehicle {veh.Model.Name} at distance {distance:F1}m");

                    var data = veh.GetVehicleData();
                    
                    ScannedVehicles[veh] = Game.GameTime;
                }
            }

            GameFiber.Wait(scanInterval);
        }
    }
    
    internal static void ScannedVehiclesCleanup()
    {
        var toRemove = new List<Vehicle>();
        foreach (var kvp in ScannedVehicles)
        {
            if (!kvp.Key.Exists() || !kvp.Key.IsDriveable)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var vehicle in toRemove)
        {
            ScannedVehicles.Remove(vehicle);
        }
    }
    
    internal static void TerminateScanManager()
    {
        ScannedVehiclesCleanup();
    }
}