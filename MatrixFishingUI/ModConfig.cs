using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MatrixFishingUI;

public class ModConfig
{
    public static ModConfig Defaults { get; } = new();
    
    public KeybindList OpenMenuKey { get; set; } = new(SButton.G);
    
    public int DefaultTab { get; set; } = 0;
}