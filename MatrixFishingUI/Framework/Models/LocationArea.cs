using System.Diagnostics.CodeAnalysis;
using StardewValley;

namespace MatrixFishingUI.Framework.Models;

public record LocationArea(string LocationName, string AreaName, string LocationReadableName)
{

	public bool TryGetGameLocation([NotNullWhen(true)] out GameLocation? gameLocation)
	{
		foreach (var location in Game1.locations)
		{
			if (!ConvertLocationNameToDataName(location).Equals(LocationName, StringComparison.OrdinalIgnoreCase)) continue;
			gameLocation = location;
			return true;
		}
		gameLocation = null;
		return false;
	}

	public static string ConvertLocationNameToDataName(GameLocation gameLocation)
	{
		var locationName = gameLocation.Name;
		if (locationName.Equals("Farm", StringComparison.OrdinalIgnoreCase)) return $"Farm_{Game1.GetFarmTypeKey()}";
		if (locationName.Equals("BeachNightMarket")) return "Beach";
		return locationName;
	}

	public virtual bool Equals(LocationArea? other)
	{
		return other is not null && LocationReadableName.Equals(other.LocationReadableName, StringComparison.OrdinalIgnoreCase);
	}

	public override int GetHashCode()
	{
		return LocationReadableName.GetHashCode(StringComparison.OrdinalIgnoreCase);
	}
}
