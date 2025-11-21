using RAGENativeUI;

namespace GRIDWATCH.Features.Menus;

internal static class MenuManager
{
    private static readonly MenuPool MenuPool = new();
    private static readonly UIMenu MainMenu = new("GRIDWATCH", "Main Menu");

    public static void Init()
    {
        MenuPool.Add(MainMenu);
        MainMenu.MouseControlsEnabled = false;
        MainMenu.AllowCameraMovement = true;

        var settingsMenu = new SettingsMenu();
        var alprMenu = new LicensePlateHitsMenu();
        var shotspotterMenu = new ShotspotterMenu();

        settingsMenu.AttachTo(MainMenu, "Settings");
        alprMenu.AttachTo(MainMenu, "License Plate Hits");
        shotspotterMenu.AttachTo(MainMenu, "Shotspotter");

        MenuPool.Add(settingsMenu.Menu, alprMenu.Menu, shotspotterMenu.Menu);
        
        GameFiber.StartNew(ProcessMenuLoop, "GRIDWATCH Menu Handler");
    }

    private static void ProcessMenuLoop()
    {
        while (true)
        {
            GameFiber.Yield();
            MenuPool.ProcessMenus();

            if (Game.IsKeyDown(UserConfig.MenuKey) &&
                (UserConfig.MenuModifierKey == Keys.None ||
                 Game.IsKeyDownRightNow(UserConfig.MenuModifierKey)))
            {
                MainMenu.Visible = !MainMenu.Visible;
            }
        }
    }
}