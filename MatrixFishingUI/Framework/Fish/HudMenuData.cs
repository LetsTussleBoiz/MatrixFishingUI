using System.ComponentModel;
using System.Runtime.CompilerServices;
using MatrixFishingUI.Framework.Enums;
using MatrixFishingUI.Framework.Models;
using StardewUI.Framework;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public class HudMenuData(Dictionary<string, FishInfo> fishInfos) : INotifyPropertyChanged
{
    public Dictionary<string,FishInfo> FishInfos { get; set; } = fishInfos;
    public string Title { get; set; } = "Fish Helper";
    public List<LocalFish> LocalCatchableFish { get; set; } = [];
    public List<LocalFish> LocalUncatchableFish { get; set; } = [];

    public void UpdateLocalFish()
    {
        LocalCatchableFish.Clear();
        LocalUncatchableFish.Clear();
        var list = FishHelper.GetLocationFish(Game1.player.currentLocation, Game1.seasonIndex);
        foreach (var fish in from pair in list let zone = pair.Key from fish in pair.Value select fish)
        {
            FishInfos.TryGetValue(fish, out var fishInfo);
            if (fishInfo.Id is null) continue;
            var qualifications = QualifyFish(fishInfo);
            if (qualifications is not null && qualifications.Contains(IsFishCatchable.Yes))
            {
                LocalCatchableFish.Add(new LocalFish(
                    true, 
                    false, 
                    false, 
                    false, 
                    fishInfo, 
                    ItemRegistry.GetData(fishInfo.Id)));
            } else if (qualifications != null)
            {
                LocalUncatchableFish.Add(new LocalFish(
                    false, 
                    qualifications.Contains(IsFishCatchable.Season), 
                    qualifications.Contains(IsFishCatchable.Time), 
                    qualifications.Contains(IsFishCatchable.Weather), 
                    fishInfo, 
                    ItemRegistry.GetData(fishInfo.Id)));
            }
        }
    }

    public List<IsFishCatchable>? QualifyFish(FishInfo fish)
    {
        var currentWeather = Game1.currentLocation.GetWeather().Weather;
        var currentSeasonNumber = Game1.currentLocation.GetSeasonIndex();
        var currentSeason = currentSeasonNumber switch
        {
            0 => LuluSeason.Spring,
            1 => LuluSeason.Summer,
            2 => LuluSeason.Fall,
            3 => LuluSeason.Winter,
            _ => LuluSeason.All
        };
        var currentTime = Game1.timeOfDay;
        var list = new List<IsFishCatchable>();
        if (fish.CatchInfo is null) return null;
        var requiredWeather = fish.CatchInfo.Value.Weather.ToString();
        var requiredSeasons = fish.Seasons;
        var startTime = fish.CatchInfo.Value.Times[0].Start;
        var endTime = fish.CatchInfo.Value.Times[0].End;
        
        if (currentTime < startTime || currentTime >= endTime)
        {
            list.Add(IsFishCatchable.Time);
        }
        if (!currentWeather.Equals(requiredWeather))
        {
            list.Add(IsFishCatchable.Weather);
        }
        if (!(requiredSeasons ?? []).Contains(currentSeason))
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