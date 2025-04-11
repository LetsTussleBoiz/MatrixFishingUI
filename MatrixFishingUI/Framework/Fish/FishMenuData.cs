using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MatrixFishingUI.Framework.Fish;

public class FishMenuData : INotifyPropertyChanged
{
    public List<FishInfo> Fish { get; set; } = [];
    public string HeaderText { get; set; } = "";

    public static FishMenuData GetFish()
    {
        // int[] fishCategory = [StardewValley.Object.FishCategory];
        // var items = ItemRegistry.ItemTypes
        //     .Single(type => type.Identifier == ItemRegistry.type_object)
        //     .GetAllIds()
        //     .Select(id => ItemRegistry.GetDataOrErrorItem(id))
        //     .Where(data => fishCategory.Contains(data.Category))
        //     .ToList();
        // return new()
        // {
        //     HeaderText = "Fishipedia",
        //     Fish = items
        // };
        return new FishMenuData
        {
            HeaderText = I18n.Ui_Fishipedia_Title(),
            Fish = ModEntry.Fish.GetAllFish().Values.ToList()
        };
    }
    
    // ReSharper disable once UnusedMember.Global
    public void Display(FishInfo fish)
    {
        if (string.IsNullOrEmpty(fish.Id))
        {
            ModEntry.LogError($"The following fish has no ItemId: {fish.Name}");
            return;
        }
        var index = Fish.IndexOf(fish);
        var nextFish = ModEntry.Fish.GetFish(index == Fish.Count-1 ? new FishId(Fish[0].Id) : new FishId(Fish[index+1].Id));
        var prevFish = ModEntry.Fish.GetFish(index == 0 ? new FishId(Fish[^1].Id) : new FishId(Fish[index-1].Id));
        ModEntry.Log(fish.ToString() ?? string.Empty);
        var context = FishInfoData.GetSingleFish(fish, prevFish, nextFish, index);
        ViewEngine.OpenChildMenu("Mods/Borealis.MatrixFishingUI/Views/FishInformation", context);
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
