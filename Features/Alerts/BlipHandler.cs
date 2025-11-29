using System.Drawing;
using Debug = System.Diagnostics.Debug;

namespace GRIDWATCH.Features.Alerts;

internal enum BlipType
{
    Shotspotter,
    ALPR
}

internal static class BlipHandler
{
    internal static readonly Dictionary<Blip, BlipType> ActiveBlips = [];

    internal static void CleanupBlips(BlipType? type = null)
    {
        Debug(type.HasValue
            ? $"Cleaning up active blips of type {type}..."
            : "Cleaning up all active blips...");

        // Snapshot to avoid modifying while enumerating
        var snapshot = ActiveBlips.ToArray();

        foreach (var kv in snapshot)
        {
            var blip = kv.Key;
            var blipType = kv.Value;

            if (type.HasValue && blipType != type.Value)
                continue;

            // Skip dead/null references
            if (blip == null || !blip.Exists())
            {
                Debug.Assert(blip != null, nameof(blip) + " != null");
                ActiveBlips.Remove(blip);
                continue;
            }

            try
            {
                blip.Delete();
            }
            catch (Exception ex)
            {
                Debug($"[CleanupBlips] Exception deleting {blipType} blip: {ex.Message}");
            }
            finally
            {
                ActiveBlips.Remove(blip);
            }
        }
    }
    
    internal static void CreateTimedBlip(Vector3 position, Color color, string name, int durationMs, BlipType type)
    {
        if (!UserConfig.EnableBlips || durationMs <= 0)
            return;
        GameFiber.StartNew(() =>
            {
                var blip = new Blip(position, 50f)
                {
                    Color = color,
                    Alpha = 0.5f,
                    Name = $"{name}"
                };

                blip.Flash(500, durationMs);
                ActiveBlips.Add(blip, type);

                GameFiber.Wait(durationMs);

                ActiveBlips.Remove(blip);
                blip?.Delete();
            },
            $"{name}");
    }
}