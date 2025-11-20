using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;
using LSPD_First_Response.Mod.API;

namespace GRIDWATCH;

/// <summary>
/// Main plugin class for GRIDWATCH.
/// </summary>
public class Main : Plugin
{
    internal static bool OnDuty;
    
    /// <summary>
    /// LSPD First Response calls this method when the plugin is initialized.
    /// </summary>
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
                IniFileSetup();
                GameFiberHandling.ActiveGameFibers.Add(GameFiber.StartNew(ScanManager.ScanProcess));
                GameFiberHandling.ActiveGameFibers.Add(GameFiber.StartNew(SpawnProcess.Start));
                SharedMethods.DisplayGridwatchAlert("GRIDWATCH", "Plugin loaded ~g~successfully~s~, scanning activated!");
                AppDomain.CurrentDomain.DomainUnload += Cleanup;
            });
        }
    }

    /// <summary>
    /// Called when the AppDomain is unloaded to perform cleanup. Fires AFTER LSPD First Response's Finally method.
    /// Better to use this instead of Finally, since Finally does not fire if the plugin is unloaded due to an error.
    /// </summary>
    private static void Cleanup(object sender, EventArgs e)
    {
        ScanManager.TerminateScanManager();
        BlipHandler.CleanupBlips();
        GameFiberHandling.CleanupFibers();

        Normal("Unloaded successfully");
    }
    
    /// <summary>
    /// LSPD First Response calls this method when the plugin is unloaded.
    /// </summary>
    public override void Finally()
    {
        // Cleanup is handled in the DomainUnload event
    }
}