using System.ComponentModel;
using System.Runtime.CompilerServices;
using MatrixFishingUI.Framework.Enums;
using PropertyChanged.SourceGenerator;
using StardewValley;

namespace MatrixFishingUI.Framework.Fish;

public class HudMenuData() : INotifyPropertyChanged
{
    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<FishId,FishInfo> FishInfos { get; set; } = [];
    public Dictionary<FishId,FishState> FishStates { get; set; } = [];
    // ReSharper disable once UnusedMember.Global
    public string Title { get; set; } = I18n.Ui_Hud_Title();
    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<string, LocalFish> FilteredCatchables { get; set; } = [];
    // ReSharper disable once MemberCanBePrivate.Global
    public List<LocalFish> LocalCatchableFish { get; set; } = [];
    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<string, LocalFish> FilteredUncatchables { get; set; } = [];
    // ReSharper disable once MemberCanBePrivate.Global
    public List<LocalFish> LocalUncatchableFish { get; set; } = [];
    public bool IsThereFish { get; set; }
    [Notify] public string hudSize { get; set; } = GetHudSize();
    [Notify] public string hudColumns { get; set; } = $"count: {ModEntry.Config.HudColumns}";
    [Notify] public string frameSize { get; set; } = GetFrameSize();

    private static string GetHudSize()
    {
        return ModEntry.Config.HudSize switch
        {
            "100%" => "scale: 1.0",
            "90%" => "scale: 0.9",
            "80%" => "scale: 0.8",
            "70%" => "scale: 0.7",
            _ => "scale: 1.0"
        };
    }

    private static string GetFrameSize()
    {
        return ModEntry.Config.HudColumns switch
        {
            "4" => "302px content",
            "5" => "372px content",
            "6" => "442px content",
            _ => "302px content"
        };
    }

    //TODO: Optimize this method
    public void UpdateLocalFish(Dictionary<FishId, FishInfo> fishInfos, Dictionary<FishId, FishState> fishStates)
    {
        FishInfos = fishInfos;
        FishStates = fishStates;
        IsThereFish = false;
        FilteredCatchables.Clear();
        FilteredUncatchables.Clear();
        LocalCatchableFish.Clear();
        LocalUncatchableFish.Clear();
        var fishByArea = FishHelper.GetFishByArea(Game1.currentLocation);
        if (fishByArea is null) return;
        IsThereFish = true;
        var currentWeather = Game1.currentLocation.GetWeather().Weather switch
        {
            "Sun" => "Sunny",
            "Rain" => "Rain",
            "GreenRain" => "GreenRain",
            "Storm" => "Rain",
            "Wind" => "Sunny",
            "Snow" => "Sunny",
            _ => Game1.currentLocation.GetWeather().Weather
        };
        var currentSeasonNumber = Game1.currentLocation.GetSeasonIndex();
        var currentTime = Game1.timeOfDay;
        var counter = 0;
        foreach (var (_, value) in fishByArea)
        {
            counter += TryAddFish(value, currentWeather, currentSeasonNumber, currentTime);
        }
        LocalCatchableFish = new List<LocalFish>(FilteredCatchables.Values);
        LocalUncatchableFish = new List<LocalFish>(FilteredUncatchables.Values);
        if (counter == 0) IsThereFish = false; 
        ModEntry.LogDebug($"Out of {counter} local fish, {LocalCatchableFish.Count} are catchable and {LocalUncatchableFish.Count} are uncatchable.");
    }

    private int TryAddFish(FishId[] fishList, string currentWeather, int currentSeasonNumber, int currentTime)
    {
        var counter = 0;
        foreach (var fish in fishList)
        {
            FishInfos.TryGetValue(fish, out var fishInfo);
            FishStates.TryGetValue(fish, out var fishState);
            if (fishInfo is null || fishState is null) continue;
            var qualifications = QualifyFish(fishInfo, currentWeather, currentSeasonNumber, currentTime);
            var localFish = new LocalFish(
                qualifications.Contains(IsFishCatchable.Yes),
                qualifications.Contains(IsFishCatchable.Season),
                qualifications.Contains(IsFishCatchable.Time),
                qualifications.Contains(IsFishCatchable.Weather),
                qualifications.Contains(IsFishCatchable.Level),
                fishState.GetCaughtStatus(Game1.player) is CaughtStatus.Caught,
                qualifications.Contains(IsFishCatchable.Condition),
                fishInfo,
                ItemRegistry.GetData(fishInfo.Id));
            if (localFish.Catchable && !FilteredCatchables.ContainsKey(localFish.Name))
            {
                if (ModEntry.Config.HideCollected && !localFish.HasBeenCaught)
                {
                    counter++;
                    FilteredCatchables.Add(localFish.Name, localFish);
                } else if (!ModEntry.Config.HideCollected)
                {
                    counter++;
                    FilteredCatchables.Add(localFish.Name, localFish);
                }
            }
            else if(!FilteredUncatchables.ContainsKey(localFish.Name) && !FilteredCatchables.ContainsKey(localFish.Name))
            {
                if (ModEntry.Config.OnlyCatchableToday && localFish is { BadSeason: false, BadLevel: false, BadWeather: false })
                {
                    if (ModEntry.Config.HideCollected && !localFish.HasBeenCaught)
                    {
                        counter++;
                        FilteredUncatchables.Add(localFish.Name, localFish);
                    } else if (!ModEntry.Config.HideCollected)
                    {
                        counter++;
                        FilteredUncatchables.Add(localFish.Name, localFish);
                    }
                } else if (!ModEntry.Config.OnlyCatchableToday && ModEntry.Config.OnlyCatchableSeason && localFish is { BadSeason: false})
                {
                    if (ModEntry.Config.HideCollected && !localFish.HasBeenCaught)
                    {
                        counter++;
                        FilteredUncatchables.Add(localFish.Name, localFish);
                    } else if (!ModEntry.Config.HideCollected)
                    {
                        counter++;
                        FilteredUncatchables.Add(localFish.Name, localFish);
                    }
                }
                else if(!ModEntry.Config.OnlyCatchableSeason && !ModEntry.Config.OnlyCatchableToday)
                {
                    if (ModEntry.Config.HideCollected && !localFish.HasBeenCaught)
                    {
                        counter++;
                        FilteredUncatchables.Add(localFish.Name, localFish);
                    } else if (!ModEntry.Config.HideCollected)
                    {
                        counter++;
                        FilteredUncatchables.Add(localFish.Name, localFish);
                    }
                }
            }
        }
        return counter;
    }

    private static List<IsFishCatchable> QualifyFish(FishInfo fish, string currentWeather, int currentSeasonNumber, int currentTime)
    {
        var currentSeason = currentSeasonNumber switch
        {
            0 => LuluSeason.Spring,
            1 => LuluSeason.Summer,
            2 => LuluSeason.Fall,
            3 => LuluSeason.Winter,
            _ => LuluSeason.All
        };
        var currentLevel = Game1.player.FishingLevel;
        var list = new List<IsFishCatchable>();
        // Can be null if it's a Trap Fish or smth non-fish
        if (fish.CatchInfo is null) return list;
        var requiredWeather = fish.CatchInfo.Weather;
        var locations = fish.CatchInfo.Locations;
        var requiredSeasons = new HashSet<LuluSeason>();
        var isSpecialConditionsMet = true;
        if (locations is not null)
        {
            foreach (var spawningCondition in locations)
            {
                if (!spawningCondition.Location.TryGetGameLocation(out var location)) continue;
                if (!location.Equals(Game1.currentLocation)) continue;
                var seasons = spawningCondition.Seasons;
                foreach (var season in seasons)
                {
                    requiredSeasons.Add(season switch
                    {
                        Season.Spring => LuluSeason.Spring,
                        Season.Summer => LuluSeason.Summer,
                        Season.Fall => LuluSeason.Fall,
                        Season.Winter => LuluSeason.Winter,
                        _ => LuluSeason.All
                    });
                }

                if (spawningCondition.HasSpecialConditions && spawningCondition.SpecialConditions is not null)
                {
                    foreach (var condition in spawningCondition.SpecialConditions)
                    {
                        if (SpawningCondition.VerifyCondition(condition, null, null)) continue;
                        isSpecialConditionsMet = false;
                    }
                }
            }
        }
        var requiredLevel = fish.CatchInfo.Minlevel;
        // Special Catch for Ice Pip, Stonefish, and Ghostfish
        if (fish.Id.Equals("161", StringComparison.OrdinalIgnoreCase) 
            || fish.Id.Equals("158", StringComparison.OrdinalIgnoreCase)
            || fish.Id.Equals("156", StringComparison.OrdinalIgnoreCase))
        {
            requiredLevel = 0;
        }

        var onTime = false;
        foreach (var (startTime, endTime) in fish.CatchInfo.Times)
        {
            if (currentTime >= startTime && currentTime <= endTime)
            {
                onTime = true;
            }
        }
        
        if (!onTime)
        {
            list.Add(IsFishCatchable.Time);
        }
        if (requiredLevel > currentLevel)
        {
            list.Add(IsFishCatchable.Level);
        }
        if (!requiredWeather.Equals("Any", StringComparison.OrdinalIgnoreCase) 
            && !currentWeather.Equals(requiredWeather, StringComparison.OrdinalIgnoreCase))
        {
            list.Add(IsFishCatchable.Weather);
        }
        if (!requiredSeasons.Contains(currentSeason) && !requiredSeasons.Contains(LuluSeason.All))
        {
            list.Add(IsFishCatchable.Season);
        }
        if (!isSpecialConditionsMet)
        {
            list.Add(IsFishCatchable.Condition);
        }
        if (list.Count == 0)
        {
            list.Add(IsFishCatchable.Yes);
        }
        return list;
    }

    #region PropertyChanges

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

public enum IsFishCatchable
{
    Yes = -1,
    Time = 0,
    Season = 1,
    Weather = 2,
    Level = 3,
    Condition = 4
}