using System.ComponentModel;
using System.Runtime.CompilerServices;
using MatrixFishingUI.Framework.Enums;
using MatrixFishingUI.Framework.Models;
using PropertyChanged.SourceGenerator;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public partial class FishInfoData : INotifyPropertyChanged
{
    public FishInfo Fish { get; set; }
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
    public List<string>? Locations { get; set; }
    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
    public FishWeather? FishWeather { get; set; }
    public int? MinLevel { get; set; } = 0;
    // Pond Info
    public int? Initial { get; set; } = 0;
    public int? SpawnTime { get; set; } = 0;
    public string? SpawnTimeString { get; set; } = "";
    public PondItemData? PondItems { get; set; }
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
        return new FishInfoData
        {
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
            Locations = GetLocations(fish.CatchInfo?.Locations?.Keys.ToList()),
            StartTime = FormatTime(fish.CatchInfo?.Times[0].Start),
            EndTime = FormatTime(fish.CatchInfo?.Times[0].End),
            FishWeather = fish.CatchInfo?.Weather,
            MinLevel = fish.CatchInfo?.Minlevel,
            Initial = fish.PondInfo?.Initial,
            SpawnTime = fish.PondInfo?.SpawnTime,
            SpawnTimeString = $"Spawn Time: {fish.PondInfo?.SpawnTime} days",
            PondItems = PondItemData.GetPondItems(fish.PondInfo, fish.Item),
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
        var localIndex = Index == 0 ? fishCatalogue.Count - 1 : Index - 1;
        var prevFish = ModEntry.Fish.GetFish(localIndex == 0 ? fishCatalogue[^1].ItemId : FishMenuData.GetFish().Fish[localIndex-1].ItemId);
        var context = GetSingleFish(Previous, prevFish, Current, localIndex);
        ViewEngine.ChangeChildMenu("Mods/Borealis.MatrixFishingUI/Views/TestView", context);
    }

    // ReSharper disable once UnusedMember.Global
    public void NextFish()
    {
        var fishCatalogue = FishMenuData.GetFish().Fish;
        var localIndex = Index == fishCatalogue.Count - 1 ? 0 : Index + 1;
        var nextFish = ModEntry.Fish.GetFish(localIndex == fishCatalogue.Count - 1 ? fishCatalogue[0].ItemId : FishMenuData.GetFish().Fish[localIndex+1].ItemId);
        var context = GetSingleFish(Next, Current, nextFish, localIndex);
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

    private static string FormatTime(int? time)
    {
        var timeEdit = time / 100;
        if (timeEdit > 12)
        {
            if (timeEdit >= 24)
            {
                return timeEdit == 24 ? $"{timeEdit - 12}:00am " : $"{timeEdit - 24}:00am ";
            }
            return $"{timeEdit - 12}:00pm ";
        }
        return $"{timeEdit}:00am ";
    }

    private static List<string> GetLocations(List<SubLocation>? list)
    {
        var toReturn = new List<string>();
        if (list is null) return toReturn;
        foreach (var sublocation in list)
        {
            if (sublocation.Location != null) toReturn.Add(sublocation.Location.DisplayName);
        }
        return toReturn;
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
