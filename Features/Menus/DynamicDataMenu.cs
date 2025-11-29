using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace GRIDWATCH.Features.Menus;

internal abstract class DynamicDataMenu<T>
{
    internal readonly UIMenu Menu;
    protected readonly Func<List<T>> DataFetcher;
    protected readonly string NoDataMessage;
    protected readonly Action ClearAction;
    protected readonly BlipType? BlipType;
    
    
    protected DynamicDataMenu(
        string title,
        Func<List<T>> dataFetcher,
        string noDataMessage,
        Action clearAction,
        BlipType? blipType = null)
    {
        Menu = new UIMenu(title, "");
        DataFetcher = dataFetcher;
        NoDataMessage = noDataMessage;
        ClearAction = clearAction;
        BlipType = blipType;
        
        Menu.OnMenuOpen += _ => PopulateMenu();
    }

    internal void AttachTo(UIMenu parent, string label)
    {
        parent.AddItem(new UIMenuItem(label));
        parent.BindMenuToItem(Menu, parent.MenuItems[parent.MenuItems.Count - 1]);
    }

    private void PopulateMenu()
    {
        Menu.Clear();
        var data = DataFetcher() ?? [];
        if (data.Count == 0)
        {
            Menu.AddItem(new UIMenuItem("No Entries", NoDataMessage));
            return;
        }

        foreach (var item in data)
            Menu.AddItem(BuildItem(item));

        var clear = new UIMenuItem("Clear", "Delete all entries.")
        {
            BackColor = Color.Red,
            HighlightedBackColor = Color.DarkRed
        };
        clear.Activated += (_, _) =>
        {
            ClearAction?.Invoke();
            if (BlipType.HasValue)
                BlipHandler.CleanupBlips(BlipType.Value);
            PopulateMenu();
            Game.DisplayNotification("commonmenu", "shop_tick_icon",
                "GRIDWATCH", $"{Menu.SubtitleText} Logs", "Cleared.");
        };
        Menu.AddItem(clear);
    }

    protected abstract UIMenuItem BuildItem(T entry);
}