using System.Diagnostics;
using CommonDataFramework.Modules.VehicleDatabase;
using static GRIDWATCH.Utils.NativeWrapper;

namespace GRIDWATCH.CameraHandlers;

internal static class ScanManager
{
    private static Dictionary<Vehicle, uint> _scannedVehicles = new();

    internal static void ScanProcess()
    {
        var scanInterval = Configs.Settings.UserConfig.ScanInterval;

        while (true)
        {
            GameFiber.Yield();

            // Pre-fetch vehicles once per scan cycle
            var vehicles = World.GetAllVehicles();

            // Grab nearby lights
            var cameras = CameraFetcher.FetchNearbyCameras();

            foreach (var cam in cameras)
            {
                foreach (var veh in vehicles)
                {
                    if (!veh.Exists()) continue;
                    
                    // avoid reprocessing already scanned vehicles
                    if (_scannedVehicles.ContainsKey(veh)) continue;
                    
                    float distance = veh.DistanceTo(cam.Position);
                    if (distance > 30f) continue;

                    // Line of sight test
                    /*if (!HasEntityClearLosToEntity(cam, veh))
                        continue;*/

                    // Run plate scan
                    // e.g. ProcessPlate(cam, veh, distance);
                    Debug($"Scanned vehicle {veh.LicensePlate} at distance {distance} from camera.");
                    ProcessPlate(cam, veh);
                    
                    _scannedVehicles.Add(veh, Game.GameTime);
                }
            }

            GameFiber.Wait(1);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static void ProcessPlate(Entity camera, Vehicle vehicle)
    {
        var vehData = vehicle.GetVehicleData();
        if (vehData == null) return;
        var stolenStatus = vehData.IsStolen ? " ~r~[STOLEN]~s~" : "";
        var boloStatus = vehData.HasAnyBOLOs ? " ~o~[BOLO]~s~" : "";
        var wantedStatus = vehData.Owner.Wanted ? " ~r~[WANTED]~s~" : "";
        
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
        var toRemove = (from kvp in _scannedVehicles where !kvp.Key.Exists() || !kvp.Key.IsDriveable select kvp.Key).ToList();

        foreach (var vehicle in toRemove)
        {
            _scannedVehicles.Remove(vehicle);
        }
    }
    
    internal static void TerminateScanManager()
    {
        ScannedVehiclesCleanup();
    }
}