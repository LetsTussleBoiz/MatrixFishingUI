using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MatrixFishingUI;

public class ModConfig
{
    public static ModConfig Defaults { get; } = new();
    
    /// <summary>
    /// Determines key to open the Fishipedia.
    /// </summary>
    public KeybindList OpenMenuKey { get; set; } = new(SButton.G);

    /// <summary>
    /// Determines the Fish Helper HUD size in percentage, determined by GMCM.
    /// </summary>
    public string HudSize { get; set; } = "100%";

    /// <summary>
    /// Determines the number of columns for Fish in the HUD.
    /// </summary>
    public string HudColumns { get; set; } = "4";

    /// <summary>
    /// Toggle to hide any fish you've already collected in the area in the HUD.
    /// </summary>
    public bool HideCollected { get; set; } = false;
    /// <summary>
    /// Toggle to hide any fish you cannot catch today in the HUD (will only show time based fish).
    /// </summary>
    public bool OnlyCatchableToday { get; set; } = false;
    /// <summary>
    /// Toggle to hide any fish you cannot catch this season in the HUD (will only hide out of season fish).
    /// </summary>
    public bool OnlyCatchableSeason { get; set; } = false;
}