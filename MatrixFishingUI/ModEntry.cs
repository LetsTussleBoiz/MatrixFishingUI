using MatrixFishingUI.Framework.Fish;
using MatrixFishingUI.integrations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewUI.Framework;
using StardewValley.Tools;

namespace MatrixFishingUI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        private static ModConfig _config = null!;
        private static IMonitor? _monitor;
        public static IViewEngine? ViewEngine;
        internal static FishManager Fish = null!;
        private bool _canShowHud;
        private IViewDrawable? _hudWidget;
        
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _monitor = Monitor;
            Monitor.Log($"Started with menu key {_config.OpenMenuKey}.");
            Fish = new FishManager();
            I18n.Init(helper.Translation);
            
            // hook events
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderedHud += Display_RenderedHud;
            helper.Events.Input.ButtonsChanged += OnButtonChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Player.Warped += OnLocationChanged;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
        }
        
        private void Display_RenderedHud(object? sender, RenderedHudEventArgs e)
        {
            _hudWidget?.Draw(e.SpriteBatch, new Vector2(0, 100));
        }
        
        private void GenerateGMCM()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;
            
            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => Helper.WriteConfig(_config)
            );
            
            // Twitch Auth
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: I18n.Gmcm_Title,
                tooltip: I18n.Gmcm_Tooltip
            );
            configMenu.AddKeybindList(
                mod: ModManifest,
                name: I18n.Gmcm_Keybind,
                getValue: () => _config.OpenMenuKey,
                setValue: value =>
                {
                    _config.OpenMenuKey = value;
                    Log($"Fishipedia Menu Key set to: {value}");
                }
            );
            // configMenu.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "\n"
            // );

            Monitor.Log("GMCM Generated", LogLevel.Debug);
        }

        /// <summary>Raised after the player changes location (using any means).</summary>
        private void OnLocationChanged(object? sender, WarpedEventArgs e)
        {
            UpdateHud();
        }

        /// <summary>Raised when any item is added or removed from the player's inventory.</summary>
        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            var flag = false;
            foreach (var item in e.Added)
            {
                if (Fish.GetAllFish().ContainsKey(new FishId(item.ItemId)))
                {
                    flag = true;
                }
            }

            if (!flag) return;
            UpdateHud();
        }

        private void UpdateHud()
        {
            Fish.RefreshFish();
            if (!_canShowHud) return;
            ToggleHud();
            ToggleHud();
        }
        
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Fish.RefreshFish();
        }
        
        private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
        {
            // open menu
            if (!_config.OpenMenuKey.JustPressed()) return;
            if (!Context.IsPlayerFree || Game1.currentMinigame != null)
            {
                if (Game1.currentMinigame != null)
                    Monitor.Log($"Received menu open key, but a '{Game1.currentMinigame.GetType().Name}' minigame is active.");
                else if (Game1.eventUp)
                    Monitor.Log("Received menu open key, but an event is active.");
                else if (Game1.activeClickableMenu != null)
                    Monitor.Log($"Received menu open key, but a '{Game1.activeClickableMenu.GetType().Name}' menu is already open.");
                else
                    Monitor.Log("Received menu open key, but the player isn't free.");
            }
            else
            {
                Monitor.Log("Received menu open key.");
                var context = FishMenuData.GetFish();
                Game1.activeClickableMenu = ViewEngine?.CreateMenuFromAsset(
                    "Mods/Borealis.MatrixFishingUI/Views/ScrollingItemGrid",
                    context);
            }
        }
        
        private void ToggleHud()
        {
            if (_hudWidget is not null)
            {
                _hudWidget.Dispose();
                _hudWidget = null;
            }
            else
            {
                _hudWidget = ViewEngine?.CreateDrawableFromAsset("Mods/Borealis.MatrixFishingUI/Views/Hud");
                if (_hudWidget is null) return;
                var data = new HudMenuData();
                data.UpdateLocalFish(Fish.GetAllFish());
                _hudWidget.Context = data;
                // TODO: If code explodes this is why
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            var canShowHud = Game1.player.CurrentTool is FishingRod && Context.IsPlayerFree;
            if (canShowHud && !_canShowHud)
            {
                _canShowHud = true;
                ToggleHud();
            } else if (!canShowHud && _canShowHud)
            {
                _canShowHud = false;
                ToggleHud();
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
        }

        /// <summary>Raised after game is launched.</summary>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ViewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            ViewEngine?.RegisterViews("Mods/Borealis.MatrixFishingUI/Views", "assets/views");
            ViewEngine?.RegisterSprites("Mods/Borealis.MatrixFishingUI/Sprites", "assets/sprites");
#if DEBUG
            ViewEngine?.EnableHotReloading();
#endif
            GenerateGMCM();
        }

        public static void Log(string input)
        {
            _monitor?.Log(input, LogLevel.Info);
        }
        
        public static void LogError(string input)
        {
            _monitor?.Log(input, LogLevel.Error);
        }
        
        public static void LogDebug(string input)
        {
            _monitor?.Log(input, LogLevel.Debug);
        }
        
        public static void LogWarn(string input)
        {
            _monitor?.Log(input, LogLevel.Warn);
        }
        
        public static void LogTrace(string input)
        {
            _monitor?.Log(input);
        }
    }
}