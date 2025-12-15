using CommonDataFramework.Modules.VehicleDatabase;
using GRIDWATCH.Features.Cameras;
using LSPD_First_Response.Mod.API;

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
        VehicleData vehData = vehicle.GetVehicleData();
        if (vehData == null)
            return;

        static string FormatFlag(bool condition, string text, string color)
        {
            return condition ? $"{color}[{text}]~s~ " : string.Empty;
        }

        string stolen = FormatFlag(vehData.IsStolen, "STOLEN", "~r~");
        string bolo = FormatFlag(vehData.HasAnyBOLOs, "BOLO", "~o~");
        string wanted = FormatFlag(vehData.Owner.Wanted, "WANTED", "~b~");

        // Skip if no meaningful flags
        if (string.IsNullOrWhiteSpace(stolen + bolo + wanted))
            return;

        string zone =
            Functions.GetZoneAtPosition(camera.Position)
                ?.RealAreaName ?? "Unknown";
        string make = GetMakeNameFromVehicleModel(Game.GetHashKey(vehicle.Model.Name));
        string model = vehicle.Model.Name ?? "N/A";
        string plate = vehicle.LicensePlate ?? "UNREADABLE";
        string primary = vehData.PrimaryColor ?? "UNK";
        string secondary = vehData.SecondaryColor ?? "UNK";

        string colors = primary == secondary
            ? $"~y~{primary}~s~"
            : $"~y~{primary}~s~ / ~y~{secondary}~s~";

        string message =
            $"~b~Camera Zone:~s~ {zone}\n" +
            $"~b~License Plate:~s~ ~y~{plate}~s~\n" +
            $"~b~Vehicle:~s~ ~y~{make} {model}~s~\n" +
            $"~b~Color:~s~ {colors}\n" +
            $"~b~Flags:~s~ {stolen}{bolo}{wanted}";

        LicensePlateHit plateHit = new(
            vehicle,
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