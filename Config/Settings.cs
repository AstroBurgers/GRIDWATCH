#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace GRIDWATCH.Config;

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
        ValidateIniValues();
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
    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 85)]
    public int ReadChance;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 1500)]
    public int ScanInterval;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 60000)]
    public int ShotspotterPollRate;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 5)]
    public int ShotspotterChance;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 20)]
    public int ShotspotterFalseAlarmChance;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: 5)]
    public int MaxCamerasPerScan;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: Keys.O)]
    public Keys MenuKey;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: Keys.LControlKey)]
    public Keys MenuModifierKey;

    [IniReflectorValue(sectionName: "General_Settings", defaultValue: false)]
    public bool DebugModeEnabled;
}