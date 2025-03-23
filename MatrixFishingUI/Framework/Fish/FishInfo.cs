using MatrixFishingUI.Framework.Enums;
using MatrixFishingUI.Framework.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.GameData.FishPonds;
using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public record TimeOfDay(
	int Start,
	int End
) {

	public bool AllDay => Start <= 600 && End >= 2600;

};

public enum FishWeather {
	None,
	Rain,
	Sunny,
	Any
}

public enum FishType {
	Trap,
	Catch
}

public enum CaughtStatus {
	Uncaught,
	Caught
}

public record FishInfo(
	// Deduplication
	string Id,

	// Main Display
	Item Item,
	ParsedItemData FishData, 
	string Name,
	string? Description,
	Texture2D? Sprite,
	string SpecialInfo,

	bool Legendary,

	// Sizes
	int MinSize,
	int MaxSize,

	// Date Range
	LuluSeason[]? Seasons,

	// Extra
	FishType FishType,
	TrapFishInfo? TrapInfo,
	CatchFishInfo? CatchInfo,
	PondInfo? PondInfo
)
{
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

public record TrapFishInfo(
	string WaterType
);

public record CatchFishInfo(
	List<SpawningCondition>? Locations,
	TimeOfDay[] Times,
	FishWeather Weather,
	int Minlevel
);

public record PondInfo(
	int Initial,
	int SpawnTime,
	List<Item> ProducedItems,
	List<FishPondReward> FishPondRewards
);