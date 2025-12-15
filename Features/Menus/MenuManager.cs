using RAGENativeUI;

namespace GRIDWATCH.Features.Menus;

internal static class MenuManager
{
    private static readonly MenuPool MenuPool = new();
    private static readonly UIMenu MainMenu = new("GRIDWATCH", "Main Menu");

    internal static void Init()
    {
        MenuPool.Add(MainMenu);
        MainMenu.MouseControlsEnabled = false;
        MainMenu.AllowCameraMovement = true;

        SettingsMenu settingsMenu = new();
        LicensePlateHitsMenu alprMenu = new();
        ShotspotterMenu shotspotterMenu = new();

        settingsMenu.AttachTo(MainMenu, "Settings");
        alprMenu.AttachTo(MainMenu, "License Plate Hits");
        shotspotterMenu.AttachTo(MainMenu, "Shotspotter");

        settingsMenu.Menu.MouseControlsEnabled = false;
        settingsMenu.Menu.AllowCameraMovement = true;

        alprMenu.Menu.MouseControlsEnabled = false;
        alprMenu.Menu.AllowCameraMovement = true;

        shotspotterMenu.Menu.MouseControlsEnabled = false;
        shotspotterMenu.Menu.AllowCameraMovement = true;

        MenuPool.Add(alprMenu.Menu, shotspotterMenu.Menu, settingsMenu.Menu);

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
                MainMenu.Visible = !MainMenu.Visible;
        }
    }
}