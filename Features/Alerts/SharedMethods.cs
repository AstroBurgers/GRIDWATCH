using CommonDataFramework.Modules.VehicleDatabase;
using GRIDWATCH.Features.Cameras;

namespace GRIDWATCH.Features.Alerts;

internal static class SharedMethods
{
    internal static void DisplayGridwatchAlert(string type, string message)
    {
        Game.DisplayNotification("3dtextures",
            "mpgroundlogo_cops",
            "GRIDWATCH Alert",
            $"~b~{type}",
            $"{message}"
        );
    }

    internal static void ProcessPlate(Entity camera, Vehicle vehicle)
    {
        var vehData = vehicle.GetVehicleData();
        if (vehData == null)
            return;

        static string FormatFlag(bool condition, string text, string color)
            => condition ? $"{color}[{text}]~s~ " : string.Empty;

        var stolen = FormatFlag(vehData.IsStolen, "STOLEN", "~r~");
        var bolo = FormatFlag(vehData.HasAnyBOLOs, "BOLO", "~o~");
        var wanted = FormatFlag(vehData.Owner.Wanted, "WANTED", "~b~");

        // Skip if no meaningful flags
        if (string.IsNullOrWhiteSpace(stolen + bolo + wanted))
            return;

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

        var plateHit = new LicensePlateHit(
            camera.Position,
            plate,
            make,
            model,
            primary,
            secondary,
            message
        );

        EventHub.Publish(plateHit);
    }

    internal static bool ShouldReadPlate(Vehicle vehicle)
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
}