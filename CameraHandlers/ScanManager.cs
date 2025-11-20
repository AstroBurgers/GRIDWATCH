using System.Diagnostics;
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
                    Debug($"Scanned vehicle {veh.LicensePlate} at distance {distance} from camera.");
                    ProcessPlate(cam, veh);
                    
                    ScannedVehicles[veh] = Game.GameTime;
                }
            }

            GameFiber.Wait(scanInterval);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    internal static void ProcessPlate(Entity camera, Vehicle vehicle)
    {
        var vehData = vehicle.GetVehicleData();
        if (vehData == null) return;
        string stolenStatus = vehData.IsStolen ? " ~r~[STOLEN]~s~" : "";
        string boloStatus = vehData.HasAnyBOLOs ? " ~o~[BOLO]~s~" : "";
        string wantedStatus = vehData.Owner.Wanted ? " ~r~[WANTED]~s~" : "";
        
        if (string.IsNullOrEmpty(stolenStatus) || string.IsNullOrEmpty(boloStatus) ||
            string.IsNullOrEmpty(wantedStatus)) return;
        
        Game.DisplayNotification("3dtextures",
            "mpgroundlogo_cops",
            "GRIDWATCH",
            "~b~License Plate Scanned",
            $"Camera Area: {LSPD_First_Response.Mod.API.Functions.GetZoneAtPosition(camera.Position).RealAreaName}\nPlate: ~y~{vehicle.LicensePlate}~s~\nModel: ~y~{vehicle.Model.Name}\nColor: ~y~{vehData.PrimaryColor} / {vehData.SecondaryColor}\nFlags: ~y~{stolenStatus} {boloStatus} {wantedStatus}"
        );
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