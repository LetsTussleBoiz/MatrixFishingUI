using System.Text.RegularExpressions;
using MatrixFishingUI.Framework.Enums;
using MatrixFishingUI.Framework.Models;
using StardewValley;
using StardewValley.GameData.FishPonds;

namespace MatrixFishingUI.Framework.Fish;

public class VanillaProvider : IFishProvider {
	private static readonly Regex WhitespaceRegex = new(@"\s\s+|\n", RegexOptions.Compiled);

	public string Name => nameof(VanillaProvider);
	public int Priority => 0;
	private static readonly KeyValuePair<string, string>[] LegendaryPairs = [
		// Son of Crimsonfish
		new("898", "(O)159"),
		// Ms. Angler
		new("899", "(O)160"),
		// The Legend II
		new("900", "(O)163"),
		// Radioactive Carp
		new("901", "(O)682"),
		// Glacierfish Jr.
		new("902", "(O)775")
	];

	public IEnumerable<FishInfo> GetFish() {
		var data = Game1.content.Load<Dictionary<string, string>>(@"Data\Fish");
		List<FishInfo> result = [];
		var locations = FishManager.GetFishLocations();
		var pondData = Game1.content.Load<List<FishPondData>>(@"Data\FishPondData");

		foreach (var entry in data)
		{
			var key = $"(O){entry.Key}";
			locations.TryGetValue(key, out var fishLocations);
			try
			{
				if (FishHelper.SkipFish(Game1.player, key))
				{
					foreach (var pair in LegendaryPairs)
					{
						if (!pair.Key.Equals(entry.Key, StringComparison.OrdinalIgnoreCase)) continue;
						locations.TryGetValue(pair.Value, out fishLocations);
						break;
					}
				}
				var info = GetFishInfo(key, entry.Value, fishLocations ?? [], pondData);
				if (info.HasValue) result.Add(info.Value);
			} catch(Exception) {
				ModEntry.LogWarn($"Unable to process fish: {entry.Key}");
			}
		}
		return result;
	}

	private static FishInfo? GetFishInfo(string id, string data, Dictionary<SubLocation, List<int>> locations, List<FishPondData> pondData) {
		if (string.IsNullOrEmpty(data))
			return null;

		string[] bits = data.Split('/');
		SObject obj = new SObject(id.Substring(3), 1);

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
			WaterType type;
			if (bits[4] == "freshwater")
				type = WaterType.Freshwater;
			else if (bits[4] == "ocean")
				type = WaterType.Ocean;
			else
				throw new ArgumentOutOfRangeException("location", bits[4], "location must be freshwater or ocean");

			minSize = Convert.ToInt32(bits[5]);
			maxSize = Convert.ToInt32(bits[6]);

			trap = new(type);

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

			caught = new(
				Locations: locations,
				Times: times,
				Weather: weather,
				Minlevel: minLevel
			);

			string[] seasonIds = bits[6].Split(' ');
			if (seasonIds.Length >= 4)
			{
				seasons = new LuluSeason[1];
				seasons[0] = LuluSeason.All;
			}
			else
			{
				seasons = new LuluSeason[seasonIds.Length];
				for(int i = 0;i < seasonIds.Length;i++) {
					string seasonId = seasonIds[i];
					switch (seasonId) {
						case "spring":
							seasons[i] = LuluSeason.Spring;
							break;
						case "summer":
							seasons[i] = LuluSeason.Summer;
							break;
						case "fall":
							seasons[i] = LuluSeason.Fall;
							break;
						case "winter":
							seasons[i] = LuluSeason.Winter;
							break;
					}
				}
			}
		}

		string desc = obj.getDescription();
		if (desc != null)
			desc = WhitespaceRegex.Replace(desc, " ");


		// Fish Ponds
		FishPondData? pond = null;
		if (pondData != null)
			foreach (var entry in pondData) {
				bool matched = true;
				foreach (string tag in entry.RequiredTags) {
					if (!obj.HasContextTag(tag)) {
						matched = false;
						break;
					}
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
				int price = obj.Price;
				pond.SpawnTime = price > 30 ? (price > 80 ? (price > 120 ? (price > 250 ? 5 : 4) : 3) : 2) : 1;
			}
			int initial = 10;
			if (pond.PopulationGates != null) {
				foreach (int key in pond.PopulationGates.Keys)
					if (key >= initial)
						initial = key - 1;
			}
			pondInfo = new(
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

		bool legend = false;
		legend = obj.HasContextTag("fish_legendary");
		return new FishInfo(
			Id: id, // bits[0],
			Item: obj,
			Name: obj.DisplayName,
			Description: desc,
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
