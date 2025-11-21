using System.Drawing;

namespace GRIDWATCH.Features.Alerts;

internal static class BlipHandler
{
    internal static readonly HashSet<Blip> ActiveBlips = [];

    internal static void CleanupBlips()
    {
        Debug("Cleaning up active blips...");

        foreach (var blip in ActiveBlips.Where(b => b.Exists()).ToList())
        {
            try
            {
                blip.Delete();
            }
            catch (Exception ex)
            {
                Debug($"Exception deleting blip: {ex.Message}");
            }
        }

        ActiveBlips.Clear();
    }

    internal static void CreateTimedBlip(Vector3 position, Color color, string name, int durationMs)
    {
        GameFiber.StartNew(() =>
            {
                var blip = new Blip(position, 50f)
                {
                    Color = color,
                    Alpha = 0.5f,
                    Name = $"{name}"
                };

                blip.Flash(500, durationMs);
                ActiveBlips.Add(blip);

                GameFiber.Wait(durationMs);

                ActiveBlips.Remove(blip);
                blip?.Delete();
            },
            $"{name}");
    }
}