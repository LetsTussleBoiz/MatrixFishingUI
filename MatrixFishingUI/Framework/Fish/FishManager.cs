using MatrixFishingUI.Framework.Models;
using StardewModdingAPI;

namespace MatrixFishingUI.Framework.Fish;

public class FishManager
{
    private readonly List<IFishProvider> _providers = new();
    private Dictionary<string,FishInfo> _fish = new();
    private bool _loaded;
    
    public FishManager(ModEntry mod)
    {
        _providers.Add(new VanillaProvider(mod));
    }
    
    private void RefreshFish() {

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

            ModEntry.Log($"Loaded {provided} fish from {provider.Name}");
        }

        _fish = working;
        _loaded = true;
        ModEntry.Log($"Loaded {_fish.Count} fish from {_providers.Count} providers.");
    }

    public FishInfo GetFish(string id)
    {
        _fish.TryGetValue(id, out var value);
        return value;
    }
    
    #region Locations

    private static void AddFish(SubLocation loc, int[] seasons, string fish, Dictionary<string, Dictionary<SubLocation, List<int>>> result ) {
        if (!result.TryGetValue(fish, out var entry)) {
            result[fish] = new() {
                [loc] = seasons.ToList(),
            };
            return;
        }
        if (!entry.TryGetValue(loc, out var slist)) {
            entry[loc] = seasons.ToList();
            return;
        }
        foreach(int season in seasons) {
            if (!slist.Contains(season))
                slist.Add(season);
        }
    }

    private static void RemoveFish(SubLocation loc, int[] seasons, string fish, Dictionary<string, Dictionary<SubLocation, List<int>>> result) {
        if (!result.TryGetValue(fish, out var entry))
            return;

        if (!entry.TryGetValue(loc, out var slist))
            return;

        for (int i = slist.Count - 1; i >= 0; i--) {
            if (seasons.Contains(slist[i]))
                slist.RemoveAt(i);
        }

        if (slist.Count > 0)
            return;

        entry.Remove(loc);

        if (entry.Count > 0)
            return;

        result.Remove(fish);
    }

    public Dictionary<string, Dictionary<SubLocation, List<int>>> GetFishLocations() {

        var result = FishHelper.GetFishLocations();
        return result;
    }

    #endregion
}