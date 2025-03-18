using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MatrixFishingUI.Framework.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace MatrixFishingUI.Framework.Fish;

public static class FishHelper
{

    public static Dictionary<FishId, List<SpawningCondition>> GetFishSpawningConditions()
    {
        var result = new Dictionary<FishId, List<SpawningCondition>>();
        var locations = Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations");
        foreach (var location in locations)
        {
            var (locationName, locationData) = location;
            if (SkipLocation(locationName)) continue;
            
            var fishSpawningConditions = GetFishSpawningConditions(locationData, locationName);
            foreach (var kv in fishSpawningConditions)
            {
                var (fishId, spawningCondition) = kv;

                if (!result.TryGetValue(fishId, out var spawningConditions))
                {
                    spawningConditions = [];
                    result[fishId] = spawningConditions;
                }

                spawningConditions.AddRange(spawningCondition);
            }
        }
        
        return result;
    }

    private static Dictionary<FishId, List<SpawningCondition>> GetFishSpawningConditions(LocationData locationData, string locationName)
    {
        var allSeasons = Enum.GetValues<Season>().ToHashSet();
        var result = new Dictionary<FishId, List<SpawningCondition>>();
        foreach (var fish in locationData.Fish)
        {
            if (fish.Season is not null)
            {
                AddSpawningCondition([fish.Season.Value], fish);
                continue;
            }

            var conditionHasSeason = fish.Condition is not null &&
                (fish.Condition.Contains("LOCATION_SEASON", StringComparison.OrdinalIgnoreCase) ||
                 fish.Condition.Contains("SEASON", StringComparison.OrdinalIgnoreCase));

            if (!conditionHasSeason)
            {
                AddSpawningCondition(allSeasons, fish);
            } else
            {
                var parsedSeasons = ParseCondition(fish.Condition!);
                if (parsedSeasons is null)
                {
                    ModEntry.LogWarn($"Failed to Parse Condition for {fish.ObjectDisplayName}: {fish.Condition}");
                    continue;
                }
                AddSpawningCondition(parsedSeasons, fish);
            }
        }

        return result;

        void AddSpawningCondition(HashSet<Season> seasons, SpawnFishData fish)
        {
            var metadata = ItemRegistry.GetMetadata(fish.Id);
            var id = new FishId(metadata.LocalItemId);

            if (!result.TryGetValue(id, out var spawningConditions))
            {
                spawningConditions = [];
                result[id] = spawningConditions;
            }

            spawningConditions.Add(new SpawningCondition(new LocationArea(locationName, fish.FishAreaId), seasons));
        }
    }

    private static HashSet<Season>? ParseCondition(string conditionQuery)
    {
        var conditions = conditionQuery.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new HashSet<Season>();
        foreach (var condition in conditions)
        {
            var split = condition.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!split[0].Equals("LOCATION_SEASON", StringComparison.OrdinalIgnoreCase) && !split[0].Equals("SEASON", StringComparison.OrdinalIgnoreCase)) continue;
            for (var i = 1; i < split.Length; i++)
            {
                var rawSeason = split[i];
                if(rawSeason.Equals("Here", StringComparison.OrdinalIgnoreCase)) continue;
                if (rawSeason.Equals("spring", StringComparison.OrdinalIgnoreCase)) result.Add(Season.Spring);
                else if (rawSeason.Equals("summer", StringComparison.OrdinalIgnoreCase)) result.Add(Season.Summer);
                else if (rawSeason.Equals("fall", StringComparison.OrdinalIgnoreCase)) result.Add(Season.Fall);
                else if (rawSeason.Equals("winter", StringComparison.OrdinalIgnoreCase)) result.Add(Season.Winter);
                else ModEntry.LogError($"Unknown Season caught when parsing: {conditionQuery}, Split: {rawSeason}");
            }
        }
        

        return result.Count == 0 ? null : result;
    }
    
    private static bool SkipLocation(string key)
    {
        return key switch
        {
            "fishingGame" or "Default" or "Temp" or "BeachNightMarket" or "IslandSecret" or "Backwoods" => true,
            _ => false
        };
    }
    
    public static bool SkipFish(Farmer farmer, FishId id)
    {
        return id.Value switch
        {
            "898" or "899" or "900" or "901" or "902" => !farmer.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"),
            _ => false
        };
    }

    public static Dictionary<LocationArea, FishId[]>? GetFishByArea(GameLocation location)
    {
        if (!TryGetLocationData(location, out var locationName, out var locationData)) return null;
        var fishSpawningConditions = GetFishSpawningConditions(locationData, locationName);

        return fishSpawningConditions
            .SelectMany(kv => kv.Value.Select(spawningCondition => new KeyValuePair<FishId, SpawningCondition>(kv.Key, spawningCondition)))
            .GroupBy(kv => kv.Value.Location)
            .ToDictionary(group => group.Key, group => group.Select(kv => kv.Key).ToArray());
    }

    private static bool TryGetLocationData(GameLocation gameLocation,
        out string locationName,
        [NotNullWhen(true)] out LocationData? locationData)
    {
        var locations = Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations");
        locationName = gameLocation.Name;
        if (locationName.Equals("BeachNightMarket")) locationName = "Beach";
        return locations.TryGetValue(locationName, out locationData);
    }
    
    public static SObject GetRoeForFish(SObject fish) {
        var color = TailoringMenu.GetDyeColor(fish) ?? Color.Orange;
        if (fish.ParentSheetIndex == 698)
            color = new Color(61, 55, 42);

        var result = new ColoredObject("812", 1, color);
        result.name = fish.Name + " Roe";
        result.preserve.Value = SObject.PreserveType.Roe;
        result.preservedParentSheetIndex.Value = fish.QualifiedItemId;
        result.Price += fish.sellToStorePrice() / 2;

        return result;
    }
}

/// <summary>
/// Unqualified fish IDs.
/// </summary>
public readonly struct FishId : IEquatable<FishId>
{
    public readonly string Value;

    public FishId(string value)
    {
        Debug.Assert(!ItemRegistry.IsQualifiedItemId(value), "Fish ID has to be unqualified!");
        Value = value;
    }

    public bool Equals(FishId other) => Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is FishId other && Equals(other);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;
}

public record struct SpawningCondition(LocationArea Location, HashSet<Season> Seasons);
