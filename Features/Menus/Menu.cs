using System.Drawing;
using GRIDWATCH.Config;
using GRIDWATCH.Core.EventBus;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

#pragma warning disable CS0618 // Type or member is obsolete

namespace GRIDWATCH.Features.Menus;

internal static class Menu
{
    #region Settings Menu Items

    private static readonly UIMenuNumericScrollerItem<int> ReadChanceItem =
        new("Read Chance", "Chance a LPR scan succeeds.", 0, 100, 1);

    private static readonly UIMenuNumericScrollerItem<int> ScanIntervalItem =
        new("Scan Interval (MS)", "How frequent cameras are polled.", 100, 5000, 100);

    private static readonly UIMenuNumericScrollerItem<int> ShotspotterPollRateItem =
        new("Shotspotter Poll Rate (MS)", "How often Shotspotter polls for gunfire.", 1000, 120000, 500);

    private static readonly UIMenuNumericScrollerItem<int> ShotspotterChanceItem =
        new("Shotspotter Chance", "Chance of a Shotspotter event per poll.", 0, 100, 1);

    private static readonly UIMenuNumericScrollerItem<int> ShotspotterFalseAlarmChanceItem =
        new("False Alarm Chance", "Chance a Shotspotter call is bogus.", 0, 100, 1);

    private static readonly UIMenuNumericScrollerItem<int> MaxCamerasPerScanItem =
        new("Max Cameras/Scan", "How many cameras are processed per cycle.", 1, 20, 1);

    private static readonly List<dynamic> KeyList =
    [
        ..typeof(Keys)
            .GetEnumValues()
            .Cast<object>()
            .ToList()
    ];

    private static readonly UIMenuListItem MenuKeyItem =
        new("Menu Key", KeyList, KeyList.IndexOf(Keys.O), "Key to open the GRIDWATCH menu.");

    private static readonly UIMenuListItem MenuModifierKeyItem =
        new("Modifier Key", KeyList, KeyList.IndexOf(Keys.LControlKey), "Modifier key for opening the menu.");

    private static readonly UIMenuCheckboxItem DebugModeItem =
        new("Enable Debug Mode", false, "Enable verbose logging and debug info.");

    private static readonly UIMenuItem SaveSettingsItem =
        new("Save Settings", "Save current settings to INI file.");

    #endregion

    private static readonly UIMenu MainMenu = new("GRIDWATCH", "Main Menu");
    private static readonly MenuPool MenuPool = new();

    private static readonly UIMenuItem SettingsMenuItem = new("Settings", "Configure GRIDWATCH settings.");
    private static readonly UIMenu SettingsMenu = new("Settings", "");

    private static readonly UIMenuItem LicensePlateHitsMenuItem =
        new("View All License Plate Hits", "View all detected license plate hits.");

    private static readonly UIMenu LicensePlateHitsMenu = new("View All License Plate Hits", "");


    private static readonly UIMenuItem ShotspotterMenuItem =
        new("View All Shotspotter Incidents", "View all detected gunfire incidents.");

    private static readonly UIMenu ShotspotterMenu = new("View All Shotspotter Incidents", "");


    internal static void CreateMenu()
    {
        MenuPool.Add(MainMenu, SettingsMenu, LicensePlateHitsMenu, ShotspotterMenu);

        MainMenu.MouseControlsEnabled = false;
        MainMenu.AllowCameraMovement = true;

        SettingsMenu.MouseControlsEnabled = false;
        SettingsMenu.AllowCameraMovement = true;

        LicensePlateHitsMenu.MouseControlsEnabled = false;
        LicensePlateHitsMenu.AllowCameraMovement = true;

        ShotspotterMenu.MouseControlsEnabled = false;
        ShotspotterMenu.AllowCameraMovement = true;

        SetupMenu();
        GameFiber.StartNew(ProcessMenus, "GRIDWATCH Menu Process");
    }

    private static void SetupMenu()
    {
        MainMenu.AddItems(SettingsMenuItem, LicensePlateHitsMenuItem, ShotspotterMenuItem);

        MainMenu.BindMenuToItem(SettingsMenu, SettingsMenuItem);
        MainMenu.BindMenuToItem(LicensePlateHitsMenu, LicensePlateHitsMenuItem);
        MainMenu.BindMenuToItem(ShotspotterMenu, ShotspotterMenuItem);

        SettingsMenu.AddItems(
            ReadChanceItem,
            ScanIntervalItem,
            ShotspotterPollRateItem,
            ShotspotterChanceItem,
            ShotspotterFalseAlarmChanceItem,
            MaxCamerasPerScanItem,
            MenuKeyItem,
            MenuModifierKeyItem,
            DebugModeItem,
            SaveSettingsItem
        );

        ReadChanceItem.Value = UserConfig.ReadChance;
        ScanIntervalItem.Value = UserConfig.ScanInterval;
        ShotspotterPollRateItem.Value = UserConfig.ShotspotterPollRate;
        ShotspotterChanceItem.Value = UserConfig.ShotspotterChance;
        ShotspotterFalseAlarmChanceItem.Value = UserConfig.ShotspotterFalseAlarmChance;
        MaxCamerasPerScanItem.Value = UserConfig.MaxCamerasPerScan;
        MenuKeyItem.Index = KeyList.IndexOf(UserConfig.MenuKey);
        MenuModifierKeyItem.Index = KeyList.IndexOf(UserConfig.MenuModifierKey);
        DebugModeItem.Checked = UserConfig.DebugModeEnabled;
        SaveSettingsItem.BackColor = Color.Green;
        SaveSettingsItem.HighlightedBackColor = Color.DarkGreen;
        SaveSettingsItem.Activated += (_, _) => { AppendToIni(); };

        LicensePlateHitsMenu.OnMenuOpen += OnLicensePlateMenuOpened;
        ShotspotterMenu.OnMenuOpen += OnShotspotterMenuOpened;
    }

    private static void OnLicensePlateMenuOpened(UIMenu menu)
    {
        PopulateLicensePlateHitsMenu();
    }

    private static void OnShotspotterMenuOpened(UIMenu menu)
    {
        PopulateShotspotterMenu();
    }
    
    private static void PopulateLicensePlateHitsMenu()
    {
        try
        {
            LicensePlateHitsMenu.Clear();
            var hits = EventConsumers.GetAllAlprHits();

            if (hits == null || hits.Count == 0)
            {
                LicensePlateHitsMenu.AddItem(new UIMenuItem("No Hits Found",
                    "No license plate hits have been logged yet."));
                return;
            }

            foreach (var hit in hits)
            {
                var item = new UIMenuItem(
                    $"{hit.LicensePlate} ({hit.Model})",
                    $"{hit.Message}"
                );
                LicensePlateHitsMenu.AddItem(item);
            }

            var clearItem = new UIMenuItem("Clear ALPR History", "Erase all logged hits.")
            {
                BackColor = Color.Red,
                HighlightedBackColor = Color.DarkRed
            };
            clearItem.Activated += (_, _) =>
            {
                EventConsumers.ClearAlprHits();
                Game.DisplayNotification("commonmenu", "shop_tick_icon", "GRIDWATCH", "ALPR History",
                    "Cleared successfully.");
                PopulateLicensePlateHitsMenu();
            };
            LicensePlateHitsMenu.AddItem(clearItem);
        }
        catch (Exception ex)
        {
            Error(ex);
            LicensePlateHitsMenu.Clear();
            LicensePlateHitsMenu.AddItem(new UIMenuItem("Error Loading Hits", ex.Message));
        }
    }

    private static void PopulateShotspotterMenu()
    {
        try
        {
            ShotspotterMenu.Clear();
            var incidents = EventConsumers.GetAllGunfireIncidents();

            if (incidents == null || incidents.Count == 0)
            {
                ShotspotterMenu.AddItem(new UIMenuItem("No Incidents Found",
                    "No gunfire reports have been detected yet."));
                return;
            }

            foreach (var incident in incidents)
            {
                var desc =
                    $"Time: {incident.Timestamp:MMM dd HH:mm:ss}\n" +
                    $"Location: {World.GetStreetName(incident.Location)}, {LSPD_First_Response.Mod.API.Functions.GetZoneAtPosition(incident.Location).RealAreaName}\n";

                var item = new UIMenuItem($"Incident at {incident.Timestamp:MMM dd HH:mm:ss}", desc);
                ShotspotterMenu.AddItem(item);
            }

            var clearItem = new UIMenuItem("Clear Shotspotter Logs", "Delete all logged gunfire reports.")
                {
                    BackColor = Color.Red,
                    HighlightedBackColor = Color.DarkRed
                };
            clearItem.Activated += (_, _) =>
            {
                EventConsumers.ClearGunfireIncidents();
                Game.DisplayNotification("commonmenu", "shop_tick_icon", "GRIDWATCH", "Shotspotter Logs",
                    "Cleared successfully.");
                PopulateShotspotterMenu();
            };
            ShotspotterMenu.AddItem(clearItem);
        }
        catch (Exception ex)
        {
            Error(ex);
            ShotspotterMenu.Clear();
            ShotspotterMenu.AddItem(new UIMenuItem("Error Loading Incidents", ex.Message));
        }
    }
    
    private static void AppendToIni()
    {
        try
        {
            Normal("Appending GRIDWATCH settings to INI...");

            Settings.IniReflector.WriteSingle("Read-Chance", ReadChanceItem.Value);
            Settings.IniReflector.WriteSingle("Scan-Interval", ScanIntervalItem.Value);
            Settings.IniReflector.WriteSingle("Shotspotter-Poll-Interval", ShotspotterPollRateItem.Value);
            Settings.IniReflector.WriteSingle("Shotspotter-Chance", ShotspotterChanceItem.Value);
            Settings.IniReflector.WriteSingle("Shotspotter-False-Alarm-Chance", ShotspotterFalseAlarmChanceItem.Value);
            Settings.IniReflector.WriteSingle("Max-Cameras-Per-Scan", MaxCamerasPerScanItem.Value);
            Settings.IniReflector.WriteSingle("Menu-Key", MenuKeyItem.SelectedItem);
            Settings.IniReflector.WriteSingle("Menu-Modifier-Key", MenuModifierKeyItem.SelectedItem);
            Settings.IniReflector.WriteSingle("Enable-Debug-Mode", DebugModeItem.Checked);

            Normal("Finished writing settings to INI.");

            Normal("Reloading updated INI into memory...");
            Settings.IniReflector.Read(UserConfig, true);
            Normal("Config successfully reloaded.");

            Game.DisplayNotification(
                "commonmenu",
                "shop_tick_icon",
                "GRIDWATCH",
                "~b~INI Update Successful",
                "Settings saved and reloaded ~g~successfully~w~!"
            );
        }
        catch (Exception ex)
        {
            Error(ex);
            Game.DisplayNotification(
                "commonmenu",
                "mp_alerttriangle",
                "GRIDWATCH",
                "~r~INI Update Failed",
                $"Error: ~w~{ex.Message}"
            );
        }
    }

    private static void ProcessMenus()
    {
        try
        {
            while (true)
            {
                GameFiber.Yield();
                MenuPool.ProcessMenus();
                if (!CheckModifierKey() || !Game.IsKeyDown(UserConfig.MenuKey))
                    continue;
                if (MenuRequirements())
                {
                    MainMenu.Visible = true;
                }
                else if (MainMenu.Visible)
                {
                    MainMenu.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    private static bool MenuRequirements()
    {
        return
            !UIMenu.IsAnyMenuVisible &&
            !TabView.IsAnyPauseMenuVisible;
    }

    private static bool CheckModifierKey()
    {
        return UserConfig.MenuModifierKey == Keys.None || Game.IsKeyDownRightNow(UserConfig.MenuModifierKey);
    }
}