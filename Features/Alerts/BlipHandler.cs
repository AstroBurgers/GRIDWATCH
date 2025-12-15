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
        KeyValuePair<Blip, BlipType>[] snapshot = ActiveBlips.ToArray();

        foreach (KeyValuePair<Blip, BlipType> kv in snapshot)
        {
            Blip blip = kv.Key;
            BlipType blipType = kv.Value;

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

    internal static void CreateTimedBlip(
        Vector3 position,
        Color color,
        string name,
        int durationMs,
        BlipType type,
        Vehicle attachedTo = null)
    {
        if (!UserConfig.EnableBlips || durationMs <= 0)
            return;

        GameFiber.StartNew(() =>
            {
                Blip blip = CreateBlipInstance(position, attachedTo, color, name, type);

                if (blip == null)
                    return;

                blip.Flash(500, durationMs);
                ActiveBlips.Add(blip, type);

                GameFiber.Wait(durationMs);

                ActiveBlips.Remove(blip);
                blip.DeleteSafe();
            }, $"{name}");
    }

    private static Blip CreateBlipInstance(
        Vector3 position,
        Vehicle attachedTo,
        Color color,
        string name,
        BlipType type)
    {
        bool isTracking = UserConfig.TrackingBlips && type == BlipType.ALPR && attachedTo != null;

        Blip blip = isTracking ? new Blip(attachedTo) : new Blip(position, 50f);

        blip.Color = color;
        blip.Alpha = 0.5f;
        blip.Name = isTracking ? $"{name} (Tracking)" : name;

        if (isTracking)
            blip.Sprite = BlipSprite.GangVehicle;

        return blip;
    }

    private static void DeleteSafe(this Blip blip)
    {
        try
        {
            blip?.Delete();
        }
        catch
        {
            /* no crash for deleted blips */
        }
    }
}