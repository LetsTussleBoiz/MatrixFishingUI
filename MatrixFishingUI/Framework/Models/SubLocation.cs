using StardewValley;

namespace MatrixFishingUI.Framework.Models;

public readonly struct SubLocation : IEquatable<SubLocation> {

	public string Key { get; }
	public string Area { get; }

	public SubLocation(string key, string area) {
		Key = key;
		Area = area;
	}

	public GameLocation? Location {
		get {
			foreach (var loc in Game1.locations)
				if (loc.Name == Key)
					return loc;

			return null;
		}
	}

	public bool Equals(SubLocation other)
	{
		return Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase) &&
		       Area.Equals(other.Area, StringComparison.OrdinalIgnoreCase);
	}

	public override bool Equals(object? obj) {
		return obj is SubLocation location &&
			   Equals(location);
	}

	public override int GetHashCode() {
		return HashCode.Combine(Key.GetHashCode(StringComparison.OrdinalIgnoreCase), Area.GetHashCode(StringComparison.OrdinalIgnoreCase));
	}

	public static bool operator ==(SubLocation left, SubLocation right) {
		return left.Equals(right);
	}

	public static bool operator !=(SubLocation left, SubLocation right) {
		return !(left == right);
	}
}
