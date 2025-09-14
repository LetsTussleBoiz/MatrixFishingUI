using StardewValley;
using StardewValley.GameData.FishPonds;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public record TimeOfDay(
	int Start,
	int End
);

public enum FishType {
	Trap,
	Catch
}



public record FishInfo{
	// Deduplication
	public string Id { get; set; } = "";
	public Item? Item { get; set; }
	public ParsedItemData? FishData { get; set; }
	public string Name { get; set; } = "";
	public string Description { get; set; } = "";
	public string SpecialInfo { get; set; } = "";
	public bool Legendary { get; set; }
	public int MinSize { get; set; }
	public int MaxSize { get; set; }
	public FishType FishType { get; set; }
	public TrapFishInfo? TrapInfo { get; set; }
	public CatchFishInfo? CatchInfo { get; set; }
	public PondInfo? PondInfo { get; set; }
}

public record TrapFishInfo
{
	public string WaterType { get; set; } = "";
	public float CatchChance { get; set; }
}

public record CatchFishInfo
{
	public List<SpawningCondition>? Locations { get; set; }
	public List<TimeOfDay> Times { get; set; } = [];
	public string Weather { get; set; } = "";
	public int Difficulty { get; set; }
	public string DifficultyType { get; set; } = "";
	public int Minlevel { get; set; }
}

public record PondInfo
{
	public int Initial { get; set; }
	public int SpawnTime { get; set; }
	public List<Item> ProducedItems { get; set; } = [];
	public List<FishPondReward> FishPondRewards { get; set; } = [];
}
