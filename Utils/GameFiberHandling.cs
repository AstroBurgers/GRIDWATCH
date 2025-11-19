namespace GRIDWATCH.Utils;

public class GameFiberHandling
{
    internal static readonly HashSet<GameFiber> OutcomeGameFibers = [];

    internal static void CleanupFibers()
    {
        GameFiber.StartNew(() =>
        {
            // Debug("Cleaning up running GameFibers...");
            OutcomeGameFibers.RemoveWhere(fiber =>
            {
                if (!fiber.IsAlive) return false;
                fiber.Abort();
                return true;
            });
        });
    }
        
}