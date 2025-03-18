using System.Diagnostics.CodeAnalysis;
using StardewValley;

namespace MatrixFishingUI.Framework.Models;

public readonly record struct LocationArea(string LocationName, string AreaName)
{

	public bool TryGetGameLocation([NotNullWhen(true)] out GameLocation? gameLocation)
	{
		foreach (var location in Game1.locations)
		{
			if (!location.Name.Equals(LocationName, StringComparison.OrdinalIgnoreCase)) continue;
			gameLocation = location;
			return true;
		}
		gameLocation = null;
		return false;
	}
}
