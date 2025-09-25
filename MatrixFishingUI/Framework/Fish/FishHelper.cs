using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MatrixFishingUI.Framework.Models;
using MatrixFishingUI.integrations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace MatrixFishingUI.Framework.Fish;

public static class FishHelper
{
    private static HashSet<string>? FarmLocalTypes { get; set; }
    private static Dictionary<string, LocationData> Locations = GetLocations()!;

    public static void DailyRefresh()
    {
        Locations = GetLocations()!;
    }

    public static Dictionary<FishId, List<SpawningCondition>> GetFishSpawningConditions()
    {
        var result = new Dictionary<FishId, List<SpawningCondition>>();
        var locations = GetLocations()!;
        var defaultLocationData = locations["Default"];
        foreach (var location in locations)
        {
            var (locationName, locationData) = location;
            if (SkipLocation(locationName)) continue;
            
            var fishSpawningConditions = GetFishSpawningConditions(locationData, defaultLocationData, locationName);
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

    private static HashSet<string>? EscaHandling(GameLocation location)
    {
        if (ModEntry.EscasApi is null) return null;
        if (FarmLocalTypes is not null) return FarmLocalTypes;
        HashSet<string> locationTypes = [];
        var waterTiles = location.waterTiles.waterTiles;
        for (int x = 0; x < waterTiles.GetLength(0); x++)
        {
            for (int y = 0; y < waterTiles.GetLength(1); y++)
            {
                if (!waterTiles[x, y].isWater) continue;
                ModEntry.EscasApi.GetFishLocationsData(location, new Vector2(x, y), 
                    out var useLocationName, out _, out _);
                if(!string.IsNullOrEmpty(useLocationName)) locationTypes.Add(useLocationName);
            }
        }

        FarmLocalTypes = locationTypes;
        return locationTypes;
    }

    public static void InvalidateCache()
    {
        FarmLocalTypes = null;
    }

    private static Dictionary<string, LocationData>? GetLocations()
    {
        var locations = DataLoader.Locations(Game1.content);
        GameLocation? farm = null;
        LocationData? farmData = null;
        
        try
        {
            farm = Game1.RequireLocation("Farm");
            farmData = farm.GetData();
            if (farmData is null)
            {
                ModEntry.LogWarn("Cannot find locationData for Farm.");
            }
        }
        catch (Exception e)
        {
            ModEntry.LogError($"Exception thrown when attempting to find Farm data: {e}");
        }

        if (farm is not null)
        {
            var escaLocations = EscaHandling(farm);
            if (escaLocations is not null && farmData is not null)
            {
                foreach (var locationName in escaLocations)
                {
                    if (locations.TryGetValue(locationName, out var locationData))
                    {
                        farmData.Fish.AddRange(locationData.Fish);
                    }
                    else
                    {
                        ModEntry.LogWarn($"The following location was not found via Esca: {locationName}");
                    }
                }
            }
            locations["Farm"] = farmData;
        }
        return locations;
    }

    private static Dictionary<FishId, List<SpawningCondition>> GetFishSpawningConditions(
        LocationData locationData, 
        LocationData defaultLocationData, 
        string locationName)
    {
        var allSeasons = Enum.GetValues<Season>().ToHashSet();
        var result = new Dictionary<FishId, List<SpawningCondition>>();
        
        foreach (var fish in locationData.Fish)
        {
            
            List<string>? specialConditions = null;
            if (fish.Condition is not null 
                && ((!fish.Condition.Contains("LOCATION_SEASON", StringComparison.OrdinalIgnoreCase) && !fish.Condition.Contains("SEASON", StringComparison.OrdinalIgnoreCase)) 
                    || fish.Condition.Contains("SEASON_DAY", StringComparison.OrdinalIgnoreCase)))
            {
                specialConditions = ParseConditionGeneric(fish.Condition);
            }
            
            if (fish.Season is not null)
            {
                AddSpawningCondition([fish.Season.Value], fish, specialConditions);
                continue;
            }

            var conditionHasSeason = fish.Condition is not null &&
                                     (fish.Condition.Contains("LOCATION_SEASON", StringComparison.OrdinalIgnoreCase) ||
                                      fish.Condition.Contains("SEASON", StringComparison.OrdinalIgnoreCase) && !fish.Condition.Contains("SEASON_DAY", StringComparison.OrdinalIgnoreCase));

            if (!conditionHasSeason)
            {
                AddSpawningCondition(allSeasons, fish, specialConditions);
            } else
            {
                var parsedSeasons = ParseConditionForSeason(fish.Condition!);
                if (parsedSeasons is null)
                {
                    ModEntry.LogWarn($"Failed to Parse Condition for {fish.ObjectDisplayName}: {fish.Condition}");
                    continue;
                }
                AddSpawningCondition(parsedSeasons, fish, specialConditions);
            }
        }

        // Ice Pip, Lava Eel, and Stonefish Hard-coded handling
        switch (Game1.currentLocation.NameOrUniqueName)
        {
            case "UndergroundMine20":
            {
                List<string>? specialConditions = null;
                AddManualSpawningCondition(allSeasons, new FishId("158"), specialConditions);
                break;
            }
            case "UndergroundMine60":
            {
                List<string>? specialConditions = null;
                AddManualSpawningCondition(allSeasons, new FishId("161"), specialConditions);
                break;
            }
            case "UndergroundMine100":
            {
                List<string>? specialConditions = null;
                AddManualSpawningCondition(allSeasons, new FishId("162"), specialConditions);
                break;
            }
        }

        return result;

        void AddSpawningCondition(HashSet<Season> seasons, SpawnFishData fish, List<string>? specialConditions = null)
        {
            if (fish.RandomItemId is not null && fish.RandomItemId.Count <= 0)
            {
                foreach (var fishId in fish.RandomItemId)
                {
                    var metadata = ItemRegistry.GetMetadata(fishId);
                    if (metadata is null) continue;
                    var id = new FishId(metadata.LocalItemId);
                    if (!result.TryGetValue(id, out var spawningConditions))
                    {
                        spawningConditions = [];
                        result[id] = spawningConditions;
                    }

                    var locationArea = new LocationArea(locationName, fish.FishAreaId, locationName);
                    if (locationArea.TryGetGameLocation(out var location))
                    {
                        locationArea = locationArea with { LocationReadableName = location.DisplayName };
                    }
                    spawningConditions.Add(new SpawningCondition(locationArea, seasons.ToList(), specialConditions));
                }
            }
            else
            {
                var metadata = ItemRegistry.GetMetadata(fish.ItemId);
                if (metadata is null) return;
                var id = new FishId(metadata.LocalItemId);
                if (!result.TryGetValue(id, out var spawningConditions) || specialConditions is not null)
                {
                    spawningConditions = [];
                    result[id] = spawningConditions;
                }

                var locationArea = new LocationArea(locationName, fish.FishAreaId, locationName);
                if (locationArea.TryGetGameLocation(out var location))
                {
                    locationArea = locationArea with { LocationReadableName = location.DisplayName };
                }
                spawningConditions.Add(new SpawningCondition(locationArea, seasons.ToList(), specialConditions));
            }
        }

        void AddManualSpawningCondition(HashSet<Season> seasons, FishId fish, List<string>? specialConditions = null)
        {
            var metadata = ItemRegistry.GetMetadata(fish.Value);
            if (metadata is null) return;
            var id = new FishId(metadata.LocalItemId);
            if (!result.TryGetValue(id, out var spawningConditions) || specialConditions is not null)
            {
                spawningConditions = [];
                result[id] = spawningConditions;
            }

            var locationArea = new LocationArea(locationName, "", locationName);
            if (locationArea.TryGetGameLocation(out var location))
            {
                locationArea = locationArea with { LocationReadableName = location.DisplayName };
            }
            spawningConditions.Add(new SpawningCondition(locationArea, seasons.ToList(), specialConditions));
        }
    }

    private static HashSet<Season>? ParseConditionForSeason(string conditionQuery)
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
    
    private static List<string> ParseConditionGeneric(string conditionQuery)
    {
        var conditions = conditionQuery.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new HashSet<string>();
        if (conditions.Length == 0)
        {
            result.Add("No special conditions.");
            return result.ToList();
        }
        foreach (var condition in conditions)
        {
            result.Add(condition);
        }

        return result.ToList();
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

    public static Dictionary<LocationArea, List<FishId>>? GetFishByArea(GameLocation location)
    {
        ModEntry.LogDebug($"Location NameOrUniqueName at function start: {location.NameOrUniqueName}");
        var locations = GetLocations()!;
        var locationName = LocationArea.ConvertLocationNameToDataName(location);
        
        if (!locations.TryGetValue(locationName, out var locationData)) return null;
        var fishSpawningConditions = GetFishSpawningConditions(locationData, locations["Default"], locationName);

        return fishSpawningConditions
            .SelectMany(kv => kv.Value.Select(spawningCondition => new KeyValuePair<FishId, SpawningCondition>(kv.Key, spawningCondition)))
            .GroupBy(kv => kv.Value.Location)
            .ToDictionary(group => group.Key, group => group.Select(kv => kv.Key).ToList());
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

public record SpawningCondition(LocationArea Location, List<Season> Seasons, List<string>? SpecialConditions = null)
{
    public bool HasSpecialConditions => SpecialConditions is not null && SpecialConditions.Count > 0;
    
    public static bool VerifyCondition(string condition, Item? targetItem, Item? inputItem)
    {
        var player = Game1.player;
        return GameStateQuery.CheckConditions(
            queryString: condition,
            player: player,
            location: player.currentLocation,
            targetItem: targetItem??null,
            inputItem: inputItem??null
        );
    }
};
