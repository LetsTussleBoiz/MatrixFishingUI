namespace MatrixFishingUI.Framework.Fish;

public class FishManager
{
    public readonly List<IFishProvider> _providers = new();
    private Dictionary<FishId,FishInfo> _fish = new();
    private Dictionary<FishId, FishState> _fishStates = new();
    private bool _loaded;
    
    public FishManager()
    {
        _providers.Add(new VanillaProvider());
    }

    public void ReadFromFile()
    {
        Dictionary<FishId, FishInfo> working = new();

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
                foreach (var info in fish)
                {
                    var id = new FishId(info.Id);
                    if (working.TryAdd(id, info)) {
                        provided++;
                    }
                }

            ModEntry.LogTrace($"Loaded {provided} fish via file from {provider.Name}");
            RefreshFish();
        }

        _fish = working;
        _loaded = true;
        ModEntry.LogDebug($"Loaded {_fish.Count} fish from {_providers.Count} providers.");
    }
    
    // TODO: Find alternative for refreshing fish without changing the entire dictionary (Like CatchFish mb?)
    public void RefreshFish() {
        Dictionary<FishId, FishState> working = new();
        
        foreach (var provider in _providers)
        {
            int provided = 0;
            IEnumerable<FishState>? fish;

            try
            {
                fish = provider.GetFishState();
            }
            catch (Exception e)
            {
                ModEntry.LogWarn($"An error occurred getting fish from provider {provider.Name}.");
                continue;
            }
            
            if (fish is not null)
                foreach (var info in fish)
                {
                    var id = info.Id;
                    if (working.TryAdd(id, info)) {
                        provided++;
                    }
                }

            ModEntry.LogTrace($"Loaded {provided} fish states from {provider.Name}");
        }
        
        _fishStates = working;
        _loaded = true;
        ModEntry.LogDebug($"Loaded {_fish.Count} fish states from {_providers.Count} providers.");
    }

    public FishInfo GetFish(FishId id)
    {
        return _fish[new FishId(id.Value)];
    }
    
    public FishState GetFishState(FishId id)
    {
        return _fishStates[new FishId(id.Value)];
    }

    public Dictionary<FishId, FishInfo> GetAllFish()
    {
        return _fish;
    }
    
    public Dictionary<FishId, FishState> GetAllFishStates()
    {
        return _fishStates;
    }
}