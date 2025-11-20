using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;

namespace GRIDWATCH.Features.SharedSystems;

internal static class AlertManager
{
    internal static class AlertHandler
    {
        static AlertHandler()
        {
            EventHub.Subscribe<LicensePlateHit>(OnLicensePlateHit);
            EventHub.Subscribe<GunfireIncident>(OnGunfireIncident);
        }

        private static void OnLicensePlateHit(LicensePlateHit hit)
        {
            SharedMethods.DisplayGridwatchAlert("VEHICLE SCAN TRIGGERED", hit.OriginalMessage);
            // spawn blips, logs, whatever
        }

        private static void OnGunfireIncident(GunfireIncident shot)
        {
            string streetName = World.GetStreetName(shot.Location);
            SharedMethods.DisplayGridwatchAlert("GUNFIRE DETECTED",
                $"Possible shooting detected on ~r~{streetName}~s~");
            // optionally: dispatch a camera scan focused around the location
        }
    }
}