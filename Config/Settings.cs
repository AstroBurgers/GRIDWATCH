#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace GRIDWATCH.Config;

internal static class Settings
{
    private struct NumericLimit(int min, int max)
    {
        public readonly int Min = min;
        public readonly int Max = max;
    }
    private static readonly Dictionary<string, NumericLimit> NumericLimits =
        new()
        {
            { nameof(Config.ReadChance), new NumericLimit(0, 100) },
            { nameof(Config.ScanInterval), new NumericLimit(100, 120000) },
            { nameof(Config.ShotspotterPollRate), new NumericLimit(1000, 120000) },
            { nameof(Config.ShotspotterChance), new NumericLimit(0, 100) },
            { nameof(Config.ShotspotterFalseAlarmChance), new NumericLimit(0, 100) },
            { nameof(Config.MaxCamerasPerScan), new NumericLimit(1, 20) },
            { nameof(Config.BlipDuration), new NumericLimit(100, 120000) }
        };
    
    internal static readonly Config UserConfig = new();
    internal static IniReflector<Config> IniReflector = new("plugins/LSPDFR/GRIDWATCH.ini");

    internal static void IniFileSetup()
    {
        if (!File.Exists("plugins/LSPDFR/GRIDWATCH.ini")) File.Create("plugins/LSPDFR/GRIDWATCH.ini").Close();

        IniReflector.Read(UserConfig, true);
        ValidateIniValues();
    }

    private static void ValidateIniValues()
    {
        List<string> fixes = [];
        Config config = UserConfig;

        foreach (KeyValuePair<string, NumericLimit> kvp in NumericLimits)
        {
            string propertyName = kvp.Key;
            NumericLimit limit = kvp.Value;

            PropertyInfo prop = typeof(Config).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(int))
                continue;

            int currentValue = (int)prop.GetValue(config);
            int clampedValue = MathHelper.Clamp(
                currentValue,
                limit.Min,
                limit.Max
            );

            if (currentValue == clampedValue)
                continue;

            prop.SetValue(config, clampedValue);

            fixes.Add($"{propertyName}: {currentValue} → {clampedValue}");

            Warn(
                $"{propertyName} was out of range ({currentValue}), clamped to {clampedValue}"
            );
        }

        if (fixes.Count == 0)
            return;

        string message = string.Join("~n~", fixes);

        SharedMethods.DisplayGridwatchAlert("Invalid Config Fixed", message);
    }
}

internal class Config
{
    [IniReflectorValue("General_Settings", defaultValue: false)]
    public bool DebugModeEnabled;

    [IniReflectorValue("General_Settings", defaultValue: true)]
    public bool EnableBlips;

    [IniReflectorValue("General_Settings", defaultValue: 30000)]
    public int BlipDuration;
    
    [IniReflectorValue("General_Settings", defaultValue: true, description:"If true, blips will be smaller, and attached to the scanned vehicle, allowing for continuous tracking.")]
    public bool TrackingBlips;
    
    [IniReflectorValue("General_Settings", defaultValue: 5)]
    public int MaxCamerasPerScan;
    
    [IniReflectorValue("General_Settings", defaultValue: Keys.O)]
    public Keys MenuKey;

    [IniReflectorValue("General_Settings", defaultValue: Keys.LControlKey)]
    public Keys MenuModifierKey;

    [IniReflectorValue("General_Settings", defaultValue: 85)]
    public int ReadChance;

    [IniReflectorValue("General_Settings", defaultValue: 5000)]
    public int ScanInterval;

    [IniReflectorValue("General_Settings", defaultValue: 5)]
    public int ShotspotterChance;

    [IniReflectorValue("General_Settings", defaultValue: 20)]
    public int ShotspotterFalseAlarmChance;

    [IniReflectorValue("General_Settings", defaultValue: 60000)]
    public int ShotspotterPollRate;
}