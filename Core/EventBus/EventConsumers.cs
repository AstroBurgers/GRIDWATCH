using System.Drawing;
using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;
using LSPD_First_Response.Mod.API;
using static GRIDWATCH.Features.Alerts.BlipHandler;
using Events = GRIDWATCH.API.Events;

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
            $"Possible shooting detected on ~r~{World.GetStreetName(shot.Location)}~s~ in ~b~{Functions.GetZoneAtPosition(shot.Location)
                ?.RealAreaName ?? "Unknown"}~s~");

        CreateTimedBlip(shot.Location, Color.OrangeRed, $"GRIDWATCH Alert: Shotspotter {shot.Timestamp}",
            UserConfig.BlipDuration, BlipType.Shotspotter);

        GunfireIncidents.Add(shot);
        Events.GunfireIncidentAdded(shot);
    }

    private static void OnPlateHit(LicensePlateHit hit)
    {
        SharedMethods.DisplayGridwatchAlert(
            "ALPR ALERT",
            hit.Message
        );

        CreateTimedBlip(hit.Location, Color.Red, $"GRIDWATCH Alert: {hit.LicensePlate}",
            UserConfig.BlipDuration, BlipType.ALPR, hit.Vehicle);

        AlprHits.Add(hit);
        Events.LicensePlateHitAdded(hit);
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