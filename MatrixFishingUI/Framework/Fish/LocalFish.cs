using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public class LocalFish (bool catchable, bool badSeason, bool badTime, bool badWeather, bool hasBeenCaught,FishInfo fishInfo, ParsedItemData parsedFish)
{
    public string Name { get; set; } = fishInfo.Name;
    public bool Catchable { get; set; } = catchable;
    public bool BadSeason { get; set; } = badSeason;
    public bool BadTime { get; set; } = badTime;
    public bool BadWeather { get; set; } = badWeather;
    public bool HasBeenCaught { get; set; } = hasBeenCaught;
    public FishInfo FishInfo { get; set; } = fishInfo;
    public ParsedItemData ParsedFish { get; set; } = parsedFish;
}