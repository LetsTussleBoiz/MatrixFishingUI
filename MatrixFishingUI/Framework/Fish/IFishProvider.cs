#nullable enable

namespace MatrixFishingUI.Framework.Fish;

public interface IFishProvider {

	string Name { get; }

	int Priority { get; }

	IEnumerable<FishInfo>? GetFish();
	
	IEnumerable<FishState>? GetFishState();

}
