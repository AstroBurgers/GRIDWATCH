namespace GRIDWATCH.Utils;

internal static class BlipHandler
{
    internal static readonly HashSet<Blip> ActiveBlips = [];

    internal static void CleanupBlips()
    {
        GameFiber.StartNew(() =>
        {
            Debug("Cleaning up running GameFibers...");
            ActiveBlips.RemoveWhere(blip =>
            {
                if (!blip.Exists()) return false;
                blip.Delete();
                return true;
            });
        });
    }
}