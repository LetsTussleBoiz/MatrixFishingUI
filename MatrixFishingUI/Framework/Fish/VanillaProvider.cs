using System.Globalization;
using System.Text.RegularExpressions;
using StardewValley;
using StardewValley.GameData.FishPonds;

namespace MatrixFishingUI.Framework.Fish;

public class VanillaProvider : IFishProvider {
	private static readonly Regex WhitespaceRegex = new(@"\s\s+|\n", RegexOptions.Compiled);

	public string Name => nameof(VanillaProvider);
	public int Priority => 0;
	private static readonly KeyValuePair<FishId, FishId>[] LegendaryPairs = [
		// Son of Crimsonfish
		new(new FishId("898"), new FishId("159")),
		// Ms. Angler
		new(new FishId("899"), new FishId("160")),
		// The Legend II
		new(new FishId("900"), new FishId("163")),
		// Radioactive Carp
		new(new FishId("901"), new FishId("682")),
		// Glacierfish Jr.
		new(new FishId("902"), new FishId("775"))
	];

	public IEnumerable<FishInfo> GetFish()
	{
		var fishData = DataLoader.Fish(Game1.content);
		List<FishInfo> result = [];
		var fishGlossary = FishHelper.GetFishSpawningConditions();
		var pondData = DataLoader.FishPondData(Game1.content);

		foreach (var entry in fishData)
		{
			var fishId = new FishId(entry.Key);

			fishGlossary.TryGetValue(fishId, out var fishLocations);
			try
			{
				if (FishHelper.SkipFish(Game1.player, fishId))
				{
					foreach (var pair in LegendaryPairs)
					{
						if (!pair.Key.Value.Equals(entry.Key, StringComparison.OrdinalIgnoreCase)) continue;
						fishGlossary.TryGetValue(pair.Value, out fishLocations);
						break;
					}
				}
				var info = GetFishInfo(fishId, entry.Value, fishLocations ?? [], pondData);
				if (info is not null) result.Add(info);
			} catch(Exception e) {
				ModEntry.LogWarn($"Unable to process fish: {entry.Key}");
				ModEntry.LogError(e.ToString());
			}
		}
		return result;
	}
	
	private static FishInfo? GetFishInfo(FishId id, ReadOnlySpan<char> rawData, List<SpawningCondition> spawningConditions, List<FishPondData> pondData)
	{
		if (rawData.IsEmpty) return null;
		
		// Documentation: https://stardewvalleywiki.com/Modding:Fish_data
		// "128": "Pufferfish/80/floater/1/36/1200 1600 1800 2100/summer/sunny/690 .4 685 .1/4/.3/.5/0/true",
		// "163": "Legend/110/mixed/50/50/600 2000/spring summer fall winter/rainy/688 .05/5/0/.1/10/false",
		// "717": "Crab/trap/.1/684 .45/ocean/1/20/false",
		// "MNF.MoreNewFish_ladyfish": "Ladyfish/50/dart/10/39/600 1900/spring summer/sunny/681 .35/2/.3/.2/0",
		// "FlashShifter.StardewValleyExpandedCP_Minnow": "Minnow/1/sinker/1/2/600 1800/spring summer fall winter/sunny/683 .4/1/.03/.2/0/false",
		
		const int indexName = 0;
		const int indexDifficultyNumber = 1;
		const int indexDifficultyType = 2;
		const int indexMinFishLength = 3;
		const int indexMaxFishLength = 4;
		const int indexTimes = 5;
		const int indexSeasons = 6; // Data Ignored
		const int indexWeathers = 7;
		const int indexLocationData = 8; // Data Ignored
		const int indexMaxDepth = 9; // Bobber Tile Placement
		const int indexSpawnMult = 10;
		const int indexDepthMult = 11;
		const int indexFishingLevel = 12;
		const int indexFirstCatchEligible = 13; // Optional

		const int indexTrapSpecification = 1;
		const int indexTrapChance = 2;
		const int indexTrapLocationData = 3; // Data Ignored
		const int indexWaterType = 4;
		const int indexMinTrapLength = 5;
		const int indexMaxTrapLength = 6;

		var fishInfo = new FishInfo();

		fishInfo.Id = id.Value;
		fishInfo.FishData = ItemRegistry.GetData(id.Value);
		
		var startIndex = 0;
		var currentSectionIndex = 0;
		for (var i = 0; i < rawData.Length; i++)
		{
			if (!rawData[i].Equals('/')) continue;
			var section = rawData.Slice(startIndex, i - startIndex);
			startIndex = i + 1;
			if (currentSectionIndex is indexName)
			{
				fishInfo.Name = section.ToString();
			} else if (currentSectionIndex is indexDifficultyNumber or indexTrapSpecification)
			{
				if (section.Equals("Trap", StringComparison.OrdinalIgnoreCase))
				{
					fishInfo.TrapInfo = new TrapFishInfo();
					fishInfo.FishType = FishType.Trap;
				}
				else
				{
					fishInfo.FishType = FishType.Catch;
					fishInfo.CatchInfo = new CatchFishInfo
					{
						Locations = spawningConditions
					};
					fishInfo.CatchInfo.Difficulty = ParseInt(section, "Difficulty", id, defaultValue: 0);
				}
			}

			if (fishInfo.FishType is FishType.Catch && fishInfo.CatchInfo is not null)
			{
				if (currentSectionIndex is indexDifficultyType)
				{
					fishInfo.CatchInfo.DifficultyType = section.ToString();
				} else if (currentSectionIndex is indexMinFishLength)
				{
					fishInfo.MinSize = ParseInt(section, "Catch MinSize", id, defaultValue: -1);
				} else if (currentSectionIndex is indexMaxFishLength)
				{
					fishInfo.MaxSize = ParseInt(section, "Catch MaxSize", id, defaultValue: -1);
				} else if (currentSectionIndex is indexTimes)
				{
					fishInfo.CatchInfo.Times = ParseTimes(section, id);
				} else if (currentSectionIndex is indexWeathers)
				{
					fishInfo.CatchInfo.Weather = section.ToString() switch
					{
						"sunny" => "Sunny",
						"rainy" => "Rain",
						"both" => "Any",
						_ => section.ToString()
					};
				} else if (currentSectionIndex is indexMaxDepth)
				{
					// TODO: Implement
				} else if (currentSectionIndex is indexSpawnMult)
				{
					// TODO: Implement
				} else if (currentSectionIndex is indexDepthMult)
				{
					// TODO: Implement
				} else if (currentSectionIndex is indexFishingLevel)
				{
					fishInfo.CatchInfo.Minlevel = ParseInt(section, "Min Level", id, defaultValue: -1);
				} else if (currentSectionIndex is indexFirstCatchEligible)
				{
					// TODO: Implement
				}
			}
			else if (fishInfo.TrapInfo is not null)
			{
				if (currentSectionIndex is indexTrapChance)
				{
					fishInfo.TrapInfo.CatchChance = ParseFloat(section, "Trap MinSize", id, defaultValue: 0);
				} else if (currentSectionIndex is indexWaterType)
				{
					if (section.Equals("freshwater", StringComparison.OrdinalIgnoreCase))
					{
						fishInfo.TrapInfo.WaterType = I18n.Watertype_Freshwater();
					} else if (section.Equals("ocean", StringComparison.OrdinalIgnoreCase))
					{
						fishInfo.TrapInfo.WaterType = I18n.Watertype_Ocean();
					}
					else
					{
						fishInfo.TrapInfo.WaterType = section.ToString();
					}
				} else if (currentSectionIndex is indexMinTrapLength)
				{
					fishInfo.MinSize = ParseInt(section, "Trap MinSize", id, defaultValue: -1);
				} else if (currentSectionIndex is indexMaxTrapLength)
				{
					fishInfo.MaxSize = ParseInt(section, "Trap MaxSize", id, defaultValue: -1);
				}
			}
			
			currentSectionIndex ++;
		}
		
		fishInfo.SpecialInfo = id.Value switch
		{
			"156" => I18n.Specialinfo_Ghostfish(),
			"158" => I18n.Specialinfo_Stonefish(),
			"161" => I18n.Specialinfo_Icepip(),
			"138" => I18n.Specialinfo_Tiger(),
			_ => I18n.Specialinfo_None()
		};

		if (FishHelper.SkipFish(Game1.player, id))
		{
			fishInfo.SpecialInfo = I18n.Specialinfo_ExtendedFamily();
		}

		fishInfo.Item = new SObject(id.Value, 1);
		fishInfo.Legendary = fishInfo.Item.HasContextTag("fish_legendary");
		fishInfo.Description = fishInfo.Item.getDescription();
		if (fishInfo.Description is not null)
			fishInfo.Description = WhitespaceRegex.Replace(fishInfo.Description, " ");

		// Pond Information
		var pondInfo = new PondInfo();
		FishPondData? correctPond = null;
		foreach (var pond in pondData)
		{
			var matched = true;
			foreach (var tag in pond.RequiredTags)
			{
				if (fishInfo.Item.HasContextTag(tag)) continue;
				matched = false;
				break;
			}

			if (!matched) continue;

			correctPond = pond;
			break;
		}

		if (correctPond is not null)
		{
			if (correctPond.SpawnTime == -1)
			{
				var price = fishInfo.Item.salePrice();
				pondInfo.SpawnTime = price > 30 ? (price > 80 ? (price > 120 ? (price > 250 ? 5 : 4) : 3) : 2) : 1;
			}

			var initial = 10;
			if (correctPond.PopulationGates != null)
			{
				foreach (var key in correctPond.PopulationGates.Keys.Where(key => key >= initial))
					initial = key - 1;
			}

			pondInfo.Initial = initial;
			pondInfo.ProducedItems = correctPond.ProducedItems
				.Select(x => x.ItemId)
				.Distinct()
				.Select(x => (x == "812" ? FishHelper.GetRoeForFish((SObject)fishInfo.Item) : ItemRegistry.Create(x, 1)))
				.ToList();
			pondInfo.FishPondRewards = correctPond.ProducedItems;
		}

		fishInfo.PondInfo = pondInfo;
		
		return fishInfo;
	}

	private static int ParseInt(ReadOnlySpan<char> input, string name, FishId id, int defaultValue = 0)
	{
		if (int.TryParse(input, out var result))
		{
			return result;
		}

		ModEntry.LogWarn($"Failed to parse integer for {name}. ID: {id.Value}, Bad Value: {input.ToString()}");
		return defaultValue;
	}
	
	private static float ParseFloat(ReadOnlySpan<char> input, string name, FishId id, float defaultValue = 0)
	{
		if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}

		ModEntry.LogWarn($"Failed to parse float for {name}. ID: {id.Value}, Bad Value: {input.ToString()}");
		return defaultValue;
	}
	
	private static List<TimeOfDay> ParseTimes(ReadOnlySpan<char> input, FishId id)
	{
		const int defaultStartTime = 600;
		const int defaultEndTime = 2600;
		
		var currentSpan = input;
		int index;
		
		List<TimeOfDay> times = [];
		
		while ((index = currentSpan.IndexOf(' ')) != -1)
		{
			var timeOneSlice = currentSpan.Slice(0, index);
			currentSpan = currentSpan.Slice(index + 1);
			index = currentSpan.IndexOf(' ');
			var timeTwoSlice = index == -1 ? currentSpan : currentSpan.Slice(0, index);
			
			times.Add(new TimeOfDay(
				ParseInt(timeOneSlice, "TimeStart", id, defaultStartTime), 
				ParseInt(timeTwoSlice, "TimeEnd", id, defaultEndTime)));
			
			if (index != -1)
			{
				currentSpan = currentSpan.Slice(index + 1);
			}
		}

		return times;
	}
}
