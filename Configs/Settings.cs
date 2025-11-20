using GRIDWATCH.Utils;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace GRIDWATCH.Configs;

internal static class Settings
{
    internal static readonly Config UserConfig = new();
    internal static IniReflector<Config> IniReflector = new("plugins/LSPDFR/GRIDWATCH.ini");

    internal static void IniFileSetup()
    {
        if (!File.Exists("plugins/LSPDFR/GRIDWATCH.ini"))
        {
            File.Create("plugins/LSPDFR/GRIDWATCH.ini").Close();
        }
        
        IniReflector.Read(UserConfig, true);
    }

    private static void ValidateIniValues()
    {
        if (UserConfig.ReadChance <= 100) return;
        Normal("Chance value was greater than 100, setting value to 100...");
        UserConfig.ReadChance = 100;
        Game.DisplayNotification("commonmenu", "mp_alerttriangle", "GRIDWATCH", "~b~By Astro",
            "Chance value is ~r~over 100~w~!!");
        Warn("Chance value set to 100");
    }


}

internal class Config
{
    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 85, name: "Read-Chance", description: "Chance that a LPR scan is successful (Default 85)")]
    public int ReadChance;
    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 1, name: "Scan-Interval", description: "How frequent cameras are polled in MS (Default 1500)")]
    public int ScanInterval;
}