using System.ComponentModel;
using System.Runtime.CompilerServices;
using MatrixFishingUI.Framework.Enums;
using StardewValley;

namespace MatrixFishingUI.Framework.Fish;

public class HudMenuData(Dictionary<string, FishInfo> fishInfos) : INotifyPropertyChanged
{
    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<string,FishInfo> FishInfos { get; set; } = fishInfos;
    public string Title { get; set; } = "Fish Helper";
    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<string, LocalFish> FilteredCatchables { get; set; } = [];
    // ReSharper disable once MemberCanBePrivate.Global
    public List<LocalFish> LocalCatchableFish { get; set; } = [];
    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<string, LocalFish> FilteredUncatchables { get; set; } = [];
    // ReSharper disable once MemberCanBePrivate.Global
    public List<LocalFish> LocalUncatchableFish { get; set; } = [];
    public bool IsThereFish { get; set; }

    public void UpdateLocalFish()
    {
        IsThereFish = false;
        FilteredCatchables.Clear();
        FilteredUncatchables.Clear();
        LocalCatchableFish.Clear();
        LocalUncatchableFish.Clear();
        var springFish = FishHelper.GetLocationFish(Game1.player.currentLocation, 0);
        var summerFish = FishHelper.GetLocationFish(Game1.player.currentLocation, 1);
        var fallFish = FishHelper.GetLocationFish(Game1.player.currentLocation, 2);
        var winterFish = FishHelper.GetLocationFish(Game1.player.currentLocation, 3);
        IsThereFish = true;
        var currentWeather = Game1.currentLocation.GetWeather().Weather;
        var currentSeasonNumber = Game1.currentLocation.GetSeasonIndex();
        var currentTime = Game1.timeOfDay;
        var counter = 0;
        foreach (var (_, value) in springFish)
        {
            counter += TryAddFish(value, currentWeather, currentSeasonNumber, currentTime);
        }
        foreach (var (_, value) in summerFish)
        {
            counter += TryAddFish(value, currentWeather, currentSeasonNumber, currentTime);
        }
        foreach (var (_, value) in fallFish)
        {
            counter += TryAddFish(value, currentWeather, currentSeasonNumber, currentTime);
        }
        foreach (var (_, value) in winterFish)
        {
            counter += TryAddFish(value, currentWeather, currentSeasonNumber, currentTime);
        }
        LocalCatchableFish = new List<LocalFish>(FilteredCatchables.Values);
        LocalUncatchableFish = new List<LocalFish>(FilteredUncatchables.Values);
        if (counter == 0) IsThereFish = false; 
        ModEntry.Log($"Out of {counter} local fish, {LocalCatchableFish.Count} are catchable and {LocalUncatchableFish.Count} are uncatchable.");
    }

    private int TryAddFish(List<string> fishList, string currentWeather, int currentSeasonNumber, int currentTime)
    {
        var counter = 0;
        foreach (var fish in fishList)
        {
            FishInfos.TryGetValue(fish, out var fishInfo);
            if (fishInfo.Id is null) continue;
            var qualifications = QualifyFish(fishInfo, currentWeather, currentSeasonNumber, currentTime);
            var localFish = new LocalFish(
                qualifications.Contains(IsFishCatchable.Yes),
                qualifications.Contains(IsFishCatchable.Season),
                qualifications.Contains(IsFishCatchable.Time),
                qualifications.Contains(IsFishCatchable.Weather),
                fishInfo.GetCaughtStatus(Game1.player) is CaughtStatus.Caught,
                fishInfo,
                ItemRegistry.GetData(fishInfo.Id));
            if (localFish.Catchable && !FilteredCatchables.ContainsKey(localFish.Name))
            {
                counter++;
                FilteredCatchables.Add(localFish.Name, localFish);
            }
            else if(!FilteredUncatchables.ContainsKey(localFish.Name) && !FilteredCatchables.ContainsKey(localFish.Name))
            {
                counter++;
                FilteredUncatchables.Add(localFish.Name, localFish);
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
        var updatedWeather = currentWeather switch
        {
            "Rain" => FishWeather.Rain,
            "GreenRain" => FishWeather.Rain,
            "Storm" => FishWeather.Rain,
            "Wind" => FishWeather.Sunny,
            "Snow" => FishWeather.Sunny,
            "Sun" => FishWeather.Sunny,
            _ => FishWeather.Sunny
        };
        var list = new List<IsFishCatchable>();
        // Can be null if it's a Trap Fish or smth non-fish
        if (fish.CatchInfo is null) return list;
        var requiredWeather = fish.CatchInfo.Value.Weather;
        var requiredSeasons = fish.Seasons;
        var startTime = fish.CatchInfo.Value.Times[0].Start;
        var endTime = fish.CatchInfo.Value.Times[0].End;
        
        if (currentTime < startTime || currentTime >= endTime)
        {
            list.Add(IsFishCatchable.Time);
        }
        if (requiredWeather is not FishWeather.Any && !updatedWeather.Equals(requiredWeather))
        {
            list.Add(IsFishCatchable.Weather);
        }
        if (!(requiredSeasons ?? []).Contains(currentSeason) && !(requiredSeasons ?? []).Contains(LuluSeason.All))
        {
            list.Add(IsFishCatchable.Season);
        }
        if (list.Count == 0)
        {
            list.Add(IsFishCatchable.Yes);
        }
        return list;
    }
    // <----------------------THE GREAT DIVIDER----------------------------> //
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

public enum IsFishCatchable
{
    Yes = -1,
    Time = 0,
    Season = 1,
    Weather = 2
}