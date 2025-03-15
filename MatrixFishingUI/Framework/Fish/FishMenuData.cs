using System.ComponentModel;
using System.Runtime.CompilerServices;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public class FishMenuData : INotifyPropertyChanged
{
    public List<ParsedItemData> Fish { get; set; } = [];
    public string HeaderText { get; set; } = "";

    public static FishMenuData GetFish()
    {
        int[] fishCategory = [StardewValley.Object.FishCategory];
        var items = ItemRegistry.ItemTypes
            .Single(type => type.Identifier == ItemRegistry.type_object)
            .GetAllIds()
            .Select(id => ItemRegistry.GetDataOrErrorItem(id))
            .Where(data => fishCategory.Contains(data.Category))
            .ToList();
        return new()
        {
            HeaderText = "Fishipedia",
            Fish = items
        };
    }
    
    // ReSharper disable once UnusedMember.Global
    public void Display(ParsedItemData fish)
    {
        // ModEntry.Fish.RefreshFish();
        var fishInfo = ModEntry.Fish.GetFish(fish.ItemId);
        var index = Fish.IndexOf(fish);
        var nextFish = ModEntry.Fish.GetFish(index == Fish.Count-1 ? Fish[0].ItemId : Fish[index+1].ItemId);
        var prevFish = ModEntry.Fish.GetFish(index == 0 ? Fish[^1].ItemId : Fish[index-1].ItemId);
        ModEntry.Log(fishInfo.ToString() ?? string.Empty);
        var context = FishInfoData.GetSingleFish(fishInfo, prevFish, nextFish, index);
        ViewEngine.OpenChildMenu("Mods/Borealis.MatrixFishingUI/Views/TestView", context);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
