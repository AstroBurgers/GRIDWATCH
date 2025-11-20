using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.SharedSystems;
using GRIDWATCH.Features.Shotspotter;

namespace GRIDWATCH.Core.EventBus;

internal static class EventConsumers
{
    static EventConsumers()
    {
        EventHub.Subscribe<LicensePlateHit>(OnPlateHit);
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
        GameFiber.StartNew(() =>
            {
                var blip = new Blip(shot.Location, 50f)
                {
                    Color = System.Drawing.Color.OrangeRed,
                    Alpha = 0.5f,
                    Name = $"GRIDWATCH Alert: Shotspotter {shot.Timestamp}"
                };

                blip.Flash(500, 30000);
                BlipHandler.ActiveBlips.Add(blip);

                GameFiber.Wait(30000);

                BlipHandler.ActiveBlips.Remove(blip);
                blip?.Delete();
            },
            $"GRIDWATCH Blip Thread Shotspotter {shot.Timestamp}");
    }

    private static void OnPlateHit(LicensePlateHit hit)
    {
        SharedMethods.DisplayGridwatchAlert(
            type: "VEHICLE SCAN TRIGGERED",
            message: hit.OriginalMessage
        );

        GameFiberHandling.ActiveGameFibers.Add(
            GameFiber.StartNew(() =>
                {
                    var blip = new Blip(hit.Location, 50f)
                    {
                        Color = System.Drawing.Color.Red,
                        Alpha = 0.5f,
                        Name = $"GRIDWATCH Alert: {hit.LicensePlate}"
                    };

                    blip.Flash(500, 30000);
                    BlipHandler.ActiveBlips.Add(blip);

                    GameFiber.Wait(30000);

                    BlipHandler.ActiveBlips.Remove(blip);
                    blip?.Delete();
                },
                $"GRIDWATCH Blip Thread {hit.LicensePlate}")
        );
    }
}