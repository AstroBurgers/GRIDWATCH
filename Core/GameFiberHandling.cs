namespace GRIDWATCH.Core;

internal static class GameFiberHandling
{
    internal static readonly HashSet<GameFiber> ActiveGameFibers = [];

    internal static void CleanupFibers()
    {
        GameFiber.StartNew(() =>
        {
            Info("Cleaning up running GameFibers...");
            ActiveGameFibers.RemoveWhere(fiber =>
            {
                if (!fiber.IsAlive) return false;
                fiber.Abort();
                return true;
            });
        });
    }
}