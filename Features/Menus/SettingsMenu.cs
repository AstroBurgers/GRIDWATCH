using System.Drawing;
using GRIDWATCH.Config;
using RAGENativeUI;
using RAGENativeUI.Elements;
#pragma warning disable CS0618 // Type or member is obsolete

namespace GRIDWATCH.Features.Menus;

internal sealed class SettingsMenu
{
    private readonly UIMenu _menu;

    #region Settings items

    private readonly UIMenuNumericScrollerItem<int> _readChanceItem =
        new UIMenuNumericScrollerItem<int>(
            "Read Chance", "Chance a LPR scan succeeds.", 0, 100, 1);

    private readonly UIMenuNumericScrollerItem<int> _scanIntervalItem =
        new UIMenuNumericScrollerItem<int>(
            "Scan Interval (MS)", "How frequent cameras are polled.", 100, 5000, 100);

    private readonly UIMenuNumericScrollerItem<int> _shotspotterPollRateItem =
        new UIMenuNumericScrollerItem<int>(
            "Shotspotter Poll Rate (MS)", "How often Shotspotter polls for gunfire.", 1000, 120000, 500);

    private readonly UIMenuNumericScrollerItem<int> _shotspotterChanceItem =
        new UIMenuNumericScrollerItem<int>(
            "Shotspotter Chance", "Chance of a Shotspotter event per poll.", 0, 100, 1);

    private readonly UIMenuNumericScrollerItem<int> _shotspotterFalseAlarmChanceItem =
        new UIMenuNumericScrollerItem<int>(
            "False Alarm Chance", "Chance a Shotspotter call is bogus.", 0, 100, 1);

    private readonly UIMenuNumericScrollerItem<int> _maxCamerasPerScanItem =
        new UIMenuNumericScrollerItem<int>(
            "Max Cameras/Scan", "How many cameras are processed per cycle.", 1, 20, 1);

    private readonly List<dynamic> _keyList =
        new List<dynamic>(Enum.GetValues(typeof(Keys)).Cast<object>().ToList());

    private readonly UIMenuListItem _menuKeyItem;
    private readonly UIMenuListItem _menuModifierKeyItem;

    private readonly UIMenuCheckboxItem _debugModeItem =
        new UIMenuCheckboxItem("Enable Debug Mode", false, "Enable verbose logging and debug info.");

    private readonly UIMenuItem _saveSettingsItem =
        new UIMenuItem("Save Settings", "Save current settings to INI file.")
        {
            BackColor = Color.Green,
            HighlightedBackColor = Color.DarkGreen
        };

    #endregion

    public SettingsMenu()
    {
        _menu = new UIMenu("Settings", "GRIDWATCH Configuration")
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

        _menu.OnItemSelect += OnItemSelect;
    }

    public void AttachTo(UIMenu parent, string label)
    {
        var item = new UIMenuItem(label);
        parent.AddItem(item);
        parent.BindMenuToItem(_menu, parent.MenuItems[parent.MenuItems.Count - 1]);
    }

    private void AddSettingsItems()
    {
        _menu.AddItem(_readChanceItem);
        _menu.AddItem(_scanIntervalItem);
        _menu.AddItem(_shotspotterPollRateItem);
        _menu.AddItem(_shotspotterChanceItem);
        _menu.AddItem(_shotspotterFalseAlarmChanceItem);
        _menu.AddItem(_maxCamerasPerScanItem);
        _menu.AddItem(_menuKeyItem);
        _menu.AddItem(_menuModifierKeyItem);
        _menu.AddItem(_debugModeItem);
        _menu.AddItem(_saveSettingsItem);
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