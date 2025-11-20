namespace GRIDWATCH.Utils;

internal static class SharedMethods
{
    internal static void DisplayGridwatchAlert(string type, string message)
    {
        Game.DisplayNotification("3dtextures",
            "mpgroundlogo_cops",
            "GRIDWATCH Alert",
            $"~b~{type}",
            $"{message}"
        );
    }
}