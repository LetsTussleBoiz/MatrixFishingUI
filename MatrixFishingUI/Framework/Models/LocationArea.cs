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
		if (locationName.Equals("Farm", StringComparison.OrdinalIgnoreCase)
		    && (Game1.GetFarmTypeKey().Equals("Standard", StringComparison.OrdinalIgnoreCase)
		        || Game1.GetFarmTypeKey().Equals("Beach", StringComparison.OrdinalIgnoreCase)
		        || Game1.GetFarmTypeKey().Equals("Forest", StringComparison.OrdinalIgnoreCase)
		        || Game1.GetFarmTypeKey().Equals("FourCorners", StringComparison.OrdinalIgnoreCase)
		        || Game1.GetFarmTypeKey().Equals("Hilltop", StringComparison.OrdinalIgnoreCase)
		        || Game1.GetFarmTypeKey().Equals("Wilderness", StringComparison.OrdinalIgnoreCase)
		        || Game1.GetFarmTypeKey().Equals("MeadowlandsFarm", StringComparison.OrdinalIgnoreCase)))
		{
			if(Game1.GetFarmTypeKey().Equals("Standard", StringComparison.OrdinalIgnoreCase)
			   || Game1.GetFarmTypeKey().Equals("Beach", StringComparison.OrdinalIgnoreCase)
			   || Game1.GetFarmTypeKey().Equals("Forest", StringComparison.OrdinalIgnoreCase)
			   || Game1.GetFarmTypeKey().Equals("FourCorners", StringComparison.OrdinalIgnoreCase)
			   || Game1.GetFarmTypeKey().Equals("Hilltop", StringComparison.OrdinalIgnoreCase)
			   || Game1.GetFarmTypeKey().Equals("Wilderness", StringComparison.OrdinalIgnoreCase)
			   || Game1.GetFarmTypeKey().Equals("MeadowlandsFarm", StringComparison.OrdinalIgnoreCase))
			return $"Farm_{Game1.GetFarmTypeKey()}";
			return Game1.GetFarmTypeID();
		}
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
