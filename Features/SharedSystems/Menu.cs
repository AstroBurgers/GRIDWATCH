using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;
using RAGENativeUI;
using RAGENativeUI.PauseMenu;

namespace GRIDWATCH.Features.SharedSystems;

internal static class Menu
{
    private static readonly UIMenu MainMenu = new("GRIDWATCH", "Main Menu");
    private static readonly MenuPool MenuPool = new();
    
    internal static void CreateMenu()
    {
        MenuPool.Add(MainMenu);
        MainMenu.RefreshIndex();
        
        SetupMenu();
    }

    private static void SetupMenu()
    {
        
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