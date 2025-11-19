using GRIDWATCH.Utils;
using Rage.Attributes;

namespace GRIDWATCH;

internal static class ConsoleCommands
{
    [ConsoleCommand("**DEVELOPMENT ONLY! DO NOT USE**")]
    internal static void GRIDWATCH_DEVELOPMENTONLY()
    {
        CameraFetcher.FetchAndPrintAllCameras();
    }
}