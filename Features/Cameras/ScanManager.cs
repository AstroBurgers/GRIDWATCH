using CommonDataFramework.Modules.VehicleDatabase;

namespace GRIDWATCH.Features.Cameras;

internal static class ScanManager
{
    private static Dictionary<Vehicle, uint> _scannedVehicles = new();

    internal static void ScanProcess()
    {
        while (true)
        {
            GameFiber.Yield();
            var vehicles = World.GetAllVehicles();
            var cameras = CameraFetcher.FetchNearbyCameras();

            foreach (var cam in cameras)
            {
                foreach (var veh in vehicles)
                {
                    if (!veh.Exists()) continue;

                    if (_scannedVehicles.ContainsKey(veh)) continue;

                    var distance = veh.DistanceTo(cam.Position);
                    if (distance >= 50f) continue;
                    
                    if (!HasEntityClearLosToEntity(cam, veh))
                        continue;
                    
                    if (!ShouldReadPlate(veh)) {
                        return;
                    }
                    Debug($"Scanned vehicle {veh.LicensePlate} at distance {distance} from camera.");
                    ProcessPlate(cam, veh);

                    _scannedVehicles.Add(veh, Game.GameTime);
                }
            }

            GameFiber.Wait(UserConfig.ScanInterval);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static void ProcessPlate(Entity camera, Vehicle vehicle)
    {
        var vehData = vehicle.GetVehicleData();
        if (vehData == null) return;

        static string FormatFlag(bool condition, string text, string color)
            => condition ? $"{color}[{text}]~s~ " : string.Empty;

        var stolen = FormatFlag(vehData.IsStolen, "STOLEN", "~r~");
        var bolo = FormatFlag(vehData.HasAnyBOLOs, "BOLO", "~o~");
        var wanted = FormatFlag(vehData.Owner.Wanted, "WANTED", "~b~");

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

        var colors = primary == secondary
            ? $"~y~{primary}~s~"
            : $"~y~{primary}~s~ / ~y~{secondary}~s~";

        var message =
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


    private static bool ShouldReadPlate(Vehicle vehicle)
    {
        // base chance (user config 0–100)
        float effectiveChance = UserConfig.ReadChance;

        // Apply modifiers
        effectiveChance *= IsWeatherInclement() ? 0.8f : 1.0f;
        effectiveChance *= GetLightLevelModifier();
        effectiveChance *= GetDirtModifier(vehicle);

        // Clamp to 0–100
        effectiveChance = MathHelper.Clamp(effectiveChance, 0f, 100f);

        // Random roll 0–100
        return Rndm.NextDouble() * 100 <= effectiveChance;
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