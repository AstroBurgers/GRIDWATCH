using System.Drawing;
using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;
using static GRIDWATCH.Features.Alerts.BlipHandler;

namespace GRIDWATCH.Core.EventBus;

internal static class EventConsumers
{
    private static List<LicensePlateHit> _alprHits = [];
    private static List<GunfireIncident> _gunfireIncidents = [];

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
        
        _gunfireIncidents.Add(shot);
    }

    private static void OnPlateHit(LicensePlateHit hit)
    {
        SharedMethods.DisplayGridwatchAlert(
            type: "ALPR ALERT",
            message: hit.Message
        );
        
        CreateTimedBlip(hit.Location, Color.Red, $"GRIDWATCH Alert: {hit.LicensePlate}",
            30000);
        
        _alprHits.Add(hit);
    }


    internal static List<LicensePlateHit> GetAllAlprHits()
    {
        return _alprHits;
    }

    internal static List<GunfireIncident> GetAllGunfireIncidents()
    {
        return _gunfireIncidents;
    }

    internal static void ClearAlprHits()
    {
        _alprHits.Clear();
    }
    
    internal static void ClearGunfireIncidents()
    {
        _gunfireIncidents.Clear();
    }
}