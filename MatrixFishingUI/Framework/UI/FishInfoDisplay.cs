using System.ComponentModel;
using System.Runtime.CompilerServices;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.UI;

public class FishInfoDisplay : INotifyPropertyChanged
{
    
    public void Display(ParsedItemData fish)
    {
        var fishInfo = ModEntry.Fish.GetFish(fish.ItemId);
        ModEntry.Log($"ItemID: {fish.ItemId}");
        ModEntry.Log(fishInfo.ToString() ?? string.Empty);
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