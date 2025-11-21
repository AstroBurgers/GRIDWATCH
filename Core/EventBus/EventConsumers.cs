using System.Drawing;
using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;
using static GRIDWATCH.Features.Alerts.BlipHandler;

namespace GRIDWATCH.Core.EventBus;

internal static class EventConsumers
{
    private static readonly List<LicensePlateHit> AlprHits = [];
    private static readonly List<GunfireIncident> GunfireIncidents = [];

    internal static void Initialize()
    {
        EventHub.Subscribe<LicensePlateHit>(OnPlateHit);
        EventHub.Subscribe<GunfireIncident>(OnGunfireIncident);
    }

    private static void OnGunfireIncident(GunfireIncident shot)
    {
        SharedMethods.DisplayGridwatchAlert("SHOTSPOTTER ALERT",
            $"Possible shooting detected on ~r~{World.GetStreetName(shot.Location)}~s~ in ~b~{LSPD_First_Response.Mod.API.Functions.GetZoneAtPosition(shot.Location)
                ?.RealAreaName ?? "Unknown"}~s~");
        
        CreateTimedBlip(shot.Location, Color.OrangeRed, $"GRIDWATCH Alert: Shotspotter {shot.Timestamp}",
            30000);
        
        GunfireIncidents.Add(shot);
    }

    private static void OnPlateHit(LicensePlateHit hit)
    {
        SharedMethods.DisplayGridwatchAlert(
            type: "ALPR ALERT",
            message: hit.Message
        );
        
        CreateTimedBlip(hit.Location, Color.Red, $"GRIDWATCH Alert: {hit.LicensePlate}",
            30000);
        
        AlprHits.Add(hit);
    }


    internal static List<LicensePlateHit> GetAllAlprHits()
    {
        return AlprHits;
    }

    internal static List<GunfireIncident> GetAllGunfireIncidents()
    {
        return GunfireIncidents;
    }

    internal static void ClearAlprHits()
    {
        AlprHits.Clear();
    }
    
    internal static void ClearGunfireIncidents()
    {
        GunfireIncidents.Clear();
    }
}