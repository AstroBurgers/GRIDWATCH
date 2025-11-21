using System.Drawing;
using GRIDWATCH.Config;
using RAGENativeUI;
using RAGENativeUI.Elements;
#pragma warning disable CS0618 // Type or member is obsolete

namespace GRIDWATCH.Features.Menus;

internal sealed class SettingsMenu
{
    internal readonly UIMenu Menu;

    #region Settings items

    private readonly UIMenuNumericScrollerItem<int> _readChanceItem =
        new(
            "Read Chance", "Chance a LPR scan succeeds.", 0, 100, 1);

    private readonly UIMenuNumericScrollerItem<int> _scanIntervalItem =
        new(
            "Scan Interval (MS)", "How frequent cameras are polled.", 100, 5000, 100);

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

    private readonly List<dynamic> _keyList = new(Enum.GetValues(typeof(Keys)).Cast<object>().ToList());

    private readonly UIMenuListItem _menuKeyItem;
    private readonly UIMenuListItem _menuModifierKeyItem;

    private readonly UIMenuCheckboxItem _debugModeItem =
        new("Enable Debug Mode", false, "Enable verbose logging and debug info.");

    private readonly UIMenuItem _saveSettingsItem =
        new("Save Settings", "Save current settings to INI file.")
        {
            BackColor = Color.Green,
            HighlightedBackColor = Color.DarkGreen
        };

    #endregion

    public SettingsMenu()
    {
        Menu = new UIMenu("Settings", "GRIDWATCH Configuration")
        {
            MouseControlsEnabled = false,
            AllowCameraMovement = true
        };

        // Initialize key items now that key list exists
        _menuKeyItem = new UIMenuListItem(
            "Menu Key", _keyList, _keyList.IndexOf(UserConfig.MenuKey),
            "Key to open the GRIDWATCH menu.");

        _menuModifierKeyItem = new UIMenuListItem(
            "Modifier Key", _keyList, _keyList.IndexOf(UserConfig.MenuModifierKey),
            "Modifier key for opening the menu.");

        AddSettingsItems();
        LoadConfigValues();

        Menu.OnItemSelect += OnItemSelect;
    }

    public void AttachTo(UIMenu parent, string label)
    {
        var item = new UIMenuItem(label);
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
        Menu.AddItem(_menuKeyItem);
        Menu.AddItem(_menuModifierKeyItem);
        Menu.AddItem(_debugModeItem);
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
        _menuKeyItem.Index = _keyList.IndexOf(UserConfig.MenuKey);
        _menuModifierKeyItem.Index = _keyList.IndexOf(UserConfig.MenuModifierKey);
        _debugModeItem.Checked = UserConfig.DebugModeEnabled;
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
            // Write each menu item to INI
            Settings.IniReflector.WriteSingle("Read-Chance", _readChanceItem.Value);
            Settings.IniReflector.WriteSingle("Scan-Interval", _scanIntervalItem.Value);
            Settings.IniReflector.WriteSingle("Shotspotter-Poll-Interval", _shotspotterPollRateItem.Value);
            Settings.IniReflector.WriteSingle("Shotspotter-Chance", _shotspotterChanceItem.Value);
            Settings.IniReflector.WriteSingle("Shotspotter-False-Alarm-Chance",
                _shotspotterFalseAlarmChanceItem.Value);
            Settings.IniReflector.WriteSingle("Max-Cameras-Per-Scan", _maxCamerasPerScanItem.Value);
            Settings.IniReflector.WriteSingle("Menu-Key", _menuKeyItem.SelectedItem);
            Settings.IniReflector.WriteSingle("Menu-Modifier-Key", _menuModifierKeyItem.SelectedItem);
            Settings.IniReflector.WriteSingle("Enable-Debug-Mode", _debugModeItem.Checked);

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
        }
    }
}