using System.Drawing;
using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.SharedSystems;
using GRIDWATCH.Features.Shotspotter;

namespace GRIDWATCH.Core.EventBus;

internal static class EventConsumers
{
    static EventConsumers()
    {
        EventHub.Subscribe<LicensePlateHit>(OnPlateHit);
        EventHub.Subscribe<GunfireIncident>(OnGunfireIncident);
    }

    internal static void Initialize()
    {
        EventHub.Subscribe<LicensePlateHit>(OnPlateHit);
        EventHub.Subscribe<GunfireIncident>(OnGunfireIncident);
    }

    private static void OnGunfireIncident(GunfireIncident shot)
    {
        SharedMethods.DisplayGridwatchAlert("GUNFIRE DETECTED",
            $"Possible shooting detected on ~r~{World.GetStreetName(shot.Location)}~s~ in {LSPD_First_Response.Mod.API.Functions.GetZoneAtPosition(shot.Location)
                ?.RealAreaName ?? "Unknown"}");
        
        BlipHandler.CreateTimedBlip(shot.Location, Color.OrangeRed, $"GRIDWATCH Alert: Shotspotter {shot.Timestamp}",
            30000);
    }

    private static void OnPlateHit(LicensePlateHit hit)
    {
        SharedMethods.DisplayGridwatchAlert(
            type: "VEHICLE SCAN TRIGGERED",
            message: hit.Message
        );
        
        BlipHandler.CreateTimedBlip(hit.Location, Color.Red, $"GRIDWATCH Alert: {hit.LicensePlate}",
            30000);
    }
}