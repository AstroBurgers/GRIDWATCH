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
}