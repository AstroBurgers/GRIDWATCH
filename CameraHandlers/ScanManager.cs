using System.Diagnostics;
using CommonDataFramework.Modules.VehicleDatabase;
using GRIDWATCH.Utils;
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
                    if (distance >= 50f) continue;

                    // Line of sight test
                    if (!HasEntityClearLosToEntity(cam, veh))
                        continue;

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

        static string FormatFlag(bool condition, string text, string color)
            => condition ? $"{color}[{text}]~s~ " : string.Empty;

        string stolen = FormatFlag(vehData.IsStolen, "STOLEN", "~r~");
        string bolo = FormatFlag(vehData.HasAnyBOLOs, "BOLO", "~o~");
        string wanted = FormatFlag(vehData.Owner.Wanted, "WANTED", "~b~");

        // Skip if there are literally no flags of interest
        if (string.IsNullOrWhiteSpace(stolen + bolo + wanted)) return;

        // Gather zone / vehicle info safely
        var zone =
            LSPD_First_Response.Mod.API.Functions.GetZoneAtPosition(camera.Position)
                ?.RealAreaName ?? "Unknown";
        var make = GetMakeNameFromVehicleModel(Game.GetHashKey(vehicle.Model.Name));
        var model = vehicle.Model.Name ?? "N/A";
        var plate = vehicle.LicensePlate ?? "UNREADABLE";
        var primary = vehData.PrimaryColor ?? "UNK";
        var secondary = vehData.SecondaryColor ?? "UNK";

        string colors = primary == secondary
            ? $"~y~{primary}~s~"
            : $"~y~{primary}~s~ / ~y~{secondary}~s~";

        string message =
            $"~b~Camera Zone:~s~ {zone}\n" +
            $"~b~License Plate:~s~ ~y~{plate}~s~\n" +
            $"~b~Vehicle:~s~ ~y~{make} {model}~s~\n" +
            $"~b~Color:~s~ {colors}\n" +
            $"~b~Flags:~s~ {stolen}{bolo}{wanted}";

        SharedMethods.DisplayGridwatchAlert(
            type: "VEHICLE SCAN TRIGGERED",
            message: message
        );

        GameFiberHandling.ActiveGameFibers.Add(
            GameFiber.StartNew(() =>
                {
                    var blip = new Blip(vehicle.Position, 50f)
                    {
                        Color = System.Drawing.Color.Red,
                        Alpha = 0.5f,
                        Name = $"GRIDWATCH Alert: {plate}"
                    };

                    blip.Flash(500, 30000);
                    BlipHandler.ActiveBlips.Add(blip);

                    GameFiber.Wait(30000);

                    BlipHandler.ActiveBlips.Remove(blip);
                    blip?.Delete();
                },
                $"GRIDWATCH Blip Thread {plate}")
        );
    }

    private static void ScannedVehiclesCleanup()
    {
        var toRemove = (from kvp in _scannedVehicles where !kvp.Key.Exists() || !kvp.Key.IsDriveable select kvp.Key)
            .ToList();

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