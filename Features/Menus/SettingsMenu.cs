using System.Drawing;
using GRIDWATCH.Config;
using RAGENativeUI;
using RAGENativeUI.Elements;

#pragma warning disable CS0618 // Type or member is obsolete

namespace GRIDWATCH.Features.Menus;

internal sealed class SettingsMenu
{
    internal readonly UIMenu Menu;

    internal SettingsMenu()
    {
        Menu = new UIMenu("Settings", "GRIDWATCH Configuration")
        {
            MouseControlsEnabled = false,
            AllowCameraMovement = true
        };

        AddSettingsItems();
        LoadConfigValues();

        Menu.OnItemSelect += OnItemSelect;
    }

    internal void AttachTo(UIMenu parent, string label)
    {
        UIMenuItem item = new(label);
        parent.AddItem(item);
        parent.BindMenuToItem(Menu, parent.MenuItems[parent.MenuItems.Count - 1]);
    }

    private void AddSettingsItems()
    {
        Menu.AddItem(_readChanceItem);
        Menu.AddItem(_scanIntervalItem);
        Menu.AddItem(_shotspotterPollRateItem);
        Menu.AddItem(_shotspotterChanceItem);
        Menu.AddItem(_shotspotterFalseAlarmChanceItem);
        Menu.AddItem(_maxCamerasPerScanItem);
        Menu.AddItem(_debugModeItem);
        Menu.AddItem(_enableBlips);
        Menu.AddItem(_trackingBlips);
        Menu.AddItem(_blipDurationItem);
        Menu.AddItem(_saveSettingsItem);
    }

    private void LoadConfigValues()
    {
        _readChanceItem.Value = UserConfig.ReadChance;
        _scanIntervalItem.Value = UserConfig.ScanInterval;
        _shotspotterPollRateItem.Value = UserConfig.ShotspotterPollRate;
        _shotspotterChanceItem.Value = UserConfig.ShotspotterChance;
        _shotspotterFalseAlarmChanceItem.Value = UserConfig.ShotspotterFalseAlarmChance;
        _maxCamerasPerScanItem.Value = UserConfig.MaxCamerasPerScan;
        _debugModeItem.Checked = UserConfig.DebugModeEnabled;
        _enableBlips.Checked = UserConfig.EnableBlips;
        _trackingBlips.Checked = UserConfig.TrackingBlips;
        _blipDurationItem.Value = UserConfig.BlipDuration;
    }

    private void OnItemSelect(UIMenu sender, UIMenuItem item, int index)
    {
        if (item == _saveSettingsItem)
            SaveToIni();
    }

    private void SaveToIni()
    {
        try
        {
            Settings.IniReflector.WriteSingle(nameof(UserConfig.ReadChance), _readChanceItem.Value);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.ScanInterval), _scanIntervalItem.Value);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.ShotspotterPollRate), _shotspotterPollRateItem.Value);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.ShotspotterChance), _shotspotterChanceItem.Value);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.ShotspotterFalseAlarmChance),
                _shotspotterFalseAlarmChanceItem.Value);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.MaxCamerasPerScan), _maxCamerasPerScanItem.Value);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.DebugModeEnabled), _debugModeItem.Checked);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.EnableBlips), _enableBlips.Checked);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.TrackingBlips), _trackingBlips.Checked);
            Settings.IniReflector.WriteSingle(nameof(UserConfig.BlipDuration), _blipDurationItem.Value);
            
            // Reload fresh copy into memory
            Settings.IniReflector.Read(UserConfig, true);

            Game.DisplayNotification(
                "commonmenu",
                "shop_tick_icon",
                "GRIDWATCH",
                "~b~INI Update Successful",
                "Settings saved and reloaded ~g~successfully~w~!");
        }
        catch (Exception ex)
        {
            Game.DisplayNotification(
                "commonmenu",
                "mp_alerttriangle",
                "GRIDWATCH",
                "~r~INI Update Failed",
                "Error: ~w~" + ex.Message);
            Error(ex);
        }
    }

    #region Settings items

    private readonly UIMenuNumericScrollerItem<int> _readChanceItem =
        new(
            "Read Chance", "Chance a LPR scan succeeds.", 0, 100, 1);

    private readonly UIMenuNumericScrollerItem<int> _scanIntervalItem =
        new(
            "Scan Interval (MS)", "How frequent cameras are polled.", 100, 50000, 100);

    private readonly UIMenuNumericScrollerItem<int> _shotspotterPollRateItem =
        new(
            "Shotspotter Poll Rate (MS)", "How often Shotspotter polls for gunfire.", 1000, 120000, 500);

    private readonly UIMenuNumericScrollerItem<int> _shotspotterChanceItem =
        new(
            "Shotspotter Chance", "Chance of a Shotspotter event per poll.", 0, 100, 1);

    private readonly UIMenuNumericScrollerItem<int> _shotspotterFalseAlarmChanceItem =
        new(
            "False Alarm Chance", "Chance a Shotspotter call is bogus.", 0, 100, 1);

    private readonly UIMenuNumericScrollerItem<int> _maxCamerasPerScanItem =
        new(
            "Max Cameras/Scan", "How many cameras are processed per cycle.", 1, 20, 1);

    private readonly UIMenuCheckboxItem _debugModeItem =
        new("Enable Debug Mode", false, "Enable verbose logging and debug info.");

    private readonly UIMenuItem _saveSettingsItem =
        new("Save Settings", "Save current settings to INI file.")
        {
            BackColor = Color.Green,
            HighlightedBackColor = Color.DarkGreen
        };

    private readonly UIMenuCheckboxItem _enableBlips =
        new("Enable Blips", false, "Toggle camera blips on/off.");

    private readonly UIMenuCheckboxItem _trackingBlips =
        new("Enable Tracking Blips", false, "Toggle tracking blips on/off.");

    private readonly UIMenuNumericScrollerItem<int> _blipDurationItem =
        new(
            "Blip Duration (MS)", "How long camera blips last.", 100, 600000, 100);

    #endregion
}