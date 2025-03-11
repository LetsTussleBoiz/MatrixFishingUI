using StardewValley.ItemTypeDefinitions;

namespace MatrixFishingUI.Framework.Fish;

public class LocalFish (bool catchable, bool badSeason, bool badTime, bool badWeather, FishInfo fishInfo, ParsedItemData parsedFish)
{
    public bool Catchable { get; set; } = catchable;
    public bool BadSeason { get; set; } = badSeason;
    public bool BadTime { get; set; } = badTime;
    public bool BadWeather { get; set; } = badWeather;
    public FishInfo FishInfo { get; set; } = fishInfo;
    public ParsedItemData ParsedFish { get; set; } = parsedFish;
}