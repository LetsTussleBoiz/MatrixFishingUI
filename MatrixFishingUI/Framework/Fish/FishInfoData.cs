using System.ComponentModel;
using System.Runtime.CompilerServices;
using MatrixFishingUI.Framework.Enums;
using MatrixFishingUI.Framework.Models;
using Microsoft.Xna.Framework.Graphics;
using PropertyChanged.SourceGenerator;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public partial class FishInfoData : INotifyPropertyChanged
{
    public FishInfo Fish { get; set; }
    public string HeaderText { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; } = "";
    public ParsedItemData? ParsedFish { get; set; }
    public bool Legendary { get; set; }
    public int MinSize { get; set; }
    public int MaxSize { get; set; }
    public LuluSeason[]? Seasons { get; set; }
    public FishType FishType { get; set; }
    // Trap Fish
    public WaterType WaterType { get; set; }
    // Caught Fish
    public Dictionary<SubLocation, List<int>>? Locations { get; set; }
    public int? StartTime { get; set; } = 0;
    public int? EndTime { get; set; } = 0;
    public FishWeather? FishWeather { get; set; }
    public int? MinLevel { get; set; } = 0;
    // Pond Info
    public int? Initial { get; set; } = 0;
    public int? SpawnTime { get; set; } = 0;
    public List<ParsedItemData>? ProducedItems { get; set; }
    // Method Calls
    public CaughtStatus CaughtStatus { get; set; }
    public int NumberCaught { get; set; }
    public int BiggestCatch { get; set; }
    [Notify] private FishInfo previous;
    [Notify] private FishInfo current;
    [Notify] private FishInfo next;
    [Notify] private int index;
    [Notify] private FishInfoTab selectedTab;
    public IReadOnlyList<FishInfoTabViewModel> AllTabs { get; } =
        Enum.GetValues<FishInfoTab>()
            .Select(tab =>
                new FishInfoTabViewModel(tab, tab == FishInfoTab.General))
            .ToArray();
    
    public static FishInfoData GetSingleFish(FishInfo fish, FishInfo prevFish, FishInfo nextFish, int index)
    {
        return new()
        {
            HeaderText = fish.Name,
            Fish = fish,
            Name = fish.Name,
            Description = fish.Description,
            ParsedFish = ItemRegistry.GetData(fish.Id),
            Legendary = fish.Legendary,
            MinSize = fish.MinSize,
            MaxSize = fish.MaxSize,
            Seasons = fish.Seasons,
            FishType = fish.FishType,
            WaterType = fish.TrapInfo?.Location ?? WaterType.None,
            Locations = fish.CatchInfo?.Locations,
            StartTime = fish.CatchInfo?.Times[0].Start,
            EndTime = fish.CatchInfo?.Times[0].End,
            FishWeather = fish.CatchInfo?.Weather,
            MinLevel = fish.CatchInfo?.Minlevel,
            Initial = fish.PondInfo?.Initial,
            SpawnTime = fish.PondInfo?.SpawnTime,
            ProducedItems = ParsePondItems(fish.PondInfo?.ProducedItems),
            CaughtStatus = fish.GetCaughtStatus(Game1.player),
            NumberCaught = fish.GetNumberCaught(Game1.player),
            BiggestCatch = fish.GetBiggestCatch(Game1.player),
            Previous = prevFish,
            Current = fish,
            Next = nextFish,
            Index = index
        };
    }

    // ReSharper disable once UnusedMember.Global
    public void PreviousFish()
    {
        var fishCatalogue = FishMenuData.GetFish().Fish;
        var index = Index == 0 ? fishCatalogue.Count - 1 : Index - 1;
        var prevFish = ModEntry.Fish.GetFish(index == 0 ? fishCatalogue[^1].ItemId : FishMenuData.GetFish().Fish[index-1].ItemId);
        var context = GetSingleFish(Previous, prevFish, Current, index);
        ViewEngine.ChangeChildMenu("Mods/Borealis.MatrixFishingUI/Views/TestView", context);
    }

    // ReSharper disable once UnusedMember.Global
    public void NextFish()
    {
        var fishCatalogue = FishMenuData.GetFish().Fish;
        var index = Index == fishCatalogue.Count - 1 ? 0 : Index + 1;
        var nextFish = ModEntry.Fish.GetFish(index == fishCatalogue.Count - 1 ? fishCatalogue[0].ItemId : FishMenuData.GetFish().Fish[index+1].ItemId);
        var context = GetSingleFish(Next, Current, nextFish, index);
        ViewEngine.ChangeChildMenu("Mods/Borealis.MatrixFishingUI/Views/TestView", context);
    }
    
    // ReSharper disable once UnusedMember.Global
    public void SelectTab(FishInfoTab tab)
    {
        SelectedTab = tab;
        foreach (var tabViewModel in AllTabs)
        {
            tabViewModel.IsActive = tabViewModel.Value == tab;
        }
    }

    public static List<ParsedItemData> ParsePondItems(List<Item>? items)
    {
        var parsedItems = new List<ParsedItemData>();
        if(items is null) return parsedItems;
        foreach (var item in items)
        {
            parsedItems.Add(ItemRegistry.GetData(item.ItemId));
        }
        ModEntry.Log(parsedItems.ToString() ?? "Error: Pond Items are null.");
        return parsedItems;
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

public enum FishInfoTab { General, CatchInfo, PondInfo }

public partial class FishInfoTabViewModel(FishInfoTab value, bool active)
    : INotifyPropertyChanged
{
    public Tuple<int, int, int, int> Margin =>
        IsActive ? new(0, 0, -12, 0) : new(0, 0, 0, 0);
    public FishInfoTab Value { get; } = value;

    [Notify] private bool isActive = active;
    public event PropertyChangedEventHandler? PropertyChanged;
}
