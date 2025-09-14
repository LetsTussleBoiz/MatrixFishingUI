using StardewValley;

namespace MatrixFishingUI.Framework.Fish;

public enum CaughtStatus {
    Uncaught,
    Caught
}

public record FishState
{
    public FishId Id { get; set; }
    public Item? Item { get; set; }
    
    public CaughtStatus GetCaughtStatus(Farmer player)
    {
        if (!player.fishCaught.TryGetValue(ItemRegistry.QualifyItemId(Id.Value), out var value)) return CaughtStatus.Uncaught;
        if (value.Length <= 0) return CaughtStatus.Uncaught;
        return value[0] > 0 ? CaughtStatus.Caught : CaughtStatus.Uncaught;
    }
    
    public int GetNumberCaught(Farmer player)
    {
        if (!player.fishCaught.TryGetValue(Id.Value, out var value)) return 0;
        if (value.Length <= 0) return 0;
        return value[0];
    }
    
    public int GetBiggestCatch(Farmer player)
    {
        if (!player.fishCaught.TryGetValue(Id.Value, out var value)) return 0;
        if (value.Length <= 0) return 0;
        return value[1];
    }
}