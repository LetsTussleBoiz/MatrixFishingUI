using MatrixFishingUI.Framework.Models;

namespace MatrixFishingUI.Framework.Fish;

public class FishManager
{
    private readonly List<IFishProvider> _providers = new();
    private Dictionary<string,FishInfo> _fish = new();
    private bool _loaded;
    
    public FishManager(ModEntry mod)
    {
        _providers.Add(new VanillaProvider());
    }
    
    public void RefreshFish() {

        Dictionary<string, FishInfo> working = new();

        foreach (IFishProvider provider in _providers) {
            int provided = 0;
            IEnumerable<FishInfo>? fish;

            try {
                fish = provider.GetFish();
            } catch(Exception) {
                ModEntry.LogWarn($"An error occurred getting fish from provider {provider.Name}.");
                continue;
            }

            if (fish is not null)
                foreach (FishInfo info in fish) {
                    if (!working.ContainsKey(info.Id)) {
                        working[info.Id] = info;
                        provided++;
                    }
                }

            ModEntry.LogTrace($"Loaded {provided} fish from {provider.Name}");
        }

        _fish = working;
        _loaded = true;
        ModEntry.Log($"Loaded {_fish.Count} fish from {_providers.Count} providers.");
    }

    public FishInfo GetFish(string id)
    {
        _fish.TryGetValue($"(O){id}", out var value);
        return value;
    }

    public Dictionary<string, FishInfo> GetAllFish()
    {
        return _fish;
    }
    
    #region Locations

    public static Dictionary<string, Dictionary<SubLocation, List<int>>> GetFishLocations() {

        var result = FishHelper.GetFishLocations();
        return result;
    }

    #endregion
}