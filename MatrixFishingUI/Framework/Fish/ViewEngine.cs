using StardewUI.Framework;
using StardewValley;

namespace MatrixFishingUI.Framework.Fish;

internal static class ViewEngine
{
    public static IViewEngine? Instance { get; } = ModEntry.ViewEngine;
    public static string ViewAssetPrefix { get; set; } = "";

    public static void OpenChildMenu(string viewName, object? context)
    {
        if (Instance is null)
        {
            throw new InvalidOperationException("ViewEngine Instance is not set up!!!");
        }

        var assetName = ViewAssetPrefix + '/' + viewName;
        var menu = Instance.CreateMenuFromAsset(assetName, context);
        var parent = Game1.activeClickableMenu;
        for(; parent?.GetChildMenu() is not null; parent = parent.GetChildMenu()) { }

        if (parent is not null)
        {
            parent.SetChildMenu(menu);
        }
        else
        {
            Game1.activeClickableMenu = menu;
        }
    }
    
    public static void ChangeChildMenu(string viewName, object? context)
    {
        var current = Game1.activeClickableMenu;
        current.exitThisMenuNoSound();
        OpenChildMenu(viewName, context);
    }
}