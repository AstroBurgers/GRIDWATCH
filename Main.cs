using LSPD_First_Response.Mod.API;
using GRIDWATCH.CameraHandlers;
using GRIDWATCH.Configs;
using GRIDWATCH.Utils;

namespace GRIDWATCH;

public class Main : Plugin
{
    internal static bool OnDuty;

    public override void Initialize()
    {
        Normal("Plugin initialized, go on duty to fully load plugin.");
        Functions.OnOnDutyStateChanged += Functions_OnDutyStateChanged;
    }

    private static void Functions_OnDutyStateChanged(bool onDuty)
    {
        OnDuty = onDuty;
        if (onDuty)
        {
            GameFiber.StartNew(() =>
            {
                Normal("Adding console commands...");
                Game.AddConsoleCommands();
                Settings.IniFileSetup();
                GameFiberHandling.ActiveGameFibers.Add(GameFiber.StartNew(ScanManager.ScanProcess));
                SharedMethods.DisplayGridwatchAlert("GRIDWATCH", "Plugin loaded ~g~successfully~s~, scanning activated!");
                AppDomain.CurrentDomain.DomainUnload += Cleanup;
            });
        }
    }

    private static void Cleanup(object sender, EventArgs e)
    {
        ScanManager.TerminateScanManager();
        BlipHandler.CleanupBlips();
        GameFiberHandling.CleanupFibers();

        Normal("Unloaded successfully");
    }
    
    public override void Finally()
    {
        ScanManager.TerminateScanManager();
        BlipHandler.CleanupBlips();
        GameFiberHandling.CleanupFibers();

        Normal("Unloaded successfully");
    }
}