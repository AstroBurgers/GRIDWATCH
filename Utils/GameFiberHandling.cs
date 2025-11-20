namespace GRIDWATCH.Utils;

public class GameFiberHandling
{
    internal static readonly HashSet<GameFiber> ActiveGameFibers = [];

    internal static void CleanupFibers()
    {
        GameFiber.StartNew(() =>
        {
            // Debug("Cleaning up running GameFibers...");
            ActiveGameFibers.RemoveWhere(fiber =>
            {
                if (!fiber.IsAlive) return false;
                fiber.Abort();
                return true;
            });
        });
    }
}