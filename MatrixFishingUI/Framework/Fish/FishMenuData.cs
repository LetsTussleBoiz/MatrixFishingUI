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
        var fishInfo = ModEntry.Fish.GetFish(new FishId(fish.ItemId));
        var index = Fish.IndexOf(fish);
        var nextFish = ModEntry.Fish.GetFish(index == Fish.Count-1 ? new FishId(Fish[0].ItemId) : new FishId(Fish[index+1].ItemId));
        var prevFish = ModEntry.Fish.GetFish(index == 0 ? new FishId(Fish[^1].ItemId) : new FishId(Fish[index-1].ItemId));
        ModEntry.Log(fishInfo.ToString() ?? string.Empty);
        if (fishInfo.CatchInfo?.Locations != null)
        {
            ModEntry.Log($"Location Amount: {fishInfo.CatchInfo.Value.Locations.Count}");
            foreach (var location in fishInfo.CatchInfo.Value.Locations)
            {
                ModEntry.Log($"Location: {location.Location.LocationName}");
            }
        }
        var context = FishInfoData.GetSingleFish(fishInfo, prevFish, nextFish, index);
        ViewEngine.OpenChildMenu("Mods/Borealis.MatrixFishingUI/Views/TestView", context);
    }

    #region Property Changes

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

    #endregion
}
