using System.Text.RegularExpressions;
using MatrixFishingUI.Framework.Enums;
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

	public IEnumerable<FishInfo> GetFish() {
		var data = Game1.content.Load<Dictionary<string, string>>(@"Data\Fish");
		List<FishInfo> result = [];
		var fishGlossary = FishHelper.GetFishSpawningConditions();
		var pondData = Game1.content.Load<List<FishPondData>>(@"Data\FishPondData");

		foreach (var entry in data)
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
				var info = GetFishInfo(fishId.Value, entry.Value, fishLocations ?? [], pondData);
				if (info is not null) result.Add(info);
			} catch(Exception) {
				ModEntry.LogWarn($"Unable to process fish: {entry.Key}");
			}
		}
		return result;
	}

	//TODO: Optimize this method
	private static FishInfo? GetFishInfo(string id, string data, List<SpawningCondition> locations, List<FishPondData> pondData) {
		if (string.IsNullOrEmpty(data))
			return null;

		string[] bits = data.Split('/');
		SObject obj = new SObject(id, 1);

		if (bits.Length < 7 || obj is null)
			return null;

		int minSize;
		int maxSize;

		LuluSeason[]? seasons;

		TrapFishInfo? trap = null;
		CatchFishInfo? caught = null;
		FishType fishType;
		if (bits[1].Equals("trap")) {
			// Trap Fish
			fishType = FishType.Trap;
			string waterType;
			if (bits[4].Equals("freshwater", StringComparison.OrdinalIgnoreCase))
				waterType = I18n.Watertype_Freshwater();
			else if (bits[4].Equals("ocean", StringComparison.OrdinalIgnoreCase))
				waterType = I18n.Watertype_Ocean();
			else
				waterType = bits[4];

			minSize = Convert.ToInt32(bits[5]);
			maxSize = Convert.ToInt32(bits[6]);

			trap = new(waterType);

			seasons = new LuluSeason[1];
			seasons[0] = LuluSeason.All;
		} else
		{
			fishType = FishType.Catch;
			if (bits.Length < 13)
				return null;

			minSize = Convert.ToInt32(bits[3]);
			maxSize = Convert.ToInt32(bits[4]);
			int minLevel = Convert.ToInt32(bits[12]);

			int[] rawTimes = bits[5].Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
			TimeOfDay[] times = new TimeOfDay[rawTimes.Length / 2];

			for (int i = 0, j = 0; i < times.Length && j < rawTimes.Length; i++, j += 2) {
				times[i] = new(
					rawTimes[j],
					rawTimes[j + 1]
				);
			}

			FishWeather weather;
			switch (bits[7]) {
				case "sunny":
					weather = FishWeather.Sunny;
					break;
				case "rainy":
					weather = FishWeather.Rain;
					break;
				case "both":
					weather = FishWeather.Any;
					break;
				default:
					throw new ArgumentOutOfRangeException("weather", bits[7]);
			}

			seasons = new LuluSeason[1];
			
			caught = new(
				Locations: locations,
				Times: times,
				Weather: weather,
				Minlevel: minLevel
			);
		}

		var desc = obj.getDescription();
		if (desc != null)
			desc = WhitespaceRegex.Replace(desc, " ");


		// Fish Ponds
		FishPondData? pond = null;
		if (pondData is not null)
			foreach (var entry in pondData) {
				var matched = true;
				foreach (var tag in entry.RequiredTags)
				{
					if (obj.HasContextTag(tag)) continue;
					matched = false;
					break;
				}
				if (!matched)
					continue;

				pond = entry;
				break;
			}

		PondInfo? pondInfo = null;
		if (pond != null) {
			// Taken from FishPond
			if (pond.SpawnTime == -1) {
				var price = obj.Price;
				pond.SpawnTime = price > 30 ? (price > 80 ? (price > 120 ? (price > 250 ? 5 : 4) : 3) : 2) : 1;
			}
			var initial = 10;
			if (pond.PopulationGates != null)
			{
				foreach (var key in pond.PopulationGates.Keys.Where(key => key >= initial))
					initial = key - 1;
			}
			pondInfo = new PondInfo(
				Initial: initial,
				SpawnTime: pond.SpawnTime,
				ProducedItems: pond.ProducedItems
					.Select(x => x.ItemId)
					.Distinct()
					.Select(x => (x == "812" ? FishHelper.GetRoeForFish(obj) : ItemRegistry.Create(x, 1)))
					.ToList(),
				FishPondRewards: pond.ProducedItems
			);
		}

		string specialInfo;
		
		switch (id)
		{
			case "156": 
				specialInfo = I18n.Specialinfo_Ghostfish();
				break;
			case "158": 
				specialInfo = I18n.Specialinfo_Stonefish();
				break;
			case "161": 
				specialInfo = I18n.Specialinfo_Icepip();
				break;
			default:
				specialInfo = I18n.Specialinfo_None();
				break;
		}
		
		if (FishHelper.SkipFish(Game1.player, new FishId(id)))
		{
			specialInfo = I18n.Specialinfo_ExtendedFamily();
		}

		var legend = obj.HasContextTag("fish_legendary");
		return new FishInfo(
			Id: id, // bits[0],
			Item: obj,
			FishData: ItemRegistry.GetData(id),
			Name: obj.DisplayName,
			Description: desc,
			SpecialInfo: specialInfo,
			Sprite: ItemRegistry.GetData(obj.ItemId).GetTexture(),
			Legendary: legend,
			MinSize: minSize,
			MaxSize: maxSize,
			Seasons: seasons,
			FishType: fishType,
			TrapInfo: trap,
			CatchInfo: caught,
			PondInfo: pondInfo
		);
	}
}
