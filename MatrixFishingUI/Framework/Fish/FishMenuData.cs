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
            HeaderText = "All Fish",
            Fish = items
        };
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