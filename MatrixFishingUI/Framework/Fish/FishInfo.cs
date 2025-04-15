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

public enum CaughtStatus {
	Uncaught,
	Caught
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
	
	public CaughtStatus GetCaughtStatus(Farmer player)
	{
		if (!player.fishCaught.TryGetValue(ItemRegistry.QualifyItemId(Id), out var value)) return CaughtStatus.Uncaught;
		if (value.Length <= 0) return CaughtStatus.Uncaught;
		return value[0] > 0 ? CaughtStatus.Caught : CaughtStatus.Uncaught;
	}

	public int GetNumberCaught(Farmer player)
	{
		if (!player.fishCaught.TryGetValue(Id, out var value)) return 0;
		if (value.Length <= 0) return 0;
		return value[0];
	}
	public int GetBiggestCatch(Farmer player)
	{
		if (!player.fishCaught.TryGetValue(Id, out var value)) return 0;
		if (value.Length <= 0) return 0;
		return value[1];
	}
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