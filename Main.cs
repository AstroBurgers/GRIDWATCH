using CommonDataFramework.API;
using LSPD_First_Response.Mod.API;

namespace GRIDWATCH;

public class Main : Plugin
{
    internal static bool OnDuty;

    public override void Initialize()
    {
        // Normal("Plugin initialized, go on duty to fully load plugin.");
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
                Game.DisplayNotification("GRIDWATCH Loaded Successfully!");
            });
        }
    }

    public override void Finally()
    {
            
    }
}