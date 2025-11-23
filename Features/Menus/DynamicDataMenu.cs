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
    
    
    protected DynamicDataMenu(
        string title,
        Func<List<T>> dataFetcher,
        string noDataMessage,
        Action clearAction)
    {
        Menu = new(title, "");
        DataFetcher = dataFetcher;
        NoDataMessage = noDataMessage;
        ClearAction = clearAction;
        Menu.OnMenuOpen += _ => PopulateMenu();
    }

    internal void AttachTo(UIMenu parent, string label)
    {
        parent.AddItem(new UIMenuItem(label));
        parent.BindMenuToItem(Menu, parent.MenuItems[parent.MenuItems.Count - 1]);
    }

    protected virtual void PopulateMenu()
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
            PopulateMenu();
            Game.DisplayNotification("commonmenu", "shop_tick_icon",
                "GRIDWATCH", $"{Menu.SubtitleText} Logs", "Cleared.");
        };
        Menu.AddItem(clear);
    }

    protected abstract UIMenuItem BuildItem(T entry);
}