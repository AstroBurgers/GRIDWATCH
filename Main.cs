using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Menus;
using GRIDWATCH.Features.Shotspotter;
using JetBrains.Annotations;
using LSPD_First_Response.Mod.API;

namespace GRIDWATCH;

/// <summary>
///     Main plugin class for GRIDWATCH.
/// </summary>
[UsedImplicitly]
public class Main : Plugin
{
    internal static bool OnDuty;

    /// <summary>
    ///     LSPD First Response calls this method when the plugin is initialized.
    /// </summary>
    public override void Initialize()
    {
        Normal("Plugin initialized, go on duty to fully load plugin.");
        Functions.OnOnDutyStateChanged += Functions_OnDutyStateChanged;
    }

    private static void Functions_OnDutyStateChanged(bool onDuty)
    {
        OnDuty = onDuty;

        if (!onDuty)
            return;
        Info("Officer is now on duty, loading GRIDWATCH...");
        Info("Setting up INI File...");
        IniFileSetup();

        Info("Creating Menus...");
        MenuManager.Init();

        Info("Initializing Event Consumers...");
        EventConsumers.Initialize();

        Info("Registering Sensors...");
        SensorScheduler.Register(new ScanManager());

        Info("Starting Sensor Scheduler and Shotspotter Spawn Process...");
        SensorScheduler.Run();
        SpawnProcess.Start();

        GameFiber.StartNew(() =>
        {
            new VersionChecker(Assembly.GetExecutingAssembly()).OnCompleted += (_, e) =>
            {
                bool updateAvailable = e.UpdateAvailable;
                Version latestVersion = e.LatestVersion;

                if (updateAvailable)
                    SharedMethods.DisplayGridwatchAlert(
                        "GRIDWATCH",
                        $"Plugin is ~r~out of to date~s~!\n" +
                        $"Online Version: ~g~{latestVersion}~s~\n" +
                        $"Installed version: ~y~{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}~s~\n" +
                        $"Please update ~r~ASAP~s~!"
                    );
            };
        }, "GRIDWATCH Version Checker");

        Info("Plugin loaded successfully, scanning activated!");
        SharedMethods.DisplayGridwatchAlert(
            "GRIDWATCH",
            "Plugin loaded ~g~successfully~s~, scanning activated!"
        );

        AppDomain.CurrentDomain.DomainUnload += Cleanup;
    }

    /// <summary>
    ///     Called when the AppDomain is unloaded to perform cleanup. Fires AFTER LSPD First Response's Finally method.
    ///     Better to use this instead of Finally, since Finally does not fire if the plugin is unloaded due to an error.
    /// </summary>
    private static void Cleanup(object sender, EventArgs e)
    {
        ScanManager.Cleanup();
        BlipHandler.CleanupBlips();
        GameFiberHandling.CleanupFibers();

        Info("Unloaded successfully!");
    }

    /// <summary>
    ///     LSPD First Response calls this method when the plugin is unloaded.
    /// </summary>
    public override void Finally()
    {
        // Cleanup is handled in the DomainUnload event
    }
}