using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace GRIDWATCH.Features.Menus;

internal sealed class LicensePlateHitsMenu() : DynamicDataMenu<LicensePlateHit>("License Plate Hits",
    EventConsumers.GetAllAlprHits,
    "No license plate hits logged yet.",
    EventConsumers.ClearAlprHits, Alerts.BlipType.Shotspotter)
{
    protected override UIMenuItem BuildItem(LicensePlateHit hit)
    {
        return new UIMenuItem(
            $"{hit.LicensePlate} ({hit.Model})",
            hit.Message);
    }
}

internal sealed class ShotspotterMenu() : DynamicDataMenu<GunfireIncident>("Shotspotter Incidents",
    EventConsumers.GetAllGunfireIncidents,
    "No gunfire reports available.",
    EventConsumers.ClearGunfireIncidents, Alerts.BlipType.Shotspotter)
{
    protected override UIMenuItem BuildItem(GunfireIncident incident)
    {
        var desc = $"Time: {incident.Timestamp:MMM dd HH:mm:ss}\n" +
                   $"Location: {World.GetStreetName(incident.Location)}";
        return new UIMenuItem($"Incident at {incident.Timestamp:HH:mm:ss}", desc);
    }
}